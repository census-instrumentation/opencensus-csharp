﻿// <copyright file="IMeanData.cs" company="OpenCensus Authors">
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

namespace OpenCensus.Stats.Aggregations
{
    /// <summary>
    /// Data accumulated by mean value aggregator.
    /// </summary>
    public interface IMeanData : IAggregationData
    {
        /// <summary>
        /// Gets the mean value.
        /// </summary>
        double Mean { get; }

        /// <summary>
        /// Gets the count of samples
        /// </summary>
        long Count { get; }

        /// <summary>
        /// Gets the maximum value.
        /// </summary>
        double Max { get; }

        /// <summary>
        /// Gets the minimum value.
        /// </summary>
        double Min { get; }
    }
}
