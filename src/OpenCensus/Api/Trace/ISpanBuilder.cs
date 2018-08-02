namespace OpenCensus.Trace
{
    using System.Collections.Generic;
    using OpenCensus.Common;

    public interface ISpanBuilder
    {
        ISpanBuilder SetSampler(ISampler sampler);

        ISpanBuilder SetParentLinks(IList<ISpan> parentLinks);

        ISpanBuilder SetRecordEvents(bool recordEvents);

        ISpan StartSpan();

        IScope StartScopedSpan();
    }
}
