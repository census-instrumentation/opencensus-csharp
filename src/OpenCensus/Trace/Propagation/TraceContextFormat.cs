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
        private static readonly int VersionLength = "00".Length;
        private static readonly int VersionPrefixIdLength = "00-".Length;
        private static readonly int TraceIdLength = "0af7651916cd43dd8448eb211c80319c".Length;
        private static readonly int VersionAndTraceIdLength = "00-0af7651916cd43dd8448eb211c80319c-".Length;
        private static readonly int SpanIdLength = "00f067aa0ba902b7".Length;
        private static readonly int VersionAndTraceIdAndSpanIdLength = "00-0af7651916cd43dd8448eb211c80319c-00f067aa0ba902b7-".Length;
        private static readonly int OptionsLength = "00".Length;

        /// <inheritdoc/>
        public override ISet<string> Fields => new HashSet<string> { "tracestate", "traceparent" };

        /// <inheritdoc/>
        public override ISpanContext Extract<T>(T carrier, Func<T, string, IEnumerable<string>> getter)
        {
            try
            {
                var traceparentCollection = getter(carrier, "traceparent");
                var tracestateCollection = getter(carrier, "tracestate");

                if (traceparentCollection.Count() > 1)
                {
                    // multiple traceparent are not allowed
                    return null;
                }

                var traceparent = traceparentCollection?.FirstOrDefault();
                var traceparentParsed = this.TryExtractTraceparent(traceparent, out ITraceId traceId, out ISpanId spanId, out TraceOptions traceoptions, out bool bestAttempt);

                if (!traceparentParsed)
                {
                    return null;
                }

                if (bestAttempt)
                {
                    // should we fix up trace id, span id, etc.
                    // or span builder will do it?
                }

                var tracestateResult = Tracestate.Empty;
                try
                {
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
                                    // TODO: decide whether we need to parse other headers or just throw from here
                                    break;
                                }

                                var valueStartIdx = keyEndIdx + 1;

                                var valueEndIdx = tracestate.IndexOf(",", valueStartIdx);
                                valueEndIdx = valueEndIdx == -1 ? length : valueEndIdx;
                                entries.Add(
                                    new KeyValuePair<string, string>(
                                        tracestate.Substring(keyStartIdx, keyEndIdx - keyStartIdx).TrimStart(),
                                        tracestate.Substring(valueStartIdx, valueEndIdx - valueStartIdx).TrimEnd()));
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

                    tracestateResult = tracestateBuilder.Build();
                }
                catch (Exception ex)
                {
                    // failure to parse tracestate should not disregard traceparent
                    // TODO: logging
                }

                return SpanContext.Create(traceId, spanId, traceoptions, tracestateResult);
            }
            catch (Exception ex)
            {
                // TODO: logging
            }

            // in case of exception indicate to upstream that there is no parseable context from the top
            return null;
        }

        /// <inheritdoc/>
        public override void Inject<T>(ISpanContext spanContext, T carrier, Action<T, string, string> setter)
        {
            var traceparent = string.Concat("00-", spanContext.TraceId.ToLowerBase16(), "-", spanContext.SpanId.ToLowerBase16());
            traceparent = string.Concat(traceparent, spanContext.TraceOptions.IsSampled ? "-01" : "-00");

            setter(carrier, "traceparent", traceparent);

            StringBuilder sb = new StringBuilder();
            var isFirst = true;

            foreach (var entry in spanContext.Tracestate.Entries)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    sb.Append(",");
                }

                sb.Append(entry.Key).Append("=").Append(entry.Value);
            }

            if (sb.Length > 0)
            {
                setter(carrier, "tracestate", sb.ToString());
            }
        }

        private bool TryExtractTraceparent(string traceparent, out ITraceId traceId, out ISpanId spanId, out TraceOptions traceoptions, out bool bestAttempt)
        {
            // from https://github.com/w3c/distributed-tracing/blob/master/trace_context/HTTP_HEADER_FORMAT.md
            // traceparent: 00-0af7651916cd43dd8448eb211c80319c-00f067aa0ba902b7-01

            traceId = TraceId.Invalid;
            spanId = SpanId.Invalid;
            traceoptions = TraceOptions.Default;
            bestAttempt = false;

            if (traceparent == null)
            {
                return false;
            }

            // if version does not end with delimeter
            if (traceparent[VersionPrefixIdLength -1] != '-')
            {
                return false;
            }

            // or version is not a hex (will throw)
            var versionArray = Arrays.StringToByteArray(traceparent, 0, VersionLength);

            if (versionArray[0] >= 1)
            {
                // expected version is 01
                // for higher versions - best attempt parsing of trace id, span id, etc.
                bestAttempt = true;
            }

            if (traceparent[VersionAndTraceIdLength - 1] != '-')
            {
                return bestAttempt;
            }

            try
            {
                traceId = TraceId.FromBytes(Arrays.StringToByteArray(traceparent, VersionPrefixIdLength, TraceIdLength));
            }
            catch (ArgumentOutOfRangeException)
            {
                // it's ok to still parse tracestate
                return bestAttempt;
            }

            if (traceparent[VersionAndTraceIdAndSpanIdLength - 1] != '-')
            {
                return bestAttempt;
            }

            try
            {
                spanId = SpanId.FromBytes(Arrays.StringToByteArray(traceparent, VersionAndTraceIdLength, SpanIdLength));
            }
            catch (ArgumentOutOfRangeException)
            {
                // it's ok to still parse tracestate
                return bestAttempt;
            }

            byte[] optionsArray;

            try
            {
                optionsArray = Arrays.StringToByteArray(traceparent, VersionAndTraceIdAndSpanIdLength, OptionsLength);
            }
            catch (ArgumentOutOfRangeException)
            {
                // it's ok to still parse tracestate
                return bestAttempt;
            }

            if ((optionsArray[0] | 1) == 1)
            {
                traceoptions = TraceOptions.Builder().SetIsSampled(true).Build();
            }

            if (traceparent.Length != VersionAndTraceIdAndSpanIdLength + OptionsLength)
            {
                return bestAttempt;
            }

            return true;
        }
    }
}
