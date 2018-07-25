using System.Collections.Generic;
using System.Threading.Tasks;

namespace Steeltoe.Management.Census.Trace.Export
{
    public interface IHandler
    {
        Task ExportAsync(IList<ISpanData> spanDataList);
    }
}
