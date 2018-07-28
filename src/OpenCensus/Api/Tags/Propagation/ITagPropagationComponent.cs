using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Tags.Propagation
{
    public interface ITagPropagationComponent
    {
        ITagContextBinarySerializer BinarySerializer { get; }
    }
}
