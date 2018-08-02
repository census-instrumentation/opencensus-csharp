namespace OpenCensus.Tags
{
    public interface ITag
    {
        ITagKey Key { get; }

        ITagValue Value { get; }
    }
}
