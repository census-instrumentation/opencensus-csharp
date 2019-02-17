// <copyright file="ZpagesExporter.cs" company="OpenCensus Authors">
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

namespace OpenCensus.Exporter.Zpages
{
    using OpenCensus.Exporter.Zpages.Implementation;
    using OpenCensus.Trace.Export;

    /// <summary>
    /// Exporter of Open Census traces to Zpages.
    /// </summary>
    public class ZpagesExporter
    {
        private const string ExporterName = "ZpagesTraceExporter";

        private readonly ZpagesExporterOptions options;

        private readonly IExportComponent exportComponent;

        private readonly object lck = new object();

        private TraceExporterHandler handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZpagesExporter"/> class.
        /// This exporter sends Open Census traces to Zpages.
        /// </summary>
        /// <param name="options">Zpages exporter configuration options.</param>
        /// <param name="exportComponent">Exporter to get traces from.</param>
        public ZpagesExporter(ZpagesExporterOptions options, IExportComponent exportComponent)
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

                this.handler = new TraceExporterHandler(this.options);

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
