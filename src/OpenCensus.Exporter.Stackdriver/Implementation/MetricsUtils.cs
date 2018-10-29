// <copyright file="ApplicationInsightsExporter.cs" company="OpenCensus Authors">
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
    using Google.Apis.Auth.OAuth2;
    using Google.Cloud.Monitoring.V3;
    using OpenCensus.Exporter.Stackdriver.Utils;
    using OpenCensus.Stats;
    using OpenCensus.Stats.Aggregations;
    using OpenCensus.Tags;
    using System;
    using System.Collections.Generic;
    using static Google.Api.Distribution.Types;
    using static Google.Api.MetricDescriptor.Types;


    /// <summary>
    /// Utility methods for metrics
    /// </summary>
    public static class MetricsUtils
    {
        public static string GetProjectId()
        {
            var instance = Google.Api.Gax.Platform.Instance();
            var projectId = instance?.ProjectId;

            return projectId;
        }
    }

    /// <summary>
    /// Conversion methods from Opencensus Stats API to Stackdriver Metrics API
    /// </summary>
    public static class MetricsConversions
    {
        /// <summary>
        /// Converts between OpenCensus aggregation  and Stackdriver metric kind
        /// </summary>
        /// <param name="aggregation">Stats Aggregation</param>
        /// <returns>Stackdriver Metric Kind</returns>
        public static MetricKind ToMetricKind(
            this IAggregation aggregation)
        {
            if (aggregation is ILastValue)
            {
                return MetricKind.Gauge;
            }

            return aggregation.Match(
                v => MetricKind.Cumulative, // Sum
                v => MetricKind.Cumulative, // Count
                v => MetricKind.Cumulative, // Mean
                v => MetricKind.Cumulative, // Distribution
                v => MetricKind.Gauge,      // Last value
                v => MetricKind.Unspecified); // Default
        }

        public static MetricDescriptor.Types.ValueType ToValueType(
            this IAggregationData aggregationData)
        {
            return aggregationData.Match(
                v => MetricDescriptor.Types.ValueType.Double,        // Sum Double
                v => MetricDescriptor.Types.ValueType.Int64,         // Sum Long
                v => MetricDescriptor.Types.ValueType.Int64,         // Count
                v => MetricDescriptor.Types.ValueType.Unspecified,   // Mean - this measure should be deprecated
                v => MetricDescriptor.Types.ValueType.Distribution,  // Distribution
                v => MetricDescriptor.Types.ValueType.Double,        // LastValue Double
                v => MetricDescriptor.Types.ValueType.Int64,         // LastValue Long
                v => MetricDescriptor.Types.ValueType.Unspecified);  // Unrecognized
        }

        public static MetricDescriptor.Types.ValueType ToValueType(
            this IMeasure measure)
        {
            return measure.Match(
                v => MetricDescriptor.Types.ValueType.Double,        // Double
                v => MetricDescriptor.Types.ValueType.Int64,         // Long
                v => MetricDescriptor.Types.ValueType.Unspecified);  // Unrecognized
        }

        public static Google.Api.Distribution CreateDistribution(
            IDistributionData distributionData,
            IBucketBoundaries bucketBoundaries)
        {
            return new Distribution
            {
                //.setBucketOptions(createBucketOptions(bucketBoundaries))
                BucketCounts = { distributionData.BucketCounts },
                Count = distributionData.Count,
                Mean = distributionData.Mean,
                SumOfSquaredDeviation = distributionData.SumOfSquaredDeviations,
                Range = { Max = distributionData.Max, Min = distributionData.Min }
            };
        }

        // Construct a MetricDescriptor using a View.
        public static MetricDescriptor CreateMetricDescriptor(
            IView view, 
            ProjectName project, 
            string domain, 
            string displayNamePrefix)
        {
            var metricDescriptor = new MetricDescriptor();
            string viewName = view.Name.AsString;
            string type = GenerateTypeName(viewName, domain);

            // Name format refers to
            // cloud.google.com/monitoring/api/ref_v3/rest/v3/projects.metricDescriptors/create
            metricDescriptor.Name = string.Format($"projects/{project.ProjectId}/metricDescriptors/{type}");
            metricDescriptor.Type = type;
            metricDescriptor.Description = view.Description;
            metricDescriptor.DisplayName = GetDisplayName(viewName, displayNamePrefix);
            foreach (ITagKey tagKey in view.Columns)
            {
                var labelDescriptor = CreateLabelDescriptor(tagKey);
                metricDescriptor.Labels.Add(labelDescriptor);
            }
            metricDescriptor.Labels.Add(
                new LabelDescriptor
                {
                    Key = Constants.OPENCENSUS_TASK,
                    Description = Constants.OPENCENSUS_TASK_DESCRIPTION,
                    ValueType = LabelDescriptor.Types.ValueType.String,
                });

            var unit = CreateUnit(view.Aggregation, view.Measure);
            metricDescriptor.Unit = unit;
            metricDescriptor.MetricKind = view.Aggregation.ToMetricKind();
            metricDescriptor.ValueType = view.Measure.ToValueType();

            return metricDescriptor;
        }

        public static TypedValue CreateTypedValue(
            IAggregation aggregation,
            IAggregationData aggregationData)
        {
            return aggregationData.Match(
                v => new TypedValue { DoubleValue = v.Sum }, // Double
                v => new TypedValue { Int64Value = v.Sum }, // Long
                v => new TypedValue { Int64Value = v.Count }, // Count
                v => new TypedValue { DoubleValue = v.Count }, // Mean
                v => new TypedValue { DistributionValue = CreateDistribution(v, ((IDistribution)aggregation).BucketBoundaries) }, //Distribution
                v => new TypedValue { DoubleValue = v.LastValue }, // LastValue Double
                v => new TypedValue { Int64Value = v.LastValue }, // LastValue Long
                v => new TypedValue { BoolValue = false }); // Default
        }

        public static Point CreatePoint(
            IAggregationData aggregationData,
            IViewData windowData,
            IAggregation aggregation)
        {
            return new Point
            {
                Interval = CreateTimeInterval(windowData, aggregation),
                Value = CreateTypedValue(aggregation, aggregationData)
            };
        }

        private static BucketOptions CreateBucketOptions(IBucketBoundaries bucketBoundaries)
        {
            return new BucketOptions
            {
                ExplicitBuckets = new BucketOptions.Types.Explicit
                {
                    Bounds = { bucketBoundaries.Boundaries }
                }
            };
        }

        private static TimeInterval CreateTimeInterval(
            IViewData windowData,
            IAggregation aggregation)
        {
            return windowData.View.Measure.Match(
                v =>
                {
                    var interval = new TimeInterval { EndTime = windowData.End.ToTimestamp() };
                    if (!(aggregation is ILastValue))
                    {
                        interval.StartTime = windowData.Start.ToTimestamp();
                    }
                    return interval;
                },
                v => throw new InvalidOperationException(),
                v => throw new InvalidOperationException());
        }

        // Create a Metric using the TagKeys and TagValues.
        public static Metric CreateMetric(
            IView view, 
            IList<ITagValue> tagValues, 
            string domain)
        {
            var metric = new Metric();
            metric.Type = GenerateTypeName(view.Name.AsString, domain);

            
            IList<ITagKey> columns = view.Columns;

            // Populate metric labels
            for (int i = 0; i < tagValues.Count; i++)
            {
                ITagKey key = columns[i];
                ITagValue value = tagValues[i];
                if (value == null)
                {
                    continue;
                }
                metric.Labels.Add(key.Name, value.AsString);
            }
            metric.Labels.Add(Constants.OPENCENSUS_TASK, Constants.OPENCENSUS_TASK_VALUE_DEFAULT);

            return metric;
        }

        /// <summary>
        /// Convert ViewData to a list of TimeSeries, so that ViewData can be uploaded to Stackdriver.
        /// </summary>
        /// <param name="viewData">OpenCensus View</param>
        /// <param name="monitoredResource">Stackdriver Resource to which the metrics belong</param>
        /// <param name="domain">The metrics domain (namespace)</param>
        /// <returns></returns>
        public static List<TimeSeries> CreateTimeSeriesList(
            IViewData viewData,
            MonitoredResource monitoredResource,
            String domain)
        {
            var timeSeriesList = new List<TimeSeries>();
            if (viewData == null)
            {
                return timeSeriesList;
            }

            IView view = viewData.View;

            // Shared fields for all TimeSeries generated from the same ViewData
            var shared = new TimeSeries
            {
                MetricKind = viewData.View.Aggregation.ToMetricKind(),
                Resource = monitoredResource,
                ValueType = view.Measure.ToValueType()
            };

            // Each entry in AggregationMap will be converted into an independent TimeSeries object
            foreach (var entry in viewData.AggregationMap)
            {
                var timeSeries = shared.Clone();
                timeSeries.ValueType = entry.Value.ToValueType();

                var point = CreatePoint(entry.Value, viewData, view.Aggregation);
                timeSeries.Metric = CreateMetric(view, entry.Key.Values, domain);
                timeSeries.Points.Add(point);
                timeSeriesList.Add(timeSeries);
            }

            return timeSeriesList;
        }

        internal static LabelDescriptor CreateLabelDescriptor(ITagKey tagKey)
        {
            var labelDescriptor = new LabelDescriptor();
            labelDescriptor.Key = tagKey.Name;
            labelDescriptor.Description = Constants.LABEL_DESCRIPTION;
            
            // TODO - zeltser - Now we only support String tags
            labelDescriptor.ValueType = LabelDescriptor.Types.ValueType.String;
            return labelDescriptor;
        }

        internal static string CreateUnit(IAggregation aggregation, IMeasure measure)
        {
            if (aggregation is ICount) {
                return "1";
            }

            return measure.Unit;
        }

        /// <summary>
        /// Determining the resource to which the metrics belong
        /// </summary>
        /// <returns>Stackdriver Monitored Resource</returns>
        public static MonitoredResource GetDefaultResource()
        {
            var builder = new MonitoredResource();
            builder.Type = Constants.GLOBAL;
            builder.Labels.Add(Constants.PROJECT_ID_LABEL_KEY, MetricsUtils.GetProjectId());

            // TODO - zeltser - setting monitored resource labels for detected resource
            // along with all the other metadata

            return builder;
        }

        private static String GenerateTypeName(string viewName, string domain)
        {
            return domain + viewName;
        }

        private static String GetDisplayName(String viewName, String displayNamePrefix)
        {
            return displayNamePrefix + viewName;
        }

    }
}
