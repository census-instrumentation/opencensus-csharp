namespace Steeltoe.Management.Census.Trace
{
    public interface IMessageEvent
    {
        MessageEventType Type { get; }
        ulong MessageId { get; }
        ulong UncompressedMessageSize { get; }
        ulong CompressedMessageSize { get; }
    }
}
