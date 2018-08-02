namespace OpenCensus.Tags
{
    using OpenCensus.Tags.Propagation;

    public sealed class Tags
    {
        private static readonly object lck = new object();

        internal static void Initialize(bool enabled)
        {
            if (tags == null)
            {
                lock (lck)
                {
                    tags =tags ?? new Tags(enabled);
                }
            }
        }

        private static Tags tags;

        internal Tags()
            : this(false)
        {
        }

        internal Tags(bool enabled)
        {
            if (enabled)
            {
                tagsComponent = new TagsComponent();
            }
            else
            {
                tagsComponent = NoopTags.NewNoopTagsComponent();
            }
        }

        private readonly ITagsComponent tagsComponent = new TagsComponent();

        public static ITagger Tagger
        {
            get
            {
                Initialize(false);
                return tags.tagsComponent.Tagger;
            }
        }

        public static ITagPropagationComponent TagPropagationComponent
        {
            get
            {
                Initialize(false);
                return tags.tagsComponent.TagPropagationComponent;
            }
        }

        public static TaggingState State
        {
            get
            {
                Initialize(false);
                return tags.tagsComponent.State;
            }
        }
    }
}
