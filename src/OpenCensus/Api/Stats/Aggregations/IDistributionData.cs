using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Stats.Aggregations
{
    public interface IDistributionData : IAggregationData
    {
        double Mean { get; }

        long Count { get; }

        double Min { get; }

        double Max { get; }

        double SumOfSquaredDeviations { get; }

        IList<long> BucketCounts { get; }
    }
}
