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
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Features;
    using OpenCensus.Collector.Implementation.Common;
    using OpenCensus.Trace;
    using OpenCensus.Trace.Propagation;

    internal class HttpInListener : ListenerHandler
    {
        private const string UnknownHostName = "UNKNOWN-HOST";
        private readonly PropertyFetcher startContextFetcher = new PropertyFetcher("HttpContext");
        private readonly PropertyFetcher stopContextFetcher = new PropertyFetcher("HttpContext");
        private readonly PropertyFetcher beforActionRouteDataFetcher = new PropertyFetcher("routeData");
        private readonly IPropagationComponent propagationComponent;

        public HttpInListener(ITracer tracer, ISampler sampler, IPropagationComponent propagationComponent)
            : base("Microsoft.AspNetCore", tracer, sampler)
        {
            this.propagationComponent = propagationComponent;
        }

        /// <summary>
        /// Proxy interface for <c>RouteData</c> class from Microsoft.AspNetCore.Routing.Abstractions
        /// </summary>
        public interface IRouteData
        {
            /// <summary>
            /// Gets the set of values produced by routes on the current routing path.
            /// </summary>
            IDictionary<string, object> Values { get; }
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
                (r, name) => r.Headers[name]);

            // see the spec https://github.com/census-instrumentation/opencensus-specs/blob/master/trace/HTTP.md

            string path = (request.PathBase.HasValue || request.Path.HasValue) ? (request.PathBase + request.Path).ToString() : "/";

            this.Tracer.SpanBuilderWithRemoteParent(path, ctx).SetSampler(this.Sampler).StartScopedSpan();

            var span = this.Tracer.CurrentSpan;

            if (span != null)
            {
                // Note, route is missing at this stage. It will be available later

                span.PutServerSpanKindAttribute();
                span.PutHttpHostAttribute(request.Host.Host, request.Host.Port ?? 80);
                span.PutHttpMethodAttribute(request.Method);
                span.PutHttpPathAttribute(path);

                var userAgent = request.Headers["User-Agent"].FirstOrDefault();
                span.PutHttpUserAgentAttribute(userAgent);
                span.PutHttpRawUrlAttribute(GetUri(request));
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

        public override void OnCustom(string name, Activity activity, object payload)
        {
            if (name == "Microsoft.AspNetCore.Mvc.BeforeAction")
            {
                var span = this.Tracer.CurrentSpan;

                if (span == null)
                {
                    // report lost span
                }

                var routeData = this.beforActionRouteDataFetcher.Fetch(payload) as IRouteData;

                span.Name = GetNameFromRouteContext(routeData);
            }
        }

        private static string GetUri(HttpRequest request)
        {
            var builder = new StringBuilder();

            builder.Append(request.Scheme).Append("://");

            if (request.Host.HasValue)
            {
                builder.Append(request.Host.Value);
            }
            else
            {
                // HTTP 1.0 request with NO host header would result in empty Host.
                // Use placeholder to avoid incorrect URL like "http:///"
                builder.Append(UnknownHostName);
            }

            if (request.PathBase.HasValue)
            {
                builder.Append(request.PathBase.Value);
            }

            if (request.Path.HasValue)
            {
                builder.Append(request.Path.Value);
            }

            if (request.QueryString.HasValue)
            {
                builder.Append(request.QueryString);
            }

            return builder.ToString();
        }

        private static string GetNameFromRouteContext(IRouteData routeData)
        {
            string name = null;
            if (routeData.Values.Count > 0)
            {
                var routeValues = routeData.Values;
                object controller;
                routeValues.TryGetValue("controller", out controller);
                string controllerString = (controller == null) ? string.Empty : controller.ToString();
                if (!string.IsNullOrEmpty(controllerString))
                {
                    name = controllerString;
                    object action;
                    routeValues.TryGetValue("action", out action);
                    string actionString = (action == null) ? string.Empty : action.ToString();
                    if (!string.IsNullOrEmpty(actionString))
                    {
                        name += "/" + actionString;
                    }

                    if (routeValues.Keys.Count > 2)
                    {
                        // Add parameters
                        var sortedKeys = routeValues.Keys
                            .Where(key =>
                                !string.Equals(key, "controller", StringComparison.OrdinalIgnoreCase) &&
                                !string.Equals(key, "action", StringComparison.OrdinalIgnoreCase) &&
                                !string.Equals(key, "!__route_group", StringComparison.OrdinalIgnoreCase))
                            .OrderBy(key => key, StringComparer.OrdinalIgnoreCase)
                            .ToArray();

                        if (sortedKeys.Length > 0)
                        {
                            string arguments = string.Join(@"/", sortedKeys);
                            name += " [" + arguments + "]";
                        }
                    }
                }
                else
                {
                    object page;
                    routeValues.TryGetValue("page", out page);
                    string pageString = (page == null) ? string.Empty : page.ToString();
                    if (!string.IsNullOrEmpty(pageString))
                    {
                        name = pageString;
                    }
                }
            }

            return name;
        }
    }
}