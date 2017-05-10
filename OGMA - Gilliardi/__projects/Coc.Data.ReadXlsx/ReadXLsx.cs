using System;
using System.IO;
using System.Text;
using Worksheet = Net.SourceForge.Koogra.IWorksheet;
using WorkbookFactory = Net.SourceForge.Koogra.WorkbookFactory;
using Workbook = Net.SourceForge.Koogra.IWorkbook;
using Row = Net.SourceForge.Koogra.IRow;

namespace Coc.Data.ReadXlsx
{
    /*
    /// <summary>
    /// <img src="images/ReadXlsx.PNG"/>
    /// </summary>
    public static class NamespaceDoc
    {
    }*/



    public class ReadXLsx
    {
        public KeywordDictionary dic { get; set; }

        public ReadXLsx()
        {
            dic = new KeywordDictionary();
        }

        public KeywordDictionary Leitor(String filePath)
        {
            Workbook genericWB = WorkbookFactory.GetExcel2007Reader(filePath);
            //pega a folha na pos [0]
            Worksheet genericWS = genericWB.Worksheets.GetWorksheetByIndex(0);
            StringBuilder SbXls = new StringBuilder();
            //for da primeira lina +1 até ultima linha
            for (uint r = genericWS.FirstRow + 1; r <= genericWS.LastRow; ++r)
            {
                Row row = genericWS.Rows.GetRow(r);
                String keyword = row.GetCell(0).GetFormattedValue();
                String obj = row.GetCell(1).GetFormattedValue();
                String action = row.GetCell(2).GetFormattedValue();
                dic.AddKeyword(obj, action, keyword);
            }
            return dic;
        }
    }
}

