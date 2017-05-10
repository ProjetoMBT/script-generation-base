using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coc.Modeling.Graph;
using Coc.Modeling.Uml;
using Coc.Data.ControlAndConversionStructures;
using Coc.Data.ControlStructure;
using System.Web;
using System.Text.RegularExpressions;

namespace Coc.Data.ConversionUnit
{
    public class UmlToGraphOATS : ModelingStructureConverter
    {
        #region Attributes
        public String id { get; set; }
        private String tag1;
        private String tag2;
        private enum hyperLinkType
        {
            Source,
            Target
        }
        #endregion

        #region Constructor
        public UmlToGraphOATS()
        {
            tag1 = "TDACTION";
            tag2 = "TDOBJECT";
        }
        #endregion

        #region Public Methods
        public DirectedGraph[] TransformToGraph(UmlModel model)
        {
            List<DirectedGraph> graphs = new List<DirectedGraph>();
            UmlUseCaseDiagram useCaseDiagram = model.Diagrams.OfType<UmlUseCaseDiagram>().FirstOrDefault();

            if (useCaseDiagram == null)
            {
                throw new Exception("No use case diagram found. Cannot continue.");
            }
            foreach (UmlUseCase useCase in useCaseDiagram.UmlObjects.OfType<UmlUseCase>())
            {
                String aux = useCase.GetTaggedValue("jude.hyperlink");
                UmlActivityDiagram activityDiagram = null;

                if (aux != null)
                {
                    activityDiagram = model.Diagrams.OfType<UmlActivityDiagram>()
                                                    .Where(x => x.Name == aux)
                                                    .FirstOrDefault();
                }
                else
                {
                    activityDiagram = model.Diagrams.OfType<UmlActivityDiagram>()
                                                    .Where(x => x.Name == useCase.Name)
                                                    .FirstOrDefault();
                }
                if (activityDiagram != null && ContainsInclude(useCaseDiagram, useCase) == false)
                {
                    DirectedGraph graph = new DirectedGraph();
                    graph = ActivityDiagramToGraph(activityDiagram, model);
                    GetUseCaseTaggedValues(useCase, graph);
                    //graph.NameUseCase = useCaseDiagram.Name;
                    graphs.Add(graph);
                }
            }
            return graphs.ToArray();
        }

        /// <summary>
        /// Converts an activity diagram to a finite state machine.
        /// </summary>
        /// <param name="diagram">Diagram to be converted</param>
        /// <param name="model">Parent model of diagram, used to get sub-diagrams</param>
        /// <returns>a FSM of diagram</returns>
        public DirectedGraph ActivityDiagramToGraph(UmlActivityDiagram diagram, UmlModel model)
        {
            List<UmlTransition> transitions = diagram.UmlObjects.OfType<UmlTransition>().ToList();
            DirectedGraph graph = new DirectedGraph(diagram.Name);
            Node source = null;
            Node target = null;
            String input = "";
            String output = "";
            Boolean haveHyperlinks = true;
            List<UmlTransition> newTransitions;

            while (haveHyperlinks)
            {
                newTransitions = new List<UmlTransition>();
                foreach (UmlTransition t in transitions)
                {
                    String taux = HttpUtility.UrlDecode(t.ToString());
                    String tactionaux = HttpUtility.UrlDecode(t.GetTaggedValue("TDACTION"));
                    String tobjectaux = HttpUtility.UrlDecode(t.GetTaggedValue("TDOBJECT"));
                    UmlTransition aux = t;

                    if (t.Source.TaggedValues.ContainsKey("jude.hyperlink"))
                    {
                        newTransitions.AddRange(GetTransitionsOfDiagram(model, ref aux, hyperLinkType.Source));
                    }
                    if (t.Target.TaggedValues.ContainsKey("jude.hyperlink"))
                    {
                        newTransitions.AddRange(GetTransitionsOfDiagram(model, ref aux, hyperLinkType.Target));
                    }
                }

                transitions.AddRange(newTransitions);
                transitions = transitions.Distinct().ToList();

                haveHyperlinks = transitions.Where(x => x.Source.TaggedValues.ContainsKey("jude.hyperlink") || x.Target.TaggedValues.ContainsKey("jude.hyperlink")).Count() > 0;
            }

            //RemoveForks(ref diagram, ref transitions);
            RemoveDecisions(ref diagram, ref transitions);
            Dictionary<String, String> tags;

            foreach (UmlTransition t in transitions)
            {
                

                tags = new Dictionary<string, string>();
                input = t.GetTaggedValue(tag1);
                source = new Node(t.Source.Name);
                source.Id = t.Source.Id;
                if (input != null)
                {
                    target = new Node(t.Target.Name);
                    target.Id = t.Target.Id;
                    output = "";
                    if (t.GetTaggedValue(tag2) != null)
                    {
                        output = t.GetTaggedValue(tag2);
                    }
                    bool cycleTran = false;
                    if (t.GetTaggedValue("TDCYCLETRAN") != null)
                    {
                        cycleTran = (t.GetTaggedValue("TDCYCLETRAN").Equals("true") ? true : false);
                        if (t.GetTaggedValue("TDCYCLETRAN").Equals("true"))
                        {
                            tags.Add("TDCYCLETRAN", "true");
                        }
                        else
                        {
                            tags.Add("TDCYCLETRAN", "false");
                        }
                    }

                    bool lastCycleTrans = false;
                    if (t.GetTaggedValue("TDLASTCYCLETRANS") != null)
                    {
                        lastCycleTrans = (t.GetTaggedValue("TDLASTCYCLETRANS").Equals("true") ? true : false);
                        if (t.GetTaggedValue("TDLASTCYCLETRANS").Equals("true"))
                        {
                            tags.Add("TDLASTCYCLETRANS", "true");
                        }
                        else
                        {
                            tags.Add("TDLASTCYCLETRANS", "false");
                        }
                    }
                    tags.Add(tag1, input);
                    tags.Add(tag2, output);

                    Edge e = new Edge(source, target, tags);
                    graph.addEdge(e);
                }
                if (t.Target is UmlFinalState)
                {
                    graph.checkFinal(source);
                }
            }

            diagram.UmlObjects.RemoveAll(IsTransition);
            diagram.UmlObjects.AddRange(transitions);

            graph = WipeOutOutermost(diagram, graph);
            graph.RootNode = GetRootNode(graph);
            graph.Name = diagram.Name;
            return graph;
        }

