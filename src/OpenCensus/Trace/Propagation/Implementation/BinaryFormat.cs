// <copyright file="BinaryFormat.cs" company="OpenCensus Authors">
// Copyright 2018, OpenCensus Authors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

using System.Diagnostics;

namespace OpenCensus.Trace.Propagation.Implementation
{
    using System;

    internal class BinaryFormat : BinaryFormatBase
    {
        private const byte VersionId = 0;
        private const int VersionIdOffset = 0;

        // The version_id/field_id size in bytes.
        private const byte IdSize = 1;
        private const byte TraceIdFieldId = 0;
        private const int TraceIdFieldIdOffset = VersionIdOffset + IdSize;
        private const int TraceIdOffset = TraceIdFieldIdOffset + IdSize;
        private const byte SpanIdFieldId = 1;
        private const int TraceIdSize = 16;
        private const int SpanIdFieldIdOffset = TraceIdOffset + TraceIdSize;
        private const int SpanIdOffset = SpanIdFieldIdOffset + IdSize;
        private const byte TraceOptionsFieldId = 2;
        private const int SpanIdSize = 8;
        private const int TraceOptionFieldIdOffset = SpanIdOffset + SpanIdSize;
        private const int TraceOptionOffset = TraceOptionFieldIdOffset + IdSize;
        private const int FormatLength = (4 * IdSize) + TraceIdSize + SpanIdSize + TraceOptions.Size;

        public override ISpanContext FromByteArray(byte[] bytes)
        {
            // TODO we should rewrite it with Span<byte>

            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            if (bytes.Length == 0 || bytes[0] != VersionId)
            {
                throw new SpanContextParseException("Unsupported version.");
            }

            ActivityTraceId traceId = default;
            ActivitySpanId spanId = default;
            TraceOptions traceOptions = TraceOptions.Default;

            int pos = 1;
            try
            {
                Span<byte> bytesSpan = new Span<byte>(bytes);
                if (bytes.Length > pos && bytes[pos] == TraceIdFieldId)
                {
                    traceId = ActivityTraceId.CreateFromBytes(bytesSpan.Slice(pos + IdSize, 16));
                    pos += IdSize + TraceIdSize;
                }

                if (bytes.Length > pos && bytes[pos] == SpanIdFieldId)
                {
                    spanId = ActivitySpanId.CreateFromBytes(bytesSpan.Slice(pos + IdSize, 8));
                    pos += IdSize + SpanIdSize;
                }

                if (bytes.Length > pos && bytes[pos] == TraceOptionsFieldId)
                {
                    traceOptions = TraceOptions.FromBytes(bytes, pos + IdSize);
                }

                return SpanContext.Create(traceId, spanId, traceOptions, Tracestate.Empty);
            }
            catch (Exception e)
            {
                throw new SpanContextParseException("Invalid input.", e);
            }
        }

        public override byte[] ToByteArray(ISpanContext spanContext)
        {
            if (spanContext == null)
            {
                throw new ArgumentNullException(nameof(spanContext));
            }

            // TODO we should do it with Span<t>
            byte[] bytes = new byte[FormatLength];
            bytes[VersionIdOffset] = VersionId;
            bytes[TraceIdFieldIdOffset] = TraceIdFieldId;

            // todo optimize
            Span<byte> traceIdBytes = stackalloc byte[TraceIdSize];
            spanContext.TraceId.CopyTo(traceIdBytes);

            for (int i = 0; i < TraceIdSize; i++)
            {
                bytes[TraceIdOffset + i] = traceIdBytes[i];
            }

            bytes[SpanIdFieldIdOffset] = SpanIdFieldId;
            Span<byte> spanIdBytes = stackalloc byte[SpanIdSize];
            spanContext.SpanId.CopyTo(spanIdBytes);

            for (int i = 0; i < SpanIdSize; i++)
            {
                bytes[SpanIdOffset + i] = spanIdBytes[i];
            }

            bytes[TraceOptionFieldIdOffset] = TraceOptionsFieldId;
            spanContext.TraceOptions.CopyBytesTo(bytes, TraceOptionOffset);
            return bytes;
        }
    }
}
