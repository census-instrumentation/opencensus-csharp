using OpenCensus.Common;
using OpenCensus.Trace.Config;
using OpenCensus.Trace.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Trace
{
    internal class SpanBuilderOptions
    {
        internal IRandomGenerator RandomHandler { get; }
        internal IStartEndHandler StartEndHandler { get; }
        internal IClock Clock { get; }
        internal ITraceConfig TraceConfig { get; }

        internal SpanBuilderOptions(IRandomGenerator randomGenerator, IStartEndHandler startEndHandler, IClock clock, ITraceConfig traceConfig )
        {
            RandomHandler = randomGenerator;
            StartEndHandler = startEndHandler;
            Clock = clock;
            TraceConfig = traceConfig;
        }
    }
}
