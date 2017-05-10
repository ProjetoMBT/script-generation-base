using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coc.Modeling.FiniteStateMachine;
using Coc.Data.Interfaces;
using Coc.Data.ControlAndConversionStructures;
using Coc.Modeling.TestPlanStructure;
using Coc.Data.ControlStructure;

namespace Coc.Data.Wpartial
{
    /*
    /// <summary>
    /// <img src="images/Wpartial.PNG"/>
    /// </summary>
    public static class NamespaceDoc
    {
    }*/



    //Classe onde recebe uma FSM e gera sequencia de teste utilizado
    //o metodo Wp
    public class Wp : SequenceGenerator
    {
        //maquina onde será armazenado os estados 
        private FiniteStateMachine fsm = null;
        //lista de sequencia final
        public List<String> ListSequence { get; set; }
        //lista onde será concatenado as sequencias do W da máquina com  as sequencias do StateCover
        public List<List<String>> ListSequenceStateCoverAndWMachine { get; set; }
        // conjunto R
        public List<List<String>> ListSequenceR { get; set; }

        //construtor do metodo Wp
        public Wp(FiniteStateMachine fsm)
        {
            // iniciando a lista  de sequencia
            ListSequence = new List<string>();
            //iniciando a lista sequencia (Sequencia Estado X sequencia Máquina).
            ListSequenceStateCoverAndWMachine = new List<List<String>>();
            //inicializando a lista sequencia R.
            ListSequenceR = new List<List<String>>();
            //guardando uma referencia da máquina.
            this.fsm = fsm;

        }

        public Wp()
        {

            ListSequence = new List<string>();
            //iniciando a lista sequencia (Sequencia Estado X sequencia Máquina).
            ListSequenceStateCoverAndWMachine = new List<List<String>>();
            //inicializando a lista sequencia R.
            ListSequenceR = new List<List<String>>();
        }
        //metodo Wp
        public List<List<String>> MethodWp()
        {
            //adicionando nos estados wi e na máquina os conjunto W.
            #region  primeira fase do metodo Wp
            //metodo que adicina Wi da máquina.
            SetWMachine(fsm);
            //metodo que retorna uma lista de sequencia concatenada entre as sequencia de cada estado com W da maquina.
            //conjunto Q concatena W da FSM.
            ListSequenceStateCoverAndWMachine = concatenatesSequenceStateCoverAndWMachine(fsm);
            //remove os valores préfixados
            removePreFixed(ListSequenceStateCoverAndWMachine);
            #endregion
            #region segunda fase do metodo Wp
            // Lista das sequncias final.
            List<List<String>> rw = new List<List<string>>();
            //R ⊗ W ou R=  conjunto TransitionCover (P) - conjunto StateCover (Q);
            ListSequenceR = TransitionCoverLessStateCover(fsm);
            //método que concatena as sequencias  R com as sequencia Wi de cada estado.
            ConcatenateSequencesRandWStates(ListSequenceR, fsm, rw);
            #endregion
            removePreFixed(rw);
            rw = ListGroup(rw, ListSequenceStateCoverAndWMachine);
            removePreFixed(rw);
            //retona uma lista de sequencia de teste.
            return rw;
        }

        private List<List<string>> ListGroup(List<List<string>> rw, List<List<string>> ListSequenceStateCoverAndWMachine)
        {
            List<List<string>> list = new List<List<string>>();

            foreach (List<String> line in ListSequenceStateCoverAndWMachine)
            {
                list.Add(line);

            }

            foreach (List<String> line in rw)
            {
                list.Add(line);
            }
            return list;
        }


        #region methods private


