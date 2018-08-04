// <copyright file="Timestamp.cs" company="OpenCensus Authors">
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
    using System;

    public class Timestamp : ITimestamp
    {
        private const long MAX_SECONDS = 315576000000L;
        private const int MAX_NANOS = 999999999;
        private const long MILLIS_PER_SECOND = 1000L;
        private const long NANOS_PER_MILLI = 1000 * 1000;
        private const long NANOS_PER_SECOND = NANOS_PER_MILLI * MILLIS_PER_SECOND;

        internal Timestamp(long seconds, int nanos)
        {
            this.Seconds = seconds;
            this.Nanos = nanos;
        }

        public long Seconds { get; }

        public int Nanos { get; }

        public static ITimestamp Create(long seconds, int nanos)
        {
            // TODO:
            if (seconds < -MAX_SECONDS || seconds > MAX_SECONDS)
            {
                return new Timestamp(0, 0);
            }

            if (nanos < 0 || nanos > MAX_NANOS)
            {
                return new Timestamp(0, 0);
            }

            return new Timestamp(seconds, nanos);
        }

        public static ITimestamp FromMillis(long millis)
        {
            Timestamp zero = new Timestamp(0, 0);
            long nanos = millis * NANOS_PER_MILLI;
            return zero.Plus(0, nanos);
        }

        public ITimestamp AddDuration(IDuration duration)
        {
            return this.Plus(duration.Seconds, duration.Nanos);
        }

        public ITimestamp AddNanos(long nanosToAdd)
        {
            return this.Plus(0, nanosToAdd);
        }

        public IDuration SubtractTimestamp(ITimestamp timestamp)
        {
            long durationSeconds = this.Seconds - timestamp.Seconds;
            int durationNanos = this.Nanos - timestamp.Nanos;
            if (durationSeconds < 0 && durationNanos > 0)
            {
                durationSeconds += 1;
                durationNanos = (int)(durationNanos - NANOS_PER_SECOND);
            }
            else if (durationSeconds > 0 && durationNanos < 0)
            {
                durationSeconds -= 1;
                durationNanos = (int)(durationNanos + NANOS_PER_SECOND);
            }

            return Duration.Create(durationSeconds, durationNanos);
        }

        public int CompareTo(ITimestamp other)
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
            return "Timestamp{"
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

            if (o is Timestamp that)
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

        private static ITimestamp OfSecond(long seconds, long nanoAdjustment)
        {
            long floor = (long)Math.Floor((double)nanoAdjustment / NANOS_PER_SECOND);
            long secs = seconds + floor;
            long nos = nanoAdjustment - (floor * NANOS_PER_SECOND);
            return Create(secs, (int)nos);
        }

        private ITimestamp Plus(long secondsToAdd, long nanosToAdd)
        {
            if ((secondsToAdd | nanosToAdd) == 0)
            {
                return this;
            }

            long sec = this.Seconds + secondsToAdd;
            long nanoSeconds = Math.DivRem(nanosToAdd, NANOS_PER_SECOND, out long nanosSpill);
            sec = sec + nanoSeconds;
            long nanoAdjustment = this.Nanos + nanosSpill;
            return OfSecond(sec, nanoAdjustment);
        }
    }
}
