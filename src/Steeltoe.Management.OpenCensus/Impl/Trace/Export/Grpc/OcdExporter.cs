using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Core.Utils;
using Opencensus.Proto.Exporter;
using Opencensus.Proto.Trace;
using Steeltoe.Management.Census.Trace;
using Steeltoe.Management.Census.Trace.Export;
using AttributeValue = Opencensus.Proto.Trace.AttributeValue;
using Span = Opencensus.Proto.Trace.Span;

namespace Steeltoe.Management.Census.Impl.Trace.Export.Grpc
{
    public class OcdHandler : IHandler, IDisposable
    {
        private readonly Channel channel;
        private readonly Opencensus.Proto.Exporter.Export.ExportClient client;

        public OcdHandler(string endpoint, ChannelCredentials credentials)
        {
            channel = new Channel(endpoint, credentials);
            client = new Opencensus.Proto.Exporter.Export.ExportClient(channel);
        }

        public Task ExportAsync(IList<ISpanData> spanDataList)
        {
            var spanExport = new ExportSpanRequest();
            spanExport.Spans.AddRange(spanDataList.Select(FromISpanData));
            var reply = client.ExportSpan();
            return reply.RequestStream.WriteAllAsync(new[] { spanExport });
        }

        public void Dispose()
        {
            channel.ShutdownAsync().Wait();
        }

        private Span FromISpanData(ISpanData source)
        {
            return new Span
            {
                // TODO: kind
                Kind = source.HasRemoteParent.GetValueOrDefault() ? Span.Types.SpanKind.Server :
                    source.ParentSpanId != null ? Span.Types.SpanKind.Client : Span.Types.SpanKind.Unspecified,
                StartTime = new Timestamp
                {
                    Nanos = source.StartTimestamp.Nanos,
                    Seconds = source.StartTimestamp.Seconds
                },
                EndTime = new Timestamp
                {
                    Nanos = source.EndTimestamp.Nanos,
                    Seconds = source.EndTimestamp.Seconds
                },
                ChildSpanCount = source.ChildSpanCount,
                Name = new TruncatableString {Value = source.Name},
                ParentSpanId = ByteString.CopyFrom(source.ParentSpanId?.Bytes ?? new byte [0]),
                SameProcessAsParentSpan = !source.HasRemoteParent.GetValueOrDefault() && source.ParentSpanId != null,
                TraceId = ByteString.CopyFrom(source.Context.TraceId.Bytes),
                SpanId = ByteString.CopyFrom(source.Context.SpanId.Bytes),
                Status = new Opencensus.Proto.Trace.Status
                {
                    Code = (int) source.Status.CanonicalCode,
                    Message = source.Status.Description
                },
                Links = new Span.Types.Links
                {
                    DroppedLinksCount = source.Links.DroppedLinksCount,
                    Link = {source.Links.Links.Select(FromILink) }
                },
                Attributes = FromIAttributes(source.Attributes),
                TimeEvents = FromITimeEvents(source.MessageEvents, source.Annotations)
            };
        }

        private Span.Types.Link FromILink(ILink source)
        {
            return new Span.Types.Link
            {
                TraceId = ByteString.CopyFrom(source.TraceId.Bytes),
                SpanId = ByteString.CopyFrom(source.SpanId.Bytes),
                Type = source.Type == LinkType.CHILD_LINKED_SPAN ? Span.Types.Link.Types.Type.ChildLinkedSpan : Span.Types.Link.Types.Type.ParentLinkedSpan,
                Attributes = FromIAttributeMap(source.Attributes)
            };
        }

        private Span.Types.TimeEvents FromITimeEvents(ITimedEvents<IMessageEvent> messages, ITimedEvents<IAnnotation> annotations)
        {
            var timedEvents = new Span.Types.TimeEvents
            {
                DroppedMessageEventsCount = messages.DroppedEventsCount,
                DroppedAnnotationsCount = annotations.DroppedEventsCount,
                TimeEvent = { messages.Events.Select(FromITimeEvent) }
            };

            timedEvents.TimeEvent.AddRange(annotations.Events.Select(FromITimeEvent));

            return timedEvents;
        }

        private Span.Types.TimeEvent FromITimeEvent(ITimedEvent<IMessageEvent> source)
        {
            return new Span.Types.TimeEvent
            {
                Time = new Timestamp
                {
                    Nanos = source.Timestamp.Nanos,
                    Seconds = source.Timestamp.Seconds
                },
                MessageEvent = new Span.Types.TimeEvent.Types.MessageEvent()
                {
                    Type = source.Event.Type == MessageEventType.SENT ? Span.Types.TimeEvent.Types.MessageEvent.Types.Type.Sent : Span.Types.TimeEvent.Types.MessageEvent.Types.Type.Received,
                    CompressedSize = source.Event.CompressedMessageSize,
                    UncompressedSize = source.Event.UncompressedMessageSize,
                    Id = source.Event.MessageId
                }
            };
        }

        private Span.Types.TimeEvent FromITimeEvent(ITimedEvent<IAnnotation> source)
        {
            return new Span.Types.TimeEvent
            {
                Time = new Timestamp
                {
                    Nanos = source.Timestamp.Nanos,
                    Seconds = source.Timestamp.Seconds
                },
                Annotation = new Span.Types.TimeEvent.Types.Annotation
                {
                    Description = new TruncatableString { Value = source.Event.Description },
                    Attributes = FromIAttributeMap(source.Event.Attributes)
                }
            };
        }

        private Span.Types.Attributes FromIAttributes(IAttributes source)
        {
            var attributes = new Span.Types.Attributes
            {
                DroppedAttributesCount = source.DroppedAttributesCount,
            };


            attributes.AttributeMap.Add(source.AttributeMap.ToDictionary(kvp => kvp.Key,
                kvp => new AttributeValue
                    {
                        StringValue = new TruncatableString { Value = kvp.Value.Match(s => s, b => b.ToString(), l => l.ToString(), d => d?.ToString()) }
                    }));

            return attributes;
        }

        private Span.Types.Attributes FromIAttributeMap(IDictionary<string, IAttributeValue> source)
        {
            var attributes = new Span.Types.Attributes();

            attributes.AttributeMap.Add(source.ToDictionary(kvp => kvp.Key,
                kvp => new AttributeValue
                {
                    StringValue = new TruncatableString { Value = kvp.Value.Match(s => s, b => b.ToString(), l => l.ToString(), d => d?.ToString()) }
                }));

            return attributes;
        }
    }
}
