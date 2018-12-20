// <copyright file="Resource.cs" company="OpenCensus Authors">
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

namespace OpenCensus.Resource
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Text.RegularExpressions;
    using OpenCensus.Tags;
    using OpenCensus.Utils;

    /// <summary>
    /// Represents a resource that captures identification information about the entities for which signals (stats or traces)
    /// are reported. It further provides a framework for detection of resource information from the environment and progressive
    /// population as signals propagate from the core instrumentation library to a backend's exporter.
    /// </summary>
    public abstract class Resource : IResource
    {
        /// <summary>
        /// Maximum length of the resource type name.
        /// </summary>
        internal const int MaxResourceTypeNameLength = 255;

        /// <summary>
        /// Special resource type name that is assigned if nothing else is detected.
        /// </summary>
        private const string GlobalResourceType = "Global";

        /// <summary>
        /// OpenCensus Resource Type Environment Variable Name
        /// </summary>
        private const string OC_RESOURCE_TYPE_ENV = "OC_RESOURCE_TYPE";

        /// <summary>
        /// OpenCensus Resource Labels Environment Variable Name
        /// </summary>
        private const string OC_RESOURCE_LABELS_ENV = "OC_RESOURCE_LABELS";

        /// <summary>
        /// Tag list splitter.
        /// </summary>
        private const char LABEL_LIST_SPLITTER = ',';

        /// <summary>
        /// Key-value splitter.
        /// </summary>
        private const char LABEL_KEY_VALUE_SPLITTER = '=';

        /// <summary>
        /// Error message when string contains invalid characters.
        /// </summary>
        private static readonly string ERROR_MESSAGE_INVALID_CHARS =
            " should be a ASCII string with a length greater than 0 and not exceed " + MaxResourceTypeNameLength + " characters.";

        /// <summary>
        /// Environment identification (for example, AKS/GKE/etc).
        /// </summary>
        private static readonly string EnvironmentType;

        private static readonly ITag[] EnvironmentToLabelMap;

        static Resource()
        {
            string openCensusResourceType;
            string openCensusEnvironmentTags;

            try
            {
                openCensusResourceType = Environment.GetEnvironmentVariable(OC_RESOURCE_TYPE_ENV);
            }
            catch (SecurityException)
            {
                openCensusResourceType = GlobalResourceType;

                // TODO - Log error
            }

            try
            {
                openCensusEnvironmentTags = Environment.GetEnvironmentVariable(OC_RESOURCE_LABELS_ENV);
            }
            catch (SecurityException)
            {
                openCensusEnvironmentTags = string.Empty;

                // TODO - Log error
            }

            TryParseResourceType(openCensusResourceType, out EnvironmentType);
            EnvironmentToLabelMap = ParseResourceLabels(Environment.GetEnvironmentVariable(OC_RESOURCE_LABELS_ENV));
        }

        /// <summary>
        /// Gets the identification of the resource.
        /// </summary>
        public abstract string Type { get; protected set; }

        /// <summary>
        /// Gets the map between the tag and its value.
        /// </summary>
        public abstract IList<ITag> Tags { get; }

        /// <summary>
        /// Creates a label/tag map from the OC_RESOURCE_LABELS environment variable.
        /// OC_RESOURCE_LABELS: A comma-separated list of labels describing the source in more detail,
        /// e.g. “key1=val1,key2=val2”. Domain names and paths are accepted as label keys.
        /// Values may be quoted or unquoted in general. If a value contains whitespaces, =, or " characters, it must
        /// always be quoted.
        /// </summary>
        /// <param name="rawEnvironmentTags">Environment tags as a raw, comma separated string</param>
        /// <returns>Environment Tags as a list</returns>
        private static ITag[] ParseResourceLabels(string rawEnvironmentTags)
        {
            if (rawEnvironmentTags == null)
            {
                return new ITag[0] { };
            }
            else
            {
                var labels = new List<ITag>();
                string[] rawLabels = rawEnvironmentTags.Split(LABEL_LIST_SPLITTER);

                Regex regex = new Regex("^\"|\"$", RegexOptions.Compiled);
                
                foreach (var rawLabel in rawLabels)
                {
                    string[] keyValuePair = rawLabel.Split(LABEL_KEY_VALUE_SPLITTER);
                    if (keyValuePair.Length != 2)
                    {
                        continue;
                    }

                    string key = keyValuePair[0].Trim();
                    string value = Regex.Replace(keyValuePair[1].Trim(), "^\"|\"$", string.Empty);

                    Arguments.Check(IsValidAndNotEmpty(key), "Label key" + ERROR_MESSAGE_INVALID_CHARS);
                    Arguments.Check(IsValid(value), "Label value" + ERROR_MESSAGE_INVALID_CHARS);

                    labels.Add(new Tag(new TagKey(key), new TagValue(value)));
                }

                return labels.ToArray();
            }
        }

        private static bool TryParseResourceType(string rawEnvironmentType, out string resourceType)
        {
            if (string.IsNullOrEmpty(rawEnvironmentType))
            {
                resourceType = GlobalResourceType;
                return false;
            }

            if (rawEnvironmentType.Length > MaxResourceTypeNameLength)
            {
                // TODO Log error $"Type {ERROR_MESSAGE_INVALID_CHARS}";
                resourceType = GlobalResourceType;
                return false;
            }

            resourceType = rawEnvironmentType.Trim();
            return true;
        }

        /// <summary>
        /// Checks whether given string is a valid printable ASCII string with a length not exeeding
        /// <see cref="MaxResourceTypeNameLength"/> characters.
        /// </summary>
        /// <param name="name">The string.</param>
        /// <returns>Whether given string is valid.</returns>
        private static bool IsValid(string name)
        {
            return name.Length <= MaxResourceTypeNameLength && StringUtil.IsPrintableString(name);
        }

        /// <summary>
        /// Checks whether given string is a valid printable ASCII string with a length
        /// greater than 0 and not exceeding <see cref="MaxLength"/> characters.
        /// </summary>
        /// <param name="name">The string.</param>
        /// <returns>Whether given string is valid.</returns>
        private static bool IsValidAndNotEmpty(string name)
        {
            return !string.IsNullOrEmpty(name) && IsValid(name);
        }
    }
}
