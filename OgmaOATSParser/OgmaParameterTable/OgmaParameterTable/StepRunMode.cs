using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OgmaParameterTable
{
    /// <summary>
    /// Represents the StepRunMode field of the OATS Framework.
    /// </summary>
    public class StepRunMode
        : StringValue, ICloneable
    {
        #region Constructors
        /// <summary>
        /// Default constructor for StringValue. Does not set fields.
        /// </summary>
        internal StringValue()
        {
            Initialize("");
        }
        /// <summary>
        /// Returns new instance of StringValue with the specified name.
        /// </summary>
        /// <param name="name">Name of the new StringValue instance</param>
        public StringValue(String name)
        {
            Initialize(name);
        }
        /// <summary>
        /// Returns new instance of StringValue as a close of the given parameter.
        /// </summary>
        /// <param name="key">StringValue to clone</param>
        internal StringValue(StringValue value)
        {
            Initialize(value.Name);
        }
        #endregion
    }
}
