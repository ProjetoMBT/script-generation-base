using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Xml.Serialization;
using System.IO;
using System.Web;
using Coc.Data.ControlAndConversionStructures;

namespace Coc.Modeling.FiniteStateMachine
{
    /*
    /// <summary>
    /// <img src="images/FiniteStateMachine.PNG"/>
    /// </summary>
    public static class NamespaceDoc
    {
    }*/



    /// <summary>
    /// Represents a finite state machine model.
    /// </summary>
    [Serializable]
    public class FiniteStateMachine : GeneralUseStructure
    {
        /// <summary>
        /// EPSILON constant. Denotes empty sets.
        /// </summary>
        public static string EPSILON = new String('\u0190', 1);
        private List<State> finals;

        /// <summary>
        /// Input alphabet property.
        /// </summary>
        [XmlElement()]
        public List<string> InputAlphabet
        {
            get;
            set;
        }

        [XmlElement()]
        public string NameUseCase
        {
            get;
            set;
        }
        /// <summary>
        /// Tag Finite State Machine.
        /// </summary>
        public Dictionary<String, String> TaggedValues { get;  set; }


        /// <summary>
        /// Output alphabet property.
        /// </summary>
        [XmlElement()]
        public List<string> OutputAlphabet
        {
            get;
            set;
        }
        /// <summary>
        /// States property.
        /// </summary>
        [XmlElement()]
        public List<State> States
        {
            get;
            set;
        }
        /// <summary>
        /// Defines a name to the instance.
        /// </summary>
        [XmlAttribute()]
        public String Name
        {
            get;
            set;
        }
        /// <summary>
        /// Initial State.
        /// </summary>
        [XmlElement()]
        public State InitialState
        {
            get;
            set;
        }
        /// <summary>
        /// Transitions.
        /// </summary>
        [XmlElement()]
        public List<Transition> Transitions
        {
            get;
            set;
        }
        /// <summary>
        /// Set Wi.
        /// </summary>
        [XmlElement()]
        public List<List<String>> WiSet
        {
            get;
            set;
        }


        /// <summary>
        /// Default constructor
        /// </summary>
        public FiniteStateMachine(String name)
        {
            this.Name = name;
            this.States = new List<State>();
            this.Transitions = new List<Transition>();
            this.InputAlphabet = new List<string>();
            this.OutputAlphabet = new List<string>();
            this.finals = new List<State>();
            TaggedValues = new Dictionary<string, string>();
            WiSet = new List<List<string>>();
        }
        /// <summary>
        /// Parameterless constructor. Used by serializer.
        /// </summary>
        public FiniteStateMachine()
            : this("")
            
        {
            /*
            this.Name = "";
            this.States = new List<State>();
            this.Transitions = new List<Transition>();
            this.InputAlphabet = new List<string>();
            this.OutputAlphabet = new List<string>();
            //*/
        }

        /// <summary>
        /// Adds a new transition to fsm transitions data.
        /// </summary>
        /// <param name="sourceStateId">Id of state from where the transition comes. That state must be inside fsm, otherwise a exception shall be thrown.</param>
        /// <param name="targetStateId">Id of state to where the transition goes. That state must be inside fsm, otherwise a exception shall be thrown.</param>
        /// <param name="inputData">Input data needed to walk throught the transition. That data will be added to input alphabet. If null, a exception shall be thrown.</param>
        /// <param name="outputData">Output data returned when walked throught the transition. That data will be added to output alphabet.If null, a exception shall be thrown. </param>
        public Boolean AddTransition(String sourceStateName, String targetStateName, string inputData, string outputData, bool create = false)
        {
            State source = this.GetStateByName(sourceStateName);
            State target = this.GetStateByName(targetStateName);

            if (source == null)
                source = new State(sourceStateName);

            if (target == null)
                target = new State(targetStateName);

            //updates dictinaries.
            AddInput(inputData);
            AddOutput(outputData);

            //Adds transition to transition list.
            Transition transition = new Transition(source, target, inputData, outputData);
            return AddTransition(transition);
        }

