
namespace OpenCensus.Exporter.Stackdriver.Utils
{
    using Google.Protobuf.WellKnownTypes;
    using OpenCensus.Common;

    public static class ProtoExtensions
    {
        public static Timestamp ToTimestamp(this ITimestamp timestamp)
        {
            return new Timestamp { Seconds = timestamp.Seconds, Nanos = timestamp.Nanos };
        }
    }
}
