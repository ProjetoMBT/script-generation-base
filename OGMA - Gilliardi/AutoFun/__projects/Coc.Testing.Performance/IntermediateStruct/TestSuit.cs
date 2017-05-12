using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Coc.Testing.Performance.IntermediateStruct
{
    /// <summary>
    /// Root of the Intermediate Structure for Performance Testing. The parsed UML diagram is used to construct this
    /// structure, which is the resulting artifact of the parsing stage.
    /// </summary>
    [Serializable]
    [XmlRoot()]
    public class TestSuit {

        public TestSuit()
        {
            this.Scenarios = new List<Scenario>();
        }

        private string name;
        [XmlAttribute("Name")]
        public String Name {
            get{return name;}
            set{this.name = value;}
        }

        [XmlElement("Scenarios")]
        public List<Scenario> Scenarios { get; set; }
    }
}
