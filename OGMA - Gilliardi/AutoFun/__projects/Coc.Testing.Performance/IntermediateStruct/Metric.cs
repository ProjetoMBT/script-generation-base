using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Coc.Testing.Performance.IntermediateStruct
{
    /// <summary>
    /// Class that represents a generic Metric
    /// </summary>
    [Serializable]
    internal class Metric
    {

        /// <summary>
        /// Operational system enum list
        /// </summary>
        public enum OperationalSystem
        {
            Windows,
            Linux,
            Unix,
            MacOS
        }

        private OperationalSystem os;
        /// <summary>
        /// OS
        /// </summary>
        public OperationalSystem OS
        {
            get { return os; }
            set { os = value; }
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

        private string name;
        /// <summary>
        /// Metric name.
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }
}
