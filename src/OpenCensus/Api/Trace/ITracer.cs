// <copyright file="ITracer.cs" company="OpenCensus Authors">
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

    public interface ITracer
    {
        ISpan CurrentSpan { get; }

        IScope WithSpan(ISpan span);

        //<C> Callable<C> withSpan(Span span, final Callable<C> callable)
        //Runnable withSpan(Span span, Runnable runnable)
        ISpanBuilder SpanBuilder(string spanName);

        ISpanBuilder SpanBuilderWithExplicitParent(string spanName, ISpan parent = null);

        ISpanBuilder SpanBuilderWithRemoteParent(string spanName, ISpanContext remoteParentSpanContext = null);
    }
}
