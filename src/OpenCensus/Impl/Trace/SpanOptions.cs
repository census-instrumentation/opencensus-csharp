namespace OpenCensus.Trace
{
    using System;

    [Flags]
    public enum SpanOptions
    {
        NONE = 0x0,
        RECORD_EVENTS = 0x1
    }
}
