using System;
using System.Collections.Generic;
using System.Linq;
using Coc.Modeling.FiniteStateMachine;
using Coc.Modeling.Uml;
using System.Web;
using System.Text.RegularExpressions;
using Coc.Data.CSV;
using Coc.Modeling.TestPlanStructure;

namespace Coc.Testing.Functional.Importers
{
    public class PopulateExcel
    {
        public List<CsvParamFile> paramFiles { get; set; }
        private int currLine = 0;
        private int maxLine = 0;
        private bool doAgain = false;
        private Regex param = new Regex(@"(?<param>{(?<file>[ZéúíóáÉÚÍÓÁèùìòàÈÙÌÒÀõãñÕÃÑêûîôâÊÛÎÔÂëÿüïöäËYÜÏÖÄçÇ\sa-zA-Z0-9_!#$%&'+\/=?^`{|}~-]*).(?<column>[ZéúíóáÉÚÍÓÁèùìòàÈÙÌÒÀõãñÕÃÑêûîôâÊÛÎÔÂëÿüïöäËYÜÏÖÄçÇ\sa-zA-Z0-9_!#$%&'+\/=?^`{|}~-]*)})", RegexOptions.IgnoreCase);
        private List<CsvParamFile> usedFiles;
        private bool readFile;

        public TestPlan PopulateTestPlan(String[][] matriz, FiniteStateMachine machine, UmlModel model, List<CsvParamFile> paramFiles)
        {
            this.paramFiles = paramFiles;
            TestPlan testPlan = new TestPlan();
            UmlUseCaseDiagram useCaseDiagram = model.Diagrams.OfType<UmlUseCaseDiagram>().FirstOrDefault();
            //Get the use case that corresponds to the current machine
            UmlUseCase useCase = GetUseCase(machine, model);

            if (!IsInclude(useCaseDiagram, useCase))
            {
                PopulateTestCase(matriz, machine, model, testPlan, useCase);
            }
            return testPlan;
        }

