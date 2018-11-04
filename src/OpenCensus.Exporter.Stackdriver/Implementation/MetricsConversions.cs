// <copyright file="ApplicationInsightsExporter.cs" company="OpenCensus Authors">
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

namespace OpenCensus.Exporter.Stackdriver.Implementation
{
    using Google.Api;
    using Google.Cloud.Monitoring.V3;
<<<<<<< HEAD
    using Google.Protobuf.WellKnownTypes;
    using OpenCensus.Common;
    using OpenCensus.Exporter.Stackdriver.Utils;
    using OpenCensus.Stats;
    using OpenCensus.Stats.Aggregations;
    using OpenCensus.Stats.Measures;
    using OpenCensus.Tags;
=======
    using OpenCensus.Exporter.Stackdriver.Utils;
    using OpenCensus.Stats;
    using OpenCensus.Stats.Aggregations;
    using OpenCensus.Tags;
    using System;
>>>>>>> - Fixing a few bugs around metric creation and labels
    using System.Collections.Generic;
    using static Google.Api.Distribution.Types;
    using static Google.Api.MetricDescriptor.Types;

    /// <summary>
    /// Conversion methods from Opencensus Stats API to Stackdriver Metrics API
    /// </summary>
    internal static class MetricsConversions
    {
        /// <summary>
        /// Converts between OpenCensus aggregation and Stackdriver metric kind
        /// </summary>
        /// <param name="aggregation">Stats Aggregation</param>
        /// <returns>Stackdriver Metric Kind</returns>
        public static MetricKind ToMetricKind(
            this IAggregation aggregation)
        {
<<<<<<< HEAD
=======
            if (aggregation is ILastValue)
            {
                return MetricKind.Gauge;
            }

>>>>>>> - Fixing a few bugs around metric creation and labels
            return aggregation.Match(
                v => MetricKind.Cumulative, // Sum
                v => MetricKind.Cumulative, // Count
                v => MetricKind.Cumulative, // Mean
                v => MetricKind.Cumulative, // Distribution
                v => MetricKind.Gauge,      // Last value
                v => MetricKind.Unspecified); // Default
        }

<<<<<<< HEAD
        /// <summary>
        /// Converts from Opencensus Measure+Aggregation to Stackdriver's ValueType
        /// </summary>
        /// <param name="measure">Opencensus Measure definition</param>
        /// <param name="aggregation">Opencensus Aggregation definition</param>
        /// <returns></returns>
        public static ValueType ToValueType(
            this IMeasure measure, IAggregation aggregation)
        {
            MetricKind metricKind = aggregation.ToMetricKind();
            if (aggregation is IDistribution && (metricKind == MetricKind.Cumulative || metricKind == MetricKind.Gauge))
                return ValueType.Distribution;

            if (measure is IMeasureDouble && (metricKind == MetricKind.Cumulative || metricKind == MetricKind.Gauge))
            {
                return ValueType.Double;
            }

            if (measure is IMeasureLong && (metricKind == MetricKind.Cumulative || metricKind == MetricKind.Gauge))
            {
                return ValueType.Int64;
            }

            // TODO - zeltser - we currently don't support money and string as Opencensus doesn't support them yet
            return ValueType.Unspecified;
=======
        public static MetricDescriptor.Types.ValueType ToValueType(
            this IAggregationData aggregationData)
        {
            return aggregationData.Match(
                v => MetricDescriptor.Types.ValueType.Double,        // Sum Double
                v => MetricDescriptor.Types.ValueType.Int64,         // Sum Long
                v => MetricDescriptor.Types.ValueType.Int64,         // Count
                v => MetricDescriptor.Types.ValueType.Unspecified,   // Mean - this measure should be deprecated
                v => MetricDescriptor.Types.ValueType.Distribution,  // Distribution
                v => MetricDescriptor.Types.ValueType.Double,        // LastValue Double
                v => MetricDescriptor.Types.ValueType.Int64,         // LastValue Long
                v => MetricDescriptor.Types.ValueType.Unspecified);  // Unrecognized
        }

        public static MetricDescriptor.Types.ValueType ToValueType(
            this IMeasure measure)
        {
            return measure.Match(
                v => MetricDescriptor.Types.ValueType.Double,        // Double
                v => MetricDescriptor.Types.ValueType.Int64,         // Long
                v => MetricDescriptor.Types.ValueType.Unspecified);  // Unrecognized
>>>>>>> - Fixing a few bugs around metric creation and labels
        }

        public static LabelDescriptor ToLabelDescriptor(this ITagKey tagKey)
        {
            var labelDescriptor = new LabelDescriptor();
<<<<<<< HEAD
            
            labelDescriptor.Key = GetStackdriverLabelKey(tagKey.Name);
=======

            // TODO - zeltser - looks like we don't support / and . in the label key. Need to confirm with Stackdriver team
            labelDescriptor.Key = MetricsUtils.GetLabelKey(tagKey.Name);
>>>>>>> - Fixing a few bugs around metric creation and labels
            labelDescriptor.Description = Constants.LABEL_DESCRIPTION;

            // TODO - zeltser - Now we only support string tags
            labelDescriptor.ValueType = LabelDescriptor.Types.ValueType.String;
            return labelDescriptor;
        }

        public static Distribution CreateDistribution(
            IDistributionData distributionData,
            IBucketBoundaries bucketBoundaries)
        {
<<<<<<< HEAD
            var bucketOptions = bucketBoundaries.ToBucketOptions();
            var distribution = new Distribution
            {
                BucketOptions = bucketOptions,
                BucketCounts = { CreateBucketCounts(distributionData.BucketCounts) },
=======
            var bucketOptions = CreateBucketOptions(bucketBoundaries);
            var distribution = new Distribution
            {
                BucketOptions = bucketOptions,
                BucketCounts = { distributionData.BucketCounts },
>>>>>>> - Fixing a few bugs around metric creation and labels
                Count = distributionData.Count,
                Mean = distributionData.Mean,
                SumOfSquaredDeviation = distributionData.SumOfSquaredDeviations,
                Range = new Range { Max = distributionData.Max, Min = distributionData.Min }
            };

            return distribution;
        }

<<<<<<< HEAD
        /// <summary>
        /// Creates Stackdriver MetricDescriptor from Opencensus View
        /// </summary>
        /// <param name="metricDescriptorTypeName">Metric Descriptor full type name</param>
        /// <param name="view">Opencensus View</param>
        /// <param name="project">Google Cloud Project Name</param>
        /// <param name="domain"></param>
        /// <param name="displayNamePrefix"></param>
        /// <returns></returns>
        public static MetricDescriptor CreateMetricDescriptor(
            string metricDescriptorTypeName,
=======
        // Construct a MetricDescriptor using a View.
        public static MetricDescriptor CreateMetricDescriptor(
>>>>>>> - Fixing a few bugs around metric creation and labels
            IView view,
            ProjectName project,
            string domain,
            string displayNamePrefix)
        {
            var metricDescriptor = new MetricDescriptor();
            string viewName = view.Name.AsString;
<<<<<<< HEAD

            metricDescriptor.Name = string.Format($"projects/{project.ProjectId}/metricDescriptors/{metricDescriptorTypeName}");
            metricDescriptor.Type = metricDescriptorTypeName;
            metricDescriptor.Description = view.Description;
            metricDescriptor.DisplayName = GetDisplayName(viewName, displayNamePrefix);
=======
            string type = MetricsUtils.GenerateTypeName(viewName, domain);

            metricDescriptor.Name = string.Format($"projects/{project.ProjectId}/metricDescriptors/{type}");
            metricDescriptor.Type = type;
            metricDescriptor.Description = view.Description;
            metricDescriptor.DisplayName = MetricsUtils.GetDisplayName(viewName, displayNamePrefix);
>>>>>>> - Fixing a few bugs around metric creation and labels

            foreach (ITagKey tagKey in view.Columns)
            {
                var labelDescriptor = tagKey.ToLabelDescriptor();
                metricDescriptor.Labels.Add(labelDescriptor);
            }
            metricDescriptor.Labels.Add(
                new LabelDescriptor
                {
                    Key = Constants.OPENCENSUS_TASK,
                    Description = Constants.OPENCENSUS_TASK_DESCRIPTION,
                    ValueType = LabelDescriptor.Types.ValueType.String,
                });

<<<<<<< HEAD
            var unit = GetUnit(view.Aggregation, view.Measure);
            metricDescriptor.Unit = unit;
            metricDescriptor.MetricKind = view.Aggregation.ToMetricKind();
            metricDescriptor.ValueType = view.Measure.ToValueType(view.Aggregation);
=======
            var unit = CreateUnit(view.Aggregation, view.Measure);
            metricDescriptor.Unit = unit;
            metricDescriptor.MetricKind = view.Aggregation.ToMetricKind();
            metricDescriptor.ValueType = view.Measure.ToValueType();
>>>>>>> - Fixing a few bugs around metric creation and labels

            return metricDescriptor;
        }

        public static TypedValue CreateTypedValue(
            IAggregation aggregation,
            IAggregationData aggregationData)
        {
            return aggregationData.Match(
                v => new TypedValue { DoubleValue = v.Sum }, // Double
                v => new TypedValue { Int64Value = v.Sum }, // Long
                v => new TypedValue { Int64Value = v.Count }, // Count
                v => new TypedValue { DoubleValue = v.Count }, // Mean
                v => new TypedValue { DistributionValue = CreateDistribution(v, ((IDistribution)aggregation).BucketBoundaries) }, //Distribution
                v => new TypedValue { DoubleValue = v.LastValue }, // LastValue Double
                v => new TypedValue { Int64Value = v.LastValue }, // LastValue Long
                v => new TypedValue { BoolValue = false }); // Default
        }

<<<<<<< HEAD
        /// <summary>
        /// Create a list of counts for Stackdriver from the list of counts in OpenCensus
        /// </summary>
        /// <param name="bucketCounts">Opencensus list of counts</param>
        /// <returns></returns>
        private static IList<long> CreateBucketCounts(IList<long> bucketCounts)
        {
            // The first bucket (underflow bucket) should always be 0 count because the Metrics first bucket
            // is [0, first_bound) but StackDriver distribution consists of an underflow bucket (number 0).
            var ret = new List<long>();
            ret.Add(0L);
            ret.AddRange(bucketCounts);
            return ret;
        }

        /// <summary>
        /// Converts <see cref="IBucketBoundaries"/> to Stackdriver's <see cref="BucketOptions"/>
        /// </summary>
        /// <param name="bucketBoundaries"></param>
        /// <returns></returns>
        private static BucketOptions ToBucketOptions(this IBucketBoundaries bucketBoundaries)
        {
            // The first bucket bound should be 0.0 because the Metrics first bucket is
            // [0, first_bound) but Stackdriver monitoring bucket bounds begin with -infinity
            // (first bucket is (-infinity, 0))
            var bucketOptions = new BucketOptions
            {
                ExplicitBuckets = new BucketOptions.Types.Explicit
                {
                    Bounds = { 0.0 }
                }
            };
            bucketOptions.ExplicitBuckets.Bounds.AddRange(bucketBoundaries.Boundaries);

            return bucketOptions;
        }

        // Create a Metric using the TagKeys and TagValues.

        /// <summary>
        /// Generate Stackdriver Metric from Opencensus View
        /// </summary>
        /// <param name="view"></param>
        /// <param name="tagValues"></param>
        /// <param name="metricDescriptor">Stackdriver Metric Descriptor</param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static Metric GetMetric(
            IView view,
            IList<ITagValue> tagValues,
            MetricDescriptor metricDescriptor,
            string domain)
        {
            var metric = new Metric();
            metric.Type = metricDescriptor.Name;
=======
        public static Point CreatePoint(
            IAggregationData aggregationData,
            IViewData windowData,
            IAggregation aggregation)
        {
            return new Point
            {
                Interval = CreateTimeInterval(windowData, aggregation),
                Value = CreateTypedValue(aggregation, aggregationData)
            };
        }

        private static BucketOptions CreateBucketOptions(IBucketBoundaries bucketBoundaries)
        {
            return new BucketOptions
            {
                ExplicitBuckets = new BucketOptions.Types.Explicit
                {
                    Bounds = { bucketBoundaries.Boundaries }
                }
            };
        }

        private static TimeInterval CreateTimeInterval(
            IViewData windowData,
            IAggregation aggregation)
        {
            return windowData.View.Measure.Match(
                v =>
                {
                    var interval = new TimeInterval { EndTime = windowData.End.ToTimestamp() };
                    if (!(windowData.View.Aggregation is ILastValue))
                    {
                        interval.StartTime = windowData.Start.ToTimestamp();
                    }
                    return interval;
                },
                v =>
                {
                    // TODO - zeltser - figure out why this is called (long variant)
                    var interval = new TimeInterval { EndTime = windowData.End.ToTimestamp() };
                    if (!(windowData.View.Aggregation is ILastValue))
                    {
                        interval.StartTime = windowData.Start.ToTimestamp();
                    }
                    return interval;
                },
                v => throw new InvalidOperationException());
        }

        // Create a Metric using the TagKeys and TagValues.
        public static Metric CreateMetric(
            IView view,
            IList<ITagValue> tagValues,
            string domain)
        {
            var metric = new Metric();
            metric.Type = MetricsUtils.GenerateTypeName(view.Name.AsString, domain);
>>>>>>> - Fixing a few bugs around metric creation and labels

            IList<ITagKey> columns = view.Columns;

            // Populate metric labels
            for (int i = 0; i < tagValues.Count; i++)
            {
                ITagKey key = columns[i];
                ITagValue value = tagValues[i];
                if (value == null)
                {
                    continue;
                }

<<<<<<< HEAD
                string labelKey = GetStackdriverLabelKey(key.Name);
=======
                string labelKey = MetricsUtils.GetLabelKey(key.Name);
>>>>>>> - Fixing a few bugs around metric creation and labels
                metric.Labels.Add(labelKey, value.AsString);
            }
            metric.Labels.Add(Constants.OPENCENSUS_TASK, Constants.OPENCENSUS_TASK_VALUE_DEFAULT);

<<<<<<< HEAD
            // TODO - zeltser - make sure all the labels from the metric descriptor were fulfilled
=======
>>>>>>> - Fixing a few bugs around metric creation and labels
            return metric;
        }

        /// <summary>
        /// Convert ViewData to a list of TimeSeries, so that ViewData can be uploaded to Stackdriver.
        /// </summary>
        /// <param name="viewData">OpenCensus View</param>
<<<<<<< HEAD
        /// <param name="metricDescriptor">Stackdriver Metric Descriptor</param>
=======
>>>>>>> - Fixing a few bugs around metric creation and labels
        /// <param name="monitoredResource">Stackdriver Resource to which the metrics belong</param>
        /// <param name="domain">The metrics domain (namespace)</param>
        /// <returns></returns>
        public static List<TimeSeries> CreateTimeSeriesList(
            IViewData viewData,
            MonitoredResource monitoredResource,
<<<<<<< HEAD
            MetricDescriptor metricDescriptor,
=======
>>>>>>> - Fixing a few bugs around metric creation and labels
            string domain)
        {
            var timeSeriesList = new List<TimeSeries>();
            if (viewData == null)
            {
                return timeSeriesList;
            }

            IView view = viewData.View;
<<<<<<< HEAD
            Timestamp startTime = viewData.Start.ToTimestamp();
=======
>>>>>>> - Fixing a few bugs around metric creation and labels

            // Each entry in AggregationMap will be converted into an independent TimeSeries object
            foreach (var entry in viewData.AggregationMap)
            {
                var timeSeries = new TimeSeries();
<<<<<<< HEAD
                IList<ITagValue> labels = entry.Key.Values;
                IAggregationData points = entry.Value;
                
                timeSeries.Resource = monitoredResource;
                timeSeries.ValueType = view.Measure.ToValueType(view.Aggregation);
                timeSeries.MetricKind = view.Aggregation.ToMetricKind();

                timeSeries.Metric = GetMetric(view, labels, metricDescriptor, domain);

                Point point = ExtractPointInInterval(viewData.Start, viewData.End, view.Aggregation, points);
                var timeSeriesPoints = new List<Point> { point };
                timeSeries.Points.AddRange(timeSeriesPoints);

=======

                timeSeries.Metric = CreateMetric(view, entry.Key.Values, domain);

                timeSeries.Resource = monitoredResource;

                //timeSeries.MetricKind = view.Aggregation.ToMetricKind();
                //timeSeries.ValueType = entry.Value.ToValueType();

                var point = CreatePoint(entry.Value, viewData, view.Aggregation);
                timeSeries.Points.Add(point);
>>>>>>> - Fixing a few bugs around metric creation and labels
                timeSeriesList.Add(timeSeries);
            }

            return timeSeriesList;
        }

<<<<<<< HEAD
        private static Point ExtractPointInInterval(
            ITimestamp startTime,
            ITimestamp endTime, 
            IAggregation aggregation, 
            IAggregationData points)
        {
            return new Point
            {
                Value = CreateTypedValue(aggregation, points),
                Interval = CreateTimeInterval(startTime, endTime)
            };
        }

        private static TimeInterval CreateTimeInterval(ITimestamp start, ITimestamp end)
        {
            return new TimeInterval { StartTime = start.ToTimestamp(), EndTime = end.ToTimestamp() };
        }

        internal static string GetUnit(IAggregation aggregation, IMeasure measure)
=======
        internal static string CreateUnit(IAggregation aggregation, IMeasure measure)
>>>>>>> - Fixing a few bugs around metric creation and labels
        {
            if (aggregation is ICount)
            {
                return "1";
            }

            return measure.Unit;
        }
<<<<<<< HEAD

        internal static string GetDisplayName(string viewName, string displayNamePrefix)
        {
            return displayNamePrefix + viewName;
        }

        /// <summary>
        /// Creates Stackdriver Label name
        /// </summary>
        /// <param name="label">Opencensus label</param>
        /// <returns>Label name that complies with Stackdriver label naming rules</returns>
        internal static string GetStackdriverLabelKey(string label)
        {
            return label.Replace('/', '_');
        }
=======
>>>>>>> - Fixing a few bugs around metric creation and labels
    }
}
