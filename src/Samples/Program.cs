namespace Samples
{
    using CommandLine;
    using System;

    [Verb("stackdriver", HelpText = "Specify the options required to test Stackdriver exporter", Hidden = false)]
    class StackdriverOptions
    {
        [Option('p', "projectId", HelpText = "Please specify the projectId of your GCP project", Required = true)]
        public string ProjectId { get; set; }
    }

    [Verb("zipkin", HelpText = "Specify the options required to test Zipkin exporter")]
    class ZipkinOptions
    {
    }

    [Verb("appInsights", HelpText = "Specify the options required to test ApplicationInsights")]
    class ApplicationInsightsOptions
    {
    }

    [Verb("prometheus", HelpText = "Specify the options required to test Prometheus")]
    class PrometheusOptions
    {
    }

    [Verb("httpclient", HelpText = "Specify the options required to test HttpClient")]
    class HttpClientOptions
    {
    }

    /// <summary>
    /// Main samples entry point.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main method - invoke this using command line.
        /// For example:
        /// 
        /// Samples.dll zipkin
        /// Sample.dll appInsights
        /// Sample.dll prometheus
        /// </summary>
        /// <param name="args">Arguments from command line.</param>
        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<ZipkinOptions, ApplicationInsightsOptions, PrometheusOptions, HttpClientOptions, StackdriverOptions>(args)
                .MapResult(
                    (ZipkinOptions options) => TestZipkin.Run(),
                    (ApplicationInsightsOptions options) => TestApplicationInsights.Run(),
                    (PrometheusOptions options) => TestPrometheus.Run(),
                    (HttpClientOptions options) => TestHttpClient.Run(),
                    (StackdriverOptions options) => TestStackdriver.Run(options.ProjectId),
                    errs => 1);
                    
            // TestZipkin.Run();
            // TestApplicationInsights.Run();
            // TestPrometheus.Run();
            // TestHttpClient.Run();
        }
    }
}
