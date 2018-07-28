using OpenCensus.Common;
using OpenCensus.Tags.Unsafe;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Tags
{
    internal static class CurrentTagContextUtils
    {
        internal static ITagContext CurrentTagContext
        {
            get { return AsyncLocalContext.CurrentTagContext; }
        }

        internal static IScope WithTagContext(ITagContext tags)
        {
            return new WithTagContextScope(tags);
        }

        private sealed class WithTagContextScope : IScope
        {

            private readonly ITagContext origContext;

            public WithTagContextScope(ITagContext tags)
            {
                origContext = AsyncLocalContext.CurrentTagContext;
                AsyncLocalContext.CurrentTagContext = tags;
            }


            public void Dispose()
            {
                var current = AsyncLocalContext.CurrentTagContext;
                AsyncLocalContext.CurrentTagContext = origContext;

                if (current != origContext)
                {
                    // Log
                }
            }
        }
    }
}
