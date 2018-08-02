namespace OpenCensus.Tags.Propagation
{
    public interface ITagPropagationComponent
    {
        ITagContextBinarySerializer BinarySerializer { get; }
    }
}
