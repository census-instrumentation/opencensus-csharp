namespace OpenCensus.Tags
{
    using System.Collections.Generic;

    public sealed class NoopTagContext : TagContextBase
    {
        internal static readonly ITagContext INSTANCE = new NoopTagContext();

        public override IEnumerator<ITag> GetEnumerator()
        {
            return new List<ITag>().GetEnumerator();
        }
    }
}
