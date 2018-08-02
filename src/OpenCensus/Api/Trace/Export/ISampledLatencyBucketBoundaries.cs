namespace OpenCensus.Trace.Export
{
    public interface ISampledLatencyBucketBoundaries
    {
        long LatencyLowerNs { get; }

        long LatencyUpperNs { get; }
    }
}
