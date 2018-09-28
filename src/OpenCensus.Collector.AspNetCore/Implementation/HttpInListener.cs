// <copyright file="HttpInListener.cs" company="OpenCensus Authors">
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

namespace OpenCensus.Collector.AspNetCore.Implementation
{
    using System.Diagnostics;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Features;
    using Microsoft.Extensions.Primitives;
    using OpenCensus.Collector.Implementation.Common;
    using OpenCensus.Trace;
    using OpenCensus.Trace.Propagation;

    internal class HttpInListener : ListenerHandler
    {
        private readonly PropertyFetcher startContextFetcher = new PropertyFetcher("HttpContext");
        private readonly PropertyFetcher stopContextFetcher = new PropertyFetcher("HttpContext");
        private readonly IPropagationComponent propagationComponent;

        public HttpInListener(ITracer tracer, ISampler sampler, IPropagationComponent propagationComponent) : base("Microsoft.AspNetCore", tracer, sampler)
        {
            this.propagationComponent = propagationComponent;
        }

        public override void OnStartActivity(Activity activity, object payload)
        {
            var context = this.startContextFetcher.Fetch(payload) as HttpContext;

            if (context == null)
            {
                // Debug.WriteLine("context is null");
                return;
            }

            var request = context.Request;

            var ctx = this.propagationComponent.TextFormat.Extract<HttpRequest>(
                request, 
                (r, name) => r.Headers[name]
            );

            this.Tracer.SpanBuilderWithRemoteParent("HttpIn", ctx).SetSampler(this.Sampler).StartScopedSpan();

            var span = this.Tracer.CurrentSpan;

            if (span != null)
            {
                span.PutServerSpanKindAttribute();
                span.PutHttpMethodAttribute(request.Method);
                span.PutHttpHostAttribute(request.Host.Value, request.Host.Port ?? 80);
                span.PutHttpPathAttribute(request.Path);
            }
        }

        public override void OnStopActivity(Activity activity, object payload)
        {
            var context = this.stopContextFetcher.Fetch(payload) as HttpContext;

            if (context == null)
            {
                // Debug.WriteLine("context is null");
                return;
            }

            var span = this.Tracer.CurrentSpan;

            if (span == null)
            {
                // report lost span
            }

            var response = context.Response;

            span.PutHttpStatusCode(response.StatusCode, response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase);
            span.End();
        }
    }
}