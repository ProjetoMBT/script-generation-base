using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coc.Data.ControlStructure;
using Coc.Data.Interfaces;
using Coc.Modeling.TestPlanStructure;
using Coc.Data.CSV;
using Coc.Modeling.FiniteStateMachine;
using Coc.Data.ControlAndConversionStructures;
using Coc.Modeling.TestSuitStructure;

namespace Coc.Data.HSI
{
    /*
    /// <summary>
    /// <img src="images/HSI.PNG"/>
    /// </summary>
    public static class NamespaceDoc
    {
    }*/


    public class HsiMethod : SequenceGenerator
    {
        #region Attributes
        private FiniteStateMachine fsm;
        private StatePair[] statePairGroup;
        private List<FailnessRecord> failnessTable;
        private String[][] hiSet;
        private String[][][] hsiSet;
        /// <summary>
        /// Denotes the status of each state pair in GetHiSet method.
        /// - Invalid state pairs doesn´t exists in Lzero set. 
        /// - Fail state pairs have different output for a same input.
        /// - Valid state pairs does point to another state pair.
        /// </summary>
        public enum Failness
        {
            Invalid = 0,
            Fail = 1,
            Valid = 2
        }
        #endregion

        #region Constructor
        public HsiMethod()
        {

        }
        #endregion

        #region Public Methods
        public override List<GeneralUseStructure> GenerateSequence(List<GeneralUseStructure> listGeneralStructure, ref int tcCount, StructureType type)
        {
            //FUNC
            GenerateTestPlan populate1 = new GenerateTestPlan();
            //PERF
         //   GenerateTestSuit populate2 = new GenerateTestSuit();

            List<TestPlan> listPlan = new List<TestPlan>();
            List<GeneralUseStructure> listScript = new List<GeneralUseStructure>();
            GeneralUseStructure modelGeneralUseStructure = listGeneralStructure.FirstOrDefault();
            List<GeneralUseStructure> listSequenceGenerationStructure = base.ConvertStructure(listGeneralStructure, type);
            List<TestStep> listTestStep = new List<TestStep>();

            foreach (GeneralUseStructure sgs in listSequenceGenerationStructure)
            {
                this.fsm = (FiniteStateMachine)sgs;
                String[][] sequence = this.GenerateTestCases();
                //Verify what to do in this step when using PerformanceTool
                List<CsvParamFile> listCSV = listGeneralStructure.OfType<CsvParamFile>().ToList();
                //FUNC
                TestPlan plan = populate1.PopulateTestPlan(sequence, fsm, listCSV);
                //PERF
              //  TestSuit suit = populate2.PopulateTestSuit(sequence, fsm, modelGeneralUseStructure);
                tcCount += plan.TestCases.Count;
                plan.NameUseCase = this.fsm.NameUseCase;
                listPlan.Add(plan);
            }

            GeneralTPGenerator(listPlan, listTestStep);
            
            foreach (TestPlan testPlan in listPlan)
            {
                GeneralUseStructure sc = (TestPlan)testPlan;
                //sc.NameUseCase = testPlan.NameUseCase;
                listScript.Add(sc);
            }

            return listScript;
        }

        /// <summary>
        /// Shortcut for GetPreamble(State s, List-of-State visited) method.
        /// </summary>
        public String[] GetPreamble(State s)
        {
            List<State> visitedStates = new List<State>();
            visitedStates.Add(s); //cannot walk through S
            
            return GetPreamble(s, visitedStates);
        }

