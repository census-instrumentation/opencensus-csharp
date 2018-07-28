using OpenCensus.Stats.Measures;
using OpenCensus.Tags;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Stats
{
    internal sealed class NoopMeasureMap : MeasureMapBase
    {
        internal static readonly NoopMeasureMap INSTANCE = new NoopMeasureMap();

        public override IMeasureMap Put(IMeasureDouble measure, double value)
        {
            return this;
        }

        public override IMeasureMap Put(IMeasureLong measure, long value)
        {
            return this;
        }

        public override void Record() { }

        public override void Record(ITagContext tags)
        {
           if (tags == null)
            {
                throw new ArgumentNullException(nameof(tags));
            }
        }
    }
}
