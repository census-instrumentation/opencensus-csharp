namespace OpenCensus.Tags.Unsafe
{
    using System.Collections.Generic;
    using System.Threading;

    internal static class AsyncLocalContext
    {
        private static readonly ITagContext EMPTY_TAG_CONTEXT = new EmptyTagContext();

        private static AsyncLocal<ITagContext> context = new AsyncLocal<ITagContext>();

        public static ITagContext CurrentTagContext
        {
            get
            {
                if (context.Value == null)
                {
                    return EMPTY_TAG_CONTEXT;
                }

                return context.Value;
            }

            set
            {
                if (value == EMPTY_TAG_CONTEXT)
                {
                    context.Value = null;
                }
                else
                {
                    context.Value = value;
                }
            }
        }

        internal sealed class EmptyTagContext : TagContextBase
        {

            public override IEnumerator<ITag> GetEnumerator()
            {
                return new List<ITag>().GetEnumerator();
            }
        }
    }
}
