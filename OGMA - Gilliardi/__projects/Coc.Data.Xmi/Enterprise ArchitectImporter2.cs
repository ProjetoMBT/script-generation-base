using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coc.Modeling.Uml;
using System.Xml;
using Coc.Data.Interfaces;
using Coc.Data.ControlAndConversionStructures;
using Coc.Data.ControlAndConversionStructures;

namespace Coc.Data.Xmi
{
    public class Enterprise_ArchitectImporter2 : Parser
    {
        #region Atributos
        public List<UmlTransition> listTransition { get; set; }
        public Dictionary<String, UmlElement> dicUseCaseAndActor { get; set; }
        public Dictionary<String, UmlBase> dicBase { get; set; }
        #endregion

        #region Construtor
        public Enterprise_ArchitectImporter2()
        {
            listTransition = new List<UmlTransition>();
            dicBase = new Dictionary<string, UmlBase>();
            dicUseCaseAndActor = new Dictionary<string, UmlElement>();
        }
        #endregion

        #region Métodos Publicos
        public override StructureCollection ParserMethod(String path, ref String name, Tuple<String, Object>[] args)
        {
            StructureCollection model = new StructureCollection();
            XmlDocument document = new XmlDocument();
            document.Load(path);
            listModelingStructure.Add(FromXmi(document, ref name));
            model.listGeneralStructure.AddRange(listModelingStructure);
            return model;
        }
       
        public UmlModel FromXmi(XmlDocument doc, ref String name)
        {
            #region Diagrama de atividades
            #region Inicialização dos OBJ
            UmlModel model = new UmlModel("");
            //uml and astah namespaces
            XmlNamespaceManager nsManager = new XmlNamespaceManager(doc.NameTable);
            nsManager.AddNamespace("UML", "omg.org/UML1.3");
            XmlNodeList modelElements = doc.SelectNodes("//UML:Model[@name]", nsManager);
            List<List<string>> listId = new List<List<string>>();
            #endregion

            #region Buscar Action State
            foreach (XmlNode state in doc.SelectNodes("//UML:ActionState[@xmi.id]", nsManager))
            {
                UmlActionState actionState = new UmlActionState();
                actionState.Id = state.Attributes["xmi.id"].Value;
                actionState.Name = state.Attributes["name"].Value;
                dicBase.Add(actionState.Id, actionState);
            }
            #endregion

            #region Buscar PseudoState
            foreach (XmlNode pseudo in doc.SelectNodes("//UML:PseudoState[@xmi.id]", nsManager))
            {
                GetElementActivityDiagram(pseudo);
            }
            #endregion

            #region Buscar Transitions
            foreach (XmlNode transitionNode in doc.SelectNodes("//UML:Transition[@xmi.id]", nsManager))
            {
                UmlTransition transition = new UmlTransition();
                transition.Id = transitionNode.Attributes["xmi.id"].Value;
                string sourceID = transitionNode.Attributes["source"].Value;
                string targetID = transitionNode.Attributes["target"].Value;
                transition.Source = (UmlActionState)dicBase[sourceID];
                transition.Target = (UmlActionState)dicBase[targetID];
                foreach (XmlNode taggedValue in transitionNode.SelectNodes("//UML:Transition[@xmi.id ='" + transition.Id + "']//UML:ModelElement.taggedValue//UML:TaggedValue", nsManager))
                {
                    String tag = taggedValue.Attributes["tag"].Value;
                    String value = taggedValue.Attributes["value"].Value;
                    if (tag.Equals("TDaction", StringComparison.CurrentCultureIgnoreCase) || tag.Equals("TDexpectedResult", StringComparison.CurrentCultureIgnoreCase))
                    {
                        transition.TaggedValues.Add(tag, value);
                    }
                }
                listTransition.Add(transition);
            }
            #endregion

            #region ForkJoin e Popula Dicionario
            ForkOrJoin(model); // Percorre os sources se tiver alguma transição com o mesmo source ele troca para Fork remove o Join padrão e adiciona o Fork criado no lugar dele
            populateDictionary(listTransition); //coloca a lista de transições dentro do dicionario de UMLBase
            #endregion

            #endregion

            #region Diagrama de casos de uso
            #region  Buscar Actor
            foreach (XmlNode actorNode in doc.SelectNodes("//UML:Actor[@xmi.id]", nsManager))
            {
                UmlActor actor = new UmlActor();
                actor.Id = actorNode.Attributes["xmi.id"].Value;
                actor.Name = actorNode.Attributes["name"].Value;
                dicBase.Add(actor.Id, actor);
                dicUseCaseAndActor.Add(actor.Name, actor);
            }
            #endregion

            #region Buscar Casos de Uso
            foreach (XmlNode useCaseNode in doc.SelectNodes("//UML:UseCase[@xmi.id]", nsManager))
            {
                UmlUseCase usecase = new UmlUseCase();
                usecase.Id = useCaseNode.Attributes["xmi.id"].Value;
                usecase.Name = useCaseNode.Attributes["name"].Value;
                dicBase.Add(usecase.Id, usecase);
                dicUseCaseAndActor.Add(usecase.Name, usecase);
            }

            #endregion


            #region Buscar Associations
            foreach (XmlNode associationNode in doc.SelectNodes("//UML:Association[@xmi.id]", nsManager))
            {
                UmlAssociation association = new UmlAssociation();
                association.Id = associationNode.Attributes["xmi.id"].Value;
                foreach (XmlNode taggedValue in associationNode.SelectNodes("//UML:Association[@xmi.id ='" + association.Id + "']//UML:ModelElement.taggedValue//UML:TaggedValue", nsManager))
                {
                    //String tag = taggedValue.Attributes["tag"].Value;
                    String value = taggedValue.Attributes["value"].Value;
                    string aux1 = taggedValue.Attributes["tag"].Value;
                    if (aux1 == "ea_sourceName")
                    {
                        association.End1 = (UmlElement)dicUseCaseAndActor[value];
                    }
                    else if (aux1 == "ea_targetName")
                    {
                        association.End2 = (UmlElement)dicUseCaseAndActor[value];
                    }
                }
                dicBase.Add(association.Id, association);
            }
            #endregion
            #endregion

            #region Buscar Diagramas

            foreach (XmlNode diagrams in doc.SelectNodes("//UML:Diagram[@xmi.id]", nsManager))
            {
                //Diagrama de Caso de uso
                if (diagrams.Attributes["diagramType"].Value.Equals("UseCaseDiagram"))
                {
                    UmlUseCaseDiagram useCaseDiagram = new UmlUseCaseDiagram();
                    useCaseDiagram.Name = diagrams.Attributes["name"].Value;
                    useCaseDiagram.Id = diagrams.Attributes["xmi.id"].Value;

                    foreach (XmlNode elementNode in doc.SelectNodes("//UML:Diagram[@xmi.id='" + useCaseDiagram.Id + "']//UML:Diagram.element//UML:DiagramElement[@subject]", nsManager))
                    {
                        useCaseDiagram.UmlObjects.Add(dicBase[elementNode.Attributes["subject"].Value]);
                    }
                    model.AddDiagram(useCaseDiagram);

                }
                //Diagrama de Atividades
                else if (diagrams.Attributes["diagramType"].Value.Equals("ActivityDiagram"))
                {
                    UmlActivityDiagram actDiagram = new UmlActivityDiagram(diagrams.Attributes["name"].Value);
                    actDiagram.Name = diagrams.Attributes["name"].Value;
                    actDiagram.Id = diagrams.Attributes["xmi.id"].Value;
                    foreach (XmlNode elementNode in doc.SelectNodes("//UML:Diagram[@xmi.id='" + actDiagram.Id + "']//UML:Diagram.element//UML:DiagramElement[@subject]", nsManager))
                    {
                        actDiagram.UmlObjects.Add(dicBase[elementNode.Attributes["subject"].Value]);
                    }
                    model.AddDiagram(actDiagram);
                }
            }
            #endregion

            return model;

        }
        #endregion

