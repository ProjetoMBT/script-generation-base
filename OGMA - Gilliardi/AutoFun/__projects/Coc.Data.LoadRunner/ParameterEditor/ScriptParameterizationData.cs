using System;

namespace Coc.Data.LoadRunner.ParameterEditor
{
    public class ScriptParameterizationData
    {

        public String Name = "";
        public String Delimiter = ",";
        public String TableLocation = "Local";
        public String ColumnName = "";
        public String GenerateNewValue = "";
        public String Table = "";
        public String Type = "Table";
        public String ValueForEachVirtualUser = "";
        public String OriginalValue = "";
        public String AutoAllocateBlockSize = "1";
        public String SelectNextRow = "";
        public String StartRow = "1";
        public String OutOfRangePolicy = "ContinueWithLast";
        public String SourcePath = "";
        /*
         * 
           [parameter:NewParam]
            Delimiter=","
            ParamName="NewParam"
            TableLocation="Local"
            ColumnName="Col 1"
            GenerateNewVal="EachOccurrence"
            Table="NewParam.dat"
            Type="Table"
            value_for_each_vuser=""
            OriginalValue=""
            auto_allocate_block_size="0"
            SelectNextRow="Unique"
            StartRow="1"
            OutOfRangePolicy="ContinueWithLast"
         * 
         */
    }
}
