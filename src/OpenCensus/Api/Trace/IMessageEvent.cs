namespace OpenCensus.Trace
{
    public interface IMessageEvent
    {
        MessageEventType Type { get; }

        long MessageId { get; }

        long UncompressedMessageSize { get; }

        long CompressedMessageSize { get; }
    }
}
