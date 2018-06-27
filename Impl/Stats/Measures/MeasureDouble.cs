﻿using Steeltoe.Management.Census.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Steeltoe.Management.Census.Stats.Measures
{
    public class MeasureDouble : Measure, IMeasureDouble
    {
        public override String Name { get; }
        public override String Description { get; }
        public override String Unit { get; }

        internal MeasureDouble(String name, String description, String unit)
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


        public static IMeasureDouble Create(String name, String description, String unit)
        {
            if (!(StringUtil.IsPrintableString(name) && name.Length <= NAME_MAX_LENGTH))
            {
                throw new ArgumentOutOfRangeException(
                    "Name should be a ASCII string with a length no greater than "
                    + NAME_MAX_LENGTH
                    + " characters.");
            }

            return new MeasureDouble(name, description, unit);
        }

        public override M Match<M>(Func<IMeasureDouble, M> p0, Func<IMeasureLong, M> p1, Func<IMeasure, M> defaultFunction)
        {
            return p0.Invoke(this);
        }

        public override String ToString()
        {
            return "MeasureDouble{"
                + "name=" + Name + ", "
                + "description=" + Description + ", "
                + "unit=" + Unit
                + "}";
        }

        public override bool Equals(Object o)
        {
            if (o == this)
            {
                return true;
            }
            if (o is MeasureDouble)
            {
                MeasureDouble that = (MeasureDouble)o;
                return (this.Name.Equals(that.Name))
                     && (this.Description.Equals(that.Description))
                     && (this.Unit.Equals(that.Unit));
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
