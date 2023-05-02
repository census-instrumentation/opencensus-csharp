> **Warning**
>
> OpenCensus and OpenTracing have merged to form [OpenTelemetry](https://opentelemetry.io), which serves as the next major version of OpenCensus and OpenTracing.
>
> OpenTelemetry has now reached feature parity with OpenCensus, with tracing and metrics SDKs available in .NET, Golang, Java, NodeJS, and Python. **All OpenCensus Github repositories, except [census-instrumentation/opencensus-python](https://github.com/census-instrumentation/opencensus-python), will be archived on July 31st, 2023**. We encourage users to migrate to OpenTelemetry by this date.
>
> To help you gradually migrate your instrumentation to OpenTelemetry, bridges are available in Java, Go, Python, and JS. [**Read the full blog post to learn more**](https://opentelemetry.io/blog/2023/sunsetting-opencensus/).

# OpenCensus .NET SDK

OpenCensus is a set of libraries for various languages that allow you to
collect application metrics and distributed traces, then transfer the data to a
backend of your choice in real time. This data can be analyzed by developers
and admins to understand the health of the application and debug problems.

After many months of planning, discussion, prototyping, more discussion, and
more planning, OpenTracing and OpenCensus are merging to form OpenTelemetry,
which is now a [CNCF sandbox
project](https://www.cncf.io/blog/2019/05/21/a-brief-history-of-opentelemetry-so-far/).

Please join us building a new and exciting [OpenTelemetry
SDK](https://github.com/open-telemetry/opentelemetry-dotnet/).

[![OpenTelemetryLogo](https://opentelemetry.io/img/logos/opentelemetry-horizontal-color.png)](https://github.com/open-telemetry/opentelemetry-dotnet/)

All the open issues and PRs on this repository will be moved to the new project
or closed. We do not expect any new versions of OpenCensus .NET SDK shipped.

OpenCensus C# has never left the Alpha release stage. Please see this [project
description](PROJECT_DESCRIPTION.md) for details of what was built. If you took
a dependency on OpenCensus C# project - please let us know at .NET gitter
Channel: [![Gitter chat](https://badges.gitter.im/open-telemetry/opentelemetry-dotnet.svg)](https://gitter.im/open-telemetry/opentelemetry-dotnet?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
so we can help with migration.
