using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Internal
{
    public interface IEventQueueEntry
    {
        void Process();
    }
}
