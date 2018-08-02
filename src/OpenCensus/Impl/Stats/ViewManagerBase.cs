namespace OpenCensus.Stats
{
    using System.Collections.Generic;

    public abstract class ViewManagerBase : IViewManager
    {
        public abstract ISet<IView> AllExportedViews { get; }

        public abstract IViewData GetView(IViewName view);

        public abstract void RegisterView(IView view);
    }
}
