using System;
using System.Collections.Generic;
using System.Web;
using Coc.Data.Interfaces;
using Coc.Modeling.TestPlanStructure;
using DocumentFormat.OpenXml.Spreadsheet;
using SpreadsheetLight;
using Coc.Data.ControlAndConversionStructures;

namespace Coc.Data.Excel
{
    /*
    /// <summary>
    /// <img src="images/Excel.PNG"/>
    /// </summary>
    public static class NamespaceDoc
    {
    }*/


    public class MTMScriptGenerator : ScriptGenerator
    {
        #region Public Methods
        public void GenerateScript(List<GeneralUseStructure> listPlanStructure, String path)
        {
            GenerateXlsFromTestPlan(path, listPlanStructure);
        }
       

        #endregion

        #region Private Methods
        private void GenerateXlsFromTestPlan(String path, List<GeneralUseStructure> listPlanStructure)
        {
            List<TestCase> listCase = new List<TestCase>();
            #region Styles and SLDocument
            SLDocument xlsx = new SLDocument();
            SLStyle style = xlsx.CreateStyle();
            style.SetWrapText(true);
            SLStyle indexStyle = xlsx.CreateStyle();
            indexStyle.Alignment.Horizontal = HorizontalAlignmentValues.Right;
            indexStyle.SetWrapText(true);
            SLStyle headerStyle = xlsx.CreateStyle();
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.Yellow, System.Drawing.Color.Black);
            headerStyle.Font.Bold = true;
            headerStyle.Font.FontName = "Calibri";
            headerStyle.Border.RightBorder.Color = System.Drawing.Color.Black;
            headerStyle.Border.LeftBorder.Color = System.Drawing.Color.Black;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            xlsx.DeleteWorksheet(SLDocument.DefaultFirstSheetName);
            #endregion
            #region Populate Excel

