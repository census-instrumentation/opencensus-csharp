using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Tags.Propagation
{
    public sealed class TagContextDeserializationException : Exception
    {
        public TagContextDeserializationException(String message) 
            : base(message)
        {
        }

        public TagContextDeserializationException(String message, Exception cause)
            : base(message, cause)
        {
        }
    }
}
