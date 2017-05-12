using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParameterizationOracle
{
    public class ScriptObject
    {
        public ScriptObject()
        {

        }

        public String Name { get; set; }

        public KeywordObject RelatedObject { get; set; }

        public ScriptProtocol Protocol { get; set; }
    }
}
