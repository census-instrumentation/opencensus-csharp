using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Stats
{
    public interface IStats
    {
        IStatsRecorder StatsRecorder { get; }
        IViewManager ViewManager { get; }
        StatsCollectionState State { get; set; }
    }
}
