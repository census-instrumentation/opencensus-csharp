// <copyright file="EndSpanOptions.cs" company="OpenCensus Authors">
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
    public class EndSpanOptions
    {
        public static readonly EndSpanOptions DEFAULT = new EndSpanOptions(false);

        internal EndSpanOptions()
        {
        }

        internal EndSpanOptions(bool sampleToLocalSpanStore, Status status = null)
        {
            this.SampleToLocalSpanStore = sampleToLocalSpanStore;
            this.Status = status;
        }

        public bool SampleToLocalSpanStore { get; }

        public Status Status { get; }

        public static EndSpanOptionsBuilder Builder()
        {
            return new EndSpanOptionsBuilder().SetSampleToLocalSpanStore(false);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }

            if (obj is EndSpanOptions that)
            {
                return (this.SampleToLocalSpanStore == that.SampleToLocalSpanStore)
                     && ((this.Status == null) ? (that.Status == null) : this.Status.Equals(that.Status));
            }

            return false;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int h = 1;
            h *= 1000003;
            h ^= this.SampleToLocalSpanStore ? 1231 : 1237;
            h *= 1000003;
            h ^= (this.Status == null) ? 0 : this.Status.GetHashCode();
            return h;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return "EndSpanOptions{"
                + "sampleToLocalSpanStore=" + this.SampleToLocalSpanStore + ", "
                + "status=" + this.Status
                + "}";
        }
    }
}
