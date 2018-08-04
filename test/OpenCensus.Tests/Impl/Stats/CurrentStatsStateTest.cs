// <copyright file="CurrentStatsStateTest.cs" company="OpenCensus Authors">
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

namespace OpenCensus.Stats.Test
{
    using System;
    using Xunit;

    public class CurrentStatsStateTest
    {
        [Fact]
        public void DefaultState()
        {
            Assert.Equal(StatsCollectionState.ENABLED, new CurrentStatsState().Value);
        }

        [Fact]
        public void SetState()
        {
            CurrentStatsState state = new CurrentStatsState();
            Assert.True(state.Set(StatsCollectionState.DISABLED));
            Assert.Equal(StatsCollectionState.DISABLED, state.Internal);
            Assert.True(state.Set(StatsCollectionState.ENABLED));
            Assert.Equal(StatsCollectionState.ENABLED, state.Internal);
            Assert.False(state.Set(StatsCollectionState.ENABLED));
        }



        [Fact]
        public void PreventSettingStateAfterReadingState()
        {
            CurrentStatsState state = new CurrentStatsState();
            var st = state.Value;
            Assert.Throws<ArgumentException>(() => state.Set(StatsCollectionState.DISABLED));
        }
    }
}
