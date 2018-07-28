using OpenCensus.Trace;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Trace.Propagation
{
    public interface IBinaryFormat
    {
        ISpanContext FromByteArray(byte[] bytes);
        byte[] ToByteArray(ISpanContext spanContext);
    }
}
