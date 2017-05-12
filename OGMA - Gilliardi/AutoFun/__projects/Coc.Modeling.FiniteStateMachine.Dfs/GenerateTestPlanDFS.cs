using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using Coc.Data.CSV;
using Coc.Modeling.TestPlanStructure;
using Coc.Modeling.Graph;

namespace Coc.Data.DFS
{
    public class GenerateTestPlan
    {
        #region Attributes
        public List<CsvParamFile> paramFiles { get; set; }
        private int currLine = 0;
        private int maxLine = 0;
        private Boolean doAgain = false;
        private Regex param = new Regex(@"(?<param>{(?<file>[ZéúíóáÉÚÍÓÁèùìòàÈÙÌÒÀõãñÕÃÑêûîôâÊÛÎÔÂëÿüïöäËYÜÏÖÄçÇ\sa-zA-Z0-9_!#$%&'+\/=?^`{|}~-]*).(?<column>[ZéúíóáÉÚÍÓÁèùìòàÈÙÌÒÀõãñÕÃÑêûîôâÊÛÎÔÂëÿüïöäËYÜÏÖÄçÇ\sa-zA-Z0-9_!#$%&'+\/=?^`{|}~-]*)})", RegexOptions.IgnoreCase);
        private List<CsvParamFile> usedFiles;
        private Boolean readFile;
        #endregion

        #region Public Methods
        public int GetUsedFilesLineCount(String TaggedValue)
        {
            int lineCount = int.MaxValue;
            MatchCollection matches = param.Matches(HttpUtility.UrlDecode(TaggedValue));
            foreach (Match m in matches)
            {
                if (m.Success)
                {
                    if (m.Groups["file"].Success)
                    {
                        IEnumerable<CsvParamFile> files = paramFiles.Where(x => x.FileName.Equals(m.Groups["file"].Value, StringComparison.InvariantCultureIgnoreCase));
                        foreach (CsvParamFile file in files)
                        {
                            if (file.LinesCount < lineCount)
                            {
                                lineCount = file.LinesCount;
                            }
                        }
                    }
                }
            }
            return lineCount;
        }

        public TestPlan PopulateTestPlan(List<String[]> sequence, DirectedGraph dg, List<CsvParamFile> paramFiles)
        {
            this.paramFiles = paramFiles;
            TestPlan testPlan = new TestPlan();
            PopulateTestCase(sequence, dg, testPlan);

            return testPlan;
        }
        #endregion

        #region Private Methods
        private void PopulateTestCase(List<String[]> sequence, DirectedGraph dg, TestPlan testPlan)
        {
            for (int k = 0; k < sequence.Count(); k++)
            {
                Edge edge = new Edge();
                List<Edge> edges = new List<Edge>();
                String[] arraySequence = sequence[k];
                int maxUseCaseLines = int.MaxValue;
                foreach (String input in arraySequence)
                {
                    edge = GetEdge(input, dg, edge.NodeB);
                    if (edge != null)
                    {
                        edges.Add(edge);

                        foreach (KeyValuePair<String, String> pair in edge.Values)
                        {
                            int aux = GetUsedFilesLineCount(pair.Value);
                            if (maxUseCaseLines > aux)
                            {
                                maxUseCaseLines = aux;
                            }
                        }
                    }
                }

                TestCase testCase = null;
                if (maxUseCaseLines == int.MaxValue)
                {
                    ResetParamFilesPointers();
                }
                else
                {
                    ResetParamFilesPointers(maxUseCaseLines);
                }

                do
                {
                    testCase = FillTestCase(dg, edges, testCase);
                    if (testCase != null)
                    {
                        testPlan.TestCases.Add(testCase);
                    }
                    currLine++;
                } while (doAgain && (currLine < maxLine));
            }
        }

