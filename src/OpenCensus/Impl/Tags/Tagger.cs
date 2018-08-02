// <copyright file="Tagger.cs" company="OpenCensus Authors">
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
    using OpenCensus.Common;
    using OpenCensus.Internal;

    public sealed class Tagger : TaggerBase
    {
        private readonly CurrentTaggingState state;

        internal Tagger(CurrentTaggingState state)
        {
            this.state = state;
        }

        public override ITagContext Empty
        {
            get { return TagContext.EMPTY; }
        }

        public override ITagContext CurrentTagContext
        {
            get
            {
                return state.Internal == TaggingState.DISABLED
                    ? TagContext.EMPTY
                    : ToTagContext(CurrentTagContextUtils.CurrentTagContext);
            }
        }

        public override ITagContextBuilder EmptyBuilder
        {
            get
            {
                return state.Internal == TaggingState.DISABLED
                    ? NoopTagContextBuilder.INSTANCE
                    : new TagContextBuilder();
            }
        }

        public override ITagContextBuilder CurrentBuilder
        {
            get
            {
                return state.Internal == TaggingState.DISABLED
                    ? NoopTagContextBuilder.INSTANCE
                    : ToBuilder(CurrentTagContextUtils.CurrentTagContext);
            }
        }

        public override ITagContextBuilder ToBuilder(ITagContext tags)
        {
            return state.Internal == TaggingState.DISABLED
                ? NoopTagContextBuilder.INSTANCE
                : ToTagContextBuilder(tags);
        }

        public override IScope WithTagContext(ITagContext tags)
        {
            return state.Internal == TaggingState.DISABLED
                ? NoopScope.INSTANCE
                : CurrentTagContextUtils.WithTagContext(ToTagContext(tags));
        }

        private static ITagContext ToTagContext(ITagContext tags)
        {
            if (tags is TagContext)
            {
                return tags;
            }
else
            {
                TagContextBuilder builder = new TagContextBuilder();
                foreach (var tag in tags)
                {
                    if (tag != null)
                    {
                        builder.Put(tag.Key, tag.Value);
                    }
                }

                return builder.Build();
            }
        }

        private static ITagContextBuilder ToTagContextBuilder(ITagContext tags)
        {
            // Copy the tags more efficiently in the expected case, when the TagContext is a TagContextImpl.
            if (tags is TagContext)
            {
                return new TagContextBuilder(((TagContext)tags).Tags);
            }
else
            {
                TagContextBuilder builder = new TagContextBuilder();
                foreach (var tag in tags)
                {
                    if (tag != null)
                    {
                        builder.Put(tag.Key, tag.Value);
                    }
                }

                return builder;
            }
        }
    }
}
