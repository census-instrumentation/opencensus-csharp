namespace Samples
{
    using System;
    using System.Threading;
    using OpenCensus.Exporter.Zipkin;
    using OpenCensus.Trace;
    using OpenCensus.Trace.Sampler;

    internal class TestZipkin
    {
        private static ITracer tracer = Tracing.Tracer;

        internal static void Run()
        {
            Console.WriteLine("Hello World!");

            var exporter = new ZipkinTraceExporter(
                new ZipkinTraceExporterOptions()
                {
                    Endpoint = new Uri("https://zipkin.azurewebsites.net/api/v2/spans"),
                    ServiceName = typeof(Program).Assembly.GetName().Name,
                },
                Tracing.ExportComponent);
            exporter.Start();

            var span = tracer.SpanBuilder("incoming request").SetSampler(Samplers.AlwaysSample).StartScopedSpan();

            Thread.Sleep(TimeSpan.FromSeconds(1));
            var span2 = tracer.CurrentSpan;
            span2.End();

            Console.ReadLine();
        }
    }
}
