namespace OpenCensus.Trace.Export
{
    using System.Collections.Generic;

    public interface IRunningSpanStore
    {
        IRunningSpanStoreSummary Summary { get; }

        IList<ISpanData> GetRunningSpans(IRunningSpanStoreFilter filter);

        void OnStart(ISpan span);

        void OnEnd(ISpan span);
    }
}
