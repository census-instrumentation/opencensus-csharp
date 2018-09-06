// <copyright file="TraceParams.cs" company="OpenCensus Authors">
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
    using OpenCensus.Trace.Sampler;

    public sealed class TraceParams : ITraceParams
    {
        public static readonly ITraceParams DEFAULT =
            new TraceParams(Samplers.GetProbabilitySampler(DefaultProbability), DefaultSpanMaxNumAttributes, DefaultSpanMaxNumAnnotations, DefaultSpanMaxNumMessageEvents, DefaultSpanMaxNumLinks);

        private const double DefaultProbability = 1e-4;
        private const int DefaultSpanMaxNumAttributes = 32;
        private const int DefaultSpanMaxNumAnnotations = 32;
        private const int DefaultSpanMaxNumMessageEvents = 128;
        private const int DefaultSpanMaxNumLinks = 128;

        internal TraceParams(ISampler sampler, int maxNumberOfAttributes, int maxNumberOfAnnotations, int maxNumberOfMessageEvents, int maxNumberOfLinks)
        {
            if (maxNumberOfAttributes <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxNumberOfAttributes));
            }

            if (maxNumberOfAnnotations <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxNumberOfAnnotations));
            }

            if (maxNumberOfMessageEvents <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxNumberOfMessageEvents));
            }

            if (maxNumberOfLinks <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxNumberOfLinks));
            }

            this.Sampler = sampler ?? throw new ArgumentNullException(nameof(sampler));
            this.MaxNumberOfAttributes = maxNumberOfAttributes;
            this.MaxNumberOfAnnotations = maxNumberOfAnnotations;
            this.MaxNumberOfMessageEvents = maxNumberOfMessageEvents;
            this.MaxNumberOfLinks = maxNumberOfLinks;
        }

        public ISampler Sampler { get; }

        public int MaxNumberOfAttributes { get; }

        public int MaxNumberOfAnnotations { get; }

        public int MaxNumberOfMessageEvents { get; }

        public int MaxNumberOfLinks { get; }

        public TraceParamsBuilder ToBuilder()
        {
            return new TraceParamsBuilder(this);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return "TraceParams{"
                + "sampler=" + this.Sampler + ", "
                + "maxNumberOfAttributes=" + this.MaxNumberOfAttributes + ", "
                + "maxNumberOfAnnotations=" + this.MaxNumberOfAnnotations + ", "
                + "maxNumberOfMessageEvents=" + this.MaxNumberOfMessageEvents + ", "
                + "maxNumberOfLinks=" + this.MaxNumberOfLinks
                + "}";
        }

    /// <inheritdoc/>
        public override bool Equals(object o)
        {
            if (o == this)
            {
                return true;
            }

            if (o is TraceParams that)
            {
                return this.Sampler.Equals(that.Sampler)
                     && (this.MaxNumberOfAttributes == that.MaxNumberOfAttributes)
                     && (this.MaxNumberOfAnnotations == that.MaxNumberOfAnnotations)
                     && (this.MaxNumberOfMessageEvents == that.MaxNumberOfMessageEvents)
                     && (this.MaxNumberOfLinks == that.MaxNumberOfLinks);
            }

            return false;
        }

    /// <inheritdoc/>
        public override int GetHashCode()
        {
            int h = 1;
            h *= 1000003;
            h ^= this.Sampler.GetHashCode();
            h *= 1000003;
            h ^= this.MaxNumberOfAttributes;
            h *= 1000003;
            h ^= this.MaxNumberOfAnnotations;
            h *= 1000003;
            h ^= this.MaxNumberOfMessageEvents;
            h *= 1000003;
            h ^= this.MaxNumberOfLinks;
            return h;
        }
    }
}
