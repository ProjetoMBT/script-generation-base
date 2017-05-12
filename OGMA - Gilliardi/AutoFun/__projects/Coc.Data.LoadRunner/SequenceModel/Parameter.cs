using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coc.Data.LoadRunner.SequenceModel
{
    /// <summary>
    /// Class that represents a generic Parameter
    /// </summary>
    public class Parameter
    {
        public Parameter()
        {
        
        }

        public Parameter(string name, string paramValue)
        {
            this.name = name;
            this.paramValue = paramValue;
        }

        private string name;
        /// <summary>
        /// Name.
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string paramValue;
        /// <summary>
        /// Value.
        /// </summary>
        public string Value
        {
            get { return paramValue; }
            set { paramValue = value; }
        }
    }
}
