using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClaferLanguage
{
    public class ClaferClassNode
    {
        private ClaferClasses type;

        public ClaferClasses Type
        {
            get { return type; }
            set { type = value; }
        }

        private List<ClaferClassNode> derivations;

        public List<ClaferClassNode> Derivations
        {
            get { return derivations; }
            set { derivations = value; }
        }

        public ClaferClassNode(ClaferClasses type)
        {
            this.type = type;
            derivations = new List<ClaferClassNode>();
        }
    }
}
