using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Coc.Testing.Performance.IntermediateStruct
{
    /// <summary>
    /// Models the entity that results from the parsing of an Action Node in an Activity Diagram.
    /// </summary>
    [Serializable]
    public class Request {

        public Request()
        {
            this.transitions = new List<Transition>();
        }

        private String name;
        [XmlAttribute("Name")]
        public String Name {
            get { return name; }
            set { name = value; }
        }

        private List<Transition> transitions;
        [XmlElement("Transitions")]
        public List<Transition> Transitions {
            get { return transitions; }
            set { transitions = value; }
        }

        private bool parallelState;

        public bool ParallelState
        {
            get { return parallelState; }
            set { parallelState = value; }
        }
        

    }
}
