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
    using Moq;
    using OpenCensus.Common;
    using OpenCensus.Trace;
    using OpenCensus.Trace.Config;
    using OpenCensus.Trace.Internal;
    using OpenCensus.Trace.Sampler;
    using System.Collections.Generic;
    using System.Net.Http;
    using Xunit;

    public class HttpClientTests
    {
        [Fact]
        public void HttpClientCallIsCollected()
        {
            var startEndHandler = new Mock<IStartEndHandler>();

            var tracer = new Tracer(new RandomGenerator(), startEndHandler.Object, new DateTimeOffsetClock(), new TraceConfig());

            var dc = new DependenciesCollector(new DependenciesCollectorOptions(), tracer, Samplers.AlwaysSample);

            using (var c = new HttpClient())
            {
                var t = c.GetAsync("http://bing.com");
                t.Wait();
                Assert.NotNull(t.Result);
            }

            Assert.Equal(2, startEndHandler.Invocations.Count); // begin and end was called
            var span = startEndHandler.Invocations[1].Arguments[0];
        }
    }
}
