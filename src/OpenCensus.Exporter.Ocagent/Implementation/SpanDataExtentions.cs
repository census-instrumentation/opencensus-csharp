// <copyright file="SpanDataExtentions.cs" company="OpenCensus Authors">
// Copyright 2018, OpenCensus Authors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of theLicense at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

namespace OpenCensus.Exporter.Ocagent.Implementation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Google.Protobuf;
    using Google.Protobuf.WellKnownTypes;

    using Opencensus.Proto.Trace.V1;
    using OpenCensus.Trace;
    using OpenCensus.Trace.Export;

    internal static class SpanDataExtentions
    {
        internal static Span ToProtoSpan(this ISpanData spanData)
        {
            try
            {
                return new Span
                {
                    Name = new TruncatableString { Value = spanData.Name },
                    Kind = spanData.Kind == SpanKind.Client ? Span.Types.SpanKind.Client : Span.Types.SpanKind.Server,
                    TraceId = ByteString.CopyFrom(spanData.Context.TraceId.Bytes),
                    SpanId = ByteString.CopyFrom(spanData.Context.SpanId.Bytes),
                    ParentSpanId =
                        ByteString.CopyFrom(spanData.ParentSpanId?.Bytes ?? new byte[0]),

                    StartTime = new Timestamp
                    {
                        Nanos = spanData.StartTimestamp.Nanos,
                        Seconds = spanData.StartTimestamp.Seconds,
                    },
                    EndTime = new Timestamp
                    {
                        Nanos = spanData.EndTimestamp.Nanos,
                        Seconds = spanData.EndTimestamp.Seconds,
                    },
                    Status = spanData.Status == null
                        ? null
                        : new Opencensus.Proto.Trace.V1.Status
                        {
                            Code = (int)spanData.Status.CanonicalCode,
                            Message = spanData.Status.Description ?? string.Empty,
                        },
                    SameProcessAsParentSpan =
                        !spanData.HasRemoteParent.GetValueOrDefault() && spanData.ParentSpanId != null,
                    ChildSpanCount = spanData.ChildSpanCount.HasValue ? (uint)spanData.ChildSpanCount.Value : 0,
                    Attributes = FromIAttributes(spanData.Attributes),
                    TimeEvents = FromITimeEvents(spanData.MessageEvents, spanData.Annotations),
                    Links = new Span.Types.Links
                    {
                        DroppedLinksCount = spanData.Links.DroppedLinksCount,
                        Link = { spanData.Links.Links.Select(FromILink), },
                    },
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                // TODO: log
            }

            return null;
        }

        private static Span.Types.Attributes FromIAttributes(IAttributes source)
        {
            var attributes = new Span.Types.Attributes
            {
                DroppedAttributesCount = source.DroppedAttributesCount,
            };

            attributes.AttributeMap.Add(source.AttributeMap.ToDictionary(
                kvp => kvp.Key,
                kvp => new Opencensus.Proto.Trace.V1.AttributeValue
                {
                    StringValue = new TruncatableString
                    {
                        Value = kvp.Value.Match(s => s, b => b.ToString(), l => l.ToString(), d => d.ToString(), o => o?.ToString()),
                    },

                    // todo: how to determine AttributeValue type?
                }));

            return attributes;
        }

        private static Span.Types.TimeEvent FromITimeEvent(ITimedEvent<IMessageEvent> source)
        {
            return new Span.Types.TimeEvent
            {
                Time = new Timestamp
                {
                    Nanos = source.Timestamp.Nanos,
                    Seconds = source.Timestamp.Seconds,
                },
                MessageEvent = new Span.Types.TimeEvent.Types.MessageEvent
                {
                    Type = source.Event.Type == MessageEventType.SENT ? Span.Types.TimeEvent.Types.MessageEvent.Types.Type.Sent : Span.Types.TimeEvent.Types.MessageEvent.Types.Type.Received,
                    CompressedSize = (ulong)source.Event.CompressedMessageSize,
                    UncompressedSize = (ulong)source.Event.UncompressedMessageSize,
                    Id = (ulong)source.Event.MessageId,
                },
            };
        }

        private static Span.Types.TimeEvents FromITimeEvents(ITimedEvents<IMessageEvent> messages, ITimedEvents<IAnnotation> annotations)
        {
            var timedEvents = new Span.Types.TimeEvents
            {
                DroppedMessageEventsCount = messages.DroppedEventsCount,
                DroppedAnnotationsCount = annotations.DroppedEventsCount,
                TimeEvent = { messages.Events.Select(FromITimeEvent), },
            };

            timedEvents.TimeEvent.AddRange(annotations.Events.Select(FromITimeEvent));

            return timedEvents;
        }

        private static Span.Types.Link FromILink(ILink source)
        {
            return new Span.Types.Link
            {
                TraceId = ByteString.CopyFrom(source.TraceId.Bytes),
                SpanId = ByteString.CopyFrom(source.SpanId.Bytes),
                Type = source.Type == LinkType.CHILD_LINKED_SPAN ? Span.Types.Link.Types.Type.ChildLinkedSpan : Span.Types.Link.Types.Type.ParentLinkedSpan,
                Attributes = FromIAttributeMap(source.Attributes),
            };
        }

        private static Span.Types.TimeEvent FromITimeEvent(ITimedEvent<IAnnotation> source)
        {
            return new Span.Types.TimeEvent
            {
                Time = new Timestamp
                {
                    Nanos = source.Timestamp.Nanos,
                    Seconds = source.Timestamp.Seconds,
                },
                Annotation = new Span.Types.TimeEvent.Types.Annotation
                {
                    Description = new TruncatableString { Value = source.Event.Description },
                    Attributes = FromIAttributeMap(source.Event.Attributes),
                },
            };
        }

        private static Span.Types.Attributes FromIAttributeMap(IDictionary<string, IAttributeValue> source)
        {
            var attributes = new Span.Types.Attributes();

            attributes.AttributeMap.Add(source.ToDictionary(
                kvp => kvp.Key,
                kvp => new Opencensus.Proto.Trace.V1.AttributeValue
                {
                    StringValue = new TruncatableString
                    {
                        Value = kvp.Value.Match(s => s, b => b.ToString(), l => l.ToString(), d => d.ToString(), o => o?.ToString()),
                    },

                    // todo: how to determine AttributeValue type?
                }));

            return attributes;
        }
    }
}
