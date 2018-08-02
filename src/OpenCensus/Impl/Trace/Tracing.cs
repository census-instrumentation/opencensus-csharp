namespace OpenCensus.Trace
{
    using OpenCensus.Common;
    using OpenCensus.Internal;
    using OpenCensus.Trace.Config;
    using OpenCensus.Trace.Export;
    using OpenCensus.Trace.Internal;
    using OpenCensus.Trace.Propagation;

    public sealed class Tracing
    {

        private static Tracing tracing = new Tracing();

        internal Tracing()
            : this(false)
        {
        }

        internal Tracing(bool enabled)
        {
            if (enabled)
            {
                traceComponent = new TraceComponent(DateTimeOffsetClock.INSTANCE, new RandomGenerator(), new SimpleEventQueue());
            }
            else
            {
                traceComponent = TraceComponent.NewNoopTraceComponent;
            }
        }

        private ITraceComponent traceComponent = null;

        public static ITracer Tracer
        {
            get
            {
                return tracing.traceComponent.Tracer;
            }
        }

        public static IPropagationComponent PropagationComponent
        {
            get
            {
                return tracing.traceComponent.PropagationComponent;
            }
        }

        public static IExportComponent ExportComponent
        {
            get
            {
                return tracing.traceComponent.ExportComponent;
            }
        }

        public static ITraceConfig TraceConfig
        {
            get
            {
                return tracing.traceComponent.TraceConfig;
            }
        }
    }
}
