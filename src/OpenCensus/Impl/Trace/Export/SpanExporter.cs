// <copyright file="SpanExporter.cs" company="OpenCensus Authors">
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

namespace OpenCensus.Trace.Export
{
    using System.Threading;
    using OpenCensus.Common;

    public sealed class SpanExporter : SpanExporterBase
    {
        private SpanExporterWorker _worker { get; }

        private readonly Thread workerThread;

        internal SpanExporter(SpanExporterWorker worker)
        {
            _worker = worker;
            workerThread = new Thread(worker.Run)
            {
                IsBackground = true,
                Name = "SpanExporter"
            };
            workerThread.Start();
        }

        internal static ISpanExporter Create(int bufferSize, IDuration scheduleDelay)
        {
            SpanExporterWorker worker = new SpanExporterWorker(bufferSize, scheduleDelay);
            return new SpanExporter(worker);
        }

        public override void AddSpan(ISpan span)
        {
            _worker.AddSpan(span);
        }

        public override void RegisterHandler(string name, IHandler handler)
        {
            _worker.RegisterHandler(name, handler);
        }

        public override void UnregisterHandler(string name)
        {
            _worker.UnregisterHandler(name);
        }

        public override void Dispose()
        {
            _worker.Dispose();
        }

        internal Thread ServiceExporterThread
        {
            get
            {
                return workerThread;
            }
        }
    }
}
