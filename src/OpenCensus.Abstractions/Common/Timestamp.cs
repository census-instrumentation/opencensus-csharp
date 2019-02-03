// <copyright file="Timestamp.cs" company="OpenCensus Authors">
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

namespace OpenCensus.Common
{
    using System;

    /// <summary>
    /// Timestamp with the nanoseconds precision.
    /// </summary>
    public class Timestamp
    {
        private const long MaxSeconds = 315576000000L;
        private const int MaxNanos = 999999999;
        private const long MillisPerSecond = 1000L;
        private const long NanosPerMilli = 1000 * 1000;
        private const long NanosPerSecond = NanosPerMilli * MillisPerSecond;

        internal Timestamp(long seconds, int nanos)
        {
            this.Seconds = seconds;
            this.Nanos = nanos;
        }

        /// <summary>
        /// Gets the number of seconds since the Unix Epoch represented by this timestamp.
        /// </summary>
        public long Seconds { get; }

        /// <summary>
        /// Gets the the number of nanoseconds after the number of seconds since the Unix Epoch represented
        /// by this timestamp.
        /// </summary>
        public int Nanos { get; }

        public static Timestamp Create(long seconds, int nanos)
        {
            // TODO:
            if (seconds < -MaxSeconds || seconds > MaxSeconds)
            {
                return new Timestamp(0, 0);
            }

            if (nanos < 0 || nanos > MaxNanos)
            {
                return new Timestamp(0, 0);
            }

            return new Timestamp(seconds, nanos);
        }

        public static Timestamp FromMillis(long millis)
        {
            Timestamp zero = new Timestamp(0, 0);
            long nanos = millis * NanosPerMilli;
            return zero.Plus(0, nanos);
        }

        public static Timestamp FromDateTimeOffset(DateTimeOffset time)
        {
            long seconds = 0;
#if NET45
            var unixZero = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
            seconds = (int)Math.Floor(time.Subtract(unixZero).TotalSeconds);
#else
            seconds = time.ToUnixTimeSeconds();
#endif

            int nanos = (int)time.Subtract(new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).Subtract(TimeSpan.FromSeconds(seconds)).Ticks * 100;
            return Timestamp.Create(seconds, nanos);
        }

        /// <summary>
        /// Adds duration to the timestamp.
        /// </summary>
        /// <param name="duration">Duration to add to the timestamp.</param>
        /// <returns>Returns the timestamp with added duration.</returns>
        public Timestamp AddDuration(IDuration duration)
        {
            return this.Plus(duration.Seconds, duration.Nanos);
        }

        /// <summary>
        /// Adds nanosToAdd nanosecond to the current timestamp.
        /// </summary>
        /// <param name="nanosToAdd">Number of nanoseconds to add.</param>
        /// <returns>Returns the timstemp with added nanoseconds.</returns>
        public Timestamp AddNanos(long nanosToAdd)
        {
            return this.Plus(0, nanosToAdd);
        }

        /// <summary>
        /// Substructs timestamp from the current timestamp. Typically to calculate duration.
        /// </summary>
        /// <param name="timestamp">Timestamp to substruct.</param>
        /// <returns>Returns the timestamp with the substructed duration.</returns>
        public IDuration SubtractTimestamp(Timestamp timestamp)
        {
            long durationSeconds = this.Seconds - timestamp.Seconds;
            int durationNanos = this.Nanos - timestamp.Nanos;
            if (durationSeconds < 0 && durationNanos > 0)
            {
                durationSeconds += 1;
                durationNanos = (int)(durationNanos - NanosPerSecond);
            }
            else if (durationSeconds > 0 && durationNanos < 0)
            {
                durationSeconds -= 1;
                durationNanos = (int)(durationNanos + NanosPerSecond);
            }

            return Duration.Create(durationSeconds, durationNanos);
        }

        public int CompareTo(Timestamp other)
        {
            int cmp = (this.Seconds < other.Seconds) ? -1 : ((this.Seconds > other.Seconds) ? 1 : 0);
            if (cmp != 0)
            {
                return cmp;
            }

            return (this.Nanos < other.Nanos) ? -1 : ((this.Nanos > other.Nanos) ? 1 : 0);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return "Timestamp{"
                + "seconds=" + this.Seconds + ", "
                + "nanos=" + this.Nanos
                + "}";
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            long h = 1;
            h *= 1000003;
            h ^= (this.Seconds >> 32) ^ this.Seconds;
            h *= 1000003;
            h ^= this.Nanos;
            return (int)h;
        }

        private static Timestamp OfSecond(long seconds, long nanoAdjustment)
        {
            long floor = (long)Math.Floor((double)nanoAdjustment / NanosPerSecond);
            long secs = seconds + floor;
            long nos = nanoAdjustment - (floor * NanosPerSecond);
            return Create(secs, (int)nos);
        }

        private Timestamp Plus(long secondsToAdd, long nanosToAdd)
        {
            if ((secondsToAdd | nanosToAdd) == 0)
            {
                return this;
            }

            long sec = this.Seconds + secondsToAdd;
            long nanoSeconds = Math.DivRem(nanosToAdd, NanosPerSecond, out long nanosSpill);
            sec = sec + nanoSeconds;
            long nanoAdjustment = this.Nanos + nanosSpill;
            return OfSecond(sec, nanoAdjustment);
        }
    }
}
