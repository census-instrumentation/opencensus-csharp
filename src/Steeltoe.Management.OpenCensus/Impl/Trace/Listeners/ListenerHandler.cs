using System;
using System.Diagnostics;
using Steeltoe.Management.Census.Trace;

namespace Steeltoe.Management.Census.Impl.Trace.Listeners
{
    internal class ListenerHandler
    {
        private readonly string sourceName;
        //private readonly Tracer tracer;

        public ListenerHandler(string sourceName)
        {
            this.sourceName = sourceName;
        }

        public virtual void OnStartActivity(Activity activity, object payload)
        {
            Console.WriteLine($"Activity started {sourceName} {activity.OperationName} {activity.Id}");
        }

        public virtual void OnStopActivity(Activity activity, object payload)
        {
            Console.WriteLine($"Activity stopped {sourceName} {activity.OperationName} {activity.Id} {activity.Duration}");
        }
    }
}
