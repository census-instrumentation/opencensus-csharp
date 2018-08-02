namespace OpenCensus.Trace.Export
{
    using System;

    public interface ISpanExporter : IDisposable
    {
        void AddSpan(ISpan span);

        void RegisterHandler(string name, IHandler handler);

        void UnregisterHandler(string name);
    }
}
