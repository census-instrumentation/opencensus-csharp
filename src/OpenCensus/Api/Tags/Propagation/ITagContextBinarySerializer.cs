using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Tags.Propagation
{
    public interface ITagContextBinarySerializer
    {
        byte[] ToByteArray(ITagContext tags);

        ITagContext FromByteArray(byte[] bytes);
    }
}
