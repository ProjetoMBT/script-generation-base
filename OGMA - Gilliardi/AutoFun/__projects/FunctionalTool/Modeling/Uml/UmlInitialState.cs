using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows;
using System.Windows.Forms;
using FunctionalTool.Exceptions;

namespace FunctionalTool.Modeling.Uml
{
    public class UmlInitialState :UmlActionState {
        
        public static UmlInitialState ParserPseudostate(XmlNode activityDiagramNode, String id)
        {
            UmlInitialState stateBegin = new UmlInitialState();
            foreach (XmlNode NodePseudostate in activityDiagramNode.SelectNodes("//UML:ActivityGraph[@xmi.id='" + id + "']//UML:Pseudostate", ns))
            {
             
                stateBegin.stateInicio = NodePseudostate.Attributes["xmi.id"].Value;
                stateBegin.Id = NodePseudostate.Attributes["xmi.id"].Value;
                stateBegin.Name = NodePseudostate.Attributes["name"].Value;

                foreach (XmlNode NodePseudoTransition in NodePseudostate.SelectNodes("//UML:Pseudostate[@xmi.id='" + stateBegin.Id + "']//UML:StateVertex.outgoing//UML:Transition", ns))
                {
                    stateBegin.outgoing = NodePseudoTransition.Attributes["xmi.idref"].Value;
                }
            }
            if (stateBegin.outgoing == null)
            {
                throw new InvalidBeginNode();
            }
            return stateBegin;
        }
        
    }
}
