namespace OpenCensus.Stats
{
    using System.Collections.Generic;

    public interface IBucketBoundaries
    {
        IList<double> Boundaries { get; }
    }
}
