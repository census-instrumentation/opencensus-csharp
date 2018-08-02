using OpenCensus.Common;
using OpenCensus.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Trace.Export
{
    public abstract class ExportComponentBase : IExportComponent
    {
        public  static IExportComponent NewNoopExportComponent
        {
            get
            {
                return new NoopExportComponent();
            }
        }

        public abstract ISpanExporter SpanExporter { get; }

        public abstract IRunningSpanStore RunningSpanStore { get; }

        public abstract ISampledSpanStore SampledSpanStore { get; }

    }
}
