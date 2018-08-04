// <copyright file="Duration.cs" company="OpenCensus Authors">
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

namespace OpenCensus.Common
{
    public class Duration : IDuration
    {
        private const long MAX_SECONDS = 315576000000L;
        private const int MAX_NANOS = 999999999;
        private static readonly IDuration ZERO = new Duration(0, 0);

        public Duration(long seconds, int nanos)
        {
            this.Seconds = seconds;
            this.Nanos = nanos;
        }

        public long Seconds { get; }

        public int Nanos { get; }

        public static IDuration Create(long seconds, int nanos)
        {
            if (seconds < -MAX_SECONDS || seconds > MAX_SECONDS)
            {
                return ZERO;
            }

            if (nanos < -MAX_NANOS || nanos > MAX_NANOS)
            {
                return ZERO;
            }

            if ((seconds < 0 && nanos > 0) || (seconds > 0 && nanos < 0))
            {
                return ZERO;
            }

            return new Duration(seconds, nanos);
        }

        public int CompareTo(IDuration other)
        {
            int cmp = (this.Seconds < other.Seconds) ? -1 : ((this.Seconds > other.Seconds) ? 1 : 0);
            if (cmp != 0)
            {
                return cmp;
            }

            return (this.Nanos < other.Nanos) ? -1 : ((this.Nanos > other.Nanos) ? 1 : 0);
        }

        public override string ToString()
        {
            return "Duration{"
                + "seconds=" + this.Seconds + ", "
                + "nanos=" + this.Nanos
                + "}";
        }

        public override bool Equals(object o)
        {
            if (o == this)
            {
                return true;
            }

            if (o is Duration that)
            {
                return (this.Seconds == that.Seconds)
                     && (this.Nanos == that.Nanos);
            }

            return false;
        }

        public override int GetHashCode()
        {
            long h = 1;
            h *= 1000003;
            h ^= (this.Seconds >> 32) ^ this.Seconds;
            h *= 1000003;
            h ^= this.Nanos;
            return (int)h;
        }
    }
}
