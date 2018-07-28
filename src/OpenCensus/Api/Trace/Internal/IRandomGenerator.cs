

namespace OpenCensus.Trace.Internal
{
    public interface IRandomGenerator
    {
        void NextBytes(byte[] bytes);
    }
}
