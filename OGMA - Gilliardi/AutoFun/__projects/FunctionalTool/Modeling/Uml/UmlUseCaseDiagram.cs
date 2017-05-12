using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace FunctionalTool.Modeling.Uml
{
    public class UmlUseCaseDiagram : UmlDiagram
    {
        public Dictionary<String, UmlActor> actors { get; set; }
        public Dictionary<String, UmlUseCase> useCases { get; set; }
       

        public UmlUseCaseDiagram()
        {
            actors = new Dictionary<string, UmlActor>();
            useCases = new Dictionary<string, UmlUseCase>();
        }


        public static UmlUseCaseDiagram ParseUmlUseCaseDiagram(XmlDocument doc, Dictionary<String, UmlActionStateDiagram> dicActionDiagram)
        {
                UmlUseCaseDiagram useCaseDiagram = new UmlUseCaseDiagram();
                foreach (XmlNode caseDiagramNode in doc.SelectNodes("//JUDE:Diagram", ns))
                {
                    if (caseDiagramNode.Attributes["xmi.id"] != null)
                    {
                        useCaseDiagram.Id = caseDiagramNode.Attributes["xmi.id"].Value;
                        useCaseDiagram.Name = caseDiagramNode.Attributes["name"].Value;
                        useCaseDiagram.actors = UmlActor.ParseActor(caseDiagramNode);
                        useCaseDiagram.useCases = UmlUseCase.ParseUseCase(caseDiagramNode, dicActionDiagram, doc);
                        return useCaseDiagram;
                    }
                }
            return null;
        }
    }
}
