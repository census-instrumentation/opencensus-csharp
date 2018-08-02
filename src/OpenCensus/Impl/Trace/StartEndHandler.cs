namespace OpenCensus.Trace
{
    using OpenCensus.Internal;
    using OpenCensus.Trace.Export;

    public sealed class StartEndHandler : IStartEndHandler
    {
        private readonly ISpanExporter spanExporter;
        private readonly IRunningSpanStore runningSpanStore;
        private readonly ISampledSpanStore sampledSpanStore;
        private readonly IEventQueue eventQueue;
        // true if any of (runningSpanStore OR sampledSpanStore) are different than null, which
        // means the spans with RECORD_EVENTS should be enqueued in the queue.
        private readonly bool enqueueEventForNonSampledSpans;

        public StartEndHandler(ISpanExporter spanExporter, IRunningSpanStore runningSpanStore, ISampledSpanStore sampledSpanStore, IEventQueue eventQueue)
        {
            this.spanExporter = spanExporter;
            this.runningSpanStore = runningSpanStore;
            this.sampledSpanStore = sampledSpanStore;
            this.enqueueEventForNonSampledSpans = runningSpanStore != null || sampledSpanStore != null;
            this.eventQueue = eventQueue;
        }

        public void OnEnd(SpanBase span)
        {
            if ((span.Options.HasFlag(SpanOptions.RECORD_EVENTS) && enqueueEventForNonSampledSpans)
                || span.Context.TraceOptions.IsSampled)
            {
                eventQueue.Enqueue(new SpanEndEvent(span, spanExporter, runningSpanStore, sampledSpanStore));
            }
        }

        public void OnStart(SpanBase span)
        {
            if (span.Options.HasFlag(SpanOptions.RECORD_EVENTS) && enqueueEventForNonSampledSpans)
            {
                eventQueue.Enqueue(new SpanStartEvent(span, runningSpanStore));
            }
        }

        private sealed class SpanStartEvent : IEventQueueEntry
        {
            private readonly SpanBase span;
            private readonly IRunningSpanStore activeSpansExporter;

            public SpanStartEvent(SpanBase span, IRunningSpanStore activeSpansExporter)
            {
                this.span = span;
                this.activeSpansExporter = activeSpansExporter;
            }

            public void Process()
            {
                if (activeSpansExporter != null)
                {
                    activeSpansExporter.OnStart(span);
                }
            }
        }

        private sealed class SpanEndEvent : IEventQueueEntry
        {
            private readonly SpanBase span;
            private readonly IRunningSpanStore runningSpanStore;
            private readonly ISpanExporter spanExporter;
            private readonly ISampledSpanStore sampledSpanStore;

            public SpanEndEvent(
                    SpanBase span,
                    ISpanExporter spanExporter,
                    IRunningSpanStore runningSpanStore,
                    ISampledSpanStore sampledSpanStore)
            {
                this.span = span;
                this.runningSpanStore = runningSpanStore;
                this.spanExporter = spanExporter;
                this.sampledSpanStore = sampledSpanStore;
            }

            public void Process()
            {
                if (span.Context.TraceOptions.IsSampled)
                {
                    spanExporter.AddSpan(span);
                }

                if (runningSpanStore != null)
                {
                    runningSpanStore.OnEnd(span);
                }

                if (sampledSpanStore != null)
                {
                    sampledSpanStore.ConsiderForSampling(span);
                }
            }
        }
    }
}
