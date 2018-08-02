namespace OpenCensus.Stats
{
    using System;
    using OpenCensus.Stats.Measures;

    public abstract class Measure : IMeasure
    {
        internal const int NAME_MAX_LENGTH = 255;

        public abstract string Name { get; }

        public abstract string Description { get; }

        public abstract string Unit { get; }

        public abstract M Match<M>(Func<IMeasureDouble, M> p0, Func<IMeasureLong, M> p1, Func<IMeasure, M> defaultFunction);
    }
}
