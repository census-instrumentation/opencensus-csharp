// <copyright file="RedisProfilerEntryToSpanConverter.cs" company="OpenCensus Authors">
// Copyright 2018, OpenCensus Authors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

namespace OpenCensus.Collector.StackExchangeRedis.Implementation
{
    using System;
    using System.Collections.Generic;
    using OpenCensus.Common;
    using OpenCensus.Stats.Aggregations;
    using OpenCensus.Trace;
    using OpenCensus.Trace.Export;
    using StackExchange.Redis.Profiling;

    internal static class RedisProfilerEntryToSpanConverter
    {
        private static readonly Random Rand = new Random();

        public static void DrainSession(ISpan parentSpan, ProfilingSession session, ITracer tracer, IHandler handler)
        {
            foreach (var entry in session.FinishProfiling())
            {
                RecordSpan(parentSpan, entry, tracer, handler);
            }
        }

        public static void RecordSpan(ISpan parentSpan, IProfiledCommand command, ITracer tracer, IHandler handler)
        {
            // TODO: Check sampling first
            var sd = ConvertProfiledCommandToSpanData(parentSpan, command);

            handler.Export(new ISpanData[] { sd });
        }

        public static ISpanData ConvertProfiledCommandToSpanData(ISpan parentSpan, IProfiledCommand command)
        {
            // use https://github.com/opentracing/specification/blob/master/semantic_conventions.md for now

            // TODO: SpanContext.Create is from OpenCensus implementation
            // TODO: deal with tracestate and traceoptions
            ISpanContext context = SpanContext.Create(parentSpan?.Context?.TraceId, SpanId.FromBytes(GenerateRandomId(8)), TraceOptions.Default, Tracestate.Empty);
            ISpanId parentSpanId = parentSpan?.Context.SpanId;
            bool? hasRemoteParent = false;
            string name = command.Command; // Example: SET;

            // Timing examples:
            // command.CommandCreated; //2019-01-10 22:18:28Z

            // command.CreationToEnqueued;      // 00:00:32.4571995
            // command.EnqueuedToSending;       // 00:00:00.0352838
            // command.SentToResponse;          // 00:00:00.0060586
            // command.ResponseToCompletion;    // 00:00:00.0002601

            // Total:
            // command.ElapsedTime;             // 00:00:32.4988020

            // TODO: make timestamp with the better precision
            ITimestamp startTimestamp = Timestamp.FromMillis(new DateTimeOffset(command.CommandCreated).ToUnixTimeMilliseconds());

            var timestamp = new DateTimeOffset(command.CommandCreated).Add(command.CreationToEnqueued);
            var annotations = TimedEvents<IAnnotation>.Create(
                new List<ITimedEvent<IAnnotation>>()
                {
                    TimedEvent<IAnnotation>.Create(Timestamp.FromMillis(timestamp.ToUnixTimeMilliseconds()), Annotation.FromDescription("Enqueued")),
                    TimedEvent<IAnnotation>.Create(Timestamp.FromMillis((timestamp = timestamp.Add(command.EnqueuedToSending)).ToUnixTimeMilliseconds()), Annotation.FromDescription("Sent")),
                    TimedEvent<IAnnotation>.Create(Timestamp.FromMillis((timestamp = timestamp.Add(command.SentToResponse)).ToUnixTimeMilliseconds()), Annotation.FromDescription("ResponseRecieved")),
                },
                droppedEventsCount: 0);

            ITimestamp endTimestamp = Timestamp.FromMillis(new DateTimeOffset(command.CommandCreated.Add(command.ElapsedTime)).ToUnixTimeMilliseconds());

            // TODO: deal with the re-transmission
            // command.RetransmissionOf;
            // command.RetransmissionReason;

            var attributesMap = new Dictionary<string, IAttributeValue>()
            {
                // TODO: pre-allocate constant attribute and reuse
                { "db.type", AttributeValue.StringAttributeValue("redis") },

                // Example: "redis.flags": None, DemandMaster
                { "redis.flags", AttributeValue.StringAttributeValue(command.Flags.ToString()) },
            };

            if (command.Command != null)
            {
                // Example: "db.statement": SET;
                attributesMap.Add("db.statement", AttributeValue.StringAttributeValue(command.Command));
            }

            if (command.EndPoint != null)
            {
                // Example: "db.instance": Unspecified/localhost:6379[0]
                attributesMap.Add("db.instance", AttributeValue.StringAttributeValue(command.EndPoint.ToString() + "[" + command.Db + "]"));
            }

            var attributes = Attributes.Create(attributesMap, 0);

            ITimedEvents<IMessageEvent> messageOrNetworkEvents = null;
            ILinks links = null;
            int? childSpanCount = 0;

            // TODO: this is strange that IProfiledCommand doesn't give the result
            Status status = Status.Ok;
            SpanKind kind = SpanKind.Client;

            // TODO: SpanData.Create is from OpenCensus implementation
            return SpanData.Create(context, parentSpanId, hasRemoteParent, name, startTimestamp, attributes, annotations, messageOrNetworkEvents, links, childSpanCount, status, kind, endTimestamp);
        }

        private static byte[] GenerateRandomId(int byteCount)
        {
            var idBytes = new byte[byteCount];
            Rand.NextBytes(idBytes);

            return idBytes;
        }
    }
}
