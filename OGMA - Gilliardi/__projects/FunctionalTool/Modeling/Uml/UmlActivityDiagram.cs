using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace FunctionalTool.Modeling.Uml
{
    public class UmlActivityDiagram : UmlDiagram{
        public UmlInitialActivity InitialActivity {get;set;}
        public UmlFinalActivity FinalActivity { get; set; }
        public Dictionary<String,UmlActivity> dicAtivities {get;set;}
        public List<UmlFlow> Flows{get;set;}
        public List<UmlNote> Notes {get;set;}
        public static Dictionary<String, String> dicJudeHyperLinks = new Dictionary<string, string>();

        public UmlActivityDiagram() {
            dicAtivities = new Dictionary<string, UmlActivity>();
         }

        private static void collectJudeHyperLinksReferences(XmlDocument doc)
        {
            String judeId, diagramId;
            foreach (XmlNode JudeDiagramNode in doc.SelectNodes("//JUDE:ActivityDiagram", ns))
            {
                XmlNode model = JudeDiagramNode.SelectSingleNode("//JUDE:StateChartDiagram.semanticModel//UML:ActivityGraph", ns);
                judeId = JudeDiagramNode.Attributes["xmi.id"].Value;
                diagramId = model.Attributes["xmi.idref"].Value;
                UmlActivityDiagram.dicJudeHyperLinks.Add(judeId, diagramId);
            }
        }

        public static void ParseActivityDiagram(XmlDocument doc, Dictionary<String, UmlActivityDiagram> dictionaryActivityDiagram)
        {
            UmlActivityDiagram.collectJudeHyperLinksReferences(doc);

            foreach (XmlNode activityDiagramNode in doc.SelectNodes("//UML:Model//UML:ActivityGraph", ns))
            {
                try
                {
                    UmlActivityDiagram activityDiagram = new UmlActivityDiagram();
                    activityDiagram.Name = activityDiagramNode.Attributes["name"].Value;
                    activityDiagram.Id = activityDiagramNode.Attributes["xmi.id"].Value;
                    dictionaryActivityDiagram.Add(activityDiagram.Id, activityDiagram);

                    foreach (XmlNode ActionStateNode in activityDiagramNode.SelectNodes("//UML:ActivityGraph[@xmi.id='" + activityDiagram.Id + "']//UML:ActionState", ns))
                    {
                        UmlActivity activity = new UmlActivity();
                        activity.Id = ActionStateNode.Attributes["xmi.id"].Value;
                        activity.Name = ActionStateNode.Attributes["name"].Value;

                        foreach (XmlNode NodeHyperLink in ActionStateNode.SelectNodes("//UML:ActionState[@xmi.id='" + activity.Id + "']//UML:TaggedValue", ns))
                        {
                            UmlTag tag = new UmlTag();
                            tag.name = NodeHyperLink.Attributes["tag"].Value;
                            tag.id = NodeHyperLink.Attributes["xmi.id"].Value;
                            activityDiagram.dicAtivities.Add(activity.Id, activity);

                            String hyperLink = NodeHyperLink.Attributes["value"].Value;

                            if (tag.name.Equals("jude.hyperlink"))
                            {
                                tag.hyperLink = retiraCabecalhoECauda(hyperLink);
                                activity.dicHyperLink.Add(tag.id, tag.hyperLink);

                            }
                            else
                            {
                                activity.dictionaryTag.Add(tag.id, tag);
                            }
                        }

                        foreach (XmlNode NodeStateVertex in ActionStateNode.SelectNodes("//UML:ActionState[@xmi.id='" + activity.Id + "']//UML:StateVertex.outgoing//UML:Transition", ns))
                        {
                            activity.outgoing = NodeStateVertex.Attributes["xmi.idref"].Value;
                        }
                        foreach (XmlNode NodeStateVertex in ActionStateNode.SelectNodes("//UML:ActionState[@xmi.id='" + activity.Id + "']//UML:StateVertex.incoming//UML:Transition", ns))
                        {
                            activity.incoming = NodeStateVertex.Attributes["xmi.idref"].Value;
                        }
                        
                    }
                    activityDiagram.InitialActivity = UmlInitialActivity.ParserPseudostate(activityDiagramNode, activityDiagram.Id);
                    activityDiagram.FinalActivity = UmlFinalActivity.ParserFinalState(activityDiagramNode, activityDiagram.Id);  
                }catch(Exception){}
            }

            foreach (var item in dictionaryActivityDiagram.Keys)
            {
                foreach (var item2 in dictionaryActivityDiagram[item].dicAtivities.Keys)
                {
                  dictionaryActivityDiagram[item].dicAtivities[item2].ActivityAssociationAtivityDiagram(dictionaryActivityDiagram);
                }
            }
        }

        private static String retiraCabecalhoECauda(String s)
        {
            String result = s.Substring(22);
            String[] aux = result.Split('%');
            return aux[0];
        }
    }
}
