using System;
using System.Collections.Generic;
using System.Linq;
using Coc.Modeling.Graph;
using Coc.Data.Interfaces;
using Coc.Data.ControlStructure;
using Coc.Modeling.TestPlanStructure;
using Coc.Data.CSV;
using Coc.Data.ControlAndConversionStructures;

namespace Coc.Data.DFS
{
    /*
    /// <summary>
    /// <img src="images/DFS.PNG"/>
    /// </summary>
    public static class NamespaceDoc
    {
    }*/



    public class DepthFirstSearch : SequenceGenerator
    {
        #region Attributes
        private DirectedGraph dg;
        List<TestStep> testStepsList = new List<TestStep>();
        List<TestCase> testCasesList = new List<TestCase>();
        private List<Edge> Edges;
        private List<Node> Nodes;
        private List<Node> Finals;
        private Stack<String> Pilha;
        private TestPlan testPlan;
        public List<CsvParamFile> paramFiles { get; set; }
        #endregion

        #region Constructor
        public DepthFirstSearch()
        {
            this.testPlan = new TestPlan();
        }
        #endregion

        #region Public Methods
        public override List<GeneralUseStructure> GenerateSequence(List<GeneralUseStructure> listGeneralStructure, ref int tcCount, StructureType type)
        {
            GenerateTestPlan populate = new GenerateTestPlan();
            List<TestPlan> listPlan = new List<TestPlan>();
            List<GeneralUseStructure> listScript = new List<GeneralUseStructure>();
            List<GeneralUseStructure> listSequenceGenerationStructure = base.ConvertStructure(listGeneralStructure, type);
            paramFiles = listGeneralStructure.OfType<CsvParamFile>().ToList();

            foreach (GeneralUseStructure sgs in listSequenceGenerationStructure)
            {
                this.dg = (DirectedGraph)sgs;
                List<String[]> sequence = this.GenerateTestCases();
                testPlan = populate.PopulateTestPlan(sequence, dg, paramFiles);
                tcCount += testPlan.TestCases.Count;
                listPlan.Add(testPlan);
            }

            GeneralTPGenerator(listPlan);

            foreach (TestPlan testPlan in listPlan)
            {
                GeneralUseStructure sc = (GeneralUseStructure)testPlan;
                listScript.Add(sc);
            }
            TestCase.contWorkItemId = 1000;
            return listScript;
        }

        /// <summary>
        /// Generate test sequences using Depth First Search method.
        /// </summary>
        public List<String[]> GenerateTestCases()
        {
            this.testPlan = new TestPlan();
            this.testPlan.Name = dg.Name;

            List<String[]> testCases = new List<String[]>();
            List<String[]> sequence = new List<String[]>();
            
            sequence = GetAllFinalSequences(this.dg.RootNode, true);
            testCases.AddRange(sequence);
            return testCases;
        }
        #endregion

        #region Private Methods
        private List<String[]> GetAllFinalSequences(Node node, Boolean avoidCycles)
        {
            List<String[]> ret = new List<String[]>();
            Pilha = new Stack<String>();
            List<String> DFSSeqs = DFS(node, avoidCycles);

            String[] delimiter = new String[] { ";" };
            String[] sequence;
            foreach (String seq in DFSSeqs)
            {
                sequence = seq.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                Array.Reverse(sequence);
                ret.Add(sequence);
            }
            return ret;
        }

        private List<String> DFS(Node v, bool avoidCycles)
        {
            this.Nodes = dg.Nodes;
            this.Edges = dg.Edges;
            this.Finals = dg.Finals;

            List<Edge> vEdges;
            List<String> ret = new List<String>();
            if (Finals.Contains(v))
            {
                ret.Add(PrintStack(Pilha));
            }

            if (v != null)
            {
                vEdges = GetAllEdgesFrom(v);
                foreach (Edge edge in vEdges)
                {
                    Node w = edge.NodeB;
                    if (!(edge.isChecked() && avoidCycles))
                    {
                        edge.Check();
                        if (!w.isChecked())
                        {
                            Pilha.Push(edge.GetTaggedValue("TDACTION"));
                            ret.AddRange(DFS(w, avoidCycles));
                            Pilha.Pop();
                        }
                        edge.UnCheck();
                    }
                }
            }
            return ret;
        }

        private List<Edge> GetAllEdgesFrom(Node v)
        {
            List<Edge> ret = new List<Edge>();
            foreach (Edge edge in Edges)
            {
                if (edge.NodeA.Equals(v))
                {
                    ret.Add(edge);
                }
            }
            return ret;
        }

        private String PrintStack(Stack<String> pilha)
        {
            String ret = "";
            foreach (String s in Pilha)
            {
                ret += s + ";";
            }
            ret = ret.Substring(0, ret.Length - 1);

            return ret;
        }

        private void GeneralTPGenerator(List<TestPlan> listPlan)
        {
            TestPlan testPlanGeral = new TestPlan();
            TestCase testCaseGeral = new TestCase("GeneralTestCase");
            TestPlan testPlanAux = listPlan[listPlan.Count - 1];
            TestCase testAux = testPlanAux.TestCases[testPlanAux.TestCases.Count - 1];
            List<TestStep> listTestStep = new List<TestStep>();

            int valor = testAux.WorkItemId;
            valor = valor + 1;
            testCaseGeral.WorkItemId = valor;
            testCaseGeral.Title = testCaseGeral + "_" + valor;
            foreach (TestPlan testPlan in listPlan)
            {
                foreach (TestCase testCase in testPlan.TestCases)
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
    }
}
