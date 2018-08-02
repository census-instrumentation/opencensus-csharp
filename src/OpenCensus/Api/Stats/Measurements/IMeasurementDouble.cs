namespace OpenCensus.Stats.Measurements
{
    public interface IMeasurementDouble : IMeasurement
    {
        double Value { get; }
    }
}
