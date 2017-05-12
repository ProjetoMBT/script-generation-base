using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Coc.Modeling.Uml;
using System.Text.RegularExpressions;
using Coc.Modeling.TestPlanStructure;

namespace Coc.Data.Xml.SequenceModelExport
{
    public class TestPlanToAstah
    {

        private static int depth;
        public static void ToXmi(List<TestCase> listTestCase)
        {
            UmlModel model = new UmlModel("ListTestPlan");
            populateModel(model, listTestCase);
            depth = 2147483636;
            int x = 50;
            double y = 150;
            double contLane = 0.0;
            XmlElement itemPresentation = null;
            XmlDocument document = new XmlDocument();
            document.LoadXml("<XMI></XMI>");

            XmlNamespaceManager nsManager = new XmlNamespaceManager(document.NameTable);
            nsManager.AddNamespace("JUDE", "http://objectclub.esm.co.jp/Jude/namespace/");
            nsManager.AddNamespace("UML", "org.omg.xmi.namespace.UML");

            //document header
            document.InsertBefore(document.CreateXmlDeclaration("1.0", "UTF-8", "yes"), document.DocumentElement);

            String umlNamespace = "org.omg.xmi.namespace.UML";
            String judeNamespace = "http://objectclub.esm.co.jp/Jude/namespace/";

            //root node attributes
            document.DocumentElement.SetAttribute("xmi.version", "1.1");
            document.DocumentElement.SetAttribute("xmlns:JUDE", judeNamespace);
            document.DocumentElement.SetAttribute("xmlns:UML", umlNamespace);

            #region XMI.header
            XmlElement header = document.CreateElement("XMI.header");
            document.DocumentElement.AppendChild(header);

            //XMI.header/XMI.documentation
            XmlElement documentation = document.CreateElement("XMI.documentation");
            header.AppendChild(documentation);

            XmlElement exporter = document.CreateElement("XMI.exporter");
            exporter.InnerText = "Jomt XMI writer";
            documentation.AppendChild(exporter);

            XmlElement exporterVersion = document.CreateElement("XMI.exporterVersion");
            exporterVersion.InnerText = "1.2.0.36";
            documentation.AppendChild(exporterVersion);

            XmlElement currentModelVersion = document.CreateElement("XMI.currentModelVersion");
            currentModelVersion.InnerText = "36";
            documentation.AppendChild(currentModelVersion);

            XmlElement maxModelVersion = document.CreateElement("XMI.maxModelVersion");
            maxModelVersion.InnerText = "36";
            documentation.AppendChild(maxModelVersion);

            XmlElement currentModelProducer = document.CreateElement("XMI.currentModelProducer");
            currentModelProducer.InnerText = "A.P";
            documentation.AppendChild(currentModelProducer);

            XmlElement sortedVersionHistories = document.CreateElement("XMI.sortedVersionHistories");
            documentation.AppendChild(sortedVersionHistories);

            XmlElement versionEntry = document.CreateElement("XMI.versionEntry");
            versionEntry.SetAttribute("productVersion", "Professional 6.6.3");
            versionEntry.SetAttribute("modelVersion", "36");
            sortedVersionHistories.AppendChild(versionEntry);

            XmlElement metaModel = document.CreateElement("XMI.metamodel");
            header.AppendChild(metaModel);
            metaModel.SetAttribute("xmi.name", "UML");
            metaModel.SetAttribute("xmi.version", "1.4");

            #endregion
            #region XMI.content/UML:ModelElement.behavior

            XmlElement content = document.CreateElement("XMI.content");
            document.DocumentElement.AppendChild(content);

            XmlElement modelElement = document.CreateElement("UML:Model", umlNamespace);
            modelElement.SetAttribute("xmi.id", model.Id);
            modelElement.SetAttribute("name", model.Name);
            modelElement.SetAttribute("version", "0");
            modelElement.SetAttribute("unSolvedFlag", "false");
            modelElement.SetAttribute("isRoot", "true");
            modelElement.SetAttribute("isLeaf", "false");
            modelElement.SetAttribute("isAbstract", "false");
            modelElement.SetAttribute("xmlns:UML", "org.omg.xmi.namespace.UML");
            content.AppendChild(modelElement);

            XmlElement modelBehaviour = null;
            XmlElement activityDiagram = null;

            foreach (UmlDiagram diagram in model.Diagrams)
            {
                if (diagram is UmlActivityDiagram)
                {
                    modelBehaviour = document.CreateElement("UML:ModelElement.behavior", umlNamespace);
                    modelElement.AppendChild(modelBehaviour);
                    activityDiagram = document.CreateElement("UML:ActivityGraph", umlNamespace);
                    activityDiagram.SetAttribute("xmi.idref", diagram.Id);
                    modelBehaviour.AppendChild(activityDiagram);
                }
            }
            #endregion
            #region XMI.content/UML:Namespace.ownedElement

            XmlElement namespaceOwned = document.CreateElement("UML:Namespace.ownedElement", umlNamespace);
            modelElement.AppendChild(namespaceOwned);

            XmlElement stateMachineTop = null;
            XmlElement compositeState = null;
            XmlElement compositeSubvertex = null;
            XmlElement stateNode = null;
            XmlElement umlModelElement = null;
            XmlElement umlTag = null;
            XmlElement umlTaggedValueModelElement = null;
            XmlElement mModelElement = null;
            XmlElement vertexContainer = null;
            XmlElement vertexContainerComposite = null;
            XmlElement vertexOutgoing = null;
            XmlElement outgoingTransition = null;
            XmlElement vertexIncoming = null;

            foreach (UmlDiagram diagram in model.Diagrams)
            {
                #region Activity Diagram
                if (diagram is UmlActivityDiagram)
                {
                    #region UML:ActivityGraph
                    activityDiagram = document.CreateElement("UML:ActivityGraph", umlNamespace);
                    activityDiagram.SetAttribute("xmi.id", diagram.Id);
                    activityDiagram.SetAttribute("name", diagram.Name);
                    activityDiagram.SetAttribute("version", "0");
                    activityDiagram.SetAttribute("unSolvedFlag", "false");
                    namespaceOwned.AppendChild(activityDiagram);
                    #endregion
                    #region UML:StateMachine.top

                    stateMachineTop = document.CreateElement("UML:StateMachine.top", umlNamespace);
                    activityDiagram.AppendChild(stateMachineTop);

                    compositeState = document.CreateElement("UML:CompositeState", umlNamespace);
                    compositeState.SetAttribute("xmi.id", Guid.NewGuid().ToString());
                    compositeState.SetAttribute("name", "");
                    compositeState.SetAttribute("version", "0");
                    compositeState.SetAttribute("unSolvedFlag", "false");
                    compositeState.SetAttribute("isConcurrent", "true");
                    compositeState.SetAttribute("isOrthogonal", "false");
                    compositeState.SetAttribute("isRegion", "true");
                    stateMachineTop.AppendChild(compositeState);

                    compositeSubvertex = document.CreateElement("UML:CompositeState.subvertex", umlNamespace);
                    compositeState.AppendChild(compositeSubvertex);

                    foreach (UmlElement element in diagram.UmlObjects.OfType<UmlElement>())
                    {
                        if (element is UmlInitialState)
                        {
                            stateNode = document.CreateElement("UML:Pseudostate", umlNamespace);
                        }
                        else if (element is UmlFinalState)
                        {
                            stateNode = document.CreateElement("UML:FinalState", umlNamespace);
                        }
                        else if (element is UmlDecision)
                        {
                            stateNode = document.CreateElement("UML:Pseudostate", umlNamespace);
                        }
                        else if (element is UmlActionState)
                        {
                            stateNode = document.CreateElement("UML:ActionState", umlNamespace);
                        }

                        if (stateNode != null)
                        {
                            stateNode.SetAttribute("xmi.id", element.Id);
                            stateNode.SetAttribute("name", element.Name);
                            stateNode.SetAttribute("version", "0");
                            stateNode.SetAttribute("unSolvedFlag", "false");
                            if (element is UmlInitialState)
                            {
                                stateNode.SetAttribute("kind", "initial");
                            }
                            else if (element is UmlDecision)
                            {
                                stateNode.SetAttribute("kind", "junction");
                            }
                            else if (element is UmlActionState)
                            {
                                stateNode.SetAttribute("isDynamic", "false");
                            }
                            compositeSubvertex.AppendChild(stateNode);

                            foreach (KeyValuePair<String, String> pair in element.TaggedValues)
                            {
                                umlModelElement = document.CreateElement("UML:ModelElement.taggedValue", umlNamespace);
                                stateNode.AppendChild(umlModelElement);
                                umlTag = document.CreateElement("UML:TaggedValue", umlNamespace);
                                umlTag.SetAttribute("xmi.id", Guid.NewGuid().ToString());
                                umlTag.SetAttribute("version", "0");
                                umlTag.SetAttribute("tag", pair.Key);
                                umlTag.SetAttribute("value", pair.Value);
                                umlTaggedValueModelElement = document.CreateElement("UML:TaggedValue.modelElement", umlNamespace);
                                mModelElement = document.CreateElement("UML:ModelElement", umlNamespace);
                                mModelElement.SetAttribute("xmi.idref", element.Id);
                                umlTaggedValueModelElement.AppendChild(mModelElement);
                                umlTag.AppendChild(umlTaggedValueModelElement);
                                umlModelElement.AppendChild(umlTag);
                            }

                            vertexContainer = document.CreateElement("UML:StateVertex.container", umlNamespace);
                            stateNode.AppendChild(vertexContainer);

                            vertexContainerComposite = document.CreateElement("UML:CompositeState", umlNamespace);
                            vertexContainerComposite.SetAttribute("xmi.idref", compositeState.Attributes["xmi.id"].Value);
                            vertexContainer.AppendChild(vertexContainerComposite);

                            #region outgoing transitions
                            if ((element is UmlInitialState || element is UmlActionState || element is UmlDecision) && !(element is UmlFinalState))
                            {
                                foreach (UmlTransition transition in diagram.UmlObjects.OfType<UmlTransition>())
                                {
                                    if (transition.End1.Equals(element))
                                    {
                                        vertexOutgoing = document.CreateElement("UML:StateVertex.outgoing", umlNamespace);
                                        stateNode.AppendChild(vertexOutgoing);
                                        outgoingTransition = document.CreateElement("UML:Transition", umlNamespace);
                                        outgoingTransition.SetAttribute("xmi.idref", transition.Id);
                                        vertexOutgoing.AppendChild(outgoingTransition);
                                    }
                                }
                            }
                            #endregion
                            #region incoming transition
                            if ((element is UmlFinalState || element is UmlActionState || element is UmlDecision) && !(element is UmlInitialState))
                            {
                                foreach (UmlTransition transition in diagram.UmlObjects.OfType<UmlTransition>())
                                {
                                    if (transition.End2.Equals(element))
                                    {
                                        vertexIncoming = document.CreateElement("UML:StateVertex.incoming", umlNamespace);
                                        stateNode.AppendChild(vertexIncoming);
                                        XmlElement incomingTransition = document.CreateElement("UML:Transition", umlNamespace);
                                        incomingTransition.SetAttribute("xmi.idref", transition.Id);
                                        vertexIncoming.AppendChild(incomingTransition);
                                    }
                                }

                                if (element is UmlActionState && !(element is UmlDecision))
                                {
                                    XmlElement stateEntry = document.CreateElement("UML:State.entry", umlNamespace);
                                    stateNode.AppendChild(stateEntry);

                                    XmlElement entryAction = document.CreateElement("UML:Action", umlNamespace);
                                    entryAction.SetAttribute("xmi.id", Guid.NewGuid().ToString());
                                    entryAction.SetAttribute("name", element.Name);
                                    entryAction.SetAttribute("version", "0");
                                    entryAction.SetAttribute("unSolvedFlag", "false");
                                    entryAction.SetAttribute("isAsynchronous", "false");
                                    entryAction.SetAttribute("actionType", "5");
                                    stateEntry.AppendChild(entryAction);
                                }
                            }
                            #endregion
                        }
                    }

                    #endregion
                    #region UML:StateMachine.context

                    XmlElement stateMachineContext = document.CreateElement("UML:StateMachine.context", umlNamespace);
                    activityDiagram.AppendChild(stateMachineContext);

                    XmlElement contextModel = document.CreateElement("UML:ModelElement", umlNamespace);
                    contextModel.SetAttribute("xmi.idref", model.Id);
                    stateMachineContext.AppendChild(contextModel);
                    #endregion
                    #region UML:StateMachine.transitions

                    XmlElement stateMachineTransitions = document.CreateElement("UML:StateMachine.transitions", umlNamespace);
                    activityDiagram.AppendChild(stateMachineTransitions);

                    foreach (UmlTransition transition in diagram.UmlObjects.OfType<UmlTransition>())
                    {
                        XmlElement transitionNode = document.CreateElement("UML:Transition", umlNamespace);
                        transitionNode.SetAttribute("xmi.id", transition.Id);
                        transitionNode.SetAttribute("name", transition.Name);
                        transitionNode.SetAttribute("version", "0");
                        transitionNode.SetAttribute("unSolvedFlag", "false");
                        stateMachineTransitions.AppendChild(transitionNode);

                        foreach (KeyValuePair<String, String> pair in transition.TaggedValues)
                        {
                            umlModelElement = document.CreateElement("UML:ModelElement.taggedValue", umlNamespace);
                            transitionNode.AppendChild(umlModelElement);
                            umlTag = document.CreateElement("UML:TaggedValue", umlNamespace);
                            umlTag.SetAttribute("xmi.id", Guid.NewGuid().ToString());
                            umlTag.SetAttribute("version", "0");
                            umlTag.SetAttribute("tag", pair.Key);
                            umlTag.SetAttribute("value", pair.Value);
                            umlTaggedValueModelElement = document.CreateElement("UML:TaggedValue.modelElement", umlNamespace);
                            mModelElement = document.CreateElement("UML:ModelElement", umlNamespace);
                            mModelElement.SetAttribute("xmi.idref", transition.Id);
                            umlTaggedValueModelElement.AppendChild(mModelElement);
                            umlTag.AppendChild(umlTaggedValueModelElement);
                            umlModelElement.AppendChild(umlTag);
                        }


                        XmlElement transitionSource = document.CreateElement("UML:Transition.source", umlNamespace);
                        transitionNode.AppendChild(transitionSource);

                        XmlElement source = document.CreateElement("UML:StateVertex", umlNamespace);
                        source.SetAttribute("xmi.idref", transition.End1.Id);
                        transitionSource.AppendChild(source);

                        XmlElement transitionTarget = document.CreateElement("UML:Transition.target", umlNamespace);
                        transitionNode.AppendChild(transitionTarget);

                        XmlElement target = document.CreateElement("UML:StateVertex", umlNamespace);
                        target.SetAttribute("xmi.idref", transition.End2.Id);
                        transitionTarget.AppendChild(target);

                        XmlElement transitionStateMachine = document.CreateElement("UML:Transition.stateMachine", umlNamespace);
                        transitionNode.AppendChild(transitionStateMachine);

                        XmlElement stateMachine = document.CreateElement("UML:StateMachine", umlNamespace);
                        stateMachine.SetAttribute("xmi.idref", diagram.Id);
                        transitionStateMachine.AppendChild(stateMachine);
                    }
                    #endregion
                    #region UML:ActivityGraph.partition
                    XmlElement partitionSubGroup = null;

                    if ((diagram as UmlActivityDiagram).Lanes.Count() != 0)
                    {
                        XmlElement activityGraphPartition = document.CreateElement("UML:ActivityGraph.partition", umlNamespace);
                        activityDiagram.AppendChild(activityGraphPartition);
                        #region Attributes
                        XmlElement dimension = document.CreateElement("UML:Partition", umlNamespace);
                        dimension.SetAttribute("xmi.id", "laneDimension.Id");
                        dimension.SetAttribute("name", "Dimension0");
                        dimension.SetAttribute("version", "0");
                        dimension.SetAttribute("unSolvedFlag", "false");
                        dimension.SetAttribute("isDimension", "false");
                        dimension.SetAttribute("isExternal", "false");
                        activityGraphPartition.AppendChild(dimension);
                        #endregion
                        XmlElement partitionActivityGraph = document.CreateElement("UML:Partition.activityGraph", umlNamespace);
                        dimension.AppendChild(partitionActivityGraph);

                        XmlElement activityGraph = document.CreateElement("UML:ActivityGraph", umlNamespace);
                        activityGraph.SetAttribute("xmi.idref", diagram.Id);
                        partitionActivityGraph.AppendChild(activityGraph);

                        partitionSubGroup = document.CreateElement("UML:Partition.subGroup", umlNamespace);
                        dimension.AppendChild(partitionSubGroup);
                    }
                    foreach (UmlLane lane in (diagram as UmlActivityDiagram).Lanes)
                    {
                        XmlElement judeModelElement = document.CreateElement("JUDE:ModelElement", judeNamespace);

                        judeModelElement.SetAttribute("xmi.idref", lane.Id);
                        judeModelElement.SetAttribute("xmlns:JUDE", judeNamespace);

                        partitionSubGroup.AppendChild(judeModelElement);
                    }
                    #endregion
                }
                #endregion
                #region Lane
                if ((diagram is UmlActivityDiagram) && ((diagram as UmlActivityDiagram).Lanes.Count != 0))
                {
                    foreach (UmlLane lane in (diagram as UmlActivityDiagram).Lanes)
                    {
                        XmlElement umlPartition = document.CreateElement("UML:Partition", umlNamespace);
                        #region Attributes
                        umlPartition.SetAttribute("xmi.id", lane.Id);
                        if (lane.Name == "")
                        {
                            umlPartition.SetAttribute("name", "+++");
                        }
                        else
                        {
                            umlPartition.SetAttribute("name", lane.Name);
                        }
                        umlPartition.SetAttribute("version", "0");
                        umlPartition.SetAttribute("unSolvedFlag", "false");
                        umlPartition.SetAttribute("isDimension", "false");
                        umlPartition.SetAttribute("isExternal", "false");
                        content.AppendChild(umlPartition);
                        #endregion
                        XmlElement partitionContents = document.CreateElement("UML:Partition.contents", umlNamespace);
                        umlPartition.AppendChild(partitionContents);

                        foreach (UmlElement element in lane.GetElements())
                        {
                            XmlElement judeModelElement = document.CreateElement("JUDE:ModelElement", judeNamespace);
                            judeModelElement.SetAttribute("xmi.idref", element.Id);
                            judeModelElement.SetAttribute("xmlns:JUDE", judeNamespace);
                            partitionContents.AppendChild(judeModelElement);
                        }

                        XmlElement partitionActivityGraph = document.CreateElement("UML:Partition.activityGraph", umlNamespace);
                        umlPartition.AppendChild(partitionActivityGraph);

                        XmlElement activityGraph = document.CreateElement("UML:ActivityGraph", umlNamespace);
                        activityGraph.SetAttribute("xmi.idref", diagram.Id);
                        partitionActivityGraph.AppendChild(activityGraph);

                        XmlElement partitionSuperPartition = document.CreateElement("UML:Partition.superPartition", umlNamespace);
                        umlPartition.AppendChild(partitionSuperPartition);

                        XmlElement partition = document.CreateElement("UML:Partition", umlNamespace);
                        partition.SetAttribute("xmi.idref", "laneDimension.Id");
                        partitionSuperPartition.AppendChild(partition);
                    }
                }
                #endregion
                #region UseCase Diagram
                if (diagram is UmlUseCaseDiagram)
                {
                    #region UML:Actor
                    foreach (UmlActor actor in diagram.UmlObjects.OfType<UmlActor>())
                    {
                        XmlElement umlActor = document.CreateElement("UML:Actor", umlNamespace);
                        umlActor.SetAttribute("xmi.id", actor.Id);
                        umlActor.SetAttribute("name", actor.Name);
                        umlActor.SetAttribute("version", "0");
                        umlActor.SetAttribute("unSolvedFlag", "false");
                        namespaceOwned.AppendChild(umlActor);

                        XmlElement modelElementNamespace = document.CreateElement("UML:ModelElement.namespace", umlNamespace);
                        umlActor.AppendChild(modelElementNamespace);

                        XmlElement uml_ns = document.CreateElement("UML:Namespace", umlNamespace);
                        uml_ns.SetAttribute("xmi.idref", model.Id);
                        modelElementNamespace.AppendChild(uml_ns);
                    }
                    #endregion
                    #region UML:UseCase
                    foreach (UmlUseCase useCase in diagram.UmlObjects.OfType<UmlUseCase>())
                    {
                        XmlElement umlUseCase = document.CreateElement("UML:UseCase", umlNamespace);
                        umlUseCase.SetAttribute("xmi.id", useCase.Id);
                        umlUseCase.SetAttribute("name", useCase.Name);
                        umlUseCase.SetAttribute("version", "0");
                        umlUseCase.SetAttribute("unSolvedFlag", "false");
                        namespaceOwned.AppendChild(umlUseCase);

                        XmlElement modelElementNamespace = document.CreateElement("UML:ModelElement.namespace", umlNamespace);
                        umlUseCase.AppendChild(modelElementNamespace);

                        XmlElement uml_ns = document.CreateElement("UML:Namespace", umlNamespace);
                        uml_ns.SetAttribute("xmi.idref", model.Id);
                        modelElementNamespace.AppendChild(uml_ns);
                    }
                    #endregion
                    #region UML:Association
                    foreach (UmlAssociation association in diagram.UmlObjects.OfType<UmlAssociation>())
                    {
                        XmlElement umlAssociation = document.CreateElement("UML:Association", umlNamespace);
                        umlAssociation.SetAttribute("xmi.id", association.Id);
                        umlAssociation.SetAttribute("name", association.Name);
                        umlAssociation.SetAttribute("version", "0");
                        umlAssociation.SetAttribute("unSolvedFlag", "false");
                        namespaceOwned.AppendChild(umlAssociation);

                        XmlElement modelElementNamespace = document.CreateElement("UML:ModelElement.namespace", umlNamespace);
                        umlAssociation.AppendChild(modelElementNamespace);

                        XmlElement uml_ns = document.CreateElement("UML:Namespace", umlNamespace);
                        uml_ns.SetAttribute("xmi.idref", model.Id);
                        modelElementNamespace.AppendChild(uml_ns);

                        XmlElement umlAssociationConnection = document.CreateElement("UML:Association.connection", umlNamespace);
                        umlAssociation.AppendChild(umlAssociationConnection);

                        #region Association End1
                        XmlElement umlAssociationEnd1 = document.CreateElement("UML:AssociationEnd", umlNamespace);
                        String idAssociationEnd1 = Guid.NewGuid().ToString();
                        umlAssociationEnd1.SetAttribute("xmi.id", idAssociationEnd1);
                        umlAssociationEnd1.SetAttribute("name", "");
                        umlAssociationEnd1.SetAttribute("version", "0");
                        umlAssociationEnd1.SetAttribute("unSolvedFlag", "false");
                        umlAssociationConnection.AppendChild(umlAssociationEnd1);

                        XmlElement modelElementNamespace2 = document.CreateElement("UML:ModelElement.namespace", umlNamespace);
                        umlAssociationEnd1.AppendChild(modelElementNamespace2);

                        XmlElement uml_ns2 = document.CreateElement("UML:Namespace", umlNamespace);
                        uml_ns2.SetAttribute("xmi.idref", model.Id);
                        modelElementNamespace2.AppendChild(uml_ns2);

                        XmlElement featureOwner = document.CreateElement("UML:Feature.owner", umlNamespace);
                        umlAssociationEnd1.AppendChild(featureOwner);

                        XmlElement umlClassifier = document.CreateElement("UML:Classifier", umlNamespace);
                        umlClassifier.SetAttribute("xmi.idref", association.End2.Id);
                        featureOwner.AppendChild(umlClassifier);

                        XmlElement associationEndParticipant = document.CreateElement("UML:AssociationEnd.participant", umlNamespace);
                        umlAssociationEnd1.AppendChild(associationEndParticipant);

                        XmlElement umlClassifier2 = document.CreateElement("UML:Classifier", umlNamespace);
                        umlClassifier2.SetAttribute("xmi.idref", association.End1.Id);
                        associationEndParticipant.AppendChild(umlClassifier2);

                        XmlElement associationEndAssociation = document.CreateElement("UML:AssociationEnd.association", umlNamespace);
                        umlAssociationEnd1.AppendChild(associationEndAssociation);

                        XmlElement umlAssociation2 = document.CreateElement("UML:Association", umlNamespace);
                        umlAssociation2.SetAttribute("xmi.idref", association.Id);
                        associationEndAssociation.AppendChild(umlAssociation2);
                        #endregion
                        #region Association End2
                        XmlElement umlAssociationEnd2 = document.CreateElement("UML:AssociationEnd", umlNamespace);
                        String idAssociationEnd2 = Guid.NewGuid().ToString();
                        umlAssociationEnd2.SetAttribute("xmi.id", idAssociationEnd2);
                        umlAssociationEnd2.SetAttribute("name", "");
                        umlAssociationEnd2.SetAttribute("version", "0");
                        umlAssociationEnd2.SetAttribute("unSolvedFlag", "false");
                        umlAssociationConnection.AppendChild(umlAssociationEnd2);

                        XmlElement modelElementNamespace3 = document.CreateElement("UML:ModelElement.namespace", umlNamespace);
                        umlAssociationEnd2.AppendChild(modelElementNamespace3);

                        XmlElement uml_ns3 = document.CreateElement("UML:Namespace", umlNamespace);
                        uml_ns3.SetAttribute("xmi.idref", model.Id);
                        modelElementNamespace3.AppendChild(uml_ns3);

                        XmlElement featureOwner2 = document.CreateElement("UML:Feature.owner", umlNamespace);
                        umlAssociationEnd2.AppendChild(featureOwner2);

                        XmlElement umlClassifier3 = document.CreateElement("UML:Classifier", umlNamespace);
                        umlClassifier3.SetAttribute("xmi.idref", association.End1.Id);
                        featureOwner2.AppendChild(umlClassifier3);

                        XmlElement associationEndParticipant2 = document.CreateElement("UML:AssociationEnd.participant", umlNamespace);
                        umlAssociationEnd2.AppendChild(associationEndParticipant2);

                        XmlElement umlClassifier4 = document.CreateElement("UML:Classifier", umlNamespace);
                        umlClassifier4.SetAttribute("xmi.idref", association.End2.Id);
                        associationEndParticipant2.AppendChild(umlClassifier4);

                        XmlElement associationEndAssociation2 = document.CreateElement("UML:AssociationEnd.association", umlNamespace);
                        umlAssociationEnd2.AppendChild(associationEndAssociation2);

                        XmlElement umlAssociation3 = document.CreateElement("UML:Association", umlNamespace);
                        umlAssociation3.SetAttribute("xmi.idref", association.Id);
                        associationEndAssociation2.AppendChild(umlAssociation3);
                        #endregion

                        XmlElement umlAssociationEnd3 = document.CreateElement("UML:AssociationEnd", umlNamespace);
                        umlAssociationEnd3.SetAttribute("xmi.idref", idAssociationEnd1);
                        namespaceOwned.AppendChild(umlAssociationEnd3);

                        XmlElement umlAssociationEnd4 = document.CreateElement("UML:AssociationEnd", umlNamespace);
                        umlAssociationEnd4.SetAttribute("xmi.idref", idAssociationEnd2);
                        namespaceOwned.AppendChild(umlAssociationEnd4);
                    }
                    #endregion
                    #region Stereotype
                    foreach (UmlActor actor in diagram.UmlObjects.OfType<UmlActor>())
                    {
                        XmlElement umlStereotype = document.CreateElement("UML:Stereotype", umlNamespace);
                        umlStereotype.SetAttribute("xmi.id", Guid.NewGuid().ToString());
                        umlStereotype.SetAttribute("name", "actor");
                        umlStereotype.SetAttribute("version", "0");
                        umlStereotype.SetAttribute("unSolvedFlag", "false");
                        umlStereotype.SetAttribute("isRoot", "false");
                        umlStereotype.SetAttribute("isLeaf", "false");
                        umlStereotype.SetAttribute("isAbstract", "false");
                        namespaceOwned.AppendChild(umlStereotype);

                        XmlElement umlStereotypeBaseClass = document.CreateElement("UML:Stereotype.baseClass", umlNamespace);
                        umlStereotypeBaseClass.InnerText = "Actor";
                        umlStereotype.AppendChild(umlStereotypeBaseClass);

                        XmlElement umlStereotypeExtendedElement = document.CreateElement("UML:Stereotype.extendedElement", umlNamespace);
                        umlStereotype.AppendChild(umlStereotypeExtendedElement);

                        //TODO: Pegar o ID corretamente
                        XmlElement judeModelElement = document.CreateElement("JUDE:ModelElement", judeNamespace);
                        judeModelElement.SetAttribute("xmi.idref", actor.Id);
                        judeModelElement.SetAttribute("xmlns:JUDE", judeNamespace);
                        umlStereotypeExtendedElement.AppendChild(judeModelElement);
                    }
                    #endregion
                }
                #endregion
            }
            #endregion
            #region XMI.extension

            XmlElement extensionNode = document.CreateElement("XMI.extension");
            content.AppendChild(extensionNode);
            #region Activity Diagram
            foreach (UmlActivityDiagram diagram in model.Diagrams.OfType<UmlActivityDiagram>())
            {
                //JUDE:ActivityDiagram
                XmlElement judeActivityDiagram = document.CreateElement("JUDE:ActivityDiagram", judeNamespace);
                #region Attributes
                String idDiagramJUDE = Guid.NewGuid().ToString();
                judeActivityDiagram.SetAttribute("xmi.id", idDiagramJUDE);
                judeActivityDiagram.SetAttribute("name", diagram.Name);
                judeActivityDiagram.SetAttribute("typeInfo", "Activity Diagram");
                judeActivityDiagram.SetAttribute("version", "0");
                judeActivityDiagram.SetAttribute("verticalMaxLevel", "-1");
                judeActivityDiagram.SetAttribute("horizontalMaxLevel", "-1");
                judeActivityDiagram.SetAttribute("xmlns:JUDE", "http://objectclub.esm.co.jp/Jude/namespace/");
                extensionNode.AppendChild(judeActivityDiagram);
                #endregion

                //JUDE:StateChartDiagram.semanticModel
                XmlElement semanticModel = document.CreateElement("JUDE:StateChartDiagram.semanticModel", judeNamespace);
                judeActivityDiagram.AppendChild(semanticModel);

                XmlElement activityGraph = document.CreateElement("UML:ActivityGraph", umlNamespace);
                activityGraph.SetAttribute("xmi.idref", diagram.Id);
                semanticModel.AppendChild(activityGraph);

                //JUDE:Diagram.presentations
                XmlElement presentations = document.CreateElement("JUDE:Diagram.presentations", judeNamespace);
                judeActivityDiagram.AppendChild(presentations);

                if ((diagram as UmlActivityDiagram).Lanes.Count() != 0)
                {
                    //Lane Dimension begin
                    XmlElement swimlanePresentation = document.CreateElement("JUDE:SwimlanePresentation", judeNamespace);
                    #region Attributes
                    swimlanePresentation.SetAttribute("xmi.id", Guid.NewGuid().ToString());
                    swimlanePresentation.SetAttribute("version", "0");
                    swimlanePresentation.SetAttribute("depth", "0");
                    swimlanePresentation.SetAttribute("stereotypeVisibility", "true");
                    swimlanePresentation.SetAttribute("constraintVisibility", "true");
                    swimlanePresentation.SetAttribute("notationType", "0");
                    swimlanePresentation.SetAttribute("width", "600.0");
                    swimlanePresentation.SetAttribute("height", "2000.0");
                    swimlanePresentation.SetAttribute("doAutoResize", "true");
                    swimlanePresentation.SetAttribute("visibility", "true");
                    swimlanePresentation.SetAttribute("label", "Dimension0");
                    swimlanePresentation.SetAttribute("isHorizontal", "false");
                    swimlanePresentation.SetAttribute("nameBlockHeight", "60.0");
                    presentations.AppendChild(swimlanePresentation);
                    #endregion

                    XmlElement uPresentationSemanticModel = document.CreateElement("JUDE:UPresentation.semanticModel", judeNamespace);
                    swimlanePresentation.AppendChild(uPresentationSemanticModel);

                    XmlElement partition = document.CreateElement("UML:Partition", umlNamespace);
                    partition.SetAttribute("xmi.idref", "laneDimension.Id");
                    uPresentationSemanticModel.AppendChild(partition);

                    XmlElement uPresentationDiagram = document.CreateElement("JUDE:UPresentation.diagram", judeNamespace);
                    swimlanePresentation.AppendChild(uPresentationDiagram);

                    XmlElement judeDiagram = document.CreateElement("JUDE:Diagram", judeNamespace);
                    judeDiagram.SetAttribute("xmi.idref", idDiagramJUDE);
                    uPresentationDiagram.AppendChild(judeDiagram);

                    XmlElement uPresentationCustomStyleMap = document.CreateElement("JUDE:UPresentation.customStyleMap", judeNamespace);
                    swimlanePresentation.AppendChild(uPresentationCustomStyleMap);

                    XmlElement uPresentationStyleProperty = document.CreateElement("JUDE:UPresentation.styleProperty", judeNamespace);
                    uPresentationStyleProperty.SetAttribute("key", "fill.color");
                    uPresentationStyleProperty.SetAttribute("value", "#FFFFCC");
                    uPresentationCustomStyleMap.AppendChild(uPresentationStyleProperty);

                    XmlElement jomtPresentationLocation1 = document.CreateElement("JUDE:JomtPresentation.location", judeNamespace);
                    swimlanePresentation.AppendChild(jomtPresentationLocation1);

                    XmlElement xmiField1 = document.CreateElement("XMI.field");

                    xmiField1.InnerText = contLane + "";
                    jomtPresentationLocation1.AppendChild(xmiField1);

                    xmiField1 = document.CreateElement("XMI.field");
                    xmiField1.InnerText = 40 + "";
                    jomtPresentationLocation1.AppendChild(xmiField1);
                }

                int j = (diagram.UmlObjects.OfType<UmlActionState>().Count() - diagram.Lanes.Count()) * 200;
                IEnumerable<UmlLane> lanes = (diagram as UmlActivityDiagram).Lanes;
                for (int i = 0; i < lanes.Count(); i++)
                {
                    UmlLane lane = lanes.ElementAt(i);

                    XmlElement swimlanePresentation = document.CreateElement("JUDE:SwimlanePresentation", judeNamespace);
                    #region Attributes
                    swimlanePresentation.SetAttribute("xmi.id", Guid.NewGuid().ToString());
                    swimlanePresentation.SetAttribute("version", "0");
                    swimlanePresentation.SetAttribute("depth", "0");
                    swimlanePresentation.SetAttribute("stereotypeVisibility", "true");
                    swimlanePresentation.SetAttribute("constraintVisibility", "true");
                    swimlanePresentation.SetAttribute("notationType", "0");
                    swimlanePresentation.SetAttribute("width", "300.0");
                    swimlanePresentation.SetAttribute("height", j.ToString());
                    swimlanePresentation.SetAttribute("doAutoResize", "true");
                    swimlanePresentation.SetAttribute("visibility", "true");
                    swimlanePresentation.SetAttribute("label", "Partition0");
                    swimlanePresentation.SetAttribute("isHorizontal", "false");
                    swimlanePresentation.SetAttribute("nameBlockHeight", "60.0");
                    presentations.AppendChild(swimlanePresentation);
                    #endregion

                    XmlElement uPresentationSemanticModel = document.CreateElement("JUDE:UPresentation.semanticModel", judeNamespace);
                    swimlanePresentation.AppendChild(uPresentationSemanticModel);

                    XmlElement partition = document.CreateElement("UML:Partition", umlNamespace);
                    partition.SetAttribute("xmi.idref", lane.Id);
                    uPresentationSemanticModel.AppendChild(partition);

                    XmlElement uPresentationDiagram = document.CreateElement("JUDE:UPresentation.diagram", judeNamespace);
                    swimlanePresentation.AppendChild(uPresentationDiagram);

                    XmlElement judeDiagram = document.CreateElement("JUDE:Diagram", judeNamespace);
                    judeDiagram.SetAttribute("xmi.idref", idDiagramJUDE);
                    uPresentationDiagram.AppendChild(judeDiagram);

                    XmlElement uPresentationCustomStyleMap = document.CreateElement("JUDE:UPresentation.customStyleMap", judeNamespace);
                    swimlanePresentation.AppendChild(uPresentationCustomStyleMap);

                    XmlElement uPresentationStyleProperty = document.CreateElement("JUDE:UPresentation.styleProperty", judeNamespace);
                    uPresentationStyleProperty.SetAttribute("key", "fill.color");
                    uPresentationStyleProperty.SetAttribute("value", "#FFFFCC");
                    uPresentationCustomStyleMap.AppendChild(uPresentationStyleProperty);

                    XmlElement jomtPresentationLocation1 = document.CreateElement("JUDE:JomtPresentation.location", judeNamespace);
                    swimlanePresentation.AppendChild(jomtPresentationLocation1);

                    XmlElement xmiField1 = document.CreateElement("XMI.field");

                    contLane += 300;
                    xmiField1.InnerText = contLane + "";
                    jomtPresentationLocation1.AppendChild(xmiField1);

                    xmiField1 = document.CreateElement("XMI.field");
                    xmiField1.InnerText = 40 + "";
                    jomtPresentationLocation1.AppendChild(xmiField1);

                    if ((lanes.Count() > 1))
                    {
                        if ((i != 0) && (i != (lanes.Count() - 1)))
                        {
                            XmlElement swimlanePresentationRightLane = document.CreateElement("JUDE:SwimlanePresentation.rightLane", judeNamespace);
                            swimlanePresentation.AppendChild(swimlanePresentationRightLane);

                            XmlElement swimlanePresentation1 = document.CreateElement("JUDE:SwimlanePresentation", judeNamespace);
                            swimlanePresentation1.SetAttribute("xmi.idref", lanes.ElementAt(i + 1).Id);
                            swimlanePresentationRightLane.AppendChild(swimlanePresentation1);

                            XmlElement swimLanePresentationLeftLane = document.CreateElement("JUDE:SwimlanePresentation.leftLane", judeNamespace);
                            swimlanePresentation.AppendChild(swimLanePresentationLeftLane);

                            XmlElement swimLanePresentation2 = document.CreateElement("JUDE:SwimlanePresentation", judeNamespace);
                            swimLanePresentation2.SetAttribute("xmi.idref", lanes.ElementAt(i - 1).Id);
                            swimLanePresentationLeftLane.AppendChild(swimLanePresentation2);
                        }
                        else if (i == 0)
                        {
                            XmlElement swimlanePresentationRightLane = document.CreateElement("JUDE:SwimlanePresentation.rightLane", judeNamespace);
                            swimlanePresentation.AppendChild(swimlanePresentationRightLane);

                            XmlElement swimlanePresentation1 = document.CreateElement("JUDE:SwimlanePresentation", judeNamespace);
                            swimlanePresentation1.SetAttribute("xmi.idref", lanes.ElementAt(i + 1).Id);
                            swimlanePresentationRightLane.AppendChild(swimlanePresentation1);
                        }
                        else if (i == (lanes.Count() - 1))
                        {
                            XmlElement swimLanePresentationLeftLane = document.CreateElement("JUDE:SwimlanePresentation.leftLane", judeNamespace);
                            swimlanePresentation.AppendChild(swimLanePresentationLeftLane);

                            XmlElement swimLanePresentation2 = document.CreateElement("JUDE:SwimlanePresentation", judeNamespace);
                            swimLanePresentation2.SetAttribute("xmi.idref", lanes.ElementAt(i - 1).Id);
                            swimLanePresentationLeftLane.AppendChild(swimLanePresentation2);
                        }
                    }
                }

                XmlElement framePresentation = document.CreateElement("JUDE:FramePresentation", judeNamespace);
                #region Attributes
                framePresentation.SetAttribute("xmi.id", Guid.NewGuid().ToString());
                framePresentation.SetAttribute("version", "0");
                framePresentation.SetAttribute("depth", "2147483646");
                framePresentation.SetAttribute("stereotypeVisibility", "true");
                framePresentation.SetAttribute("constraintVisibility", "true");
                framePresentation.SetAttribute("notationType", "0");
                framePresentation.SetAttribute("width", "5000.0");
                framePresentation.SetAttribute("height", "5000.0");
                framePresentation.SetAttribute("doAutoResize", "true");
                framePresentation.SetAttribute("visibility", "true");
                framePresentation.SetAttribute("label", diagram.Name);
                presentations.AppendChild(framePresentation);
                #endregion

                XmlElement uPresentation = document.CreateElement("JUDE:UPresentation.diagram", judeNamespace);
                framePresentation.AppendChild(uPresentation);

                XmlElement uDiagram = document.CreateElement("JUDE:Diagram", judeNamespace);
                uDiagram.SetAttribute("xmi.idref", idDiagramJUDE);
                uPresentation.AppendChild(uDiagram);

                XmlElement jomtPresentationLocation = document.CreateElement("JUDE:JomtPresentation.location", judeNamespace);
                framePresentation.AppendChild(jomtPresentationLocation);

                XmlElement xmiField = document.CreateElement("XMI.field");
                xmiField.InnerText = "0";
                jomtPresentationLocation.AppendChild(xmiField);

                xmiField = document.CreateElement("XMI.field");
                xmiField.InnerText = "0";
                jomtPresentationLocation.AppendChild(xmiField);

                UmlLane currentLane = null;
                foreach (UmlActionState elem in diagram.UmlObjects.OfType<UmlActionState>().OrderBy(h => h.ParentLane))
                {
                    #region Attributes
                    if (elem is UmlInitialState)
                    {
                        itemPresentation = document.CreateElement("JUDE:InitialStatePresentation", judeNamespace);
                    }
                    else if (elem is UmlFinalState)
                    {
                        itemPresentation = document.CreateElement("JUDE:FinalStatePresentation", judeNamespace);
                    }
                    else if (elem is UmlDecision)
                    {
                        itemPresentation = document.CreateElement("JUDE:MergePresentation", judeNamespace);
                    }
                    else if (elem is UmlActionState)
                    {
                        itemPresentation = document.CreateElement("JUDE:ActionStatePresentation", judeNamespace);
                    }

                    if (itemPresentation == null)
                    {
                        continue;
                    }

                    itemPresentation.SetAttribute("xmi.id", elem.Id + "p");
                    itemPresentation.SetAttribute("version", "0");
                    itemPresentation.SetAttribute("depth", (TestPlanToAstah.depth--).ToString());
                    itemPresentation.SetAttribute("stereotypeVisibility", "true");
                    itemPresentation.SetAttribute("constraintVisibility", "true");
                    itemPresentation.SetAttribute("notationType", "0");
                    itemPresentation.SetAttribute("visibility", "true");

                    if (elem is UmlInitialState || elem is UmlFinalState)
                    {
                        itemPresentation.SetAttribute("width", "20.0");
                        itemPresentation.SetAttribute("height", "20.0");
                        itemPresentation.SetAttribute("doAutoResize", "false");
                        itemPresentation.SetAttribute("label", elem.Name);
                    }
                    else if (elem is UmlDecision)
                    {
                        itemPresentation.SetAttribute("width", "30.0");
                        itemPresentation.SetAttribute("height", "20.0");
                        itemPresentation.SetAttribute("doAutoResize", "false");
                        itemPresentation.SetAttribute("label", elem.Name);
                    }
                    else if (elem is UmlActionState)
                    {
                        itemPresentation.SetAttribute("width", "150.0");
                        itemPresentation.SetAttribute("height", "100.0");
                        itemPresentation.SetAttribute("doAutoResize", "false");
                        itemPresentation.SetAttribute("label", elem.Name);
                    }

                    if (elem is UmlActionState || elem is UmlFinalState && !(elem is UmlDecision))
                    {
                        itemPresentation.SetAttribute("allActionVisibility", "true");

                        if (elem is UmlActionState)
                        {
                            itemPresentation.SetAttribute("signalReverse", "false");
                        }
                    }
                    #endregion
                    #region Semantic Model
                    XmlElement uSemanticModelE = document.CreateElement("JUDE:UPresentation.semanticModel", judeNamespace);
                    itemPresentation.AppendChild(uSemanticModelE);

                    XmlElement uSemanticModelInnerE = null;
                    if (elem is UmlInitialState || elem is UmlDecision)
                    {
                        uSemanticModelInnerE = document.CreateElement("UML:Pseudostate", umlNamespace);
                        uSemanticModelInnerE.SetAttribute("xmi.idref", elem.Id);
                    }
                    else if (elem is UmlFinalState)
                    {
                        uSemanticModelInnerE = document.CreateElement("UML:FinalState", umlNamespace);
                        uSemanticModelInnerE.SetAttribute("xmi.idref", elem.Id);
                    }
                    else if (elem is UmlActionState)
                    {
                        uSemanticModelInnerE = document.CreateElement("UML:ActionState", umlNamespace);
                        uSemanticModelInnerE.SetAttribute("xmi.idref", elem.Id);
                    }

                    if (uSemanticModelInnerE != null)
                    {
                        uSemanticModelE.AppendChild(uSemanticModelInnerE);
                    }
                    #endregion
                    #region Presentation Diagram
                    XmlElement presentationDiagramE = document.CreateElement("JUDE:UPresentation.diagram", judeNamespace);
                    XmlElement presentationDiagramInnerE = document.CreateElement("JUDE:Diagram", judeNamespace);

                    presentationDiagramInnerE.SetAttribute("xmi.idref", judeActivityDiagram.Attributes["xmi.id"].Value);
                    presentationDiagramE.AppendChild(presentationDiagramInnerE);
                    itemPresentation.AppendChild(presentationDiagramE);
                    #endregion
                    #region Server / Client
                    XmlElement clients = document.CreateElement("JUDE:UPresentation.clients", judeNamespace);
                    itemPresentation.AppendChild(clients);

                    foreach (UmlTransition client in diagram.UmlObjects.OfType<UmlTransition>())
                    {
                        if (client.End1 == elem || client.End2 == elem)
                        {
                            XmlElement clientNode = document.CreateElement("JUDE:TransitionPresentation", judeNamespace);
                            clients.AppendChild(clientNode);
                            clientNode.SetAttribute("xmi.idref", client.Id + "p");
                        }
                    }
                    #endregion

                    XmlElement jomtPresentation = document.CreateElement("JUDE:JomtPresentation.location", judeNamespace);
                    itemPresentation.AppendChild(jomtPresentation);

                    if (elem.ParentLane != currentLane)
                    {
                        currentLane = elem.ParentLane;
                        x += 300;
                        y -= 150;
                    }

                    xmiField = document.CreateElement("XMI.field");
                    if (elem is UmlInitialState || elem is UmlFinalState)
                    {
                        xmiField.InnerText = (x + 65) + "";
                    }
                    else
                    {
                        xmiField.InnerText = x + "";
                    }
                    jomtPresentation.AppendChild(xmiField);

                    y += 150;

                    xmiField = document.CreateElement("XMI.field");
                    xmiField.InnerText = y + "";
                    jomtPresentation.AppendChild(xmiField);

                    if (itemPresentation != null)
                    {
                        presentations.AppendChild(itemPresentation);
                    }
                }
                foreach (UmlTransition tran in diagram.UmlObjects.OfType<UmlTransition>())
                {
                    #region Attributes
                    itemPresentation = document.CreateElement("JUDE:TransitionPresentation", judeNamespace);

                    if (itemPresentation == null)
                    {
                        continue;
                    }

                    itemPresentation.SetAttribute("xmi.id", tran.Id + "p");
                    itemPresentation.SetAttribute("version", "0");
                    itemPresentation.SetAttribute("depth", (TestPlanToAstah.depth--).ToString());
                    itemPresentation.SetAttribute("stereotypeVisibility", "true");
                    itemPresentation.SetAttribute("constraintVisibility", "true");
                    itemPresentation.SetAttribute("notationType", "0");
                    itemPresentation.SetAttribute("visibility", "true");
                    itemPresentation.SetAttribute("rightAngle", "false");
                    #endregion
                    #region Semantic Model
                    XmlElement uSemanticModelT = document.CreateElement("JUDE:UPresentation.semanticModel", judeNamespace);
                    itemPresentation.AppendChild(uSemanticModelT);

                    XmlElement uSemanticModelInnerT = null;

                    uSemanticModelInnerT = document.CreateElement("UML:Transition", umlNamespace);
                    uSemanticModelInnerT.SetAttribute("xmi.idref", tran.Id);

                    if (uSemanticModelInnerT != null)
                    {
                        uSemanticModelT.AppendChild(uSemanticModelInnerT);
                    }
                    #endregion
                    #region Presentation Diagram
                    XmlElement presentationDiagramT = document.CreateElement("JUDE:UPresentation.diagram", judeNamespace);
                    XmlElement presentationDiagramInnerT = document.CreateElement("JUDE:Diagram", judeNamespace);

                    presentationDiagramInnerT.SetAttribute("xmi.idref", judeActivityDiagram.Attributes["xmi.id"].Value);
                    presentationDiagramT.AppendChild(presentationDiagramInnerT);
                    itemPresentation.AppendChild(presentationDiagramT);
                    #endregion
                    #region Server / Client
                    XmlElement servers = document.CreateElement("JUDE:UPresentation.servers", judeNamespace);
                    itemPresentation.AppendChild(servers);

                    UmlTransition t = (UmlTransition)tran;

                    XmlElement sourceNode = null;

                    if (t.End1 is UmlFinalState)
                    {
                        sourceNode = document.CreateElement("JUDE:FinalStatePresentation", judeNamespace);
                    }
                    else if (t.End1 is UmlInitialState)
                    {
                        sourceNode = document.CreateElement("JUDE:InitialStatePresentation", judeNamespace);
                    }
                    else if (t.End1 is UmlDecision)
                    {
                        sourceNode = document.CreateElement("JUDE:MergePresentation", judeNamespace);
                    }
                    else if (t.End1 is UmlActionState)
                    {
                        sourceNode = document.CreateElement("JUDE:ActionStatePresentation", judeNamespace);
                    }

                    if (sourceNode != null)
                    {
                        sourceNode.SetAttribute("xmi.idref", t.End1.Id + "p");
                        servers.AppendChild(sourceNode);
                    }

                    XmlElement targetNode = null;

                    if (t.End2 is UmlFinalState)
                    {
                        targetNode = document.CreateElement("JUDE:FinalStatePresentation", judeNamespace);
                    }
                    else if (t.End2 is UmlFinalState)
                    {
                        targetNode = document.CreateElement("JUDE:InitialStatePresentation", judeNamespace);
                    }
                    else if (t.End2 is UmlDecision)
                    {
                        targetNode = document.CreateElement("JUDE:MergePresentation", judeNamespace);
                    }
                    else if (t.End2 is UmlActionState)
                    {
                        targetNode = document.CreateElement("JUDE:ActionStatePresentation", judeNamespace);
                    }

                    if (targetNode != null)
                    {
                        targetNode.SetAttribute("xmi.idref", t.End2.Id + "p");
                        servers.AppendChild(targetNode);
                    }
                    #endregion

                    XmlElement uPresentationStyleMapT = document.CreateElement("JUDE:UPresentation.customStyleMap", judeNamespace);
                    itemPresentation.AppendChild(uPresentationStyleMapT);

                    XmlElement stylePropertyT = document.CreateElement("JUDE:UPresentation.styleProperty", judeNamespace);
                    stylePropertyT.SetAttribute("key", "line.shape");
                    stylePropertyT.SetAttribute("value", "line");
                    uPresentationStyleMapT.AppendChild(stylePropertyT);

                    XmlElement pathPresentationT = document.CreateElement("JUDE:PathPresentation.namePresentation", judeNamespace);
                    itemPresentation.AppendChild(pathPresentationT);

                    XmlElement labelPresentationT = document.CreateElement("JUDE:LabelPresentation", judeNamespace);
                    #region Attributes
                    labelPresentationT.SetAttribute("xmi.id", Guid.NewGuid().ToString());
                    labelPresentationT.SetAttribute("version", "0");
                    labelPresentationT.SetAttribute("depth", "0");
                    labelPresentationT.SetAttribute("constraintVisibility", "true");
                    labelPresentationT.SetAttribute("stereotypeVisibility", "true");
                    labelPresentationT.SetAttribute("notationType", "0");
                    labelPresentationT.SetAttribute("width", "11");
                    labelPresentationT.SetAttribute("height", "16");
                    labelPresentationT.SetAttribute("doAutoResize", "true");
                    labelPresentationT.SetAttribute("visibility", "true");
                    #endregion
                    pathPresentationT.AppendChild(labelPresentationT);

                    XmlElement jomtLocationT = document.CreateElement("JUDE:JomtPresentation.location", judeNamespace);
                    labelPresentationT.AppendChild(jomtLocationT);

                    xmiField = document.CreateElement("XMI.field");
                    xmiField.InnerText = "128.0";
                    jomtLocationT.AppendChild(xmiField);

                    xmiField = document.CreateElement("XMI.field");
                    xmiField.InnerText = "128.0";
                    jomtLocationT.AppendChild(xmiField);

                    XmlElement jomtCompositeParentT = document.CreateElement("JUDE:JomtPresentation.compositeParent", judeNamespace);
                    labelPresentationT.AppendChild(jomtCompositeParentT);

                    XmlElement uPresentationParentT = document.CreateElement("JUDE:UPresentation", judeNamespace);
                    uPresentationParentT.SetAttribute("xmi.idref", tran.Id + "p");
                    jomtCompositeParentT.AppendChild(uPresentationParentT);

                    XmlElement judePathPresentationPoints = document.CreateElement("JUDE:PathPresentation.points", judeNamespace);
                    itemPresentation.AppendChild(judePathPresentationPoints);

                    if (itemPresentation != null)
                    {
                        presentations.AppendChild(itemPresentation);
                    }
                }
            }
            #endregion
            #region UseCase Diagram
            foreach (UmlUseCaseDiagram diagram in model.Diagrams.OfType<UmlUseCaseDiagram>())
            {
                XmlElement judeDiagram = document.CreateElement("JUDE:Diagram", judeNamespace);
                String idDiagramJUDE = Guid.NewGuid().ToString();
                judeDiagram.SetAttribute("xmi.id", idDiagramJUDE);
                judeDiagram.SetAttribute("name", diagram.Name);
                judeDiagram.SetAttribute("typeInfo", "UseCase Diagram");
                judeDiagram.SetAttribute("version", "0");
                judeDiagram.SetAttribute("xmlns:JUDE", "http://objectclub.esm.co.jp/Jude/namespace/");
                extensionNode.AppendChild(judeDiagram);

                XmlElement umlModelElementNamespace = document.CreateElement("UML:ModelElement.namespace", umlNamespace);
                judeDiagram.AppendChild(umlModelElementNamespace);

                XmlElement uml_ns = document.CreateElement("UML:Namespace", umlNamespace);
                uml_ns.SetAttribute("xmi.idref", model.Id);
                umlModelElementNamespace.AppendChild(uml_ns);

                XmlElement judeDiagramPresentations = document.CreateElement("JUDE:Diagram.presentations", judeNamespace);
                judeDiagram.AppendChild(judeDiagramPresentations);

                XmlElement judeFramePresentation = document.CreateElement("JUDE:FramePresentation", judeNamespace);
                judeFramePresentation.SetAttribute("xmi.id", Guid.NewGuid().ToString());
                judeFramePresentation.SetAttribute("version", "0");
                judeFramePresentation.SetAttribute("depth", "2147483646");
                judeFramePresentation.SetAttribute("stereotypeVisibility", "true");
                judeFramePresentation.SetAttribute("constraintVisibility", "true");
                judeFramePresentation.SetAttribute("notationType", "0");
                judeFramePresentation.SetAttribute("width", "640.0");
                judeFramePresentation.SetAttribute("height", "480.0");
                judeFramePresentation.SetAttribute("doAutoResize", "true");
                judeFramePresentation.SetAttribute("visibility", "true");
                judeFramePresentation.SetAttribute("label", "+");
                judeDiagramPresentations.AppendChild(judeFramePresentation);

                XmlElement judeUPresentationDiagram = document.CreateElement("JUDE:UPresentation.diagram", judeNamespace);
                judeFramePresentation.AppendChild(judeUPresentationDiagram);

                XmlElement judeDiagram2 = document.CreateElement("JUDE:Diagram", judeNamespace);
                judeDiagram2.SetAttribute("xmi.idref", idDiagramJUDE);
                judeUPresentationDiagram.AppendChild(judeDiagram2);

                XmlElement judeJomtPresentationLocation = document.CreateElement("JUDE:JomtPresentation.location", judeNamespace);
                judeFramePresentation.AppendChild(judeJomtPresentationLocation);

                XmlElement xmiField = document.CreateElement("XMI.field");
                xmiField.InnerText = "10.0";
                judeJomtPresentationLocation.AppendChild(xmiField);

                XmlElement xmiField2 = document.CreateElement("XMI.field");
                xmiField2.InnerText = "10.0";
                judeJomtPresentationLocation.AppendChild(xmiField2);

                #region Actor
                foreach (UmlActor actor in diagram.UmlObjects.OfType<UmlActor>())
                {
                    XmlElement judeClassifierPresentation = document.CreateElement("JUDE:ClassifierPresentation", judeNamespace);
                    judeClassifierPresentation.SetAttribute("xmi.id", actor.Id + "p");
                    judeClassifierPresentation.SetAttribute("version", "0");
                    judeClassifierPresentation.SetAttribute("depth", "2147483636");
                    judeClassifierPresentation.SetAttribute("stereotypeVisibility", "true");
                    judeClassifierPresentation.SetAttribute("constraintVisibility", "true");
                    judeClassifierPresentation.SetAttribute("notationType", "1");
                    judeClassifierPresentation.SetAttribute("width", "40.0");
                    judeClassifierPresentation.SetAttribute("height", "71.0");
                    judeClassifierPresentation.SetAttribute("doAutoResize", "true");
                    judeClassifierPresentation.SetAttribute("visibility", "true");
                    judeClassifierPresentation.SetAttribute("label", actor.Name);
                    judeDiagramPresentations.AppendChild(judeClassifierPresentation);

                    XmlElement judeUPresentationSemanticModel = document.CreateElement("JUDE:UPresentation.semanticModel", judeNamespace);
                    judeClassifierPresentation.AppendChild(judeUPresentationSemanticModel);

                    XmlElement umlActor = document.CreateElement("UML:Actor", umlNamespace);
                    umlActor.SetAttribute("xmi.idref", actor.Id);
                    judeUPresentationSemanticModel.AppendChild(umlActor);

                    XmlElement judeUPresentationDiagram2 = document.CreateElement("JUDE:UPresentation.diagram", judeNamespace);
                    judeClassifierPresentation.AppendChild(judeUPresentationDiagram2);

                    XmlElement judeDiagram3 = document.CreateElement("JUDE:Diagram", judeNamespace);
                    judeDiagram3.SetAttribute("xmi.idref", idDiagramJUDE);
                    judeUPresentationDiagram2.AppendChild(judeDiagram3);

                    XmlElement judeUPresentationClients = document.CreateElement("JUDE:UPresentation.clients", judeNamespace);
                    judeClassifierPresentation.AppendChild(judeUPresentationClients);

                    foreach (UmlAssociation client in diagram.UmlObjects.OfType<UmlAssociation>())
                    {
                        if (client.End1 == actor || client.End2 == actor)
                        {
                            XmlElement judeAssociationPresentation = document.CreateElement("JUDE:AssociationPresentation", judeNamespace);
                            judeAssociationPresentation.SetAttribute("xmi.idref", client.Id + "p");
                            judeUPresentationClients.AppendChild(judeAssociationPresentation);
                        }
                    }

                    XmlElement judeJomtPresentationLocation2 = document.CreateElement("JUDE:JomtPresentation.location", judeNamespace);
                    judeClassifierPresentation.AppendChild(judeJomtPresentationLocation2);

                    XmlElement xmiField3 = document.CreateElement("XMI.field");
                    xmiField3.InnerText = "100.0";
                    judeJomtPresentationLocation2.AppendChild(xmiField3);

                    XmlElement xmiField4 = document.CreateElement("XMI.field");
                    xmiField4.InnerText = "150.0";
                    judeJomtPresentationLocation2.AppendChild(xmiField4);
                }
                #endregion
                #region UseCase
                foreach (UmlUseCase useCase in diagram.UmlObjects.OfType<UmlUseCase>())
                {
                    XmlElement judeUseCasePresentation = document.CreateElement("JUDE:UseCasePresentation", judeNamespace);
                    judeUseCasePresentation.SetAttribute("xmi.id", useCase.Id + "p");
                    judeUseCasePresentation.SetAttribute("version", "0");
                    judeUseCasePresentation.SetAttribute("depth", "2147483635");
                    judeUseCasePresentation.SetAttribute("stereotypeVisibility", "true");
                    judeUseCasePresentation.SetAttribute("constraintVisibility", "true");
                    judeUseCasePresentation.SetAttribute("notationType", "0");
                    judeUseCasePresentation.SetAttribute("width", "120.0");
                    judeUseCasePresentation.SetAttribute("height", "40.0");
                    judeUseCasePresentation.SetAttribute("doAutoResize", "true");
                    judeUseCasePresentation.SetAttribute("visibility", "true");
                    judeUseCasePresentation.SetAttribute("label", useCase.Name);
                    judeDiagramPresentations.AppendChild(judeUseCasePresentation);

                    XmlElement judeUPresentationSemanticModel = document.CreateElement("JUDE:UPresentation.semanticModel", judeNamespace);
                    judeUseCasePresentation.AppendChild(judeUPresentationSemanticModel);

                    XmlElement umlUseCase = document.CreateElement("UML:UseCase", umlNamespace);
                    umlUseCase.SetAttribute("xmi.idref", useCase.Id);
                    judeUPresentationSemanticModel.AppendChild(umlUseCase);

                    XmlElement judeUPresentationDiagram2 = document.CreateElement("JUDE:UPresentation.diagram", judeNamespace);
                    judeUseCasePresentation.AppendChild(judeUPresentationDiagram2);

                    XmlElement judeDiagram3 = document.CreateElement("JUDE:Diagram", judeNamespace);
                    judeDiagram3.SetAttribute("xmi.idref", idDiagramJUDE);
                    judeUPresentationDiagram2.AppendChild(judeDiagram3);

                    XmlElement judeUPresentationClients = document.CreateElement("JUDE:UPresentation.clients", judeNamespace);
                    judeUseCasePresentation.AppendChild(judeUPresentationClients);

                    foreach (UmlAssociation client in diagram.UmlObjects.OfType<UmlAssociation>())
                    {
                        if (client.End1 == useCase || client.End2 == useCase)
                        {
                            XmlElement judeAssociationPresentation = document.CreateElement("JUDE:AssociationPresentation", judeNamespace);
                            judeAssociationPresentation.SetAttribute("xmi.idref", client.Id + "p");
                            judeUPresentationClients.AppendChild(judeAssociationPresentation);
                        }
                    }

                    XmlElement judeJomtPresentationLocation2 = document.CreateElement("JUDE:JomtPresentation.location", judeNamespace);
                    judeUseCasePresentation.AppendChild(judeJomtPresentationLocation2);

                    XmlElement xmiField3 = document.CreateElement("XMI.field");
                    xmiField3.InnerText = "380.0";
                    judeJomtPresentationLocation2.AppendChild(xmiField3);

                    XmlElement xmiField4 = document.CreateElement("XMI.field");
                    xmiField4.InnerText = "190.0";
                    judeJomtPresentationLocation2.AppendChild(xmiField4);
                }
                #endregion
                #region Association
                foreach (UmlAssociation association in diagram.UmlObjects.OfType<UmlAssociation>())
                {
                    XmlElement judeAssociationPresentation = document.CreateElement("JUDE:AssociationPresentation", judeNamespace);
                    judeAssociationPresentation.SetAttribute("xmi.id", association.Id + "p");
                    judeAssociationPresentation.SetAttribute("version", "0");
                    judeAssociationPresentation.SetAttribute("depth", "2147483634");
                    judeAssociationPresentation.SetAttribute("stereotypeVisibility", "true");
                    judeAssociationPresentation.SetAttribute("constraintVisibility", "true");
                    judeAssociationPresentation.SetAttribute("notationType", "0");
                    judeDiagramPresentations.AppendChild(judeAssociationPresentation);

                    XmlElement uPresentationSemanticModel = document.CreateElement("JUDE:UPresentation.semanticModel", judeNamespace);
                    judeAssociationPresentation.AppendChild(uPresentationSemanticModel);

                    XmlElement umlAssociation2 = document.CreateElement("UML:Association", umlNamespace);
                    umlAssociation2.SetAttribute("xmi.idref", association.Id);
                    uPresentationSemanticModel.AppendChild(umlAssociation2);

                    XmlElement judeUPresentationDiagram2 = document.CreateElement("JUDE:UPresentation.diagram", judeNamespace);
                    judeAssociationPresentation.AppendChild(judeUPresentationDiagram2);

                    XmlElement judeDiagram3 = document.CreateElement("JUDE:Diagram", judeNamespace);
                    judeDiagram3.SetAttribute("xmi.idref", idDiagramJUDE);
                    judeUPresentationDiagram2.AppendChild(judeDiagram3);

                    XmlElement judeUPresentationServers = document.CreateElement("JUDE:UPresentation.servers", judeNamespace);
                    judeAssociationPresentation.AppendChild(judeUPresentationServers);

                    UmlAssociation a = (UmlAssociation)association;

                    XmlElement sourceNode = null;

                    if (a.End1 is UmlActor)
                    {
                        sourceNode = document.CreateElement("JUDE:ClassifierPresentation", judeNamespace);
                    }
                    else if (a.End1 is UmlUseCase)
                    {
                        sourceNode = document.CreateElement("JUDE:UseCasePresentation", judeNamespace);
                    }

                    if (sourceNode != null)
                    {
                        sourceNode.SetAttribute("xmi.idref", a.End1.Id + "p");
                        judeUPresentationServers.AppendChild(sourceNode);
                    }

                    XmlElement targetNode = null;

                    if (a.End2 is UmlActor)
                    {
                        targetNode = document.CreateElement("JUDE:ClassifierPresentation", judeNamespace);
                    }
                    else if (a.End2 is UmlUseCase)
                    {
                        targetNode = document.CreateElement("JUDE:UseCasePresentation", judeNamespace);
                    }

                    if (targetNode != null)
                    {
                        targetNode.SetAttribute("xmi.idref", a.End2.Id + "p");
                        judeUPresentationServers.AppendChild(targetNode);
                    }

                    XmlElement judeJomtPresentationLocation2 = document.CreateElement("JUDE:JomtPresentation.location", judeNamespace);
                    judeAssociationPresentation.AppendChild(judeJomtPresentationLocation2);

                    XmlElement xmiField3 = document.CreateElement("XMI.field");
                    xmiField3.InnerText = "0.0";
                    judeJomtPresentationLocation2.AppendChild(xmiField3);

                    XmlElement xmiField4 = document.CreateElement("XMI.field");
                    xmiField4.InnerText = "0.0";
                    judeJomtPresentationLocation2.AppendChild(xmiField4);

                    XmlElement judePathPresentationPoints = document.CreateElement("JUDE:PathPresentation.points", judeNamespace);
                    judeAssociationPresentation.AppendChild(judePathPresentationPoints);
                }
                #endregion
            }
            #endregion
            #endregion
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = new UTF8Encoding(false);
            settings.Indent = true;
            settings.CheckCharacters = true;
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            using (XmlWriter writer = XmlWriter.Create(path + @"\TestPlanGenerate.xml", settings))
                document.Save(writer);
        }

