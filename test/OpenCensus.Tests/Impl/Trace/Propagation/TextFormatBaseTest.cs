// <copyright file="TextFormatBaseTest.cs" company="OpenCensus Authors">
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

namespace OpenCensus.Trace.Propagation.Test
{
    using System;
    using Xunit;

    public class TextFormatTest
    {
        private static readonly ITextFormat textFormat = TextFormatBase.NoopTextFormat;

        [Fact]
        public void Inject_NullSpanContext()
        {
            Assert.Throws<ArgumentNullException>(() => textFormat.Inject(null, new object(), new TestSetter()));
        }

        [Fact]
        public void Inject_NotNullSpanContext_DoesNotFail()
        {
            textFormat.Inject(SpanContext.INVALID, new object(), new TestSetter());
        }

        [Fact]
        public void FromHeaders_NullGetter()
        {
            Assert.Throws<ArgumentNullException>(() => textFormat.Extract(new object(), null));
        }

        [Fact]
        public void FromHeaders_NotNullGetter()
        {
            Assert.Same(SpanContext.INVALID, textFormat.Extract(new object(), new TestGetter()));
        }

        class TestSetter : ISetter<object>
        {
            public void Put(object carrier, string key, string value)
            {
            }
        }

        class TestGetter : IGetter<object>
        {
            public string Get(object carrier, string key)
            {
                return null;
            }
        }
    }
}
