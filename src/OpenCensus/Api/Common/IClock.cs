using OpenCensus.Trace;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Common
{
    public interface IClock
    {
        ITimestamp Now { get; }
        long NowNanos { get; }
    }
}
