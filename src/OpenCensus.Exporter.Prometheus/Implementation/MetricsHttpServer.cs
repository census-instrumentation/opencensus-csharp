// <copyright file="MetricsHttpServer.cs" company="OpenCensus Authors">
// Copyright 2018, OpenCensus Authors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
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
    using System.IO;
    using System.Net;
    using System.Threading;
    using OpenCensus.Stats;

    internal class MetricsHttpServer
    {
        private readonly IViewManager viewManager;

        private readonly CancellationToken token;

        private readonly HttpListener httpListener = new HttpListener();

        public MetricsHttpServer(IViewManager viewManager, PrometheusExporterOptions options, CancellationToken token)
        {
            this.viewManager = viewManager;
            this.token = token;
            this.httpListener.Prefixes.Add(options.Url.ToString());
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

                    ctx.Response.StatusCode = 200;
                    ctx.Response.ContentType = PrometheusMetricBuilder.ContentType;

                    using (var output = ctx.Response.OutputStream)
                    {
                        MetricsWriter.WriteMetrics(output, this.viewManager);
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
