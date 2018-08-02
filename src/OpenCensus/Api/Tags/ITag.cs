using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Tags
{
    public interface ITag
    {
        ITagKey Key { get; }

        ITagValue Value { get; }
    }
}
