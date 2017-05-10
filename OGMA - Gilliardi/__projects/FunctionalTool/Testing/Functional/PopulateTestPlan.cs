using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FunctionalTool.Modeling.Uml;
using System.Windows;
using System.Web;
using System.Diagnostics;
using System.Windows.Forms;
using FunctionalTool.Exceptions;
using System.Text.RegularExpressions;

namespace FunctionalTool.Testing.Functional
{
    public class PopulateTestPlan
    {
        static int SharedStepId = 001;
        static bool isLastStep = true;

        public class TreeNode
        {
            public List<TestCase> list = new List<TestCase>();
        }

        public static TestPlan PopulateTP(UmlUseCaseDiagram UseCaseDiagram, Dictionary<String, UmlActionStateDiagram> dicActionDiagram)
        {
            int index=2;

            TestPlan testPlan = new TestPlan(); 

            foreach (String keyUC in UseCaseDiagram.useCases.Keys)
            {
                UmlUseCase useCase = UseCaseDiagram.useCases[keyUC];

                if (useCase.includeList.Count == 0)
                {

                        TestCase testCase = new TestCase(HttpUtility.UrlDecode(useCase.Name));

                        testCase.Title += "_" + TestCase.contWorkItemId;
                        testCase.WorkItemId = TestCase.contWorkItemId;
                        TestCase.contWorkItemId++;

                        TestStep testStep = new TestStep();

                        testStep.FTstate = HttpUtility.UrlDecode(useCase.FTstate);
                        testStep.FTassigned = HttpUtility.UrlDecode(useCase.FTassigned);
                        testStep.FTreason = HttpUtility.UrlDecode(useCase.FTreason);
                        testStep.FTiterationPath = HttpUtility.UrlDecode(useCase.FTiterationPath);
                        testStep.FTareaPath = HttpUtility.UrlDecode(useCase.FTareaPath);
                        testStep.FTapplication = HttpUtility.UrlDecode(useCase.FTapplication);
                        testStep.FTcomplexity = HttpUtility.UrlDecode(useCase.FTcomplexity);
                        testStep.FTrisks = HttpUtility.UrlDecode(useCase.FTrisks);
                        testStep.FTtcLifecycle = HttpUtility.UrlDecode(useCase.FTtcLifecycle);
                        testStep.FTlifecycleType = HttpUtility.UrlDecode(useCase.FTlifecycleType);
                        testStep.FTtcTeamUsage = HttpUtility.UrlDecode(useCase.FTtcTeamUsage);

                        testStep.Title = useCase.Name;
                        testStep.workItemIdString = "Test Case " + testCase.WorkItemId;
                        testStep.Index += index;
                        testStep.Description = "";
                        testStep.ExpectedResult =   HttpUtility.UrlDecode(useCase.posCondition);

                        foreach (String tagKey in useCase.dictionaryTag.Keys)
                        {
                            testStep.Description += useCase.dictionaryTag[tagKey].value;
                        }

                        if (testStep.Description != null)
                        {
                            testStep.Description = HttpUtility.UrlDecode(testStep.Description).Replace("|", Environment.NewLine + "-");
                        }

                        if (testStep.Description.Contains("["))
                        {

                            testStep.Description = testStep.Description.Replace("[", Environment.NewLine + "[-");
                            testStep.Description = testStep.Description.Replace("[", "[/");
                            testStep.Description = testStep.Description.Replace("]", "/]");

                            String[] list = testStep.Description.Split(new char[] { '[', ']' });


                            for (int i = 0; i < list.Count() - 1; i++)
                            {
                                if (list[i].ToArray()[0] == '/')
                                {
                                    list[i] = list[i].Replace("/", "");
                                    list[i] = list[i].Replace("-", "  -");
                                }
                            }

                            testStep.Description = String.Join("", list);

                        }

                        if (testStep.Description != "")
                        {
                            testStep.Description = "Pre-Requirements" + Environment.NewLine + Environment.NewLine + testStep.Description;
                        }

                        testCase.TestSteps.Add(testStep);

                        foreach (var item in useCase.dicTagHyperLinkUseCase.Keys)
                        {
                            UmlActionStateDiagram actionDiagram = useCase.dicTagHyperLinkUseCase[item];
                            String idInicio = actionDiagram.InitialActivity.outgoing;
                            UmlTransition t = actionDiagram.buscaTransition(idInicio, dicActionDiagram);
                            testStep.Title = actionDiagram.Name;
                            int subIndex = 0;
                            populateActionDiagram("", testStep, "", t, actionDiagram, dicActionDiagram, testCase, index, subIndex, false);
                        }
                        if (testCase.TestSteps.Count > 1)
                        {
                            testCase.TestCaseId = TestCase.contTestCaseId++;
                            testPlan.TestCases.Add(testCase);
                        }
                   
                }
            }

            PopulateTestPlan.ExpectedResultValidation(testPlan);

            return testPlan;
        }