        /// <summary>
        /// concatena as sequencia R com as sequencia W de cada estado.
        /// </summary>
        /// <param name="ListSequenceR"></param>
        /// <param name="fsm"></param>
        /// <param name="rw"></param>
        private void ConcatenateSequencesRandWStates(List<List<String>> ListSequenceR, FiniteStateMachine fsm, List<List<String>> rw)
        {
            //para cada linha da seuqencia R 
            foreach (List<String> line in ListSequenceR)
            {
                //caminha sobre a maquina com as entradas R par verificar se as sequencia estão correta
                MachineWalkthrough(line, fsm, rw);
            }
        }
        /// <summary>
        /// metodo que caminha sobre a maquina com as sequencias R
        /// </summary>
        /// <param name="seqLine"></param>
        /// <param name="fsm"></param>
        /// <param name="rw"></param>
        private void MachineWalkthrough(List<String> seqLine, FiniteStateMachine fsm, List<List<String>> rw)
        {
            //pe ga o estado inicial da maquina FSM.
            String s = fsm.InitialState.Name;
            //Estado que guardará o ultimo estado visitado
            State lastState = null;
            //para cada sequencia R
            foreach (String seq in seqLine)
            {
                //cria um estado com o nome
                State state = new State(s);
                //pega a transição onde o estado passado por parametro e origem e a sequencia (seq) é a mesma.
                Transition t = GetTransitionStateSource(state, seq, fsm.Transitions)[0];
                //caso a transição não e encontrada  retorna uma excessão.
                if (t == null)
                {
                    throw new System.ArgumentException("Sequence null");
                }
                //pega o target da transição
                s = t.TargetState.Name;
                //guarda o ultimo estado visitado.
                lastState = t.TargetState;
            }
            //pega todos os valores de entrada na transição onde o estado e origem.
            String[] arr = GetAllowedInputs(lastState);
            //para cada entrada da transição
            foreach (String seq in arr)
            {
                //faz uma copia da lista de sequencia.
                List<String> list = CopyList(seqLine);
                //adiciona  na lista de sequencia.
                list.Add(seq);
                //adiciona na lista R
                rw.Add(list);
            }
        }
        /// <summary>
        /// método retorna a diferença entre os conj StateCover e conj TransitionCover.
        /// </summary>
        /// <param name="fsm"></param>
        /// <returns></returns>
        private List<List<String>> TransitionCoverLessStateCover(FiniteStateMachine fsm)
        {
            //lista de sequencia do transitionCover para todos os estados da FSM.
            List<List<String>> listSequenceTransitionCover = SequenceTransitionCover(fsm);
            //lista de sequencia do StateCover para todos os estados da FSM.
            List<List<String>> listStateCover = GetSequenceStateCover(fsm);

            //lista do resultado R =  TransitionCover - StateCover.
            List<List<String>> resultR = new List<List<string>>();

            //para cada sequencia da lista do Transitin Cover.
            for (int i = 0; i < listSequenceTransitionCover.Count; i++)
            {
                //pega uma sequencia na posição i.
                List<String> line = listSequenceTransitionCover[i];
                //concatena as sequencia  .
                String seq = "";
                //para cada linha.
                for (int j = 0; j < line.Count; j++)
                {
                    //concatena as sequencia na posição j.
                    seq += line[j];
                }
                bool contem = false;
                //para cada elemento (sequencia) 
                for (int l = 0; l < listStateCover.Count; l++)
                {
                    //pega a linha na lista na posição l.
                    List<String> line2 = listStateCover[l];
                    //String onde será concatenado as  sequencia de cada posição l.
                    String seq2 = "";
                    //para cada sequencia  da lista de sequencia do transitionCover.
                    for (int k = 0; k < line2.Count; k++)
                    {
                        //pega a sequencia na posição k e concatena.
                        seq2 += line2[k];

                    }
                    if (seq.Equals(seq2) && contem != true)
                    {
                        contem = true;
                    }
                }
                if (!contem)
                {
                    resultR.Add(line);
                }
            }
            //retorna a lista 
            return resultR;
        }
        /// <summary>
        /// metodo que retorna uma lista de sequencia utilizando transitionCover.
        /// </summary>
        /// <param name="fsm"></param>
        /// <returns></returns>
        private List<List<String>> SequenceTransitionCover(FiniteStateMachine fsm)
        {
            fsm = GetTransition(fsm);
            //lista final do transitionCover;.
            List<List<String>> listSequenceTransitionCover = new List<List<String>>();
            //lista com sequencia VAZIA para ser adicionado na lista final.
            List<String> sequenceEmpty = new List<string>();
            //adicona  na lista uma String vazia.
            sequenceEmpty.Add("");
            //adiciona na lista final a lista com a string vazia.
            listSequenceTransitionCover.Add(sequenceEmpty);
            //para cada estado da FSM
            foreach (State state in fsm.States)
            {
                //pega todas as sequencia de entrada que leve até o estado passado por paramentro
                //concatenado com os valores de entradas(s) da transição onde o estado, que é passado por paramentro
                //é origem.
                String[][] arr = TransitionCover(state);
                //para cada sequencia.
                for (int i = 0; i < arr.Length; i++)
                {   //instaciando uma lista onde será adicionado as sequencia do TransitionCover.
                    List<String> listAux = new List<string>();
                    //para cada sequencia na pos (i)
                    for (int j = 0; j < arr[i].Length; j++)
                    {
                        //adicionan o elemento na pos i e j.
                        listAux.Add(arr[i][j]);
                    }
                    //adiciona as sequencia na lista final do TrnasitionCover.
                    listSequenceTransitionCover.Add(listAux);
                }
            }
            //retorna a lista final com todas as sequencia da TransitionCover.
            return listSequenceTransitionCover;
        }

