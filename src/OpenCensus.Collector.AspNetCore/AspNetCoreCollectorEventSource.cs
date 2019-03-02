// <copyright file="AspNetCoreCollectorEventSource.cs" company="OpenCensus Authors">
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

namespace OpenCensus.Collector.AspNetCore
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Tracing;
    using System.Text;

    /// <summary>
    /// EventSource listing ETW events emitted from the project.
    /// </summary>
    [EventSource(Name = "OpenCensus.Collector.AspNetCore")]
    internal class AspNetCoreCollectorEventSource : EventSource
    {
        internal static AspNetCoreCollectorEventSource Log = new AspNetCoreCollectorEventSource();

        [Event(1, Message = "Context is NULL in end callback. Span will not be recorded.", Level = EventLevel.Warning)]
        public void NullContext()
        {
            this.WriteEvent(1);
        }

        [Event(2, Message = "Error getting custom sampler, the default sampler will be used", Level = EventLevel.Warning)]
        public void ExceptionInCustomSampler(string message, string stackTrace)
        {
            this.WriteEvent(2, message, stackTrace);
        }
    }
}
