// <copyright file="Samplers.cs" company="OpenCensus Authors">
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

namespace OpenCensus.Trace.Sampler
{
    public sealed class Samplers
    {
        private static readonly ISampler AlwaysSampleInstance = new AlwaysSampleSampler();
        private static readonly ISampler NeverSampleInstance = new NeverSampleSampler();

        public static ISampler AlwaysSample
        {
            get
            {
                return AlwaysSampleInstance;
            }
        }

        public static ISampler NeverSample
        {
            get
            {
                return NeverSampleInstance;
            }
        }

        public static ISampler GetProbabilitySampler(double probability)
        {
            return ProbabilitySampler.Create(probability);
        }
    }
}
