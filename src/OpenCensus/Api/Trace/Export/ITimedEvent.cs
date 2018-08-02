

using OpenCensus.Common;

namespace OpenCensus.Trace.Export
{
    public interface ITimedEvent<T>
    {
        ITimestamp Timestamp { get; }

        T Event { get; }
    }
}
