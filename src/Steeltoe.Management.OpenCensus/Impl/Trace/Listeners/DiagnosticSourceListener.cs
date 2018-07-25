using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Steeltoe.Management.Census.Impl.Trace.Listeners
{
    public class DiagnosticSourceListener : IObserver<KeyValuePair<string, object>>, IDisposable
    {
        private readonly string sourceName;
        private readonly ListenerHandler handler;

        public IDisposable Subscription { get; set; }

        public DiagnosticSourceListener(string sourceName)
        {
            this.sourceName = sourceName;
            this.handler = ListenerHandlerFactory.GetHandler(sourceName);
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(KeyValuePair<string, object> value)
        {
            if (Activity.Current == null)
            {
                Debug.WriteLine("Activity is null");
                return;
            }

            try
            {
                if (value.Key.EndsWith("Start"))
                {
                    handler.OnStartActivity(Activity.Current, value.Value);
                }
                else if (value.Key.EndsWith("Stop"))
                {
                    handler.OnStopActivity(Activity.Current, value.Value);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        public void Dispose()
        {
            Subscription?.Dispose();
        }
    }
}