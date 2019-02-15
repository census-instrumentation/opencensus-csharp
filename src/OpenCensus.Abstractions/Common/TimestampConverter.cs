// <copyright file="TimestampConverter.cs" company="OpenCensus Authors">
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

namespace OpenCensus.Internal
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Converts nanoseconds into timestamp.
    /// </summary>
    public sealed class TimestampConverter
    {
        private readonly DateTimeOffset timestamp;
        private readonly Func<TimeSpan> stopwatch;

        private TimestampConverter(DateTimeOffset timestamp, Func<TimeSpan> watch)
        {
            this.timestamp = timestamp;
            this.stopwatch = watch;
        }

        /// <summary>
        /// Gets the time of creation of this timer.
        /// </summary>
        /// <returns>Time of this timer creation.</returns>
        public DateTimeOffset StartTime
        {
            get
            {
                return this.timestamp;
            }
        }

        /// <summary>
        /// Gets the current timestamp based on start time and high precision elapsed time.
        /// </summary>
        /// <returns>Current timestamp based on start time and high precision elapsed time.<returns>
        public DateTimeOffset Now
        {
            get
            {
                return this.timestamp.Add(this.stopwatch());
            }
        }

        /// <summary>
        /// Creates a new instance of a timer.
        /// </summary>
        /// <returns>New insance of a timer.</returns>
        public static TimestampConverter StartNew()
        {
            var stopwatch = Stopwatch.StartNew();
            return new TimestampConverter(DateTimeOffset.Now, () => stopwatch.Elapsed);
        }

        /// <summary>
        /// Creates a new instance of a timer with the given start time. Used for test purposes.
        /// </summary>
        /// <param name="time">Start time to use in this timer.</param>
        /// <param name="watch">Stopwatch to use. Should be started.</param>
        /// <returns>New instance of a timer.</returns>
        public static TimestampConverter StartNew(DateTimeOffset time, Func<TimeSpan> watch)
        {
            return new TimestampConverter(time, watch);
        }
    }
}
