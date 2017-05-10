using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Coc.Testing.Performance
{
    [Serializable]
    public class SaveParameter
    {
        private string name;
        [XmlAttribute("Name")]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }
}
