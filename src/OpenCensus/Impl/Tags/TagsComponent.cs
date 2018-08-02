namespace OpenCensus.Tags
{
    using OpenCensus.Tags.Propagation;

    public class TagsComponent : TagsComponentBase
    {
        // The TaggingState shared between the TagsComponent, Tagger, and TagPropagationComponent
        private readonly CurrentTaggingState state;
        private readonly ITagger tagger;
        private readonly ITagPropagationComponent tagPropagationComponent;

        public TagsComponent()
        {
            state = new CurrentTaggingState();
            tagger = new Tagger(state);
            tagPropagationComponent = new TagPropagationComponent(state);
        }

        public override ITagger Tagger
        {
            get { return tagger; }
        }

        public override ITagPropagationComponent TagPropagationComponent
        {
            get { return tagPropagationComponent; }
        }

        public override TaggingState State
        {
            get { return state.Value; }
        }
    }
}
