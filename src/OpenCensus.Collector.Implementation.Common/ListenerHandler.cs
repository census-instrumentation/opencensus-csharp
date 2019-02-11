// <copyright file="ListenerHandler.cs" company="OpenCensus Authors">
// Copyright 2018, OpenCensus Authors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

namespace OpenCensus.Collector.Implementation.Common
{
    using System;
    using System.Diagnostics;
    using OpenCensus.Trace;

    internal abstract class ListenerHandler
    {
        protected readonly ITracer Tracer;

        protected readonly Func<Uri, ISampler> Sampler;

        public ListenerHandler(string sourceName, ITracer tracer, Func<Uri, ISampler> sampler)
        {
            this.SourceName = sourceName;
            this.Tracer = tracer;
            this.Sampler = sampler;
        }

        public string SourceName { get; }

        public abstract void OnStartActivity(Activity activity, object payload);

        // {
            // this.Tracer.SpanBuilder(activity.OperationName).SetSampler(this.Sampler).StartScopedSpan();
        // }

        public virtual void OnStopActivity(Activity activity, object payload)
        {
            var span = this.Tracer.CurrentSpan;

            if (span == null)
            {
                // TODO: Notify that span got lost
                return;
            }

            foreach (var tag in activity.Tags)
            {
                span.PutAttribute(tag.Key, AttributeValue.StringAttributeValue(tag.Value));
            }
        }

        public virtual void OnException(Activity activity, object payload)
        {
            var span = this.Tracer.CurrentSpan;

            // TODO: gather exception information
        }

        public virtual void OnCustom(string name, Activity activity, object payload)
        {
            // if custom handler needs to react on other events - this method should be overridden
        }
    }
}
