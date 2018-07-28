using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Trace.Export
{
    public interface IRunningSpanStoreSummary
    {
        IDictionary<string, IRunningPerSpanNameSummary> PerSpanNameSummary { get; }
    }
}
