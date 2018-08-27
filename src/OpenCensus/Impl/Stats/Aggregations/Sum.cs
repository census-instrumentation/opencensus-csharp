﻿// <copyright file="Sum.cs" company="OpenCensus Authors">
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
    using System;

    public sealed class Sum : Aggregation, ISum
    {
        internal Sum()
        {
        }

        private static readonly Sum Instance = new Sum();

        public static ISum Create()
        {
            return Instance;
        }

        public override M Match<M>(Func<ISum, M> p0, Func<ICount, M> p1, Func<IMean, M> p2, Func<IDistribution, M> p3, Func<ILastValue, M> p4, Func<IAggregation, M> p5)
        {
            return p0.Invoke(this);
        }

        public override string ToString()
        {
            return "Sum{"
                + "}";
        }

        public override bool Equals(object o)
        {
            if (o == this)
            {
                return true;
            }

            if (o is Sum)
            {
                return true;
            }

            return false;
        }

        public override int GetHashCode()
        {
            int h = 1;
            return h;
        }
    }
}
