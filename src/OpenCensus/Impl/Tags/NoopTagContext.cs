using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Tags
{
    public sealed class NoopTagContext : TagContextBase
    {
        internal static readonly ITagContext INSTANCE = new NoopTagContext();

        public override IEnumerator<ITag> GetEnumerator()
        {
            return new List<ITag>().GetEnumerator();
        }
    }
}
