namespace OpenCensus.Tags
{
    using OpenCensus.Tags.Propagation;

    public abstract class TagsComponentBase : ITagsComponent
    {
        public abstract ITagger Tagger { get; }

        public abstract ITagPropagationComponent TagPropagationComponent { get; }

        public abstract TaggingState State { get; }
    }
}
