namespace OpenCensus.Stats.Measurements
{
    public interface IMeasurementLong : IMeasurement
    {
        long Value { get; }
    }
}
