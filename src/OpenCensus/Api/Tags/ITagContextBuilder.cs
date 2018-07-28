using OpenCensus.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Tags
{
    public interface ITagContextBuilder
    {
        ITagContextBuilder Put(ITagKey key, ITagValue value);
        ITagContextBuilder Remove(ITagKey key);
        ITagContext Build();
        IScope BuildScoped();
    }
}
