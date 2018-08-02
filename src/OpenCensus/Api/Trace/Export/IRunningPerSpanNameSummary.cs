namespace OpenCensus.Trace.Export
{
    public interface IRunningPerSpanNameSummary
    {
        int NumRunningSpans { get; }
    }
}
