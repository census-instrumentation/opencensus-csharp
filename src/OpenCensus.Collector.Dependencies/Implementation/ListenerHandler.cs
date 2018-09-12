// <copyright file="ListenerHandler.cs" company="OpenCensus Authors">
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

namespace OpenCensus.Collector.Dependencies.Implementation
{
    using System.Diagnostics;
    using System.Threading;
    using OpenCensus.Common;
    using OpenCensus.Trace;
    using OpenCensus.Trace.Sampler;

    internal class ListenerHandler
    {
        protected readonly ITracer Tracer;

        protected readonly ISampler Sampler;

        // TODO: fix IScope and AsyncLocalContext so that current is always available
        protected readonly AsyncLocal<IScope> LocalScope = new AsyncLocal<IScope>();

        public ListenerHandler(string sourceName, ITracer tracer, ISampler sampler)
        {
            this.SourceName = sourceName;
            this.Tracer = tracer;
            this.Sampler = sampler;
        }

        public string SourceName { get; }

        public virtual void OnStartActivity(Activity activity, object payload)
        {
            this.LocalScope.Value = this.Tracer.SpanBuilder(activity.OperationName).SetRecordEvents(true).SetSampler(this.Sampler).StartScopedSpan();
        }

        public virtual void OnStopActivity(Activity activity, object payload)
        {
            var span = this.Tracer.CurrentSpan;
            foreach (var tag in activity.Tags)
            {
                span.PutAttribute(tag.Key, AttributeValue.StringAttributeValue(tag.Value));
            }

            this.LocalScope.Value?.Dispose();
        }
    }
}
