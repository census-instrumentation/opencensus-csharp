// <copyright file="TraceComponent.cs" company="OpenCensus Authors">
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

    public sealed class TraceComponent : TraceComponentBase
    {
        public TraceComponent()
            : this(DateTimeOffsetClock.Instance, new RandomGenerator(), new SimpleEventQueue())
        {
        }

        public TraceComponent(IClock clock, IRandomGenerator randomHandler, IEventQueue eventQueue)
        {
            this.Clock = clock;
            this.TraceConfig = new Config.TraceConfig();

            // TODO(bdrutu): Add a config/argument for supportInProcessStores.
            if (eventQueue is SimpleEventQueue)
            {
                this.ExportComponent = Export.ExportComponent.CreateWithoutInProcessStores(eventQueue);
            }
            else
            {
                this.ExportComponent = Export.ExportComponent.CreateWithInProcessStores(eventQueue);
            }

            this.PropagationComponent = new PropagationComponent();
            IStartEndHandler startEndHandler =
                new StartEndHandler(
                    this.ExportComponent.SpanExporter,
                    this.ExportComponent.RunningSpanStore,
                    this.ExportComponent.SampledSpanStore,
                    eventQueue);
            this.Tracer = new Tracer(randomHandler, startEndHandler, clock, this.TraceConfig);
        }

        public override ITracer Tracer { get; }

        public override IPropagationComponent PropagationComponent { get; }

        public override IClock Clock { get; }

        public override IExportComponent ExportComponent { get; }

        public override ITraceConfig TraceConfig { get; }
    }
}
