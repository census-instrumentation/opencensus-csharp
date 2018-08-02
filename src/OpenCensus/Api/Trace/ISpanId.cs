namespace OpenCensus.Trace
{
    using System;

    public interface ISpanId : IComparable<ISpanId>
    {
        byte[] Bytes { get; }

        bool IsValid { get; }

        void CopyBytesTo(byte[] dest, int destOffset);

        string ToLowerBase16();
    }
}
