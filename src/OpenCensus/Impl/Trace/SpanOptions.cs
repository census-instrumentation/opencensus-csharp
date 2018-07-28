using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Trace
{
    [Flags]
    public enum SpanOptions
    {
        NONE = 0x0,
        RECORD_EVENTS = 0x1
    }
}
