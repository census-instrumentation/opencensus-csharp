
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

            // Base span settings
            var span = new Span
            {
                SpanName = new SpanName(projectId, spanData.Context.TraceId.ToLowerBase16(), spanId),
                SpanId = spanId,
                DisplayName = new TruncatableString { Value = spanData.Name },
                StartTime = spanData.StartTimestamp.ToTimestamp(),
                EndTime = spanData.EndTimestamp.ToTimestamp(),
                ChildSpanCount = spanData.ChildSpanCount,
            };
            if (spanData.ParentSpanId != null)
            {
                string parentSpanId = spanData.ParentSpanId.ToLowerBase16();
                if (!string.IsNullOrEmpty(parentSpanId))
                {
                    span.ParentSpanId = parentSpanId;
                }
            }

            // Span Links
            if (spanData.Links != null)
            {
                span.Links = new Span.Types.Links
                {
                    DroppedLinksCount = spanData.Links.DroppedLinksCount,
                    Link = { spanData.Links.Links.Select(l => l.ToLink()) }
                };
            }

            // Span Attributes
            if (spanData.Attributes != null)
            {
                span.Attributes = new Span.Types.Attributes
                {
                    DroppedAttributesCount = spanData.Attributes != null ? spanData.Attributes.DroppedAttributesCount : 0,

                    AttributeMap = { spanData.Attributes?.AttributeMap?.ToDictionary(
                                        s => s.Key,
                                        s => s.Value?.ToAttributeValue()) },
                };
            }

            return span;
        }

        public static Span.Types.Link ToLink(this ILink link)
        {
            var ret = new Span.Types.Link();
            ret.SpanId = link.SpanId.ToLowerBase16();
            ret.TraceId = link.TraceId.ToLowerBase16();
            
            if (link.Attributes != null)
            {
                ret.Attributes = new Span.Types.Attributes
                {
                    
                    DroppedAttributesCount = OpenCensus.Trace.Config.TraceParams.DEFAULT.MaxNumberOfAttributes - link.Attributes.Count,

                    AttributeMap = { link.Attributes.ToDictionary(
                         att => att.Key,
                         att => att.Value.ToAttributeValue()) }
                };
            }

            return ret;
        }

        public static Google.Cloud.Trace.V2.AttributeValue ToAttributeValue(this IAttributeValue av)
        {
            var ret = new Google.Cloud.Trace.V2.AttributeValue();
            var attributeType = av.GetType();

            // Handle all primitive types
            if (attributeType == typeof(AttributeValue<bool>))
            {
                ret.BoolValue = ((AttributeValue<bool>)av).Value;
            }
            else if (attributeType == typeof(AttributeValue<long>))
            {
                ret.IntValue = ((AttributeValue<long>)av).Value;
            }
            else // String or anything else is written as string
            {
                ret.StringValue = new TruncatableString()
                {
                    Value = av.Match(
                        s => s,
                        b => b.ToString(),
                        l => l.ToString(),
                        d => d.ToString(),
                        o => o.ToString())
                };
            }

            return ret;
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
