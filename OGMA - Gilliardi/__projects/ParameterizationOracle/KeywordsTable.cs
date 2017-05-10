using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParameterizationOracle
{
    public class KeywordsTable
    {
        public KeywordsTable()
        {
            Table = new List<KeywordValuePair>();
        }

        public List<KeywordValuePair> Table { get; set; }
    }
}
