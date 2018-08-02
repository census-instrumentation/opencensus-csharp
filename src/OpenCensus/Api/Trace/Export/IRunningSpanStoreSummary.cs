namespace OpenCensus.Trace.Export
{
    using System.Collections.Generic;

    public interface IRunningSpanStoreSummary
    {
        IDictionary<string, IRunningPerSpanNameSummary> PerSpanNameSummary { get; }
    }
}
