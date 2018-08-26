﻿// <copyright file="ApplicationInsightsExporter.cs" company="OpenCensus Authors">
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

namespace OpenCensus.Exporter.ApplicationInsights
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights.Extensibility;
    using OpenCensus.Exporter.ApplicationInsights.Implementation;
    using OpenCensus.Stats;
    using OpenCensus.Trace.Export;

    /// <summary>
    /// Exporter of Open Census traces and metrics to Azure Application Insights.
    /// </summary>
    public class ApplicationInsightsExporter
    {
        private const string TraceExporterName = "ApplicationInsightsTraceExporter";

        private const string MetricsExporterName = "ApplicationInsightsMetricsExporter";

        private readonly TelemetryConfiguration telemetryConfiguration;

        private readonly IViewManager viewManager;

        private readonly IExportComponent exportComponent;

        private readonly object lck = new object();

        private TraceExporterHandler handler;

        /// <summary>
        /// Instantiates a new instance of an exporter from Open Census to Azure Application Insights.
        /// </summary>
        /// <param name="telemetryConfiguration">Telemetry configuration to use to report telemetry.</param>
        /// <param name="exportComponent">Exporter to get traces and metrics from.</param>
        /// <param name="viewManager">View manager to get stats from.</param>
        public ApplicationInsightsExporter(TelemetryConfiguration telemetryConfiguration, IExportComponent exportComponent, IViewManager viewManager)
        {
            this.exportComponent = exportComponent;
            this.viewManager = viewManager;
            this.telemetryConfiguration = telemetryConfiguration;
        }

        /// <summary>
        /// Start exporter.
        /// </summary>
        public void Start()
        {
            lock (this.lck)
            {
                if (this.handler != null)
                {
                    return;
                }

                this.handler = new TraceExporterHandler(this.telemetryConfiguration);

                this.exportComponent.SpanExporter.RegisterHandler(TraceExporterName, this.handler);

                var metricsExporter = new MetricsExporterThread(this.telemetryConfiguration, this.viewManager);
                Task.Factory.StartNew((Action)metricsExporter.WorkerThread, TaskCreationOptions.LongRunning);
            }
        }

        /// <summary>
        /// Stop exporter.
        /// </summary>
        public void Stop()
        {
            lock (this.lck)
            {
                if (this.handler == null)
                {
                    return;
                }

                this.exportComponent.SpanExporter.UnregisterHandler(TraceExporterName);

                this.handler = null;
            }
        }
    }
}