        private void PopulateTestCase(String[][] matriz, FiniteStateMachine machine, UmlModel model, TestPlan testPlan, UmlUseCase useCase)
        {
            for (int k = 0; k < matriz.Length; k++)
            {
                testPlan.Name = useCase.Name;
                Transition t = new Transition();
                List<Transition> listTransition = new List<Transition>();
                String[] arraySequence = matriz[k];
                int maxUseCaseLines = int.MaxValue;
                foreach (String input in arraySequence)
                {
                    t = GetTransition(input, t.TargetState, machine);
                    if (t != null)
                    {
                        listTransition.Add(t);
                        UmlTransition tran = GetUmlTransition(model, t);

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
                    testCase = FillTestCase(model, useCase, listTransition, testCase);
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
            foreach (CsvParamFile file in paramFiles)
            {
                if (file.LinesCount < maxLine)
                    maxLine = file.LinesCount;
            }
            doAgain = false;
        }

        private void ResetParamFilesPointers(int maxValue)
        {
            currLine = 0;
            maxLine = maxValue;
            doAgain = false;
        }

        private TestCase FillTestCase(UmlModel model, UmlUseCase useCase, List<Transition> listTransition, TestCase testCase)
        {
            int index = 1;
            TestStep testStep;
            Boolean existsTag = false;
            Boolean added = false;

            testCase = new TestCase(HttpUtility.UrlDecode(useCase.Name));
            testCase.Title += "_" + TestCase.contWorkItemId;
            //testCase.TestCaseId = TestCase.contWorkItemId;
            testCase.WorkItemId = TestCase.contWorkItemId;
            TestCase.contWorkItemId++;
            testStep = new TestStep();
            
            existsTag = CheckTestStepTags(useCase, testStep, existsTag);
            
            //if TDpreConditions or TDpostConditions exists
            if (!String.IsNullOrEmpty(testStep.Description) || !String.IsNullOrEmpty(testStep.ExpectedResult))
            {
                testStep.Title = useCase.Name;
             //   testStep.workItemIdString = "Test Case " + testCase.WorkItemId;
                testStep.Index = index.ToString();
                testStep.Description = "- " + testStep.Description.Replace(" | ", ";" + Environment.NewLine + "- ");
                testStep.Description = "Pre-Requirements" + Environment.NewLine + Environment.NewLine + testStep.Description;
                testCase.TestSteps.Add(testStep);
                added = true;
                index++;
            }
            
            UmlActivityDiagram actDiagram = model.Diagrams
                          .OfType<UmlActivityDiagram>()
                          .Where(y => y.Name == useCase.Name)
                          .FirstOrDefault();

            if (actDiagram != null)
            {
                List<UmlTransition> listUmlTransition = GetListUmlTransition(listTransition, model);
                usedFiles = new List<CsvParamFile>();
                foreach (UmlTransition tran in listUmlTransition)
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

                    //se existiam tags que não foram adicionadas anteriormente, não dá "new teststep"
                    if (existsTag && !added)
                    {
                        testStep.Index = index.ToString();
                        testStep.Description = GenerateDescription(tran);
                        testStep.ExpectedResult = GenerateExpectedResult(tran);
                        testCase.TestSteps.Add(testStep);
                        index++;
                        existsTag = false;
                    }
                    else
                    {
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
                }
            }
            else
            {
                testCase = null;
            }
            return testCase;
            
        }

        private Boolean CheckTestStepTags(UmlUseCase useCase, TestStep testStep, Boolean existsTag)
        {
            foreach (KeyValuePair<String, String> pair in useCase.TaggedValues)
            {
                switch (pair.Key)
                {
                    //case "TDSTATE":
                    //    testStep.TDstate = HttpUtility.UrlDecode(pair.Value);
                    //    existsTag = true;
                    //    break;
                    //case "TDASSIGNED":
                    //    testStep.TDassigned = HttpUtility.UrlDecode(pair.Value);
                    //    existsTag = true;
                    //    break;
                    //case "TDREASON":
                    //    testStep.TDreason = HttpUtility.UrlDecode(pair.Value);
                    //    existsTag = true;
                    //    break;
                    //case "TDITERATIONPATH":
                    //    testStep.TDiterationPath = HttpUtility.UrlDecode(pair.Value);
                    //    existsTag = true;
                    //    break;
                    //case "TDAREAPATH":
                    //    testStep.TDareaPath = HttpUtility.UrlDecode(pair.Value);
                    //    existsTag = true;
                    //    break;
                    //case "TDAPPLICATION":
                    //    testStep.TDapplication = HttpUtility.UrlDecode(pair.Value);
                    //    existsTag = true;
                    //    break;
                    //case "TDCOMPLEXITY":
                    //    testStep.TDcomplexity = HttpUtility.UrlDecode(pair.Value);
                    //    existsTag = true;
                    //    break;
                    //case "TDRISKS":
                    //    testStep.TDrisks = HttpUtility.UrlDecode(pair.Value);
                    //    existsTag = true;
                    //    break;
                    //case "TDTCLIFECYCLE":
                    //    testStep.TDtcLifecycle = HttpUtility.UrlDecode(pair.Value);
                    //    existsTag = true;
                    //    break;
                    //case "TDLIFECYCLETYPE":
                    //    testStep.TDlifecycleType = HttpUtility.UrlDecode(pair.Value);
                    //    existsTag = true;
                    //    break;
                    //case "TDTCTEAMUSAGE":
                    //    testStep.TDtcTeamUsage = HttpUtility.UrlDecode(pair.Value);
                    //    existsTag = true;
                    //    break;
                    //case "TDPOSTCONDITIONS":
                    //    testStep.ExpectedResult = HttpUtility.UrlDecode(pair.Value);
                    //    break;
                    //case "TDPRECONDITIONS":
                    //    testStep.Description += HttpUtility.UrlDecode(pair.Value);
                    //    break;
                    //default:
                    //    break;
                }
            }
            return existsTag;
        }

        private UmlUseCase GetUseCase(FiniteStateMachine machine, UmlModel model)
        {
            foreach (UmlUseCaseDiagram item in model.Diagrams.OfType<UmlUseCaseDiagram>())
            {
                foreach (UmlUseCase useCase in item.UmlObjects.OfType<UmlUseCase>())
                {
                    if (machine.Name.Equals(useCase.Name))
                    {
                        return useCase;
                    }
                }
            }
            return null;
        }

        private Transition GetTransition(String input, State target, FiniteStateMachine machine)
        {
            foreach (Transition t in machine.Transitions)
            {
                if (target == null)
                {
                    if (t.Input.Equals(input))
                    {
                        return t;
                    }
                }
                if (t.Input.Equals(input) && t.SourceState.Equals(target))
                {
                    return t;
                }
            }
            return null;
        }

        /// <summary>
        /// Transform a list of Transition in a list of UmlTransition
        /// </summary>
        /// <param name="listTransition"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        private List<UmlTransition> GetListUmlTransition(List<Transition> listTransition, UmlModel model)
        {
            List<UmlTransition> list = new List<UmlTransition>();
            for (int i = 0; i < listTransition.Count; i++)
            {
                Transition t = listTransition[i];
                Boolean contem = false;
                //UmlTransition a = GetUmlTransition(model, t);
                contem = GetNewTransition(model, list, t, contem);
            }
            return list;
        }

        private Boolean GetNewTransition(UmlModel model, List<UmlTransition> list, Transition t, Boolean contem)
        {
            UmlTransition tranSource = GetUmlTransition(t.SourceState, model, true);
            UmlTransition tranTarget = GetUmlTransition(t.TargetState, model, false);
            UmlTransition newTransition = new UmlTransition();
            newTransition.Source = tranSource.Source;
            newTransition.Target = tranTarget.Target;
            newTransition.Id = tranSource.Id;
            foreach (KeyValuePair<String, String> pair in tranTarget.TaggedValues)
            {
                newTransition.SetTaggedValue(pair.Key, pair.Value);
            }
            if (t.CycleTransition)
                newTransition.SetTaggedValue("TDcycleTran", "true");
            if (t.EndCycle)
                newTransition.SetTaggedValue("TDlastCycleTrans", "true");
            list.Add(newTransition);
            contem = true;
            return contem;
        }

        private UmlTransition GetUmlTransition(UmlModel model, Transition t)
        {
            UmlTransition tranSource = GetUmlTransition(t.SourceState, model, true);
            UmlTransition tranTarget = GetUmlTransition(t.TargetState, model, false);
            UmlTransition newTransition = new UmlTransition();
            newTransition.Source = tranSource.Source;
            newTransition.Target = tranTarget.Target;
            newTransition.Id = tranSource.Id;
            foreach (KeyValuePair<String, String> pair in tranTarget.TaggedValues)
            {
                newTransition.SetTaggedValue(pair.Key, pair.Value);
            }
            if (t.CycleTransition)
                newTransition.SetTaggedValue("TDcycleTran", "true");
            if (t.EndCycle)
                newTransition.SetTaggedValue("TDlastCycleTrans", "true");
            return newTransition;
        }

        private UmlTransition GetUmlTransition(State state, UmlModel model, Boolean p)
        {
            foreach (UmlActivityDiagram act in model.Diagrams.OfType<UmlActivityDiagram>())
            {
                foreach (UmlTransition transition in act.UmlObjects.OfType<UmlTransition>())
                {
                    if (p && state.Id.Equals(transition.Source.Id))
                        return transition;
                    if (!p && state.Id.Equals(transition.Target.Id))
                        return transition;
                    
                    //if (p && state.Name.Equals(transition.Source.Name))
                    //    return transition;
                    //if (!p && state.Name.Equals(transition.Target.Name))
                    //    return transition;
                }
            }
            return null;
        }

        private Boolean IsInclude(UmlUseCaseDiagram diagram, UmlUseCase useCase)
        {
            String idActor = "";

            foreach (UmlActor item in diagram.UmlObjects.OfType<UmlActor>())
            {
                idActor = item.Id;
            }
            foreach (UmlAssociation item in diagram.UmlObjects.OfType<UmlAssociation>())
            {
                if ((item.End2.Id.Equals(useCase.Id) && item.End1.Id.Equals(idActor)) || (item.End1.Id.Equals(useCase.Id) && item.End2.Id.Equals(idActor)))
                {
                    return false;
                }
            }
            return true;
        }

        private String GenerateDescription(UmlTransition tran)
        {
            String aux = tran.Target.Name + Environment.NewLine + Environment.NewLine;
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

        private string FillTD(string tdAction)
        {
            return FillTD(tdAction, false);
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

        private String GenerateExpectedResult(UmlTransition tran)
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


                //colocar info


                foreach (TestStep st in testCase.TestSteps)
                {
                    if (passed)
                    {
                       // st.workItemIdString = "Test Case " + testCase.TestCaseId;
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
         
    }
}
