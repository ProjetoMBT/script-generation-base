package Servico;

import Diagrams.UmlActivityDiagram;
import Diagrams.UmlUseCaseDiagram;
import Objects.UmlActionState;
import Objects.UmlActor;
import Objects.UmlAssociation;
import Objects.UmlElement;
import Objects.UmlFinalState;
import Objects.UmlModel;
import Objects.UmlPseudoState;
import Objects.UmlTransition;
import Objects.UmlUseCase;
import java.util.HashMap;
import java.util.UUID;
import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.NodeList;

public class PopulateStructureModel
{

    public UmlModel methodPopulateModel(Document doc)
    {
        UmlModel model = new UmlModel();
        UmlUseCaseDiagram ucDiagram = new UmlUseCaseDiagram();
        ucDiagram.setName("UseCase Diagram");
        ucDiagram.setId(UUID.randomUUID().toString());
        ucDiagram.setParentModel(model);
        model.addDiagram(ucDiagram);
        //model.getDiagrams().add(ucDiagram);
        HashMap<String, UmlActionState> activityDiagramStates = new HashMap<>();
        HashMap<String, UmlElement> useCaseDiagramStates = new HashMap<>();
        FactoryPseudoStateType factoryPseudoState = new FactoryPseudoStateType();
        HashMap<String, String> tagDefinitions = new HashMap<>();
        NodeList queue_umlModel = doc.getElementsByTagName("UML:Model");
        Element umlModel = (Element) queue_umlModel.item(0);
        model.setId(umlModel.getAttribute("xmi.id"));
        model.setName(umlModel.getAttribute("name"));
        NodeList queue_TagDefinitions = umlModel.getElementsByTagName("UML:TagDefinition");

        for (int i = 0; i < queue_TagDefinitions.getLength(); i++)
        {
            Element td = (Element) queue_TagDefinitions.item(i);
            if (td.hasAttribute("xmi.id") && td.hasAttribute("name"))
            {
                String id = td.getAttribute("xmi.id");
                String name = td.getAttribute("name");
                tagDefinitions.put(id, name);
            }
        }
        //UseCase Diagram
        //Actor
        NodeList queue_umlActor = umlModel.getElementsByTagName("UML:Actor");
        for (int i = 0; i < queue_umlActor.getLength(); i++)
        {
            Element umlActor = (Element) queue_umlActor.item(i);
            if (umlActor.hasAttribute("xmi.id"))
            {
                UmlActor actor = new UmlActor();
                actor.setId(umlActor.getAttribute("xmi.id"));
                actor.setName(umlActor.getAttribute("name"));
                //Tags
                NodeList queue_TaggedValues = umlActor.getElementsByTagName("UML:TaggedValue");
                for (int k = 0; k < queue_TaggedValues.getLength(); k++)
                {
                    Element tag = (Element) queue_TaggedValues.item(k);
                    NodeList auxList = tag.getElementsByTagName("UML:TagDefinition");
                    Element auxListValue = (Element) auxList.item(0);
                    String key = auxListValue.getAttribute("xmi.idref");
                    NodeList auxList2 = tag.getElementsByTagName("UML:TaggedValue.dataValue");
                    Element auxListValue2 = (Element) auxList2.item(0);
                    String value = auxListValue2.getTextContent();
                    String tagName = tagDefinitions.get(key);
                    actor.addTag(tagName, value);
                }
                useCaseDiagramStates.put(actor.getId(), actor);

                ucDiagram.getUmlObjects().add(actor);
            }
        }
        //UseCase
        NodeList queue_umlUseCase = umlModel.getElementsByTagName("UML:UseCase");
        for (int i = 0; i < queue_umlUseCase.getLength(); i++)
        {
            Element umlUseCase = (Element) queue_umlUseCase.item(i);
            if (umlUseCase.hasAttribute("xmi.id"))
            {
                UmlUseCase uc = new UmlUseCase();
                uc.setId(umlUseCase.getAttribute("xmi.id"));
                uc.setName(umlUseCase.getAttribute("name"));
                useCaseDiagramStates.put(uc.getId(), uc);

                ucDiagram.getUmlObjects().add(uc);
            }
        }
        //Association
        NodeList queue_umlAssociation = umlModel.getElementsByTagName("UML:Association");
        for (int i = 0; i < queue_umlAssociation.getLength(); i++)
        {
            Element umlAssociation = (Element) queue_umlAssociation.item(i);
            UmlAssociation association = new UmlAssociation();
            association.setId(umlAssociation.getAttribute("xmi.id"));
            association.setName(umlAssociation.getAttribute("name"));

            //Association End1
            NodeList queue_AssociationEnd = umlAssociation.getElementsByTagName("UML:AssociationEnd.participant");
            Element end1 = (Element) queue_AssociationEnd.item(0);
            String idEnd1 = getTransitionElementID(end1);
            association.setEnd1(useCaseDiagramStates.get(idEnd1));
            //Association End2
            Element end2 = (Element) queue_AssociationEnd.item(1);
            String idEnd2 = getTransitionElementID(end2);
            association.setEnd2(useCaseDiagramStates.get(idEnd2));

            ucDiagram.getUmlObjects().add(association);
        }
        //Activity Diagram
        NodeList queue_umlActivityGraph = umlModel.getElementsByTagName("UML:ActivityGraph");
        for (int i = 0; i < queue_umlActivityGraph.getLength(); i++)
        {
            Element umlActivityGraph = (Element) queue_umlActivityGraph.item(i);
            UmlActivityDiagram actDiagram = new UmlActivityDiagram();
            actDiagram.setId(umlActivityGraph.getAttribute("xmi.id"));
            actDiagram.setName(umlActivityGraph.getAttribute("name"));
            actDiagram.setParentModel(model);
            //Pseudostates
            NodeList queue_umlPseudostate = umlActivityGraph.getElementsByTagName("UML:Pseudostate");
            for (int j = 0; j < queue_umlPseudostate.getLength(); j++)
            {
                Element umlPseudoState = (Element) queue_umlPseudostate.item(j);
                if (umlPseudoState.hasAttribute("kind") && umlPseudoState.hasAttribute("xmi.id"))
                {
                    String kind = umlPseudoState.getAttribute("kind");
                    UmlPseudoState pseudoState = factoryPseudoState.methodFactoryPseudoStateType(kind);
                    pseudoState.setId(umlPseudoState.getAttribute("xmi.id"));
                    actDiagram.getUmlObjects().add(pseudoState);
                    activityDiagramStates.put(pseudoState.getId(), pseudoState);
                }
            }
            //Final State
            NodeList queue_umlFinalState = umlActivityGraph.getElementsByTagName("UML:FinalState");
            for (int j = 0; j < queue_umlFinalState.getLength(); j++)
            {
                UmlFinalState stateFinal = new UmlFinalState();
                Element umlFinalState = (Element) queue_umlFinalState.item(j);
                if (umlFinalState.hasAttribute("xmi.id"))
                {
                    stateFinal.setId(umlFinalState.getAttribute("xmi.id"));
                    actDiagram.getUmlObjects().add(stateFinal);
                    activityDiagramStates.put(stateFinal.getId(), stateFinal);
                }
            }
            //Action States
            NodeList queue_umlActionState = umlActivityGraph.getElementsByTagName("UML:ActionState");
            for (int j = 0; j < queue_umlActionState.getLength(); j++)
            {
                UmlActionState actionState = new UmlActionState();
                Element umlActionState = (Element) queue_umlActionState.item(j);
                if (umlActionState.hasAttribute("xmi.id"))
                {
                    actionState.setId(umlActionState.getAttribute("xmi.id"));
                    actionState.setName(umlActionState.getAttribute("name"));
                    actDiagram.getUmlObjects().add(actionState);
                    activityDiagramStates.put(actionState.getId(), actionState);
                }
            }
            //Transitions
            NodeList queue_umltransition = umlActivityGraph.getElementsByTagName("UML:Transition");
            for (int j = 0; j < queue_umltransition.getLength(); j++)
            {
                UmlTransition transition = new UmlTransition();
                Element umlTransition = (Element) queue_umltransition.item(j);
                if (umlTransition.hasAttribute("xmi.id"))
                {
                    transition.setId(umlTransition.getAttribute("xmi.id"));
                    //Transition tags
                    NodeList queue_TaggedValues = umlTransition.getElementsByTagName("UML:TaggedValue");
                    for (int k = 0; k < queue_TaggedValues.getLength(); k++)
                    {
                        Element tag = (Element) queue_TaggedValues.item(k);
                        NodeList auxList = tag.getElementsByTagName("UML:TagDefinition");
                        Element auxListValue = (Element) auxList.item(0);
                        String key = auxListValue.getAttribute("xmi.idref");
                        NodeList auxList2 = tag.getElementsByTagName("UML:TaggedValue.dataValue");
                        Element auxListValue2 = (Element) auxList2.item(0);
                        String value = auxListValue2.getTextContent();
                        String tagName = tagDefinitions.get(key);
                        transition.addTag(tagName, value);
                    }
                    //Transition source
                    NodeList queue_TransitionSource = umlTransition.getElementsByTagName("UML:Transition.source");
                    Element source = (Element) queue_TransitionSource.item(0);
                    String idSource = getTransitionElementID(source);
                    transition.setSource(activityDiagramStates.get(idSource));
                    //Transition target
                    NodeList queue_TransitionTarget = umlTransition.getElementsByTagName("UML:Transition.target");
                    Element target = (Element) queue_TransitionTarget.item(0);
                    String idTarget = getTransitionElementID(target);
                    transition.setTarget(activityDiagramStates.get(idTarget));

                    actDiagram.getUmlObjects().add(transition);
                }
            }
            model.getDiagrams().add(actDiagram);
        }
        return model;
    }

    public String getTransitionElementID(Element element)
    {
        NodeList auxList;
        Element elementID;
        auxList = element.getElementsByTagName("UML:ActionState");
        if (auxList.getLength() != 0)
        {
            elementID = (Element) auxList.item(0);
            return elementID.getAttribute("xmi.idref");
        }
        auxList = element.getElementsByTagName("UML:Pseudostate");
        if (auxList.getLength() != 0)
        {
            elementID = (Element) auxList.item(0);
            return elementID.getAttribute("xmi.idref");
        }
        auxList = element.getElementsByTagName("UML:FinalState");
        if (auxList.getLength() != 0)
        {
            elementID = (Element) auxList.item(0);
            return elementID.getAttribute("xmi.idref");
        }
        auxList = element.getElementsByTagName("UML:Actor");
        if (auxList.getLength() != 0)
        {
            elementID = (Element) auxList.item(0);
            return elementID.getAttribute("xmi.idref");
        }
        auxList = element.getElementsByTagName("UML:UseCase");
        if (auxList.getLength() != 0)
        {
            elementID = (Element) auxList.item(0);
            return elementID.getAttribute("xmi.idref");
        }
        return null;
    }
}
