# Application Insights Exporter

## Notes on questionable decisions

1. Why `Span.Kind` has option to be `Unspecified`? Is it only needed for back
   compatibility or it is valid long term?
2. Span Kind detection logic and relation to `HasRemoteParent` flag. Should
   `HasRemoteParent` define span kind when `Span.Kind` was `Unspecified`?
   What if `HasRemoteParent` is `true` when `Span.Kind` is `Client`?
3. Should attribute `span.kind` be respected and has priority over `Span.Kind`
   field?
4. When `Status` wasn't set - should it be treated as `OK` or `Unknown`?

5. ResultCode and ResponseCode – different for Request and Dependency
6. Why use Canonical code, not the textual representation of it?
7. When http.url is bad formed – should we store it in properties?
8. I don't understand why this concatenation is required for identifiers
9. Should we recover url as https or http?
10. Will url or individual components win when looking at port, host, path?
11. Why start and end time of span are not required fields?
12. Span [name](https://github.com/census-instrumentation/opencensus-proto/blob/ba49f56771b83cff7bea7f34d1236fc139dbc471/src/opencensus/proto/trace/v1/trace.proto#L85-L86) is required. Does it mean that it's not empty?