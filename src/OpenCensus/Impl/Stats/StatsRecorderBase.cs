using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Stats
{
    public abstract class StatsRecorderBase : IStatsRecorder
    {
        public abstract IMeasureMap NewMeasureMap();
    }
}