        private FiniteStateMachine GetTransition(FiniteStateMachine fsm)
        {
            for (int i = 0; i < fsm.Transitions.Count; i++)
            {
                Transition t = fsm.Transitions[i];
                if (t.Output.Equals("Falha", StringComparison.CurrentCultureIgnoreCase))
                {
                    fsm.Transitions.Remove(t);
                    i--;
                }
            }
            return fsm;
        }
        /// <summary>
        /// metodo que concatena as sequencia do StateCover com as sequencia W da máquina.
        /// </summary>
        /// <param name="fsm"></param>
        /// <returns></returns>
        private List<List<String>> concatenatesSequenceStateCoverAndWMachine(FiniteStateMachine fsm)
        {
            //pega a sequencia do StateCover da máquina
            List<List<String>> list = GetSequenceStateCover(fsm);
            //concatena as sequencia StateCover com W da máquina
            List<List<String>> listSequence = ConcatenateSequences(list, fsm.WiSet);
            //retona a lista com as sequencia gerada do metodo acima
            return listSequence;
        }
        /// <summary>
        /// metodo que recebe duas lista de sequencia e concatena
        /// </summary>
        /// <param name="listStateCover"></param>
        /// <param name="listFsm"></param>
        /// <returns></returns>
        private List<List<String>> ConcatenateSequences(List<List<String>> listStateCover, List<List<String>> listFsm)
        {
            //lista final da concatenação das sequencia
            List<List<String>> listConcatenate = new List<List<string>>();
            //para cada sequencia da lista
            foreach (List<String> item in listStateCover)
            {
                //cria uma lista de  sequencia
                List<String> seq = new List<String>();
                //para cada sequencia
                foreach (String s in item)
                {
                    //se a sequncia for diferente de vazio
                    if (!s.Equals(""))
                    {
                        //adiona na lista de sequencia
                        seq.Add(s);
                    }
                }
                //para cada sequencia da lista da FSM
                foreach (List<String> line in listFsm)
                {
                    //cria uma copia da lista
                    List<String> listAux = CopyList(seq);
                    //para cada lista de sequencia
                    foreach (String s1 in line)
                    {
                        //adiciona na lista
                        listAux.Add(s1);
                    }
                    //concatena a sequncia na lista.
                    listConcatenate.Add(listAux);
                }
            }
            //retorna a sequencia final da concatenação da sequencia STATECOVER e W da máquina
            return listConcatenate;
        }
        /// <summary>
        /// metodo para criar uma copia de uma lista.
        /// </summary>
        /// <param name="seq"></param>
        /// <returns></returns>
        private List<String> CopyList(List<String> seq)
        {
            //cria uma lista de string
            List<String> list = new List<String>();
            //para cada sequncia na lista
            foreach (String item in seq)
            {
                //adiciona na lista
                list.Add(item);
            }
            //retorna a lista
            return list;
        }
        /// <summary>
        /// metodo que gera as sequencia do STATECOVER
        /// </summary>
        /// <param name="fsm"></param>
        /// <returns></returns>
        private List<List<String>> GetSequenceStateCover(FiniteStateMachine fsm)
        {
            //cria um a lista de string
            List<List<String>> listStateCover = new List<List<String>>();
            //cria uma lista de string par o conjunto VAZIO do primeiro estado
            List<String> stateCoverInitial = new List<string>();
            //adiciona um string vazia na lista
            stateCoverInitial.Add("");
            //adicona na lista do StateCover.
            listStateCover.Add(stateCoverInitial);
            //para ccada State da FSM
            foreach (State state in fsm.States)
            {
                //cria uma lista de string
                List<String> list = new List<string>();
                //busca o preambulo do estado passado por paramentro.
                String[] arr = Preambulo(state);
                //para cada sequencia 
                for (int i = 0; i < arr.Length; i++)
                {
                    //adiciona na lista 
                    list.Add(arr[i]);
                }
                //casso a lista for diferente de 0.
                if (list.Count != 0)
                {
                    //adicina na lista
                    listStateCover.Add(list);
                }
            }
            //retona a lista do STATECOVER.
            return listStateCover;
        }
        /// <summary>
        /// Este método obtem uma lista de pares de estado e uma lista com elementos do conjunto W.
        /// </summary>
        /// <param name="fsm"></param>
        private void SetWMachine(FiniteStateMachine fsm)
        {
            //lista de pares de estado.
            List<StatePair> listStatePair = new List<StatePair>();
            //método que obtem os pares de estados.
            GetListTransitionWi(fsm, listStatePair);
            //metodo que remove os  pares iguais.
            removeEqualsState(listStatePair);
            //para cada par de estado da FSM.
            foreach (StatePair statePair in listStatePair)
            {
                //obtem o conjunto W de cada par de estado.
                GetWiStatePair(statePair, fsm);

            }
            //remove as sequência de entradas equivalentes.
            RemoveEqualsSequence(fsm);
        }
        /// <summary>
        /// método que remove sequencia repitidas do conjunto W da Máquina
        /// </summary>
        /// <param name="fsm"></param>
        private void RemoveEqualsSequence(FiniteStateMachine fsm)
        {

            for (int i = 0; i < fsm.WiSet.Count; i++)
            {
                List<String> line = fsm.WiSet[i];
                String column = "";
                for (int j = 0; j < line.Count; j++)
                {
                    column += line[j];
                }
                for (int k = i + 1; k < fsm.WiSet.Count; k++)
                {
                    List<String> line2 = fsm.WiSet[k];
                    String column2 = "";
                    for (int l = 0; l < line2.Count; l++)
                    {
                        column2 += line2[l];
                    }
                    if (column.Equals(column2))
                    {
                        line2.Remove(column2);
                        k--;
                        if (line2.Count == 0)
                        {
                            fsm.WiSet.Remove(line2);
                        }
                    }
                }

            }
        }
        /// <summary>
        /// remove duplicate String
        /// </summary>
        /// <param name="rw"></param>
        private void removePreFixed(List<List<string>> rw)
        {

            for (int i = 0; i < rw.Count; i++)
            {
                List<String> line = rw[i];
                for (int j = i + 1; j < rw.Count; j++)
                {
                    List<String> line2 = rw[j];
                    if (line.Count <= line2.Count)
                    {
                        String lineAux1 = "";
                        String lineAux2 = "";
                        for (int k = 0; k < line.Count; k++)
                        {
                            lineAux1 += line[k];
                            lineAux2 += line2[k];
                        }
                        if (lineAux1.Equals(lineAux2))
                        {
                            rw.RemoveAt(i);
                            i = 0;
                            line = rw[i];
                        }
                    }
                }
            }
        }
        /// <summary>
        /// método que popula a lista de SetW da FSM com conjunto W.
        /// </summary>
        /// <param name="statePair"></param>
        /// <param name="fsm"></param>
        private void GetWiStatePair(StatePair statePair, FiniteStateMachine fsm)
        {
            Transition t = GetListTransitionOutputOfState(statePair, fsm);
            if (t != null)
            {
                List<String> list = new List<string>();
                list.Add(t.Input);
                fsm.WiSet.Add(list);
            }
            else
            {
                List<String> listSeq = new List<string>();
                GetListOutoutStatePair(listSeq, statePair, fsm);
                List<String> list = new List<string>();
                foreach (String i in listSeq)
                {
                    list.Add(i);
                }
                fsm.WiSet.Add(list);
            }
        }
        /// <summary>
        /// metodo recursivo que obtem uma lista de entradas onde 
        /// obtem o conjunto W da FSM.
        /// </summary>
        /// <param name="listSeq"></param>
        /// <param name="statePair"></param>
        /// <param name="fsm"></param>
        /// <returns></returns>
        private List<string> GetListOutoutStatePair(List<string> listSeq, StatePair statePair, FiniteStateMachine fsm)
        {
            String[][] tcSi = TransitionCover(statePair.Si);
            String[][] tcSj = TransitionCover(statePair.Sj);
            String inputEquals = "";
            inputEquals = GetInputEquals(statePair.Si, statePair.Sj);

            if (!inputEquals.Equals(""))
            {

                Transition t1 = GetTransitionStateSource(statePair.Si, inputEquals, fsm.Transitions)[0];
                Transition t2 = GetTransitionStateSource(statePair.Sj, inputEquals, fsm.Transitions)[0];

                if (!t1.Output.Equals(t2.Output))
                {
                    listSeq.Add(inputEquals);
                    return listSeq;
                }
                else
                {
                    StatePair statPairAux = new StatePair();
                    State s = new State(t1.TargetState.Name);
                    State t = new State(t2.TargetState.Name);
                    statPairAux.Si = s;
                    statPairAux.Sj = t;
                    statPairAux.Si.Name = s.Name;
                    statPairAux.Sj.Name = t.Name;
                    GetListOutoutStatePair(listSeq, statPairAux, fsm);
                }
            }
            else
            {
                listSeq.Add(GetAllowedInputs(statePair.Si)[0]);
                return listSeq;
            }
            return listSeq;
        }
        /// <summary>
        /// metodo que verifica se as entradas da transição onde o estados passado por parament
        /// são iguais.
        /// </summary>
        /// <param name="si"></param>
        /// <param name="sj"></param>
        /// <returns></returns>
        private string GetInputEquals(State si, State sj)
        {
            String[] inputsSi = GetAllowedInputs(si);
            String[] inputsSj = GetAllowedInputs(sj);

            for (int i = 0; i < inputsSi.Length; i++)
            {
                String inputSi = inputsSi[i];
                for (int j = 0; j < inputsSj.Length; j++)
                {
                    String inputSj = inputsSj[j];
                    if (inputSi.Equals(inputSj))
                    {
                        return inputSi;
                    }
                }
            }
            return "";
        }
        /// <summary>
        /// método que retorna todas as transição onde os pares de estados são
        /// origem na transição na lista de de transição da FSM.
        /// </summary>
        /// <param name="statePair"></param>
        /// <param name="fsm"></param>
        /// <returns></returns>
        private Transition GetListTransitionOutputOfState(StatePair statePair, FiniteStateMachine fsm)
        {
            List<Transition> listTtransitionSource = new List<Transition>();
            List<Transition> listTtransitionTarget = new List<Transition>();
            foreach (Transition transition in fsm.Transitions)
            {
                if (statePair.Si.Name.Equals(transition.SourceState.Name))
                {
                    listTtransitionSource.Add(transition);
                }
                if (statePair.Sj.Name.Equals(transition.SourceState.Name))
                {
                    listTtransitionTarget.Add(transition);
                }
            }
            foreach (var t1 in listTtransitionSource)
            {
                foreach (var t2 in listTtransitionTarget)
                {
                    if (t1.Input.Equals(t2.Input) && !t1.Output.Equals(t2.Output))
                    {
                        return t1;
                    }
                }
            }
            return null;
        }

