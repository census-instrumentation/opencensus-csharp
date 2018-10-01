﻿// <copyright file="DefaultPropagationComponent.cs" company="OpenCensus Authors">
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

namespace OpenCensus.Trace.Propagation
{
    using OpenCensus.Trace.Propagation.Implementation;

    /// <summary>
    /// Default propagation used by Open Census.
    /// </summary>
    public sealed class DefaultPropagationComponent : PropagationComponentBase
    {
        private readonly BinaryFormat binaryFormat = new BinaryFormat();
        private readonly TraceContextFormat b3Format = new TraceContextFormat();

        /// <inheritdoc/>
        public override IBinaryFormat BinaryFormat
        {
            get
            {
                return this.binaryFormat;
            }
        }

        /// <inheritdoc/>
        public override ITextFormat TextFormat
        {
            get
            {
                return this.b3Format;
            }
        }
    }
}