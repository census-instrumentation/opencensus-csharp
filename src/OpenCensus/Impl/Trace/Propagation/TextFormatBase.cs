namespace OpenCensus.Trace.Propagation
{
    using System.Collections.Generic;

    public abstract class TextFormatBase : ITextFormat
    {
        private static readonly NoopTextFormat NOOP_TEXT_FORMAT = new NoopTextFormat();

        internal static ITextFormat NoopTextFormat
        {
            get
            {
                return NOOP_TEXT_FORMAT;
            }
        }

        public abstract IList<string> Fields { get; }

        public abstract ISpanContext Extract<C>(C carrier, IGetter<C> getter);

        public abstract void Inject<C>(ISpanContext spanContext, C carrier, ISetter<C> setter);
    }
}
