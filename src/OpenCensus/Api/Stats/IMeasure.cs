namespace OpenCensus.Stats
{
    using System;
    using OpenCensus.Stats.Measures;

    public interface IMeasure
    {
        M Match<M>(
            Func<IMeasureDouble, M> p0,
            Func<IMeasureLong, M> p1,
            Func<IMeasure, M> defaultFunction);

        string Name { get; }

        string Description { get; }

        string Unit { get; }
    }
}
