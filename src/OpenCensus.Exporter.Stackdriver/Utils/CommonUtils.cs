// <copyright file="ApplicationInsightsExporter.cs" company="OpenCensus Authors">
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

namespace OpenCensus.Exporter.Stackdriver.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Common Utility Methods that are not metrics/trace specific
    /// </summary>
    public static class CommonUtils
    {
        /// <summary>
        /// Divide the source list into batches of lists of given size
        /// </summary>
        /// <typeparam name="T">The type of the list</typeparam>
        /// <param name="source">The list</param>
        /// <param name="size">Size of the batch</param>
        /// <returns></returns>
        public static IEnumerable<List<T>> Partition<T>(this IList<T> source, Int32 size)
        {
            for (int i = 0; i < Math.Ceiling(source.Count / (double)size); i++)
            {
                yield return new List<T>(source.Skip(size * i).Take(size));
            }
        }
    }
}
