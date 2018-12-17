// <copyright file="IncomingRequestsCollectionsIsAccordingToTheSpecTests.cs" company="OpenCensus Authors">
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

namespace OpenCensus.Collector.AspNetCore.Tests
{
    using Xunit;
    using Microsoft.AspNetCore.Mvc.Testing;
    using TestApp.AspNetCore._2._0;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using OpenCensus.Trace;
    using OpenCensus.Trace.Config;
    using OpenCensus.Trace.Internal;
    using OpenCensus.Common;
    using Moq;
    using Microsoft.AspNetCore.TestHost;
    using System.Threading;
    using System;
    using Microsoft.AspNetCore.Http;

    public class IncomingRequestsCollectionsIsAccordingToTheSpecTests
        : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> factory;

        public IncomingRequestsCollectionsIsAccordingToTheSpecTests(WebApplicationFactory<Startup> factory)
        {
            this.factory = factory;

        }

        public class TestCallbackMiddlewareImpl : CallbackMiddleware.CallbackMiddlewareImpl
        {
            public override bool Process(HttpContext context)
            {
                context.Response.StatusCode = 503;
                return false;
            }
        }

        [Fact]
        public async Task SuccesfulTemplateControllerCallGeneratesASpan()
        {
            var startEndHandler = new Mock<IStartEndHandler>();
            var tracer = new Tracer(new RandomGenerator(), startEndHandler.Object, new DateTimeOffsetClock(), new TraceConfig());

            // Arrange
            var client = this.factory
                .WithWebHostBuilder(builder =>
                    builder.ConfigureTestServices((IServiceCollection services) => {
                        services.AddSingleton<CallbackMiddleware.CallbackMiddlewareImpl>(new TestCallbackMiddlewareImpl());
                        services.AddSingleton<ITracer>(tracer);
                    }))
                .CreateClient();

            try
            {
                // Act
                var response = await client.GetAsync("/api/values");
            }
            catch (Exception ex)
            {
                // ignore errors
            }

            // TODO: this is needed as span generation happens after response is returned for some reason. 
            // need to investigate
            Thread.Sleep(TimeSpan.FromMilliseconds(1));

            Assert.Equal(2, startEndHandler.Invocations.Count); // begin and end was called
            var spanData = ((Span)startEndHandler.Invocations[1].Arguments[0]).ToSpanData();

            Assert.Equal(SpanKind.Server, spanData.Kind);
            Assert.Equal(AttributeValue.StringAttributeValue("/api/values"), spanData.Attributes.AttributeMap["http.path"]);
            Assert.Equal(AttributeValue.LongAttributeValue(503), spanData.Attributes.AttributeMap["http.status_code"]);
        }
    }
}