        public List<GeneralUseStructure> Converter(List<GeneralUseStructure> listModel, StructureType type)
        {
            UmlModel model = listModel.OfType<UmlModel>().FirstOrDefault();
            List<DirectedGraph> listGraph = TransformToGraph((UmlModel)model).ToList();
            return listGraph.Cast<GeneralUseStructure>().ToList();
        }
        #endregion

        #region Private Methods
        private void GetUseCaseTaggedValues(UmlUseCase useCase, DirectedGraph graph)
        {
            foreach (KeyValuePair<String, String> pair in useCase.TaggedValues)
            {
                if (!pair.Key.Equals("jude.hyperlink"))
                {
                    graph.Values.Add(pair.Key, pair.Value);
                }
            }
        }

        private Node GetRootNode(DirectedGraph graph)
        {
            Node root = new Node();
            root = (from e in graph.Edges
                    where graph.Edges.Count(x => x.NodeB.Equals(e.NodeA)) == 0
                    select e.NodeA).First();
            return root;
        }

        private bool ContainsInclude(UmlUseCaseDiagram diagram, UmlUseCase useCase)
        {
            bool IsInclude = true;
            foreach (UmlAssociation item in diagram.UmlObjects.OfType<UmlAssociation>())
            {
                if (item.End1.Id.Equals(useCase.Id) && item.End2 is UmlActor || item.End2.Id.Equals(useCase.Id) && item.End1 is UmlActor)
                {
                    IsInclude = false;
                }
            }

            return IsInclude;
        }

