namespace OpenCensus.Stats
{
    using System.Collections.Generic;
    using OpenCensus.Common;
    using OpenCensus.Tags;

    public interface IViewData
    {
        IView View { get; }

        IDictionary<TagValues, IAggregationData> AggregationMap { get; }

        ITimestamp Start { get; }

        ITimestamp End { get; }
    }
}
