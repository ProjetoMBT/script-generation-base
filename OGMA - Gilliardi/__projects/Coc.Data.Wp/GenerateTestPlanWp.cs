using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using Coc.Data.CSV;
using Coc.Modeling.TestPlanStructure;
using Coc.Modeling.FiniteStateMachine;

namespace Coc.Data.Wpartial
{
    public class GenerateTestPlanWp
    {
        #region Attributes
        public List<CsvParamFile> paramFiles { get; set; }
        private int currLine = 0;
        private int maxLine = 0;
        private bool doAgain = false;
        private Regex param = new Regex(@"(?<param>{(?<file>[ZéúíóáÉÚÍÓÁèùìòàÈÙÌÒÀõãñÕÃÑêûîôâÊÛÎÔÂëÿüïöäËYÜÏÖÄçÇ\sa-zA-Z0-9_!#$%&'+\/=?^`{|}~-]*).(?<column>[ZéúíóáÉÚÍÓÁèùìòàÈÙÌÒÀõãñÕÃÑêûîôâÊÛÎÔÂëÿüïöäËYÜÏÖÄçÇ\sa-zA-Z0-9_!#$%&'+\/=?^`{|}~-]*)})", RegexOptions.IgnoreCase);
        private List<CsvParamFile> usedFiles;
        private bool readFile;
        #endregion

        #region Public Methods
        public TestPlan PopulateTestPlan(List<List<String>>matriz, FiniteStateMachine machine, List<CsvParamFile> paramFiles)
        {
            this.paramFiles = paramFiles;
            TestPlan testPlan = new TestPlan();
            PopulateTestCase(matriz, machine, testPlan);

            return testPlan;
        }

        public int GetUsedFilesLineCount(string TaggedValue)
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
        #endregion

        #region Private Methods
        private Transition GetTransitionFSM(string input, FiniteStateMachine fsm)
        {
            foreach (Transition t in fsm.Transitions)
            {
                if (!t.Isvisited) 
                {
                    if (t.Input.Equals(input)) 
                    {
                        t.Isvisited = true;
                        return t;
                    }
                }
            }
            return null;
        }

