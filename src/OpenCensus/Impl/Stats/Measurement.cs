using System;
using System.Collections.Generic;
using System.Text;
using OpenCensus.Stats.Measurements;

namespace OpenCensus.Stats
{
    public abstract class Measurement : IMeasurement
    {
        public abstract IMeasure Measure { get; }

        public abstract M Match<M>(Func<IMeasurementDouble, M> p0, Func<IMeasurementLong, M> p1, Func<IMeasurement, M> defaultFunction);
    }
}
