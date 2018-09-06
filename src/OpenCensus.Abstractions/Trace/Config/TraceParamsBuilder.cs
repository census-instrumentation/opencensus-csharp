﻿// <copyright file="TraceParamsBuilder.cs" company="OpenCensus Authors">
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

namespace OpenCensus.Trace.Config
{
    using System;

    public sealed class TraceParamsBuilder
    {
        private ISampler sampler;
        private int? maxNumberOfAttributes;
        private int? maxNumberOfAnnotations;
        private int? maxNumberOfMessageEvents;
        private int? maxNumberOfLinks;

        internal TraceParamsBuilder(TraceParams source)
        {
            this.sampler = source.Sampler;
            this.maxNumberOfAttributes = source.MaxNumberOfAttributes;
            this.maxNumberOfAnnotations = source.MaxNumberOfAnnotations;
            this.maxNumberOfMessageEvents = source.MaxNumberOfMessageEvents;
            this.maxNumberOfLinks = source.MaxNumberOfLinks;
        }

        public TraceParamsBuilder SetSampler(ISampler sampler)
        {
            this.sampler = sampler ?? throw new ArgumentNullException("Null sampler");
            return this;
        }

        public TraceParamsBuilder SetMaxNumberOfAttributes(int maxNumberOfAttributes)
        {
            this.maxNumberOfAttributes = maxNumberOfAttributes;
            return this;
        }

        public TraceParamsBuilder SetMaxNumberOfAnnotations(int maxNumberOfAnnotations)
        {
            this.maxNumberOfAnnotations = maxNumberOfAnnotations;
            return this;
        }

        public TraceParamsBuilder SetMaxNumberOfMessageEvents(int maxNumberOfMessageEvents)
        {
            this.maxNumberOfMessageEvents = maxNumberOfMessageEvents;
            return this;
        }

        public TraceParamsBuilder SetMaxNumberOfLinks(int maxNumberOfLinks)
        {
            this.maxNumberOfLinks = maxNumberOfLinks;
            return this;
        }

        public TraceParams Build()
        {
            string missing = string.Empty;
            if (this.sampler == null)
            {
                missing += " sampler";
            }

            if (!this.maxNumberOfAttributes.HasValue)
            {
                missing += " maxNumberOfAttributes";
            }

            if (!this.maxNumberOfAnnotations.HasValue)
            {
                missing += " maxNumberOfAnnotations";
            }

            if (!this.maxNumberOfMessageEvents.HasValue)
            {
                missing += " maxNumberOfMessageEvents";
            }

            if (!this.maxNumberOfLinks.HasValue)
            {
                missing += " maxNumberOfLinks";
            }

            if (!string.IsNullOrEmpty(missing))
            {
                throw new ArgumentOutOfRangeException("Missing required properties:" + missing);
            }

            return new TraceParams(
                this.sampler,
                this.maxNumberOfAttributes.Value,
                this.maxNumberOfAnnotations.Value,
                this.maxNumberOfMessageEvents.Value,
                this.maxNumberOfLinks.Value);
        }
    }
}
