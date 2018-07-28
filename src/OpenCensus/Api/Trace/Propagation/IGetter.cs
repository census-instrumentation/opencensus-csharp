using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Trace.Propagation
{
    public interface IGetter<C>
    {
        string Get(C carrier, string key);
    }
}
