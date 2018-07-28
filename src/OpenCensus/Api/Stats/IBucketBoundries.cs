using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Stats
{
    public interface IBucketBoundaries
    {
        IList<double> Boundaries { get; }
    }
}
