using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Tags.Propagation
{
    public abstract class TagPropagationComponentBase : ITagPropagationComponent
    {
        protected TagPropagationComponentBase() { }
        public abstract ITagContextBinarySerializer BinarySerializer { get; }
    }
}
