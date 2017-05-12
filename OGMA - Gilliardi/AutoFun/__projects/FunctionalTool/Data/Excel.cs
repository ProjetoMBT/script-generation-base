using System;
using System.Collections.Generic;
using System.Text;
using FunctionalTool.Testing.Functional;
using System.IO;
using System.Windows.Forms;
using System.Web;
using DocumentFormat;
using DocumentFormat.OpenXml;
using SpreadsheetLight;
using DocumentFormat.OpenXml.Spreadsheet;

namespace FunctionalTool.Data
{
    public class Excel
    {
        

        internal static void GenerateXlsFromTestPlan( TestPlan testPlan, String savePath)
        {

            FunctionalTool.MainWindow.ProgressBar.Maximum = testPlan.TestCases.Count;

            for (int k = 0; k < testPlan.TestCases.Count; k++)
            {
                    SLDocument xlsx = new SLDocument();

                    #region Styles

                    SLStyle style = xlsx.CreateStyle();
                    style.SetWrapText(true);
                    SLStyle indexStyle = xlsx.CreateStyle();
                    indexStyle.Alignment.Horizontal = HorizontalAlignmentValues.Right;
                    indexStyle.SetWrapText(true);
                    SLStyle headerStyle = xlsx.CreateStyle();
                    headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.Yellow, System.Drawing.Color.Black);
                    headerStyle.Border.RightBorder.Color = System.Drawing.Color.Black;
                    headerStyle.Border.LeftBorder.Color = System.Drawing.Color.Black;
                    headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                    headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                    headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                    headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

                    #endregion

                    xlsx.AddWorksheet("List");
                    xlsx.SetCellValue(1,1, 
                        
                    "'- Complexity" 
                    +"\n    Complex"
                    +"\n    Extremely Complex"
                    +"\n    Moderate"
                    +"\n    Simple"
                    +"\n"
                    +"\n - Risk"
                    +"\n    1-Critical"
                    +"\n    2-High"
                    +"\n    3-Medium"
                    +"\n    4-Low"
                    +"\n"
                    +"\n - TC Lifecycle"
                    +"\n    Current"
                    +"\n    Future"
                    +"\n    Obsolete"
                    +"\n"
                    +"\n - Lifecycle Type"
                    +"\n    Core App"
                    +"\n    E2E"
                    +"\n    Smoke"
                    +"\n"
                    +"\n - TC Team Usage"
                    +"\n    DIT"
                    +"\n    DUT"
                    +"\n    PERF"
                    +"\n    SIT"
                    +"\n    UAT"
                    +"\n    VIT"
                    +"\n"
                    +"\n - State"
                    +"\n    Design"
                    +"\n "
                    +"\n - Reason"
                    +"\n    New"
                      
                        );

                    xlsx.SetColumnStyle(1,1, style);
                    xlsx.SetColumnWidth(1,1, 50);    

                    xlsx.SelectWorksheet("Sheet1");
                    xlsx.RenameWorksheet(SLDocument.DefaultFirstSheetName, "Test Cases");


                    
                    FunctionalTool.MainWindow.ProgressBar.Value = k + 1;
                    FunctionalTool.MainWindow.ProgressBar.UpdateLayout();

                    TestCase testCase = testPlan.TestCases[k];

                    xlsx.SetCellValue(1, 1,"Test Case #");
                    xlsx.SetColumnWidth(1, 13);
                    xlsx.SetCellValue(1, 2, "Work Item ID");
                    xlsx.SetColumnWidth(2, 22);
                    xlsx.SetCellValue(1, 3,"Test Title");
                    xlsx.SetColumnWidth(3, 30);
                    xlsx.SetCellValue(1, 4,"Summary");
                    xlsx.SetColumnWidth(4, 30);
                    xlsx.SetCellValue(1, 5,"Test Step");
                    xlsx.SetColumnWidth(5, 10);
                    xlsx.SetCellValue(1, 6,"Action/Description");
                    xlsx.SetColumnWidth(6, 60);
                    xlsx.SetCellValue(1, 7,"Expected Results");
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

                    xlsx.SetRowHeight(1,1,18);
                    
                    xlsx.SetCellValue(2, 1, "TC"+ testCase.TestCaseId.ToString("000"));
                    xlsx.SetCellStyle(2, 1, style);
                    xlsx.SetCellValue(2, 2, testCase.TestSteps[0].workItemIdString);
                    xlsx.SetCellStyle(2, 2, style);
                    xlsx.SetCellValue(2, 3, testCase.Title);
                    xlsx.SetCellStyle(2, 3, style);
                    xlsx.SetCellValue(2, 4, testCase.Summary);
                    xlsx.SetCellStyle(2, 4, style);
                    xlsx.SetCellValue(2, 5, "1");
                    xlsx.SetCellStyle(2, 5, indexStyle);
                    xlsx.SetCellValue(2, 6, testCase.TestSteps[0].Description.Trim());
                    xlsx.SetCellStyle(2, 6, style);
                    xlsx.SetCellValue(2, 7, testCase.TestSteps[0].ExpectedResult);
                    xlsx.SetCellStyle(2, 7, style);
                    xlsx.SetCellValue(2, 8, testCase.TestSteps[0].FTassigned);
                    xlsx.SetCellStyle(2, 8, style);
                    xlsx.SetCellValue(2, 9, testCase.TestSteps[0].FTstate);
                    xlsx.SetCellStyle(2, 9, style);
                    xlsx.SetCellValue(2, 10, testCase.TestSteps[0].FTreason);
                    xlsx.SetCellStyle(2, 10, style);
                    xlsx.SetCellValue(2, 11, testCase.TestSteps[0].FTiterationPath);
                    xlsx.SetCellStyle(2, 11, style);
                    xlsx.SetCellValue(2, 12, testCase.TestSteps[0].FTareaPath);
                    xlsx.SetCellStyle(2, 12, style);
                    xlsx.SetCellValue(2, 13, testCase.TestSteps[0].FTapplication);
                    xlsx.SetCellStyle(2, 13, style);
                    xlsx.SetCellValue(2, 14, testCase.TestSteps[0].FTcomplexity);
                    xlsx.SetCellStyle(2, 14, style);
                    xlsx.SetCellValue(2, 15, testCase.TestSteps[0].FTrisks);
                    xlsx.SetCellStyle(2, 15, style);
                    xlsx.SetCellValue(2, 16, testCase.TestSteps[0].FTtcLifecycle);
                    xlsx.SetCellStyle(2, 16, style);
                    xlsx.SetCellValue(2, 17, testCase.TestSteps[0].FTlifecycleType);
                    xlsx.SetCellStyle(2, 17, style);
                    xlsx.SetCellValue(2, 18, testCase.TestSteps[0].FTtcTeamUsage);
                    xlsx.SetCellStyle(2, 18, style);

                    for (int i = 1; i < testCase.TestSteps.Count; i++)
                    {
                        TestStep step = testCase.TestSteps[i];
                        xlsx.SetCellValue(i + 2, 2,step.workItemIdString);
                        xlsx.SetCellStyle(i + 2, 2, style);
                        xlsx.SetCellValue(i + 2, 3,step.Title);
                        xlsx.SetCellStyle(i + 2, 3, style);
                        xlsx.SetCellValue(i + 2, 5,step.Index);
                        xlsx.SetCellStyle(i + 2, 5, indexStyle);
                        xlsx.SetCellValue(i + 2, 6,step.Description.Trim().Replace("'", "\""));
                        xlsx.SetCellStyle(i + 2, 6, style);
                        xlsx.SetCellValue(i + 2, 7,step.ExpectedResult.Replace("'", "\""));
                        xlsx.SetCellStyle(i + 2, 7, style);
                    }

                    string str = "@,!#$%\"^&:*<>?|//\\";
                    char[] ch = str.ToCharArray();
                    string name = HttpUtility.UrlDecode(testCase.Title);

                    foreach (char c in ch)
                    {
                        if (name.Contains(c.ToString()))
                        {
                            name.Replace(c.ToString(),"");        
                        }
                    }

                    xlsx.SaveAs(savePath +"\\" + testCase.Title + ".xlsx");
            }
        }

     

    }
}
