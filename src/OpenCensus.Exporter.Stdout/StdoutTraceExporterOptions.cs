// <copyright file="StdoutTraceExporterOptions.cs" company="OpenCensus Authors">
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

namespace OpenCensus.Exporter.Stdout
{
    using System;

    /// <summary>
    /// Specifies Lineending types.
    /// </summary>
    public enum LineEndings
    {
        /// <summary>
        /// CRLF : Carriage return - line feed - Windows
        /// </summary>
        CRLF,

        /// <summary>
        /// CR : Carriage return only
        /// </summary>
        CR,

        /// <summary>
        /// LF : Line feed only
        /// </summary>
        LF,
    }

    /// <summary>
    /// Stdout trace exporter options.
    /// </summary>
    public sealed class StdoutTraceExporterOptions
    {
        /// <summary>
        /// Gets or sets the line ending types, defaulting to Windows style.
        /// </summary>
        public LineEndings LineTermination { get; set; } = LineEndings.CRLF;
    }
}
