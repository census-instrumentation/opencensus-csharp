// <copyright file="StackExchangeRedisCallsCollectorTests.cs" company="OpenCensus Authors">
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
    using Moq;
    using StackExchange.Redis.Profiling;
    using Xunit;
    using OpenCensus.Trace.Internal;

    public class RedisProfilerEntryToSpanConverterTests
    {
        [Fact]
        public void ConvertProfiledCommandToSpanDataUsesCommandAsName()
        {
            var parentSpan = BlankSpan.Instance;
            var profiledCommand = new Mock<IProfiledCommand>();
            profiledCommand.Setup(m => m.Command).Returns("SET");
            var result = RedisProfilerEntryToSpanConverter.ConvertProfiledCommandToSpanData(parentSpan, profiledCommand.Object);
            Assert.Equal("SET", result.Name);
        }
    }
}
