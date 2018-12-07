﻿// <copyright file="ICountData.cs" company="OpenCensus Authors">
// Copyright 2018, OpenCensus Authors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of theLicense at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

namespace OpenCensus.Exporter.Stackdriver.Implementation
{
    using Google.Api;
    using Google.Api.Gax;
    using Google.Api.Gax.Grpc;
    using Google.Cloud.Monitoring.V3;
    using Grpc.Core;
    using OpenCensus.Exporter.Stackdriver.Utils;
    using OpenCensus.Stats;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    internal class StackdriverStatsExporter
    {
        private readonly MonitoredResource monitoredResource;
        private readonly IViewManager viewManager;
        private readonly Dictionary<IViewName, IView> registeredViews = new Dictionary<IViewName, IView>();

        private readonly Dictionary<IView, MetricDescriptor> metricDescriptors = new Dictionary<IView, MetricDescriptor>(new ViewNameComparer());
        private readonly ProjectName project;
        private MetricServiceClient metricServiceClient;
        private CancellationTokenSource tokenSource;

        private const int MAX_BATCH_EXPORT_SIZE = 200;
        private const string DEFAULT_DISPLAY_NAME_PREFIX = "OpenCensus/";
        private const string CUSTOM_METRIC_DOMAIN = "custom.googleapis.com/";
        private const string CUSTOM_OPENCENSUS_DOMAIN = CUSTOM_METRIC_DOMAIN + "opencensus/";

        // TODO - zeltser - figure out how to extract OpenCensus package version
        private const string USER_AGENT = "opencensus-csharp [0.0]";
        private const string USER_AGENT_KEY = "user-agent";

        private readonly string domain;
        private readonly string displayNamePrefix;

        private bool isStarted;

        /// <summary>
        /// Interval between two subsequent stats collection operations
        /// </summary>
        private TimeSpan collectionInterval = TimeSpan.FromMinutes(1);

        /// <summary>
        /// Interval within which the cancellation should be honored
        /// from the point it was requested
        /// </summary>
        private readonly TimeSpan cancellationInterval = TimeSpan.FromSeconds(3);

        private object locker = new object();

        public StackdriverStatsExporter(
           IViewManager viewManager,
           StackdriverStatsConfiguration configuration)
        {
            GaxPreconditions.CheckNotNull(configuration, "configuration");
            GaxPreconditions.CheckNotNullOrEmpty(configuration.ProjectId, "configuration.ProjectId");
            GaxPreconditions.CheckNotNull(configuration.MonitoredResource, "configuration.MonitoredResource");
            GaxPreconditions.CheckArgument(
                configuration.ExportInterval != TimeSpan.Zero,
                paramName: "configuration.ExportInterval", 
                message: "Export interval can't be zero. Typically it's 1 minute");

            this.viewManager = viewManager;
            monitoredResource = configuration.MonitoredResource;
            collectionInterval = configuration.ExportInterval;
            project = new ProjectName(configuration.ProjectId);

            domain = GetDomain(configuration.MetricNamePrefix);
            displayNamePrefix = GetDisplayNamePrefix(configuration.MetricNamePrefix);
        }

        public void Start()
        {
            lock (locker)
            {
                if (!isStarted)
                {
                    tokenSource = new CancellationTokenSource();
                    metricServiceClient = CreateMetricServiceClient(tokenSource);

                    Task.Factory.StartNew(DoWork, tokenSource.Token);

                    isStarted = true;
                }
            }
        }

        public void Stop()
        {
            lock (locker)
            {
                if (!isStarted)
                {
                    return;
                }

                tokenSource.Cancel();
            }
        }

        /// <summary>
        /// Periodic operation happening on a dedicated thread that is 
        /// capturing the metrics collected within a collection interval
        /// </summary>
        private void DoWork()
        {
            try
            {
                TimeSpan sleepTime = collectionInterval;
                var stopWatch = new Stopwatch();

                while (!tokenSource.IsCancellationRequested)
                {
                    // Calculate the duration of collection iteration
                    stopWatch.Start();

                    // Collect metrics
                    Export();

                    stopWatch.Stop();

                    // Adjust the wait time - reduce export operation duration
                    sleepTime = collectionInterval.Subtract(stopWatch.Elapsed);
                    sleepTime = sleepTime.Duration();

                    // If the cancellation was requested, we should honor
                    // that within the cancellation interval, so we wait in 
                    // intervals of <cancellationInterval>
                    while (sleepTime > cancellationInterval && !tokenSource.IsCancellationRequested)
                    {
                        Thread.Sleep(cancellationInterval);
                        sleepTime = sleepTime.Subtract(cancellationInterval);
                    }

                    Thread.Sleep(sleepTime);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private bool RegisterView(IView view)
        {
            IView existing = null;
            if (registeredViews.TryGetValue(view.Name, out existing))
            {
                // Ignore views that are already registered.
                return existing.Equals(view);
            }
            registeredViews.Add(view.Name, view);

            string metricDescriptorTypeName = GenerateMetricDescriptorTypeName(view.Name, domain);

            // TODO - zeltser: don't need to create MetricDescriptor for RpcViewConstants once we defined
            // canonical metrics. Registration is required only for custom view definitions. Canonical
            // views should be pre-registered.
            MetricDescriptor metricDescriptor = MetricsConversions.CreateMetricDescriptor(
                metricDescriptorTypeName,
                view,
                project,
                domain,
                displayNamePrefix);

            if (metricDescriptor == null)
            {
                // Don't register interval views in this version.
                return false;
            }

            // Cache metric descriptor and ensure it exists in Stackdriver
            if (!metricDescriptors.ContainsKey(view))
            {
                metricDescriptors.Add(view, metricDescriptor);
            }
            return EnsureMetricDescriptorExists(metricDescriptor);
        }

        private bool EnsureMetricDescriptorExists(MetricDescriptor metricDescriptor)
        {
            try
            {
                var request = new CreateMetricDescriptorRequest();
                request.ProjectName = project;
                request.MetricDescriptor = metricDescriptor;
                metricServiceClient.CreateMetricDescriptor(request);
            }
            catch (RpcException e)
            {
                // Metric already exists
                if (e.StatusCode == StatusCode.AlreadyExists)
                {
                    return true;
                }

                return false;
            }

            return true;
        }

        private void Export()
        {
            var viewDataList = new List<IViewData>();
            foreach (var view in viewManager.AllExportedViews)
            {
                if (RegisterView(view))
                {
                    var data = viewManager.GetView(view.Name);
                    viewDataList.Add(data);
                }
            }

            // Add time series from all the views that need exporting
            var timeSeriesList = new List<TimeSeries>();
            foreach (var viewData in viewDataList)
            {
                MetricDescriptor metricDescriptor = metricDescriptors[viewData.View];
                List<TimeSeries> timeSeries = MetricsConversions.CreateTimeSeriesList(viewData, monitoredResource, metricDescriptor, domain);
                timeSeriesList.AddRange(timeSeries);
            }

            // Perform the operation in batches of MAX_BATCH_EXPORT_SIZE
            foreach (IEnumerable<TimeSeries> batchedTimeSeries in timeSeriesList.Partition(MAX_BATCH_EXPORT_SIZE))
            {
                var request = new CreateTimeSeriesRequest();
                request.ProjectName = project;
                request.TimeSeries.AddRange(batchedTimeSeries);

                try
                {
                    metricServiceClient.CreateTimeSeries(request);
                }
                catch (RpcException e)
                {
                    // TODO - zeltser - figure out where to send the error from exception
                    Console.WriteLine(e);
                }
            }
        }

        private static MetricServiceClient CreateMetricServiceClient(CancellationTokenSource tokenSource)
        {
            // Make sure to add Opencensus header to every outgoing call to Stackdriver APIs
            Action<Metadata> addOpencensusHeader = m => m.Add(USER_AGENT_KEY, USER_AGENT);
            var callSettings = new CallSettings(
                cancellationToken: tokenSource.Token,
                credentials: null,
                timing: null,
                headerMutation: addOpencensusHeader,
                writeOptions: WriteOptions.Default,
                propagationToken: null);
            var metricServiceSettings = new MetricServiceSettings()
            {
                CallSettings = callSettings
            };

            return MetricServiceClient.Create(settings: metricServiceSettings);
        }

        private static string GetDomain(string metricNamePrefix)
        {
            string domain;
            if (string.IsNullOrEmpty(metricNamePrefix))
            {
                domain = CUSTOM_OPENCENSUS_DOMAIN;
            }
            else
            {
                if (!metricNamePrefix.EndsWith("/"))
                {
                    domain = metricNamePrefix + '/';
                }
                else
                {
                    domain = metricNamePrefix;
                }
            }
            return domain;
        }

        private string GetDisplayNamePrefix(string metricNamePrefix)
        {
            if (metricNamePrefix == null)
            {
                return DEFAULT_DISPLAY_NAME_PREFIX;
            }
            else
            {
                if (!metricNamePrefix.EndsWith("/") && !string.IsNullOrEmpty(metricNamePrefix))
                {
                    metricNamePrefix += '/';
                }
                return metricNamePrefix;
            }
        }

        private static string GenerateMetricDescriptorTypeName(IViewName viewName, string domain)
        {
            return domain + viewName.AsString;
        }

        /// <summary>
        /// Comparison between two OpenCensus Views
        /// </summary>
        private class ViewNameComparer : IEqualityComparer<IView>
        {
            public bool Equals(IView x, IView y)
            {
                if (x == null && y == null)
                    return true;
                else if (x == null || y == null)
                    return false;
                else if (x.Name.AsString.Equals(y.Name.AsString))
                    return true;
                else
                    return false;
            }

            public int GetHashCode(IView obj)
            {
                return obj.Name.GetHashCode();
            }
        }
    }
}
