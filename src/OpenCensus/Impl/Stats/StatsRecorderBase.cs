namespace OpenCensus.Stats
{
    public abstract class StatsRecorderBase : IStatsRecorder
    {
        public abstract IMeasureMap NewMeasureMap();
    }
}
