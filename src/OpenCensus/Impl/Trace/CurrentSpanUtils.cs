// <copyright file="CurrentSpanUtils.cs" company="OpenCensus Authors">
// Copyright 2018, OpenCensus Authors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of theLicense at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

namespace OpenCensus.Trace
{
    using OpenCensus.Common;
    using OpenCensus.Trace.Unsafe;

    internal static class CurrentSpanUtils
    {
        public static ISpan CurrentSpan
        {
            get
            {
                return AsyncLocalContext.CurrentSpan;
            }
        }

        public static IScope WithSpan(ISpan span, bool endSpan)
        {
            return new ScopeInSpan(span, endSpan);
        }

        private sealed class ScopeInSpan : IScope
        {
            private readonly ISpan origContext;
            private readonly ISpan span;
            private readonly bool endSpan;

            public ScopeInSpan(ISpan span, bool endSpan)
            {
                this.span = span;
                this.endSpan = endSpan;
                origContext = AsyncLocalContext.CurrentSpan;
                AsyncLocalContext.CurrentSpan = span;
            }

            public void Dispose()
            {
                var current = AsyncLocalContext.CurrentSpan;
                AsyncLocalContext.CurrentSpan = origContext;

                if (current != origContext)
                {
                    // Log
                }

                if (endSpan)
                {
                    span.End();
                }
            }
        }
    }
}
