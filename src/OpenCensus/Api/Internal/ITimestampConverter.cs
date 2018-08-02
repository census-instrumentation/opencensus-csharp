namespace OpenCensus.Internal
{
    using OpenCensus.Common;

    public interface ITimestampConverter
    {
        ITimestamp ConvertNanoTime(long nanoTime);
    }
}
