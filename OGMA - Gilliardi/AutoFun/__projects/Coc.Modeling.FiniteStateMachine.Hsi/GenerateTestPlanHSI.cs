using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Coc.Data.CSV;
using Coc.Modeling.FiniteStateMachine;
using Coc.Modeling.TestPlanStructure;

namespace Coc.Data.HSI
{
    public class GenerateTestPlan
    {
        #region Attributes
        public List<CsvParamFile> paramFiles { get; set; }
        private List<CsvParamFile> usedFiles;
        private int currLine = 0;
        private int maxLine = 0;
        private bool doAgain = false;
        private Regex param = new Regex(@"(?<param>{(?<file>[ZéúíóáÉÚÍÓÁèùìòàÈÙÌÒÀõãñÕÃÑêûîôâÊÛÎÔÂëÿüïöäËYÜÏÖÄçÇ\sa-zA-Z0-9_!#$%&'+\/=?^`{|}~-]*).(?<column>[ZéúíóáÉÚÍÓÁèùìòàÈÙÌÒÀõãñÕÃÑêûîôâÊÛÎÔÂëÿüïöäËYÜÏÖÄçÇ\sa-zA-Z0-9_!#$%&'+\/=?^`{|}~-]*)})", RegexOptions.IgnoreCase);
        private bool readFile;
        #endregion

