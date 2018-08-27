# OpenCensus .NET SDK - distributed tracing and stats collection framework

[![Gitter chat][gitter-image]][gitter-url]

MyGet: [![MyGet Nightly][opencensus-myget-image]][opencensus-myget-url]

OpenCensus is a toolkit for collecting application performance and behavior
data. It currently includes 3 APIs: stats, tracing and tags.

The library is in [Beta](#versioning) stage and APIs are expected to be mostly
stable. The library is expected to move to [GA](#versioning) stage after v1.0.0
major release.

Please join [gitter](https://gitter.im/census-instrumentation/Lobby) for help
or feedback on this project.

We encourage contributions. Use tags
[up-for-grabs](https://github.com/census-instrumentation/opencensus-csharp/issues?q=is%3Aissue+is%3Aopen+label%3Aup-for-grabs)
and [good first
issue](https://github.com/census-instrumentation/opencensus-csharp/issues?q=is%3Aissue+is%3Aopen+label%3A%22good+first+issue%22)
to get started with the project. Follow [CONTRIBUTING](CONTRIBUTING.md) guide
to report issues or submit a proposal.

## OpenCensus Quickstart

### Using Zipkin exporter

Configure Zipkin exporter to see traces in Zipkin UI.

1. Get Zipkin using getting [started
   guide](https://zipkin.io/pages/quickstart.html).
2. Start `ZipkinTraceExporter` as below:

``` csharp
var exporter = new ZipkinTraceExporter(
  new ZipkinTraceExporterOptions() {
    Endpoint = "https://<zipkin-server:9411>/api/v2/spans",
    ServiceName = typeof(Program).Assembly.GetName().Name,
  },
  Tracing.ExportComponent);
exporter.Start();

var span = tracer.SpanBuilder("incoming request").SetSampler(Samplers.AlwaysSample).StartSpan();
Thread.Sleep(TimeSpan.FromSeconds(1));
span.End();
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
[gitter-url]:
https://gitter.im/census-instrumentation/lobby?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge
[opencensus-myget-image]:
https://img.shields.io/myget/opencensus/vpre/OpenCensus.svg
[opencensus-myget-url]:
https://www.myget.org/feed/opencensus/package/nuget/OpenCensus [semver]:
http://semver.org/