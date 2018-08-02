namespace OpenCensus.Trace.Export
{
    using System.Collections.Generic;

    public interface ITimedEvents<T>
    {
        IList<ITimedEvent<T>> Events { get; }

        int DroppedEventsCount { get; }
    }
}
