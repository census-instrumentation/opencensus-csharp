using OpenCensus.Tags;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Stats
{
    public interface IView
    {
        IViewName Name { get; }
        string Description { get; }
        IMeasure Measure { get; }
        IAggregation Aggregation { get; }
        IList<ITagKey> Columns { get; }
    }
}
