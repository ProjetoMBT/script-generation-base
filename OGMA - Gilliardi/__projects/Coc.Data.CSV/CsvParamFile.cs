using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Coc.Data.ControlStructure;
using Coc.Data.ControlAndConversionStructures;

namespace Coc.Data.CSV
{
    /*
    /// <summary>
    /// <img src="images/CSV.PNG"/>
    /// </summary>
    public static class NamespaceDoc
    {
    }*/



    public class CsvParamFile : GeneralUseStructure
    {
        private String nameUseCase;

        public String NameUseCase
        {
            get { return nameUseCase; }
            set { nameUseCase = value; }
        }
        
        #region Attributes
        private FileInfo file;
        private String[] columns;
        private List<String[]> lines;
        private int lastLine;
        #endregion

        #region Public Methods
        public CsvParamFile(FileInfo aFile)
        {
            this.file = aFile;
            StreamReader sr = new StreamReader(file.FullName);
            String[] delimiters = new String[] { ";", "\t" };
            String firstLine = sr.ReadLine();
            if (firstLine.Length > 0)
            {
                this.columns = firstLine.Split(delimiters, StringSplitOptions.None);
                for (int i = 0; i < columns.Length; i++)
                {
                    columns[i] = columns[i].Trim();
                }
            }
            lines = new List<string[]>();
            while (!sr.EndOfStream)
            {
                String line = sr.ReadLine();
                lines.Add(line.Split(delimiters, StringSplitOptions.None));
            }
            sr.Close();

            lastLine = -1;
        }

        public Dictionary<String, String> GetDicFromLine(int line)
        {
            Dictionary<String, String> ret = new Dictionary<string, string>();
            for (int i = 0; i < columns.Length; i++)
            {
                String key = file.Name + "." + columns[i];
                String value = GetValue(i, line);
                ret.Add(key, value);
            }
            return ret;
        }

        public int LinesCount
        {
            get { return lines.Count; }
        }

        public String GetValueNextLine(String columnName)
        {
            int i = GetColumnIndex(columnName);
            return GetValueNextLine(i);
        }

        public String GetValueNextLine(int columnIndex)
        {
            int line = lastLine + 1;
            return GetValue(columnIndex, line);
        }

        public String GetValueCurrentLine(String columnName)
        {
            int i = GetColumnIndex(columnName);
            return GetValueCurrentLine(i);
        }

        public String GetValueCurrentLine(int columnIndex)
        {
            int line = lastLine;
            if (line == -1)
                line = 0;
            return GetValue(columnIndex, line);
        }

        public String GetValue(String columnName, int line)
        {
            int i = GetColumnIndex(columnName);
            return GetValue(i, line);
        }

        public String GetValue(int columnIndex, int line)
        {
            if (columnIndex >= 0)
            {
                if (line < lines.Count)
                {
                    lastLine = line;
                    string[] l = lines.ElementAt(line);
                    if (l.Length > columnIndex)
                    {
                        return l[columnIndex];
                    }
                }
                else
                {
                    line = 0;
                    lastLine = line;
                    string[] l = lines.ElementAt(line);
                    if (l.Length > columnIndex)
                    {
                        return l[columnIndex];
                    }
                }
            }
            return null;
        }

        public String FileName { get { return file.Name.Substring(0, file.Name.LastIndexOf(".")); } }

        public void NextLine()
        {
            lastLine = lastLine + 1;
        }
        #endregion

        #region Private Methods
        private int GetColumnIndex(string columnName)
        {
            for (int i = 0; i < columns.Length; i++)
            {
                if (columns[i].Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    return i;
            }
            return -1;
        }
        #endregion
    }
}
