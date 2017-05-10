using System;
using System.Collections.Generic;
using System.Linq;
using Coc.Modeling.Uml;
using SpreadsheetLight;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Web;
using System.IO;
using Coc.Data.Interfaces;
using Coc.Data.ControlAndConversionStructures;
using Coc.Data.ReadXlsx;

namespace Coc.Data.OATS
{
    public class ParserToExcelOATS : ScriptGenerator
    {
        #region Public Methods
        public void GenerateScript(List<GeneralUseStructure> structure, String OATSExcelPath)
        {
            UmlModel model = structure.OfType<UmlModel>().First();
            CleanModel(model);
            GenerateXlsTestScript(model);
            GenerateXlsTestData(model);
        }

        #endregion

        #region Private Methods

        //TestScript excel generation
        private void GenerateXlsTestScript(UmlModel model)
        {
            int currentLine = 0;
            String modelName = model.Name;
            String directory = CreateTestScriptDirectory();

            #region Styles and SLDocument
            SLDocument xlsx = new SLDocument();
            SLStyle style = xlsx.CreateStyle();
            style.SetWrapText(true);
            SLStyle style_aux = xlsx.CreateStyle();
            style_aux.SetWrapText(true);
            style_aux.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            SLStyle headerStyle = xlsx.CreateStyle();
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.FromArgb(0x8EB4E3), System.Drawing.Color.Black);
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Font.FontName = "Calibri";
            headerStyle.Border.RightBorder.Color = System.Drawing.Color.Black;
            headerStyle.Border.LeftBorder.Color = System.Drawing.Color.Black;
            headerStyle.Border.TopBorder.Color = System.Drawing.Color.Black;
            headerStyle.Border.BottomBorder.Color = System.Drawing.Color.Black;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            xlsx.DeleteWorksheet(SLDocument.DefaultFirstSheetName);
            #endregion

            if (model.Diagrams.Count(X => X is UmlActivityDiagram) > 1)
            {
                throw new Exception("Only one Activity diagram allowed! (For now)");
            }

            UmlActivityDiagram act = model.Diagrams.OfType<UmlActivityDiagram>().First();

            if (act.Name.Length > 31)
            {
                String aux = act.Name;
                aux = aux.Substring(0, 25) + aux.Substring(aux.Length - 5, 5);
                aux = HttpUtility.UrlDecode(aux);
                xlsx.AddWorksheet(aux);
                xlsx.SelectWorksheet(aux);
            }
            else
            {
                xlsx.AddWorksheet(HttpUtility.UrlDecode(act.Name));
                xlsx.SelectWorksheet(HttpUtility.UrlDecode(act.Name));
            }

            ReadXLsx read = new ReadXLsx();
            //Must be in the output folder
            KeywordDictionary dic = read.Leitor(@"KeyWords_2_2.xlsx");

            #region Header
            xlsx.SetCellValue(1, 1, "StepRunMode");
            xlsx.SetCellValue(1, 2, "Keyword");
            xlsx.SetCellValue(1, 3, "Object");
            xlsx.SetCellValue(1, 4, "ActionValue1");
            xlsx.SetCellValue(1, 5, "ManualStepDescription");
            xlsx.SetCellValue(1, 6, "StopOnFail");
            xlsx.SetCellStyle("A1", "F1", headerStyle);
            #endregion
            #region Dados Fixos
            xlsx.SetCellValue(2, 1, "r");
            xlsx.SetCellStyle(2, 1, style_aux);
            xlsx.SetCellValue(2, 2, "calludf");
            xlsx.SetCellStyle(2, 2, style);
            xlsx.SetCellValue(2, 3, "setbasestate");
            xlsx.SetCellStyle(2, 3, style);

            xlsx.SetCellValue(3, 1, "w");
            xlsx.SetCellStyle(3, 1, style_aux);
            xlsx.SetCellValue(3, 2, "perform");
            xlsx.SetCellStyle(3, 2, style);
            xlsx.SetCellValue(3, 3, "browser;*about:blank*");
            xlsx.SetCellStyle(3, 3, style);
            xlsx.SetCellValue(3, 4, "navigate;dt_URL");
            xlsx.SetCellStyle(3, 4, style);

