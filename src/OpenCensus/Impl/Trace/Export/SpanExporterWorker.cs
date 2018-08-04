// <copyright file="SpanExporterWorker.cs" company="OpenCensus Authors">
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
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using OpenCensus.Common;

    internal class SpanExporterWorker : IDisposable
    {
        private readonly int bufferSize;
        private TimeSpan scheduleDelay;
        private bool shutdown = false;
        private BlockingCollection<ISpan> spans;
        private ConcurrentDictionary<string, IHandler> serviceHandlers = new ConcurrentDictionary<string, IHandler>();

        public SpanExporterWorker(int bufferSize, IDuration scheduleDelay)
        {
            this.bufferSize = bufferSize;
            this.scheduleDelay = TimeSpan.FromSeconds(scheduleDelay.Seconds);
            this.spans = new BlockingCollection<ISpan>();
        }

        public void Dispose()
        {
            this.shutdown = true;
            this.spans.CompleteAdding();
        }

        internal void AddSpan(ISpan span)
        {
            if (!this.spans.IsAddingCompleted)
            {
                if (!this.spans.TryAdd(span))
                {
                    // Log failure, dropped span
                }
            }
        }

        internal void Run(object obj)
        {
            List<ISpanData> toExport = new List<ISpanData>();
            while (!this.shutdown)
            {
                try
                {
                    if (this.spans.TryTake(out ISpan item, this.scheduleDelay))
                    {
                        // Build up list
                        this.BuildList(item, toExport);

                        // Export them
                        this.Export(toExport);

                        // Get ready for next batch
                        toExport.Clear();
                    }

                    if (this.spans.IsCompleted)
                    {
                        break;
                    }
                }
                catch (Exception)
                {
                    // Log
                    return;
                }
            }
        }

        private void BuildList(ISpan item, List<ISpanData> toExport)
        {
            Span span = item as Span;
            if (span != null)
            {
                toExport.Add(span.ToSpanData());
            }

            // Grab as many as we can
            while (this.spans.TryTake(out item))
            {
                span = item as Span;
                if (span != null)
                {
                    toExport.Add(span.ToSpanData());
                }

                if (toExport.Count >= this.bufferSize)
                {
                    break;
                }
            }
        }

        private void Export(IList<ISpanData> export)
        {
            var handlers = this.serviceHandlers.Values;
            foreach (var handler in handlers)
            {
                try
                {
                    handler.Export(export);
                }
                catch (Exception)
                {
                    // Log warning
                }
            }
        }

        internal void RegisterHandler(string name, IHandler handler)
        {
            this.serviceHandlers[name] = handler;
        }

        internal void UnregisterHandler(string name)
        {
            this.serviceHandlers.TryRemove(name, out IHandler prev);
        }

        internal ISpanData ToSpanData(ISpan span)
        {
            Span spanImpl = span as Span;
            if (spanImpl == null)
            {
                throw new InvalidOperationException("ISpan not a Span");
            }

            return spanImpl.ToSpanData();
        }
    }
}
