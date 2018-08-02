namespace OpenCensus.Tags
{
    using OpenCensus.Tags.Propagation;

    public sealed class NoopTagsComponent : TagsComponentBase
    {
        //private volatile bool isRead;

        public override ITagger Tagger
        {
            get
            {
                return NoopTags.NoopTagger;
            }
        }

        public override ITagPropagationComponent TagPropagationComponent
        {
            get
            {
                return NoopTags.NoopTagPropagationComponent;
            }
        }

        public override TaggingState State
        {
            get
            {
                //isRead = true;
                return TaggingState.DISABLED;
            }
        }
    }
}
