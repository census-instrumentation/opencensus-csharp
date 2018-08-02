namespace OpenCensus.Stats.Measures
{
    using System;
    using OpenCensus.Utils;

    public sealed class MeasureLong : Measure, IMeasureLong
    {
        public override string Name { get; }

        public override string Description { get; }

        public override string Unit { get; }

        internal MeasureLong(string name, string description, string unit)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            this.Name = name;
            if (description == null)
            {
                throw new ArgumentNullException(nameof(description));
            }

            this.Description = description;
            if (unit == null)
            {
                throw new ArgumentNullException(nameof(unit));
            }

            this.Unit = unit;
        }

        public static IMeasureLong Create(string name, string description, string unit)
        {
            if (!(StringUtil.IsPrintableString(name) && name.Length <= NAME_MAX_LENGTH))
            {
                throw new ArgumentOutOfRangeException(
                    "Name should be a ASCII string with a length no greater than "
                    + NAME_MAX_LENGTH
                    + " characters.");
            }
    
            return new MeasureLong(name, description, unit);
        }

        public override M Match<M>(Func<IMeasureDouble, M> p0, Func<IMeasureLong, M> p1, Func<IMeasure, M> defaultFunction)
        {
            return p1.Invoke(this);
        }

        public override string ToString()
        {
            return "MeasureLong{"
                + "name=" + Name + ", "
                + "description=" + Description + ", "
                + "unit=" + Unit
                + "}";
        }

        public override bool Equals(object o)
        {
            if (o == this)
            {
                return true;
            }

            if (o is MeasureLong)
            {
                MeasureLong that = (MeasureLong)o;
                return this.Name.Equals(that.Name)
                     && this.Description.Equals(that.Description)
                     && this.Unit.Equals(that.Unit);
            }

            return false;
        }

        public override int GetHashCode()
        {
            int h = 1;
            h *= 1000003;
            h ^= this.Name.GetHashCode();
            h *= 1000003;
            h ^= this.Description.GetHashCode();
            h *= 1000003;
            h ^= this.Unit.GetHashCode();
            return h;
        }
    }
}
