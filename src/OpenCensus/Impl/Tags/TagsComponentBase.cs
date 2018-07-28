using System;
using System.Collections.Generic;
using System.Text;
using OpenCensus.Tags.Propagation;

namespace OpenCensus.Tags
{
    public abstract class TagsComponentBase : ITagsComponent
    {
        public abstract ITagger Tagger { get; }
        public abstract ITagPropagationComponent TagPropagationComponent { get; }
        public abstract TaggingState State { get; }
    }
}