        private void ResetParamFilesPointers()
        {
            currLine = 0;
            maxLine = int.MaxValue;
            if (paramFiles != null)
            {
                foreach (CsvParamFile file in paramFiles)
                {
                    if (file.LinesCount < maxLine)
                        maxLine = file.LinesCount;
                }
            }
            doAgain = false;
        }

        private void ResetParamFilesPointers(int maxValue)
        {
            currLine = 0;
            maxLine = maxValue;
            doAgain = false;
        }

        private TestCase FillTestCase(DirectedGraph dg, List<Edge> edges, TestCase testCase)
        {
            TestStep testStep;
            int index = 1;
            testCase = new TestCase(HttpUtility.UrlDecode(dg.Name));
            testCase.Title += "_" + TestCase.contWorkItemId;
            testCase.WorkItemId = TestCase.contWorkItemId;
            testCase.WriteFirstLine = TestCaseTags(dg, testCase);
            TestCase.contWorkItemId++;
            testStep = new TestStep();
            usedFiles = new List<CsvParamFile>();
            foreach (Edge edge in edges)
            {
                readFile = false;
                Boolean isCycle = false;
                Boolean lastCycleTrans = false;
                if (edge.GetTaggedValue("TDlastCycleTrans") != null)
                {
                    lastCycleTrans = (edge.GetTaggedValue("TDlastCycleTrans").Equals("true") ? true : false);
                }
                if (edge.GetTaggedValue("TDcycleTran") != null)
                {
                    isCycle = true;
                }
                if (lastCycleTrans)
                {
                    usedFiles = usedFiles.Distinct().ToList();
                    foreach (CsvParamFile csv in usedFiles)
                    {
                        csv.NextLine();
                    }
                    usedFiles.Clear();
                }

                if (isCycle)
                {
                    testStep = new TestStep();
                    testStep.Index = index.ToString();
                    testStep.Description = GenerateDescription(edge);
                    testStep.ExpectedResult = GenerateExpectedResult(edge);
                    testCase.TestSteps.Add(testStep);
                    index++;
                }
                else
                {
                    testStep = new TestStep();
                    testStep.Index = index.ToString();
                    testStep.Description = GenerateDescription(edge);
                    testStep.ExpectedResult = GenerateExpectedResult(edge);
                    testCase.TestSteps.Add(testStep);
                    index++;
                    if (readFile)
                    {
                        doAgain = true;
                    }
                }
            }
            return testCase;
        }

        private Boolean TestCaseTags(DirectedGraph dg, TestCase tc)
        {
            Boolean ret = false;
            foreach (KeyValuePair<String, String> pair in dg.Values)
            {
                switch (pair.Key)
                {
                    case "TDSTATE":
                        tc.TDstate = HttpUtility.UrlDecode(pair.Value);
                        break;
                    case "TDASSIGNED":
                        tc.TDassigned = HttpUtility.UrlDecode(pair.Value);
                        break;
                    case "TDREASON":
                        tc.TDreason = HttpUtility.UrlDecode(pair.Value);
                        break;
                    case "TDITERATIONPATH":
                        tc.TDiterationPath = HttpUtility.UrlDecode(pair.Value);
                        break;
                    case "TDAREAPATH":
                        tc.TDareaPath = HttpUtility.UrlDecode(pair.Value);
                        break;
                    case "TDAPPLICATION":
                        tc.TDapplication = HttpUtility.UrlDecode(pair.Value);
                        break;
                    case "TDCOMPLEXITY":
                        tc.TDcomplexity = HttpUtility.UrlDecode(pair.Value);
                        break;
                    case "TDRISKS":
                        tc.TDrisks = HttpUtility.UrlDecode(pair.Value);
                        break;
                    case "TDTCLIFECYCLE":
                        tc.TDtcLifecycle = HttpUtility.UrlDecode(pair.Value);
                        break;
                    case "TDLIFECYCLETYPE":
                        tc.TDlifecycleType = HttpUtility.UrlDecode(pair.Value);
                        break;
                    case "TDTCTEAMUSAGE":
                        tc.TDtcTeamUsage = HttpUtility.UrlDecode(pair.Value);
                        break;
                    case "TDPOSTCONDITIONS":
                        tc.TDpostConditions = HttpUtility.UrlDecode(pair.Value);
                        ret = true;
                        break;
                    case "TDPRECONDITIONS":
                        tc.TDpreConditions = HttpUtility.UrlDecode(pair.Value);
                        ret = true;
                        break;
                    default:
                        break;
                }
            }
            return ret;
        }

