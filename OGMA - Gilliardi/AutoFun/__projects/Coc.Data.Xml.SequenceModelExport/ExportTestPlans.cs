using System.Collections.Generic;
using System.Xml;
using Coc.Modeling.TestPlanStructure;

namespace Coc.Data.Xml.SequenceModelExport
{
    /*
    /// <summary>
    /// <img src="images/Xml.SequenceModelExport.PNG"/>
    /// </summary>
    public static class NamespaceDoc
    {
    }*/


    public class ExportTestPlans
    {
        public XmlDocument ToExport(List<TestPlan> listTestPlans)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml("<XML></XML>");
            XmlElement header = document.CreateElement("ListTestPlans");
            document.DocumentElement.AppendChild(header);
            foreach (TestPlan testPlan in listTestPlans)
            {
                XmlElement testPlanXml = document.CreateElement("TestPlan");
                testPlanXml.SetAttribute("Name", testPlan.Name);
                header.AppendChild(testPlanXml);
                foreach (TestCase testCase in testPlan.TestCases)
                {
                    XmlElement testCaseXml = document.CreateElement("TestCase");
                    testPlanXml.AppendChild(testCaseXml);
                    testCaseXml.SetAttribute("Summary", testCase.Summary);
                    testCaseXml.SetAttribute("TestCaseId", testCase.WorkItemId + "");
                    testCaseXml.SetAttribute("Title", testCase.Title);
                    testCaseXml.SetAttribute("WorkItemId", testCase.WorkItemId + "");
                    foreach (TestStep testStep in testCase.TestSteps)
                    {
                        XmlElement testStepXml = document.CreateElement("TestCase");
                        testStepXml.SetAttribute("Description", testCase.TDpreConditions);
                        testStepXml.SetAttribute("ExpectedResult", testCase.TDpostConditions);
                        //testStepXml.SetAttribute("Index", testCase.Index + "");
                        testStepXml.SetAttribute("TDAPPLICATION", testCase.TDapplication);
                        testStepXml.SetAttribute("TDAREAPATH", testCase.TDareaPath);
                        testStepXml.SetAttribute("TDASSIGNED", testCase.TDassigned);
                        testStepXml.SetAttribute("TDCOMPLEXITY", testCase.TDcomplexity);
                        testStepXml.SetAttribute("TDITERATIONPATH", testCase.TDiterationPath);
                        testStepXml.SetAttribute("TDLIFECYCLETYPE", testCase.TDlifecycleType);
                        testStepXml.SetAttribute("TDREASON", testCase.TDreason);
                        testStepXml.SetAttribute("TDRISKS", testCase.TDrisks);
                        testStepXml.SetAttribute("TDSTATE", testCase.TDstate);
                        testStepXml.SetAttribute("TDTCLIFECYCLE", testCase.TDtcLifecycle);
                        testStepXml.SetAttribute("TDTCTEAMUSAGE", testCase.TDtcTeamUsage);
                        testStepXml.SetAttribute("Title", testCase.Title);
                        testStepXml.SetAttribute("workItemIdString", testCase.WorkItemId + "");
                        testCaseXml.AppendChild(testStepXml);
                    }
                }
            }

            //XmlWriterSettings settings = new XmlWriterSettings();
            //settings.Encoding = new UTF8Encoding(false);
            //settings.Indent = true;
            //settings.CheckCharacters = true;

            //SaveFileDialog saveFileDialog = new SaveFileDialog();
            //saveFileDialog.Filter = "Xml (*.xml)|*.xml";
            //if (saveFileDialog.ShowDialog() == DialogResult.OK)
            //{
            //    using (XmlWriter writer = XmlWriter.Create(saveFileDialog.FileName, settings))
            //        document.Save(writer);

            //}

            return document;
        }
    }
}
