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
    using System.Net.Http;
    using System.Reflection;
    using System.Threading;
    using Xunit;

    public class HttpClientTests
    {
        private class HttpOutTestCase
        {
            public string name { get; set; }

            public string method { get; set; }

            public string url { get; set; }

            public string spanName { get; set; }
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
                    testCase.name,
                    testCase.method,
                    testCase.url,
                    testCase.spanName,
                    testCase.spanStatus,
                    testCase.spanAttributes,
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
        public void HttpOutCallsAreCollectedSuccesfully(
            string name,
            string method,
            string url,
            string spanName,
            string spanStatus,
            Dictionary<string, string> spanAttributes)
        {
            var startEndHandler = new Mock<IStartEndHandler>();
            startEndHandler.Invocations.Clear();

            var tracer = new Tracer(new RandomGenerator(), startEndHandler.Object, new DateTimeOffsetClock(), new TraceConfig());
            using (var dc = new DependenciesCollector(new DependenciesCollectorOptions(), tracer, Samplers.AlwaysSample))
            {

                try
                {
                    using (var c = new HttpClient())
                    {
                        var request = new HttpRequestMessage();
                        request.RequestUri = new Uri(url);
                        request.Method = new HttpMethod(method);
                        var t = c.SendAsync(request);
                        t.Wait();
                    }
                }
                catch (Exception)
                {
                    //test case can intentiaonlly send request that will result in exception
                }
            }

            Assert.Equal(2, startEndHandler.Invocations.Count); // begin and end was called
            var spanData = ((Span)startEndHandler.Invocations[1].Arguments[0]).ToSpanData();

            Assert.Equal(spanName, spanData.Name);

            Assert.Equal(spanStatus, spanData.Status.CanonicalCode.ToString());

            var normilizedAttributes = spanData.Attributes.AttributeMap.ToDictionary(x => x.Key, x => AttributeToSimpleString(x.Value));

            Assert.Equal(spanAttributes.ToHashSet(), normilizedAttributes.ToHashSet());
        }

        [Fact]
        public void DebugIndividualTest()
        {
            var serializer = new JsonSerializer();
            var input = serializer.Deserialize<HttpOutTestCase[]>(new JsonTextReader(new StringReader(@"
[  {
    ""name"": ""Name is populated as a path"",
    ""method"": ""GET"",
    ""url"": ""https://bing.com/path/to/page/"",
    ""spanName"": ""HttpOut"",
    ""spanStatus"": ""NotFound"",
    ""spanAttributes"": {
                ""http.url"": ""https://bing.com/path/to/page/"",
      ""http.path"": ""/path/to/page/"",
      ""http.method"": ""GET"",
      ""http.host"": ""bing.com"",
      ""http.status_code"": ""404"",
      ""span.kind"": ""client""
    }
        }]
")));

            this.GetType().InvokeMember(nameof(HttpOutCallsAreCollectedSuccesfully), BindingFlags.InvokeMethod, null, this, getArgumentsFromTestCaseObject(input).First());
        }

        private string AttributeToSimpleString(IAttributeValue value)
        {
            return value.Match<string>(
                x => x.ToString(),
                x => x.ToString(),
                x => x.ToString(),
                x => x.ToString()
            );
        }
    }
}
