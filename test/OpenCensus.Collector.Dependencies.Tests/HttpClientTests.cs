﻿// <copyright file="DurationTest.cs" company="OpenCensus Authors">
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
    using Newtonsoft.Json;
    using OpenCensus.Common;
    using OpenCensus.Trace;
    using OpenCensus.Trace.Config;
    using OpenCensus.Trace.Internal;
    using OpenCensus.Trace.Sampler;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;

    public class HttpClientTests
    {
        public class HttpOutTestCase
        {
            public string name { get; set; }

            public string method { get; set; }

            public string url { get; set; }

            public Dictionary<string, string> headers { get; set; }

            public int responseCode { get; set; }

            public string spanName { get; set; }

            public string spanKind { get; set; }

            public string spanStatus { get; set; }

            public Dictionary<string, string> spanAttributes { get; set; }
        }

        private static IEnumerable<object[]> readTestCases()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            var serializer = new JsonSerializer();
            var input = serializer.Deserialize<HttpOutTestCase[]>(new JsonTextReader(new StreamReader(assembly.GetManifestResourceStream("OpenCensus.Collector.Dependencies.Tests.http-out-test-cases.json"))));

            return getArgumentsFromTestCaseObject(input);
        }

        private static IEnumerable<object[]> getArgumentsFromTestCaseObject(IEnumerable<HttpOutTestCase> input)
        {
            List<object[]> result = new List<object[]>();

            foreach (var testCase in input)
            {
                result.Add(new object[] {
                    testCase,
                });
            }

            return result;
        }

        public static IEnumerable<object[]> TestData
        {
            get
            {
                return readTestCases();
            }
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void HttpOutCallsAreCollectedSuccesfully(HttpOutTestCase tc)
        {
            var startEndHandler = new Mock<IStartEndHandler>();

            var random = new Random();
            random.Next();

            var host = "localhost";
            var port = random.Next(2000, 5000);

            tc.url = NormaizeValues(tc.url, host, port);

            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;
            var httpListener = new HttpListener();
            Task httpListenerTask = null;

            try
            {
                httpListener.Prefixes.Add($"http://{host}:{port}/");
                httpListener.Start();

                httpListenerTask = new Task(() =>
                {
                    var ctxTask = httpListener.GetContextAsync();

                    try
                    {
                        ctxTask.Wait(token);

                        var ctx = ctxTask.Result;
                        if (tc.responseCode != 0)
                        {
                            ctx.Response.StatusCode = tc.responseCode;
                        }
                        else
                        {
                            ctx.Response.StatusCode = 200;
                        }

                        using (var output = ctx.Response.OutputStream)
                        {
                            using (var writer = new StreamWriter(output))
                            {
                                writer.WriteLine(DateTime.UtcNow.ToString());
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
                using (var dc = new DependenciesCollector(new DependenciesCollectorOptions(), tracer, Samplers.AlwaysSample))
                {

                    try
                    {
                        using (var c = new HttpClient())
                        {
                            var request = new HttpRequestMessage();
                            request.RequestUri = new Uri(tc.url);
                            request.Method = new HttpMethod(tc.method);

                            if (tc.headers != null)
                            {
                                foreach(var header in tc.headers)
                                {
                                    request.Headers.Add(header.Key, header.Value);
                                }
                            }

                            var t = c.SendAsync(request);
                            t.Wait();
                        }
                    }
                    catch (Exception)
                    {
                        //test case can intentiaonlly send request that will result in exception
                    }
                }
            }
            finally
            {
                cts.Cancel();
                httpListener?.Stop();
            }


            Assert.Equal(2, startEndHandler.Invocations.Count); // begin and end was called
            var spanData = ((Span)startEndHandler.Invocations[1].Arguments[0]).ToSpanData();

            Assert.Equal(tc.spanName, spanData.Name);
            Assert.Equal(tc.spanKind, spanData.Kind.ToString());


            var d = new Dictionary<CanonicalCode, string>()
            {
                { CanonicalCode.Ok, "OK"},
                { CanonicalCode.Cancelled, "CANCELLED"},
                { CanonicalCode.Unknown, "UNKNOWN"},
                { CanonicalCode.InvalidArgument, "INVALID_ARGUMENT"},
                { CanonicalCode.DeadlineExceeded, "DEADLINE_EXCEEDED"},
                { CanonicalCode.NotFound, "NOT_FOUND"},
                { CanonicalCode.AlreadyExists, "ALREADY_EXISTS"},
                { CanonicalCode.PermissionDenied, "PERMISSION_DENIED"},
                { CanonicalCode.ResourceExhausted, "RESOURCE_EXHAUSTED"},
                { CanonicalCode.FailedPrecondition, "FAILED_PRECONDITION"},
                { CanonicalCode.Aborted, "ABORTED"},
                { CanonicalCode.OutOfRange, "OUT_OF_RANGE"},
                { CanonicalCode.Unimplemented, "UNIMPLEMENTED"},
                { CanonicalCode.Internal, "INTERNAL"},
                { CanonicalCode.Unavailable, "UNAVAILABLE"},
                { CanonicalCode.DataLoss, "DATA_LOSS"},
                { CanonicalCode.Unauthenticated, "UNAUTHENTICATED"},
            };

            Assert.Equal(tc.spanStatus, d[spanData.Status.CanonicalCode]);

            var normilizedAttributes = spanData.Attributes.AttributeMap.ToDictionary(x => x.Key, x => AttributeToSimpleString(x.Value));
            tc.spanAttributes = tc.spanAttributes.ToDictionary(x => x.Key, x => NormaizeValues(x.Value, host, port));

            Assert.Equal(tc.spanAttributes.ToHashSet(), normilizedAttributes.ToHashSet());
        }

        [Fact]
        public void DebugIndividualTest()
        {
            var serializer = new JsonSerializer();
            var input = serializer.Deserialize<HttpOutTestCase[]>(new JsonTextReader(new StringReader(@"
[   {
    ""name"": ""Response code 404"",
    ""method"": ""GET"",
    ""url"": ""http://{host}:{port}/"",
    ""responseCode"": 404,
    ""spanName"": ""/"",
    ""spanStatus"": ""NOT_FOUND"",
    ""spanKind"": ""Client"",
    ""spanAttributes"": {
                ""http.url"": ""http://{host}:{port}/"",
      ""http.path"": ""/"",
      ""http.method"": ""GET"",
      ""http.host"": ""{host}"",
""http.status_code"": ""404""
    }
        }
]
")));

            this.GetType().InvokeMember(nameof(HttpOutCallsAreCollectedSuccesfully), BindingFlags.InvokeMethod, null, this, getArgumentsFromTestCaseObject(input).First());
        }

        private string AttributeToSimpleString(IAttributeValue value)
        {
            return value.Match<string>(
                x => x.ToString(),
                x => x ? "true" : "false",
                x => x.ToString(),
                x => x.ToString()
            );
        }

        private string NormaizeValues(string value, string host, int port)
        {
            return value.Replace("{host}", host).Replace("{port}", port.ToString());
        }

    }
}
