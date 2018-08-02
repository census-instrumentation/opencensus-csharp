namespace OpenCensus.Trace.Export
{
    using System.Collections.Generic;

    public interface ILinks
    {
        IList<ILink> Links { get; }

        int DroppedLinksCount { get; }
    }
}