        /// <summary>
        /// Get all transitions of the desired diagram adjusting the initial and final insertion points using <paramref name="t"/>
        /// </summary>
        /// <param name="model">The model where the diagram is</param>
        /// <param name="t">the transation with hyperlink</param>
        /// <param name="tp">the side where the hyperlink is (source or target)</param>
        /// <returns>a list of the transitions</returns>
        private List<UmlTransition> GetTransitionsOfDiagram(UmlModel model, ref UmlTransition t, hyperLinkType tp)
        {
            List<UmlTransition> subTransitions;
            UmlElement s;
            String hyperlink = "";
            int c = 0;
            Boolean paramcycle = false;

            if (tp == hyperLinkType.Source)
            {
                /*
                if (t.Source.TaggedValues.ContainsKey("cycles"))
                {
                    c = Convert.ToInt32(t.Source.GetTaggedValue("cycles"));
                    paramcycle = true;
                }*/
                hyperlink = t.Source.TaggedValues["jude.hyperlink"];
            }
            else
            {
                /*
                if (t.Target.TaggedValues.ContainsKey("cycles"))
                {
                    c = Convert.ToInt32(t.Target.GetTaggedValue("cycles"));
                    paramcycle = true;
                }*/
                hyperlink = t.Target.TaggedValues["jude.hyperlink"];
            }

            //recupera o subdiagrama que foi atribuido a variavel hyperlink 
            UmlActivityDiagram subDiagram = model.Diagrams.OfType<UmlActivityDiagram>()
                                                          .Where(y => y.Name.Equals(hyperlink))
                                                          .FirstOrDefault();
            
            if (subDiagram == null)
            {
                throw new Exception("Could not find any Activity Diagram named " + hyperlink);
            }


            //recupera a primeira transicao para buscar o valor de marcacao que deve ser atribuido
            subTransitions = subDiagram.UmlObjects.OfType<UmlTransition>().ToList();
            foreach (UmlTransition tran in subTransitions)
            {
                if (tran.End1.Name.Equals("InitialNode0"))
                {
                    paramcycle = tran.TaggedValues.ContainsKey("TDITERATIONS");
                    try
                    {
                        c = Convert.ToInt32(tran.GetTaggedValue("TDITERATIONS"));
                    }
                    catch (FormatException)
                    {
                        c = 0;
                    }
                }
            }
            
            
            if (paramcycle)
            {
                foreach (UmlTransition subTransition in subTransitions)
                {
                    if (!subTransition.TaggedValues.ContainsKey("paramcycle"))
                    {
                        subTransition.TaggedValues.Add("paramcycle", c.ToString());
                        //subTransition.TaggedValues.Add("paramcycle", "exists");
                    }
                }
            }
            List<UmlTransition> fs = null;
            UmlTransition f = null;
            if (tp == hyperLinkType.Source)
            {
                fs = subTransitions.Where(x => x.Target is UmlFinalState).ToList();
                f = fs.ElementAt(0);
            }
            else
            {
                f = subTransitions.Single(x => x.Source is UmlInitialState);
            }

            if (f != null)
            {
                if (tp == hyperLinkType.Source)
                {
                    s = f.Source;
                    for (int i = 1; i < fs.Count; i++)
                    {
                        UmlTransition temp = fs.ElementAt(i);
                        temp.Target = t.Target;
                        foreach (KeyValuePair<string, string> tag in t.TaggedValues)
                        {
                            if (!temp.TaggedValues.ContainsKey(tag.Key))
                            {
                                temp.TaggedValues.Add(tag.Key, tag.Value);
                            }
                        }
                    }
                }
                else
                {
                    s = f.Target;
                }
                foreach (KeyValuePair<string, string> tag in f.TaggedValues)
                {
                    if (!t.TaggedValues.ContainsKey(tag.Key))
                    {
                        t.TaggedValues.Add(tag.Key, tag.Value);
                    }
                }
                //subTransitions.Remove(f);
            }
            else
            {
                if (tp == hyperLinkType.Source)
                {
                    s = subDiagram.UmlObjects.OfType<UmlFinalState>().FirstOrDefault();
                }
                else
                {
                    s = subDiagram.UmlObjects.OfType<UmlInitialState>().FirstOrDefault();
                }
            }

            UmlTransition initialT = subTransitions.SingleOrDefault(x => x.Source is UmlInitialState);

            subTransitions.RemoveAll(x => x.Target is UmlFinalState);
            subTransitions.RemoveAll(x => x.Source is UmlInitialState);

            if (tp == hyperLinkType.Source)
            {
                t.Source = s;
            }
            else
            {
                t.Target = s;
            }

            #region cycles

            //List<UmlTransition> hyperlinkTrans2 = new List<UmlTransition>();
            //if (c > 1)
            //{
            //    List<UmlTransition> temp = subTransitions;
            //    UmlElement firstFirstState = null;
            //    UmlElement prevLastState = null;
            //    UmlElement lastSate = null;
            //    UmlTransition lastTran = null;
            //    for (int i = 0; i < c; i++)
            //    {
            //        UmlElement currFirstState = null;
            //        foreach (UmlTransition trans in temp)
            //        {
            //            UmlTransition cycleTran = new UmlTransition();
            //            UmlElement src = (UmlElement)Activator.CreateInstance(trans.Source.GetType());
            //            UmlElement targ = (UmlElement)Activator.CreateInstance(trans.Target.GetType());
            //            if (i != 0)
            //            {
            //                src.Name = trans.Source.Name + "_" + (i);
            //                targ.Name = trans.Target.Name + "_" + (i);
            //            }
            //            else
            //            {
            //                src.Name = trans.Source.Name;
            //                targ.Name = trans.Target.Name;
            //            }
            //            src.Id = trans.Source.Id;
            //            targ.Id = trans.Target.Id;
            //            foreach (KeyValuePair<String, String> tag in trans.TaggedValues)
            //            {
            //                cycleTran.SetTaggedValue(tag.Key, tag.Value + "$@#ITERATION@#" + i);
            //            }
            //            cycleTran.SetTaggedValue("TDCYCLETRAN", "true");
            //            cycleTran.Source = src;
            //            cycleTran.Target = targ;
            //            hyperlinkTrans2.Add(cycleTran);
            //            lastTran = cycleTran;
            //            if (currFirstState == null)
            //                currFirstState = src;
            //            lastSate = targ;
            //        }
            //        if (prevLastState != null)
            //        {
            //            UmlTransition cycleTran = new UmlTransition();
            //            if (initialT != null)
            //            {
            //                foreach (KeyValuePair<String, String> tag in initialT.TaggedValues)
            //                {
            //                    cycleTran.SetTaggedValue(tag.Key, tag.Value + "$@#ITERATION@#" + i);
            //                }
            //                cycleTran.SetTaggedValue("TDCYCLETRAN", "true");
            //                cycleTran.SetTaggedValue("TDLASTCYCLETRANS", "true");
            //            }
            //            cycleTran.Source = prevLastState;
            //            cycleTran.Target = currFirstState;
            //            if (cycleTran.TaggedValues.Count > 0)
            //            {
            //                hyperlinkTrans2.Add(cycleTran);
            //                lastTran = cycleTran;
            //            }
            //        }
            //        if (currFirstState != null)
            //            if (firstFirstState == null)
            //                firstFirstState = currFirstState;
            //        prevLastState = lastSate;
            //    }

            //    if (tp == hyperLinkType.Source)
            //    {
            //        t.Source = lastSate;
            //        t.SetTaggedValue("TDLASTCYCLETRANS", "true");
            //    }
            //    else
            //    {
            //        t.Target = firstFirstState;
            //    }
            //    t.SetTaggedValue("TDCYCLETRAN", "true");
            //    subTransitions = hyperlinkTrans2;
            //}
            #endregion

            return subTransitions;
        }

