namespace OpenCensus.Stats
{
    using OpenCensus.Stats.Measures;
    using OpenCensus.Tags;

    public interface IMeasureMap
    {
        IMeasureMap Put(IMeasureDouble measure, double value);

        IMeasureMap Put(IMeasureLong measure, long value);

        void Record();

        void Record(ITagContext tags);
    }
}
