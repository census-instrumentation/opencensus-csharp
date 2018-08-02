namespace OpenCensus.Tags
{
    using OpenCensus.Common;

    public interface ITagger
    {
        ITagContext Empty { get; }

        ITagContext CurrentTagContext { get; }

        ITagContextBuilder EmptyBuilder { get; }

        ITagContextBuilder ToBuilder(ITagContext tags);

        ITagContextBuilder CurrentBuilder { get; }
  
        IScope WithTagContext(ITagContext tags);
    }
}
