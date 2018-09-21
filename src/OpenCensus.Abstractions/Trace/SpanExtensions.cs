// <copyright file="SpanExtensions.cs" company="OpenCensus Authors">
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

namespace OpenCensus.Trace
{
    using System.Collections.Generic;
    using System.Collections.Specialized;

    public static class SpanExtensions
    {
        public static ISpan PutClientSpanKindAttribute(this ISpan span)
        {
            span.Kind = SpanKind.Client;
            return span;
        }

        public static ISpan PutServerSpanKindAttribute(this ISpan span)
        {
            span.Kind = SpanKind.Server;
            return span;
        }

        public static ISpan PutHttpMethodAttribute(this ISpan span, string method)
        {
            span.PutAttribute(SpanAttributeConstants.HttpMethodKey, AttributeValue.StringAttributeValue(method));
            return span;
        }

        /// <summary>
        /// Helper method that populates span properties from http status code according 
        /// to https://github.com/census-instrumentation/opencensus-specs/blob/4954074adf815f437534457331178194f6847ff9/trace/HTTP.md
        /// </summary>
        /// <param name="span">Span to fill out.</param>
        /// <param name="statusCode">Http status code.</param>
        /// <returns>Span with populated status code properties.</returns>
        public static ISpan PutHttpStatusCodeAttribute(this ISpan span, int statusCode)
        {
            span.PutAttribute(SpanAttributeConstants.HttpStatusCodeKey, AttributeValue.LongAttributeValue(statusCode));
            return span;
        }

        /// <summary>
        /// Helper method that populates span properties from http user agent according 
        /// to https://github.com/census-instrumentation/opencensus-specs/blob/4954074adf815f437534457331178194f6847ff9/trace/HTTP.md
        /// </summary>
        /// <param name="span">Span to fill out.</param>
        /// <param name="userAgent">Http status code.</param>
        /// <returns>Span with populated user agent code properties.</returns>
        public static ISpan PutHttpUserAgentAttribute(this ISpan span, string userAgent)
        {
            if (!string.IsNullOrWhiteSpace(userAgent))
            {
                span.PutAttribute(SpanAttributeConstants.HttpUserAgentKey, AttributeValue.StringAttributeValue(userAgent));
            }

            return span;
        }

        /// <summary>
        /// Helper method that populates span properties from host and port
        /// to https://github.com/census-instrumentation/opencensus-specs/blob/4954074adf815f437534457331178194f6847ff9/trace/HTTP.md
        /// </summary>
        /// <param name="span">Span to fill out.</param>
        /// <param name="hostName">Hostr name.</param>
        /// <param name="port">Port number.</param>
        /// <returns>Span with populated host properties.</returns>
        public static ISpan PutHttpHostAttribute(this ISpan span, string hostName, int port)
        {
            if (port == 80 || port == 443)
            {
                span.PutAttribute(SpanAttributeConstants.HttpHostKey, AttributeValue.StringAttributeValue(hostName));
            }
            else
            {
                span.PutAttribute(SpanAttributeConstants.HttpHostKey, AttributeValue.StringAttributeValue(hostName + ":" + port));
            }

            return span;
        }

        public static ISpan PutHttpPathAttribute(this ISpan span, string path)
        {
            span.PutAttribute(SpanAttributeConstants.HttpPathKey, AttributeValue.StringAttributeValue(path));
            return span;
        }

        public static ISpan PutHttpResponseSizeAttribute(this ISpan span, long size)
        {
            span.PutAttribute(SpanAttributeConstants.HttpResponseSizeKey, AttributeValue.LongAttributeValue(size));
            return span;
        }

        public static ISpan PutHttpRequestSizeAttribute(this ISpan span, long size)
        {
            span.PutAttribute(SpanAttributeConstants.HttpRequestSizeKey, AttributeValue.LongAttributeValue(size));
            return span;
        }

        /// <summary>
        /// Helper method that populates span properties from error attribute
        /// according to https://github.com/opentracing/specification/blob/master/semantic_conventions.md#span-tags-table.
        /// </summary>
        /// <param name="span">Span to fill out.</param>
        /// <param name="error">Indicate whether span ended with error.</param>
        /// <returns>Span with populated properties.</returns>
        public static ISpan PutErrorAttribute(this ISpan span, bool error = true)
        {
            span.PutAttribute(SpanAttributeConstants.ErrorKey, AttributeValue.BooleanAttributeValue(error));
            return span;
        }

