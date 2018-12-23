// <copyright file="DependenciesCollector.cs" company="OpenCensus Authors">
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

namespace OpenCensus.Collector.Dependencies
{
    using System;
    using System.Collections.Generic;
    using OpenCensus.Collector.Dependencies.Implementation;
    using OpenCensus.Collector.Implementation.Common;
    using OpenCensus.Trace;
    using OpenCensus.Trace.Propagation;

    /// <summary>
    /// Dependencies collector.
    /// </summary>
    public class DependenciesCollector : IDisposable
    {
        private readonly DiagnosticSourceSubscriber diagnosticSourceSubscriber;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependenciesCollector"/> class.
        /// </summary>
        /// <param name="options">Configuration options for dependencies collector.</param>
        /// <param name="tracer">Tracer to record traced with.</param>
        /// <param name="sampler">Sampler to use to sample dependnecy calls.</param>
        /// <param name="propagationComponent">Propagation component to use to encode span context to the wire.</param>
        public DependenciesCollector(DependenciesCollectorOptions options, ITracer tracer, ISampler sampler, IPropagationComponent propagationComponent)
        {
            this.diagnosticSourceSubscriber = new DiagnosticSourceSubscriber(
                new Dictionary<string, Func<ITracer, ISampler, ListenerHandler>>()
                { { "HttpHandlerDiagnosticListener", (t, s) => new HttpHandlerDiagnosticListener(t, s, propagationComponent) } },
                tracer,
                sampler);
            this.diagnosticSourceSubscriber.Subscribe();
        }

        public void Dispose()
        {
            this.diagnosticSourceSubscriber.Dispose();
        }
    }
}
