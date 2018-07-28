using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Tags
{
    public interface ITagValue
    {
        string AsString { get; }
    }
}
