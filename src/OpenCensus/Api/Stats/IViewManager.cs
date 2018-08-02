namespace OpenCensus.Stats
{
    using System.Collections.Generic;

    public interface IViewManager
    {
        ISet<IView> AllExportedViews { get; }

        IViewData GetView(IViewName view);

        void RegisterView(IView view);
    }
}
