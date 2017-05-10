using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace FunctionalTool.Modeling.Uml {
    public class UmlActionState :UmlComponent {

        public String outgoing { get; set; }
        public String incoming { get; set; }
        public String stateInicio { get; set; }
        public String stateFinal { get; set; }
        public Dictionary<String, UmlActionStateDiagram> dicMyLinkedDiagrams;
        public Dictionary<String, String> dicJudeHyperLink { get; set; }

        public UmlActionState() {
            dicMyLinkedDiagrams = new Dictionary<string, UmlActionStateDiagram>();
            dicJudeHyperLink = new Dictionary<string, string>();
        }


        public static void ParseActivity(XmlNode node, String id, UmlActionStateDiagram activityDiagram)
        {
            foreach (XmlNode ActionStateNode in node.SelectNodes("//UML:ActivityGraph[@xmi.id='" + id + "']//UML:ActionState", ns))
            {
                UmlActionState activity = new UmlActionState();
                activity.Id = ActionStateNode.Attributes["xmi.id"].Value;
                activity.Name = ActionStateNode.Attributes["name"].Value;
                activityDiagram.dicAtivities.Add(activity.Id, activity);

                UmlTag.ParserTag(ActionStateNode, ns, activity);
                
                foreach (XmlNode NodeStateVertex in ActionStateNode.SelectNodes("//UML:ActionState[@xmi.id='" + activity.Id + "']//UML:StateVertex.outgoing//UML:Transition", ns))
                {
                    activity.outgoing = NodeStateVertex.Attributes["xmi.idref"].Value;
                }
                foreach (XmlNode NodeStateVertex in ActionStateNode.SelectNodes("//UML:ActionState[@xmi.id='" + activity.Id + "']//UML:StateVertex.incoming//UML:Transition", ns))
                {
                    activity.incoming = NodeStateVertex.Attributes["xmi.idref"].Value;
                }
            }
        }
    }
}
