using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Tags.Propagation
{
    public sealed class TagPropagationComponent : TagPropagationComponentBase
    {
        private readonly ITagContextBinarySerializer tagContextBinarySerializer;

        public TagPropagationComponent(CurrentTaggingState state)
        {
            tagContextBinarySerializer = new TagContextBinarySerializer(state);
        }

        public override ITagContextBinarySerializer BinarySerializer
        {
            get { return tagContextBinarySerializer; }
        }
    }
}
