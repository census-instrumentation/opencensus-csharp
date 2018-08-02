namespace OpenCensus.Trace
{
    using System.Collections.Generic;

    public interface ISampler
    {
        string Description { get; }

        bool ShouldSample(ISpanContext parentContext, bool hasRemoteParent, ITraceId traceId, ISpanId spanId, string name, IList<ISpan> parentLinks);
    }
}