        private void PopulateTestCase(List<List<String>> matriz, FiniteStateMachine machine, TestPlan testPlan)
        {
            for (int k = 0; k < matriz.Count; k++)
            {
                ResetMark(machine);
                List<Transition> listTransition = new List<Transition>();
                List<String> arraySequence = matriz[k];
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

        private void ResetMark(FiniteStateMachine machine)
        {
            foreach  (Transition t in machine.Transitions)
            {
                t.Isvisited = false;
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


            //if TDpreConditions or TDpostConditions exists
            //if (!String.IsNullOrEmpty(testStep.Description) || !String.IsNullOrEmpty(testStep.ExpectedResult))
            //{
            //    testStep.Title = machine.Name;
            //    testStep.workItemIdString = "Test Case " + testCase.WorkItemId;
            //    testStep.Index = index.ToString();
            //    testStep.Description = "- " + testStep.Description.Replace(" | ", ";" + Environment.NewLine + "- ");
            //    testStep.Description = "Pre-Requirements" + Environment.NewLine + Environment.NewLine + testStep.Description;
            //    testCase.TestSteps.Add(testStep);
            //    added = true;
            //    index++;
            //}



            //   List<Transition> listUmlTransition = GetListUmlTransition(listTransition, model);
            usedFiles = new List<CsvParamFile>();
            foreach (Transition tran in listTransition)
            {
                readFile = false;
                bool isCycle = false;
                bool lastCycleTrans = false;
                if (tran.GetTaggedValue("TDlastCycleTrans") != null)
                {
                    lastCycleTrans = (tran.GetTaggedValue("TDcycleTran").Equals("true") ? true : false);
                    //doAgain = false;
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

                ////se existiam tags que não foram adicionadas anteriormente, não dá "new teststep"
                //if (existsTag && !added)
                //{
                //    testStep.Index = index.ToString();
                //    testStep.Description = GenerateDescription(tran);
                //    testStep.ExpectedResult = GenerateExpectedResult(tran);
                //    testCase.TestSteps.Add(testStep);
                //    index++;
                //    existsTag = false;
                //}
                //else
                // {
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
            //  }

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

        ///// <summary>
        ///// Transform a list of Transition in a list of UmlTransition
        ///// </summary>
        ///// <param name="listTransition"></param>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //private List<UmlTransition> GetListUmlTransition(List<Transition> listTransition, UmlModel model)
        //{
        //    List<UmlTransition> list = new List<UmlTransition>();
        //    for (int i = 0; i < listTransition.Count; i++)
        //    {
        //        Transition t = listTransition[i];
        //        Boolean contem = false;
        //        //UmlTransition a = GetUmlTransition(model, t);
        //        contem = GetNewTransition(model, list, t, contem);
        //    }
        //    return list;
        //}

        //private Boolean GetNewTransition(UmlModel model, List<UmlTransition> list, Transition t, Boolean contem)
        //{
        //    UmlTransition tranSource = GetUmlTransition(t.SourceState, model, true);
        //    UmlTransition tranTarget = GetUmlTransition(t.TargetState, model, false);
        //    UmlTransition newTransition = new UmlTransition();
        //    newTransition.Source = tranSource.Source;
        //    newTransition.Target = tranTarget.Target;
        //    newTransition.Id = tranSource.Id;
        //    foreach (KeyValuePair<String, String> pair in tranTarget.TaggedValues)
        //    {
        //        newTransition.SetTaggedValue(pair.Key, pair.Value);
        //    }
        //    if (t.CycleTransition)
        //        newTransition.SetTaggedValue("TDcycleTran", "true");
        //    if (t.EndCycle)
        //        newTransition.SetTaggedValue("TDlastCycleTrans", "true");
        //    list.Add(newTransition);
        //    contem = true;
        //    return contem;
        //}

        //private UmlTransition GetUmlTransition(UmlModel model, Transition t)
        //{
        //    UmlTransition tranSource = GetUmlTransition(t.SourceState, model, true);
        //    UmlTransition tranTarget = GetUmlTransition(t.TargetState, model, false);
        //    UmlTransition newTransition = new UmlTransition();
        //    newTransition.Source = tranSource.Source;
        //    newTransition.Target = tranTarget.Target;
        //    newTransition.Id = tranSource.Id;
        //    foreach (KeyValuePair<String, String> pair in tranTarget.TaggedValues)
        //    {
        //        newTransition.SetTaggedValue(pair.Key, pair.Value);
        //    }
        //    if (t.CycleTransition)
        //        newTransition.SetTaggedValue("TDcycleTran", "true");
        //    if (t.EndCycle)
        //        newTransition.SetTaggedValue("TDlastCycleTrans", "true");
        //    return newTransition;
        //}

        //private UmlTransition GetUmlTransition(State state, UmlModel model, Boolean p)
        //{
        //    foreach (UmlActivityDiagram act in model.Diagrams.OfType<UmlActivityDiagram>())
        //    {
        //        foreach (UmlTransition transition in act.UmlObjects.OfType<UmlTransition>())
        //        {
        //            if (p && state.Id.Equals(transition.Source.Id))
        //                return transition;
        //            if (!p && state.Id.Equals(transition.Target.Id))
        //                return transition;

        //            //if (p && state.Name.Equals(transition.Source.Name))
        //            //    return transition;
        //            //if (!p && state.Name.Equals(transition.Target.Name))
        //            //    return transition;
        //        }
        //    }
        //    return null;
        //}

        //private Boolean IsInclude(UmlUseCaseDiagram diagram, UmlUseCase useCase)
        //{
        //    String idActor = "";

        //    foreach (UmlActor item in diagram.UmlObjects.OfType<UmlActor>())
        //    {
        //        idActor = item.Id;
        //    }
        //    foreach (UmlAssociation item in diagram.UmlObjects.OfType<UmlAssociation>())
        //    {
        //        if ((item.End2.Id.Equals(useCase.Id) && item.End1.Id.Equals(idActor)) || (item.End1.Id.Equals(useCase.Id) && item.End2.Id.Equals(idActor)))
        //        {
        //            return false;
        //        }
        //    }
        //    return true;
        //}
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

        private string FillTD(string tdAction, bool useCyclePointer)
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
                                    //doAgain = true;
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
