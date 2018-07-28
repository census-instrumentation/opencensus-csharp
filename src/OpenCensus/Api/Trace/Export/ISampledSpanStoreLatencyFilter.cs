using System;
using System.Collections.Generic;
using System.Text;

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
