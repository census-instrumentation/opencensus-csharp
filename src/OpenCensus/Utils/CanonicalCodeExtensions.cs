namespace OpenCensus.Utils
{
    using OpenCensus.Trace;

    public static class CanonicalCodeExtensions
    {
        public static Status ToStatus(this CanonicalCode code)
        {
            return new Status(code);
        }
    }
}
