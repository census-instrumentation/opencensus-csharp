using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Trace.Export
{
    public interface ISampledSpanStoreErrorFilter
    {
        string SpanName { get; }
        CanonicalCode? CanonicalCode { get; }
        int MaxSpansToReturn { get; }
    }
}
