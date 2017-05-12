using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coc.Testing.Performance.AbstractTestCases
{
    /// <summary>
    /// Class that represents a generic Transaction
    /// </summary>
    public class Transaction {

        private string name;
        /// <summary>
        /// Name.
        /// </summary>
        public string Name {
            get { return name; }
            set { name = value; }
        }


        private Request begin;
        /// <summary>
        /// Begin trans.
        /// </summary>
        public Request Begin {
            get { return begin; }
            set { begin = value; }
        }


        private Request end;
        /// <summary>
        /// End trans.
        /// </summary>
        public Request End {
            get { return end; }
            set { end = value; }
        }

        private List<Subtransaction> subtransactions;
        /// <summary>
        /// Subtransactions.
        /// </summary>
        public List<Subtransaction> Subtransactions
        {
            get { return subtransactions; }
            set { subtransactions = value; }
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
