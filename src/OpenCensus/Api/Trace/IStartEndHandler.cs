using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Trace
{
    public interface IStartEndHandler
    {
        void OnStart(SpanBase span);

        void OnEnd(SpanBase span);
    }
}
