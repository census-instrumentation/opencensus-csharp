namespace OpenCensus.Tags
{
    using System;
    using OpenCensus.Common;
    using OpenCensus.Internal;

    internal sealed class NoopTagger : TaggerBase
    {
        internal static readonly ITagger INSTANCE = new NoopTagger();

        public override ITagContext Empty
        {
            get
            {
                return NoopTags.NoopTagContext;
            }
        }

        public override ITagContext CurrentTagContext
        {
            get
            {
                return NoopTags.NoopTagContext;
            }
        }

        public override ITagContextBuilder EmptyBuilder
        {
            get
            {
                return NoopTags.NoopTagContextBuilder;
            }
        }

        public override ITagContextBuilder ToBuilder(ITagContext tags)
        {
            if (tags == null)
            {
                throw new ArgumentNullException(nameof(tags));
            }

            return NoopTags.NoopTagContextBuilder;
        }

        public override ITagContextBuilder CurrentBuilder
        {
            get
            {
                return NoopTags.NoopTagContextBuilder;
            }
        }

        public override IScope WithTagContext(ITagContext tags)
        {
            if (tags == null)
            {
                throw new ArgumentNullException(nameof(tags));
            }

            return NoopScope.INSTANCE;
        }
    }
}
