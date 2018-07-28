using OpenCensus.Tags.Propagation;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Tags
{
    public interface ITagsComponent
    {

        ITagger Tagger { get; }

        ITagPropagationComponent TagPropagationComponent { get; }

        TaggingState State { get; }
    }
}