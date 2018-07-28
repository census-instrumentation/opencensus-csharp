using System;
using System.Collections.Generic;
using System.Text;
using OpenCensus.Trace;

namespace OpenCensus.Trace.Propagation
{
    internal class NoopBinaryFormat : IBinaryFormat
    {
        public ISpanContext FromByteArray(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }
            return SpanContext.INVALID;
        }

        public byte[] ToByteArray(ISpanContext spanContext)
        {
           if (spanContext == null)
            {
                throw new ArgumentNullException(nameof(spanContext));
            }
            return new byte[0];
        }
    }
}
