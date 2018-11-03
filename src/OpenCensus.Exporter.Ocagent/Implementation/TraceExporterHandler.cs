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
        private readonly Channel channel;
        private readonly Opencensus.Proto.Agent.Trace.V1.TraceService.TraceServiceClient traceClient;
        private readonly ConcurrentQueue<ISpanData> spans = new ConcurrentQueue<ISpanData>();
        private readonly Node node;

        private CancellationTokenSource cts;
        private Task runTask;

        public TraceExporterHandler(string agentEndpoint, string hostName, string serviceName, ChannelCredentials credentials)
        {
            this.channel = new Channel(agentEndpoint, credentials);
            this.traceClient = new TraceService.TraceServiceClient(this.channel);

            this.node = new Node
            {
                Identifier = new ProcessIdentifier
                {
                    HostName = hostName,
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
                    Name = serviceName,
                },
            };

            this.Start();
        }

        public void Export(IList<ISpanData> spanDataList)
        {
            if (this.cts == null || this.cts.IsCancellationRequested)
            {
                return;
            }

            foreach (var spanData in spanDataList)
            {
                this.spans.Enqueue(spanData);
            }
        }

        public void Dispose()
        {
            this.Stop().Wait();
        }

        private static string GetAssemblyVersion(Assembly assembly)
        {
            AssemblyFileVersionAttribute fileVersionAttr = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
            return fileVersionAttr?.Version ?? "0.0.0";
        }

        private void Start()
        {
            // TODO Config
            // TODO handle connection errors & retries

            this.cts = new CancellationTokenSource();
            this.runTask = this.RunAsync(this.cts.Token);
        }

        private async Task Stop()
        {
            if (this.cts != null)
            {
                this.cts.Cancel(false);

                // ignore all exceptions
                await this.runTask.ContinueWith(t => { }).ConfigureAwait(false);

                this.cts.Dispose();
                this.cts = null;
                this.runTask = null;
            }
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            try
            {
                // TODO backpressure on the queue

                while (!cancellationToken.IsCancellationRequested)
                {
                    // Spans
                    if (this.spans.TryDequeue(out var spanData))
                    {
                        var protoSpan = spanData.ToProtoSpan();
                        if (protoSpan == null)
                        {
                            continue;
                        }

                        var spanExport = new ExportTraceServiceRequest();
                        spanExport.Node = this.node;
                        spanExport.Spans.Add(protoSpan);

                        // TODO:
                        // write stream and read response stream (do not close)
                        // add node to the first request only
                        // workaround for https://github.com/Microsoft/ApplicationInsights-LocalForwarder/issues/31
                        var duplexCall = this.traceClient.Export();
                        await duplexCall.RequestStream.WriteAllAsync(new ExportTraceServiceRequest[] { spanExport }).ConfigureAwait(false);
                    }
                    else
                    {
                        await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken).ConfigureAwait(false);
                    }
                }
            }
            catch (RpcException)
            {
                // TODO: log

                throw;
            }
        }
    }
}
