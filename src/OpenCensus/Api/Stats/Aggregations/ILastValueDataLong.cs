namespace OpenCensus.Stats.Aggregations
{
    public interface ILastValueDataLong : IAggregationData
    {
        long LastValue { get; }
    }
}
