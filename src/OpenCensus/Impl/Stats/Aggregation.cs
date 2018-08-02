namespace OpenCensus.Stats
{
    using System;
    using OpenCensus.Stats.Aggregations;

    public abstract class Aggregation : IAggregation
    {
        public abstract M Match<M>(Func<ISum, M> p0, Func<ICount, M> p1, Func<IMean, M> p2, Func<IDistribution, M> p3, Func<ILastValue, M> p4, Func<IAggregation, M> p5);
    }
}
