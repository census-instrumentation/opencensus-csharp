# OpenCensus .NET SDK - distributed tracing and stats collection framework

[![Gitter chat][gitter-image]][gitter-url]
[![Build Status](https://opencensus.visualstudio.com/continuous-integration/_apis/build/status/ci-myget-update.yml)](https://opencensus.visualstudio.com/continuous-integration/_build/latest?definitionId=3)

| Source | OpenCensus                                                       | Zipkin Exporter                                                                                  | Application Insights exporter                                                            |
|--------|------------------------------------------------------------------|--------------------------------------------------------------------------------------------------|------------------------------------------------------------------------------------------|
| MyGet  | [![MyGet Nightly][opencensus-myget-image]][opencensus-myget-url] | [![MyGet Nightly][opencensus-exporter-zipkin-myget-image]][opencensus-exporter-zipkin-myget-url] | [![MyGet Nightly][opencensus-exporter-ai-myget-image]][opencensus-exporter-ai-myget-url] |


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

### Using Application Insights exporter

1. Create [Application Insights][ai-get-started] resource.
  1. Set instrumentation key via configuration or global settings
     (`TelemetryConfiguration.Active`).
  2. Create a separate instance (`new TelemetryConfiguration("iKey")`)

``` csharp
var exporter = new ApplicationInsightsExporter(
    Tracing.ExportComponent, 
    Stats.ViewManager,
    TelemetryConfiguration.Active); // either global or local config can be used
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
[opencensus-myget-exporter-zipkin-image]:https://img.shields.io/myget/opencensus/vpre/OpenCensus.Exporter.Zipkin.svg
[opencensus-myget-exporter-zipkin-url]: https://www.myget.org/feed/opencensus/package/nuget/OpenCensus.Exporter.Zipkin
[opencensus-myget-exporter-ai-image]:https://img.shields.io/myget/opencensus/vpre/OpenCensus.Exporter.ApplicationInsights.svg
[opencensus-myget-exporter-ai-url]: https://www.myget.org/feed/opencensus/package/nuget/OpenCensus.Exporter.ApplicationInsights
[up-for-grabs-issues]: https://github.com/census-instrumentation/opencensus-csharp/issues?q=is%3Aissue+is%3Aopen+label%3Aup-for-grabs
[good-first-issues]: https://github.com/census-instrumentation/opencensus-csharp/issues?q=is%3Aissue+is%3Aopen+label%3A%22good+first+issue%22
[zipkin-get-started]: https://zipkin.io/pages/quickstart.html
[ai-get-started]: https://docs.microsoft.com/azure/application-insights
[semver]:http://semver.org/