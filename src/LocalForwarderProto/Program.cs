using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;
using Grpc.Core.Utils;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Opencensus.Proto.Exporter;
using Opencensus.Proto.Trace;

namespace LocalForwarderProto
{
    class ExportImpl : Export.ExportBase
    {
        private const string LINK_PROPERTY_NAME = "link";
        private const string LINK_SPAN_ID_PROPERTY_NAME = "spanId";
        private const string LINK_TRACE_ID_PROPERTY_NAME = "traceId";
        private const string LINK_TYPE_PROPERTY_NAME = "type";
        private readonly TelemetryClient telemetryClient = new TelemetryClient();
        public override async Task ExportSpan(IAsyncStreamReader<ExportSpanRequest> requestStream, IServerStreamWriter<ExportSpanResponse> responseStream, ServerCallContext context)
        {
            await requestStream.ForEachAsync(spanRequest =>
            {
                foreach (var span in spanRequest.Spans)
                {
                    TrackSpan(span);
                }
                return Task.CompletedTask;
            });
        }

        private void TrackSpan(Span span)
        {
            if (span.Kind == Span.Types.SpanKind.Client ||
                (span.Kind == Span.Types.SpanKind.Unspecified && span.SameProcessAsParentSpan.GetValueOrDefault()))
            {
                TrackDependencyFromSpan(span);
            }
            else
            {
                TrackRequestFromSpan(span);
            }

            foreach (var evnt in span.TimeEvents.TimeEvent)
            {
                TrackTraceFromTimeEvent(evnt, span);
            }
        }

        private void TrackRequestFromSpan(Span span)
        {
            RequestTelemetry request = new RequestTelemetry();
            SetOperationContext(span, request.Context.Operation);

            //TODO:
            request.Id = ToStr(span.SpanId);
            request.Timestamp = span.StartTime.ToDateTime();
            request.Duration = span.EndTime.ToDateTime() - request.Timestamp;
            request.Success = span.Status.Code == 0;

            string host = null;
            string method = null;
            string path = null;
            string route = null;
            int port = -1;
            bool isResultSet = false;

            foreach (var attribute in span.Attributes.AttributeMap)
            {
                switch (attribute.Key)
                {
                    case "http.status_code":
                        request.ResponseCode = attribute.Value.StringValue.Value;
                        isResultSet = true;
                        break;
                    case "http.user_agent":
                        request.Context.User.UserAgent = attribute.Value.StringValue.Value;
                        break;
                    case "http.route":
                        route = attribute.Value.StringValue.Value;
                        break;
                    case "http.path":
                        path = attribute.Value.StringValue.Value;
                        break;
                    case "http.method":
                        method = attribute.Value.StringValue.Value;
                        break;
                    case "http.host":
                        host = attribute.Value.StringValue.Value;
                        break;
                    case "http.port":
                        port = (int)attribute.Value.IntValue;
                        break;
                    default:
                        if (!request.Properties.ContainsKey(attribute.Key))
                        {
                            request.Properties[attribute.Key] = attribute.Value.StringValue.Value;
                        }

                        break;
                }

                if (host != null)
                {
                    request.Url = GetUrl(host, port, path);
                    request.Name = $"{method} {route ?? path}";
                }
                else
                { // perhaps not http
                    request.Name = span.Name.Value;
                }

                if (!isResultSet)
                {
                    request.ResponseCode = span.Status.Message;
                }
            }

            SetLinks(span.Links, request.Properties);

            telemetryClient.TrackRequest(request);
        }

        private void TrackDependencyFromSpan(Span span)
        {
            String host = null;
            if (span.Attributes.AttributeMap.ContainsKey("http.host"))
            {
                host = span.Attributes.AttributeMap["http.host"].StringValue.Value;
                if (IsApplicationInsightsUrl(host))
                {
                    return;
                }
            }

            DependencyTelemetry dependency = new DependencyTelemetry();
            SetOperationContext(span, dependency.Context.Operation);

            dependency.Id = ToStr(span.SpanId);
            dependency.Timestamp = span.StartTime.ToDateTime();
            dependency.Duration = span.EndTime.ToDateTime() - dependency.Timestamp;
            dependency.Success = span.Status.Code == 0;

            dependency.ResultCode = span.Status.Message;

            string method = null;
            string path = null;
            int port = -1;

            bool isHttp = false;
            bool isResultSet = false;
            foreach (var attribute in span.Attributes.AttributeMap)
            {
                switch (attribute.Key)
                {
                    case "http.status_code":
                        dependency.ResultCode = attribute.Value.StringValue.Value;
                        isHttp = true;
                        isResultSet = true;
                        break;
                    case "http.path":
                        path = attribute.Value.StringValue.Value;
                        isHttp = true;
                        break;
                    case "http.method":
                        method = attribute.Value.StringValue.Value;
                        isHttp = true;
                        break;
                    case "http.host":
                        break;
                    case "http.port":
                        port = (int)attribute.Value.IntValue;
                        break;
                    default:
                        if (!dependency.Properties.ContainsKey(attribute.Key))
                        {
                            dependency.Properties[attribute.Key] = attribute.Value.StringValue.Value;
                        }

                        break;
                }
            }

            dependency.Target = host;
            if (isHttp)
            {
                dependency.Type = "HTTP";
            }

            if (!isResultSet)
            {
                dependency.ResultCode = span.Status.Message;
            }

            if (host != null)
            {
                dependency.Data = GetUrl(host, port, path).ToString();
            }

            if (method != null && path != null)
            {
                dependency.Name= $"{method} {path}";
            }
            else
            {
                dependency.Name = span.Name.Value;
            }

            SetLinks(span.Links, dependency.Properties);

            telemetryClient.TrackDependency(dependency);
        }

