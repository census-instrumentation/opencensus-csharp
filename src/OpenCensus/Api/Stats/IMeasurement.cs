using OpenCensus.Stats.Measurements;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Stats
{
    public interface IMeasurement
    {
        M Match<M>(Func<IMeasurementDouble, M> p0, Func<IMeasurementLong, M> p1, Func<IMeasurement, M> defaultFunction);

        IMeasure Measure { get; }
    }
}
