using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Stats.Measurements
{
    public interface IMeasurementDouble : IMeasurement
    {
        double Value { get; }
    }
}
