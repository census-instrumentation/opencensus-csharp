namespace OpenCensus.Trace.Export
{
    public interface ISampledSpanStoreErrorFilter
    {
        string SpanName { get; }

        CanonicalCode? CanonicalCode { get; }

        int MaxSpansToReturn { get; }
    }
}
