using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OgmaParameterTable
{
    /// <summary>
    /// Represents an Object-Action pair, as identified within a script.
    /// </summary>
    public class ScriptValuePair
    {
        private ScriptObject sObject;
        private ScriptAction sAction;
        private KeywordValuePair rKeywordPair;

        public ScriptObject SObject
        {
            get { return sObject; }
            set { sObject = value; }
        }
        public ScriptAction SAction
        {
            get { return sAction; }
            set { sAction = value; }
        }
        public KeywordValuePair RKeywordPair
        {
            get { return rKeywordPair; }
            set { rKeywordPair = value; }
        }
    }
}
