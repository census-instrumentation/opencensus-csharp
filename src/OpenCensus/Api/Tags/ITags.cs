namespace OpenCensus.Tags
{
    using OpenCensus.Tags.Propagation;

    public interface ITags
    {
        ITagger Tagger { get; }

        ITagPropagationComponent TagPropagationComponent { get; }

        TaggingState State { get; }
    }
}
