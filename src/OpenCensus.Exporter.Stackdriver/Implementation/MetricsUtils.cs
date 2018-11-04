// <copyright file="ApplicationInsightsExporter.cs" company="OpenCensus Authors">
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

    /// <summary>
    /// Utility methods for metrics
    /// </summary>
    public static class MetricsUtils
    {
        public static string GetProjectId()
        {
            var instance = Google.Api.Gax.Platform.Instance();
            var projectId = instance?.ProjectId;

            return projectId;
        }

        /// <summary>
        /// Determining the resource to which the metrics belong
        /// </summary>
        /// <returns>Stackdriver Monitored Resource</returns>
        public static MonitoredResource GetDefaultResource(string projectId)
        {
            var builder = new MonitoredResource();
            builder.Type = Constants.GLOBAL;
            builder.Labels.Add(Constants.PROJECT_ID_LABEL_KEY, projectId);

            // TODO - zeltser - setting monitored resource labels for detected resource
            // along with all the other metadata

            return builder;
        }

        public static string GetLabelKey(string label)
        {
            return label.Replace('/', '_');
        }

        public static string GenerateTypeName(string viewName, string domain)
        {
            return domain + viewName;
        }

        public static string GetDisplayName(string viewName, string displayNamePrefix)
        {
            return displayNamePrefix + viewName;
        }
    }
}
