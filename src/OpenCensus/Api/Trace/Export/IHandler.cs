using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Trace.Export
{
    public interface IHandler
    {
        void Export(IList<ISpanData> spanDataList);
    }
}
