using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coc.Testing.Performance.AbstractTestCases
{
    public class Cookie
    {

         private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        

        public Cookie(string name)
        {
            this.name = name;
        }

        public Cookie()
        {
            
        }
    }
}
