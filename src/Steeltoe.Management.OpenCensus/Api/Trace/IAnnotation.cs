using System.Collections.Generic;

namespace Steeltoe.Management.Census.Trace
{
    public interface IAnnotation
    {
        string Description { get; }
        IDictionary<string, IAttributeValue> Attributes { get; }
    }
}
