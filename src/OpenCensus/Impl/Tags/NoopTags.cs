namespace OpenCensus.Tags
{
    using OpenCensus.Tags.Propagation;

    internal sealed class NoopTags
    {

        internal static ITagsComponent NewNoopTagsComponent()
        {
            return new NoopTagsComponent();
        }

        internal static ITagger NoopTagger
        {
            get
            {
                return OpenCensus.Tags.NoopTagger.INSTANCE;
            }
        }

        internal static ITagContextBuilder NoopTagContextBuilder
        {
            get
            {
                return OpenCensus.Tags.NoopTagContextBuilder.INSTANCE;
            }
        }

        internal static ITagContext NoopTagContext
        {
            get
            {
                return OpenCensus.Tags.NoopTagContext.INSTANCE;
            }
        }

        internal static ITagPropagationComponent NoopTagPropagationComponent
        {
            get
            {
                return OpenCensus.Tags.NoopTagPropagationComponent.INSTANCE;
            }
        }

        internal static ITagContextBinarySerializer NoopTagContextBinarySerializer
        {
            get
            {
                return OpenCensus.Tags.NoopTagContextBinarySerializer.INSTANCE;
            }
        }
    }
}
