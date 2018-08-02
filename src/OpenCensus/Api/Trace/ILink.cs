namespace OpenCensus.Trace
{
    using System.Collections.Generic;

    public interface ILink
    {
        ITraceId TraceId { get; }

        ISpanId SpanId { get; }

        LinkType Type { get; }

        IDictionary<string, IAttributeValue> Attributes { get; }
    }
}
