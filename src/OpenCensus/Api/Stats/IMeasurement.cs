namespace OpenCensus.Stats
{
    using System;
    using OpenCensus.Stats.Measurements;

    public interface IMeasurement
    {
        M Match<M>(Func<IMeasurementDouble, M> p0, Func<IMeasurementLong, M> p1, Func<IMeasurement, M> defaultFunction);

        IMeasure Measure { get; }
    }
}
