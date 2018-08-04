// <copyright file="Tracing.cs" company="OpenCensus Authors">
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
    using OpenCensus.Common;
    using OpenCensus.Internal;
    using OpenCensus.Trace.Config;
    using OpenCensus.Trace.Export;
    using OpenCensus.Trace.Internal;
    using OpenCensus.Trace.Propagation;

    public sealed class Tracing
    {

        private static Tracing tracing = new Tracing();

        internal Tracing()
            : this(false)
        {
        }

        internal Tracing(bool enabled)
        {
            if (enabled)
            {
                traceComponent = new TraceComponent(DateTimeOffsetClock.INSTANCE, new RandomGenerator(), new SimpleEventQueue());
            }
            else
            {
                traceComponent = TraceComponent.NewNoopTraceComponent;
            }
        }

        private ITraceComponent traceComponent = null;

        public static ITracer Tracer
        {
            get
            {
                return tracing.traceComponent.Tracer;
            }
        }

        public static IPropagationComponent PropagationComponent
        {
            get
            {
                return tracing.traceComponent.PropagationComponent;
            }
        }

        public static IExportComponent ExportComponent
        {
            get
            {
                return tracing.traceComponent.ExportComponent;
            }
        }

        public static ITraceConfig TraceConfig
        {
            get
            {
                return tracing.traceComponent.TraceConfig;
            }
        }
    }
}
