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
    public class AspNetCoreCollectorEventSource : EventSource
    {
        public static AspNetCoreCollectorEventSource Log = new AspNetCoreCollectorEventSource();

        /// <summary>
        /// Log Context is null event
        /// </summary>
        [Event(100, Message = "Context is NULL", Level = EventLevel.Warning)]
        public void NullContext()
        {
            this.WriteEvent(100);
        }

        /// <summary>
        /// Exception executing Custom Sampler event.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="stackTrace">Exception stack trace.</param>
        [Event(200, Message = "Exception executing Custom Sampler", Level = EventLevel.Warning)]
        public void ExceptionInCustomSampler(string message, string stackTrace)
        {
            this.WriteEvent(200, message, stackTrace);
        }
    }
}
