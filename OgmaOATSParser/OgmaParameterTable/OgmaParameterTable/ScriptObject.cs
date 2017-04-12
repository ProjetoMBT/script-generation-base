using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OgmaParameterTable
{
    /// <summary>
    /// Represents an Object value identified within a script.
    /// </summary>
    public class ScriptObject
    {
        private String name;
        private KeywordObject relatedObject;

        public String Name
        {
            get { return name; }
            set { name = value; }
        }
        public KeywordObject RelatedObject
        {
            get { return relatedObject; }
            set { relatedObject = value; }
        }
    }
}
