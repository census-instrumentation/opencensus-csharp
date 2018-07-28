using OpenCensus.Trace.Config;
using OpenCensus.Trace.Export;
using OpenCensus.Trace.Propagation;

namespace OpenCensus.Trace
{
    public interface ITracing
    {
        ITracer Tracer { get; }
        IPropagationComponent PropagationComponent { get; }
        IExportComponent ExportComponent { get; }
        ITraceConfig TraceConfig { get; }
    }
}
