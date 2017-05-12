using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Web;
using DocumentFormat;
using DocumentFormat.OpenXml;
using SpreadsheetLight;
using DocumentFormat.OpenXml.Spreadsheet;
using Coc.Testing.Functional;

namespace Coc.Data.Excel
{
    public class Excel
    {
        public static void GenerateXlsFromTestPlan(String path, List<TestPlan> ListPlans)
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
            foreach (TestPlan testPlan in ListPlans)
            {

                for (int k = 0; k < testPlan.TestCases.Count; k++)
                {
                    TestCase testCase = testPlan.TestCases[k];
                    listCase.Add(testCase);
                    xlsx.SetColumnStyle(1, 1, style);
                    xlsx.SetColumnWidth(1, 1, 50);
                    String title = testCase.Title;
                    title = title.Replace(" ", "");

                    if (title.Length > 31)
                    {
                        title = title.Substring(0, 25) + title.Substring(title.Length - 5, 5);
                    }

                    bool dssd = xlsx.AddWorksheet(HttpUtility.UrlDecode(title));
                    bool re = xlsx.SelectWorksheet(title);

                    xlsx.SetCellValue(1, 1, "Test Case #");
                    xlsx.SetColumnWidth(1, 13);
                    xlsx.SetCellValue(1, 2, "Work Item ID");
                    xlsx.SetColumnWidth(2, 22);
                    xlsx.SetCellValue(1, 3, "Test Title");
                    xlsx.SetColumnWidth(3, 30);
                    xlsx.SetCellValue(1, 4, "Summary");
                    xlsx.SetColumnWidth(4, 30);
                    xlsx.SetCellValue(1, 5, "Test Step");
                    xlsx.SetColumnWidth(5, 10);
                    xlsx.SetCellValue(1, 6, "Action/Description");
                    xlsx.SetColumnWidth(6, 60);
                    xlsx.SetCellValue(1, 7, "Expected Results");
                    xlsx.SetColumnWidth(7, 50);
                    xlsx.SetCellValue(1, 8, "Assigned To");
                    xlsx.SetColumnWidth(8, 20);
                    xlsx.SetCellValue(1, 9, "State");
                    xlsx.SetColumnWidth(9, 10);
                    xlsx.SetCellValue(1, 10, "Reason");
                    xlsx.SetColumnWidth(10, 10);
                    xlsx.SetCellValue(1, 11, "Iteration Path");
                    xlsx.SetColumnWidth(11, 30);
                    xlsx.SetCellValue(1, 12, "Area Path");
                    xlsx.SetColumnWidth(12, 10);
                    xlsx.SetCellValue(1, 13, "Application");
                    xlsx.SetColumnWidth(13, 13);
                    xlsx.SetCellValue(1, 14, "Complexity");
                    xlsx.SetColumnWidth(14, 13);
                    xlsx.SetCellValue(1, 15, "Risks");
                    xlsx.SetColumnWidth(15, 15);
                    xlsx.SetCellValue(1, 16, "TC Lifecycle");
                    xlsx.SetColumnWidth(16, 12);
                    xlsx.SetCellValue(1, 17, "Lifecycle Type");
                    xlsx.SetColumnWidth(17, 15);
                    xlsx.SetCellValue(1, 18, "TC Team Usage");
                    xlsx.SetColumnWidth(18, 15);
                    xlsx.SetCellStyle("A1", "R1", headerStyle);
                    xlsx.SetRowHeight(1, 1, 18);
                    xlsx.SetCellValue(2, 1, "TC" + testCase.TestCaseId.ToString("000"));
                    xlsx.SetCellStyle(2, 1, style);
                    xlsx.SetCellValue(2, 2, "Test Case " + HttpUtility.UrlDecode(testCase.WorkItemId.ToString()));
                    xlsx.SetCellStyle(2, 2, style);
                    xlsx.SetCellValue(2, 3, HttpUtility.UrlDecode(testCase.Title));
                    xlsx.SetCellStyle(2, 3, style);
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
                        if (!testCase.WriteFirstLine)
                        {
                            xlsx.SetCellValue(i + 2, 5, HttpUtility.UrlDecode(step.Index));
                            xlsx.SetCellStyle(i + 2, 5, indexStyle);
                            xlsx.SetCellValue(i + 2, 6, HttpUtility.UrlDecode(step.Description));
                            xlsx.SetCellStyle(i + 2, 6, style);
                            xlsx.SetCellValue(i + 2, 7, HttpUtility.UrlDecode(step.ExpectedResult));
                            xlsx.SetCellStyle(i + 2, 7, style);
                        }
                        else
                        {
                            xlsx.SetCellValue(i + 3, 5, HttpUtility.UrlDecode(step.Index));
                            xlsx.SetCellStyle(i + 3, 5, indexStyle);
                            xlsx.SetCellValue(i + 3, 6, HttpUtility.UrlDecode(step.Description));
                            xlsx.SetCellStyle(i + 3, 6, style);
                            xlsx.SetCellValue(i + 3, 7, HttpUtility.UrlDecode(step.ExpectedResult));
                            xlsx.SetCellStyle(i + 3, 7, style);
                        }
                    }
                    #endregion
                }
            }

            xlsx.DeleteWorksheet(SLDocument.DefaultFirstSheetName);
            //xlsx.DeleteRow(2,1);
            xlsx.SaveAs(path + @"\Plan.xlsx");
            #endregion
        }
    }
}
