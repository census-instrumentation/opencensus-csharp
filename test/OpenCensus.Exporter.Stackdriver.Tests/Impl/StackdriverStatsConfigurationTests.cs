// <copyright file="StackdriverStatsConfigurationTests.cs" company="OpenCensus Authors">
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

namespace OpenCensus.Exporter.Stackriver.Tests
{
    using OpenCensus.Exporter.Stackdriver.Implementation;
    using System;
    using Xunit;

    public class StackdriverStatsConfigurationTests
    {
<<<<<<< HEAD
<<<<<<< HEAD
=======
>>>>>>> Adding an option to set google cloud projectId in environment variable for testing purposes(Stackdriver exporter).
        public StackdriverStatsConfigurationTests()
        {
            // Setting this for unit testing purposes, so we don't need credentials for real Google Cloud Account
            Environment.SetEnvironmentVariable("GOOGLE_PROJECT_ID", "test", EnvironmentVariableTarget.Process);
        }

<<<<<<< HEAD
=======
>>>>>>> - Added test project for Stackdriver Exporter
=======
>>>>>>> Adding an option to set google cloud projectId in environment variable for testing purposes(Stackdriver exporter).
        [Fact]
        public void StatsConfiguration_ByDefault_MetricNamePrefixEmpty()
        {
            Assert.NotNull(StackdriverStatsConfiguration.Default);
            Assert.Equal(GoogleCloudResourceUtils.GetProjectId(), StackdriverStatsConfiguration.Default.ProjectId);
            Assert.Equal(string.Empty, StackdriverStatsConfiguration.Default.MetricNamePrefix);
        }

        [Fact]
        public void StatsConfiguration_ByDeafult_ProjectIdIsGoogleCloudProjectId()
        {
            Assert.NotNull(StackdriverStatsConfiguration.Default);
            Assert.Equal(GoogleCloudResourceUtils.GetProjectId(), StackdriverStatsConfiguration.Default.ProjectId);
        }

        [Fact]
        public void StatsConfiguration_ByDefault_ExportIntervalMinute()
        {
            Assert.Equal(TimeSpan.FromMinutes(1), StackdriverStatsConfiguration.Default.ExportInterval);
        }

        [Fact]
        public void StatsConfiguration_ByDefault_MonitoredResourceIsGlobal()
        {
            Assert.NotNull(StackdriverStatsConfiguration.Default.MonitoredResource);
<<<<<<< HEAD
<<<<<<< HEAD

            Assert.Equal(Constants.GLOBAL, StackdriverStatsConfiguration.Default.MonitoredResource.Type);

            Assert.NotNull(StackdriverStatsConfiguration.Default.MonitoredResource.Labels);

            Assert.True(StackdriverStatsConfiguration.Default.MonitoredResource.Labels.ContainsKey("project_id"));
=======
            Assert.Equal(Constants.GLOBAL, StackdriverStatsConfiguration.Default.MonitoredResource.Type);
            Assert.NotNull(StackdriverStatsConfiguration.Default.MonitoredResource.Labels);
>>>>>>> - Added test project for Stackdriver Exporter
            Assert.True(StackdriverStatsConfiguration.Default.MonitoredResource.Labels.ContainsKey(Constants.PROJECT_ID_LABEL_KEY));
=======

            Assert.Equal(Constants.GLOBAL, StackdriverStatsConfiguration.Default.MonitoredResource.Type);

            Assert.NotNull(StackdriverStatsConfiguration.Default.MonitoredResource.Labels);

            Assert.True(StackdriverStatsConfiguration.Default.MonitoredResource.Labels.ContainsKey("project_id"));
<<<<<<< HEAD
>>>>>>> Temporarily commenting out lines that read from a friend assembly as it doesn't work on a build server
=======
            Assert.True(StackdriverStatsConfiguration.Default.MonitoredResource.Labels.ContainsKey(Constants.PROJECT_ID_LABEL_KEY));
>>>>>>> Added AssemblyInfo.cs into Stackdriver Exporter, so it's visible to tests on both signed and unsigned builds
            Assert.Equal(
                StackdriverStatsConfiguration.Default.ProjectId,
                StackdriverStatsConfiguration.Default.MonitoredResource.Labels[Constants.PROJECT_ID_LABEL_KEY]);
        }
    }
}