        /// <summary>
        /// Remove decisions/merge nodes of diagram
        /// </summary>
        /// <param name="diagram">targeted diagram to remove decision/merge nodes from</param>
        /// <param name="transitions">transitions to be searched and replaced</param>
        private void RemoveDecisions(ref UmlActivityDiagram diagram, ref List<UmlTransition> transitions)
        {
            List<UmlDecision> decs = (from t in transitions
                                      where t.Target is UmlDecision
                                      select (UmlDecision)t.Target).Distinct().ToList();
            while (decs.Count > 0)
            {
                foreach (UmlDecision decision in decs)
                {
                    List<UmlTransition> decisionProspects = transitions.Where(x => x.Source.Equals(decision)).ToList();
                    List<UmlTransition> newTransitions = new List<UmlTransition>();
                    List<UmlTransition> Ss = transitions.Where(x => x.Target.Equals(decision)).ToList();
                    foreach (UmlTransition sT in Ss)
                    {
                        UmlElement s = sT.Source;
                        UmlElement t = null;
                        UmlTransition tran;
                        for (int i = 0; i < decisionProspects.Count; i++)
                        {
                            t = decisionProspects[i].Target;
                            tran = new UmlTransition();
                            tran.Source = s;
                            tran.Target = t;
                            foreach (KeyValuePair<string, string> tag in decisionProspects[i].TaggedValues)
                                tran.TaggedValues.Add(tag.Key, tag.Value);
                            newTransitions.Add(tran);
                        }
                    }
                    transitions.RemoveAll(x => x.Target.Equals(decision) || x.Source.Equals(decision));
                    diagram.UmlObjects.Remove(decision);
                    transitions.AddRange(newTransitions);
                }

                decs = (from t in transitions
                        where t.Target is UmlDecision
                        select (UmlDecision)t.Target).Distinct().ToList();
            }
        }

        /// <summary>
        /// Wipe out of the given FSM the outermost states of it, i.e. UmlInitialState and UmlFinalState
        /// </summary>
        /// <param name="diagram">Uml diagram of the given FSM</param>
        /// <param name="graph">graph to clean</param>
        /// <returns>cleaned FSM</returns>
        private DirectedGraph WipeOutOutermost(UmlActivityDiagram diagram, DirectedGraph graph)
        {
            UmlFinalState digFinal = diagram.UmlObjects.OfType<UmlFinalState>().FirstOrDefault();
            if (digFinal == null)
                throw new Exception("Activity Diagram " + diagram.Name + " must have a final state.");


            Node nF = new Node(digFinal.Name, digFinal.Id);
            graph.WipeOutNode(nF);

            UmlInitialState digInitial = diagram.UmlObjects.OfType<UmlInitialState>().FirstOrDefault();
            if (digInitial == null)
                throw new Exception("Activity Diagram " + diagram.Name + " must have a initial state.");
            if (diagram.UmlObjects.OfType<UmlTransition>().Where(x => x.Target.Equals(digInitial)
                                                                   || x.Source.Equals(digInitial)
                                                                   && x.TaggedValues.Count > 0
                                                                ).Count() == 0)
            {
                graph.WipeOutNode(new Node(digInitial.Name));
            }
            return graph;
        }

        private Boolean IsTransition(UmlBase element)
        {
            return element is UmlTransition;
        }
        #endregion

    }
}
