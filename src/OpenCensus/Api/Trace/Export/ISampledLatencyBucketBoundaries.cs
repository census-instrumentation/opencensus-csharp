using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Trace.Export
{
    public interface ISampledLatencyBucketBoundaries
    {
        long LatencyLowerNs { get; }

        long LatencyUpperNs { get; }
    }
}
