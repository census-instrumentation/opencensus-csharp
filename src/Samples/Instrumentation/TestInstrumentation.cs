using System;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.Extensibility;
using OpenCensus.Exporter.Ocagent;
using OpenCensus.Stats;
using OpenCensus.Trace;
using OpenCensus.Trace.Config;
using OpenCensus.Trace.Sampler;
using Samples.Instrumentation;

namespace Samples
{
    internal class TestInstrumentation
    {
        internal static object Run()
        {
            var exporter = new OcagentExporter(Tracing.ExportComponent, 
                "localhost:55678",
                Environment.MachineName,
                "test-app");

            exporter.Start();

            Tracing.TraceConfig.UpdateActiveTraceParams(
                Tracing.TraceConfig.ActiveTraceParams.ToBuilder()
                    .SetSampler(Samplers.AlwaysSample)
                    .Build());

            var sampleClient = new SampleClient("myaccount.storage.vendor.com");

            Console.WriteLine("Starting...");
            for (int i = 0; i < 100; i++)
            {
                // getting data with sample client
                sampleClient.GetAsync(new SampleRequest {ContainerName = $"foo{i}"}).GetAwaiter().GetResult();
            }

            // TODO: we need to make exporter flush on stop
            Task.Delay(TimeSpan.FromSeconds(5)).GetAwaiter().GetResult();

            exporter.Stop();

            Console.WriteLine("Done! Check out traces on the backend");
            return null;
        }
    }
}
