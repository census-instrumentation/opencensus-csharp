using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Stats.Aggregations
{
    public interface ILastValueDataDouble : IAggregationData
    {
        double LastValue { get; }
    }
}
