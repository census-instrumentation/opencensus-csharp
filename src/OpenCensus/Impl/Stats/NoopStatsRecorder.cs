namespace OpenCensus.Stats
{
    internal sealed class NoopStatsRecorder : StatsRecorderBase
    {
        internal static readonly IStatsRecorder INSTANCE = new NoopStatsRecorder();

    public override IMeasureMap NewMeasureMap()
        {
            return NoopStats.NoopMeasureMap;
        }
    }
}
