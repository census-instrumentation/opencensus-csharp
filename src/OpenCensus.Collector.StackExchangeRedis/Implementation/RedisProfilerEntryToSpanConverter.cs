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
    using OpenCensus.Trace;
    using StackExchange.Redis.Profiling;

    internal static class RedisProfilerEntryToSpanConverter
    {
        public static void DrainSession(ProfilingSession session, ITracer tracer)
        {
            foreach (var entry in session.FinishProfiling())
            {
                RecordSpan(entry, tracer);
            }
        }

        public static void RecordSpan(IProfiledCommand command, ITracer tracer)
        {
            // use https://github.com/opentracing/specification/blob/master/semantic_conventions.md for now

            var builder = tracer.SpanBuilder(command.Command); // Example: SET

            var span = builder.StartSpan();

            span.PutAttribute("db.statement", AttributeValue.StringAttributeValue(command.Command)); // Example: SET
            span.PutAttribute("db.type", AttributeValue.StringAttributeValue("redis"));

            span.End();

            // db.instance
            // command.Db; // 0

            // command.EndPoint; // Unspecified/localhost:6379

            // command.Flags; // None, DemandMaster

            // command.CommandCreated; / /2019-01-10 22:18:28Z

            // command.CreationToEnqueued; // 00:00:32.4571995
            // command.ElapsedTime; // 00:00:32.4988020
            // command.EnqueuedToSending; // 00:00:00.0352838
            // command.ResponseToCompletion; // 00:00:00.0002601
            // command.RetransmissionOf;
            // command.RetransmissionReason;
            // command.SentToResponse; // 00:00:00.0060586
        }
    }
}
