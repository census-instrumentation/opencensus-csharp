namespace OpenCensus.Tags.Propagation
{
    using System;

    public sealed class TagContextDeserializationException : Exception
    {
        public TagContextDeserializationException(string message) 
            : base(message)
        {
        }

        public TagContextDeserializationException(string message, Exception cause)
            : base(message, cause)
        {
        }
    }
}
