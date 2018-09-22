
namespace OpenCensus.Exporter.Stackdriver.Implementation
{
    using System.Collections.Generic;
    using System.Linq;
    using Google.Cloud.Trace.V2;
    using OpenCensus.Trace;
    using OpenCensus.Trace.Export;

    static class SpanExtensions
    {
        public static Span ToSpan(this ISpanData spanData)
        {
            return new Span
            {
                Attributes =
                {
                    DroppedAttributesCount = spanData.Attributes.DroppedAttributesCount,
                    AttributeMap = { spanData.Attributes.AttributeMap.ToDictionary(
                                        s => s.Key, 
                                        s => s.Value.ToAttributeValue()) },
                },
            };
        }

        public static AttributeValue ToAttributeValue(this IAttributeValue av)
        {
            // TODO Currently we assume we store only strings.
            return new AttributeValue
            {
                StringValue = new TruncatableString
                {
                    Value = av.Match(
                    s => s,
                    b => b.ToString(),
                    l => l.ToString(),
                    obj => obj.ToString())
                }
            };
        }
    }

    /// <summary>
    /// Exports a group of spans to Stackdriver
    /// </summary>
    class StackdriverTraceExporter : IHandler
    {
        private Google.Api.Gax.ResourceNames.ProjectName googleCloudProjectId;

        public StackdriverTraceExporter(string projectId)
        {
            googleCloudProjectId = new Google.Api.Gax.ResourceNames.ProjectName(projectId); 
        }

        public void Export(IList<ISpanData> spanDataList)
        {
            TraceServiceClient traceWriter = TraceServiceClient.Create();
            var batchSpansRequest = new BatchWriteSpansRequest
            {
                ProjectName = googleCloudProjectId,
                Spans = { spanDataList.Select(s => s.ToSpan()) },
            };

            traceWriter.BatchWriteSpans(batchSpansRequest);
        }
    }
}
