namespace OpenCensus.Trace.Propagation
{
    public interface ISetter<C>
    {
        void Put(C carrier, string key, string value);
    }
}
