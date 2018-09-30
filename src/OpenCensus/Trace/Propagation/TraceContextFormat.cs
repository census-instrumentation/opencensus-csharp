// <copyright file="TraceContextFormat.cs" company="OpenCensus Authors">
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

namespace OpenCensus.Trace.Propagation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using OpenCensus.Utils;

    /// <summary>
    /// W3C trace context text wire protocol formatter. See https://github.com/w3c/distributed-tracing/.
    /// </summary>
    public class TraceContextFormat : TextFormatBase
    {
        /// <inheritdoc/>
        public override IList<string> Fields => new string[] { "tracestate", "traceparent" };

        /// <inheritdoc/>
        public override ISpanContext Extract<T>(T carrier, Func<T, string, IEnumerable<string>> getter)
        {
            // from https://github.com/w3c/distributed-tracing/blob/master/trace_context/HTTP_HEADER_FORMAT.md
            // traceparent: 00-0af7651916cd43dd8448eb211c80319c-00f067aa0ba902b7-01

            var traceparent = getter(carrier, "traceparent")?.FirstOrDefault();
            var tracestateCollection = getter(carrier, "tracestate");

            if (traceparent == null)
            {
                return null;
            }

            var traceId = TraceId.FromBytes(Arrays.StringToByteArray(traceparent, "00-".Length, "0af7651916cd43dd8448eb211c80319c".Length));
            var spanId = SpanId.FromBytes(Arrays.StringToByteArray(traceparent, "00-0af7651916cd43dd8448eb211c80319c-".Length, "00f067aa0ba902b7".Length));
            var optionsArray = Arrays.StringToByteArray(traceparent, "00-0af7651916cd43dd8448eb211c80319c-00f067aa0ba902b7-".Length, 2);

            var options = TraceOptions.Default;
            if ((optionsArray[0] | 1) == 1)
            {
                options = TraceOptions.Builder().SetIsSampled(true).Build();
            }

            List<KeyValuePair<string, string>> entries = new List<KeyValuePair<string, string>>();
            if (tracestateCollection != null)
            {
                foreach (var tracestate in tracestateCollection)
                {
                    // tracestate: rojo=00-0af7651916cd43dd8448eb211c80319c-00f067aa0ba902b7-01,congo=BleGNlZWRzIHRohbCBwbGVhc3VyZS4
                    var keyStartIdx = 0;
                    var length = tracestate.Length;
                    while (keyStartIdx < length)
                    {
                        var keyEndIdx = tracestate.IndexOf("=", keyStartIdx);

                        if (keyEndIdx == -1)
                        {
                            // error happened. Ignore this tracestate
                            break;
                        }

                        var valueStartIdx = keyEndIdx + 1;

                        var valueEndIdx = tracestate.IndexOf(",", valueStartIdx);
                        valueEndIdx = valueEndIdx == -1 ? length : valueEndIdx;
                        entries.Add(
                            new KeyValuePair<string, string>(
                                tracestate.Substring(keyStartIdx, keyEndIdx - keyStartIdx), 
                                tracestate.Substring(valueStartIdx, valueEndIdx - valueStartIdx)));
                        keyStartIdx = valueEndIdx + 1;
                    }
                }
            }

            var tracestateBuilder = Tracestate.Builder;

            entries.Reverse();
            foreach (var entry in entries)
            {
                tracestateBuilder.Set(entry.Key, entry.Value);
            }

            return SpanContext.Create(traceId, spanId, options, tracestateBuilder.Build());
        }

        /// <inheritdoc/>
        public override void Inject<T>(ISpanContext spanContext, T carrier, Action<T, string, string> setter)
        {
            var sb = new StringBuilder();
            sb.Append("00-").Append(spanContext.TraceId.ToLowerBase16());
            sb.Append('-').Append(spanContext.SpanId.ToLowerBase16());
            sb.Append('-').Append(spanContext.TraceOptions.IsSampled ? "01" : "00");
            setter(carrier, "traceparent", sb.ToString());

            sb.Clear();
            var isFirst = true;

            foreach (var entry in spanContext.Tracestate.Entries)
            {
                sb.Append(entry.Key).Append("=").Append(entry.Value);

                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    sb.Append(",");
                }
            }

            setter(carrier, "tracestate", sb.ToString());
        }
    }
}
