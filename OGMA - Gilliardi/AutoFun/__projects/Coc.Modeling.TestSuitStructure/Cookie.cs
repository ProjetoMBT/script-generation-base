using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coc.Modeling.TestSuitStructure
{
    public class Cookie
    {
        #region Constructor
        public Cookie(string name)
        {
            this.name = name;
        }

        public Cookie()
        {

        }
        #endregion

        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

    }
}
