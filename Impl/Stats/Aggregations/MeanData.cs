﻿using Steeltoe.Management.Census.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Steeltoe.Management.Census.Stats.Aggregations
{
    public class MeanData : AggregationData, IMeanData
    {
        public double Mean { get; }
        public long Count { get; }
        public double Max { get; }
        public double Min { get; }

        internal MeanData(double mean, long count, double min, double max)
        {
            this.Mean = mean;
            this.Count = count;
            this.Min = min;
            this.Max = max;
        }

        public static IMeanData Create(double mean, long count, double min, double max)
        {
            return new MeanData(mean, count, min, max);
        }

        public override M Match<M>(
            Func<ISumDataDouble, M> p0,
            Func<ISumDataLong, M> p1,
            Func<ICountData, M> p2,
            Func<IMeanData, M> p3,
            Func<IDistributionData, M> p4,
            Func<ILastValueDataDouble, M> p5,
            Func<ILastValueDataLong, M> p6,
            Func<IAggregationData, M> defaultFunction)
        {
            return p3.Invoke(this);
        }


        public override String ToString()
        {
            return "MeanData{"
                + "mean=" + Mean + ", "
                + "min=" + Min + ", "
                + "max=" + Max + ", "
                + "count=" + Count
                + "}";
        }

        public override bool Equals(Object o)
        {
            if (o == this)
            {
                return true;
            }
            if (o is MeanData)
            {
                MeanData that = (MeanData)o;
                return (DoubleUtil.ToInt64(this.Mean) == DoubleUtil.ToInt64(that.Mean))
                        && (this.Count == that.Count)
                        && (DoubleUtil.ToInt64(this.Min) == DoubleUtil.ToInt64(that.Min))
                        && (DoubleUtil.ToInt64(this.Max) == DoubleUtil.ToInt64(that.Max));
            }
            return false;
        }

        public override int GetHashCode()
        {
            long h = 1;
            h *= 1000003;
            h ^= (DoubleUtil.ToInt64(this.Mean) >> 32) ^ DoubleUtil.ToInt64(this.Mean);
            h *= 1000003;
            h ^= (DoubleUtil.ToInt64(this.Min) >> 32) ^ DoubleUtil.ToInt64(this.Min);
            h *= 1000003;
            h ^= (DoubleUtil.ToInt64(this.Max) >> 32) ^ DoubleUtil.ToInt64(this.Max);
            h *= 1000003;
            h ^= (this.Count >> 32) ^ this.Count;
            return (int)h;
        }
    }
}
