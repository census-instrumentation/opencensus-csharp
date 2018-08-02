namespace OpenCensus.Tags
{
    using OpenCensus.Common;

    public interface ITagContextBuilder
    {
        ITagContextBuilder Put(ITagKey key, ITagValue value);

        ITagContextBuilder Remove(ITagKey key);

        ITagContext Build();

        IScope BuildScoped();
    }
}
