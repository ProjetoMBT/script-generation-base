using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OgmaParameterTable
{
    /// <summary>
    /// Represents a Keyword, as presented in the OATS Framework.
    /// </summary>
    public class Keyword
        : IComparable, ICloneable
    {
        #region Fields and Properties
        private String name;
        private StepRunMode stepRunMode;

        public String Name
        {
            get { return name; }
            set { name = value; }
        }
        /// <summary>
        /// The StepRunMode field is a placeholder. It is unlikely to be necessary but is being left here for forward-compatibility.
        /// </summary>
        internal StepRunMode StepRunMode
        {
            get { return stepRunMode; }
            set { stepRunMode = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor for Keyword. Does not set fields.
        /// </summary>
        internal Keyword()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Returns new instance of Keyword with the specified name.
        /// </summary>
        /// <param name="name">Name of the new Keyword instance</param>
        public Keyword(String name)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Returns new instance of Keyword with the specified name and StepRunMode.
        /// </summary>
        /// <param name="name">Name of the new Keyword instance</param>
        /// <param name="srm">StepRunMode value of the new Keyword instance</param>
        internal Keyword(String name, StepRunMode srm)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Returns new instance of Keyword with the specified name and StepRunMode.
        /// </summary>
        /// <param name="name">Name of the new Keyword instance</param>
        /// <param name="srm">StepRunMode value of the new Keyword instance</param>
        internal Keyword(String name, String srm)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Returns new instance of Keyword as a close of the given parameter.
        /// </summary>
        /// <param name="key">Keyword to clone</param>
        internal Keyword(Keyword key)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Initializes an instance of Keyword with the given parameters.
        /// </summary>
        /// <param name="name">Keyword name</param>
        /// <param name="srm">Keyword's StepRunMode value</param>
        private void Initialize(String name, String srm)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Auxiliary Methods
        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }
        // Development note: The following method does not and should not take the StepRunMode field into consideration, regardless of its availability.
        /// <summary>
        /// Compares the name of this Keyword instance to the name of the given Keyword instance.
        /// </summary>
        /// <param name="obj">Keyword instance to compare with</param>
        /// <returns>True if names are equal, False if names are different</returns>
        public override bool Equals(object obj)
        {
            throw new NotImplementedException();
        }
        public object Clone()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Conversion Operators
        /// <summary>
        /// Conversion from Keyword to String.
        /// </summary>
        /// <param name="value">Keyword</param>
        /// <returns>Name of the given Keyword</returns>
        public static explicit operator String(Keyword value)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Conversion from String to Keyword.
        /// </summary>
        /// <param name="value">Name of the new Keyword</param>
        /// <returns>Keyword object</returns>
        public static explicit operator Keyword(String value)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Conversion from Keyword to String.
        /// </summary>
        /// <param name="value">Keyword</param>
        /// <returns>Name of the given Keyword</returns>
        internal static implicit operator String(Keyword value)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Conversion from String to Keyword.
        /// </summary>
        /// <param name="value">Name of the new Keyword</param>
        /// <returns>Keyword object</returns>
        internal static implicit operator Keyword(String value)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
