// <copyright file="MetricsHttpServer.cs" company="OpenCensus Authors">
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

namespace OpenCensus.Exporter.Prometheus.Implementation
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Threading;
    using OpenCensus.Stats;
    using OpenCensus.Stats.Aggregations;

    internal class MetricsHttpServer
    {
        private readonly IViewManager viewManager;

        private readonly CancellationToken token;

        private readonly HttpListener httpListener = new HttpListener();

        public MetricsHttpServer(IViewManager viewManager, CancellationToken token)
        {
            this.viewManager = viewManager;
            this.token = token;
            this.httpListener.Prefixes.Add($"http://localhost:9184/metrics/");
        }

        public void WorkerThread()
        {
            this.httpListener.Start();

            try
            {
                while (!this.token.IsCancellationRequested)
                {
                    var ctxTask = this.httpListener.GetContextAsync();
                    ctxTask.Wait(this.token);

                    var ctx = ctxTask.Result;

                    ctx.Response.ContentType = "text/plain; version = 0.0.4";
                    ctx.Response.StatusCode = 200;

                    using (var output = ctx.Response.OutputStream)
                    {
                        using (var writer = new StreamWriter(output))
                        {
                            // https://prometheus.io/docs/instrumenting/exposition_formats/

                            foreach (var view in this.viewManager.AllExportedViews)
                            {
                                var data = this.viewManager.GetView(view.Name);

                                foreach (var value in data.AggregationMap)
                                {
                                    PrometheusMetricBuilder builder = new PrometheusMetricBuilder();
                                    builder.WithName(data.View.Name.AsString);
                                    builder.WithDescription(data.View.Description);

                                    for (int i = 0; i < value.Key.Values.Count; i++)
                                    {
                                        var name = data.View.Columns[i].Name;
                                        var val = value.Key.Values[i].AsString;

                                        value.Value.Match<object>(
                                            (combined) =>
                                            {
                                                if (combined is ISumDataDouble sum)
                                                {
                                                    //metricTelemetry.Sum = sum.Sum;
                                                }
                                                return null;
                                            },
                                            (combined) =>
                                            {
                                                if (combined is ISumDataLong sum)
                                                {
                                                    //metricTelemetry.Sum = sum.Sum;
                                                }
                                                return null;
                                            },
                                            (combined) =>
                                            {
                                                if (combined is ICountData count)
                                                {
                                                    //metricTelemetry.Sum = count.Count;
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
                                                    //metricTelemetry.Sum = dist.Mean;
                                                    //metricTelemetry.Min = dist.Min;
                                                    //metricTelemetry.Max = dist.Max;
                                                    //metricTelemetry.StandardDeviation = dist.SumOfSquaredDeviations;
                                                }

                                                return null;
                                            },
                                            (combined) =>
                                            {
                                                if (combined is ILastValueDataDouble lastValue)
                                                {
                                                    //metricTelemetry.Sum = lastValue.LastValue;
                                                }
                                                return null;
                                            },
                                            (combined) =>
                                            {
                                                if (combined is ILastValueDataLong lastValue)
                                                {
                                                    //metricTelemetry.Sum = lastValue.LastValue;
                                                }
                                                return null;
                                            },
                                            (combined) =>
                                            {
                                                if (combined is IAggregationData aggregationData)
                                                {
                                                    // TODO: report an error
                                                }
                                                return null;
                                            });
                                    }

                                    builder.Write(writer);
                                }
                            }
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // this will happen when cancellation will be requested
            }
            catch (Exception)
            {
                // TODO: report error
            }
            finally
            {
                this.httpListener.Stop();
                this.httpListener.Close();
            }
        }
    }
}
