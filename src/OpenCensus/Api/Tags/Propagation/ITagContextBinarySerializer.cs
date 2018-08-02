namespace OpenCensus.Tags.Propagation
{
    public interface ITagContextBinarySerializer
    {
        byte[] ToByteArray(ITagContext tags);

        ITagContext FromByteArray(byte[] bytes);
    }
}