        private static void populateModel(UmlModel model, List<TestCase> listTestCase)
        {
            foreach (TestCase testCase in listTestCase)
            {
                UmlActivityDiagram actDiagram = new UmlActivityDiagram(testCase.Title);
                UmlInitialState stateInitial = new UmlInitialState();
                stateInitial.Name = "initial";
                actDiagram.UmlObjects.Add(stateInitial);
                UmlTransition t = new UmlTransition();
                t.Source = stateInitial;
                foreach (TestStep step in testCase.TestSteps)
                {
                    UmlActionState action = new UmlActionState();
                    string[] lines = Regex.Split(step.Description, "\r\n");
                    action.Name = lines[0];

                    t.TaggedValues.Add("TDACTION", step.Description);
                    t.TaggedValues.Add("TDEXPECTEDRESULT", step.ExpectedResult);
                    actDiagram.UmlObjects.Add(action);
                    t.Target = action;
                    actDiagram.UmlObjects.Add(t);
                    t = new UmlTransition();
                    t.Source = action;
                }
                UmlFinalState stateFinal = new UmlFinalState();
                stateFinal.Name = "final";
                t.Target = stateFinal;
                actDiagram.UmlObjects.Add(stateFinal);
                actDiagram.UmlObjects.Add(t);
                model.AddDiagram(actDiagram);
            }
        }



    }

}
