namespace OpenCensus.Trace
{
    using System.Collections.Generic;

    public interface ISpan
    {
        ISpanContext Context { get; }

        SpanOptions Options { get; }

        Status Status { get; set; }

        void PutAttribute(string key, IAttributeValue value);

        void PutAttributes(IDictionary<string, IAttributeValue> attributes);

        void AddAnnotation(string description);

        void AddAnnotation(string description, IDictionary<string, IAttributeValue> attributes);

        void AddAnnotation(IAnnotation annotation);

        void AddMessageEvent(IMessageEvent messageEvent);

        void AddLink(ILink link);

        void End(EndSpanOptions options);

        void End();
    }
}