        #region Metodos Privados
        private void populateDictionary(List<UmlTransition> listTransition)
        {
            foreach (UmlTransition t in listTransition)
            {
                dicBase.Add(t.Id, t);
            }
        }

        private void ForkOrJoin(UmlModel model)
        {

            for (int i = 0; i < listTransition.Count; i++)
            {
                UmlTransition t1 = listTransition[i];
                int count = (listTransition.Where(x => x.Source.Id.Equals(t1.Source.Id)).Count());
                if (count > 1)
                {
                    //Source
                    UmlFork fork = new UmlFork();
                    fork.Id = t1.Source.Id;
                    fork.Name = t1.Source.Name;
                    t1.Source = fork;
                    List<UmlTransition> listtransition = listTransition.Where(x => x.Target.Id.Equals(t1.Source.Id)).ToList();
                    UmlTransition t2 = listtransition[0];

                    t2.Target = fork;
                }
            }
        }

        private void GetElementActivityDiagram(XmlNode pseudo)
        {
            String kind;
            try
            {
                kind = pseudo.Attributes["kind"].Value;
                if (kind == "final")
                {
                    UmlFinalState final = new UmlFinalState();
                    final.Id = pseudo.Attributes["xmi.id"].Value;
                    try
                    {
                        final.Name = pseudo.Attributes["name"].Value;
                    }
                    catch
                    {
                        final.Name = "";
                    }
                    dicBase.Add(final.Id, final);
                }
                if (kind == "join")
                {
                    UmlJoin join = new UmlJoin();
                    join.Id = pseudo.Attributes["xmi.id"].Value;
                    try
                    {
                        join.Name = pseudo.Attributes["name"].Value;
                    }
                    catch
                    {
                        join.Name = "";
                    }
                    dicBase.Add(join.Id, join);
                }
                if (kind == "branch")
                {
                    UmlDecision decision = new UmlDecision();
                    decision.Id = pseudo.Attributes["xmi.id"].Value;
                    try
                    {
                        decision.Name = pseudo.Attributes["name"].Value;
                    }
                    catch
                    {
                        decision.Name = "";
                    }
                    dicBase.Add(decision.Id, decision);
                }
            }
            catch (Exception)
            {
                UmlInitialState initial = new UmlInitialState();
                initial.Id = pseudo.Attributes["xmi.id"].Value;
                initial.Name = pseudo.Attributes["name"].Value;
                dicBase.Add(initial.Id, initial);
            }

        }

        private List<GeneralUseStructure> listModelingStructure = new List<GeneralUseStructure>();
        #endregion
    }
}