        public static ISpan PutErrorStackTraceAttribute(this ISpan span, string errorStackTrace)
        {
            span.PutAttribute(SpanAttributeConstants.ErrorStackTrace, AttributeValue.StringAttributeValue(errorStackTrace));
            return span;
        }

        public static ISpan PutMvcControllerClass(this ISpan span,  string className)
        {
            span.PutAttribute(SpanAttributeConstants.MvcControllerClass, AttributeValue.StringAttributeValue(className));
            return span;
        }

        public static ISpan PutMvcControllerAction(this ISpan span, string actionName)
        {
            span.PutAttribute(SpanAttributeConstants.MvcControllerMethod, AttributeValue.StringAttributeValue(actionName));
            return span;
        }

        public static ISpan PutMvcViewExecutingFilePath(this ISpan span, string actionName)
        {
            span.PutAttribute(SpanAttributeConstants.MvcViewFilePath, AttributeValue.StringAttributeValue(actionName));
            return span;
        }

        public static ISpan PutHttpRequestHeadersAttribute(this ISpan span, List<KeyValuePair<string, IEnumerable<string>>> headers)
        {
            PutHeadersAttribute(span, "http.request.", headers);
            return span;
        }

        public static ISpan PutHttpResponseHeadersAttribute(this ISpan span, List<KeyValuePair<string, IEnumerable<string>>> headers)
        {
            PutHeadersAttribute(span, "http.response.", headers);
            return span;
        }

        public static void PutHeadersAttribute(ISpan span, string key, List<KeyValuePair<string, IEnumerable<string>>> headers)
        {
            foreach (var header in headers)
            {
                span.PutAttribute(key + header.Key, ToCommaDelimitedStringAttribute(header.Value));
            }
        }

        public static ISpan PutHttpRequestHeadersAttribute(this ISpan span, NameValueCollection headers)
        {
            PutHeadersAttribute(span, "http.request.", headers);
            return span;
        }

        public static ISpan PutHttpResponseHeadersAttribute(this ISpan span, NameValueCollection headers)
        {
            PutHeadersAttribute(span, "http.response.", headers);
            return span;
        }

        /// <summary>
        /// Helper method that populates span properties from http status code according 
        /// to https://github.com/census-instrumentation/opencensus-specs/blob/4954074adf815f437534457331178194f6847ff9/trace/HTTP.md
        /// </summary>
        /// <param name="span">Span to fill out.</param>
        /// <param name="statusCode">Http status code.</param>
        /// <param name="reasonPhrase">Http reason phrase.</param>
        /// <returns>Span with populated properties.</returns>
        public static ISpan PutHttpStatusCode(this ISpan span, int statusCode, string reasonPhrase)
        {
            span.PutHttpStatusCodeAttribute(statusCode);

            if ((int)statusCode < 200)
            {
                span.Status = Status.Unknown;
            } else if ((int)statusCode >= 200 && (int)statusCode <= 399)
            {
                span.Status = Status.Ok;
            }
            else if ((int)statusCode == 400)
            {
                span.Status = Status.InvalidArgument;
            }
            else if ((int)statusCode == 401)
            {
                span.Status = Status.Unauthenticated;
            }
            else if ((int)statusCode == 403)
            {
                span.Status = Status.PermissionDenied;
            }
            else if ((int)statusCode == 404)
            {
                span.Status = Status.NotFound;
            }
            else if ((int)statusCode == 429)
            {
                span.Status = Status.ResourceExhausted;
            }
            else if ((int)statusCode == 501)
            {
                span.Status = Status.Unimplemented;
            }
            else if ((int)statusCode == 503)
            {
                span.Status = Status.Unavailable;
            }
            else if ((int)statusCode == 504)
            {
                span.Status = Status.DeadlineExceeded;
            }
            else
            {
                span.Status = Status.Unknown;
            }

            span.Status = span.Status.WithDescription(reasonPhrase);

            return span;
        }

        private static void PutHeadersAttribute(ISpan span, string key, NameValueCollection headers)
        {
            foreach (var header in headers.AllKeys)
            {
                IAttributeValue values = ToCommaDelimitedStringAttribute(headers.GetValues(header));
                span.PutAttribute(key + header, values);
            }
        }

        private static IAttributeValue ToCommaDelimitedStringAttribute(IEnumerable<string> values)
        {
            var list = string.Join(",", values);
            return AttributeValue.StringAttributeValue(list);
        }
    }
}
