using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OgmaParameterTable
{
    /// <summary>
    /// Represents an Action value identified within a script.
    /// </summary>
    public class ScriptAction
        : StringValue
    {
        #region Fields and Properties
        private ScriptObject relatedObject;

        public ScriptObject RelatedObject
        {
            get { return relatedObject; }
            set { relatedObject = value; }
        }
        #endregion

        #region Constructors

        #endregion
    }
}
