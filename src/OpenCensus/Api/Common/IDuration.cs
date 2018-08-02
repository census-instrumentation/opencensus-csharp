namespace OpenCensus.Common
{
    using System;

    public interface IDuration : IComparable<IDuration>
    {
        long Seconds { get; }

        int Nanos { get; }
    }
}
