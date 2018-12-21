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

                SpanKind resultKind = span.Kind;

                if (span.Attributes.AttributeMap.ContainsKey("span.kind"))
                {
                    var kind = span.Attributes.AttributeMap["span.kind"].Match((s) => s, null, null, null, null);
                    if (kind == "server")
                    {
                        resultKind = SpanKind.Server;
                        result = new RequestTelemetry();
                    }
                    else
                    {
                        resultKind = SpanKind.Client;
                        result = new DependencyTelemetry();
                    }
                }
                else if (span.Kind == SpanKind.Server)
                {
                    result = new RequestTelemetry();
                }
                else
                {
                    result = new DependencyTelemetry();
                }

                result.Timestamp = DateTimeOffset.FromUnixTimeSeconds(span.StartTimestamp.Seconds);

                // 1 tick is 100 ns
                result.Timestamp = result.Timestamp.Add(TimeSpan.FromTicks(span.StartTimestamp.Nanos / 100));
                result.Name = span.Name;
                result.Context.Operation.Id = span.Context.TraceId.ToLowerBase16();

                if (span.ParentSpanId != null && span.ParentSpanId.IsValid)
                {
                    result.Context.Operation.ParentId =
                        string.Concat("|", span.Context.TraceId.ToLowerBase16(), ".", span.ParentSpanId.ToLowerBase16(), ".");
                }

                // TODO: I don't understant why this concatanation is required
                result.Id = string.Concat("|", span.Context.TraceId.ToLowerBase16(), ".", span.Context.SpanId.ToLowerBase16(), ".");

                foreach (var ts in span.Context.Tracestate.Entries)
                {
                    result.Properties[ts.Key] = ts.Value;
                }

                var duration = span.EndTimestamp.SubtractTimestamp(span.StartTimestamp);
                result.Duration = TimeSpan.FromTicks((duration.Seconds * TimeSpan.TicksPerSecond) + (duration.Nanos / 100));
                if (span.Status != null)
                {
                    result.Success = span.Status.IsOk;
                    if (resultKind == SpanKind.Server)
                    {
                        ((RequestTelemetry)result).ResponseCode = span.Status.Description ?? span.Status.CanonicalCode.ToString();
                    }
                    else
                    {
                        ((DependencyTelemetry)result).ResultCode = span.Status.Description ?? span.Status.CanonicalCode.ToString();
                    }

                    if (!string.IsNullOrEmpty(span.Status.Description))
                    {
                        result.Properties["statusDescription"] = span.Status.Description;
                    }
                }

                foreach (var attr in span.Attributes.AttributeMap)
                {
                    var value = attr.Value.Match<string>(
                        (s) => { return s; },
                        (b) => { return b.ToString(); },
                        (l) => { return l.ToString(); },
                        (d) => { return d.ToString(); },
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
    }
}