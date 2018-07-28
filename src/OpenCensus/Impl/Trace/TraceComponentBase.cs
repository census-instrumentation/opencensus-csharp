using OpenCensus.Common;
using OpenCensus.Trace.Config;
using OpenCensus.Trace.Export;
using OpenCensus.Trace.Propagation;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Trace
{
    public abstract class TraceComponentBase : ITraceComponent
    {
        internal static ITraceComponent NewNoopTraceComponent
        {
            get
            {
                return new NoopTraceComponent();
            }
        }

        public abstract ITracer Tracer { get; }

        public abstract IPropagationComponent PropagationComponent { get; }

        public abstract IClock Clock { get; }

        public abstract IExportComponent ExportComponent { get; }

        public abstract ITraceConfig TraceConfig { get; }
    }
}
