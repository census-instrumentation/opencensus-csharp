using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Stats.Aggregations
{
    public interface IDistribution : IAggregation
    {

        IBucketBoundaries BucketBoundaries { get; }
    }
}
