using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FunctionalTool.Modeling.Uml {
    public class UmlActivity :UmlComponent {

        public String outgoing { get; set; }
        public String incoming { get; set; }
        public String stateInicio { get; set; }
        public String stateFinal { get; set; }
        public Dictionary<String, UmlActivityDiagram> listActivityDiagram;
        public Dictionary<String ,UmlTag> listTag;
        public Dictionary<String, String> dicHyperLink { get; set; }


        public UmlActivity() {

            listTag = new Dictionary<string, UmlTag>();
            listActivityDiagram = new Dictionary<string, UmlActivityDiagram>();
            dicHyperLink= new Dictionary<string,string>();
        }

        public void ActivityAssociationAtivityDiagram(Dictionary<String,UmlActivityDiagram>activityDiagram) {
            
            foreach (String item in dicHyperLink.Keys)
            {
                listActivityDiagram.Add(dicHyperLink[item], 
                    activityDiagram[UmlActivityDiagram.dicJudeHyperLinks[dicHyperLink[item]]]);
            }  
        }
    }
}
