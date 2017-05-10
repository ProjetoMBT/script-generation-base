using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParameterizationOracle
{
    public class ScriptsTable
    {
        public ScriptsTable()
        {
            Table = new List<ScriptValuePair>();
        }

        public List<ScriptValuePair> Table { get; set; }
    }
}
