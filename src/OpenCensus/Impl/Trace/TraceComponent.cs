
using OpenCensus.Common;
using OpenCensus.Internal;
using OpenCensus.Trace.Config;
using OpenCensus.Trace.Export;
using OpenCensus.Trace.Internal;
using OpenCensus.Trace.Propagation;

namespace OpenCensus.Trace
{
    public sealed class TraceComponent : TraceComponentBase
    {
        public TraceComponent()
            : this(DateTimeOffsetClock.INSTANCE, new RandomGenerator(), new SimpleEventQueue())
        {

        }

        public TraceComponent(IClock clock, IRandomGenerator randomHandler, IEventQueue eventQueue)
        {
            Clock = clock;
            TraceConfig = new Config.TraceConfig();
            // TODO(bdrutu): Add a config/argument for supportInProcessStores.
            if (eventQueue is SimpleEventQueue) {
                ExportComponent = Export.ExportComponent.CreateWithoutInProcessStores(eventQueue);
            } else {
                ExportComponent = Export.ExportComponent.CreateWithInProcessStores(eventQueue);
            }
            PropagationComponent = new PropagationComponent();
            IStartEndHandler startEndHandler =
                new StartEndHandler(
                    ExportComponent.SpanExporter,
                    ExportComponent.RunningSpanStore,
                    ExportComponent.SampledSpanStore,
                    eventQueue);
            Tracer = new Tracer(randomHandler, startEndHandler, clock, TraceConfig);
        }

        public override ITracer Tracer { get; }

        public override IPropagationComponent PropagationComponent { get; }

        public override  IClock Clock { get; }

        public override  IExportComponent ExportComponent { get; }

        public override  ITraceConfig TraceConfig { get; }
    }
}
