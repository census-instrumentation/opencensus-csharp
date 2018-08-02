using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Trace.Export
{
    public interface ITimedEvents<T>
    {
        IList<ITimedEvent<T>> Events { get; }

        int DroppedEventsCount { get; }
    }
}
