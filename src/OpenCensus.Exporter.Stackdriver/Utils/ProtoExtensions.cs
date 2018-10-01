
namespace OpenCensus.Exporter.Stackdriver.Utils
{
    using Google.Protobuf.WellKnownTypes;
    using OpenCensus.Common;

    /// <summary>
    /// Translation methods from Opencensus structures to common
    /// Protobuf structures
    /// </summary>
    public static class ProtoExtensions
    {
        /// <summary>
        /// Translates Opencensus Timestamp to Protobuf's timestamp
        /// </summary>
        /// <param name="timestamp">Opencensus timestamp</param>
        /// <returns>Protobuf's timestamp</returns>
        public static Timestamp ToTimestamp(this ITimestamp timestamp)
        {
            return new Timestamp { Seconds = timestamp.Seconds, Nanos = timestamp.Nanos };
        }
    }
}
