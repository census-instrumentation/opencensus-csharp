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

namespace OpenCensus.Exporter.Ocagent.Implementation
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Google.Protobuf.WellKnownTypes;

    using Grpc.Core;
    using Grpc.Core.Utils;

    using Opencensus.Proto.Agent.Common.V1;
    using Opencensus.Proto.Agent.Trace.V1;
    using OpenCensus.Trace.Export;

    internal class TraceExporterHandler : IHandler, IDisposable
    {
        private readonly CancellationTokenSource cts;
        private readonly Channel channel;
        private readonly Opencensus.Proto.Agent.Trace.V1.TraceService.TraceServiceClient traceClient;
        private readonly ConcurrentQueue<ISpanData> spans = new ConcurrentQueue<ISpanData>();
        private readonly Task runTask;
        private readonly Node node;

        public TraceExporterHandler(string endpoint, string hostname, ChannelCredentials credentials)
        {
            this.cts = new CancellationTokenSource();
            this.channel = new Channel(endpoint, credentials);
            this.traceClient = new TraceService.TraceServiceClient(this.channel);

            this.runTask = this.RunAsync(this.cts.Token);
            this.node = new Node
            {
                Identifier = new ProcessIdentifier
                {
                    HostName = hostname,
                    Pid = (uint)Process.GetCurrentProcess().Id,
                    StartTimestamp = Timestamp.FromDateTime(DateTime.UtcNow),
                },
                LibraryInfo = new LibraryInfo
                {
                    Language = LibraryInfo.Types.Language.CSharp,
                    CoreLibraryVersion = GetAssemblyVersion(typeof(ISpanData).Assembly),
                    ExporterVersion = GetAssemblyVersion(typeof(OcagentExporter).Assembly),
                },
                ServiceInfo = new ServiceInfo
                {
                    Name = hostname,
                },
            };
        }

        public void Export(IList<ISpanData> spanDataList)
        {
            foreach (var spanData in spanDataList)
            {
                this.spans.Enqueue(spanData);
            }
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            bool firstSpan = true;
            while (!cancellationToken.IsCancellationRequested)
            {
                // TODO Config
                // TODO handle connection errors
                // TODO backpressure on the queue

                // Spans
                if (this.spans.TryDequeue(out var spanData))
                {
                    var spanExport = new ExportTraceServiceRequest();

                    if (firstSpan)
                    {
                        spanExport.Node = this.node;
                    }

                    spanExport.Spans.Add(spanData.ToProtoSpan());
                    var reply = this.traceClient.Export();
                    await reply.RequestStream.WriteAllAsync(new[] { spanExport }).ConfigureAwait(false);
                    firstSpan = false;
                }
                else
                {
                    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken).ConfigureAwait(false);
                }
            }
        }

        public void Dispose()
        {
            this.cts.Cancel(false);
            this.runTask.Wait();
            this.cts.Dispose();
        }

        private static string GetAssemblyVersion(Assembly assembly)
        {
            AssemblyFileVersionAttribute fileVersionAttr = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
            return fileVersionAttr?.Version ?? "0.0.0";
        }
    }
}