        /// <summary>
        /// Gets a preamble of a given state S.
        /// </summary>
        public String[] GetPreamble(State s, List<State> visited)
        {
            //Initial State's preamble is EPSILON
            if (s.Equals(fsm.InitialState))
            {
                return new String[] { };
            }

            String[] shortestPreamble = null;
            String lastInput = "";

            //Get ancestors' preambles
            IEnumerable<Transition> filteredTransitions = fsm.Transitions.Where(x => x.TargetState.Equals(s));

            foreach (Transition transition in filteredTransitions)
            {
                if (visited.Contains(transition.SourceState))
                {
                    continue;
                }

                if (transition.SourceState.Equals(transition.TargetState))
                {
                    continue;
                }

                visited.Add(transition.SourceState);
                String[] currentPreamble = this.GetPreamble(transition.SourceState, visited);

                if (shortestPreamble == null || shortestPreamble.Count() > currentPreamble.Count())
                {
                    shortestPreamble = currentPreamble;
                    lastInput = transition.Input;
                }
            }

            if (shortestPreamble == null)
            {
                return new String[] { };
            }

            //Creates a new preamble with size + 1
            String[] preamble = new String[shortestPreamble.Count() + 1];

            //Copy antecessor preamble to the new preable
            shortestPreamble.CopyTo(preamble, 0);
            //adds current transition input to current state S
            preamble[preamble.Count() - 1] = lastInput;

            String q = "";

            for (int p = 0; p < preamble.Length; p++)
            {
                q += "[" + preamble[p] + "]";
            }

            return preamble;
        }

        /// <summary>
        /// Gets the allowed inputs list of a given state S.
        /// </summary>
        public String[] GetAllowedInputs(State s)
        {
            IEnumerable<String> inputs = from Transition transition in fsm.Transitions
                                         where transition.SourceState.Equals(s)
                                         select transition.Input;

            return inputs.Distinct().ToArray();
        }

        /// <summary>
        /// Gets the transitio cover of given state S.
        /// </summary>
        public String[][] GetTransitionCover(State s)
        {
            //Concatenate preamble with allowed inputs.
            String[] preamble = this.GetPreamble(s);
            String[] allowedInputs = this.GetAllowedInputs(s);
            String[][] transitionCover = new String[allowedInputs.Length][];

            for (int i = 0; i < allowedInputs.Length; i++)
            {
                String currentInput = allowedInputs[i];
                transitionCover[i] = new String[preamble.Length + 1];

                for (int j = 0; j < preamble.Length; j++)
                {
                    transitionCover[i][j] = preamble[j];
                }

                transitionCover[i][preamble.Length] = currentInput;
            }

            return transitionCover;
        }

        /// <summary>
        /// Gets the transition cover of given state S.
        /// </summary>
        public String[][] GetTransitionCover(StateNode s)
        {
            //Concatenate preamble with allowed inputs.
            String[] preamble = s.Preamble;
            String[] allowedInputs = this.GetAllowedInputs(s.State);
            String[][] transitionCover = new String[allowedInputs.Length][];

            for (int i = 0; i < allowedInputs.Length; i++)
            {
                String currentInput = allowedInputs[i];
                transitionCover[i] = new String[preamble.Length + 1];

                for (int j = 0; j < preamble.Length; j++)
                {
                    transitionCover[i][j] = preamble[j];
                }

                transitionCover[i][preamble.Length] = currentInput;
            }

            return transitionCover;
        }

        /// <summary>
        /// Generate every combination of state-state for current fsm.
        /// </summary>
        public StatePair[] GetStatePairGroup()
        {
            //if (this.statePairGroup != null)
            //    return this.statePairGroup;

            //Pairs of X-X or Y-Y are not included. Left element must be different from the right one.
            //List size is equals to (N * (N -1)) / 2, where N is the fsm.States list size.
            int statesCount = this.fsm.States.Count;
            int indexer = 0;
            StatePair[] statePairGroup = new StatePair[(statesCount * (statesCount - 1)) / 2];

            for (int i = 0; i < this.fsm.States.Count; i++)
            {
                for (int j = i + 1; j < this.fsm.States.Count; j++)
                {
                    State s1 = this.fsm.States[i];
                    State s2 = this.fsm.States[j];
                    StatePair sp = new StatePair();
                    
                    sp.StateA = s1;
                    sp.StateB = s2;
                    statePairGroup[indexer++] = sp;
                }
            }
            this.statePairGroup = statePairGroup;
            
            return statePairGroup;
        }

        /// <summary>
        /// Stores information about state pairs and its transitions.
        /// Used by HSI method to generate harmonized sets.
        /// </summary>


