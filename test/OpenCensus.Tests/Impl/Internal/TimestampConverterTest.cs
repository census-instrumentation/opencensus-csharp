// <copyright file="TimestampConverterTest.cs" company="OpenCensus Authors">
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

namespace OpenCensus.Internal.Test
{
    using System;
    using Moq;
    using OpenCensus.Common;
    using Xunit;

    public class TimestampConverterTest
    {
        private readonly DateTimeOffset timestamp = new DateTimeOffset((1234 * TimeSpan.TicksPerSecond) + 56, TimeSpan.Zero);

        private Mock<IClock> mockClock;

        public TimestampConverterTest()
        {
            mockClock = new Mock<IClock>();
        }

        [Fact]
        public void ConvertNanoTime()
        {
            mockClock.Setup(clock => clock.Now).Returns(timestamp);
            mockClock.Setup(clock => clock.NowNanos).Returns(1234L);

            ITimestampConverter timeConverter = TimestampConverter.Now(mockClock.Object);
            Assert.Equal(new DateTimeOffset((1234 * TimeSpan.TicksPerSecond) + 106, TimeSpan.Zero), timeConverter.ConvertNanoTime(6234));
            Assert.Equal(new DateTimeOffset((1234 * TimeSpan.TicksPerSecond) + 54, TimeSpan.Zero), timeConverter.ConvertNanoTime(1000));
            Assert.Equal(new DateTimeOffset((1235 * TimeSpan.TicksPerSecond), TimeSpan.Zero), timeConverter.ConvertNanoTime(999995556));
        }
    } 
}
