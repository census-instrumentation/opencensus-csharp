namespace OpenCensus.Tags
{
    using OpenCensus.Tags.Propagation;

    public class NoopTagPropagationComponent : TagPropagationComponentBase
    {
        internal static readonly ITagPropagationComponent INSTANCE = new NoopTagPropagationComponent();

        public override ITagContextBinarySerializer BinarySerializer
        {
            get
            {
                return NoopTags.NoopTagContextBinarySerializer;
            }
        }
    }
}