        /// <summary>
        /// Apply fsm inputs and set points from a state-pair to another one.
        /// </summary>
        public List<FailnessRecord> GetFailnessTable()
        {
            //if (this.failnessTable != null)
            //    return this.failnessTable;

            this.failnessTable = new List<FailnessRecord>();
            StatePair[] pairs = this.GetStatePairGroup();

            for (int i = 0; i < pairs.Length; i++)
            {
                StatePair pair = pairs[i];

                for (int j = 0; j < this.fsm.InputAlphabet.Count; j++)
                {
                    FailnessRecord record = new FailnessRecord();

                    record.SourcePair = pair;
                    record.Input = this.fsm.InputAlphabet[j];

                    String outputA = String.Empty, outputB = String.Empty;
                    State targetA = null, targetB = null;

                    //gets record target
                    foreach (Transition t in this.fsm.Transitions.Where(x => x.Input == record.Input))
                    {
                        if (t.SourceState == pair.StateA)
                        {
                            targetA = t.TargetState;
                            outputA = t.Output;
                        }
                        else if (t.SourceState == pair.StateB)
                        {
                            targetB = t.TargetState;
                            outputB = t.Output;
                        }
                    }

                    if (targetA == null || targetB == null)
                    {
                        record.Status = Failness.Invalid;
                    }
                    else if (outputA != outputB)
                    {
                        record.Status = Failness.Fail;
                    }
                    else
                    {
                        record.Status = Failness.Valid;
                    }
                    record.TargetPair = pairs.Where(x => (x.StateA == targetA && x.StateB == targetB) || (x.StateB == targetA && x.StateA == targetB)).FirstOrDefault();
                    this.failnessTable.Add(record);
                }
            }

            return failnessTable;
        }

        /// <summary>
        /// Gets the harmonized set for each state
        /// </summary>
        public String[][] GetHiSet()
        {
            //if (this.hiSet != null)
            //    return this.hiSet;

            List<FailnessRecord> failnessTable = this.GetFailnessTable();
            StatePair[] statePairGroup = this.GetStatePairGroup();
            // Jagged! Sequences have different sizes.
            // hiSet is a Matrix[mxn], where [m] is the StatePairCount (see formula below) and [n] is a unknow sequence length.
            String[][] hiSet = new String[statePairGroup.Length][];

            // for each StatePair, if pair is fail, use input which failed as HI, else
            // use failness from pointed state.
            for (int i = 0; i < statePairGroup.Length; i++)
            {
                StatePair sp = statePairGroup[i];
                String[] seq = this.FindShortestInputToFail(failnessTable, statePairGroup, sp);

                if (seq == null)
                {
                    seq = new String[] { };
                }
                hiSet[i] = seq;
            }
            this.hiSet = hiSet;
            
            return hiSet;
        }

        /// <summary>
        /// Generate a separating set combining generated Hi sets.
        /// </summary>
        public String[][][] GetHsiSet()
        {
            //if (this.hsiSet != null)
            //{
            //    return this.hsiSet;
            //}

            //Hi set stores families by state pair
            //this method will organize families by state, generating the HSI set.
            StatePair[] lZeroSet = this.GetStatePairGroup();
            // M[]xy => x(state pairs) by x(inputs)
            String[][] hiSet = this.GetHiSet();
            
            //result will be delivered as M[]ab, where M is a(State) by b(input)
            hsiSet = new String[this.fsm.States.Count][][];

            //sorting by state
            for (int i = 0; i < lZeroSet.Length; i++)
            {
                StatePair pair = lZeroSet[i];
            
                this.AddPrefixToState(pair.StateA, hiSet[i]);
                this.AddPrefixToState(pair.StateB, hiSet[i]);
            }

            return hsiSet;
        }

