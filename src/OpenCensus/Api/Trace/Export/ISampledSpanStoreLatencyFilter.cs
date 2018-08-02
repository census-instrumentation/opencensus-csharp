namespace OpenCensus.Trace.Export
{
    public interface ISampledSpanStoreLatencyFilter
    {
        string SpanName { get; }

        long LatencyLowerNs { get; }

        long LatencyUpperNs { get; }

        int MaxSpansToReturn { get; }
    }
}
