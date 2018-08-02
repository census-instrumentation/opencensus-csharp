namespace OpenCensus.Internal
{
    using OpenCensus.Common;

    public sealed class NoopScope : IScope
    {
        public static readonly IScope INSTANCE = new NoopScope();

        public static IScope Instance
        {
            get
            {
                return INSTANCE;
            }
        }

        private NoopScope() { }

        public void Dispose()
        {
        }
    }
}
