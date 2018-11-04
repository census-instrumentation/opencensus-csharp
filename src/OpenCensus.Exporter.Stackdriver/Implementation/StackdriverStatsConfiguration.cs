// <copyright file="ICountData.cs" company="OpenCensus Authors">
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

namespace OpenCensus.Exporter.Stackdriver.Implementation
{
    using Google.Api;
    using System;

    public class StackdriverStatsConfiguration
    {
        private static readonly TimeSpan DEFAULT_INTERVAL = TimeSpan.FromMinutes(1);

        public TimeSpan ExportInterval { get; set; }

        public string MetricNamePrefix { get; set; }

        public string ProjectId { get; set; }

        public MonitoredResource MonitoredResource { get; set; }

        /// <summary>
        /// Default Stats Configuration for Stackdriver
        /// </summary>
        public static StackdriverStatsConfiguration Default { get; } = new StackdriverStatsConfiguration
        {
            ExportInterval = DEFAULT_INTERVAL,
            ProjectId = MetricsUtils.GetProjectId(),
            MetricNamePrefix = "",
        };
    }
}
