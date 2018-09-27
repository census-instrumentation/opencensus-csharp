
namespace OpenCensus.Exporter.Stackdriver.Implementation
{
    using System.Collections.Generic;
    using System.Linq;
    using Google.Cloud.Trace.V2;
    using OpenCensus.Exporter.Stackdriver.Utils;
    using OpenCensus.Trace;
    using OpenCensus.Trace.Export;

    static class SpanExtensions
    {
        /// <summary>
        /// Translating <see cref="ISpanData"/> to Stackdriver's Span
        /// According to <see cref="https://cloud.google.com/trace/docs/reference/v2/rpc/google.devtools.cloudtrace.v2"/> specifications
        /// </summary>
        /// <param name="spanData">Span in OpenCensus format</param>
        /// <param name="projectId">Google Cloud Platform Project Id</param>
        /// <returns></returns>
        public static Span ToSpan(this ISpanData spanData, string projectId)
        {
            string spanId = spanData.Context.SpanId.ToLowerBase16();
            var span = new Span
            {
                Name = string.Format($"project/{projectId}/traces/{spanData.Context.TraceId.ToLowerBase16()}/spans/{spanId}"),
                SpanId = spanId,
                DisplayName = new TruncatableString { Value = spanData.Name },
                StartTime = spanData.StartTimestamp.ToTimestamp(),
                EndTime = spanData.EndTimestamp.ToTimestamp(),
                ChildSpanCount = spanData.ChildSpanCount,
                /*
                Attributes =
                {
                    DroppedAttributesCount = spanData.Attributes != null ? spanData.Attributes.DroppedAttributesCount : 0,
                    AttributeMap = { spanData.Attributes?.AttributeMap?.ToDictionary(
                                        s => s.Key, 
                                        s => s.Value?.ToAttributeValue()) },
                },*/
            };

            if (spanData.ParentSpanId != null)
            {
                string parentSpanId = spanData.ParentSpanId.ToLowerBase16();
                if (!string.IsNullOrEmpty(parentSpanId))
                {
                    span.ParentSpanId = parentSpanId;
                }
            }

            return span;
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
                Spans = { spanDataList.Select(s => s.ToSpan(googleCloudProjectId.ProjectId)) },
            };

            traceWriter.BatchWriteSpans(batchSpansRequest);
        }
    }
}
