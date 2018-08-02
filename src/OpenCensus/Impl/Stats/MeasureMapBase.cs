namespace OpenCensus.Stats
{
    using OpenCensus.Stats.Measures;
    using OpenCensus.Tags;

    public abstract class MeasureMapBase : IMeasureMap
    {

        public abstract IMeasureMap Put(IMeasureDouble measure, double value);

        public abstract IMeasureMap Put(IMeasureLong measure, long value);

        public abstract void Record();

        public abstract void Record(ITagContext tags);
    }
}
