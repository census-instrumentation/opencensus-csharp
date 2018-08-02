namespace OpenCensus.Stats
{
    public class Stats
    {
        private static Stats stats = new Stats();

        internal Stats()
            : this(false)
        {
        }

        internal Stats(bool enabled)
        {
            if (enabled)
            {
                statsComponent = new StatsComponent();
            }
            else
            {
                statsComponent = NoopStats.NewNoopStatsComponent();
            }
        }

        private  IStatsComponent statsComponent = new StatsComponent();

        public static IStatsRecorder StatsRecorder
        {
            get
            {
                return stats.statsComponent.StatsRecorder;
            }
        }

        public static IViewManager ViewManager
        {
            get
            {
                return stats.statsComponent.ViewManager;
            }
        }

        public static StatsCollectionState State
        {
            get
            {
                return stats.statsComponent.State;
            }
        }
    }
}