        private static void ExpectedResultValidation(TestPlan testPlan)
        {
                //search for problems in expectedResults list 
                foreach (var testCase in testPlan.TestCases)
                {
                    
                    if (testCase.TestSteps.Count > 1)
                    {
                        foreach (var testStep in testCase.TestSteps)
                        {
                            if (testStep.ExpectedResult == "")
                            {
                                DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Error. \nTest Case: " + testCase.Title + "\nIndex:[" + testStep.Index + "] don't have expected result. Are you sure you want to continue executing a parsing method?", "Error", MessageBoxButtons.YesNo);
                                    if (dialogResult == DialogResult.No)
                                    {
                                        throw new InvalidExpectedResult();
                                    }
                            }
                        }
                    }
                }
        }
        
        private static TestStep populateActionDiagram(String activityName ,TestStep testStep ,String workTitle, UmlTransition t, UmlActionStateDiagram activityDiagram, Dictionary<String, UmlActionStateDiagram> dicActionDiagram, TestCase testCase,int index, int subIndex, Boolean begin)
        {
            isLastStep = true;
            TestStep lastStep = null;
            String idInicio = "";
            try
            {
                if (t.Target != activityDiagram.FinalActivity.Id)
                {
                    UmlActionState activity = activityDiagram.dicAtivities[t.Target];
                    if (activity.dicMyLinkedDiagrams.Count == 0)
                    {
                        testStep = new TestStep();

                        testStep.Description = activity.Name + Environment.NewLine;

                        if (begin == true)
                        {
                            testStep.Title = testCase.Title;
                            testStep.workItemIdString = "Test Case" + testCase.WorkItemId;
                            begin = false;
                        }
                        else
                        {
                            testStep.Title = activityName;
                            testStep.workItemIdString = workTitle;
                            workTitle = "";
                        }

                        testStep.ExpectedResult = getExpectedResults(t);

                        if(returnTagTransition(t) == "")
                        {
                            DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Error. \nTest Case: " + testCase.Title + "\nIndex:[" + testStep.Index + "] don't have description/action result. Are you sure you want to continue executing a parsing method?", "Error", MessageBoxButtons.YesNo);
                            if (dialogResult == DialogResult.No)
                            {
                                throw new InvalidDescription();
                            }
                        }

                        testStep.Description += "|" + returnTagTransition(t);
                        testStep.Description = HttpUtility.UrlDecode(testStep.Description);
                        testStep.Description = testStep.Description.Replace("|", Environment.NewLine + "- ");
                        testCase.TestSteps.Add(testStep);
                       
                        if (subIndex >= 1)
                        {
                            testStep.Index += "" + index + "." + subIndex;
                        }
                        else
                        {
                            testStep.Index += "" + index;
                        }
                    }

                    if (activity.dicMyLinkedDiagrams.Count != 0)
                    {
                        foreach (String item in activity.dicJudeHyperLink.Keys)
                        {
                            subIndex = 1;
                            String chave = UmlActionStateDiagram.dicJudeHyperLinks[item];
                            UmlActionStateDiagram actionDiagram = activity.dicMyLinkedDiagrams[chave];
                            
                            idInicio = actionDiagram.InitialActivity.outgoing;
                            t = actionDiagram.buscaTransition(idInicio, dicActionDiagram);

                            int sharedId = SharedStepId;
                            TestStep test = populateActionDiagram(HttpUtility.UrlDecode(actionDiagram.Name), testStep, "Shared Steps " + sharedId.ToString("000"), t, actionDiagram, dicActionDiagram, testCase, index, subIndex, false);
                            if (test != null)
                            {
                                test.workItemIdString = "End of Shared Steps " + sharedId.ToString("000");
                                SharedStepId++;
                                begin = true;
                                isLastStep = true;
                            }
                            isLastStep = true;
                            subIndex = 0;
                        }
                    }
                    if (subIndex == 0)
                    {
                        index++;
                    }
                    else
                    {
                        subIndex++;
                    }
                    idInicio = activity.outgoing;
                    t = activityDiagram.buscaTransition(idInicio, dicActionDiagram);

                    lastStep = populateActionDiagram("", testStep, workTitle, t, activityDiagram, dicActionDiagram, testCase, index, subIndex, begin);

                    if (isLastStep == true)
                    {
                        lastStep = testStep;
                    }
                    isLastStep = false;
                }
            
            }
            catch (Exception)
            {
                throw new InvalidTransition();
            }
            return lastStep;
        }

        private static string getExpectedResults(UmlTransition transition)
        {
            String result = "  ";
            foreach (var expectResult in transition.listExpectedResults)
            {
                result += HttpUtility.UrlDecode(expectResult.value);
            }
            return result.Trim();
        }

        public static String returnTagTransition(UmlTransition t) {
            String tagText = ""; 
            foreach (String tagKey in t.dictionaryTag.Keys)
            {
                tagText += t.dictionaryTag[tagKey].value;
                tagText = HttpUtility.UrlDecode(tagText);     
            }

            tagText = tagText.Replace("[", Environment.NewLine + " -" );
            tagText = tagText.Replace("]", "");

            return tagText;
        }
    }
}
