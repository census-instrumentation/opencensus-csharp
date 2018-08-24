// <copyright file="MetricsExporterThread.cs" company="OpenCensus Authors">
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

namespace OpenCensus.Exporter.ApplicationInsights.Implementation
{
    using Microsoft.ApplicationInsights.Extensibility;
    using OpenCensus.Stats;
    using System;
    using System.Threading;

    internal class MetricsExporterThread
    {
        private readonly IViewManager viewManager;

        private readonly TelemetryConfiguration telemetryConfiguration;

        private readonly TimeSpan interval = TimeSpan.FromSeconds(10); // TODO: make configurable

        public MetricsExporterThread(TelemetryConfiguration telemetryConfiguration, IViewManager viewManager)
        {
            this.telemetryConfiguration = telemetryConfiguration;
            this.viewManager = viewManager;
        }

        public void WorkerThread()
        {
            try
            {
                // TODO: continuation token
                while (true)
                {
                    Thread.Sleep(this.interval);
                    this.Export();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
            }
        }

        internal void Export()
        {
            foreach (var view in this.viewManager.AllExportedViews)
            {
                var data = this.viewManager.GetView(view.Name);

                foreach(var value in data.AggregationMap)
                {
                    value.Value.Match<DistributionData>
                }
                Console.WriteLine(view);
                Console.WriteLine(data);
            }
        }
    }
}
