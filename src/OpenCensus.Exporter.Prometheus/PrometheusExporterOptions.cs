﻿// <copyright file="PrometheusExporterOptions.cs" company="OpenCensus Authors">
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

namespace OpenCensus.Exporter.Prometheus
{
    /// <summary>
    /// Options to run prometheus exporter.
    /// </summary>
    public class PrometheusExporterOptions
    {
        /// <summary>
        /// Gets or sets the port to listen to.
        /// </summary>
        public uint Port { get; set; }
    }
}