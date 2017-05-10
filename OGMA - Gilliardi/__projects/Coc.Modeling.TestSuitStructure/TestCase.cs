using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coc.Modeling.TestSuitStructure
{
    public class TestCase
    {
        #region Constructor
        public TestCase()
        {
            this.requests = new List<Request>();
            this.Transactions = new List<Transaction>();
        }
        #endregion

        private float probability;
        /// <summary>
        /// Test Case Propability.
        /// </summary>
        public float Probability
        {
            get { return probability; }
            set { probability = value; }
        }

        private string name;
        /// <summary>
        /// Test case name;
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private List<Request> requests;
        /// <summary>
        /// Request list.
        /// </summary>
        public List<Request> Requests
        {
            get { return requests; }
            set { requests = value; }
        }

        private List<Transaction> transactions;
        /// <summary>
        /// Transactions.
        /// </summary>
        public List<Transaction> Transactions
        {
            get { return transactions; }
            set { transactions = value; }
        }

        private string parameterFile;

        public string ParameterFile
        {
            get { return parameterFile; }
            set { parameterFile = value; }
        }

        public override string ToString()
        {
            return this.Name + (this.Requests.Count > 0 ? " <Requests:" + Requests.Count + ">" : "") + (this.Transactions.Count > 0 ? " <Transactions:" + Transactions.Count + ">" : "");
            //return this.Name;
        }
    }
}
