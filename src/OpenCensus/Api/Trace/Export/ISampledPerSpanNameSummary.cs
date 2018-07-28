using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Trace.Export
{
    public interface ISampledPerSpanNameSummary
    {
        IDictionary<ISampledLatencyBucketBoundaries, int> NumbersOfLatencySampledSpans { get; }
        IDictionary<CanonicalCode, int> NumbersOfErrorSampledSpans { get; }
    }
}
