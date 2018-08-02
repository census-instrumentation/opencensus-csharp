namespace OpenCensus.Stats
{
    using System.Collections.Generic;
    using OpenCensus.Tags;

    public interface IView
    {
        IViewName Name { get; }

        string Description { get; }

        IMeasure Measure { get; }

        IAggregation Aggregation { get; }

        IList<ITagKey> Columns { get; }
    }
}
