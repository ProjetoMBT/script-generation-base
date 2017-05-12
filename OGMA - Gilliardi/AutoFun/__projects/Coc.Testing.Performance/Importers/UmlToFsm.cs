using System;
using System.Collections.Generic;
using System.Linq;

using Coc.Modeling.Uml;
using Coc.Modeling.FiniteStateMachine;

namespace Coc.Testing.Performance.Importers
{
    public class UmlToFsm 
    {
        public String id { get; set; }
        private static UmlModel model2;
        // private static UmlModelImporter java;
        /// <summary>
        /// Converts given model to an array of FiniteStateMachine.
        /// </summary>
        public static FiniteStateMachine[] TransformToFsm(UmlModel model)
        {
            model2 = new UmlModel("");
            model2 = model;
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

                //if (activityDiagram == null)
                //{
                //    throw new Exception("Could not find any Activity Diagram named " + useCase.Name);
                //}
                if (activityDiagram != null && ContainsInclude(useCaseDiagram, useCase) == false)
                {
                    FiniteStateMachine generatedMachine = new FiniteStateMachine(useCase.Name);
                    generatedMachine = ActivityDiagramToFsm(activityDiagram, model);
                    machines.Add(generatedMachine);
                }
            }
            return machines.ToArray();
        }

        /// <summary>
        /// Converts an activity diagram to a finite state machine.
        /// </summary>
        /// <param name="diagram">Diagram to be converted</param>
        /// <param name="model">Parent model of diagram, used to get sub-diagrams</param>
        /// <returns>a FSM of diagram</returns>
        public static FiniteStateMachine ActivityDiagramToFsm(UmlActivityDiagram diagram, UmlModel model)
        {
            List<UmlTransition> transitions = diagram.UmlObjects.OfType<UmlTransition>().ToList();
            FiniteStateMachine fsm = new FiniteStateMachine(diagram.Name);
            State source = null;
            State target = null;
            String input = "";
            String output = "";
            Boolean haveHyperlinks = true;
            List<UmlTransition> newTransitions;

            while (haveHyperlinks)
            {
                newTransitions = new List<UmlTransition>();
                foreach (UmlTransition t in transitions)
                {
                    UmlTransition aux = t;
                    if (t.Source.TaggedValues.ContainsKey("jude.hyperlink"))
                        newTransitions.AddRange(GetTransitionsOfDiagram(model, ref aux, hyperLinkType.Source));
                    if (t.Target.TaggedValues.ContainsKey("jude.hyperlink"))
                        newTransitions.AddRange(GetTransitionsOfDiagram(model, ref aux, hyperLinkType.Target));
                }

                transitions.AddRange(newTransitions);
                transitions = transitions.Distinct().ToList();

                haveHyperlinks = transitions.Where(x => x.Source.TaggedValues.ContainsKey("jude.hyperlink") || x.Target.TaggedValues.ContainsKey("jude.hyperlink")).Count() > 0;
            }

            RemoveForks(ref diagram, ref transitions);
            RemoveDecisions(ref diagram, ref transitions);

            foreach (UmlTransition t in transitions)
            {
                input = t.GetTaggedValue("TDACTION");
                source = new State(t.Source.Name);
                source.Id = t.Source.Id;
                
                if (input != null)
                {
                    target = new State(t.Target.Name);
                    target.Id = t.Target.Id;
                    output = "";
                    if (t.GetTaggedValue("TDPARAMETERS") != null)
                    {
                        output = t.GetTaggedValue("TDPARAMETERS");
                    }
                    fsm.AddTransition(new Transition(source, target, input, output));
                }
                if (t.Target is UmlFinalState)
                {
                    fsm.CheckAsFinal(source);
                }
            }

            foreach (Transition t in fsm.Transitions)
            {
                State s = fsm.States
                             .Where(x => x.Name.Equals(t.SourceState.Name))
                             .FirstOrDefault();
                s.Transitions.Add(t);
            }

            fsm = WipeOutOutermost(diagram, fsm);
            fsm.InitialState = getInitial(fsm);
            return fsm;
        }
        enum hyperLinkType
        {
            Source,
            Target
        }

