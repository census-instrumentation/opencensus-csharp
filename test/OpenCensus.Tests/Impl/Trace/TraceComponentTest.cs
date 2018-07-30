using OpenCensus.Common;
using OpenCensus.Internal;
using OpenCensus.Trace.Export;
using OpenCensus.Trace.Internal;
using OpenCensus.Trace.Propagation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenCensus.Trace.Test
{
    public class TraceComponentTest
    {
        private readonly TraceComponent traceComponent = new TraceComponent(DateTimeOffsetClock.GetInstance(), new RandomGenerator(), new SimpleEventQueue());

        [Fact]
        public void ImplementationOfTracer()
        {
            Assert.IsType<Tracer>(traceComponent.Tracer);
        }

        [Fact]
        public void IplementationOfBinaryPropagationHandler()
        {
            Assert.IsType<PropagationComponent>(traceComponent.PropagationComponent);
        }


        [Fact]
        public void ImplementationOfClock()
        {
            Assert.IsType<DateTimeOffsetClock>(traceComponent.Clock);
        }

        [Fact]
        public void ImplementationOfTraceExporter()
        {
            Assert.IsType<ExportComponent>(traceComponent.ExportComponent);
        }
    }
}