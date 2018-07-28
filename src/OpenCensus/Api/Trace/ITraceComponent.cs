

using OpenCensus.Common;
using OpenCensus.Trace.Config;
using OpenCensus.Trace.Export;
using OpenCensus.Trace.Propagation;

namespace OpenCensus.Trace
{
    public interface ITraceComponent
    {
        ITracer Tracer { get; }
        IPropagationComponent PropagationComponent { get; }
        IClock Clock { get; }
        IExportComponent ExportComponent { get; }
        ITraceConfig TraceConfig { get; }
    }
}
