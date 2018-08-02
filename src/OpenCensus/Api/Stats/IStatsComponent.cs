namespace OpenCensus.Stats
{
    public interface IStatsComponent
    {
        IViewManager ViewManager { get; }

        IStatsRecorder StatsRecorder { get; }

        StatsCollectionState State { get; }
    }
}
