using OpenCensus.Tags.Propagation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenCensus.Tags.Test
{
    public class TagsTest
    {
        public TagsTest()
        {
            Tags.Initialize(true);
        }

        [Fact]
        public void GetTagger()
        {
            Assert.Equal(typeof(Tagger), Tags.Tagger.GetType());
        }

        [Fact]
        public void GetTagContextSerializer()
        {
            Assert.Equal(typeof(TagPropagationComponent), Tags.TagPropagationComponent.GetType());
        }
    }
}