            xlsx.SetCellValue(4, 1, "w");
            xlsx.SetCellStyle(4, 1, style_aux);
            xlsx.SetCellValue(4, 2, "perform");
            xlsx.SetCellStyle(4, 2, style);
            xlsx.SetCellValue(4, 3, "browser;*about:blank*");
            xlsx.SetCellStyle(4, 3, style);
            xlsx.SetCellValue(4, 4, "waitforpage;dt_MaxWebPageSyncTime");
            xlsx.SetCellStyle(4, 4, style);

            currentLine = 4;
            #endregion

            for (int i = 0; i < act.UmlObjects.OfType<UmlTransition>().Count(); i++)
            {
                UmlTransition tran = act.UmlObjects.OfType<UmlTransition>().ElementAt(i);
                String[] tdAction = { "" };
                String tdObject = "";

                if (tran.Target is UmlFinalState)
                {
                    continue;
                }

                foreach (KeyValuePair<String, String> pair in tran.TaggedValues)
                {
                    if (pair.Key.Equals("TDACTION"))
                    {
                        tdAction = HttpUtility.UrlDecode(pair.Value).Split(',');
                    }

                    if (pair.Key.Equals("TDOBJECT"))
                    {
                        tdObject = HttpUtility.UrlDecode(pair.Value);
                        tdObject = tdObject.Substring(1);
                        tdObject = tdObject.Substring(0, tdObject.Length - 1);
                    }
                }

                for (int j = 0; j < tdAction.Count(); j++)
                {
                    //if (!tdObject.Split(';')[0].Equals("window"))
                    //{
                    String tdActionSingle = tdAction[j];
                    tdActionSingle = tdActionSingle.Substring(1);
                    tdActionSingle = tdActionSingle.Substring(0, tdActionSingle.Length - 1);
                    String[] tdActionAux = tdActionSingle.Split(';');

                    if (tdActionAux[0].Equals("wait"))
                    {
                        xlsx.SetCellValue(currentLine + 1, 1, "w");
                        xlsx.SetCellStyle(currentLine + 1, 1, style_aux);
                        xlsx.SetCellValue(currentLine + 1, 2, tdActionAux[0]);
                        xlsx.SetCellStyle(currentLine + 1, 2, style);
                        xlsx.SetCellValue(currentLine + 1, 3, tdActionAux[1]);
                        xlsx.SetCellStyle(currentLine + 1, 3, style);
                        xlsx.SetCellValue(currentLine + 1, 4, "");
                        xlsx.SetCellStyle(currentLine + 1, 4, style);
                    }
                    else
                    {
                        String tdObjectToCompare = tdObject.Split(';')[0];
                        String tdActionToCompare = tdActionSingle.Split(';')[0];
                        tdObjectToCompare = tdObjectToCompare.ToLower();
                        tdActionToCompare = tdActionToCompare.ToLower();
                        String keyword = dic.GetKeyword(tdObjectToCompare, tdActionToCompare);

                        xlsx.SetCellValue(currentLine + 1, 1, "w");
                        xlsx.SetCellStyle(currentLine + 1, 1, style_aux);
                        xlsx.SetCellValue(currentLine + 1, 2, keyword);
                        xlsx.SetCellStyle(currentLine + 1, 2, style);
                        xlsx.SetCellValue(currentLine + 1, 3, tdObject);
                        xlsx.SetCellStyle(currentLine + 1, 3, style);
                        xlsx.SetCellValue(currentLine + 1, 4, tdActionSingle);
                        xlsx.SetCellStyle(currentLine + 1, 4, style);
                    }
                    currentLine++;
                    //}
                }
            }

