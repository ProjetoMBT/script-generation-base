using System;
using System.Collections.Generic;
using Coc.Modeling.FiniteStateMachine;
using Coc.Data.VFSMachine;


namespace Coc.Data.ConversionUnit.TesteMachineFinite.Negocio
{
    public class ConvertToFSM
    {
        //metodo que chama o metodo converte uma VFSM em FSM.
        public FiniteStateMachine convertToFSM(VFSM vfsm)
        {
            FiniteStateMachine fsm = new FiniteStateMachine();
            fsm.Name = vfsm.Name;
            addInitialStateVFSMToFSM(vfsm, fsm);
            convertFSM(vfsm.StateInitial, fsm, vfsm);
            return fsm;
        }
        //metodo que procura o estado inicial da VFSM  e converte as informação para estado da FSM.
        private void addInitialStateVFSMToFSM(VFSM vfsm, FiniteStateMachine fsm)
        {
            State stateInitial = new State();
            TransitionVFSM tVFSM = GetStateInitialVFSM(vfsm.StateInitial, vfsm);
            String name = vfsm.StateInitial.Name;
            foreach (Variable v in tVFSM.ListGuardian)
            {
                name += v.Condition;
            }
            stateInitial.Name = name;
            stateInitial.Id = vfsm.StateInitial.Id;
            fsm.InitialState = stateInitial;
        }
        //metodo que retorna a transição do estado inicial da VFSM.
        private TransitionVFSM GetStateInitialVFSM(State state, VFSM vfsm)
        {
            foreach (TransitionVFSM t in vfsm.ListTransition)
            {
                if (t.Source.Name.Equals(state.Name) && ValidaListGuardian(t.ListGuardian, t.ListGuardian, vfsm))
                {
                    return t;
                }
            }
            return null;
        }


        //metodo recursivo que converte as VFSM para FSM.
        private void convertFSM(State state, FiniteStateMachine fsm, VFSM vfsm)
        {
            List<TransitionVFSM> ListTranitionVFSM = GetTransition(state, vfsm);
            if (ListTranitionVFSM != null)
            {
                foreach (TransitionVFSM tVFSM in ListTranitionVFSM)
                {
                    Transition t = ConvertToTransitionFSM(tVFSM, vfsm);
                    addInfoFSM(t, fsm);
                    convertFSM(tVFSM.Target, fsm, vfsm);
                }

            }
        }
        //adiciona as informação da transição na FSM.
        private void addInfoFSM(Transition t, FiniteStateMachine fsm)
        {
            fsm.Transitions.Add(t);
            if (!fsm.States.Contains(t.SourceState))
            {
                fsm.States.Add(t.SourceState);
            }
            if (!fsm.InputAlphabet.Contains(t.Input))
            {
                fsm.InputAlphabet.Add(t.Input);
            }
            if (!fsm.OutputAlphabet.Contains(t.Output))
            {
                fsm.OutputAlphabet.Add(t.Output);
            }
        }
        //meetodo que converte uma transição de uma VFSm para uma transição de uma FSM.
        private Transition ConvertToTransitionFSM(TransitionVFSM tVFSM, VFSM vfsm)
        {
            Transition t = new Transition();
            t.Input = tVFSM.Input;
            t.Output = tVFSM.Output;

            State sSource = new State();
            sSource.Name = Concatenate(vfsm.ListOfGuardian, tVFSM.Source);
            //sSource.Id = t.SourceState.Id;

            UpdateVariableCurrentVFSM(vfsm, tVFSM.ListNewGuardian);
            State Starget = new State();
            Starget.Name = Concatenate(tVFSM.ListNewGuardian, tVFSM.Target);
            //Starget.Id = t.TargetState.Id;
            t.SourceState = sSource;
            t.TargetState = Starget;


            return t;
        }
        //metodo que concatena sa informações da transição de uma VFSM.
        private string Concatenate(List<Variable> list, State state)
        {
            String value = state.Name;
            foreach (Variable v in list)
            {
                value += v.Condition;
            }
            return value;
        }



        //retorna a transição com o mesmo lista de guardian.
        private List<TransitionVFSM> GetTransition(State state, VFSM vfsm)
        {
            List<TransitionVFSM> listTransitionVFSM = new List<TransitionVFSM>();
            foreach (TransitionVFSM t in vfsm.ListTransition)
            {
                if (t.Source.Name.Equals(state.Name) && ValidaListGuardian(t.ListGuardian, t.ListNewGuardian, vfsm))
                {
                    if (!t.Isvisited)
                    {
                        t.Isvisited = true;
                        listTransitionVFSM.Add(t);
                        // return ;
                    }
                }
            }
            return listTransitionVFSM;
        }
        //valida se todas os guardian são iguais as variaveis corrente da VFSM.
        private bool ValidaListGuardian(List<Variable> listOfTransition, List<Variable> ListNewGuardian, VFSM vfsm)
        {
            try
            {
                for (int i = 0; i < vfsm.ListOfGuardian.Count; i++)
                {
                    if (!vfsm.ListOfGuardian[i].Condition.Equals(listOfTransition[i].Condition))
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        //atualiza as informções do guardian da VFSM.
        private void UpdateVariableCurrentVFSM(VFSM vfsm, List<Variable> listOfTransition)
        {
            vfsm.ListOfGuardian = new List<Variable>();
            foreach (Variable v in listOfTransition)
            {
                vfsm.ListOfGuardian.Add(v);
            }
        }
        /// <summary>
        /// Caso a máquina(FSM) Não estejas totalmente especificado
        /// este método faz que a mpaquina fique totalemte especificado.
        /// </summary>
        /// <param name="fsm"></param>
        public void FullySpecified(FiniteStateMachine fsm)
        {

            foreach (State s in fsm.States)
            {
                List<Transition> listTransition = GetTransitionFSM(s, fsm);
                List<String> listInput = ContainsInput(listTransition, fsm.InputAlphabet);
                foreach (String input in listInput)
                {
                    Transition tError = new Transition(listTransition[0].SourceState, listTransition[0].SourceState, input, "Falha");
                    fsm.AddTransition(tError);
                }


            }

        }

        private List<string> ContainsInput(List<Transition> listTransition, List<string> listInput)
        {
            List<string> listNewInput = new List<string>();

            foreach (String input in listInput)
            {
                bool contem = false;
                foreach (Transition t in listTransition)
                {
                    if (!contem && t.Input.Equals(input))
                    {
                        contem = true;

                    }
                }
                if (!contem)
                {
                    listNewInput.Add(input);
                }
            }
            return listNewInput;
        }



        private List<Transition> GetTransitionFSM(State s, FiniteStateMachine fsm)
        {
            List<Transition> listTransition = new List<Transition>();
            foreach (Transition t in fsm.Transitions)
            {
                if (t.SourceState.Name.Equals(s.Name))
                {
                    listTransition.Add(t);
                }
            }
            return listTransition;
        }


    }
}
