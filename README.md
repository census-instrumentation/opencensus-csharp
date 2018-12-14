# OpenCensus .NET SDK - distributed tracing and stats collection framework

[![Gitter chat][gitter-image]][gitter-url]
[![Build Status](https://opencensus.visualstudio.com/continuous-integration/_apis/build/status/ci-myget-update.yml)](https://opencensus.visualstudio.com/continuous-integration/_build/latest?definitionId=3)

| Source | OpenCensus                                                       | Zipkin Exporter                                                                                  | Application Insights exporter                                                            | Stackdriver                                                       |
|--------|------------------------------------------------------------------|--------------------------------------------------------------------------------------------------|------------------------------------------------------------------------------------------|------------------------------------------------------------------|
| MyGet  | [![MyGet Nightly][opencensus-myget-image]][opencensus-myget-url] | [![MyGet Nightly][opencensus-exporter-zipkin-myget-image]][opencensus-exporter-zipkin-myget-url] | [![MyGet Nightly][opencensus-exporter-ai-myget-image]][opencensus-exporter-ai-myget-url] |
 [![MyGet Nightly][opencensus-exporter-stackdriver-myget-image]][opencensus-exporter-stackdriver-myget-url]

OpenCensus is a toolkit for collecting application performance and behavior
data. It currently includes 3 APIs: stats, tracing and tags.

The library is in [Beta](#versioning) stage and APIs are expected to be mostly
stable. The library is expected to move to [GA](#versioning) stage after v1.0.0
major release.

Please join [gitter](https://gitter.im/census-instrumentation/Lobby) for help
or feedback on this project.

We encourage contributions. Use tags [up-for-grabs][up-for-grabs-issues] and
[good first issue][good-first-issues] to get started with the project. Follow
[CONTRIBUTING](CONTRIBUTING.md) guide to report issues or submit a proposal.

## OpenCensus Quickstart

### Using Zipkin exporter

Configure Zipkin exporter to see traces in Zipkin UI.

1. Get Zipkin using [getting started guide][zipkin-get-started].
2. Start `ZipkinTraceExporter` as below:
3. See [sample][zipkin-sample] for example use.

``` csharp
var exporter = new ZipkinTraceExporter(
  new ZipkinTraceExporterOptions() {
    Endpoint = new Uri("https://<zipkin-server:9411>/api/v2/spans"),
    ServiceName = typeof(Program).Assembly.GetName().Name,
  },
  Tracing.ExportComponent);
exporter.Start();

var span = tracer
            .SpanBuilder("incoming request")
            .SetSampler(Samplers.AlwaysSample)
            .StartSpan();

Thread.Sleep(TimeSpan.FromSeconds(1));
span.End();
```

### Using Prometheus exporter

Configure Prometheus exporter to have stats collected by Prometheus.

1. Get Prometheus using [getting started guide][prometheus-get-started].
2. Start `PrometheusExporter` as below.
3. See [sample][prometheus-sample] for example use.

``` csharp
var exporter = new PrometheusExporter(
    new PrometheusExporterOptions()
    {
        Url = new Uri("http://localhost:9184/metrics/")
    },
    Stats.ViewManager);

exporter.Start();

try
{
    // record metrics
    statsRecorder.NewMeasureMap().Put(VideoSize, values[0] * MiB).Record();
}
finally
{
    exporter.Stop();
}
```

### Using Stackdriver Exporter

1. Enable [Stackdriver Trace][stackdriver-setup] resource.
2. Instantiate a new instance of `StackdriverExporter` with your Google Cloud's ProjectId
3. See [sample][stackdriver-sample] for example use.

``` csharp
var exporter = new StackdriverExporter("YOUR-GOOGLE-PROJECT-ID", Tracing.ExportComponent);
exporter.Start();
```

### Using Application Insights exporter

1. Create [Application Insights][ai-get-started] resource.
2. Set instrumentation key via telemetry configuration object
   (`new TelemetryConfiguration("iKey")`). This object may be injected via
   dependency injection as well.
3. Instantiate a new instance of `ApplicationInsightsExporter`.
4. See [sample][ai-sample] for example use.

``` csharp
var config = new TelemetryConfiguration("iKey")
var exporter = new ApplicationInsightsExporter(
    Tracing.ExportComponent,
    Stats.ViewManager,
    config); // either global or local config can be used
exporter.Start();
```

## Versioning
  
This library follows [Semantic Versioning][semver].
  
**GA**: Libraries defined at a GA quality level are stable, and will not
introduce backwards-incompatible changes in any minor or patch releases. We
will address issues and requests with the highest priority. If we were to make
a backwards-incompatible changes on an API, we will first mark the existing API
as deprecated and keep it for 18 months before removing it.
  
**Beta**: Libraries defined at a Beta quality level are expected to be mostly
stable and we're working towards their release candidate. We will address
issues and requests with a higher priority. There may be backwards incompatible
changes in a minor version release, though not in a patch release. If an
element is part of an API that is only meant to be used by exporters or other
opencensus libraries, then there is no deprecation period. Otherwise, we will
deprecate it for 18 months before removing it, if possible.

[gitter-image]: https://badges.gitter.im/census-instrumentation/lobby.svg
[gitter-url]:https://gitter.im/census-instrumentation/lobby?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge
[opencensus-myget-image]:https://img.shields.io/myget/opencensus/vpre/OpenCensus.svg
[opencensus-myget-url]: https://www.myget.org/feed/opencensus/package/nuget/OpenCensus
[opencensus-exporter-zipkin-myget-image]:https://img.shields.io/myget/opencensus/vpre/OpenCensus.Exporter.Zipkin.svg
[opencensus-exporter-zipkin-myget-url]: https://www.myget.org/feed/opencensus/package/nuget/OpenCensus.Exporter.Zipkin
[opencensus-exporter-ai-myget-image]:https://img.shields.io/myget/opencensus/vpre/OpenCensus.Exporter.ApplicationInsights.svg
[opencensus-exporter-ai-myget-url]: https://www.myget.org/feed/opencensus/package/nuget/OpenCensus.Exporter.ApplicationInsights
[opencensus-exporter-stackdriver-myget-image]:https://img.shields.io/myget/opencensus/vpre/OpenCensus.Exporter.Stackdriver.svg
[opencensus-exporter-stackdriver-myget-url]: https://www.myget.org/feed/opencensus/package/nuget/OpenCensus.Exporter.Stackdriver
[up-for-grabs-issues]: https://github.com/census-instrumentation/opencensus-csharp/issues?q=is%3Aissue+is%3Aopen+label%3Aup-for-grabs
[good-first-issues]: https://github.com/census-instrumentation/opencensus-csharp/issues?q=is%3Aissue+is%3Aopen+label%3A%22good+first+issue%22
[zipkin-get-started]: https://zipkin.io/pages/quickstart.html
[ai-get-started]: https://docs.microsoft.com/azure/application-insights
[stackdriver-setup]: https://cloud.google.com/trace/docs/setup/
[semver]: http://semver.org/
[ai-sample]: https://github.com/census-instrumentation/opencensus-csharp/blob/develop/src/Samples/TestApplicationInsights.cs
[stackdriver-sample]: https://github.com/census-instrumentation/opencensus-csharp/blob/develop/src/Samples/TestStackdriver.cs
[zipkin-sample]: https://github.com/census-instrumentation/opencensus-csharp/blob/develop/src/Samples/TestZipkin.cs
[prometheus-get-started]: https://prometheus.io/docs/introduction/first_steps/
[prometheus-sample]: https://github.com/census-instrumentation/opencensus-csharp/blob/develop/src/Samples/TestPrometheus.cs