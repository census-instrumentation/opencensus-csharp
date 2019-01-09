﻿// <copyright file="AssemblyInfo.cs" company="OpenCensus Authors">
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

namespace OpenCensus.Exporter.Stackdriver.Implementation
{
    using System;
    using System.Diagnostics.Tracing;
    using System.Globalization;
    using System.Threading;

    [EventSource(Name = "OpenCensus-Exporter-StackDriver")]
    internal class ExporterStackDriverEventSource : EventSource
    {
        public static readonly ExporterStackDriverEventSource Log = new ExporterStackDriverEventSource();

        [NonEvent]
        public void UnknownProblemInWorkerThreadError(Exception ex)
        {
            if (Log.IsEnabled(EventLevel.Error, EventKeywords.All))
            {
                this.UnknownProblemInWorkerThreadError(ToInvariantString(ex));
            }
        }

        [Event(1, Message = "StackDriver exporter encountered an unknown error and will shut down. Exception: {0}", Level = EventLevel.Error)]
        public void UnknownProblemInWorkerThreadError(string ex)
        {
            this.WriteEvent(1, ex);
        }

        [NonEvent]
        public void UnknownProblemWhileCreatingStackDriverTimeSeriesError(Exception ex)
        {
            if (Log.IsEnabled(EventLevel.Error, EventKeywords.All))
            {
                this.UnknownProblemWhileCreatingStackDriverTimeSeriesError(ToInvariantString(ex));
            }
        }

        [Event(2, Message = "StackDriver exporter failed to create time series. Time series will be lost. Exception: {0}", Level = EventLevel.Error)]
        public void UnknownProblemWhileCreatingStackDriverTimeSeriesError(string ex)
        {
            this.WriteEvent(2, ex);
        }

        /// <summary>
        /// Returns a culture-independent string representation of the given <paramref name="exception"/> object,
        /// appropriate for diagnostics tracing.
        /// </summary>
        private static string ToInvariantString(Exception exception)
        {
            CultureInfo originalUICulture = Thread.CurrentThread.CurrentUICulture;

            try
            {
                Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
                return exception.ToString();
            }
            finally
            {
                Thread.CurrentThread.CurrentUICulture = originalUICulture;
            }
        }
    }
}
