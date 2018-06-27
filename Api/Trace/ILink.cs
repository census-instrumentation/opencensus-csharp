﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Steeltoe.Management.Census.Trace
{
    public interface ILink
    {
        ITraceId TraceId { get; }
        ISpanId SpanId { get; }
        LinkType Type { get; }
        IDictionary<string, IAttributeValue> Attributes { get; }
    }
}
