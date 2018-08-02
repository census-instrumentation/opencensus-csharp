namespace OpenCensus.Trace.Export
{
    using System.Collections.Generic;

    public interface IHandler
    {
        void Export(IList<ISpanData> spanDataList);
    }
}
