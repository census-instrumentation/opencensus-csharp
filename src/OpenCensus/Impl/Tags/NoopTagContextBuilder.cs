namespace OpenCensus.Tags
{
    using System;
    using OpenCensus.Common;
    using OpenCensus.Internal;

    internal sealed class NoopTagContextBuilder : TagContextBuilderBase
    {
        internal static readonly ITagContextBuilder INSTANCE = new NoopTagContextBuilder();

        private NoopTagContextBuilder() { }

        public override ITagContextBuilder Put(ITagKey key, ITagValue value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return this;
        }

        public override ITagContextBuilder Remove(ITagKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return this;
        }

        public override ITagContext Build()
        {
            return NoopTagContext.INSTANCE;
        }

        public override IScope BuildScoped()
        {
            return NoopScope.INSTANCE;
        }
    }
}
