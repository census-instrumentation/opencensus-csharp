using System;
using System.Collections.Generic;
using System.Text;

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
