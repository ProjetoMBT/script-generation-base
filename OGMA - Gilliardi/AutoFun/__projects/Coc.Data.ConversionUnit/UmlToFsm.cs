using System;
using System.Collections.Generic;
using System.Linq;
using Coc.Modeling.Uml;
using Coc.Modeling.FiniteStateMachine;
using Coc.Data.ControlStructure;
using Coc.Data.ControlAndConversionStructures;


namespace Coc.Data.ConversionUnit
{
    public class UmlToFsm : ModelingStructureConverter
    {
        #region Attributes
        private enum hyperLinkType { Source, Target }
        public String id { get; set; }
        #endregion

        #region Public Methods
        public List<GeneralUseStructure> Converter(List<GeneralUseStructure> listModel, StructureType type)
        {
            UmlModel model = listModel.OfType<UmlModel>().FirstOrDefault();
            List<FiniteStateMachine> listFSM = TransformToFsm(model).ToList();
            return listFSM.Cast<GeneralUseStructure>().ToList();
        }

        /// <summary>
        /// Converts given model to an array of FiniteStateMachine.
        /// </summary>
        public FiniteStateMachine[] TransformToFsm(UmlModel model)
        {
            List<FiniteStateMachine> machines = new List<FiniteStateMachine>();
            UmlUseCaseDiagram useCaseDiagram = model.Diagrams.OfType<UmlUseCaseDiagram>().FirstOrDefault();

            if (useCaseDiagram == null)
            {
                throw new Exception("No use case diagram found. Cannot continue.");
            }
            foreach (UmlUseCase useCase in useCaseDiagram.UmlObjects.OfType<UmlUseCase>())
            {
                UmlActivityDiagram activityDiagram = model.Diagrams
                    .OfType<UmlActivityDiagram>()
                    .Where(x => x.Name == useCase.Name)
                    .FirstOrDefault();

                if (activityDiagram != null && ContainsInclude(useCaseDiagram, useCase) == false)
                {
                    FiniteStateMachine generatedMachine = new FiniteStateMachine();
                    generatedMachine = ActivityDiagramToFsm(activityDiagram, model);
                    generatedMachine.Name = useCase.Name;
                    generatedMachine.NameUseCase = useCaseDiagram.Name;
                    generatedMachine.TaggedValues = useCase.TaggedValues;
                    machines.Add(generatedMachine);
                }
            }
            return machines.ToArray();
        }

