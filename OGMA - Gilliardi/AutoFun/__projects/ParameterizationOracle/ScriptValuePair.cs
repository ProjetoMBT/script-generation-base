using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParameterizationOracle
{
    public class ScriptValuePair
    {
        public ScriptValuePair()
        {

        }

        public ScriptObject SObject { get; set; }

        public ScriptAction SAction { get; set; }

        public KeywordValuePair SRelatedObject { get; set; }
    }
}
