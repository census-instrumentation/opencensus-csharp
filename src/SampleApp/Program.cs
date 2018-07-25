using System;
using System.Threading;
using Grpc.Core;
using Steeltoe.Management.Census.Impl.Trace.Export.Grpc;
using Steeltoe.Management.Census.Trace;
using Steeltoe.Management.Census.Trace.Sampler;
using Status = Steeltoe.Management.Census.Trace.Status;

namespace SampleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var tracingComponent = new TraceComponent();
            tracingComponent.ExportComponent.SpanExporter.RegisterHandler("ocd", new OcdHandler("127.0.0.1:50051", ChannelCredentials.Insecure));

            while (true)
            {
                for (int i = 0; i < 100; i++)
                {
                    var span = tracingComponent.Tracer.SpanBuilder("span").SetSampler(Samplers.AlwaysSample);
                    using (span.StartScopedSpan())
                    {
                        tracingComponent.Tracer.CurrentSpan.AddAnnotation("annotation");
                        tracingComponent.Tracer.CurrentSpan.Status = Status.OK.WithDescription("all good");
                    }
                }

                Thread.Sleep(5000);
            }
        }
    }
}
