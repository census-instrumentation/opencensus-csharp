// <copyright file="NoopTags.cs" company="OpenCensus Authors">
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

namespace OpenCensus.Tags
{
    using OpenCensus.Tags.Propagation;

    internal sealed class NoopTags
    {
        internal static ITagsComponent NewNoopTagsComponent()
        {
            return new NoopTagsComponent();
        }

        internal static ITagger NoopTagger
        {
            get
            {
                return OpenCensus.Tags.NoopTagger.INSTANCE;
            }
        }

        internal static ITagContextBuilder NoopTagContextBuilder
        {
            get
            {
                return OpenCensus.Tags.NoopTagContextBuilder.INSTANCE;
            }
        }

        internal static ITagContext NoopTagContext
        {
            get
            {
                return OpenCensus.Tags.NoopTagContext.INSTANCE;
            }
        }

        internal static ITagPropagationComponent NoopTagPropagationComponent
        {
            get
            {
                return OpenCensus.Tags.NoopTagPropagationComponent.INSTANCE;
            }
        }

        internal static ITagContextBinarySerializer NoopTagContextBinarySerializer
        {
            get
            {
                return OpenCensus.Tags.NoopTagContextBinarySerializer.INSTANCE;
            }
        }
    }
}
