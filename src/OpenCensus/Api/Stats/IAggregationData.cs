// <copyright file="IAggregationData.cs" company="OpenCensus Authors">
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

namespace OpenCensus.Stats
{
    using System;
    using OpenCensus.Stats.Aggregations;

    public interface IAggregationData
    {
        M Match<M>(
             Func<ISumDataDouble, M> p0,
             Func<ISumDataLong, M> p1,
             Func<ICountData, M> p2,
             Func<IMeanData, M> p3,
             Func<IDistributionData, M> p4,
             Func<ILastValueDataDouble, M> p5,
             Func<ILastValueDataLong, M> p6,
             Func<IAggregationData, M> defaultFunction);
    }
}
