using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Stats.Aggregations
{
    public interface ILastValueDataLong : IAggregationData
    {
        long LastValue { get; }
    }
}
