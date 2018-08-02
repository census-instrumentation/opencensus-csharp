namespace OpenCensus.Tags.Propagation
{
    using System;

    public sealed class TagContextSerializationException : Exception
    {
        public TagContextSerializationException(String message)
             : base(message)
        {
        }

        public TagContextSerializationException(String message, Exception cause)
            : base(message, cause)
        {
        }
    }
}