            foreach (GeneralUseStructure planStructure in listPlanStructure)
            {
                TestPlan testPlan = (TestPlan)planStructure;

                for (int k = 0; k < testPlan.TestCases.Count; k++)
                {
                    TestCase testCase = testPlan.TestCases[k];
                    listCase.Add(testCase);
                    xlsx.SetColumnStyle(1, 1, style);
                    //xlsx.SetColumnWidth(1, 1, 50);
                    String title = testCase.Title;
                    title = title.Replace(" ", "");

                    if (title.Length > 31)
                    {
                        title = title.Substring(0, 25) + title.Substring(title.Length - 5, 5);
                    }
                    xlsx.AddWorksheet(HttpUtility.UrlDecode(title));
                    xlsx.SelectWorksheet(title);

                    xlsx.SetCellValue(1, 1, "Test Case #");
                    xlsx.SetCellValue(1, 2, "Work Item ID");
                    xlsx.SetCellValue(1, 3, "Test Title");
                    xlsx.SetCellValue(1, 4, "Summary");
                    xlsx.SetCellValue(1, 5, "Test Step");
                    xlsx.SetCellValue(1, 6, "Action/Description");
                    xlsx.SetCellValue(1, 7, "Expected Results");
                    xlsx.SetCellValue(1, 8, "Assigned To");
                    xlsx.SetCellValue(1, 9, "State");
                    xlsx.SetCellValue(1, 10, "Reason");
                    xlsx.SetCellValue(1, 11, "Iteration Path");
                    xlsx.SetCellValue(1, 12, "Area Path");
                    xlsx.SetCellValue(1, 13, "Application");
                    xlsx.SetCellValue(1, 14, "Complexity");
                    xlsx.SetCellValue(1, 15, "Risks");
                    xlsx.SetCellValue(1, 16, "TC Lifecycle");
                    xlsx.SetCellValue(1, 17, "Lifecycle Type");
                    xlsx.SetCellValue(1, 18, "TC Team Usage");
                    xlsx.SetCellValue(1, 19, "Category");
                    xlsx.SetCellValue(1, 20, "Automation Complexity");
                    xlsx.SetCellStyle("A1", "T1", headerStyle);
                    xlsx.SetCellValue(2, 1, "TC" + testCase.WorkItemId);
                    xlsx.SetCellStyle(2, 1, style);
                    xlsx.SetCellValue(2, 2, "Test Case " + HttpUtility.UrlDecode(testCase.WorkItemId.ToString()));
                    xlsx.SetCellStyle(2, 2, style);
                    if (k != (testPlan.TestCases.Count - 1))
                    {
                        xlsx.SetCellValue(2, 3, HttpUtility.UrlDecode(testCase.Title));
                        xlsx.SetCellStyle(2, 3, style);
                    }
                    xlsx.SetCellValue(2, 4, HttpUtility.UrlDecode(testCase.Summary));
                    xlsx.SetCellStyle(2, 4, style);
                    xlsx.SetCellValue(2, 5, "1");
                    xlsx.SetCellStyle(2, 5, indexStyle);
                    xlsx.SetCellValue(2, 6, HttpUtility.UrlDecode(testCase.TDpreConditions));
                    xlsx.SetCellStyle(2, 6, style);
                    xlsx.SetCellValue(2, 7, HttpUtility.UrlDecode(testCase.TDpostConditions));
                    xlsx.SetCellStyle(2, 7, style);
                    xlsx.SetCellValue(2, 8, HttpUtility.UrlDecode(testCase.TDassigned));
                    xlsx.SetCellStyle(2, 8, style);
                    xlsx.SetCellValue(2, 9, HttpUtility.UrlDecode(testCase.TDstate));
                    xlsx.SetCellStyle(2, 9, style);
                    xlsx.SetCellValue(2, 10, HttpUtility.UrlDecode(testCase.TDreason));
                    xlsx.SetCellStyle(2, 10, style);
                    xlsx.SetCellValue(2, 11, HttpUtility.UrlDecode(testCase.TDiterationPath));
                    xlsx.SetCellStyle(2, 11, style);
                    xlsx.SetCellValue(2, 12, HttpUtility.UrlDecode(testCase.TDareaPath));
                    xlsx.SetCellStyle(2, 12, style);
                    xlsx.SetCellValue(2, 13, HttpUtility.UrlDecode(testCase.TDapplication));
                    xlsx.SetCellStyle(2, 13, style);
                    xlsx.SetCellValue(2, 14, HttpUtility.UrlDecode(testCase.TDcomplexity));
                    xlsx.SetCellStyle(2, 14, style);
                    xlsx.SetCellValue(2, 15, HttpUtility.UrlDecode(testCase.TDrisks));
                    xlsx.SetCellStyle(2, 15, style);
                    xlsx.SetCellValue(2, 16, HttpUtility.UrlDecode(testCase.TDtcLifecycle));
                    xlsx.SetCellStyle(2, 16, style);
                    xlsx.SetCellValue(2, 17, HttpUtility.UrlDecode(testCase.TDlifecycleType));
                    xlsx.SetCellStyle(2, 17, style);
                    xlsx.SetCellValue(2, 18, HttpUtility.UrlDecode(testCase.TDtcTeamUsage));
                    xlsx.SetCellStyle(2, 18, style);

                    #region Step
                    for (int i = 0; i < testCase.TestSteps.Count; i++)
                    {
                        TestStep step = testCase.TestSteps[i];
                        if (k == (testPlan.TestCases.Count - 1))
                        {
                            xlsx.SetCellValue(i + 2, 3, HttpUtility.UrlDecode(step.Title));
                            xlsx.SetCellStyle(i + 2, 3, style);
                        }
                        if (!testCase.WriteFirstLine)
                        {
                            xlsx.SetCellValue(i + 2, 5, HttpUtility.UrlDecode(step.Index));
                            xlsx.SetCellStyle(i + 2, 5, indexStyle);
                            if (step.Description.Contains("$@#ITERATION@#"))
                            {
                                String[] aux = step.Description.Split('$');

                                xlsx.SetCellValue(i + 2, 6, HttpUtility.UrlDecode(aux[0]));
                            }
                            else
                            {
                                xlsx.SetCellValue(i + 2, 6, HttpUtility.UrlDecode(step.Description));
                            }
                            xlsx.SetCellStyle(i + 2, 6, style);
                            if (step.ExpectedResult.Contains("$@#ITERATION@#"))
                            {
                                String[] aux = step.ExpectedResult.Split('$');

                                xlsx.SetCellValue(i + 2, 7, HttpUtility.UrlDecode(aux[0]));
                            }
                            else
                            {
                                xlsx.SetCellValue(i + 2, 7, HttpUtility.UrlDecode(step.ExpectedResult));
                            }
                            xlsx.SetCellStyle(i + 2, 7, style);
                        }
                        else
                        {
                            xlsx.SetCellValue(i + 3, 5, HttpUtility.UrlDecode((Int32.Parse(step.Index) + 1).ToString()));
                            xlsx.SetCellStyle(i + 3, 5, indexStyle);
                            if (step.Description.Contains("$@#ITERATION@#"))
                            {
                                String[] aux = step.Description.Split('$');

                                xlsx.SetCellValue(i + 3, 6, HttpUtility.UrlDecode(aux[0]));
                            }
                            else
                            {
                                xlsx.SetCellValue(i + 3, 6, HttpUtility.UrlDecode(step.Description));
                            }
                            xlsx.SetCellStyle(i + 3, 6, style);
                            if (step.ExpectedResult.Contains("$@#ITERATION@#"))
                            {
                                String[] aux = step.ExpectedResult.Split('$');

                                xlsx.SetCellValue(i + 3, 7, HttpUtility.UrlDecode(aux[0]));
                            }
                            else
                            {
                                xlsx.SetCellValue(i + 3, 7, HttpUtility.UrlDecode(step.ExpectedResult));
                            }
                            xlsx.SetCellStyle(i + 3, 7, style);
                        }
                        if (!testCase.Title.Equals("GeneralTestCase") && step.Index.Equals(1))
                        {
                            xlsx.SetCellValue(i + 2, 3, HttpUtility.UrlDecode(step.Title));
                        }

                    }
                    xlsx.AutoFitRow(1, 10000);
                    xlsx.AutoFitColumn(1, 20);
                    #endregion
                }
            }
            xlsx.DeleteWorksheet(SLDocument.DefaultFirstSheetName);
            xlsx.SaveAs(path + @"\Plan.xls");
           // xlsx.SaveAs(path + @"\Plan.xlxs");
            
            #endregion
        }
        #endregion
    }
}