        private void GetListTransitionWi(FiniteStateMachine fsm, List<StatePair> listStatePair)
        {
            for (int i = 0; i < fsm.States.Count; i++)
            {
                State s1 = fsm.States[i];
                for (int j = i + 1; j < fsm.States.Count; j++)
                {
                    State s2 = fsm.States[j];
                    List<Transition> list = GetTransitionStateSource(s1, fsm.Transitions);
                    foreach (Transition t in list)
                    {
                        StatePair statePair = new StatePair();
                        statePair.Si = s1;
                        statePair.Sj = s2;
                        listStatePair.Add(statePair);
                    }
                }
            }
        }
        /// <summary>
        /// remove os pares de estados iguais.
        /// </summary>
        /// <param name="listStatePair"></param>
        private void removeEqualsState(List<StatePair> listStatePair)
        {
            for (int i = 0; i < listStatePair.Count; i++)
            {
                StatePair s1 = listStatePair[i];
                for (int j = i + 1; j < listStatePair.Count; j++)
                {
                    StatePair s2 = listStatePair[j];
                    if (IsEquals(s1, s2))
                    {
                        listStatePair.Remove(s2);
                        j--;
                    }
                }

            }
        }
        /// <summary>
        /// método que verifica se os pares de estados são iguais.
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        private bool IsEquals(StatePair s1, StatePair s2)
        {
            if (s1.Si.Name.Equals(s2.Si.Name) && s1.Sj.Name.Equals(s2.Sj.Name))
            {
                return true;
            }
            if (s1.Si.Name.Equals(s1.Sj.Name))
            {
                return true;
            }
            if (s2.Si.Name.Equals(s2.Sj.Name))
            {
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// metodo que retorna uma lista de transição 
        /// da FSM onde o estado passado por parametro e orgim na transição.
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        private List<Transition> GetTransitionStateSource(State s1, List<Transition> list)
        {
            List<Transition> listTransition = new List<Transition>();
            foreach (Transition t in list)
            {
                if (t.SourceState.Name.Equals(s1.Name))
                {
                    listTransition.Add(t);
                }
            }
            return listTransition;
        }

        /// <summary>
        /// método que retona uma lista de transição onde
        /// o estado  for igual ao estado de origem na transição
        /// e o input for igual ao input da transição.
        /// 
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="input"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        private List<Transition> GetTransitionStateSource(State s1, String input, List<Transition> list)
        {
            List<Transition> listTransition = new List<Transition>();
            foreach (Transition t in list)
            {
                if (t.SourceState.Name.Equals(s1.Name) && t.Input.Equals(input))
                {
                    listTransition.Add(t);
                }
            }
            return listTransition;
        }
        /// <summary>
        /// Shortcut for StateCover(State s, List-of-State visited) method.
        /// </summary>
        public String[] Preambulo(State s)
        {
            List<State> visitedStates = new List<State>();
            // System.Diagnostics.Debug.WriteLineIf(debug, "Strinting in " + s.Name);
            visitedStates.Add(s); //cannot walk through S
            return StateCover(s, visitedStates);
        }
        /// <summary>
        /// Gets a preamble of a given state S.
        /// </summary>
        private String[] StateCover(State s, List<State> visited)
        {
            //Initial State´s preamble is EPSILON
            if (s.Name.Equals(fsm.InitialState.Name))
                return new String[] { };

            string[] shortestPreamble = null;
            string[] currentPreamble = null;
            string lastInput = "";

            //Get ancestors´ preambles
            IEnumerable<Transition> filteredTransitions = fsm.Transitions.Where(x => x.TargetState.Name.Equals(s.Name));
            List<Transition> initialTransitions = fsm.Transitions.Where(x => x.SourceState.Name.Equals(fsm.InitialState.Name) && x.TargetState.Name.Equals(s.Name)).ToList<Transition>();

            if (initialTransitions.Count > 0)
            {
                currentPreamble = new string[1];
                lastInput = initialTransitions[0].Input;
                currentPreamble[0] = lastInput;
                return currentPreamble;
            }
            else
            {
                foreach (Transition transition in filteredTransitions)
                {

                    if (!(visited.Contains(transition.SourceState)) && !(transition.SourceState.Name.Equals(transition.TargetState.Name)))
                    {
                        visited.Add(transition.SourceState);
                        currentPreamble = this.StateCover(transition.SourceState, visited);
                        if (currentPreamble != null && shortestPreamble == null)
                        {
                            shortestPreamble = currentPreamble;
                            lastInput = transition.Input;
                        }
                    }
                }
            }
            if (shortestPreamble == null)
                return null;

            //Creates a new preamble with size + 1
            string[] preamble = new string[shortestPreamble.Count() + 1];

            //Copy antecessor preamble to the new preable
            shortestPreamble.CopyTo(preamble, 0);

            //adds current transition input to current state S
            preamble[preamble.Count() - 1] = lastInput;

            return preamble;
        }
        /// <summary>
        /// Gets the transitio cover of given state S.
        /// </summary>
        private String[][] TransitionCover(State s)
        {
            //Concatenate preamble with allowed inputs.
            // debug = s.Name.Equals("4");
            String[] preamble = this.Preambulo(s);
            String[] allowedInputs = this.GetAllowedInputs(s);

            String[][] transitionCover = new String[allowedInputs.Length][];

            for (int i = 0; i < allowedInputs.Length; i++)
            {
                string currentInput = allowedInputs[i];
                transitionCover[i] = new String[preamble.Length + 1];

                for (int j = 0; j < preamble.Length; j++)
                    transitionCover[i][j] = preamble[j];

                transitionCover[i][preamble.Length] = currentInput;
            }

            return transitionCover;
        }
        /// <summary>
        /// Gets the allowed inputs list of a given state S.
        /// </summary>
        private String[] GetAllowedInputs(State s)
        {
            IEnumerable<string> inputs = from Transition transition in fsm.Transitions
                                         where transition.SourceState.Equals(s)
                                         select transition.Input;
            return inputs.Distinct().ToArray();
        }
        #endregion

        // public override List<GeneralUseStructure> GenerateSequence(GeneralUseStructure model, ref int tcCount, StructureType type)
        public override List<GeneralUseStructure> GenerateSequence(List<GeneralUseStructure> listGeneralStructure, ref int tcCount, StructureType type)
        {
            List<GeneralUseStructure> listScript = new List<GeneralUseStructure>();

            List<GeneralUseStructure> listSequenceGenerationStructure = base.ConvertStructure(listGeneralStructure, type);

            List<TestStep> listTestStep = new List<TestStep>();

            foreach (GeneralUseStructure sgs in listSequenceGenerationStructure)
            {
                this.fsm = (FiniteStateMachine)sgs;


            }

            return listScript;
        }
    }
}
