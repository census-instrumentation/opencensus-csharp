namespace OpenCensus.Trace
{
    using System;

    public interface ITraceId : IComparable<ITraceId>
    {
        byte[] Bytes { get; }

        bool IsValid { get; }

        long LowerLong { get; }

        void CopyBytesTo(byte[] dest, int destOffset);

        string ToLowerBase16();
    }
}
