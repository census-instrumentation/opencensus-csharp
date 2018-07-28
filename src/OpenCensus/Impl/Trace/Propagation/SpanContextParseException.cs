using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Trace.Propagation
{
    public class SpanContextParseException : Exception
    {
        public SpanContextParseException(string message)
            : base(message)
        { 
        }
        public SpanContextParseException(string message, Exception cause)
            : base(message, cause)
        { 
        }
    }
}
