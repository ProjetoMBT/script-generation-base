using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OgmaParameterTable
{
    /// <summary>
    /// Represents all the Object-Action pairs as identified within a script.
    /// </summary>
    public class ScriptValuesTable
    {
        private List<ScriptValuePair> table;

        public List<ScriptValuePair> Table
        {
            get { return table; }
            set { table = value; }
        }
    }
}
