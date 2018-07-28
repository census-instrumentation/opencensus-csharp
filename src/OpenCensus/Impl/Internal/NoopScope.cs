using OpenCensus.Common;
using OpenCensus.Trace;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Internal
{
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
