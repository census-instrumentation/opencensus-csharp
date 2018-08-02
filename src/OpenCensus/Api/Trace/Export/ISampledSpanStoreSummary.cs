namespace OpenCensus.Trace.Export
{
    using System.Collections.Generic;

    public interface ISampledSpanStoreSummary
    {
        IDictionary<string, ISampledPerSpanNameSummary> PerSpanNameSummary { get; }
    }
}
