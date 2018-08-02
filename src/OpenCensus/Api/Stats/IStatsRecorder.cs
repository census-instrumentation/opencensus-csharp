namespace OpenCensus.Stats
{
    public interface IStatsRecorder
    {
        IMeasureMap NewMeasureMap();
    }
}
