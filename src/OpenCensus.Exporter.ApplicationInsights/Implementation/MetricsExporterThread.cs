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
    using System;
    using System.Threading;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.ApplicationInsights.Extensibility;
    using OpenCensus.Stats;
    using OpenCensus.Stats.Aggregations;

    internal class MetricsExporterThread
    {
        private readonly IViewManager viewManager;

        private readonly TelemetryClient telemetryClient;

        private readonly TimeSpan interval = TimeSpan.FromSeconds(10); // TODO: make configurable

        public MetricsExporterThread(TelemetryConfiguration telemetryConfiguration, IViewManager viewManager)
        {
            this.telemetryClient = new TelemetryClient(telemetryConfiguration);
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

                foreach (var value in data.AggregationMap)
                {
                    var metricTelemetry = new MetricTelemetry
                    {
                        Name = data.View.Name.AsString
                    };

                    for (int i = 0; i < value.Key.Values.Count; i++)
                    {
                        var name = data.View.Columns[i].Name;
                        var val = value.Key.Values[i].AsString;
                        metricTelemetry.Properties.Add(name, val);
                    }

                    value.Value.Match<object>(
                        (combined) =>
                        {
                            if (combined is ISumDataDouble sum)
                            {
                            }
                            return null;
                        },
                        (combined) =>
                        {
                            if (combined is ISumDataLong sum)
                            {
                            }
                            return null;
                        },
                        (combined) =>
                        {
                            if (combined is ICountData count)
                            {
                            }
                            return null;
                        },
                        (combined) =>
                        {
                            if (combined is IMeanData mean)
                            {
                            }
                            return null;
                        },
                        (combined) =>
                        {
                            if (combined is IDistributionData dist)
                            {
#pragma warning disable CS0618 // Type or member is obsolete
                                metricTelemetry.Value = dist.Mean;
#pragma warning restore CS0618 // Type or member is obsolete
                                metricTelemetry.Min = dist.Min;
                                metricTelemetry.Max = dist.Max;
                            }

                            return null;
                        },
                        (combined) =>
                        {
                            if (combined is ILastValueDataDouble lastValue)
                            {
                            }
                            return null;
                        },
                        (combined) =>
                        {
                            if (combined is ILastValueDataLong lastValue)
                            {
                            }
                            return null;
                        },
                        (combined) =>
                        {
                            if (combined is IAggregationData aggregationData)
                            {
                            }
                            return null;
                        });
                    this.telemetryClient.TrackMetric(metricTelemetry);
                }

                Console.WriteLine(view);
                Console.WriteLine(data);
            }
        }
    }
}
