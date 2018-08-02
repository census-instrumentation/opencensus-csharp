namespace OpenCensus.Trace.Export
{
    using OpenCensus.Common;

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
