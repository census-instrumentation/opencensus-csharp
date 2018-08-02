namespace OpenCensus.Stats.Aggregations
{
    public interface ISumDataDouble : IAggregationData
    {
        double Sum { get; }
    }
}
