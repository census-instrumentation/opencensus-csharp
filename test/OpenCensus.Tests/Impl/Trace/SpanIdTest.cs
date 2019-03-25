// <copyright file="SpanIdTest.cs" company="OpenCensus Authors">
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

    public class SpanIdTest
    {
        private static readonly byte[] firstBytes = new byte[] { 0, 0, 0, 0, 0, 0, 0, (byte)'a' };
        private static readonly byte[] secondBytes = new byte[] { 0xFF, 0, 0, 0, 0, 0, 0, (byte)'A' };
        private static readonly ActivitySpanId first = ActivitySpanId.CreateFromBytes(firstBytes);
        private static readonly ActivitySpanId second = ActivitySpanId.CreateFromBytes(secondBytes);

        [Fact]
        public void invalidSpanId()
        {
            Span<byte> spanIdBytes = stackalloc byte[8];
            default(ActivitySpanId).CopyTo(spanIdBytes);
            Assert.Equal(new byte[8], spanIdBytes.ToArray());
        }

        [Fact]
        public void IsValid()
        {
            Assert.True(first != default);
            Assert.True(second != default);
        }

        [Fact]
        public void FromLowerBase16()
        {
            Assert.Equal(default(ActivitySpanId), ActivitySpanId.CreateFromString("0000000000000000".AsSpan()));
            Assert.Equal(first, ActivitySpanId.CreateFromString("0000000000000061".AsSpan()));
            Assert.Equal(second, ActivitySpanId.CreateFromString("ff00000000000041".AsSpan()));
        }

        [Fact]
        public void ToLowerBase16()
        {
            Assert.Equal("0000000000000000", default(ActivitySpanId).ToHexString());
            Assert.Equal("0000000000000061", first.ToHexString());
            Assert.Equal("ff00000000000041", second.ToHexString());
        }

        [Fact]
        public void Bytes()
        {
            Span<byte> firstIdBytes = stackalloc byte[8];
            first.CopyTo(firstIdBytes);

            Span<byte> secondIdBytes = stackalloc byte[8];
            second.CopyTo(secondIdBytes);

            Assert.Equal(firstBytes, firstIdBytes.ToArray());
            Assert.Equal(secondBytes, secondIdBytes.ToArray());
        }

        [Fact]
        public void SpanId_CompareTo()
        {
/*            Assert.Equal(1, first.CompareTo(second));
            Assert.Equal(-1, second.CompareTo(first));
            Assert.Equal(0, first.CompareTo(ActivitySpanId.CreateFromBytes((firstBytes)));*/
        }

        [Fact]
        public void SpanId_EqualsAndHashCode()
        {
            // EqualsTester tester = new EqualsTester();
            // tester.addEqualityGroup(default(ActivitySpanId), default(ActivitySpanId));
            // tester.addEqualityGroup(first, ActivitySpanId.CreateFromBytes((Arrays.copyOf(firstBytes, firstBytes.length)));
            // tester.addEqualityGroup(
            //    second, ActivitySpanId.CreateFromBytes((Arrays.copyOf(secondBytes, secondBytes.length)));
            // tester.testEquals();
        }

        [Fact]
        public void SpanId_ToString()
        {
            Assert.Contains("0000000000000000", default(ActivitySpanId).ToString());
            Assert.Contains("0000000000000061", first.ToString());
            Assert.Contains("ff00000000000041", second.ToString());
        }
    }
}
