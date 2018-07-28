using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Common
{
    public interface IDuration : IComparable<IDuration>
    {
        long Seconds { get; }
        int Nanos { get; }
    }
}