        private bool IsApplicationInsightsUrl(string host)
        {
            return host.StartsWith("dc.services.visualstudio.com")
                   || host.StartsWith("rt.services.visualstudio.com");
        }

        private void TrackTraceFromTimeEvent(Span.Types.TimeEvent evnt, Span span)
        {
            Span.Types.TimeEvent.Types.Annotation annotation = evnt.Annotation;
            if (annotation != null)
            {
                TraceTelemetry trace = new TraceTelemetry();
                SetParentOperationContext(span, trace.Context.Operation);
                trace.Timestamp = evnt.Time.ToDateTime();

                trace.Message = annotation.Description.Value;
                SetAttributes(annotation.Attributes.AttributeMap, trace.Properties);
                telemetryClient.TrackTrace(trace);
            }

            Span.Types.TimeEvent.Types.MessageEvent message = evnt.MessageEvent;
            if (message != null)
            {
                TraceTelemetry trace = new TraceTelemetry();
                SetParentOperationContext(span, trace.Context.Operation);
                trace.Timestamp = evnt.Time.ToDateTime();

                trace.Message = $"MessageEvent. messageId: '{message.Id}', type: '{message.Type}', compressed size: '{message.CompressedSize}', uncompressed size: '{message.UncompressedSize}'";
                telemetryClient.TrackTrace(trace);
            }
        }

        private void SetOperationContext(Span span, OperationContext context)
        {
            context.Id = ToStr(span.TraceId);
            context.ParentId = ToStr(span.ParentSpanId);
        }

        private void SetParentOperationContext(Span span, OperationContext context)
        {
            context.Id = ToStr(span.TraceId);
            context.ParentId = ToStr(span.SpanId);
        }

        private static string ToStr(ByteString str)
        {
            return BitConverter.ToString(str.ToByteArray()).Replace("-", "").ToLower();
        }

        private Uri GetUrl(String host, int port, String path)
        {
            // todo: better way to determine scheme?
            String schema = port == 80 ? "http" : "https";
            if (port == 80 || port == 443)
            {
                return new Uri(string.Format("{0}://{1}{2}", schema, host, path));
            }

            return new Uri($"{schema}://{host}:{port}{path}");
        }

        private void SetLinks(Span.Types.Links spanLinks, IDictionary<string, string> telemetryProperties)
        {
            // for now, we just put links to telemetry properties
            // link0_spanId = ...
            // link0_traceId = ...
            // link0_type = child | parent | other
            // link0_<attributeKey> = <attributeValue>
            // this is not convenient for querying data
            // We'll consider adding Links to operation telemetry schema

            int num = 0;
            foreach (var link in spanLinks.Link)
            {
                string prefix = $"{LINK_PROPERTY_NAME}{num++}_";
                telemetryProperties[prefix + LINK_SPAN_ID_PROPERTY_NAME] = ToStr(link.SpanId);
                telemetryProperties[prefix + LINK_TRACE_ID_PROPERTY_NAME] = ToStr(link.TraceId);
                telemetryProperties[prefix + LINK_TYPE_PROPERTY_NAME] = link.Type.ToString();

                foreach (var attribute in link.Attributes.AttributeMap)
                {
                    if (!telemetryProperties.ContainsKey(attribute.Key))
                    {
                        telemetryProperties[attribute.Key] = attribute.Value.StringValue.Value;
                    }
                }
            }
        }

        private void SetAttributes(IDictionary<string, AttributeValue> attributes, IDictionary<string, string> telemetryProperties)
        {
            foreach (var attribute in attributes)
            {
                if (!telemetryProperties.ContainsKey(attribute.Key))
                {
                    telemetryProperties[attribute.Key] = attribute.Value.StringValue.Value;
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            TelemetryConfiguration.Active.InstrumentationKey = "83b2d986-e491-489e-bf90-35dea15f9df2";
            Server server = new Server
            {
                Services = { Export.BindService(new ExportImpl()) },
                Ports = { new ServerPort("localhost", 50051, ServerCredentials.Insecure) }
            };
            server.Start();

            Console.WriteLine("Greeter server listening on port " + 50051);
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();

            server.ShutdownAsync().Wait();
        }
    }
}
