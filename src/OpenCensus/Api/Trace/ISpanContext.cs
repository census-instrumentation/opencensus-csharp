using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Trace
{
    public interface ISpanContext
    {
        ITraceId TraceId { get; }

        ISpanId SpanId { get; }

        TraceOptions TraceOptions { get; }

        bool IsValid { get; }
    }
}
