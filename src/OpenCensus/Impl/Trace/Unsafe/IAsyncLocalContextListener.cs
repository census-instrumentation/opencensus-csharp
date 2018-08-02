namespace OpenCensus.Trace.Unsafe
{
    public interface IAsyncLocalContextListener
    {
        void ContextChanged(ISpan oldSpan, ISpan newSapn, bool threadContextSwitch);
    }
}
