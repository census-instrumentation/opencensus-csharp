namespace OpenCensus.Trace.Internal
{
    using System;

    internal class RandomGenerator : IRandomGenerator
    {
        private static readonly Random global = new Random();

        private readonly int seed;
        private readonly bool sameSeed;
        [ThreadStatic] private static Random local;

        internal RandomGenerator()
        {
            sameSeed = false;
        }

        /// <summary>
        /// This constructur uses the same seed for all the thread static random objects.
        /// You might get the same values if a random is accessed from different threads.
        /// Use only for unit tests...
        /// </summary>
        internal RandomGenerator(int seed)
        {
            sameSeed = true;
            this.seed = seed;
        }

        public void NextBytes(byte[] bytes)
        {
            if (local == null)
            {
                local = new Random(sameSeed ? seed : global.Next());
            }

            local.NextBytes(bytes);
        }
    }
}
