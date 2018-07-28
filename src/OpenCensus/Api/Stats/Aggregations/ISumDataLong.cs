using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Stats.Aggregations
{
    public interface ISumDataLong : IAggregationData
    {
        long Sum { get; }
    }
}
