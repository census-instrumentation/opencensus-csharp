namespace OpenCensus.Tags
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    public sealed class TagContext : TagContextBase
    {
        public static readonly ITagContext EMPTY =  new TagContext(new Dictionary<ITagKey, ITagValue>());

        public IDictionary<ITagKey, ITagValue> Tags { get; }

        public TagContext(IDictionary<ITagKey,ITagValue> tags)
        {
            this.Tags = new ReadOnlyDictionary<ITagKey, ITagValue>(new Dictionary<ITagKey, ITagValue>(tags));
        }

        public override IEnumerator<ITag> GetEnumerator()
        {
            var result = Tags.Select((kvp) => Tag.Create(kvp.Key, kvp.Value));
            return result.ToList().GetEnumerator();
        }
    }
}