        public UmlTransition GetUmlTransition(UmlElement act, UmlModel model)
        {
            foreach (UmlActivityDiagram actDiagram in model.Diagrams.OfType<UmlActivityDiagram>())
            {
                foreach (UmlTransition t in actDiagram.UmlObjects.OfType<UmlTransition>())
                {
                    if (t.Source.Id.Equals(act.Id))
                    {
                        return t;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Converts an activity diagram to a finite state machine.
        /// </summary>
        /// <param name="diagram">Diagram to be converted</param>
        /// <param name="model">Parent model of diagram, used to get sub-diagrams</param>
        /// <returns>a FSM of diagram</returns>
        public FiniteStateMachine ActivityDiagramToFsm(UmlActivityDiagram diagram, UmlModel model)
        {
            List<UmlTransition> transitions = diagram.UmlObjects.OfType<UmlTransition>().ToList();
            List<UmlTransition> newTransitions;
            //FiniteStateMachine fsm = new FiniteStateMachine(diagram.Name);
            FiniteStateMachine fsm = new FiniteStateMachine();
            State source = null;
            State target = null;
            String input = "";
            String output = "";
            Boolean haveHiperlinks = true;

            while (haveHiperlinks)
            {
                newTransitions = new List<UmlTransition>();
                foreach (UmlTransition t in transitions)
                {
                    UmlTransition aux = new UmlTransition();
                    aux = t;
                    List<UmlTransition> hyperlinkTrans = null;
                    if (t.Source.TaggedValues.ContainsKey("jude.hyperlink"))
                        hyperlinkTrans = GetTransitionsOfDiagram(model, ref aux, hyperLinkType.Source);
                    if (hyperlinkTrans != null)
                        newTransitions.AddRange(hyperlinkTrans);

                    hyperlinkTrans = null;
                    if (t.Target.TaggedValues.ContainsKey("jude.hyperlink"))
                        hyperlinkTrans = GetTransitionsOfDiagram(model, ref aux, hyperLinkType.Target);
                    if (hyperlinkTrans != null)
                        newTransitions.AddRange(hyperlinkTrans);
                }

                #region new UmlDecision ID - unsuccessful
                List<UmlDecision> ignoreList = new List<UmlDecision>();
                foreach (UmlTransition newT in newTransitions)
                {
                    UmlElement src = newT.Source;
                    UmlElement trg = newT.Target;
                    if (src is UmlDecision)
                    {
                        if (!ignoreList.Contains(src))
                        {
                            List<UmlDecision> decs = (from t in newTransitions
                                                      where t.Source.Name.Equals(src.Name)
                                                      select (UmlDecision)t.Source).Distinct().ToList();
                            decs.AddRange((from t in newTransitions
                                           where t.Target.Name.Equals(src.Name)
                                           select (UmlDecision)t.Target).Distinct().ToList());

                            String decID = Guid.NewGuid().ToString();
                            foreach (UmlDecision d in decs)
                            {
                                d.Id = decID;
                            }
                            ignoreList.AddRange(decs);
                        }
                    }
                    if (trg is UmlDecision)
                    {
                        if (!ignoreList.Contains(trg))
                        {
                            List<UmlDecision> decs = (from t in newTransitions
                                                      where t.Target.Name.Equals(trg.Name)
                                                      select (UmlDecision)t.Target).Distinct().ToList();
                            decs.AddRange((from t in newTransitions
                                           where t.Source.Name.Equals(trg.Name)
                                           select (UmlDecision)t.Source).Distinct().ToList());

                            String decID = Guid.NewGuid().ToString();
                            foreach (UmlDecision d in decs)
                            {
                                d.Id = decID;
                            }
                            ignoreList.AddRange(decs);
                        }
                    }
                }
                #endregion

                transitions.AddRange(newTransitions);
                transitions = transitions.Distinct().ToList();

                haveHiperlinks = transitions.Where(x => x.Source.TaggedValues.ContainsKey("jude.hyperlink") || x.Target.TaggedValues.ContainsKey("jude.hyperlink")).Count() > 0;
            }

            RemoveForks(ref diagram, ref transitions);
            RemoveDecisions(ref diagram, ref transitions);

            UmlTransition auxTran = transitions.Where(x => x.Source is UmlInitialState).FirstOrDefault();
            List<UmlTransition> auxList = new List<UmlTransition>();
            auxList.Add(auxTran);

            for (int i = 0; i < transitions.Count; i++)
            {
                auxTran = transitions.Where(x => x.Source.Equals(auxTran.Target)).FirstOrDefault();
                if (auxTran != null)
                {
                    auxList.Add(auxTran);
                }
            }

            transitions.Clear();
            transitions.AddRange(auxList);

            foreach (UmlTransition t in transitions)
            {
                input = t.GetTaggedValue("TDACTION");
                source = new State(t.Source.Name);
                source.Id = t.Source.Id;

                if (input != null)
                {
                    target = new State(t.Target.Name);
                    target.Id = t.Target.Id;
                    if ((((UmlActionState)t.Target).ParentLane != null) && !String.IsNullOrEmpty(((UmlActionState)t.Target).ParentLane.Name))
                    {
                        target.TaggedValues.Add("Lane", ((UmlActionState)t.Target).ParentLane.Name);
                    }
                    if ((((UmlActionState)t.Source).ParentLane != null) && !String.IsNullOrEmpty(((UmlActionState)t.Source).ParentLane.Name))
                    {
                        source.TaggedValues.Add("Lane", ((UmlActionState)t.Source).ParentLane.Name);
                    }
                    output = "";
                    if (t.GetTaggedValue("TDEXPECTEDRESULT") != null)
                    {
                        output = t.GetTaggedValue("TDEXPECTEDRESULT");
                    }

                    #region Cycles
                    bool cycleTran = false;
                    if (t.GetTaggedValue("TDCYCLETRAN") != null)
                    {
                        cycleTran = (t.GetTaggedValue("TDCYCLETRAN").Equals("true") ? true : false);
                    }
                    bool lastCycleTrans = false;
                    if (t.GetTaggedValue("TDLASTCYCLETRANS") != null)
                    {
                        //lastCycleTrans = (t.GetTaggedValue("TDCYCLETRAN").Equals("true") ? true : false);
                        lastCycleTrans = (t.GetTaggedValue("TDLASTCYCLETRANS").Equals("true") ? true : false);
                    }
                    Transition trans = new Transition(source, target, input, output, cycleTran, lastCycleTrans);
                    if (t.GetTaggedValue("TDLASTCYCLETRANS") != null)
                    {
                        trans.TaggedValues.Add("TDCYCLETRAN", t.GetTaggedValue("TDCYCLETRAN"));
                    }
                    #endregion

                    foreach (KeyValuePair<String, String> pair in t.TaggedValues)
                    {
                        trans.TaggedValues.Add(pair.Key, pair.Value);
                    }
                    //trans.TaggedValues.Add("TDACTION", t.GetTaggedValue("TDACTION"));
                    //trans.TaggedValues.Add("TDEXPECTEDRESULT", t.GetTaggedValue("TDEXPECTEDRESULT") + "");

                    fsm.AddTransition(trans);
                }

                if (t.Target is UmlFinalState)
                {
                    fsm.CheckAsFinal(source);
                }
            }
            fsm = WipeOutOutermost(diagram, fsm);
            fsm.InitialState = GetFsmInitialState(fsm);
            fsm.Name = diagram.Name;
            
            return fsm;
        }
        #endregion

        #region  Private Methods
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
            if (tp == hyperLinkType.Source)
            {
                if (t.Source.TaggedValues.ContainsKey("cycles"))
                    c = Convert.ToInt32(t.Source.GetTaggedValue("cycles"));
                hyperlink = t.Source.TaggedValues["jude.hyperlink"];
            }
            else
            {
                if (t.Target.TaggedValues.ContainsKey("cycles"))
                    c = Convert.ToInt32(t.Target.GetTaggedValue("cycles"));
                hyperlink = t.Target.TaggedValues["jude.hyperlink"];
            }

            UmlActivityDiagram subDiagram = model.Diagrams.OfType<UmlActivityDiagram>()
                                                          .Where(y => y.Name.Equals(hyperlink))
                                                          .FirstOrDefault();

            if (subDiagram == null)
                throw new Exception("Could not find any Activity Diagram named " + hyperlink);

            subTransitions = subDiagram.UmlObjects.OfType<UmlTransition>().ToList();
            List<UmlTransition> fs = null;
            UmlTransition f = null;
            if (tp == hyperLinkType.Source)
            {
                fs = subTransitions.Where(x => x.Target is UmlFinalState).ToList();
                f = fs.ElementAt(0);
            }
            else
                f = subTransitions.Single(x => x.Source is UmlInitialState);

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
                            if (!temp.TaggedValues.ContainsKey(tag.Key))
                                temp.TaggedValues.Add(tag.Key, tag.Value);
                    }
                }
                else
                    s = f.Target;
                foreach (KeyValuePair<string, string> tag in f.TaggedValues)
                    if (!t.TaggedValues.ContainsKey(tag.Key))
                        t.TaggedValues.Add(tag.Key, tag.Value);
                //subTransitions.Remove(f);
            }
            else
                if (tp == hyperLinkType.Source)
                    s = subDiagram.UmlObjects.OfType<UmlFinalState>().FirstOrDefault();
                else
                    s = subDiagram.UmlObjects.OfType<UmlInitialState>().FirstOrDefault();

            UmlTransition initialT = subTransitions.SingleOrDefault(x => x.Source is UmlInitialState);

            subTransitions.RemoveAll(x => x.Target is UmlFinalState);
            subTransitions.RemoveAll(x => x.Source is UmlInitialState);
            if (tp == hyperLinkType.Source)
                t.Source = s;
            else
            {
                if (t.Source is UmlInitialState)
                {
                    ((UmlActionState)t.Source).ParentLane = ((UmlActionState)f.Source).ParentLane;
                    ((UmlActionState)t.Source).ParentLane.AddElement(t.Source);
                }
                t.Target = s;
            }

            #region cycles

            List<UmlTransition> hyperlinkTrans2 = new List<UmlTransition>();
            if (c > 1)
            {
                List<UmlTransition> temp = subTransitions;
                UmlElement firstFirstState = null;
                UmlElement prevLastState = null;
                UmlElement lastSate = null;
                UmlTransition lastTran = null;
                for (int i = 0; i < c; i++)
                {
                    UmlElement currFirstState = null;
                    foreach (UmlTransition trans in temp)
                    {
                        UmlTransition cycleTran = new UmlTransition();
                        UmlElement src = (UmlElement)Activator.CreateInstance(trans.Source.GetType());
                        UmlElement targ = (UmlElement)Activator.CreateInstance(trans.Target.GetType());
                        if (i != 0)
                        {
                            src.Name = trans.Source.Name + "_" + i;
                            targ.Name = trans.Target.Name + "_" + i;
                        }
                        else
                        {
                            src.Name = trans.Source.Name;
                            targ.Name = trans.Target.Name;
                        }
                        src.Id = trans.Source.Id;
                        //src.Id = Guid.NewGuid().ToString();
                        //targ.Name = trans.Target.Name + "_" + i;
                        targ.Id = trans.Target.Id;
                        //targ.Id = Guid.NewGuid().ToString();
                        foreach (KeyValuePair<String, String> tag in trans.TaggedValues)
                        {
                            cycleTran.SetTaggedValue(tag.Key, tag.Value + "$@#ITERATION@#" + i);
                        }
                        cycleTran.SetTaggedValue("TDCYCLETRAN", "true");
                        cycleTran.Source = src;
                        cycleTran.Target = targ;
                        hyperlinkTrans2.Add(cycleTran);
                        lastTran = cycleTran;
                        if (currFirstState == null)
                            currFirstState = src;
                        lastSate = targ;
                    }
                    if (prevLastState != null)
                    {
                        UmlTransition cycleTran = new UmlTransition();
                        if (initialT != null)
                        {
                            foreach (KeyValuePair<String, String> tag in initialT.TaggedValues)
                            {
                                // cycleTran.SetTaggedValue(tag.Key, tag.Value + ". Iteration " + i);
                                cycleTran.SetTaggedValue(tag.Key, tag.Value + "$@#ITERATION@#" + i);
                            }
                            cycleTran.SetTaggedValue("TDCYCLETRAN", "true");
                            cycleTran.SetTaggedValue("TDLASTCYCLETRANS", "true");
                        }

                        cycleTran.Source = prevLastState;
                        cycleTran.Target = currFirstState;
                        if (cycleTran.TaggedValues.Count > 0)
                        {
                            hyperlinkTrans2.Add(cycleTran);
                            lastTran = cycleTran;
                        }
                    }
                    if (currFirstState != null)
                        if (firstFirstState == null)
                            firstFirstState = currFirstState;
                    prevLastState = lastSate;
                }

                if (tp == hyperLinkType.Source)
                {
                    t.Source = lastSate;
                    t.SetTaggedValue("TDLASTCYCLETRANS", "true");
                }
                else
                {
                    t.Target = firstFirstState;
                }
                t.SetTaggedValue("TDCYCLETRAN", "true");
                subTransitions = hyperlinkTrans2;
            }
            #endregion

            return subTransitions;
        }

