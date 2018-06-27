﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Steeltoe.Management.Census.Trace.Export
{
    public sealed class TimedEvents<T> : ITimedEvents<T>
    {
        public static ITimedEvents<T> Create(IList<ITimedEvent<T>> events, int droppedEventsCount)
        {
            if (events == null)
            {
                throw new ArgumentNullException(nameof(events));
            }
            List<ITimedEvent<T>> ev = new List<ITimedEvent<T>>();
            ev.AddRange(events);
            return new TimedEvents<T>(ev.AsReadOnly(), droppedEventsCount);
        }

        public IList<ITimedEvent<T>> Events { get; }
        public int DroppedEventsCount { get; }

        internal TimedEvents(IList<ITimedEvent<T>> events, int droppedEventsCount)
        {
            if (events == null)
            {
                throw new ArgumentNullException("Null events");
            }
            this.Events = events;
            this.DroppedEventsCount = droppedEventsCount;
        }

        public override string ToString()
        {
            return "TimedEvents{"
                + "events=" + Events + ", "
                + "droppedEventsCount=" + DroppedEventsCount
                + "}";
        }

        public override bool Equals(object o)
        {
            if (o == this)
            {
                return true;
            }
            if (o is TimedEvents<T>)
            {
                TimedEvents<T> that = (TimedEvents<T>)o;
                return (this.Events.SequenceEqual(that.Events))
                     && (this.DroppedEventsCount == that.DroppedEventsCount);
            }
            return false;
        }

        public override int GetHashCode()
        {
            int h = 1;
            h *= 1000003;
            h ^= this.Events.GetHashCode();
            h *= 1000003;
            h ^= this.DroppedEventsCount;
            return h;
        }
    }
}
