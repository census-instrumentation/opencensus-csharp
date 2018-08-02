namespace OpenCensus.Trace.Propagation
{
    public interface IBinaryFormat
    {
        ISpanContext FromByteArray(byte[] bytes);

        byte[] ToByteArray(ISpanContext spanContext);
    }
}
