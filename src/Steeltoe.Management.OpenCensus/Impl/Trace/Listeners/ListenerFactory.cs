using System.Collections.Generic;

namespace Steeltoe.Management.Census.Impl.Trace.Listeners
{
    internal static class ListenerHandlerFactory
    {
        private static readonly Dictionary<string, ListenerHandler> KnownHandlers =
            new Dictionary<string, ListenerHandler>();

        public static ListenerHandler GetHandler(string name)
        {
            if (!KnownHandlers.TryGetValue(name, out var handler))
            {
                handler = new ListenerHandler(name);
                KnownHandlers[name] = handler;
            }
            return handler;
        }
    }
}