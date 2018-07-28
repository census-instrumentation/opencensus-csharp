using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenCensus.Tags.Test
{
    public class TagsComponentBaseTest
    {
        private readonly TagsComponent tagsComponent = new TagsComponent();

        [Fact]
        public void DefaultState()
        {
            Assert.Equal(TaggingState.ENABLED, tagsComponent.State);
        }
    }
}
