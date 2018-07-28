using OpenCensus.Tags;
using OpenCensus.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Stats
{
    public interface IViewData
    {
        IView View { get; }
        IDictionary<TagValues, IAggregationData> AggregationMap { get; }
        ITimestamp Start { get; }
        ITimestamp End { get; }
    }
}