        /// <summary>
        /// Generate test sequences using HSI method.
        /// </summary>
        public String[][] GenerateTestCases()
        {
            List<String[]> transitionCovers = new List<String[]>();
            List<String[]> testCases = new List<String[]>();
            StateNode tree = GenerateTree(this.fsm.InitialState);

            //group transition covers into the same group.
            for (int i = 0; i < this.fsm.States.Count; i++)
            {
                TestPlan plan = new TestPlan();
                String[][] transitionCover;
                //transitionCover = this.GetTransitionCover(this.fsm.States[i]);
                State stateAux = this.fsm.States[i];
                String stateAuxName = stateAux.Name;
                StateNode aux = tree.getState(stateAux);

                if (aux == null)
                {
                    throw new Exception("State not found: " + this.fsm.States[i]);
                }
                transitionCover = this.GetTransitionCover(aux);

                for (int j = 0; j < transitionCover.Length; j++)
                {
                    //not all input are allowed
                    try
                    {
                        String[] iii = transitionCover[j];
                        transitionCovers.Add(iii);
                    }
                    catch (IndexOutOfRangeException) { }
                }
            }

            //generate separating families
            String[][][] hsiSet = this.GetHsiSet();

            foreach (String[] transitionCover in transitionCovers)
            {
                State parentState = this.GetIdentifierState(transitionCover);
                int stateIndex = this.fsm.States.IndexOf(parentState);

                foreach (String[] s in hsiSet[stateIndex])
                {
                    if (s == null)
                    {
                        continue;
                    }

                    String[] sequence = new String[transitionCover.Length + s.Length];

                    //copy transition cover into sequence
                    for (int k = 0; k < transitionCover.Length; k++)
                    {
                        sequence[k] = transitionCover[k];
                    }

                    //add hsi element into sequence
                    for (int k = 0; k < s.Length; k++)
                    {
                        sequence[k + transitionCover.Length] = s[k];
                    }
                    testCases.Add(sequence);
                }
            }
            //remove duplicated sequences and prefix
            testCases = this.RemoveDuplicatedSequences(testCases);

            return testCases.ToArray();
        }

        /// <summary>
        /// Equality compare implementation. Compares two arrays of String.
        /// </summary>