        private Edge GetEdge(String input, DirectedGraph dg, Node nodeB)
        {
            List<Edge> edges = dg.Edges.Where(x => x.GetTaggedValue("TDACTION").Equals(input)).ToList();
            foreach (Edge edge in edges)
            {
                if (nodeB == null)
                {
                    return edge;
                }

                if (edge.NodeA.Equals(nodeB))
                {
                    return edge;
                }
            }
            return null;
        }

        private String GenerateDescription(Edge edge)
        {
            String aux = edge.NodeB.Name + Environment.NewLine + Environment.NewLine;
            Boolean cycle = false;
            if (edge.GetTaggedValue("TDCYCLETRAN") != null)
            {
                cycle = (edge.GetTaggedValue("TDCYCLETRAN").Equals("true") ? true : false);
            }
            if (!String.IsNullOrEmpty(aux))
            {
                String TDaction = HttpUtility.UrlDecode(edge.GetTaggedValue("TDACTION"));
                TDaction = FillTD(TDaction, cycle);
                aux += "- " + TDaction;
                aux = HttpUtility.UrlDecode(aux);
                aux = aux.Replace(" | ", "|");
                aux = aux.Replace("| ", "|");
                aux = aux.Replace(" |", "|");
                aux = aux.Replace("|", ";" + Environment.NewLine + "- ");
                aux = aux + ";";

                return aux;
            }
            else
            {
                return " ";
            }
        }

        private String GenerateExpectedResult(Edge edge)
        {
            String TDexpectedResult = HttpUtility.UrlDecode(edge.GetTaggedValue("TDEXPECTEDRESULT"));
            String aux;
            Boolean cycle = false;
            if (edge.GetTaggedValue("TDCYCLETRAN") != null)
                cycle = (edge.GetTaggedValue("TDCYCLETRAN").Equals("true") ? true : false);
            if (!String.IsNullOrEmpty(TDexpectedResult))
            {
                TDexpectedResult = FillTD(TDexpectedResult, cycle);
                aux = HttpUtility.UrlDecode(TDexpectedResult);
                aux = aux.Replace(" | ", "|");
                aux = aux.Replace("| ", "|");
                aux = aux.Replace(" |", "|");
                aux = aux.Replace("|", "." + Environment.NewLine);
                aux = aux + ".";

                return aux;
            }
            else
            {
                return " ";
            }
        }

        private String FillTD(String tdAction, Boolean useCyclePointer)
        {
            MatchCollection matches = param.Matches(tdAction);
            foreach (Match m in matches)
            {
                if (m.Success)
                {
                    if (m.Groups["file"].Success)
                    {
                        IEnumerable<CsvParamFile> files = paramFiles.Where(x => x.FileName.Equals(m.Groups["file"].Value, StringComparison.InvariantCultureIgnoreCase));
                        foreach (CsvParamFile file in files)
                        {
                            readFile = true;
                            if (m.Groups["column"].Success)
                            {
                                String value = "";
                                if (useCyclePointer)
                                {
                                    value = file.GetValueCurrentLine(m.Groups["column"].Value);
                                    usedFiles.Add(file);
                                }
                                else
                                {
                                    value = file.GetValue(m.Groups["column"].Value, currLine);

                                }
                                if (value != null)
                                {
                                    tdAction = tdAction.Replace(m.Value, "'" + value + "'");
                                }
                            }
                        }
                    }
                }
            }
            return tdAction;
        }
        #endregion
    }
}
