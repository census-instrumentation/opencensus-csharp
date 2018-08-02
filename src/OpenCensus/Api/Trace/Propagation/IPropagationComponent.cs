namespace OpenCensus.Trace.Propagation
{
    public interface IPropagationComponent
    {
        IBinaryFormat BinaryFormat { get; }

        ITextFormat TextFormat { get; }
    }
}