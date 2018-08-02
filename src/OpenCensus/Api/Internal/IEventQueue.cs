namespace OpenCensus.Internal
{
    public interface IEventQueue
    {
        void Enqueue(IEventQueueEntry entry);
    }
}