        /// <summary>
        /// Get all transitions of the desired diagram adjusting the initial and final insertion points using <paramref name="t"/>
        /// </summary>
        /// <param name="model">The model where the diagram is</param>
        /// <param name="t">the transation with hyperlink</param>
        /// <param name="tp">the side where the hyperlink is (source or target)</param>
        /// <returns>a list of the transitions</returns>
        private static List<UmlTransition> GetTransitionsOfDiagram(UmlModel model, ref UmlTransition t, hyperLinkType tp)
        {
            List<UmlTransition> subTransitions;
            UmlElement s;
            String hyperlink = "";
            if (tp == hyperLinkType.Source)
                hyperlink = t.Source.TaggedValues["jude.hyperlink"];
            else
                hyperlink = t.Target.TaggedValues["jude.hyperlink"];

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
                subTransitions.Remove(f);
            }
            else
                if (tp == hyperLinkType.Source)
                    s = subDiagram.UmlObjects.OfType<UmlFinalState>().FirstOrDefault();
                else
                    s = subDiagram.UmlObjects.OfType<UmlInitialState>().FirstOrDefault();

            subTransitions.RemoveAll(x => x.Target is UmlFinalState);
            subTransitions.RemoveAll(x => x.Source is UmlInitialState);
            if (tp == hyperLinkType.Source)
                t.Source = s;
            else
                t.Target = s;