            xlsx.AutoFitColumn(1, 6);
            xlsx.AutoFitRow(1, currentLine);
            xlsx.DeleteWorksheet(SLDocument.DefaultFirstSheetName);

            if (!File.Exists(directory + @"\" + modelName + "_TestScript.xlsx"))
            {
                xlsx.SaveAs(directory + @"\" + modelName + "_TestScript.xlsx");
            }
            else
            {
                int i = 1;
                string name;
                string[] sub;
                string[] files = System.IO.Directory.GetFiles(directory);

                foreach (String file in files)
                {
                    name = System.IO.Path.GetFileNameWithoutExtension(file);
                    if (name.Contains("("))
                    {
                        char[] splitTokens = { '(', ')' };

                        sub = name.Split(splitTokens);

                        if (sub[0] == modelName)
                        {
                            //descubro o maior index
                            if (Convert.ToInt16(sub[1]) > i)
                            {
                                i = Convert.ToInt16(sub[1]);
                            }
                        }
                    }
                }
                xlsx.SaveAs(directory + @"\" + modelName + "(" + (i + 1) + ")" + "_TestScript.xlsx");
            }
        }

        //TestData excel generation
        private void GenerateXlsTestData(UmlModel model)
        {
            int currentColumn = 0;
            String modelName = model.Name;

            //Verifica se diretorio existe, se não existe cria um novo.
            String directory = CreateTestDataDirectory();

            #region Styles and SLDocument
            SLDocument xlsx = new SLDocument();
            SLStyle style = xlsx.CreateStyle();
            SLStyle headerStyle = xlsx.CreateStyle();
            style.SetWrapText(true);
            //style.Border.RightBorder.Color = System.Drawing.Color.Black;
            //style.Border.LeftBorder.Color = System.Drawing.Color.Black;
            //style.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            //style.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            //style.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            //style.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.FromArgb(0x8EB4E3), System.Drawing.Color.Black);
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

            if (model.Diagrams.Count(X => X is UmlActivityDiagram) > 1)
            {
                throw new Exception("Only one Activity diagram allowed! (For now)");
            }

            UmlActivityDiagram act = model.Diagrams.OfType<UmlActivityDiagram>().First();

            if (act.Name.Length > 31)
            {
                String aux = act.Name;
                aux = aux.Substring(0, 25) + aux.Substring(aux.Length - 5, 5);
                aux = HttpUtility.UrlDecode(aux);
                xlsx.AddWorksheet(aux);
                xlsx.SelectWorksheet(aux);
            }
            else
            {
                xlsx.AddWorksheet(HttpUtility.UrlDecode(act.Name));
                xlsx.SelectWorksheet(HttpUtility.UrlDecode(act.Name));
            }

            for (int i = 0; i < act.UmlObjects.OfType<UmlTransition>().Count(); i++)
            {
                UmlTransition tran = act.UmlObjects.OfType<UmlTransition>().ElementAt(i);
                foreach (KeyValuePair<String, String> pair in tran.TaggedValues)
                {
                    String valueTDObject = "";
                    if (pair.Key.Equals("TDOBJECT"))
                    {
                        valueTDObject = HttpUtility.UrlDecode(pair.Value);
                        valueTDObject = valueTDObject.Substring(1);
                        valueTDObject = valueTDObject.Substring(0, valueTDObject.Length - 1);

                        if (valueTDObject.Contains("textbox") || valueTDObject.Contains("selectbox"))
                        {
                            char[] delimiterChars = { '_' };
                            string[] words = valueTDObject.Split(delimiterChars);
                            String aux = words[words.Count() - 1];

                            xlsx.SetCellValue(1, currentColumn + 1, aux);
                            //xlsx.SetColumnWidth(1, 30);
                            xlsx.SetCellStyle(1, currentColumn + 1, headerStyle);
                            //xlsx.SetRowHeight(1, 1, 18);
                            currentColumn++;
                        }
                    }
                }
            }

            xlsx.AutoFitColumn(1, currentColumn);
            xlsx.AutoFitRow(1);
            xlsx.DeleteWorksheet(SLDocument.DefaultFirstSheetName);

            if (!File.Exists(directory + @"\" + modelName + "_TestData.xlsx"))
            {
                xlsx.SaveAs(directory + @"\" + modelName + "_TestData.xlsx");
            }
            else
            {
                int i = 1;
                string name;
                string[] sub;
                string[] files = System.IO.Directory.GetFiles(directory);

                foreach (String file in files)
                {
                    name = System.IO.Path.GetFileNameWithoutExtension(file);
                    if (name.Contains("("))
                    {
                        char[] splitTokens = { '(', ')' };

                        sub = name.Split(splitTokens);

                        if (sub[0] == modelName)
                        {
                            //descubro o maior index
                            if (Convert.ToInt16(sub[1]) > i)
                            {
                                i = Convert.ToInt16(sub[1]);
                            }
                        }
                    }
                }
                xlsx.SaveAs(directory + @"\" + modelName + "(" + (i + 1) + ")" + "_TestData.xlsx");
            }
        }

        private String CreateTestScriptDirectory()
        {
            String directory = Environment.CurrentDirectory + "\\Result Files\\TestScript";
            //verifica se diretório não existe
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            return directory;
        }

        private String CreateTestDataDirectory()
        {
            String directory = Environment.CurrentDirectory + "\\Result Files\\TestData";
            //verifica se diretório não existe
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            return directory;
        }

        private UmlModel CleanModel(UmlModel model)
        {
            UmlTransition currentTransition = new UmlTransition();
            UmlTransition lastTransition = new UmlTransition();
            List<UmlBase> newObjectsList = new List<UmlBase>();

            foreach (UmlActivityDiagram acDiagram in model.Diagrams.OfType<UmlActivityDiagram>())
            {
                List<UmlTransition> transitionsToExclude = new List<UmlTransition>();
                List<UmlElement> elementsToExclude = new List<UmlElement>();

                foreach (UmlTransition t in acDiagram.UmlObjects.OfType<UmlTransition>())
                {
                    try
                    {
                        if (t.GetTaggedValue("TDACTION").Equals("{waitForPage;null}"))
                        {
                            transitionsToExclude.Add(t);
                            elementsToExclude.Add(t.Target);
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }

                foreach (UmlTransition t in acDiagram.UmlObjects.OfType<UmlTransition>())
                {
                    try
                    {
                        if (t.GetTaggedValue("TDACTION").Equals("{waitForPage;null}"))
                        {
                            UmlTransition tran = new UmlTransition();
                            UmlTransition tranAux = acDiagram.UmlObjects.OfType<UmlTransition>().Where(x => x.Source.Equals(t.Target)).FirstOrDefault();
                            tran.Source = t.Source;
                            tran.Target = tranAux.Target;

                            foreach (KeyValuePair<String, String> tag in tranAux.TaggedValues)
                            {
                                tran.TaggedValues.Add(tag.Key, tag.Value);
                            }

                            if (!newObjectsList.Contains(tran))
                            {
                                newObjectsList.Add(tran);
                            }

                            if (!newObjectsList.Contains(tran.Source))
                            {
                                newObjectsList.Add(tran.Source);
                            }

                            if (!newObjectsList.Contains(tran.Target))
                            {
                                newObjectsList.Add(tran.Target);
                            }
                        }
                        else
                        {
                            if (!elementsToExclude.Contains(t.Source))
                            {
                                if (!newObjectsList.Contains(t))
                                {
                                    newObjectsList.Add(t);
                                }

                                if (!newObjectsList.Contains(t.Source))
                                {
                                    newObjectsList.Add(t.Source);
                                }

                                if (!newObjectsList.Contains(t.Target))
                                {
                                    newObjectsList.Add(t.Target);
                                }
                            }
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
                acDiagram.UmlObjects.Clear();
                acDiagram.UmlObjects.AddRange(newObjectsList);
            }
            return model;
        }
        #endregion
    }
}