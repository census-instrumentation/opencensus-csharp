using OpenCensus.Internal;
using OpenCensus.Trace;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Utils
{
    public class DefaultEventQueue : IEventQueue
    {
        public void Enqueue(IEventQueueEntry entry)
        {
            throw new NotImplementedException();
        }
    }
}