            return subTransitions;
        }

        /// <summary>
        /// Get initial state of FSM
        /// </summary>
        /// <param name="fsm"></param>
        /// <returns></returns>
        private static State getInitial(FiniteStateMachine fsm)
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
        private static void RemoveDecisions(ref UmlActivityDiagram diagram, ref List<UmlTransition> transitions)
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
        private static FiniteStateMachine WipeOutOutermost(UmlActivityDiagram diagram, FiniteStateMachine fsm)
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

        private static State getInitial(UmlActivityDiagram actDiagram, List<State> listState)
        {
            foreach (UmlInitialState initial in actDiagram.UmlObjects.OfType<UmlInitialState>())
            {
                foreach (UmlTransition tr in actDiagram.UmlObjects.OfType<UmlTransition>())
                {
                    if (tr.Source.Id.Equals(initial.Id))
                    {
                        if (tr.Target.GetTaggedValue("jude.hyperlink") == null)
                        {
                            return getState(listState, tr.Target.Name);
                        }
                        else
                        {
                            UmlActivityDiagram activityDiagram = model2.Diagrams
                                .OfType<UmlActivityDiagram>()
                                .Where(x => x.Name == tr.Target.GetTaggedValue("jude.hyperlink"))
                                .FirstOrDefault();
                            State v = getInitial(activityDiagram, listState);
                            return v;
                        }
                    }
                }
            }
            return null;
        }

        private static State getState(List<State> listState, String name)
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

        public static List<Transition> parallelTransitions(UmlTransition t, UmlActivityDiagram actDiagram)
        {
            List<UmlTransition> listUmlTransition = new List<UmlTransition>();
            List<Transition> listTransition = new List<Transition>();
            Transition tran1 = new Transition();
            foreach (UmlTransition tran in actDiagram.UmlObjects.OfType<UmlTransition>())
            {
                if (t.Target.Id.Equals(tran.Source.Id))
                {
                    listUmlTransition.Add(tran);
                }
            }

            Transition tran2 = new Transition();
            tran2.Input = listUmlTransition[0].GetTaggedValue("TDACTION");
            if (listUmlTransition[0].GetTaggedValue("TDPARAMETERS") == null)
            {
                tran2.Output = "";
            }
            else
            {
                tran2.Output = listUmlTransition[0].GetTaggedValue("TDPARAMETERS");
            }
            tran2.SourceState = getState(t.Source.Name, actDiagram, listTransition);
            tran2.TargetState = getState(listUmlTransition[0].Target.Name, actDiagram, listTransition); ;
            listTransition.Add(tran2);

            UmlTransition transition = listUmlTransition[0];
            tran1.Input = transition.GetTaggedValue("TDACTION");
            if (transition.GetTaggedValue("TDPARAMETERS") == null)
            {
                tran1.Output = "";
            }
            else
            {
                tran1.Output = transition.GetTaggedValue("TDPARAMETERS");
            }
            tran1.SourceState = getState(transition.Target.Name, actDiagram, listTransition);
            for (int i = 1; i < listUmlTransition.Count; i++)
            {
                transition = listUmlTransition[i];
                tran1.Input = transition.GetTaggedValue("TDACTION");
                if (transition.GetTaggedValue("TDPARAMETERS") == null)
                {
                    tran1.Output = "";
                }
                else
                {
                    tran1.Output = transition.GetTaggedValue("TDPARAMETERS");
                }
                tran1.TargetState = getState(transition.Target.Name, actDiagram, listTransition);
                listTransition.Add(tran1);
                tran1 = new Transition();
                tran1.SourceState = listTransition[listTransition.Count - 1].TargetState;
            }
            Transition tran3 = new Transition();
            UmlTransition transitionAux = getNextState(listTransition[listTransition.Count - 1].TargetState.Name, actDiagram);
            tran3.Input = transitionAux.GetTaggedValue("TDACTION");
            if (transitionAux.GetTaggedValue("TDPARAMETERS") == null)
            {
                tran3.Output = "";
            }
            else
            {
                tran3.Output = transitionAux.GetTaggedValue("TDPARAMETERS");
            }
            tran3.SourceState = listTransition[listTransition.Count - 1].TargetState;
            tran3.TargetState = getState(transitionAux.Target.Name, actDiagram, listTransition);
            listTransition.Add(tran3);
            return listTransition;
        }

        private static UmlTransition getNextState(String act, UmlActivityDiagram actDiagram)
        {
            foreach (UmlTransition transition in actDiagram.UmlObjects.OfType<UmlTransition>())
            {
                if (transition.Source.Name.Equals(act))
                {
                    foreach (UmlTransition tran in actDiagram.UmlObjects.OfType<UmlTransition>())
                    {
                        if (transition.Target.Id.Equals(tran.Source.Id))
                        {
                            return tran;
                        }
                    }
                }
            }
            return null;
        }

        private static State getState(String name, UmlActivityDiagram actdiagram, List<Transition> transitions)
        {
            foreach (UmlElement act in actdiagram.UmlObjects.OfType<UmlElement>())
            {
                if (act.Name.Equals(name))
                {
                    State sSource = new State();
                    sSource.Name = act.Name;
                    sSource.Id = act.Id;

                    foreach (Transition item in transitions)
                    {
                        if (item.SourceState.Equals(sSource.Name))
                        {
                            Transition t = new Transition();
                            t.SourceState = sSource;
                            State sTarget = new State();
                            UmlElement element = new UmlActionState();
                            element = getTransition(act).Target;
                            sTarget.Id = element.Id;
                            sTarget.Name = element.Name;
                            sSource.Transitions.Add(t);
                        }
                    }
                    return sSource;
                }
            }
            return null;
        }

        private static void transitionOutputOfState(FiniteStateMachine m)
        {
            foreach (State s in m.States)
            {
                List<Transition> listTransition = new List<Transition>();
                listTransition = getTransitionOfState(s, m.Transitions);

                foreach (Transition t in listTransition)
                {
                    s.Transitions.Add(t);
                }
            }
        }

        public static UmlTransition getTransition(UmlElement act)
        {
            foreach (UmlActivityDiagram actDiagram in model2.Diagrams.OfType<UmlActivityDiagram>())
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

        private static bool ContainsInclude(UmlUseCaseDiagram diagram, UmlUseCase useCase)
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

        private static List<Transition> getTransitionOfState(State s, List<Transition> listTransition)
        {
            IEnumerable<Transition> list;
            list = from Transition t in listTransition
                   where s.Id.Equals(t.SourceState.Id)
                   select t;
            return list.ToList();
        }
    }
}
