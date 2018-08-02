namespace OpenCensus.Tags.Propagation
{
    using System;

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
