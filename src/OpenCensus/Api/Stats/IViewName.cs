using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Stats
{
    public interface IViewName
    {
        string AsString { get; }
    }
}
