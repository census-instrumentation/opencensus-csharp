namespace OpenCensus.Trace
{
    using System.Collections.Generic;

    public interface IAnnotation
    {
        string Description { get; }

        IDictionary<string, IAttributeValue> Attributes { get; }
    }
}
