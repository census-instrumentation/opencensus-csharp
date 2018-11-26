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
    using System.Collections.Generic;

    /// <summary>
    /// Utility methods for working with Google Cloud Resources
    /// </summary>
    public static class GoogleCloudResourceUtils
    {
        private static Dictionary<string, string> gcpResourceLabelMappings = new Dictionary<string, string>()
        {
            { "project_id", Constants.PROJECT_ID_LABEL_KEY },
            { "instance_id", Constants.GCP_GCE_INSTANCE },
            { "zone", null }
        };

        /// <summary>
        /// Detects Google Cloud ProjectId based on the environment on which the code runs.
        /// Supports GCE/GKE/GAE
        /// In case the code runs in a different environment,
        /// the method returns null
        /// </summary>
        /// <returns>Google Cloud Project ID</returns>
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
            var resource = new MonitoredResource();
            resource.Type = Constants.GLOBAL;
            resource.Labels.Add(Constants.PROJECT_ID_LABEL_KEY, projectId);

            // TODO - zeltser - setting monitored resource labels for detected resource
            // along with all the other metadata

            return resource;
        }

        /*
        public static Resource detectResource()
        {
            List<Resource> resourceList = new ArrayList<Resource>();
            resourceList.add(Resource.createFromEnvironmentVariables());

            if (System.getenv("KUBERNETES_SERVICE_HOST") != null)
            {
                resourceList.add(GcpGkeContainerMonitoredResource.createResource());
            }
            else if (GcpMetadataConfig.getInstanceId() != null)
            {
                resourceList.add(GcpGceInstanceMonitoredResource.createResource());
            }

            if (AwsIdentityDocUtils.isRunningOnAwsEc2())
            {
                resourceList.add(AwsEc2InstanceMonitoredResource.createResource());
            }
            return Resource.mergeResources(resourceList);
        }*/

    }
}
