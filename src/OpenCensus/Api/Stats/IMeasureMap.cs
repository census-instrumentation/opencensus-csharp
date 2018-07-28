using OpenCensus.Stats.Measures;
using OpenCensus.Tags;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Stats
{
    public interface IMeasureMap
    {
        IMeasureMap Put(IMeasureDouble measure, double value);


        IMeasureMap Put(IMeasureLong measure, long value);


        void Record();


        void Record(ITagContext tags);
    }
}
