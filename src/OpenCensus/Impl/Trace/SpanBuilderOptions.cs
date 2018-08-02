namespace OpenCensus.Trace
{
    using OpenCensus.Common;
    using OpenCensus.Trace.Config;
    using OpenCensus.Trace.Internal;

    internal class SpanBuilderOptions
    {
        internal IRandomGenerator RandomHandler { get; }

        internal IStartEndHandler StartEndHandler { get; }

        internal IClock Clock { get; }

        internal ITraceConfig TraceConfig { get; }

        internal SpanBuilderOptions(IRandomGenerator randomGenerator, IStartEndHandler startEndHandler, IClock clock, ITraceConfig traceConfig)
        {
            RandomHandler = randomGenerator;
            StartEndHandler = startEndHandler;
            Clock = clock;
            TraceConfig = traceConfig;
        }
    }
}
