# Changelog

Please update changelog as part of any significant pull request. Place short
description of your change into "Unreleased" section. As part of release
process content of "Unreleased" section content will generate release notes for
the release.

## Unreleased

- ASP.NET Core collector now uses `http.route` for the span name.
- Initial implementation of Resource Specification.

## 0.1.0-alpha-33381

Released
[12/18/2018](https://github.com/census-instrumentation/opencensus-csharp/releases/tag/0.1.0-alpha-33381).

- Collectors for ASP.NET Core and .NET Core HttpClient.
- Initial version of Ocagent exporter implemented.
- Initial version of StackDriver exporter implemented.
- Support double attributes according to the [spec
  change](https://github.com/census-instrumentation/opencensus-specs/issues/172).
- Initial implementation of Prometheus exporter.
- Initial version of Application Insights exporter implemented.
- Zipkin exporter implemented.
- Initial version of SDK published. It is based on contribution from Pivotal
  [from](https://github.com/SteeltoeOSS/Management/tree/dev/src/Steeltoe.Management.OpenCensus).