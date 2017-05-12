using System;
using System.Collections.Generic;
using System.Xml;
using Coc.Modeling.TestPlanStructure;


namespace Coc.Data.Xml.SequenceModelExport
{
    public class ImportTestPlans
    {
        public List<TestPlan> FromXmi(XmlDocument doc)
        {
            List<TestPlan> listTestPlan = new List<TestPlan>();
            XmlNodeList nodesWithActivityDiagrams = doc.SelectNodes("//TestPlan");
            foreach (XmlNode node in doc.SelectNodes("//TestPlan"))
            {
                TestPlan testPlan = new TestPlan();
                testPlan.Name = node.Attributes["Name"].Value;
                String id=node.Attributes["id"].Value;
                foreach (XmlNode testCaseXml in node.SelectNodes("//TestPlan[@id='" + id + "']//TestCase"))
                {

                    TestCase testCase = new TestCase(testCaseXml.Attributes["Title"].Value);
                    testCase.Summary = testCaseXml.Attributes["Summary"].Value;
                    //testCase.TestCaseId = int.Parse(testCaseXml.Attributes["TestCaseId"].Value);
                    testCase.WorkItemId = int.Parse(testCaseXml.Attributes["WorkItemId"].Value);
                  
                    foreach (XmlNode testStepXml in testCaseXml.SelectNodes("//TestPlan//TestCase[@TestCaseId='" + testCase.WorkItemId + "']//TestStep"))
                    {
                        TestStep testStep = new TestStep();
                        testStep.Description = testStepXml.Attributes["Description"].Value;
                        testStep.ExpectedResult = testStepXml.Attributes["ExpectedResult"].Value;
                        testStep.Index = testStepXml.Attributes["Index"].Value;
                        //testStep.TDapplication = testStepXml.Attributes["TDAPPLICATION"].Value;
                        //testStep.TDareaPath = testStepXml.Attributes["TDAREAPATH"].Value;
                        //testStep.TDassigned = testStepXml.Attributes["TDASSIGNED"].Value;
                        //testStep.TDcomplexity = testStepXml.Attributes["TDCOMPLEXITY"].Value;
                        //testStep.TDiterationPath = testStepXml.Attributes["TDITERATIONPATH"].Value;
                        //testStep.TDlifecycleType = testStepXml.Attributes["TDLIFECYCLETYPE"].Value;
                        //testStep.TDreason = testStepXml.Attributes["TDREASON"].Value;
                        //testStep.TDrisks = testStepXml.Attributes["TDRISKS"].Value;
                        //testStep.TDstate = testStepXml.Attributes["TDSTATE"].Value;
                        //testStep.TDtcLifecycle = testStepXml.Attributes["TDTCLIFECYCLE"].Value;
                        //testStep.TDtcTeamUsage = testStepXml.Attributes["TDTCTEAMUSAGE"].Value;
                        testStep.workItemIdString = testStepXml.Attributes["workItemIdString"].Value;
                        testStep.Title = testStepXml.Attributes["Title"].Value;
                        testCase.TestSteps.Add(testStep);
                    }
                    testPlan.TestCases.Add(testCase);
                }
                listTestPlan.Add(testPlan);
            }
            return listTestPlan;
        }
    }
}