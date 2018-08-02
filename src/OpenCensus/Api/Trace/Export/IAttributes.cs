namespace OpenCensus.Trace.Export
{
    using System.Collections.Generic;

    public interface IAttributes
    {
        IDictionary<string, IAttributeValue> AttributeMap { get; }

        int DroppedAttributesCount { get; }
    }
}
