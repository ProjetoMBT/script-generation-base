using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Coc.Testing.Performance.IntermediateStruct
{
    /// <summary>
    /// Class that represents a generic Host
    /// </summary>
    [Serializable]
    public class Host
    {
        /// <summary>
        /// HostType
        /// </summary>
        public enum HostType
        {
            Application,
            Database,
            LoadGenerator,
            Controller,
            ServiceBus,
            LoadBalance
        }

        private HostType type;
        /// <summary>
        /// Host type.
        /// </summary>
        [XmlElement("Type")]
        public HostType Type
        {
            get { return type; }
            set { type = value; }
        }

        private String name;
        /// <summary>
        /// Host name (or IP adress).
        /// </summary>
        [XmlAttribute("Name")]
        public String Name
        {
            get { return name; }
            set { name = value; }
        }

        private Boolean monitoring;
        /// <summary>
        /// Monitoring.
        /// </summary>
        [XmlAttribute("Monitoring")]
        public Boolean Monitoring
        {
            get { return monitoring; }
            set { monitoring = value; }
        }

        private List<Counter> counters;
        /// <summary>
        /// Counter list.
        /// </summary>
        [XmlElement("Counters")]
        public List<Counter> Counters
        {
            get { return counters; }
            set { counters = value; }
        }
    }
}