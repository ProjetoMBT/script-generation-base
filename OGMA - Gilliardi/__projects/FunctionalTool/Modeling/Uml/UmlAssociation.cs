using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace FunctionalTool.Modeling.Uml
{
    public class UmlAssociation : UmlBase
    {
        public String Source { get; set; }
        public String Target { get; set; }
        public String  refDiagramActivity { get; set; }

    }
}