        #region Public Methods
        public TestPlan PopulateTestPlan(String[][] matriz, FiniteStateMachine machine, List<CsvParamFile> paramFiles)
        {
            this.paramFiles = paramFiles;
            TestPlan testPlan = new TestPlan();
            PopulateTestCase(matriz, machine, testPlan);
            return testPlan;
        }

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
                                lineCount = file.LinesCount;
                        }
                    }
                }
            }
            return lineCount;
        }

        public void CopyTestCases(List<TestCase> listCases, TestPlan testPlan)
        {
            foreach (TestCase testCase in testPlan.TestCases)
            {
                listCases.Add(testCase);
            }
        }

        public TestCase CopyTestSteps(List<TestCase> listCases)
        {
            TestCase testCaseGeral = new TestCase("General TestCase");
            foreach (TestCase testCase in listCases)
            {
                Boolean passed = true;
                foreach (TestStep st in testCase.TestSteps)
                {
                    if (passed)
                    {
                        st.workItemIdString = "Test Case " + testCase.WorkItemId;
                        st.Title = testCase.Title;
                        passed = false;
                    }

                    if (!String.IsNullOrEmpty(st.Description))
                    {
                        testCaseGeral.TestSteps.Add(st);
                        TestCase.contWorkItemId = 1000;
                    }
                }
            }
            return testCaseGeral;
        }
        #endregion

        #region Private Methods
        private Transition GetTransitionFSM(String input, FiniteStateMachine fsm)
        {
            List<Transition> transition = fsm.Transitions.Where(x => x.Input.Equals(input)).ToList();
            foreach (Transition t in transition)
            {
                return t;
            }
            return null;
        }

        private void PopulateTestCase(String[][] matriz, FiniteStateMachine machine, TestPlan testPlan)
        {
            for (int k = 0; k < matriz.Length; k++)
            {
                List<Transition> listTransition = new List<Transition>();
                String[] arraySequence = matriz[k];
                int maxUseCaseLines = int.MaxValue;
                foreach (String input in arraySequence)
                {
                    Transition tran = new Transition();
                    tran = GetTransitionFSM(input, machine);
                    if (tran != null)
                    {
                        listTransition.Add(tran);

                        foreach (KeyValuePair<String, String> pair in tran.TaggedValues)
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
                    testCase = FillTestCase(machine, listTransition, testCase);
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

        private TestCase FillTestCase(FiniteStateMachine machine, List<Transition> listTransition, TestCase testCase)
        {
            int index = 1;
            TestStep testStep;
            testCase = new TestCase(HttpUtility.UrlDecode(machine.Name));
            testCase.Title += "_" + TestCase.contWorkItemId;
            //testCase.TestCaseId = TestCase.contWorkItemId;
            testCase.WorkItemId = TestCase.contWorkItemId;
            TestCase.contWorkItemId++;
            testStep = new TestStep();
            testCase.WriteFirstLine = CheckTestStepTags(machine, testCase);
            usedFiles = new List<CsvParamFile>();
            foreach (Transition tran in listTransition)
            {
                readFile = false;
                bool isCycle = false;
                bool lastCycleTrans = false;
                if (tran.GetTaggedValue("TDlastCycleTrans") != null)
                {
                    lastCycleTrans = (tran.GetTaggedValue("TDcycleTran").Equals("true") ? true : false);
                }
                if (tran.GetTaggedValue("TDcycleTran") != null)
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
                    testStep.Description = GenerateDescription(tran);
                    testStep.ExpectedResult = GenerateExpectedResult(tran);
                    testCase.TestSteps.Add(testStep);
                    index++;
                }
                else
                {
                    testStep = new TestStep();
                    testStep.Index = index.ToString();
                    testStep.Description = GenerateDescription(tran);
                    testStep.ExpectedResult = GenerateExpectedResult(tran);
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

        private Boolean CheckTestStepTags(FiniteStateMachine machine, TestCase testCase)
        {
            Boolean ret = false;
            foreach (KeyValuePair<String, String> pair in machine.TaggedValues)
            {
                switch (pair.Key)
                {
                    case "TDSTATE":
                        testCase.TDstate = HttpUtility.UrlDecode(pair.Value);
                        break;
                    case "TDASSIGNED":
                        testCase.TDassigned = HttpUtility.UrlDecode(pair.Value);
                        break;
                    case "TDREASON":
                        testCase.TDreason = HttpUtility.UrlDecode(pair.Value);
                        break;
                    case "TDITERATIONPATH":
                        testCase.TDiterationPath = HttpUtility.UrlDecode(pair.Value);
                        break;
                    case "TDAREAPATH":
                        testCase.TDareaPath = HttpUtility.UrlDecode(pair.Value);
                        break;
                    case "TDAPPLICATION":
                        testCase.TDapplication = HttpUtility.UrlDecode(pair.Value);
                        break;
                    case "TDCOMPLEXITY":
                        testCase.TDcomplexity = HttpUtility.UrlDecode(pair.Value);
                        break;
                    case "TDRISKS":
                        testCase.TDrisks = HttpUtility.UrlDecode(pair.Value);
                        break;
                    case "TDTCLIFECYCLE":
                        testCase.TDtcLifecycle = HttpUtility.UrlDecode(pair.Value);
                        break;
                    case "TDLIFECYCLETYPE":
                        testCase.TDlifecycleType = HttpUtility.UrlDecode(pair.Value);
                        break;
                    case "TDTCTEAMUSAGE":
                        testCase.TDtcTeamUsage = HttpUtility.UrlDecode(pair.Value);
                        break;
                    case "TDPOSTCONDITIONS":
                        testCase.TDpostConditions = HttpUtility.UrlDecode(pair.Value);
                        ret = true;
                        break;
                    case "TDPRECONDITIONS":
                        testCase.TDpreConditions = HttpUtility.UrlDecode(pair.Value);
                        ret = true;
                        break;
                    default:
                        break;
                }
            }
            return ret;
        }

        private String GenerateDescription(Transition tran)
        {
            String aux = tran.TargetState.Name + Environment.NewLine + Environment.NewLine;
            bool cycle = false;
            if (tran.GetTaggedValue("TDCYCLETRAN") != null)
                cycle = (tran.GetTaggedValue("TDCYCLETRAN").Equals("true") ? true : false);
            if (!String.IsNullOrEmpty(aux))
            {
                String TDaction = HttpUtility.UrlDecode(tran.GetTaggedValue("TDACTION"));
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

        private String GenerateExpectedResult(Transition tran)
        {
            String TDexpectedResult = HttpUtility.UrlDecode(tran.GetTaggedValue("TDEXPECTEDRESULT"));
            String aux;
            bool cycle = false;
            if (tran.GetTaggedValue("TDCYCLETRAN") != null)
                cycle = (tran.GetTaggedValue("TDCYCLETRAN").Equals("true") ? true : false);
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

        private String FillTD(String tdAction, bool useCyclePointer)
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
