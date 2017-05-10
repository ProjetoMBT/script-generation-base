using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Coc.Testing.Performance
{
    [Serializable]
    internal class Metric {

        public enum OperationalSystem
        {
            Windows,
            Linux,
            Unix,
            MacOS
        }

        private OperationalSystem os;
        public OperationalSystem OS
        {
            get { return os; }
            set { os = value; }
        }

        private List<Counter> counters;
        [XmlElement("Counters")]
        public List<Counter> Counters {
            get { return counters; }
            set { counters = value; }
        }

        private string name;
        public string Name {
            get { return name; }
            set { name = value; }
        }
    }
}
