// <copyright file="TraceOptions.cs" company="OpenCensus Authors">
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
    using System;

    public sealed class TraceOptions
    {
        // Default options. Nothing set.
        internal const byte DefaultOptions = 0;

        // Bit to represent whether trace is sampled or not.
        internal const byte IsSampledBit = 0x1;

        public const int Size = 1;

        public static readonly TraceOptions Default = new TraceOptions(DefaultOptions);
        public static readonly TraceOptions Sampled = new TraceOptions(1);

        // The set of enabled features is determined by all the enabled bits.
        private byte options;

        // Creates a new TraceOptions with the given options.
        internal TraceOptions(byte options)
        {
            this.options = options;
        }

        public static TraceOptions FromBytes(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if (buffer.Length != Size)
            {
                throw new ArgumentOutOfRangeException(string.Format("Invalid size: expected {0}, got {1}", Size, buffer.Length));
            }

            byte[] bytesCopied = new byte[Size];
            Buffer.BlockCopy(buffer, 0, bytesCopied, 0, Size);
            return new TraceOptions(bytesCopied[0]);
        }

        public static TraceOptions FromBytes(byte[] src, int srcOffset)
        {
            if (srcOffset < 0 || srcOffset >= src.Length)
            {
                throw new IndexOutOfRangeException("srcOffset");
            }

            return new TraceOptions(src[srcOffset]);
        }

        public static TraceOptionsBuilder Builder()
        {
            return new TraceOptionsBuilder(DefaultOptions);
        }

        public static TraceOptionsBuilder Builder(TraceOptions traceOptions)
        {
            return new TraceOptionsBuilder(traceOptions.options);
        }

        public byte[] Bytes
        {
            get
            {
                byte[] bytes = new byte[Size];
                bytes[0] = this.options;
                return bytes;
            }
        }

        public bool IsSampled
        {
            get
            {
                return this.HasOption(IsSampledBit);
            }
        }

        public void CopyBytesTo(byte[] dest, int destOffset)
        {
            if (destOffset < 0 || destOffset >= dest.Length)
            {
                throw new IndexOutOfRangeException("destOffset");
            }

            dest[destOffset] = this.options;
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }

            if (!(obj is TraceOptions))
            {
                return false;
            }

            TraceOptions that = (TraceOptions)obj;
            return this.options == that.options;
        }

        public override int GetHashCode()
        {
            int result = (31 * 1) + this.options;
            return result;
        }

        public override string ToString()
        {
            return "TraceOptions{"
                + "sampled=" + this.IsSampled
                + "}";
        }

        internal sbyte Options
        {
            get { return (sbyte)this.options; }
        }

        private bool HasOption(int mask)
        {
            return (this.options & mask) != 0;
        }

        private void ClearOption(int mask)
        {
            this.options = (byte)(this.options & ~mask);
        }

        private void SetOption(int mask)
        {
            this.options = (byte)(this.options | mask);
        }
    }
}
