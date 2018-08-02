namespace OpenCensus.Stats.Aggregations
{
    public interface ISumDataLong : IAggregationData
    {
        long Sum { get; }
    }
}
