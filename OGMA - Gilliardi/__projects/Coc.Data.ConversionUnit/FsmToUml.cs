using System;
using System.Collections.Generic;
using System.Linq;
using Coc.Modeling.FiniteStateMachine;
using Coc.Modeling.Uml;
using Coc.Data.ControlAndConversionStructures;
using Coc.Data.ControlStructure;

namespace Coc.Data.ConversionUnit
{
    public class FsmToUml: ModelingStructureConverter
    {
        public FsmToUml(){}
        #region Public Methods
        public UmlModel TransformToUml(FiniteStateMachine[] fsms)
        {
            UmlModel model = new UmlModel("model");
            UmlUseCaseDiagram ucDiagram = new UmlUseCaseDiagram();
            ucDiagram.Id = Guid.NewGuid().ToString();
            ucDiagram.Name = "useCase diagram";
            UmlActor user = new UmlActor();
            user.Name = "user";
            user.Id = Guid.NewGuid().ToString();
            ucDiagram.UmlObjects.Add(user);
            State s = new State();

            foreach (FiniteStateMachine fsm in fsms)
            {
                UmlUseCase useCase = new UmlUseCase();
                useCase.Name = fsm.Name;
                useCase.Id = Guid.NewGuid().ToString();
                UmlAssociation association = new UmlAssociation();
                association.Id = Guid.NewGuid().ToString();
                association.End1 = user;
                association.End2 = useCase;

                ucDiagram.UmlObjects.Add(useCase);
                ucDiagram.UmlObjects.Add(association);

                UmlActivityDiagram actDiagram = new UmlActivityDiagram(fsm.Name);

                Transition initialTransition = new Transition();
                State initialState = new State();
                initialTransition = (from t in fsm.Transitions
                                     where fsm.Transitions.Count(x => x.TargetState.Equals(t.SourceState)) == 0
                                     select t).First();
                UmlInitialState initial = new UmlInitialState();
                UmlActionState initialTarget = new UmlActionState();
                UmlTransition transInitial = new UmlTransition();
                initial.Name = initialTransition.SourceState.Name;
                initial.Id = initialTransition.SourceState.Id;
                initialTarget.Name = initialTransition.TargetState.Name;
                initialTarget.Id = initialTransition.TargetState.Id;
                transInitial.Source = initial;
                transInitial.Target = initialTarget;

                actDiagram.UmlObjects.Add(initialTarget);
                actDiagram.UmlObjects.Add(initial);
                actDiagram.UmlObjects.Add(transInitial);


                foreach (State state in fsm.States)
                {
                    List<Transition> listTransition = new List<Transition>();
                    listTransition = GetTransitions(state, fsm);
                    if (listTransition.Count < 2 && listTransition.Count != 0)
                    {
                        Transition transitionFSM = listTransition[0];
                        UmlActionState actionState_Source = new UmlActionState();
                        actionState_Source.Name = transitionFSM.SourceState.Name;
                        actionState_Source.Id = transitionFSM.SourceState.Id;
                        UmlActionState actionState_Target1 = new UmlActionState();
                        actionState_Target1.Name = transitionFSM.TargetState.Name;
                        actionState_Target1.Id = transitionFSM.TargetState.Id;
                        UmlTransition trans2 = new UmlTransition();

                        UmlTransition trans1 = new UmlTransition();
                        trans1.Source = actionState_Source;
                        trans1.Target = actionState_Target1;
                        trans1.SetTaggedValue("TDACTION", transitionFSM.Input);
                        trans1.SetTaggedValue("TDEXPECTEDRESULT", transitionFSM.Output);
                        if (!actDiagram.UmlObjects.Contains(trans1))
                        {
                            actDiagram.UmlObjects.Add(trans1);
                        }
                        if (!actDiagram.UmlObjects.Contains(actionState_Source))
                        {
                            actDiagram.UmlObjects.Add(actionState_Source);
                        }
                        if (!actDiagram.UmlObjects.Contains(actionState_Target1))
                        {
                            actDiagram.UmlObjects.Add(actionState_Target1);
                        }
                    }
                    else
                    {
                        //criar decision
                        bool createDecision = false;
                        UmlDecision decision = new UmlDecision();
                        foreach (Transition transition in listTransition)
                        {
                            if (createDecision == false)
                            {
                                actDiagram.UmlObjects.Add(decision);
                                createDecision = true;
                            }

                            UmlActionState source = new UmlActionState();
                            UmlActionState target = new UmlActionState();
                            UmlTransition transitionBeforeDecision = new UmlTransition();
                            UmlTransition transitionAfterDecision = new UmlTransition();
                            source.Name = transition.SourceState.Name;
                            source.Id = transition.SourceState.Id;
                            target.Name = transition.TargetState.Name;
                            target.Id = transition.TargetState.Id;
                            transitionBeforeDecision.Source = source;
                            transitionBeforeDecision.Target = decision;
                            transitionAfterDecision.Source = decision;
                            transitionAfterDecision.Target = target;
                            transitionAfterDecision.SetTaggedValue("TDACTION", transition.Input);
                            transitionAfterDecision.SetTaggedValue("TDEXPECTEDRESULT", transition.Output);
                            actDiagram.UmlObjects.Add(transitionBeforeDecision);
                            actDiagram.UmlObjects.Add(transitionAfterDecision);
                            actDiagram.UmlObjects.Add(source);
                            actDiagram.UmlObjects.Add(target);
                        }
                    }
                }

                State finalState = new State();
                Transition aux = new Transition();

                finalState = (from t in fsm.Transitions
                              where fsm.Transitions.Count(x => x.SourceState.Equals(t.TargetState)) == 0
                              select t.TargetState).First();

                UmlActionState finalSource = new UmlActionState();
                finalSource.Name = finalState.Name;
                finalSource.Id = finalState.Id;
                UmlFinalState final = new UmlFinalState();
                final.Name = "finalNode";
                final.Id = Guid.NewGuid().ToString();
                UmlTransition transFinal = new UmlTransition();
                transFinal.Source = finalSource;
                transFinal.Target = final;
                
                if (!actDiagram.UmlObjects.Contains(finalSource))
                {
                    actDiagram.UmlObjects.Add(finalSource);
                }

                actDiagram.UmlObjects.Add(final);
                actDiagram.UmlObjects.Add(transFinal);
                model.AddDiagram(actDiagram);
                model.AddDiagram(ucDiagram);
            }

            return model;
        }
        #endregion

        #region Private Methods
        private List<Transition> GetTransitions(State s, FiniteStateMachine fsm)
        {
            List<Transition> listTransition = new List<Transition>();

            foreach (Transition transition in fsm.Transitions)
            {
                if (transition.SourceState.Id.Equals(s.Id))
                {
                    listTransition.Add(transition);
                }
            }
            return listTransition;
        }
        #endregion

        public List<GeneralUseStructure> Converter(List<GeneralUseStructure> listModel,StructureType type)
        {
            throw new NotImplementedException();
        }
    }
}
