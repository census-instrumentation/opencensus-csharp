using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Trace.Export
{
    public interface ILinks
    {
        IList<ILink> Links { get; }

        int DroppedLinksCount { get; }
    }
}
