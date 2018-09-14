﻿// <copyright file="HttpHandlerDiagnosticListener.cs" company="OpenCensus Authors">
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

    internal class HttpHandlerDiagnosticListener : ListenerHandler
    {
        private readonly PropertyFetcher startRequestFetcher = new PropertyFetcher("Request");
        private readonly PropertyFetcher stopResponseFetcher = new PropertyFetcher("Response");
        private readonly PropertyFetcher stopRequestStatusFetcher = new PropertyFetcher("RequestTaskStatus");

        public HttpHandlerDiagnosticListener(ITracer tracer, ISampler sampler) : base("HttpHandlerDiagnosticListener", tracer, sampler)
        {
        }

        public override void OnStartActivity(Activity activity, object payload)
        {
            if (!(this.startRequestFetcher.Fetch(payload) is HttpRequestMessage request))
            {
                // Debug.WriteLine("request is null");
                return;
            }

            if (request.RequestUri.ToString().Contains("zipkin.azurewebsites.net"))
            {
                return;
            }

            this.LocalScope.Value = this.Tracer.SpanBuilder("HttpOut").SetSampler(this.Sampler).StartScopedSpan();
            var span = this.Tracer.CurrentSpan;
            span.PutClientSpanKindAttribute();
            span.PutHttpMethodAttribute(request.Method.ToString());
            span.PutHttpHostAttribute(request.RequestUri.Host);
            span.PutHttpPathAttribute(request.RequestUri.AbsolutePath);
            span.PutHttpUrlAttribute(request.RequestUri.ToString());
        }

        public override void OnStopActivity(Activity activity, object payload)
        {
            if (!(this.stopResponseFetcher.Fetch(payload) is HttpResponseMessage response))
            {
                // Debug.WriteLine("response is null");
                return;
            }

            var requestTaskStatus = this.stopRequestStatusFetcher.Fetch(payload) as TaskStatus?;

            var span = this.Tracer.CurrentSpan;
            if (requestTaskStatus.HasValue)
            {
                if (requestTaskStatus != TaskStatus.RanToCompletion)
                {
                    span.PutErrorAttribute(requestTaskStatus.ToString());
                }
            }

            // TODO status
            // span.Status = new Status(((int)response.StatusCode >= 200 && (int)response.StatusCode < 300) ? CanonicalCode.Ok : CanonicalCode.Unknown, response.StatusCode.ToString());
            span.PutHttpStatusCodeAttribute((int)response.StatusCode);
            this.LocalScope.Value?.Dispose();
        }
    }
}
