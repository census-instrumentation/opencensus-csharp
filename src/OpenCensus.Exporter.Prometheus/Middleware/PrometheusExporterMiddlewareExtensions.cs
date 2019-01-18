// <copyright file="PrometheusExporterMiddlewareExtensions.cs" company="OpenCensus Authors">
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

    /// <summary>
    /// Adds Prometheus Export capabilities
    /// </summary>
    public static class PrometheusExporterMiddlewareExtensions
    {
        /// <summary>
        /// Adds PrometheusExporterMiddleware to the given IApplicationBuilder
        /// </summary>
        public static IApplicationBuilder UsePrometheusExporter(
            this IApplicationBuilder builder, PrometheusMiddlewareOptions options, IViewManager viewManager)
        {
            return builder.UseMiddleware<PrometheusExporterMiddleware>(options, viewManager);
        }
    }
}