namespace OpenCensus.Stats.Aggregations
{
    public interface ICountData : IAggregationData
    {
        long Count { get; }
    }
}
