
using System;

namespace OpenCensus.Trace.Internal
{
    internal class RandomGenerator : IRandomGenerator
    {
        private static readonly Random _global = new Random();
        private static readonly object _lockObj = new object();

        private readonly int _seed;
        [ThreadStatic] private static Random _local;


        internal RandomGenerator()
        {
            _seed = _global.Next();
        }

        internal RandomGenerator(int seed)
        {
            _seed = seed;
        }

        public void NextBytes(byte[] bytes)
        {
            if (_local == null)
            {
                lock(_lockObj)
                {
                    if (_local == null)
                        _local = new Random(_seed);
                }
            }
            _local.NextBytes(bytes);
        }
    }
}
