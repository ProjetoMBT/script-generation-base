using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Coc.Testing.Performance
{
    /// <summary>
    /// Models the entity that results from the parsing of an Use Case.
    /// </summary>
    [Serializable]
    public class TestCase {

        private float propability;
        /// <summary>
        /// Probability of occurrence of an Use Case.
        /// </summary>
        [XmlAttribute("Probability")]
        public float Propability {
            get { return propability; }
            set { propability = value; }
        }

        private string name;
        [XmlAttribute("Name")]
        public string Name {
            get { return name; }
            set { name = value; }
        }

        private List<Request> requests;
        [XmlElement("Requests")]
        public List<Request> Requests {
            get { return requests; }
            set { requests = value; }
        }

        private List<Transaction> transactions;
        [XmlElement("Transactions")]
        public List<Transaction> Transactions {
            get { return transactions; }
            set { transactions = value; }
        }
    }
}