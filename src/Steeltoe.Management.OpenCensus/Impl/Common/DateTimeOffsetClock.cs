using System;

namespace Steeltoe.Management.Census.Common
{
    internal class DateTimeOffsetClock : IClock
    {
        internal const long MILLIS_PER_SECOND = 1000L;
        internal const long NANOS_PER_MILLI = 1000 * 1000;
        internal const long NANOS_PER_SECOND = NANOS_PER_MILLI * MILLIS_PER_SECOND;

        public static readonly DateTimeOffsetClock INSTANCE = new DateTimeOffsetClock();
        public static IClock GetInstance()
        {
            return INSTANCE;
        }

        public ITimestamp Now
        {
            get
            {
                var nowNanoTicks = NowNanos;
                double nowSecTicks = (double)nowNanoTicks / NANOS_PER_SECOND;
                var excessNanos = (int)((nowSecTicks - Math.Truncate(nowSecTicks)) * NANOS_PER_SECOND);
                return new Timestamp((long)nowSecTicks, excessNanos);
            }
        }

        public long NowNanos
        {
            get
            {
                var millis = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeMilliseconds();
                return millis * NANOS_PER_MILLI;
            }
        }
    }
}
