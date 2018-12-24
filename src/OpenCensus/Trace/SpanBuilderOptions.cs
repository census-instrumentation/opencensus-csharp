﻿// <copyright file="SpanBuilderOptions.cs" company="OpenCensus Authors">
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

namespace OpenCensus.Trace
{
    using OpenCensus.Common;
    using OpenCensus.Trace.Config;
    using OpenCensus.Trace.Internal;

    internal class SpanBuilderOptions
    {
        internal SpanBuilderOptions(IRandomGenerator randomGenerator, IStartEndHandler startEndHandler, IClock clock, ITraceConfig traceConfig)
        {
            this.RandomHandler = randomGenerator;
            this.StartEndHandler = startEndHandler;
            this.Clock = clock;
            this.TraceConfig = traceConfig;
        }

        internal IRandomGenerator RandomHandler { get; }

        internal IStartEndHandler StartEndHandler { get; }

        internal IClock Clock { get; }

        internal ITraceConfig TraceConfig { get; }
    }
}
