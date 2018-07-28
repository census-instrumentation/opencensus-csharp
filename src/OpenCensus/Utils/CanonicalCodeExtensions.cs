using OpenCensus.Trace;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Utils
{
    public static class CanonicalCodeExtensions
    {
        public static Status ToStatus(this CanonicalCode code)
        {
            return new Status(code);
        }
    }
}
