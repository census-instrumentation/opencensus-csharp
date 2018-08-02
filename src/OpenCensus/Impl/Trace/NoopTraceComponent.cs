namespace OpenCensus.Trace
{
    using OpenCensus.Common;
    using OpenCensus.Trace.Config;
    using OpenCensus.Trace.Export;
    using OpenCensus.Trace.Propagation;

    internal sealed class NoopTraceComponent : ITraceComponent
    {
        private readonly IExportComponent noopExportComponent = Export.ExportComponentBase.NewNoopExportComponent;

        public ITracer Tracer
        {
         
            get
            {
                return Trace.Tracer.NoopTracer;
            }
        }

        public IPropagationComponent PropagationComponent
        {
            get
            {
                return Propagation.PropagationComponentBase.NoopPropagationComponent;
            }
        }

        public IClock Clock
        {
            get
            {
                return ZeroTimeClock.INSTANCE;
            }
        }

        public IExportComponent ExportComponent
        {
            get
            {
                return noopExportComponent;
            }
        }

        public ITraceConfig TraceConfig
        {
            get
            {
                return Config.TraceConfigBase.NoopTraceConfig;
            }
        }
    }
}
