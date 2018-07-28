using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Trace.Export
{
    public interface IAttributes
    {
        IDictionary<string, IAttributeValue> AttributeMap { get; }
        int DroppedAttributesCount { get; }
    }
}
