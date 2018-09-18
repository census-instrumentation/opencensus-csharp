// <copyright file="HttpHandlerDiagnosticListener.cs" company="OpenCensus Authors">
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
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using OpenCensus.Trace;

    internal class HttpHandlerDiagnosticListener : ListenerHandler
    {
        private readonly PropertyFetcher startRequestFetcher = new PropertyFetcher("Request");
        private readonly PropertyFetcher stopResponseFetcher = new PropertyFetcher("Response");
        private readonly PropertyFetcher stopExceptionFetcher = new PropertyFetcher("Exception");
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

            // TODO: this needs to be generilized
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

            if ((int)response.StatusCode >= 200 && (int)response.StatusCode <= 301)
            {
                span.Status = Status.Ok;
            }
            else if ((int)response.StatusCode == 401)
            {
                span.Status = Status.Unauthenticated;
            }
            else if ((int)response.StatusCode == 403)
            {
                span.Status = Status.PermissionDenied;
            }
            else if ((int)response.StatusCode == 404)
            {
                span.Status = Status.NotFound;
            }
            else if ((int)response.StatusCode == 501)
            {
                span.Status = Status.Unimplemented;
            }
            else
            {
                span.Status = Status.Unknown;
            }

            span.Status = span.Status.WithDescription(response.StatusCode + " " + response.ReasonPhrase);

            span.PutHttpStatusCodeAttribute((int)response.StatusCode);
            this.LocalScope.Value?.Dispose();
        }

        public override void OnStopActivityWithException(Activity activity, object payload)
        {
            if (!(this.stopExceptionFetcher.Fetch(payload) is Exception exc))
            {
                // Debug.WriteLine("response is null");
                return;
            }

            var span = this.Tracer.CurrentSpan;
            var requestTaskStatus = this.stopRequestStatusFetcher.Fetch(payload) as TaskStatus?;
            if (requestTaskStatus.HasValue)
            {
                if (requestTaskStatus != TaskStatus.RanToCompletion)
                {
                    span.PutErrorAttribute(requestTaskStatus.ToString());
                }
            }

            this.LocalScope.Value?.Dispose();

            if (exc is HttpRequestException)
            {
                // this will be System.Net.Http.WinHttpException: The server name or address could not be resolved
                // on netstandard...
                if (exc.InnerException is WebException &&
                    ((WebException)exc.InnerException).Status == WebExceptionStatus.NameResolutionFailure)
                {
                    span.Status = Status.InvalidArgument;
                }
            }

        }
    }
}

/*
public static readonly Status Ok = new Status(CanonicalCode.Ok);
public static readonly Status Cancelled = new Status(CanonicalCode.Cancelled);
public static readonly Status Unknown = new Status(CanonicalCode.Unknown);
public static readonly Status InvalidArgument = new Status(CanonicalCode.InvalidArgument);
public static readonly Status DeadlineExceeded = new Status(CanonicalCode.DeadlineExceeded);
public static readonly Status AlreadyExists = new Status(CanonicalCode.AlreadyExists);
public static readonly Status ResourceExhausted = new Status(CanonicalCode.ResourceExhausted);
public static readonly Status FailedPrecondition = new Status(CanonicalCode.FailedPrecondition);
public static readonly Status Aborted = new Status(CanonicalCode.Aborted);
public static readonly Status OutOfRange = new Status(CanonicalCode.OutOfRange);
public static readonly Status Unimplemented = new Status(CanonicalCode.Unimplemented);
public static readonly Status Internal = new Status(CanonicalCode.Internal);
public static readonly Status Unavailable = new Status(CanonicalCode.Unavailable);
public static readonly Status DataLoss = new Status(CanonicalCode.DataLoss);

*/