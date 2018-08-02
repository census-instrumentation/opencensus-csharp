namespace OpenCensus.Stats.Aggregations
{
    using System.Collections.Generic;

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
