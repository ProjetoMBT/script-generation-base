using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace FunctionalTool.Modeling.Uml
{
    public class UmlTransition : UmlAssociation
    {

        public List<UmlTag> listExpectedResults;

        public UmlTransition()
        {
            this.listExpectedResults = new List<UmlTag>();
        }

        public static Dictionary<String, UmlTransition> ParserTransition(XmlNode node,String id)
        {
            Dictionary<String, UmlTransition> dicTransition=new Dictionary<string,UmlTransition>();

                foreach (XmlNode NodeTransition in node.SelectNodes("//UML:ActivityGraph[@xmi.id='" + id + "']/*/UML:Transition", ns))
                {
                    if (NodeTransition.Attributes["name"] != null)
                    {
                        UmlTransition t = new UmlTransition();
                        t.Name = NodeTransition.Attributes["name"].Value;
                        t.Id = NodeTransition.Attributes["xmi.id"].Value;

                        UmlTag.ParserTag(NodeTransition, ns, t);

                        foreach (XmlNode NodeTStateVertex in NodeTransition.SelectNodes("//UML:Transition[@xmi.id='" + t.Id + "']//UML:Transition.source//UML:StateVertex", ns))
                        {
                            t.Source = NodeTStateVertex.Attributes["xmi.idref"].Value;

                        }
                        foreach (XmlNode NodeStateVertex in NodeTransition.SelectNodes("//UML:Transition[@xmi.id='" + t.Id + "']//UML:Transition.target//UML:StateVertex", ns))
                        {
                            t.Target = NodeStateVertex.Attributes["xmi.idref"].Value;

                        }

                        foreach (XmlNode NodeNodeStateMachine in NodeTransition.SelectNodes("//UML:Transition[@xmi.id='" + t.Id + "']//UML:Transition.stateMachine//UML:StateMachine", ns))
                        {
                            t.refDiagramActivity = NodeNodeStateMachine.Attributes["xmi.idref"].Value;

                        }
                        dicTransition.Add(t.Id, t);
                    }
                }
            return dicTransition;
        }
    }
}
