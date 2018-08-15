// <copyright file="TimedEvent.cs" company="OpenCensus Authors">
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

namespace OpenCensus.Trace.Export
{

    using System;
    using OpenCensus.Common;

    public sealed class TimedEvent<T> : ITimedEvent<T>
    {
        public static ITimedEvent<T> Create(DateTimeOffset timestamp, T @event)
        {
            return new TimedEvent<T>(timestamp, @event);
        }

        public DateTimeOffset Timestamp { get; }

        public T Event { get; }

        internal TimedEvent(DateTimeOffset timestamp, T @event)
        {
            this.Timestamp = timestamp;
            this.Event = @event;
        }

        public override string ToString()
        {
            return "TimedEvent{"
                + "timestamp=" + this.Timestamp + ", "
                + "event=" + this.Event
                + "}";
        }

        public override bool Equals(object o)
        {
            if (o == this)
            {
                return true;
            }

            if (o is TimedEvent<T> that)
            {
                return this.Timestamp.Equals(that.Timestamp)
                     && this.Event.Equals(that.Event);
            }

            return false;
        }

        public override int GetHashCode()
        {
            int h = 1;
            h *= 1000003;
            h ^= this.Timestamp.GetHashCode();
            h *= 1000003;
            h ^= this.Event.GetHashCode();
            return h;
        }
    }
}
