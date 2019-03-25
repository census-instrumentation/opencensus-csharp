// <copyright file="RedisProfilerEntryToSpanConverterSamplingTests.cs" company="OpenCensus Authors">
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

using System.Diagnostics;

namespace OpenCensus.Collector.StackExchangeRedis.Implementation
{
    using Moq;
    using OpenCensus.Trace;
    using OpenCensus.Trace.Export;
    using OpenCensus.Trace.Internal;
    using StackExchange.Redis.Profiling;
    using System.Collections.Generic;
    using Xunit;

    public class RedisProfilerEntryToSpanConverterSamplingTests
    {
        [Fact]
        public void ShouldSampleRespectsSamplerChoice()
        {
            var m = new Mock<ISampler>();
            m.Setup(x => x.ShouldSample(It.IsAny<ISpanContext>(), It.IsAny<bool>(), It.IsAny<ActivityTraceId>(), It.IsAny<ActivitySpanId>(), It.IsAny<string>(), It.IsAny<IEnumerable<ISpan>>())).Returns(true);
            Assert.True(RedisProfilerEntryToSpanConverter.ShouldSample(SpanContext.Invalid, "SET", m.Object, out var context, out var parentId));

            m = new Mock<ISampler>();
            m.Setup(x => x.ShouldSample(It.IsAny<ISpanContext>(), It.IsAny<bool>(), It.IsAny<ActivityTraceId>(), It.IsAny<ActivitySpanId>(), It.IsAny<string>(), It.IsAny<IEnumerable<ISpan>>())).Returns(false);
            Assert.False(RedisProfilerEntryToSpanConverter.ShouldSample(SpanContext.Invalid, "SET", m.Object, out context, out parentId));
        }

        [Fact]
        public void ShouldSampleDoesntThrowWithoutSampler()
        {
            RedisProfilerEntryToSpanConverter.ShouldSample(SpanContext.Invalid, "SET", null, out var context, out var parentId);
        }

        [Fact]
        public void ShouldSamplePassesArgumentsToSamplerAndReturnsInContext()
        {
            var m = new Mock<ISampler>();
            var r = new RandomGenerator();
            var traceId = ActivityTraceId.CreateRandom();
            var parentContext = SpanContext.Create(traceId, ActivitySpanId.CreateRandom(), TraceOptions.Sampled, Tracestate.Builder.Set("a", "b").Build());
            RedisProfilerEntryToSpanConverter.ShouldSample(parentContext, "SET", m.Object, out var context, out var parentId);

            m.Verify(x => x.ShouldSample(
                It.Is<ISpanContext>(y => y == parentContext),
                It.Is<bool>(y => y == false),
                It.Is<ActivityTraceId>(y => y == traceId && y == context.TraceId),
                It.Is<ActivitySpanId>(y => y != default && y == context.SpanId),
                It.Is<string>(y => y == "SET"),
                It.Is<IEnumerable<ISpan>>(y => y == null)));
        }

        [Fact]
        public void ShouldSampleGeneratesNewTraceIdForInvalidContext()
        {
            var m = new Mock<ISampler>();
            m.Setup(x => x.ShouldSample(It.IsAny<ISpanContext>(), It.IsAny<bool>(), It.IsAny<ActivityTraceId>(), It.IsAny<ActivitySpanId>(), It.IsAny<string>(), It.IsAny<IEnumerable<ISpan>>())).Returns((ISpanContext parentContext, bool hasRemoteParent, ActivityTraceId traceId, ActivitySpanId spanId, string name, IEnumerable<ISpan> parentLinks) => parentContext.TraceOptions.IsSampled);

            RedisProfilerEntryToSpanConverter.ShouldSample(SpanContext.Invalid, "SET", m.Object, out var context, out var parentId);

            m.Verify(x => x.ShouldSample(
                It.Is<ISpanContext>(y => !y.IsValid),
                It.Is<bool>(y => y == false),
                It.Is<ActivityTraceId>(y => y != default && y == context.TraceId),
                It.Is<ActivitySpanId>(y => y != default && y == context.SpanId),
                It.Is<string>(y => y == "SET"),
                It.Is<IEnumerable<ISpan>>(y => y == null)));

            Assert.Equal(TraceOptions.Default, context.TraceOptions);
        }
    }
}
