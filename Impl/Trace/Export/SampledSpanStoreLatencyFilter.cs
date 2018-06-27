﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Steeltoe.Management.Census.Trace.Export
{
    public sealed class SampledSpanStoreLatencyFilter : ISampledSpanStoreLatencyFilter
    {
        public static ISampledSpanStoreLatencyFilter Create(string spanName, long latencyLowerNs, long latencyUpperNs, int maxSpansToReturn)
        {
            if (maxSpansToReturn < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxSpansToReturn));
            }
            if (latencyLowerNs < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(latencyLowerNs));
            }
            if (latencyUpperNs < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(latencyUpperNs));
            }
  
            return new SampledSpanStoreLatencyFilter(spanName, latencyLowerNs, latencyUpperNs, maxSpansToReturn);
        }

        public string SpanName { get; }

        public long LatencyLowerNs { get; }

        public long LatencyUpperNs { get; }

        public int MaxSpansToReturn { get; }

        internal SampledSpanStoreLatencyFilter(string spanName, long latencyLowerNs, long latencyUpperNs, int maxSpansToReturn)
        {
            if (spanName == null)
            {
                throw new ArgumentNullException(nameof(spanName));
            }
            SpanName = spanName;
            LatencyLowerNs = latencyLowerNs;
            LatencyUpperNs = latencyUpperNs;
            MaxSpansToReturn = maxSpansToReturn;
        }

        public override string ToString()
        {
            return "LatencyFilter{"
                + "spanName=" + SpanName + ", "
                + "latencyLowerNs=" + LatencyLowerNs + ", "
                + "latencyUpperNs=" + LatencyUpperNs + ", "
                + "maxSpansToReturn=" + MaxSpansToReturn
                + "}";
        }

        public override bool Equals(object o)
        {
            if (o == this)
            {
                return true;
            }
            if (o is SampledSpanStoreLatencyFilter) {
                SampledSpanStoreLatencyFilter that = (SampledSpanStoreLatencyFilter)o;
                return (this.SpanName.Equals(that.SpanName))
                     && (this.LatencyLowerNs == that.LatencyLowerNs)
                     && (this.LatencyUpperNs == that.LatencyUpperNs)
                     && (this.MaxSpansToReturn == that.MaxSpansToReturn);
            }
            return false;
        }

        public override int GetHashCode()
        {
            long h = 1;
            h *= 1000003;
            h ^= this.SpanName.GetHashCode();
            h *= 1000003;
            h ^= (this.LatencyLowerNs >> 32) ^ this.LatencyLowerNs;
            h *= 1000003;
            h ^= (this.LatencyUpperNs >> 32) ^ this.LatencyUpperNs;
            h *= 1000003;
            h ^= this.MaxSpansToReturn;
            return (int)h;
        }
    }
}
