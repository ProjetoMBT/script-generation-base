/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package Servico;

import Diagrams.UmlDiagram;
import Objects.UmlActionState;
import Objects.UmlActor;
import Objects.UmlAssociation;
import Objects.UmlBase;
import Objects.UmlFinalState;
import Objects.UmlInitialState;
import Objects.UmlModel;
import Objects.UmlTransition;
import Objects.UmlUseCase;
import argoparser.Configuration;
import java.io.File;
import java.io.FileWriter;
import java.io.IOException;
import java.util.Map;
import org.jdom2.Document;
import org.jdom2.Element;
import org.jdom2.output.XMLOutputter;

/**
 *
 * @author COC-7-01
 */
public class ArgoUMLtoAstahXML
{
    public void ToXmi(UmlModel model) throws InterruptedException, IOException
    {
        Document doc = new Document();
        Element Emodel = new Element(model.getClass().getName());
        Emodel.setAttribute("id", model.getId() + "");
        Emodel.setAttribute("name", model.getName() + "");
        doc.setRootElement(Emodel);
        for (UmlDiagram diagram : model.getDiagrams())
        {
            Element EDiagram = new Element(diagram.getClass().getName());
            EDiagram.setAttribute("id", diagram.getId() + "");
            EDiagram.setAttribute("name", diagram.getName() + "");

            for (UmlBase umlObject : diagram.getUmlObjects())
            {
                //verificar o conteudo para a tag tdaction e substituir
                String action = umlObject.getTaggedValues().get("TDACTION");
                if(action !=null){
                    action = action.replaceAll("%", "%25");
                    umlObject.getTaggedValues().put("TDACTION", action);
                }
                
                Element EUmlObject = getElement(umlObject);
                EDiagram.addContent(EUmlObject);
                
            }
            Emodel.addContent(EDiagram);
            //doc.setRootElement(EDiagram);
        }

        //Imprimindo o XML
        File out = File.createTempFile("IntermediateFile", ".xml");//, new File(Configuration.getInstance().getProperty("workspacepath")));
        
        
        try (FileWriter arquivo = new FileWriter(out))
        {
            XMLOutputter xout = new XMLOutputter();
            xout.output(doc, arquivo);
        }
        catch (Exception e)
        {
            throw new IllegalAccessError("Não foi possivel escrever o arquivo xml.");
        }
    }

    private Element getElement(UmlBase umlObject)
    {
        Element EUmlObject = new Element(umlObject.getClass().getName());

        if (umlObject instanceof UmlInitialState)
        {
            EUmlObject.setAttribute("id", umlObject.getId());
            EUmlObject.setAttribute("name", "initialState");
            return EUmlObject;
        }
        if (umlObject instanceof UmlFinalState)
        {
            EUmlObject.setAttribute("id", umlObject.getId());
            EUmlObject.setAttribute("name", "finalState");
            return EUmlObject;
        }
        if (umlObject instanceof UmlActionState)
        {
            EUmlObject.setAttribute("id", umlObject.getId());
            EUmlObject.setAttribute("name", umlObject.getName());
            return EUmlObject;
        }
        if (umlObject instanceof UmlTransition)
        {
            UmlTransition t = (UmlTransition) umlObject;
            EUmlObject.setAttribute("id", t.getId());
            //Source State
            //EUmlObject.setAttribute("stateSourceName", t.getSource().getName());
            EUmlObject.setAttribute("stateSourceId", t.getSource().getId());
            //Target State
            //EUmlObject.setAttribute("stateTargetName", t.getTarget().getName());
            EUmlObject.setAttribute("stateTargetId", t.getTarget().getId());

            //Tags da Transition
            for (Map.Entry<String, String> pair : t.getTaggedValues().entrySet())
            {
                Element children = new Element("TAG");
                children.setAttribute("tagName", pair.getKey());
                children.setAttribute("tagValue", pair.getValue());
                EUmlObject.addContent(children);
            }
            return EUmlObject;
        }
        if (umlObject instanceof UmlActor)
        {
            UmlActor a = (UmlActor) umlObject;
            EUmlObject.setAttribute("id", umlObject.getId());
            EUmlObject.setAttribute("name", umlObject.getName());

            for (Map.Entry<String, String> pair : a.getTaggedValues().entrySet())
            {
                Element children = new Element("TAG");
                children.setAttribute("tagName", pair.getKey());
                children.setAttribute("tagValue", pair.getValue());
                EUmlObject.addContent(children);
            }
            return EUmlObject;
        }
        if (umlObject instanceof UmlUseCase)
        {
            EUmlObject.setAttribute("id", umlObject.getId());
            EUmlObject.setAttribute("name", umlObject.getName());
            return EUmlObject;
        }

        if (umlObject instanceof UmlAssociation)
        {
            UmlAssociation a = (UmlAssociation) umlObject;
            EUmlObject.setAttribute("id", a.getId());
            //Source State
            //EUmlObject.setAttribute("stateSourceName", t.getSource().getName());
            EUmlObject.setAttribute("end1Id", a.getEnd1().getId());
            //Target State
            //EUmlObject.setAttribute("stateTargetName", t.getTarget().getName());
            EUmlObject.setAttribute("end2Id", a.getEnd2().getId());
            return EUmlObject;
        }
        return null;
    }
}
