namespace OpenCensus.Trace.Export
{
    public interface IRunningSpanStoreFilter
    {
        string SpanName { get; }

        int MaxSpansToReturn { get; }
    }
}
