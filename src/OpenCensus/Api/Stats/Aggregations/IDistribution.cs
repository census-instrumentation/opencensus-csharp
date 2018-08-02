namespace OpenCensus.Stats.Aggregations
{
    public interface IDistribution : IAggregation
    {

        IBucketBoundaries BucketBoundaries { get; }
    }
}
