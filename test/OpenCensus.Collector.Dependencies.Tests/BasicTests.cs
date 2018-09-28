// <copyright file="BasicTests.cs" company="OpenCensus Authors">
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
    using OpenCensus.Trace.Propagation;
    using OpenCensus.Trace.Sampler;
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;

    public class BasicTests
    {
        [Fact]
        public async void HttpDepenenciesCollectorInjectsHeaders()
        {
            var startEndHandler = new Mock<IStartEndHandler>();

            var random = new Random();
            random.Next();

            var host = "localhost";
            var port = random.Next(2000, 5000);
            var url = $"http://{host}:{port}/";

            var cts = new CancellationTokenSource();
            var token = cts.Token;
            var httpListener = new HttpListener();
            Task httpListenerTask = null;

            ITraceId expectedTraceId = TraceId.Invalid;
            ISpanId expectedSpanId = SpanId.Invalid;

            try
            {
                httpListener.Prefixes.Add(url);
                httpListener.Start();

                httpListenerTask = new Task(() =>
                {
                    var ctxTask = httpListener.GetContextAsync();

                    try
                    {
                        ctxTask.Wait(token);

                        if (ctxTask.Status == TaskStatus.RanToCompletion)
                        {
                            var ctx = ctxTask.Result;
                            ctx.Response.StatusCode = 200;

                            using (var output = ctx.Response.OutputStream)
                            {
                                using (var writer = new StreamWriter(output))
                                {
                                    writer.WriteLine(DateTime.UtcNow.ToString());
                                }
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                    }
                    catch (Exception ex)
                    {
                        Assert.True(false, ex.ToString());
                    }
                });

                httpListenerTask.Start();

                var tracer = new Tracer(new RandomGenerator(), startEndHandler.Object, new DateTimeOffsetClock(), new TraceConfig());

                var tf = new Mock<ITextFormat>();
                tf
                    .Setup(m => m.Inject<HttpRequestMessage>(It.IsAny<ISpanContext>(), It.IsAny<HttpRequestMessage>(), It.IsAny<Action<HttpRequestMessage, string, string>>()))
                    .Callback((ISpanContext sc, HttpRequestMessage obj, Action<HttpRequestMessage, string, string>  setter) => {
                        expectedTraceId = sc.TraceId;
                        expectedSpanId = sc.SpanId;
                    });

                var propagationComponent = new Mock<IPropagationComponent>();
                propagationComponent.SetupGet(m => m.TextFormat).Returns(tf.Object);

                using (var dc = new DependenciesCollector(new DependenciesCollectorOptions(), tracer, Samplers.AlwaysSample, propagationComponent.Object))
                {

                    using (var c = new HttpClient())
                    {
                        var request = new HttpRequestMessage
                        {
                            RequestUri = new Uri(url),
                            Method = new HttpMethod("GET"),
                        };

                        await c.SendAsync(request);
                    }
                }
            }
            finally
            {
                httpListener?.Stop();
                cts.Cancel();
            }


            Assert.Equal(2, startEndHandler.Invocations.Count); // begin and end was called
            var spanData = ((Span)startEndHandler.Invocations[1].Arguments[0]).ToSpanData();

            Assert.Equal(expectedTraceId, spanData.Context.TraceId);
            Assert.Equal(expectedSpanId, spanData.Context.SpanId);
        }
    }
}
