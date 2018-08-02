namespace OpenCensus.Utils
{
    using System;
    using OpenCensus.Internal;

    public class DefaultEventQueue : IEventQueue
    {
        public void Enqueue(IEventQueueEntry entry)
        {
            throw new NotImplementedException();
        }
    }
}
