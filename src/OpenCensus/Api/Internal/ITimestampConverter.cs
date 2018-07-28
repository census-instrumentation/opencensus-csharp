using OpenCensus.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Internal
{
    public interface ITimestampConverter
    {
        ITimestamp ConvertNanoTime(long nanoTime);
    }
}
