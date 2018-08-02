namespace OpenCensus.Trace.Export
{
    public interface IExportComponent
    {
        ISpanExporter SpanExporter { get; }

        IRunningSpanStore RunningSpanStore { get; }

        ISampledSpanStore SampledSpanStore { get; }
    }
}