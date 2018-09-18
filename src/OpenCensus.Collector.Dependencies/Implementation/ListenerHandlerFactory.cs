// <copyright file="ListenerHandlerFactory.cs" company="OpenCensus Authors">
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

namespace OpenCensus.Collector.Dependencies.Implementation
{
    using System.Collections.Generic;
    using OpenCensus.Trace;

    internal static class ListenerHandlerFactory
    {
        private static readonly Dictionary<string, ListenerHandler> KnownHandlers =
            new Dictionary<string, ListenerHandler>();

        public static ListenerHandler GetHandler(string name, ITracer tracer, ISampler sampler)
        {
            if (!KnownHandlers.TryGetValue(name, out var handler))
            {
                switch (name)
                {
                    case "HttpHandlerDiagnosticListener":
                        handler = new HttpHandlerDiagnosticListener(tracer, sampler);
                        break;
                    default:
                        handler = new ListenerHandler(name, tracer, sampler);
                        break;
                }

                KnownHandlers[name] = handler;
            }

            return handler;
        }
    }
}