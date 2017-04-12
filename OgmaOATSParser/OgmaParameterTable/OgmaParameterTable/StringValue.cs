using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OgmaParameterTable
{
    /// <summary>
    /// Represents either value field of an Object-Action pair, as presented in the OATS Framework.
    /// </summary>
    public abstract class StringValue<T>
        : IComparable<T> where T : StringValue<T>, ICloneable
    {
        #region Fields and Properties
        private String name;

        public String Name
        {
            get { return name; }
            set { name = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes an instance of StringValue with the given parameters.
        /// </summary>
        /// <param name="name">StringValue name</param>
        protected void Initialize(String name)
        {
            Name = name;
        }
        #endregion

        #region Auxiliary Methods
        public int CompareTo(T obj)
        {
            return obj.Name.CompareTo(this.Name);
        }
        /// <summary>
        /// Compares the name of this StringValue instance to the name of the given StringValue instance.
        /// </summary>
        /// <param name="obj">StringValue instance to compare with</param>
        /// <returns>True if names are equal, False if names are different</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            T value = (T)obj;
            if (value != null)
                return value.Name.Equals(this.Name);
            else
                throw new ArgumentException("Object is not a " + typeof(T).Name);
        }
        public object Clone()
        {
            return new T(this);
        }
        #endregion

        #region Conversion Methods
        /// <summary>
        /// Conversion from String to StringValue.
        /// </summary>
        /// <param name="value">Name of the new StringValue</param>
        /// <returns>StringValue object</returns>
        public static explicit operator String(StringValue value)
        {
            return value.Name;
        }
        /// <summary>
        /// Conversion from String to StringValue.
        /// </summary>
        /// <param name="value">Name of the new StringValue</param>
        /// <returns>StringValue object</returns>
        internal static implicit operator String(StringValue value)
        {
            return value.Name;
        }
        #endregion
    }
}
