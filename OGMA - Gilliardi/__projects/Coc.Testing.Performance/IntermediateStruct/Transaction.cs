using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Coc.Testing.Performance.IntermediateStruct
{
    [Serializable]
    public class Transaction 
    {
        private string name;
        [XmlAttribute("Name")]
        public string Name {
            get { return name; }
            set { name = value; }
        }

        private Request begin;
        [XmlElement("Begin")]
        public Request Begin {
            get { return begin; }
            set { begin = value; }
        }

        private Request end;
        [XmlElement("End")]
        public Request End {
            get { return end; }
            set { end = value; }
        }


        //na UML: igual a Lanes ou States em Lanes SEM nome
        private List<Transaction> transactions;
        public List<Transaction> Transactions
        {
            get { return transactions; }
            set { transactions = value; }
        }

        //na UML: igual a States em Lanes COM nome
        private List<Subtransaction> subtransactions;
        public List<Subtransaction> Subtransactions
        {
            get { return subtransactions; }
            set { subtransactions = value; }
        }

        private List<Request> requests;
        [XmlElement("Requests")]
        public List<Request> Requests
        {
            get { return requests; }
            set { requests = value; }
        }

    }
}