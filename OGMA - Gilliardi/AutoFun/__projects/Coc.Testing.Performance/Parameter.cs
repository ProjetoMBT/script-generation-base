using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;


namespace Coc.Testing.Performance
{
    /// <summary>
    /// Models a parameter of a certain entity, which will generally be sent along with the request to the SUT.
    /// </summary>
    [Serializable]
    public class Parameter {

        private string name;
        [XmlAttribute("Name")]
        public string Name {
            get { return name; }
            set { name = value; }
        }

        private string paramValue;
        [XmlAttribute("Value")]
        public string Value {
            get { return paramValue; }
            set { paramValue = value; }
        }
    }
}
