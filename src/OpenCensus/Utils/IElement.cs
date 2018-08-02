namespace OpenCensus.Utils
{
    public interface IElement<T> where T: IElement<T> 
    {
        T Next { get; set; }

        T Previous { get; set; }
    }
}
