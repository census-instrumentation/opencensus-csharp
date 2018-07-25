using System;

namespace Steeltoe.Management.Census.Trace
{
    public class MessageEventBuilder
    {
        private MessageEventType? type;
        private UInt64? messageId;
        private UInt64? uncompressedMessageSize;
        private UInt64? compressedMessageSize;

        internal MessageEventBuilder()
        {
        }
        internal MessageEventBuilder(
            MessageEventType type,
            ulong messageId,
            ulong uncompressedMessageSize,
            ulong compressedMessageSize)
        {
            this.type = type;
            this.messageId = messageId;
            this.uncompressedMessageSize = uncompressedMessageSize;
            this.compressedMessageSize = compressedMessageSize;
        }
        internal MessageEventBuilder SetType(MessageEventType type)
        {
            this.type = type;
            return this;
        }
        internal MessageEventBuilder SetMessageId(ulong messageId)
        {
            this.messageId = messageId;
            return this;
        }
        public MessageEventBuilder SetUncompressedMessageSize(ulong uncompressedMessageSize)
        {
            this.uncompressedMessageSize = uncompressedMessageSize;
            return this;
        }
        public MessageEventBuilder SetCompressedMessageSize(ulong compressedMessageSize)
        {
            this.compressedMessageSize = compressedMessageSize;
            return this;
        }

        public IMessageEvent Build()
        {
            string missing = "";
            if (!type.HasValue)
            {
                missing += " type";
            }
            if (!this.messageId.HasValue)
            {
                missing += " messageId";
            }
            if (!this.uncompressedMessageSize.HasValue)
            {
                missing += " uncompressedMessageSize";
            }
            if (!this.compressedMessageSize.HasValue)
            {
                missing += " compressedMessageSize";
            }
            if (!string.IsNullOrEmpty(missing))
            {
                throw new ArgumentOutOfRangeException("Missing required properties:" + missing);
            }
            return new MessageEvent(
                this.type.Value,
                this.messageId.Value,
                this.uncompressedMessageSize.Value,
                this.compressedMessageSize.Value);
        }
    }
}
