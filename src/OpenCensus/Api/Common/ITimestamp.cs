namespace OpenCensus.Common
{
    using System;

    public interface ITimestamp : IComparable<ITimestamp>
    {
        long Seconds { get; }

        int Nanos { get; }

        ITimestamp AddNanos(long nanosToAdd);

        ITimestamp AddDuration(IDuration duration);

        IDuration SubtractTimestamp(ITimestamp timestamp);
    }
}
