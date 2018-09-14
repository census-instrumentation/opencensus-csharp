// <copyright file="DurationTest.cs" company="OpenCensus Authors">
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

namespace OpenCensus.Collector.Dependencies.Tests
{
    using OpenCensus.Common;
    using OpenCensus.Trace;
    using OpenCensus.Trace.Config;
    using OpenCensus.Trace.Export;
    using OpenCensus.Trace.Propagation;
    using OpenCensus.Trace.Sampler;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Xunit;

    public class HttpClientTests
    {
        private class SpanBuilder : ISpanBuilder
        {
            public ISpanBuilder SetParentLinks(IList<ISpan> parentLinks)
            {
                return this;
            }

            public ISpanBuilder SetRecordEvents(bool recordEvents)
            {
                return this;
            }

            public ISpanBuilder SetSampler(ISampler sampler)
            {
                return this;
            }

            public IScope StartScopedSpan()
            {
                return null;
            }

            public ISpan StartSpan()
            {
                return null;
            }
        }

        private class CustomTracer : ITracer
        {
            public ISpan CurrentSpan => throw new System.NotImplementedException();

            public ISpanBuilder SpanBuilder(string spanName)
            {
                return new SpanBuilder();
            }

            public ISpanBuilder SpanBuilderWithExplicitParent(string spanName, ISpan parent = null)
            {
                return new SpanBuilder();
            }

            public ISpanBuilder SpanBuilderWithRemoteParent(string spanName, ISpanContext remoteParentSpanContext = null)
            {
                return new SpanBuilder();
            }

            public IScope WithSpan(ISpan span)
            {
                return null;
            }
        }

        [Fact]
        public void HttpClientCallIsCollected()
        {
            var tracer = new CustomTracer();

            var dc = new DependenciesCollector(new DependenciesCollectorOptions(), tracer, Samplers.AlwaysSample);

            var t = new HttpClient().GetStringAsync("http://bing.com");
            t.Wait();

            Assert.NotNull(t.Result);
        }
    }
}
