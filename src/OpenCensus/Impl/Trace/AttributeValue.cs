// <copyright file="AttributeValue.cs" company="OpenCensus Authors">
// Copyright 2018, OpenCensus Authors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of theLicense at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

namespace OpenCensus.Trace
{
    using System;

    public abstract class AttributeValue : IAttributeValue
    {
        internal AttributeValue()
        {
        }

        public static IAttributeValue<string> StringAttributeValue(string stringValue)
        {
            if (stringValue == null)
            {
                throw new ArgumentNullException(nameof(stringValue));
            }

            return new AttributeValue<string>(stringValue);
        }

        public static IAttributeValue<long> LongAttributeValue(long longValue)
        {
            return new AttributeValue<long>(longValue);
        }

        public static IAttributeValue<bool> BooleanAttributeValue(bool booleanValue)
        {
            return new AttributeValue<bool>(booleanValue);
        }

        public abstract T Match<T>(
            Func<string, T> stringFunction,
            Func<bool, T> booleanFunction,
            Func<long, T> longFunction,
            Func<object, T> defaultFunction);
    }

    public sealed class AttributeValue<T> : AttributeValue, IAttributeValue<T>
    {
        public static IAttributeValue<string> Create(string stringValue)
        {
            if (stringValue == null)
            {
                throw new ArgumentNullException(nameof(stringValue));
            }

            return new AttributeValue<string>(stringValue);
        }

        public static IAttributeValue<long> Create(long longValue)
        {
            return new AttributeValue<long>(longValue);
        }

        public static IAttributeValue<bool> Create(bool booleanValue)
        {
            return new AttributeValue<bool>(booleanValue);
        }

        public T Value { get; }

        internal AttributeValue(T value)
        {
            this.Value = value;
        }

        public M Apply<M>(Func<T, M> function)
        {
            return function(this.Value);
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }

            if (obj is AttributeValue<T> attribute)
            {
                return attribute.Value.Equals(this.Value);
            }

            return false;
        }

        public override int GetHashCode()
        {
            int h = 1;
            h *= 1000003;
            h ^= this.Value.GetHashCode();
            return h;
        }

        public override string ToString()
        {
            return "AttributeValue{"
                + "Value=" + this.Value.ToString()
                + "}";
        }

        public override M Match<M>(
            Func<string, M> stringFunction,
            Func<bool, M> booleanFunction,
            Func<long, M> longFunction,
            Func<object, M> defaultFunction)
        {
            if (typeof(T) == typeof(string))
            {
                string value = this.Value as string;
                return stringFunction(value);
            }

            if (typeof(T) == typeof(long))
            {
                long val = (long)(object)this.Value;
                return longFunction(val);
            }

            if (typeof(T) == typeof(bool))
            {
                bool val = (bool)(object)this.Value;
                return booleanFunction(val);
            }

            return defaultFunction(this.Value);
        }
    }
}
