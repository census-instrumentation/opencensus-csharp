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
            spans = new BlockingCollection<ISpan>();
        }

        public void Dispose()
        {
            shutdown = true;
            spans.CompleteAdding();
        }

        internal void AddSpan(ISpan span)
        {
            if (!spans.IsAddingCompleted)
            {
                if (!spans.TryAdd(span))
                {
                    // Log failure, dropped span
                }
            }
        }

        internal void Run(object obj)
        {
            List<ISpanData> toExport = new List<ISpanData>();
            while (!shutdown)
            {
                try
                {
                    if (spans.TryTake(out ISpan item, scheduleDelay))
                    {
                        // Build up list
                        BuildList(item, toExport);

                        // Export them
                        Export(toExport);

                        // Get ready for next batch
                        toExport.Clear();
                    }

                    if (spans.IsCompleted)
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
            while (spans.TryTake(out item))
            {
                span = item as Span;
                if (span != null)
                {
                    toExport.Add(span.ToSpanData());
                }

                if (toExport.Count >= bufferSize)
                {
                    break;
                }
            }
        }

        private void Export(IList<ISpanData> export)
        {
            var handlers = serviceHandlers.Values;
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
            serviceHandlers[name] = handler;
        }

        internal void UnregisterHandler(string name)
        {
            serviceHandlers.TryRemove(name, out IHandler prev);
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
