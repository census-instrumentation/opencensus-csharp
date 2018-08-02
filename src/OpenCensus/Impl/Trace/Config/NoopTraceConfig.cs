namespace OpenCensus.Trace.Config
{
    public class NoopTraceConfig : TraceConfigBase
    {
        public override ITraceParams ActiveTraceParams
        {
            get { return TraceParams.DEFAULT; }
        }

        public override void UpdateActiveTraceParams(ITraceParams traceParams)
        {
        }
    }
}
