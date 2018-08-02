namespace OpenCensus.Utils
{
    using System;

    internal static class DoubleUtil
    {
        public static long ToInt64(double arg)
        {

            if (double.IsPositiveInfinity(arg))
            {
                return 0x7ff0000000000000L;
            }

            if (double.IsNegativeInfinity(arg))
            {
                unchecked
                {
                    return (long)0xfff0000000000000L;
                }
            }

            if (double.IsNaN(arg))
            {
                return 0x7ff8000000000000L;
            }

            if (arg == double.MaxValue)
            {
                return long.MaxValue;
            }

            if (arg == double.MinValue)
            {
                return long.MinValue;
            }

            return Convert.ToInt64(arg);
        }
    }
}