        /// <summary>
        /// Add a input data to input alphabet.
        /// </summary>
        /// <param name="inputData"></param>
        public Boolean AddInput(string inputData)
        {
            if (!InputAlphabet.Contains(inputData))
            {
                InputAlphabet.Add(inputData);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds a output to output data.
        /// </summary>
        /// <param name="outputData"></param>
        public Boolean AddOutput(string outputData)
        {
            if (!OutputAlphabet.Contains(outputData))
            {
                OutputAlphabet.Add(outputData);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds a state to the state machine. Make sure that the added state has one or more trasitions with it.
        /// </summary>
        /// <param name="state">A state reference. States are identified by their names - if the machine has a state with the same name the new state wont be added.</param>
        /// <returns>Returns true if the state were added to the state list. Returns false if the machine already has the state in the list.</returns>
        public Boolean AddState(State state)
        {
            //checks if this state is already in state list.
            IEnumerable<State> stateList = from s in this.States
                                           where s.Equals(state)
                                           select s;

            //if not exists a state with the same name (same name, same state), adds it.
            if (stateList.Count() == 0)
            {
                this.States.Add(state);
                return true;
            }

            //state found in list. Abort.
            return false;
        }

        /// <summary>
        /// Cleans Input and Output dictionaries generating data from current transition list.
        /// USE WITH CARE.
        /// </summary>
        public void RefreshData()
        {
            //cleans dictionaries.
            this.InputAlphabet = new List<string>();
            this.OutputAlphabet = new List<string>();

            //Adds Input/Output to lists
            foreach (Transition t in this.Transitions)
            {
                if (!InputAlphabet.Contains(t.Input))
                    AddInput(t.Input);

                if (!OutputAlphabet.Contains(t.Output))
                    AddOutput(t.Output);

                if (!States.Contains(t.SourceState))
                    States.Add(t.SourceState);

                if (!States.Contains(t.TargetState))
                    States.Add(t.TargetState);
            }
        }

        /// <summary>
        /// Adds a existing transition to the machine's transition list
        /// </summary>
        public Boolean AddTransition(Transition transition)
        {
            //checks if the transitions list already has the transition.
            foreach (Transition t in Transitions)
            {
                if (t.CompareTo(transition) == 0)
                { // is the same transition
                    return false;
                }
            }

            AddState(transition.SourceState);
            AddState(transition.TargetState);

            //else, adds to list.
            Transitions.Add(transition);
            //transition.SourceState.Transitions.Add(transition);
            AddInput(transition.Input);
            AddOutput(transition.Output);

            //These are not the droids you're looking for.
            return true;
        }

        /// <summary>
        /// ToString implementation.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            String msg = "";
            if (!Name.Equals(""))
                msg += "\n" + Name + ":\n";

            foreach (Transition t in Transitions)
            {
                msg += "(" + t.SourceState.Name + ":" + t.TargetState.Name +
                    "[" + t.Input.ToString() + ":" + t.Output.ToString() + "])\n";
            }

            return HttpUtility.UrlDecode(msg);
        }

        public State GetStateByName(string p)
        {
            return this.States.Where(x => x.Name == p).FirstOrDefault();
        }

        public void MinimizeMe()
        {
            List<State> reachables = new List<State>();
            //Step 0: Remove unreacheble states
            reachables.Add(InitialState);

            foreach (State s in States)
                if (!reachables.Contains(s))
                    if ((Transitions.Where(x => x.TargetState.Equals(s))).Count() > 0)
                        reachables.Add(s);

            //1. Construct implication chart, one square for each combination of states taken two at a time

            String[][] chart = new String[reachables.Count][];
            int innerMax = reachables.Count - 1;
            for (int i = chart.Length - 1; i >= 0; i--, innerMax--)
                chart[i] = new String[innerMax];

            //2. Square labeled Si, Sj, if outputs differ than square gets "X". Otherwise write down implied state pairs for all input combinations
            for (int i = 0; i < chart.Length; i++)
                for (int j = 0; j < chart[i].Length; j++)
                {
                    State Si = reachables.ElementAt(i);
                    State Sj = reachables.ElementAt(j);
                    IEnumerable<Transition> SiT = Transitions.Where(x => x.SourceState.Equals(Si));

                    String outs = "";
                    foreach (Transition Ti in SiT)
                    {
                        IEnumerable<Transition> SjT = Transitions.Where(x => x.SourceState.Equals(Sj));
                        foreach (Transition Tj in SjT)
                            if (Ti.Output.Equals(Tj.Output))
                                if (Ti.Input.Equals(Tj.Input))
                                    outs += Ti.TargetState + ":" + Tj.TargetState + ";";

                        if (outs.Equals(""))
                            break;
                    }
                    if (outs.Equals(""))
                        outs = "X";
                    chart[i][j] = outs;
                }

            //printChart(reachables, chart, "chartUnmarked.txt");

            //3. Advance through chart top-to-bottom and left-to-right.
            //If square Si, Sj contains next state pair Sm, Sn and that pair labels a square already labeled "X", then Si, Sj is labeled "X".
            //4. Continue executing Step 3 until no new squares are marked with "X".
            bool marked = true;
            while (marked)
            {
                marked = false;
                for (int i = 0; i < chart.Length; i++)
                    for (int j = 0; j < chart[i].Length; j++)
                        if (!(chart[i][j].Equals("X")))
                        {
                            string[] labels = chart[i][j].Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                            for (int k = 0; k < labels.Length; k++)
                            {
                                string[] states = labels[k].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                                State Sm = reachables.Find(x => x.Name.Equals(states[0]));
                                State Sn = reachables.Find(x => x.Name.Equals(states[1]));
                                int SmI = reachables.IndexOf(Sm);
                                int SnI = reachables.IndexOf(Sn);
                                if (IsValidIndex(chart, SmI, SnI))
                                {
                                    if (chart[SmI][SnI].Equals("X"))
                                    {
                                        chart[i][j] = "X";
                                        marked = true;
                                    }
                                }
                                else if (IsValidIndex(chart, SnI, SmI))
                                {
                                    if (chart[SnI][SmI].Equals("X"))
                                    {
                                        chart[i][j] = "X";
                                        marked = true;
                                    }
                                }
                            }
                        }
            }
            //printChart(reachables, chart, "chartMarked.txt");
            //5. For each remaining unmarked square Si, Sj, then Si and Sj are equivalent. 
            List<Transition> trans = Transitions.ToList();

            trans.RemoveAll(x => !reachables.Contains(x.SourceState));

            for (int i = 0; i < chart.Length; i++)
                for (int j = 0; j < chart[i].Length; j++)
                    if (!(chart[i][j].Equals("X")))
                    {
                        State Si = reachables.ElementAt(i);
                        State Sj = reachables.ElementAt(j);
                        State fusion = new State(Si.Name + Sj.Name);
                        foreach (Transition t in trans.Where(x => x.SourceState.Equals(Si) || x.TargetState.Equals(Si) ||
                                                                   x.SourceState.Equals(Sj) || x.TargetState.Equals(Sj)))
                        {
                            if (t.SourceState.Equals(Si) || t.SourceState.Equals(Sj))
                                t.SourceState = fusion;

                            if (t.TargetState.Equals(Si) || t.TargetState.Equals(Sj))
                                t.TargetState = fusion;
                        }
                    }

            trans = trans.Distinct().ToList();

            this.States = new List<State>();
            this.Transitions = new List<Transition>();
            this.InputAlphabet = new List<string>();
            this.OutputAlphabet = new List<string>();
            foreach (Transition t in trans)
                this.AddTransition(t);
        }

        private bool IsValidIndex(string[][] chart, int SmI, int SnI)
        {
            if (SmI < chart.Length)
                if (SnI < chart[SmI].Length)
                    return true;

            return false;
        }

        private static void printChart(List<State> reachables, String[][] chart, String fileName)
        {
            StreamWriter fw = new StreamWriter(fileName);
            for (int i = 0; i < chart.Length; i++)
            {
                fw.Write(reachables.ElementAt(i) + " ");
                for (int j = 0; j < chart[i].Length; j++)
                {
                    //Console.Write("[" + States.ElementAt(i) + "][" + States.ElementAt(j) + "] ");
                    //fw.Write("[" + i + "][" + j + "]=" + chart[i][j] + " ");
                    fw.Write(chart[i][j] + "| ");
                }
                fw.WriteLine();
            }
            fw.Close();
        }

        public List<State> FinalStates
        {
            get
            {
                /*
                List<State> ret = new List<State>();

                if (Transitions.Count > 0)
                {
                    ret = (from t in Transitions
                           where Transitions.Where(x => x.SourceState.Equals(t.TargetState)).Count() == 0
                           select t.TargetState
                          ).ToList();
                }
                else
                {
                    ret.Add(InitialState);
                }
                return ret;
                */
                return finals;
            }
        }

        public void WipeOutState(State s)
        {
            Transitions.RemoveAll(x => x.TargetState.Equals(s) || x.SourceState.Equals(s));
            States.RemoveAll(x => x.Equals(s));
        }

        public void CheckAsFinal(State aState)
        {
            if (!finals.Contains(aState))
            {
                finals.Add(aState);
            }
        }
        public void UncheckAsFinal(State aState)
        {
            if (finals.Contains(aState))
            {
                finals.Remove(aState);
            }
        }

      
    }
}

