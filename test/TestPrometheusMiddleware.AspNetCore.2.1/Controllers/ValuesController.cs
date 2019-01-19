// <copyright file="ValuesController.cs" company="OpenCensus Authors">
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

namespace TestPrometheusMiddleware.AspNetCore._2._0.Controllers
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc;
    using OpenCensus.Exporter.Prometheus;
    using OpenCensus.Stats;
    using OpenCensus.Stats.Aggregations;
    using OpenCensus.Stats.Measures;
    using OpenCensus.Tags;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;


    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private static long MiB = 1 << 20;

        private static readonly IMeasureLong ValueSize = MeasureLong.Create("my.org/measure/values_size", "size of processed values", "By");

        private static readonly ITagKey FrontendKey = TagKey.Create("my.org/keys/frontend");

        private static readonly IViewName ValueSizeViewName = ViewName.Create("my.org/views/value_size");
        private static readonly IView ValueSizeView = OpenCensus.Stats.View.Create(
                   ValueSizeViewName,
                   "processed value size over time",
                   ValueSize,
                   Distribution.Create(BucketBoundaries.Create(new List<double>() { 0.0, 16.0 * MiB, 256.0 * MiB })),
                   new List<ITagKey>() { FrontendKey });

        private readonly IStatsRecorder recorder;

        private readonly ITagger tagger;

        public ValuesController(IStatsRecorder statsRecorder, ITagger tagger, IViewManager viewManager) : base()
        {
            viewManager.RegisterView(ValueSizeView);
            this.recorder = statsRecorder;
            this.tagger = tagger;
        }

        // GET api/values
        /// <summary>
        /// Returns a pseudorandom set of strings of format value{i} where i is between 1 and 100
        /// </summary>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            var retval = new List<string>();
            var tagContextBuilder = tagger.CurrentBuilder.Put(FrontendKey, TagValue.Create("mobile-ios9.3.5"));
            using (var scopedTags = tagContextBuilder.BuildScoped())
            {
                Random r = new Random();
                for (int i = 0; i < r.Next(0, 100); i++)
                {
                    retval.Add($"value{i}");
                }
                this.recorder.NewMeasureMap().Put(ValueSize, string.Join(",", retval).Length * MiB).Record();
            }
            return retval;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }
    }
}
