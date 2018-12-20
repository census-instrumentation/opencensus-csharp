// <copyright file="Arguments.cs" company="OpenCensus Authors">
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

namespace OpenCensus.Utils
{
    using System;

    /// <summary>
    /// Method for arguments validation.
    /// </summary>
    public class Arguments
    {
        /// <summary>
        /// Validates a condition.
        /// </summary>
        /// <param name="condition">Boolean condition</param>
        /// <param name="errorMessage">Error that is thrown in case of condition being false</param>
        /// <returns>Whether condition is true</returns>
        public static bool Check(bool condition, string errorMessage)
        {
            if (!condition)
            {
                throw new ArgumentException(errorMessage);
            }

            return condition;
        }
    }
}
