using OpenCensus.Stats.Measures;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Stats.Measurements
{
    public interface IMeasurementLong : IMeasurement
    {
        long Value { get; }
    }
}
