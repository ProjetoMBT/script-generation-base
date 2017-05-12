using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows;
using System.Windows.Forms;

namespace FunctionalTool.Modeling.Uml
{
    public class UmlActionStateDiagram : UmlDiagram
    {
        public UmlInitialState InitialActivity { get; set; }
        public UmlFinalState FinalActivity { get; set; }
        public Dictionary<String, UmlActionState> dicAtivities { get; set; }
        public Dictionary<String, UmlTransition> transitions { get; set; }
        public List<UmlNote> Notes { get; set; }
        public String refDiagramJude { get; set; }
        public static Dictionary<String, String> dicJudeHyperLinks = new Dictionary<string, string>();

        public UmlActionStateDiagram()
        {
            dicAtivities = new Dictionary<string, UmlActionState>();
        }

        private static void collectJudeHyperLinksReferences(XmlDocument doc)
        {
            String judeId, diagramId;
            foreach (XmlNode JudeDiagramNode in doc.SelectNodes("//JUDE:ActivityDiagram", ns))
            {

                judeId = JudeDiagramNode.Attributes["xmi.id"].Value;
                XmlNode model = JudeDiagramNode.SelectSingleNode("//JUDE:ActivityDiagram[@xmi.id='" + judeId + "']" + "//JUDE:StateChartDiagram.semanticModel//UML:ActivityGraph", ns);

                diagramId = model.Attributes["xmi.idref"].Value;
                UmlActionStateDiagram.dicJudeHyperLinks.Add(judeId, diagramId);
            }
        }

        public static Dictionary<String, UmlActionStateDiagram> ParseActivityDiagram(XmlDocument doc)
        {

            UmlActionStateDiagram.collectJudeHyperLinksReferences(doc);
            Dictionary<String, UmlActionStateDiagram> dictionaryActivityDiagram = new Dictionary<string, UmlActionStateDiagram>();

           
                foreach (XmlNode activityDiagramNode in doc.SelectNodes("//UML:Model//UML:ActivityGraph", ns))
                {
                    if (activityDiagramNode.Attributes["xmi.id"] != null)
                    {
                        UmlActionStateDiagram activityDiagram = new UmlActionStateDiagram();
                        activityDiagram.Name = activityDiagramNode.Attributes["name"].Value;
                        activityDiagram.Id = activityDiagramNode.Attributes["xmi.id"].Value;
                        activityDiagram.transitions = UmlTransition.ParserTransition(activityDiagramNode, activityDiagram.Id);
                        dictionaryActivityDiagram.Add(activityDiagram.Id, activityDiagram);
                        UmlActionState.ParseActivity(activityDiagramNode, activityDiagram.Id, activityDiagram);
                        activityDiagram.InitialActivity = UmlInitialState.ParserPseudostate(activityDiagramNode, activityDiagram.Id);
                        activityDiagram.FinalActivity = UmlFinalState.ParserFinalState(activityDiagramNode, activityDiagram.Id);
                    }
                }
                foreach (var item in dictionaryActivityDiagram.Keys)
                {
                    foreach (var item2 in dictionaryActivityDiagram[item].dicAtivities.Keys)
                    {
                        UmlActionState activity = dictionaryActivityDiagram[item].dicAtivities[item2];
                        foreach (String item3 in activity.dicJudeHyperLink.Keys)
                        {
                            //Id UML
                            String key = UmlActionStateDiagram.dicJudeHyperLinks[item3];
                            UmlActionStateDiagram diagram = dictionaryActivityDiagram[key];
                            activity.dicMyLinkedDiagrams.Add(diagram.Id, diagram);
                        }
                    }
                }
            
            return dictionaryActivityDiagram;
        }
        public UmlTransition buscaTransition(String id, Dictionary<String, UmlActionStateDiagram> dicActionDiagram)
        {

            foreach (String keyDiagram in dicActionDiagram.Keys)
            {
                UmlActionStateDiagram d = dicActionDiagram[keyDiagram];

                foreach (String keyTransition in d.transitions.Keys)
                {
                    if (id.Equals(keyTransition))
                    {
                        UmlTransition t = d.transitions[keyTransition];
                        return t;
                    }
                }
            }
            return null;
        }
        public static void cleardicJudeHyperLinks()
        {
            UmlActionStateDiagram.dicJudeHyperLinks.Clear();
        }
    }
}
