using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Coc.Testing.Performance.IntermediateStruct
{
    [Serializable]
    public class Cookie
    {

        private string name;
        [XmlAttribute("Name")]
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
            : this("")
        {
        }
    }
}
