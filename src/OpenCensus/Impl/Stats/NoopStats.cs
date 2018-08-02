using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Stats
{
    internal sealed class NoopStats
    {
        private NoopStats() { }

        internal static IStatsComponent NewNoopStatsComponent()
        {
            return new NoopStatsComponent();
        }

        internal static IStatsRecorder NoopStatsRecorder
        {
            get
            {
                return OpenCensus.Stats.NoopStatsRecorder.INSTANCE;
            }
        }

        internal static IMeasureMap NoopMeasureMap
        {
            get
            {
                return OpenCensus.Stats.NoopMeasureMap.INSTANCE;
            }
        }

        internal static IViewManager NewNoopViewManager()
        {
            return new NoopViewManager();
        }
    }
}
