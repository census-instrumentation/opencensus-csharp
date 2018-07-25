using System;

namespace Steeltoe.Management.Census.Trace
{
    public interface IAttributeValue
    {
         T Match<T>(
             Func<string, T> stringFunction,
             Func<bool, T> booleanFunction,
             Func<long, T> longFunction,
             Func<object, T> defaultFunction);
    }

    public interface IAttributeValue<out T> : IAttributeValue
    {
        T Value { get; }
        TM Apply<TM>(Func<T, TM> function);
    }
}
