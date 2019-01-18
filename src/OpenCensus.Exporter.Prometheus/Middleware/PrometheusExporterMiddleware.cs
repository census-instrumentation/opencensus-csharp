// <copyright file="PrometheusExporterMiddleware.cs" company="OpenCensus Authors">
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

namespace OpenCensus.Exporter.Prometheus.Middleware
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using OpenCensus.Exporter.Prometheus.Implementation;
    using OpenCensus.Stats;

    public class PrometheusExporterMiddleware
    {
        private readonly RequestDelegate next;

        private readonly IViewManager viewManager;

        private readonly PrometheusMiddlewareOptions options;

        public PrometheusExporterMiddleware(RequestDelegate next, PrometheusMiddlewareOptions options, IViewManager viewManager)
        {
            this.options = options;
            this.viewManager = viewManager;
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext ctx)
        {
            // Check the request for the given url path
            if (ctx.Request.Path.Equals(this.options.Path))
            {
                ctx.Response.StatusCode = 200;
                ctx.Response.ContentType = PrometheusMetricBuilder.ContentType;
                using (var output = ctx.Response.Body)
                {
                    MetricsWriter.WriteMetrics(output, this.viewManager);
                }

                // Handle the request. Do not call next
                return;
            }

            // Call the next delegate/middleware in the pipeline
            await this.next(ctx);
        }
    }
}