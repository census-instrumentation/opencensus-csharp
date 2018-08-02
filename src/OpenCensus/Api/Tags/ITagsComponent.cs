namespace OpenCensus.Tags
{
    using OpenCensus.Tags.Propagation;

    public interface ITagsComponent
    {

        ITagger Tagger { get; }

        ITagPropagationComponent TagPropagationComponent { get; }

        TaggingState State { get; }
    }
}