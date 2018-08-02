namespace OpenCensus.Trace.Export
{
    using OpenCensus.Common;

    public interface ITimedEvent<T>
    {
        ITimestamp Timestamp { get; }

        T Event { get; }
    }
}
