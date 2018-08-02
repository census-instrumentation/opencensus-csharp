using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Stats
{
    public interface IViewManager
    {
        ISet<IView> AllExportedViews { get; }

        IViewData GetView(IViewName view);

        void RegisterView(IView view);
    }
}
