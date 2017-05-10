using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace FunctionalTool.Modeling.Uml
{
    public class UmlActor : UmlComponent
    {
        public String refUseCaseDiagram { get; set; }

        public static Dictionary<String, UmlActor> ParseActor(XmlNode node)
        {
            Dictionary<String, UmlActor> dictionaryActor = new Dictionary<string, UmlActor>();
            foreach (XmlNode actorNode in node.SelectNodes("//UML:Model/*/UML:Actor", ns))
            {
                UmlActor actor = new UmlActor();
                actor.Name = actorNode.Attributes["name"].Value;
                actor.Id = actorNode.Attributes["xmi.id"].Value;

                UmlTag.ParserTag(actorNode, ns, actor);

                foreach (XmlNode NodeTag in actorNode.SelectNodes("//UML:Model/*/UML:Actor[@xmi.id='" + actor.Id + "']//UML:Namespace", ns))
                {
                   actor.refUseCaseDiagram = NodeTag.Attributes["xmi.idref"].Value;  
                }
                dictionaryActor.Add(actor.Id, actor);
            }
            return dictionaryActor;
        }
    }
}
