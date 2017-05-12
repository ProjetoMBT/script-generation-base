using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Coc.Testing.Performance
{
    [Serializable]
    public class Counter {

        private string name;
        [XmlAttribute("Name")]
        public string Name {
            get { return name; }
            set { name = value; }
        }

        private string thresholds;
        /// <summary>
        /// Thresholds. Expected SLA value.
        /// </summary>
        [XmlAttribute("Thresholds")]
        public string Thresholds {
            get { return thresholds; }
            set { thresholds = value; }
        }
    }
}
