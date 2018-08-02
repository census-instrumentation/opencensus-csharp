namespace OpenCensus.Trace.Config
{
    public sealed class TraceConfig : TraceConfigBase
    {
        public TraceConfig()
        {
            activeTraceParams = TraceParams.DEFAULT;
        }

        private ITraceParams activeTraceParams;

        public override ITraceParams ActiveTraceParams
        {
            get
            {
                return activeTraceParams;
            }
        }

        public override void UpdateActiveTraceParams(ITraceParams traceParams)
        {
            activeTraceParams = traceParams;
        }
    }
}
