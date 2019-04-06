// <copyright file="StdoutTraceExporter.cs" company="OpenCensus Authors">
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

namespace OpenCensus.Exporter.Stdout
{
    using OpenCensus.Exporter.Stdout.Implementation;
    using OpenCensus.Trace.Export;

    /// <summary>
    /// Exporter of Open Census traces to Zipkin.
    /// </summary>
    public class StdoutTraceExporter
    {
        private const string ExporterName = "StdoutTraceExporter";
        private readonly StdoutTraceExporterOptions options;
        private readonly IExportComponent exportComponent;
        private readonly object lck = new object();
        private StdoutTraceExporterHandler handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="StdoutTraceExporter"/> class.
        /// This exporter sends Open Census traces to Zipkin.
        /// </summary>
        /// <param name="options">Zipkin exporter configuration options.</param>
        /// <param name="exportComponent">Exporter to get traces from.</param>
        /// For local development with invalid certificates use code like this:
        /// new HttpClient(new HttpClientHandler() { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator }).
        /// </param>
        public StdoutTraceExporter(StdoutTraceExporterOptions options, IExportComponent exportComponent)
        {
            this.options = options;
            this.exportComponent = exportComponent;
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

                this.handler = new StdoutTraceExporterHandler(this.options);
                this.exportComponent.SpanExporter.RegisterHandler(ExporterName, this.handler);
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

                this.exportComponent.SpanExporter.UnregisterHandler(ExporterName);
                this.handler = null;
            }
        }
    }
}
