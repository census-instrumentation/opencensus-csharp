// <copyright file="TraceComponentBase.cs" company="OpenCensus Authors">
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
    using OpenCensus.Trace.Config;
    using OpenCensus.Trace.Export;
    using OpenCensus.Trace.Propagation;

    public abstract class TraceComponentBase : ITraceComponent
    {
        public abstract ITracer Tracer { get; }

        public abstract IPropagationComponent PropagationComponent { get; }

        public abstract IClock Clock { get; }

        public abstract IExportComponent ExportComponent { get; }

        public abstract ITraceConfig TraceConfig { get; }

        internal static ITraceComponent NewNoopTraceComponent
        {
            get
            {
                return new NoopTraceComponent();
            }
        }
    }
}
