using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Trace.Export
{
    public interface IRunningPerSpanNameSummary
    {
        int NumRunningSpans { get; }
    }
}
