// <copyright file="HttpClientListener.cs" company="OpenCensus Authors">
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

namespace OpenCensus.Collector.Dependencies.Implementation
{
    using System.Diagnostics;
    using System.Net.Http;
    using System.Threading.Tasks;
    using OpenCensus.Trace;
    using OpenCensus.Trace.Sampler;

    internal class HttpClientListener : ListenerHandler
    {
        private readonly PropertyFetcher startRequestFetcher = new PropertyFetcher("Request");
        private readonly PropertyFetcher stopResponseFetcher = new PropertyFetcher("Response");
        private readonly PropertyFetcher stopRequestStatusFetcher = new PropertyFetcher("RequestTaskStatus");

        public HttpClientListener(ITracer tracer) : base("HttpHandlerDiagnosticListener", tracer)
        {
        }

        public override void OnStartActivity(Activity activity, object payload)
        {
            var request = this.startRequestFetcher.Fetch(payload) as HttpRequestMessage;
            if (request == null)
            {
                Debug.WriteLine("request is null");
                return;
            }

            // TODO; sampling
            this.LocalScope.Value = this.Tracer.SpanBuilder("HttpOut").SetSampler(Samplers.AlwaysSample).StartScopedSpan();
            var span = this.Tracer.CurrentSpan;
            // span.PutClientSpanKindAttribute();
            // span.PutHttpMethodAttribute(request.Method.ToString());
            // span.PutHttpHostAttribute(request.RequestUri.Host);
            // span.PutHttpPathAttribute(request.RequestUri.AbsolutePath);
            // span.PutHttpUrlAttribute(request.RequestUri.ToString());
        }

        public override void OnStopActivity(Activity activity, object payload)
        {
            var response = this.stopResponseFetcher.Fetch(payload) as HttpResponseMessage;
            if (response == null)
            {
                Debug.WriteLine("response is null");
                return;
            }

            var requestTaskStatus = this.stopRequestStatusFetcher.Fetch(payload) as TaskStatus?;

            var span = this.Tracer.CurrentSpan;
            if (requestTaskStatus.HasValue)
            {
                if (requestTaskStatus != TaskStatus.RanToCompletion)
                {
                    // span.PutErrorAttribute(requestTaskStatus.ToString());
                }
            }

            // TODO status
            // span.Status = new Status(((int)response.StatusCode >= 200 && (int)response.StatusCode < 300) ? CanonicalCode.Ok : CanonicalCode.Unknown, response.StatusCode.ToString());
            // span.PutHttpStatusCodeAttribute((int)response.StatusCode);
            this.LocalScope.Value?.Dispose();
        }
    }
}
