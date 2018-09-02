// <copyright file="B3Format.cs" company="OpenCensus Authors">
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

    public sealed class B3Format : TextFormatBase
    {
        public const string XB3TraceId = "X-B3-TraceId";
        public const string XB3SpanId = "X-B3-SpanId";
        public const string XB3ParentSpanId = "X-B3-ParentSpanId";
        public const string XB3Sampled = "X-B3-Sampled";
        public const string XB3Flags = "X-B3-Flags";

        // Used as the upper TraceId.SIZE hex characters of the traceID. B3-propagation used to send
        // TraceId.SIZE hex characters (8-bytes traceId) in the past.
        internal const string UpperTraceId = "0000000000000000";

        // Sampled value via the X_B3_SAMPLED header.
        internal const string SampledValue = "1";

        // "Debug" sampled value.
        internal const string FlagsValue = "1";

        private static readonly List<string> FIELDS = new List<string>() { XB3TraceId, XB3SpanId, XB3ParentSpanId, XB3Sampled, XB3Flags };

        public override IList<string> Fields
        {
            get
            {
                return FIELDS.AsReadOnly();
            }
        }

        public override ISpanContext Extract<T>(T carrier, IGetter<T> getter)
        {
            if (carrier == null)
            {
                throw new ArgumentNullException(nameof(carrier));
            }

            if (getter == null)
            {
                throw new ArgumentNullException(nameof(getter));
            }

            try
            {
                ITraceId traceId;
                string traceIdStr = getter.Get(carrier, XB3TraceId);
                if (traceIdStr != null)
                {
                    if (traceIdStr.Length == TraceId.Size)
                    {
                        // This is an 8-byte traceID.
                        traceIdStr = UpperTraceId + traceIdStr;
                    }

                    traceId = TraceId.FromLowerBase16(traceIdStr);
                }
                else
                {
                    throw new SpanContextParseException("Missing X_B3_TRACE_ID.");
                }

                ISpanId spanId;
                string spanIdStr = getter.Get(carrier, XB3SpanId);
                if (spanIdStr != null)
                {
                    spanId = SpanId.FromLowerBase16(spanIdStr);
                }
                else
                {
                    throw new SpanContextParseException("Missing X_B3_SPAN_ID.");
                }

                TraceOptions traceOptions = TraceOptions.Default;
                if (SampledValue.Equals(getter.Get(carrier, XB3Sampled))
                    || FlagsValue.Equals(getter.Get(carrier, XB3Flags)))
                {
                    traceOptions = TraceOptions.Builder().SetIsSampled(true).Build();
                }

                return SpanContext.Create(traceId, spanId, traceOptions);
            }
            catch (Exception e)
            {
                throw new SpanContextParseException("Invalid input.", e);
            }
        }

        public override void Inject<T>(ISpanContext spanContext, T carrier, ISetter<T> setter)
        {
            if (spanContext == null)
            {
                throw new ArgumentNullException(nameof(spanContext));
            }

            if (carrier == null)
            {
                throw new ArgumentNullException(nameof(carrier));
            }

            if (setter == null)
            {
                throw new ArgumentNullException(nameof(setter));
            }

            setter.Put(carrier, XB3TraceId, spanContext.TraceId.ToLowerBase16());
            setter.Put(carrier, XB3SpanId, spanContext.SpanId.ToLowerBase16());
            if (spanContext.TraceOptions.IsSampled)
            {
                setter.Put(carrier, XB3Sampled, SampledValue);
            }
        }
    }
}
