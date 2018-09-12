﻿// <copyright file="DependenciesCollector.cs" company="OpenCensus Authors">
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

namespace OpenCensus.Collector.Dependencies
{
    using System.Collections.Generic;
    using OpenCensus.Collector.Dependencies.Implementation;
    using OpenCensus.Trace;

    /// <summary>
    /// Dependencies collector.
    /// </summary>
    public class DependenciesCollector
    {
        private readonly DiagnosticSourceSubscriber diagnosticSourceSubscriber;

        /// <summary>
        /// Dependencies collector.
        /// </summary>
        /// <param name="options">Configuration options for dependencies collector.</param>
        /// <param name="tracer">Tracer to record traced with.</param>
        /// <param name="sampler">Sampler to use to sample dependnecy calls.</param>
        public DependenciesCollector(DependenciesCollectorOptions options, ITracer tracer, ISampler sampler)
        {
            this.diagnosticSourceSubscriber = new DiagnosticSourceSubscriber(
                new HashSet<string>() { "System.Net.Http.Desktop", "HttpHandlerDiagnosticListener" }, 
                tracer, 
                sampler);
            this.diagnosticSourceSubscriber.Subscribe();
        }
    }
}
