using OpenCensus.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Trace.Export
{
    public interface ISpanData
    {
        ISpanContext Context { get; }

        ISpanId ParentSpanId { get; }

        bool? HasRemoteParent { get; }

        string Name { get; }

        ITimestamp StartTimestamp { get; }

        IAttributes Attributes { get; }

        ITimedEvents<IAnnotation> Annotations { get; }

        ITimedEvents<IMessageEvent> MessageEvents { get; }

        ILinks Links { get; }

        int? ChildSpanCount { get; }

        Status Status { get; }

        ITimestamp EndTimestamp { get; }
    }
}
