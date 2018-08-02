namespace OpenCensus.Trace.Export
{
    using System.Collections.Generic;

    public interface ISampledPerSpanNameSummary
    {
        IDictionary<ISampledLatencyBucketBoundaries, int> NumbersOfLatencySampledSpans { get; }

        IDictionary<CanonicalCode, int> NumbersOfErrorSampledSpans { get; }
    }
}
