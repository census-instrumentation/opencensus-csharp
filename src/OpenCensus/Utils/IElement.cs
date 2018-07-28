using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCensus.Utils
{
    public interface IElement<T> where T: IElement<T> 
    {
        T Next { get; set; }
        T Previous { get; set; }
    }
}
