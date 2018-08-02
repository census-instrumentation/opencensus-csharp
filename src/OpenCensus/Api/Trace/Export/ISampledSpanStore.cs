namespace OpenCensus.Trace.Export
{
    using System.Collections.Generic;

    public interface ISampledSpanStore
    {
        ISampledSpanStoreSummary Summary { get; }

        IList<ISpanData> GetLatencySampledSpans(ISampledSpanStoreLatencyFilter filter);

        IList<ISpanData> GetErrorSampledSpans(ISampledSpanStoreErrorFilter filter);

        void RegisterSpanNamesForCollection(IList<string> spanNames);

        void UnregisterSpanNamesForCollection(IList<string> spanNames);

        ISet<string> RegisteredSpanNamesForCollection { get; }

        void ConsiderForSampling(ISpan span);
    }
}
