namespace OpenCensus.Tags.Propagation
{
    using System;

    public sealed class TagContextSerializationException : Exception
    {
        public TagContextSerializationException(string message)
             : base(message)
        {
        }

        public TagContextSerializationException(string message, Exception cause)
            : base(message, cause)
        {
        }
    }
}
