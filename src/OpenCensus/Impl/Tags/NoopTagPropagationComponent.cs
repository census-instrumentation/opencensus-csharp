using OpenCensus.Tags.Propagation;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Tags
{
    public class NoopTagPropagationComponent : TagPropagationComponentBase
    {
        internal static readonly ITagPropagationComponent INSTANCE = new NoopTagPropagationComponent();

        public override ITagContextBinarySerializer BinarySerializer
        {
            get
            {
                return NoopTags.NoopTagContextBinarySerializer;
            }
        }
    }
}
