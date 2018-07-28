using OpenCensus.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Tags
{
    public interface ITagger
    {
        ITagContext Empty { get; }

        ITagContext CurrentTagContext { get; }

        ITagContextBuilder EmptyBuilder { get; }

        ITagContextBuilder ToBuilder(ITagContext tags);

        ITagContextBuilder CurrentBuilder { get; }
  
        IScope WithTagContext(ITagContext tags);
    }
}
