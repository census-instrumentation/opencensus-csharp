namespace OpenCensus.Common
{
    public interface IClock
    {
        ITimestamp Now { get; }

        long NowNanos { get; }
    }
}
