using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Stats
{
    public interface IStatsRecorder
    {
        IMeasureMap NewMeasureMap();
    }
}
