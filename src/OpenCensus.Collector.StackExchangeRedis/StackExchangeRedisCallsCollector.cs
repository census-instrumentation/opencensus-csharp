﻿// <copyright file="StackExchangeRedisCallsCollector.cs" company="OpenCensus Authors">
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

namespace OpenCensus.Collector.StackExchangeRedis
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using OpenCensus.Collector.StackExchangeRedis.Implementation;
    using OpenCensus.Trace;
    using OpenCensus.Trace.Export;
    using OpenCensus.Trace.Propagation;
    using StackExchange.Redis.Profiling;

    /// <summary>
    /// Redis calls collector.
    /// </summary>
    public class StackExchangeRedisCallsCollector : IDisposable
    {
        private readonly ITracer tracer;
        private readonly IHandler handler;
        private readonly ISampler sampler;

        private readonly CancellationTokenSource cancellationTokenSource;
        private readonly CancellationToken cancellationToken;

        private readonly ProfilingSession defaultSession = new ProfilingSession();
        private readonly ConcurrentDictionary<ISpan, ProfilingSession> cache = new ConcurrentDictionary<ISpan, ProfilingSession>();

        /// <summary>
        /// Initializes a new instance of the <see cref="StackExchangeRedisCallsCollector"/> class.
        /// </summary>
        /// <param name="options">Configuration options for dependencies collector.</param>
        /// <param name="tracer">Tracer to record traced with.</param>
        /// <param name="sampler">Sampler to use to sample dependnecy calls.</param>
        /// <param name="handler">TEMPORARY: handler to send data to.</param>
        public StackExchangeRedisCallsCollector(StackExchangeRedisCallsCollectorOptions options, ITracer tracer, ISampler sampler, IHandler handler)
        {
            this.tracer = tracer;
            this.handler = handler;
            this.sampler = sampler;

            this.cancellationTokenSource = new CancellationTokenSource();
            this.cancellationToken = this.cancellationTokenSource.Token;
            Task.Factory.StartNew(this.DumpEntries, TaskCreationOptions.LongRunning, this.cancellationToken);
        }

        /// <summary>
        /// Returns session for the Redis calls recording.
        /// </summary>
        /// <returns>Session associated with the current span context to record Redis calls.</returns>
        public Func<ProfilingSession> GetProfilerSessionsFactory()
        {
            return () =>
            {
                var span = this.tracer.CurrentSpan;

                // when there are no spans in current context - BlankSpan will be returned
                // BlankSpan has invalid context. It's OK to use a single profiler session
                // for all invalid context's spans.
                //
                // TODO: It will be great to allow to check sampling here, but it is impossible
                // to start a new trace id here - no way to pass it to the resulting Span.
                if (span == null || !span.Context.IsValid)
                {
                    return this.defaultSession;
                }

                // TODO: check sampling of a current span
                var session = this.cache.GetOrAdd(span, (s) => new ProfilingSession(s));
                return session;
            };
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.cancellationTokenSource.Cancel();
            this.cancellationTokenSource.Dispose();
        }

        private void DumpEntries(object state)
        {
            while (!this.cancellationToken.IsCancellationRequested)
            {
                var spans = new List<ISpanData>();

                RedisProfilerEntryToSpanConverter.DrainSession(null, this.defaultSession, this.sampler, spans);

                foreach (var entry in this.cache)
                {
                    var span = entry.Key;
                    if (span.HasEnded)
                    {
                        this.cache.TryRemove(span, out var session);
                        RedisProfilerEntryToSpanConverter.DrainSession(span, session, this.sampler, spans);
                    }
                    else
                    {
                        this.cache.TryGetValue(span, out var session);
                        RedisProfilerEntryToSpanConverter.DrainSession(span, session, this.sampler, spans);
                    }
                }

                this.handler.Export(spans);

                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
        }
    }
}
