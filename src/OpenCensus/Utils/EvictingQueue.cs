namespace OpenCensus.Utils
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class EvictingQueue<T> : IEnumerable<T>, IEnumerable
    {
        private readonly Queue<T> @delegate;
        private readonly int maxSize;

        public int Count
        {
            get
            {
                return @delegate.Count;
            }
        }

        public EvictingQueue(int maxSize)
        {
            if (maxSize < 0)
            {
                throw new ArgumentOutOfRangeException("maxSize must be >= 0");
            }

            this.maxSize = maxSize;
            @delegate = new Queue<T>(maxSize);
        }

        public int RemainingCapacity()
        {
            return maxSize - @delegate.Count;
        }

        public bool Offer(T e)
        {
            return Add(e);
        }

        public bool Add(T e)
        {
            if (e == null)
            {
                throw new ArgumentNullException();
            }

            if (maxSize == 0)
            {
                return true;
            }

            if (@delegate.Count == maxSize)
            {
                @delegate.Dequeue();
            }

            @delegate.Enqueue(e);
            return true;
        }

        public bool AddAll(ICollection<T> collection)
        {
            foreach (var e in collection)
            {
                Add(e);
            }

            return true;
        }

        public bool Contains(T e)
        {
            if (e == null)
            {
                throw new ArgumentNullException();
            }

            return @delegate.Contains(e);
        }

        public T[] ToArray()
        {
            return @delegate.ToArray();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return @delegate.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return @delegate.GetEnumerator();
        }
    }
}
