using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Internal
{
    public class SimpleEventQueue : IEventQueue
    {
        public void Enqueue(IEventQueueEntry entry)
        {
            entry.Process();
        }
    }
}
