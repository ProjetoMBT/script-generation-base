using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Coc.Testing.Performance
{
    [Serializable]
    public class Subtransaction
    {
        private string name;
        [XmlAttribute("Name")]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private Request begin;
        [XmlElement("Begin")]
        public Request Begin
        {
            get { return begin; }
            set { begin = value; }
        }

        private Request end;
        [XmlElement("End")]
        public Request End
        {
            get { return end; }
            set { end = value; }
        }
    }
}
