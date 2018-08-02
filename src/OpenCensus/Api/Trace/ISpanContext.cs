namespace OpenCensus.Trace
{
    public interface ISpanContext
    {
        ITraceId TraceId { get; }

        ISpanId SpanId { get; }

        TraceOptions TraceOptions { get; }

        bool IsValid { get; }
    }
}
