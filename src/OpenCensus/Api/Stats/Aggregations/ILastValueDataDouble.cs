namespace OpenCensus.Stats.Aggregations
{
    public interface ILastValueDataDouble : IAggregationData
    {
        double LastValue { get; }
    }
}
