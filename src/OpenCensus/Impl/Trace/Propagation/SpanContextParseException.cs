namespace OpenCensus.Trace.Propagation
{
    using System;

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
