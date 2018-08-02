
using System;

namespace OpenCensus.Trace.Internal
{
    internal class RandomGenerator : IRandomGenerator
    {
        private static readonly Random _global = new Random();

        private readonly int _seed;
        private readonly bool _sameSeed;
        [ThreadStatic] private static Random _local;

        internal RandomGenerator()
        {
            _sameSeed = false;
        }

        /// <summary>
        /// This constructur uses the same seed for all the thread static random objects.
        /// You might get the same values if a random is accessed from different threads.
        /// Use only for unit tests...
        /// </summary>
        internal RandomGenerator(int seed)
        {
            _sameSeed = true;
            _seed = seed;
        }

        public void NextBytes(byte[] bytes)
        {            
            if (_local == null)
            {                
                _local = new Random(_sameSeed ? _seed : _global.Next());                
            }

            _local.NextBytes(bytes);
        }
    }
}