        /// <summary>
        /// Apply a sequence of input to the fsm. Returns last visited state.
        /// </summary>
        public State GetIdentifierState(String[] transitionCover)
        {
            State s = this.fsm.InitialState;

            for (int i = 0; i < transitionCover.Length; i++)
            {
                foreach (Transition t in this.fsm.Transitions)
                {
                    if (t.SourceState.Equals(s) && t.Input.Equals(transitionCover[i]))
                    {
                        s = t.TargetState;
                        break;
                    }
                }
            }

            return s;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Associate a prefix with given state
        /// </summary>
        private void AddPrefixToState(State state, String[] p)
        {
            int stateIndex = this.fsm.States.IndexOf(state);
            String[][] currentPrefix = this.hsiSet[stateIndex];

            if (currentPrefix == null || currentPrefix.Length == 0)
            {
                this.hsiSet[stateIndex] = new String[][] { p };
            }
            else
            {
                String[][] buffer = this.hsiSet[stateIndex];
                String[][] newSet = new String[buffer.Length + 1][];

                for (int i = 0; i < buffer.Length; i++)
                {
                    newSet[i] = buffer[i];
                }
                newSet[newSet.Length - 1] = p;
                this.hsiSet[stateIndex] = newSet;
            }
        }

        /// <summary>
        /// Returns input which made state pair failed. If current
        /// State pair is not fail, locate the shortest sequence to
        /// reach a failed state pair.
        /// </summary>
        private String[] FindShortestInputToFail(List<FailnessRecord> failnessTable, StatePair[] statePairGroup, StatePair sp)
        {
            IEnumerable<FailnessRecord> foundRecords = from FailnessRecord f in failnessTable
                                                       where f.SourcePair.Equals(sp) && f.Status == Failness.Fail
                                                       select f;

            //it is fail. get input which made it fail
            if (foundRecords.Count() > 0)
            {
                FailnessRecord record = foundRecords.First(); //will never be null

                return new String[] { record.Input };
            }
            else //is invalid or valid
            {
                //make a new query, so we can proceed on the valid way
                foundRecords = from FailnessRecord f in failnessTable
                               where f.SourcePair.Equals(sp) && f.Status == Failness.Valid
                               select f;
                //find available sequences to fail
                List<String[]> foundSequences = new List<String[]>();

                foreach (FailnessRecord f in foundRecords)
                {
                    String[] sequence = FindShortestInputToFail(failnessTable, statePairGroup, f.TargetPair);
                
                    if (sequence != null)
                    {
                        List<String> newSequence = new List<String>();
                    
                        newSequence.Add(f.Input);
                        newSequence.AddRange(sequence);
                        foundSequences.Add(newSequence.ToArray());
                    }
                }

                //return the shortest sequence
                String[] bestFit = null;

                foreach (String[] sequence in foundSequences)
                {
                    if (sequence.Length < 2)
                    {
                        continue;
                    }

                    if (bestFit == null || bestFit.Length > sequence.Length)
                    {
                        bestFit = sequence;
                    }
                }

                return bestFit;
            }
        }

        /// <summary>
        /// Represents an unsorted pair of distinct states.
        /// </summary>
        ///
        private StateNode GenerateTree(State initialState)
        {
            List<State> inT = new List<State>();
            inT.Add(initialState);

            StateNode root = new StateNode(initialState, new String[] { });
            root.AddChildren(GetChildren(root, inT));

            return root;
        }

        private List<StateNode> GetChildren(StateNode root, List<State> inTree)
        {
            StateNode[] children = new StateNode[this.fsm.States.Count];
            IEnumerable<Transition> s = this.fsm.Transitions.Where(x => x.SourceState.Name.Equals(root.State.Name));
            StateNode q;
            int i = 0;
            String[] preamble;

            foreach (Transition t in s)
            {
                if (!inTree.Contains(t.TargetState))
                {
                    preamble = new String[root.Preamble.Length + 1];
                    //Copy antecessor preamble to the new preamble
                    root.Preamble.CopyTo(preamble, 0);
                    //adds current transition input to current state S
                    preamble[preamble.Length - 1] = t.Input;
                    q = new StateNode(t.TargetState, preamble);
                    children[i] = q;
                    inTree.Add(q.State);
                    i++;
                }
            }
            for (int j = 0; j < children.Length; j++)
            {
                if (children[j] != null)
                {
                    if (children[j].State != null)
                    {
                        children[j].AddChildren(GetChildren(children[j], inTree));
                    }
                }
            }
            List<StateNode> r = children.ToList();
            r.RemoveAll(x => x == null);
            r.RemoveAll(x => x.State == null);
            
            return r;
        }

        /// <summary>
        /// Remove duplicated sequences and prefixes from test case set.
        /// </summary>
        private List<String[]> RemoveDuplicatedSequences(List<String[]> testCases)
        {
            //1st step - remove duplicated sequences
            List<String[]> withoutRedundance = testCases.Distinct<String[]>(new ArrayComparer()).ToList();

            //2nd step - remove prefix
            List<String[]> withoutPrefix = new List<String[]>();
            //List<String[]> filteredList = new List<String[]>();

            //filters clockwise
            foreach (String[] seq in withoutRedundance)
            {
                bool isPrefixOf = false;

                foreach (String[] seqq in withoutPrefix)
                {
                    if (ArrayComparer.IsPrefixOf(seqq, seq))
                    {
                        withoutPrefix.Remove(seqq);
                        if (!withoutPrefix.Contains(seq))
                        {
                            withoutPrefix.Add(seq);
                        }
                        isPrefixOf = true;
                        break;
                    }
                    else
                    {
                        if (ArrayComparer.IsPrefixOf(seq, seqq))
                        {
                            withoutPrefix.Remove(seq);
                            if (!withoutPrefix.Contains(seqq))
                            {
                                withoutPrefix.Add(seqq);
                            }
                            isPrefixOf = true;
                            break;
                        }
                    }
                }
                if (!isPrefixOf)
                {
                    withoutPrefix.Add(seq);
                }
            }

            withoutPrefix.Reverse();

            ////filters counterclockwise 
            //foreach (String[] seq in withoutPrefix)
            //{
            //    bool isPrefixOf = false;
            //    foreach (String[] seqq in filteredList)
            //    {
            //        if (ArrayComparer.IsPrefixOf(seq, seqq))
            //        {
            //            isPrefixOf = true;
            //            break;
            //        }
            //    }

            //    if (!isPrefixOf)
            //        filteredList.Add(seq);
            //}

            return withoutPrefix;
        }

        private void GeneralTPGenerator(List<TestPlan> listPlan, List<TestStep> listTestStep)
        {
            TestPlan testPlanGeral = new TestPlan();
            Coc.Modeling.TestPlanStructure.TestCase testCaseGeral = new Coc.Modeling.TestPlanStructure.TestCase("GeneralTestCase1");
            TestPlan testPlanAux = listPlan[listPlan.Count - 1];
            Coc.Modeling.TestPlanStructure.TestCase testAux = testPlanAux.TestCases[testPlanAux.TestCases.Count - 1];
            int valor = testAux.WorkItemId;
            
            valor = valor + 1;
            testCaseGeral.WorkItemId = valor;
            testCaseGeral.Title = testCaseGeral + "_" + valor;
            
            foreach (TestPlan testPlan in listPlan)
            {
                foreach (Coc.Modeling.TestPlanStructure.TestCase testCase in testPlan.TestCases)
                {
                    bool initial = true;
                    foreach (TestStep testStep in testCase.TestSteps)
                    {
                        if (initial)
                        {
                            testStep.Title = testCase.Title;
                            initial = false;
                        }
                        listTestStep.Add(testStep);
                    }
                }
            }
            testCaseGeral.TestSteps.AddRange(listTestStep);
            testPlanGeral.TestCases.Add(testCaseGeral);
            listPlan.Add(testPlanGeral);
        }
        #endregion

        #region Internal Class
        public class StatePair
        {
            public State StateA = null;
            public State StateB = null;

            public override String ToString()
            {
                return StateA.Name + " - " + StateB.Name;
            }

            public override bool Equals(object obj)
            {
                if (!(obj is StatePair))
                {
                    return false;
                }

                StatePair s = (StatePair)obj;

                if (this.StateA.Equals(s.StateA) && this.StateB.Equals(s.StateB))
                {
                    return true;
                }
                if (this.StateB.Equals(s.StateA) && this.StateA.Equals(s.StateB))
                {
                    return true;
                }
                return false;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public class FailnessRecord
        {
            public StatePair SourcePair = null;
            public StatePair TargetPair = null;
            public String Input = FiniteStateMachine.EPSILON;
            public Failness Status = Failness.Invalid;
            
            public override String ToString()
            {
                StringBuilder b = new StringBuilder();
                b.Append(SourcePair.ToString());
                b.Append(" (" + Input + ") ");
                b.Append((TargetPair == null) ? "(NIL)" : TargetPair.ToString());
                b.Append(" [" + Status.ToString() + "]");

                return b.ToString();
            }
        }

        public class ArrayComparer : IEqualityComparer<String[]>
        {
            /// <summary>
            /// Interface Implementation
            /// </summary>
            public bool Equals(String[] x, String[] y)
            {
                if (x.Length != y.Length)
                {
                    return false;
                }
                for (int i = 0; i < x.Length; i++)
                {
                    if (x[i] != y[i])
                    {
                        return false;
                    }
                }
                return true;
            }

            /// <summary>
            /// Interface implementation
            /// </summary>
            public int GetHashCode(String[] obj)
            {
                return base.GetHashCode();
            }

            /// <summary>
            /// returns true if seq is prefix of seqq
            /// </summary>
            public static bool IsPrefixOf(String[] seq, String[] seqq)
            {
                //prefix should contains less elements than the sequence
                if (seq.Length >= seqq.Length)
                {
                    return false;
                }
                //checks every element
                for (int i = 0; i < seq.Length; i++)
                {
                    if (seq[i] != seqq[i])
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        #endregion
    }
}