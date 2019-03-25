// <copyright file="TraceIdTest.cs" company="OpenCensus Authors">
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

using System;

namespace OpenCensus.Trace.Test
{
    using System.Diagnostics;
    using Xunit;

    public class TraceIdTest
    {
        private static readonly byte[] firstBytes =
            new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, (byte)'a' };

        private static readonly byte[] secondBytes =
            new byte[] { 0xFF, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, (byte)'A' };

        private static readonly ActivityTraceId first = ActivityTraceId.CreateFromBytes(firstBytes);
        private static readonly ActivityTraceId second = ActivityTraceId.CreateFromBytes(secondBytes);

        [Fact]
        public void invalidTraceId()
        {
            Span<byte> traceIdBytes = stackalloc byte[16];
            default(ActivityTraceId).CopyTo(traceIdBytes);
            Assert.Equal(new byte[16], traceIdBytes.ToArray());
        }

        [Fact]
        public void IsValid()
        {
            Assert.True(first != default);
            Assert.True(second != default);
        }

        [Fact]
        public void Bytes()
        {
            Span<byte> firstIdBytes = stackalloc byte[16];
            first.CopyTo(firstIdBytes);

            Span<byte> secondIdBytes = stackalloc byte[16];
            second.CopyTo(secondIdBytes);

            Assert.Equal(firstBytes, firstIdBytes.ToArray());
            Assert.Equal(secondBytes, secondIdBytes.ToArray());
        }

        [Fact]
        public void FromLowerBase16()
        {
            Assert.Equal(default(ActivityTraceId), ActivityTraceId.CreateFromString("00000000000000000000000000000000".AsSpan()));
            Assert.Equal(first, ActivityTraceId.CreateFromString("00000000000000000000000000000061".AsSpan()));
            Assert.Equal(second, ActivityTraceId.CreateFromString("ff000000000000000000000000000041".AsSpan()));
        }

        [Fact]
        public void ToLowerBase16()
        {
            Assert.Equal("00000000000000000000000000000000", default(ActivityTraceId).ToHexString());
            Assert.Equal("00000000000000000000000000000061", first.ToHexString());
            Assert.Equal("ff000000000000000000000000000041", second.ToHexString());
        }

        [Fact]
        public void TraceId_CompareTo()
        {
          /*  Assert.Equal(1, first.CompareTo(second));
            Assert.Equal(-1, second.CompareTo(first));
            Assert.Equal(0, first.CompareTo(ActivityTraceId.CreateFromBytes(firstBytes)));*/
        }

        [Fact]
        public void TraceId_EqualsAndHashCode()
        {
            // EqualsTester tester = new EqualsTester();
            // tester.addEqualityGroup(default(ActivityTraceId), default(ActivityTraceId));
            // tester.addEqualityGroup(first, ActivityTraceId.CreateFromBytes(Arrays.copyOf(firstBytes, firstBytes.length)));
            // tester.addEqualityGroup(
            //    second, ActivityTraceId.CreateFromBytes(Arrays.copyOf(secondBytes, secondBytes.length)));
            // tester.testEquals();
        }

        [Fact]
        public void TraceId_ToString()
        {
            Assert.Contains("00000000000000000000000000000000", default(ActivityTraceId).ToString());
            Assert.Contains("00000000000000000000000000000061", first.ToString());
            Assert.Contains("ff000000000000000000000000000041", second.ToString());
        }
    }
}
