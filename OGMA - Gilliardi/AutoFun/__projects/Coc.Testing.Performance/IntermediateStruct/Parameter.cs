using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;


namespace Coc.Testing.Performance.IntermediateStruct
{
    /// <summary>
    /// Models a parameter of a certain entity, which will generally be sent along with the request to the SUT.
    /// </summary>
    [Serializable]
    public class Parameter
    {

        public Parameter()
            :this("","")
        {
        }

        public Parameter(string name, string paramValue)
        {
            this.name = name;
            this.paramValue = paramValue;
        }

        private string name;
        /// <summary>
        /// Name.
        /// </summary>
        [XmlAttribute("Name")]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string paramValue;
        /// <summary>
        /// Value.
        /// </summary>
        [XmlAttribute("Value")]
        public string Value
        {
            get { return paramValue; }
            set { paramValue = value; }
        }
    }
}
