// <copyright file="TraceExporterHandler.cs" company="OpenCensus Authors">
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

namespace OpenCensus.Exporter.ApplicationInsights.Implementation
{
    using System;
    using System.Collections.Generic;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.ApplicationInsights.Extensibility.Implementation;
    using OpenCensus.Trace;
    using OpenCensus.Trace.Export;

    internal class TraceExporterHandler : IHandler
    {
        private readonly TelemetryClient telemetryClient;

        public TraceExporterHandler(TelemetryConfiguration telemetryConfiguration)
        {
            this.telemetryClient = new TelemetryClient(telemetryConfiguration);
        }

        public void Export(IList<ISpanData> spanDataList)
        {
            foreach (var span in spanDataList)
            {
                OperationTelemetry result;
                var kind = span.Attributes.AttributeMap[SpanAttributeConstants.SpanKindKey];

                if (this.IsServerSpanKind(kind))
                {
                    result = new RequestTelemetry();
                }
                else
                {
                    result = new DependencyTelemetry();
                }

                result.Name = span.Name;
                result.Id = span.Context.SpanId.ToLowerBase16();
                result.Context.Operation.Id = span.Context.TraceId.ToLowerBase16();
                result.Context.Operation.ParentId = span.ParentSpanId.ToLowerBase16();
                var duration = span.StartTimestamp.SubtractTimestamp(span.EndTimestamp);
                result.Duration = TimeSpan.FromTicks((duration.Seconds * TimeSpan.TicksPerSecond) + (duration.Nanos / 100));
                result.Success = span.Status.IsOk;
                ((RequestTelemetry)result).ResponseCode = span.Status.Description ?? span.Status.CanonicalCode.ToString();

                foreach (var attr in span.Attributes.AttributeMap)
                {
                    var value = attr.Value.Match<string>(
                        (s) => { return s; },
                        (b) => { return b.ToString(); },
                        (l) => { return l.ToString(); },
                        (obj) => { return obj.ToString(); });

                    result.Properties.Add(attr.Key, value);
                }

                // TODO: deal with those:
                // span.Annotations
                // span.ChildSpanCount
                // span.Context.IsValid;
                // span.Context.TraceOptions;
                // span.HasRemoteParent
                // span.Links
                // span.MessageEvents

                this.telemetryClient.Track(result);
            }
        }

        private bool IsClientSpanKind(IAttributeValue value)
        {
            return value.Match(
                (arg) =>
                {
                    return arg.Equals(SpanAttributeConstants.ClientSpanKind);
                },
                (arg) =>
                {
                    return false;
                },
                (arg) =>
                {
                    return false;
                },
                (arg) =>
                {
                    return false;
                });
        }

        private bool IsServerSpanKind(IAttributeValue value)
        {
            return value.Match(
                (arg) =>
                {
                    return arg.Equals(SpanAttributeConstants.ServerSpanKind);
                },
                (arg) =>
                {
                    return false;
                },
                (arg) =>
                {
                    return false;
                },
                (arg) =>
                {
                    return false;
                });
        }
    }
}