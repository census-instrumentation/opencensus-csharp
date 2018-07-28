using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Tags.Propagation
{
    public abstract class TagContextBinarySerializerBase : ITagContextBinarySerializer
    {
        public abstract ITagContext FromByteArray(byte[] bytes);
        public abstract byte[] ToByteArray(ITagContext tags);
    }
}
