// <copyright file="StdoutTraceExporterHandler.cs" company="OpenCensus Authors">
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

namespace OpenCensus.Exporter.Stdout.Implementation
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using OpenCensus.Common;
    using OpenCensus.Trace;
    using OpenCensus.Trace.Export;

    internal class StdoutTraceExporterHandler : IHandler
    {
        private readonly StdoutTraceExporterOptions options;

        public StdoutTraceExporterHandler(StdoutTraceExporterOptions options)
        {
            this.options = options;
        }

        public async Task ExportAsync(IEnumerable<ISpanData> spanDataList)
        {
            string savedLineTermination = Console.Out.NewLine;
            Console.Out.NewLine = this.options.LineTermination;

            var settings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All };
            Console.WriteLine("[ ");
            foreach (var data in spanDataList)
            {
                string serialized = JsonConvert.SerializeObject(data, Formatting.Indented, settings);
                Console.WriteLine(serialized);
                Console.WriteLine(" , ");
            }

            Console.WriteLine("]");
            Console.Out.NewLine = savedLineTermination;
        }
    }
}
