using OpenCensus.Common;
using OpenCensus.Trace.Config;
using OpenCensus.Trace.Export;
using OpenCensus.Trace.Propagation;
using Xunit;

namespace OpenCensus.Trace.Test
{
    public class TraceComponentBaseTest
    {
        [Fact]
        public void DefaultTracer()
        {
            Assert.Same(Tracer.NoopTracer, TraceComponentBase.NewNoopTraceComponent.Tracer);
        }

        [Fact]
        public void DefaultBinaryPropagationHandler()
        {
            Assert.Same(PropagationComponentBase.NoopPropagationComponent, TraceComponentBase.NewNoopTraceComponent.PropagationComponent);
        }

        [Fact]
        public void DefaultClock()
        {
            Assert.True(TraceComponentBase.NewNoopTraceComponent.Clock is ZeroTimeClock);
        }

        [Fact]
        public void DefaultTraceExporter()
        {
            Assert.Equal(ExportComponentBase.NewNoopExportComponent.GetType(), TraceComponentBase.NewNoopTraceComponent.ExportComponent.GetType());
        }

        [Fact]
        public void DefaultTraceConfig()
        {
            Assert.Same(TraceConfigBase.NoopTraceConfig, TraceComponentBase.NewNoopTraceComponent.TraceConfig);

        }
    }
}