        /// <summary>
        /// Get initial state of FSM
        /// </summary>
        /// <param name="fsm"></param>
        /// <returns></returns>
        private State GetFsmInitialState(FiniteStateMachine fsm)
        {
            return (from t in fsm.Transitions
                    where fsm.Transitions.Count(x => x.TargetState.Equals(t.SourceState)) == 0
                    select t.SourceState).First();
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
                            //tran.TaggedValues.Add("TDParalellState", "true");
                            newTransitions.Add(tran);
                            //s = t;
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
        /// Remove forks and joins of diagram
        /// </summary>
        /// <param name="diagram">targeted diagram to remove fork/join nodes from</param>
        /// <param name="transitions">transitions to be searched and replaced</param>
        private static void RemoveForks(ref UmlActivityDiagram diagram, ref List<UmlTransition> transitions)
        {
            List<UmlFork> forks = (from t in transitions
                                   where t.Target is UmlFork
                                   select (UmlFork)t.Target).Distinct().ToList();

            foreach (UmlFork fork in forks)
            {
                List<UmlTransition> forkLeafs = transitions.Where(x => x.Source.Equals(fork)).ToList();
                List<UmlTransition> newTransitions = new List<UmlTransition>();
                UmlElement s = transitions.Where(x => x.Target.Equals(fork)).FirstOrDefault().Source;
                UmlElement t = null;
                UmlTransition tran;
                for (int i = 0; i < forkLeafs.Count; i++)
                {
                    t = forkLeafs[i].Target;
                    tran = new UmlTransition();
                    tran.Source = s;
                    tran.Target = t;
                    foreach (KeyValuePair<String, String> tag in forkLeafs[i].TaggedValues)
                    {
                        tran.TaggedValues.Add(tag.Key, tag.Value);
                    }
                    tran.TaggedValues.Add("TDPARALELLSTATE", "true");
                    newTransitions.Add(tran);
                    s = t;
                }
                UmlTransition toJoin = transitions.Where(x => x.Source.Equals(s) && x.Target is UmlJoin).FirstOrDefault();
                UmlJoin join = (UmlJoin)toJoin.Target;
                UmlTransition fromJoin = transitions.Where(x => x.Source.Equals(join)).FirstOrDefault();
                tran = new UmlTransition();
                foreach (KeyValuePair<String, String> tag in fromJoin.TaggedValues)
                {
                    tran.TaggedValues.Add(tag.Key, tag.Value);
                }
                tran.Source = s;
                tran.Target = fromJoin.Target;
                newTransitions.Add(tran);

                transitions.RemoveAll(x => x.Target.Equals(fork) || x.Source.Equals(fork));
                transitions.RemoveAll(x => x.Target.Equals(join) || x.Source.Equals(join));
                diagram.UmlObjects.Remove(fork);
                diagram.UmlObjects.Remove(join);
                transitions.AddRange(newTransitions);
            }
        }

        /// <summary>
        /// Wipe out of the given FSM the outermost states of it, i.e. UmlInitialState and UmlFinalState
        /// </summary>
        /// <param name="diagram">Uml diagram of the given FSM</param>
        /// <param name="fsm">FSM to clean</param>
        /// <returns>cleaned FSM</returns>
        private FiniteStateMachine WipeOutOutermost(UmlActivityDiagram diagram, FiniteStateMachine fsm)
        {
            UmlFinalState digFinal = diagram.UmlObjects.OfType<UmlFinalState>().FirstOrDefault();
            if (digFinal == null)
                throw new Exception("Activity Diagram " + diagram.Name + " must have a final state.");

            fsm.WipeOutState(new State(digFinal.Name));

            UmlInitialState digInitial = diagram.UmlObjects.OfType<UmlInitialState>().FirstOrDefault();
            if (digInitial == null)
                throw new Exception("Activity Diagram " + diagram.Name + " must have a initial state.");
            if (diagram.UmlObjects.OfType<UmlTransition>().Where(x => x.Target.Equals(digInitial)
                                                                   || x.Source.Equals(digInitial)
                                                                   && x.TaggedValues.Count > 0
                                                                ).Count() == 0)
            {
                fsm.WipeOutState(new State(digInitial.Name));
            }
            return fsm;
        }

        private State getInitial(UmlActivityDiagram actDiagram, List<State> listState, UmlModel model)
        {
            foreach (UmlInitialState initial in actDiagram.UmlObjects.OfType<UmlInitialState>())
            {
                foreach (UmlTransition tr in actDiagram.UmlObjects.OfType<UmlTransition>())
                {
                    if (tr.Source.Id.Equals(initial.Id))
                    {
                        if (tr.Target.GetTaggedValue("jude.hyperlink") == null)
                        {
                            return GetState(listState, tr.Target.Name);
                        }
                        else
                        {
                            UmlActivityDiagram activityDiagram = model.Diagrams
                                .OfType<UmlActivityDiagram>()
                                .Where(x => x.Name == tr.Target.GetTaggedValue("jude.hyperlink"))
                                .FirstOrDefault();
                            State v = getInitial(activityDiagram, listState, model);
                            return v;
                        }
                    }
                }
            }
            return null;
        }

        private State GetState(List<State> listState, String name)
        {
            foreach (State item in listState)
            {
                if (item.Name.Equals(name))
                {
                    return item;
                }
            }
            return null;
        }
        #endregion
    }
}
