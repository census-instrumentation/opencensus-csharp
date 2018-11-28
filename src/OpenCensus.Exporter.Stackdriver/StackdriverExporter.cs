
// <copyright file="StackdriverExporter.cs" company="OpenCensus Authors">
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

namespace OpenCensus.Exporter.Stackdriver
{
    using OpenCensus.Exporter.Stackdriver.Implementation;
    using OpenCensus.Stats;
    using OpenCensus.Trace.Export;

    /// <summary>
    /// Implementation of the exporter to Stackdriver
    /// </summary>
    public class StackdriverExporter
    {
        private const string ExporterName = "StackdriverTraceExporter";

        private readonly IExportComponent exportComponent;
        private readonly IViewManager viewManager;
        private readonly string projectId;
<<<<<<< HEAD
<<<<<<< HEAD
        private StackdriverStatsExporter statsExporter;
=======
        private StackdriverStatsExporter metricsExporter;
>>>>>>> First working version of Stackdriver Stats Exporter.
=======
        private StackdriverStatsExporter statsExporter;
>>>>>>> Fixed Stackdriver export interval propagation
        private object locker = new object();
        private bool isInitialized = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="StackdriverExporter"/> class.
        /// </summary>
        /// <param name="projectId">Google Cloud ProjectId that is used to send data to Stackdriver</param>
        /// <param name="exportComponent">Exporter to get traces from</param>
        /// <param name="viewManager">View manager to get the stats from</param>
        public StackdriverExporter(
            string projectId,
            IExportComponent exportComponent,
            IViewManager viewManager)
        {
            this.projectId = projectId;
            this.exportComponent = exportComponent;
            this.viewManager = viewManager;
        }

        /// <summary>
        /// Starts the exporter
        /// </summary>
        public void Start()
        {
            lock (locker)
            {
                if (isInitialized)
                {
                    return;
                }

                // Register trace exporter
                if (exportComponent != null)
                {
                    var traceExporter = new StackdriverTraceExporter(projectId);
                    exportComponent.SpanExporter.RegisterHandler(ExporterName, traceExporter);
                }

                // Register stats(metrics) exporter
                if (viewManager != null)
                {
                    StackdriverStatsConfiguration statsConfig = StackdriverStatsConfiguration.Default;
<<<<<<< HEAD
<<<<<<< HEAD
=======
>>>>>>> Fixed Stackdriver export interval propagation
                    if (statsConfig.ProjectId != projectId)
                    {
                        statsConfig.ProjectId = projectId;
                        statsConfig.MonitoredResource = GoogleCloudResourceUtils.GetDefaultResource(projectId);
                    }
<<<<<<< HEAD
=======
                    statsConfig.ProjectId = projectId;
<<<<<<< HEAD
<<<<<<< HEAD
>>>>>>> Fixing a few bugs in Stackdriver metrics exporter
=======
                    statsConfig.MonitoredResource = MetricsUtils.GetDefaultResource(projectId);
>>>>>>> - Fixing a few bugs around metric creation and labels

                    statsExporter = new StackdriverStatsExporter(viewManager, statsConfig);
                    statsExporter.Start();
=======
                    statsConfig.MonitoredResource = GoogleCloudResourceUtils.GetDefaultResource(projectId);

                    metricsExporter = new StackdriverStatsExporter(viewManager, statsConfig);
                    metricsExporter.Start();
>>>>>>> First working version of Stackdriver Stats Exporter.
=======

                    statsExporter = new StackdriverStatsExporter(viewManager, statsConfig);
                    statsExporter.Start();
>>>>>>> Fixed Stackdriver export interval propagation
                }

                isInitialized = true;
            }
        }

        /// <summary>
        /// Stops the exporter
        /// </summary>
        public void Stop()
        {
            lock (locker)
            {
                if (!isInitialized)
                {
                    return;
                }

                // Stop tracing exporter
                if (exportComponent != null)
                {
                    exportComponent.SpanExporter.UnregisterHandler(ExporterName);
                }

                // Stop metrics exporter
                if (statsExporter != null)
                {
                    statsExporter.Stop();
                }

                isInitialized = false;
            }
        }
    }
}
