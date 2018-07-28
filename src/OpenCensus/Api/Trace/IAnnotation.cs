using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Trace
{
    public interface IAnnotation
    {
        string Description { get; }
        IDictionary<string, IAttributeValue> Attributes { get; }
    }
}
