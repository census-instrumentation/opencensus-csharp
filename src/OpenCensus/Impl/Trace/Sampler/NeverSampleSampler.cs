namespace OpenCensus.Trace.Sampler
{
    using System.Collections.Generic;

    internal sealed class NeverSampleSampler : ISampler
    {
        internal NeverSampleSampler() { }

        public string Description
        {
            get
            {
                return ToString();
            }
        }

        public bool ShouldSample(ISpanContext parentContext, bool hasRemoteParent, ITraceId traceId, ISpanId spanId, string name, IList<ISpan> parentLinks)
        {
            return false;
        }

        public override string ToString()
        {
            return "NeverSampleSampler";
        }
    }
}
