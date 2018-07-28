
using OpenCensus.Tags.Propagation;

namespace OpenCensus.Tags
{
    public interface ITags
    {
        ITagger Tagger { get; }

        ITagPropagationComponent TagPropagationComponent { get; }
  
        TaggingState State { get; }
    }
}
