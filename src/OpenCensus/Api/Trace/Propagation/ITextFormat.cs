namespace OpenCensus.Trace.Propagation
{
    using System.Collections.Generic;

    public interface ITextFormat
    {
        IList<string> Fields { get; }

        void Inject<C>(ISpanContext spanContext, C carrier, ISetter<C> setter);

        ISpanContext Extract<C>(C carrier, IGetter<C> getter);
    }
}
