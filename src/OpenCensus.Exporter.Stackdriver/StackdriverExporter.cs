
// <copyright file="StackdriverExporter.cs" company="OpenCensus Authors">
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

namespace OpenCensus.Exporter.Stackdriver
{
    using OpenCensus.Exporter.Stackdriver.Implementation;
    using OpenCensus.Trace.Export;
    using System;
    using System.Threading;

    public class StackdriverExporter
    {
        private const string ExporterName = "StackdriverTraceExporter";

        private readonly IExportComponent exportComponent;
        private readonly string projectId;
        private object locker = new object();
        private bool isInitialized = false;

        public StackdriverExporter(string projectId, IExportComponent exportComponent)
        {
            this.projectId = projectId;
            this.exportComponent = exportComponent;
        }

        public void Start()
        {
            lock (locker)
            {
                if (isInitialized)
                {
                    return;
                }

                var traceExporter = new StackdriverTraceExporter(projectId);
                exportComponent.SpanExporter.RegisterHandler(ExporterName, traceExporter);

                isInitialized = true;
            }
        }

        public void Stop()
        {
            lock (locker)
            {
                if (!isInitialized)
                {
                    return;
                }

                exportComponent.SpanExporter.UnregisterHandler(ExporterName);
                isInitialized = false;
            }
        }
    }
}
