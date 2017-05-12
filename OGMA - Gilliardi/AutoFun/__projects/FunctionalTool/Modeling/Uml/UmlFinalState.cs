using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using FunctionalTool.Exceptions;

namespace FunctionalTool.Modeling.Uml
{
    public class UmlFinalState : UmlActionState{

        public static UmlFinalState ParserFinalState(XmlNode activityDiagramNode, String id)
        {
            UmlFinalState stateEnd = new UmlFinalState();
            foreach (XmlNode NodeFinalState in activityDiagramNode.SelectNodes("//UML:ActivityGraph[@xmi.id='" + id + "']//UML:FinalState ", ns))
            {
                stateEnd.stateInicio = NodeFinalState.Attributes["xmi.id"].Value;
                stateEnd.Id = NodeFinalState.Attributes["xmi.id"].Value;
                stateEnd.Name = NodeFinalState.Attributes["name"].Value;

                foreach (XmlNode NodeStateVertex in NodeFinalState.SelectNodes("//UML:FinalState[@xmi.id='" + stateEnd.Id + "']//UML:StateVertex.incoming//UML:Transition", ns))
                {
                    stateEnd.incoming = NodeStateVertex.Attributes["xmi.idref"].Value;
                }
            }
            if (stateEnd.incoming == null)
            {
                throw new InvalidEndNode();
            }
            return stateEnd;
        }
    
    
    
    
    
    
    
    
    
    
    
    
    }
}
