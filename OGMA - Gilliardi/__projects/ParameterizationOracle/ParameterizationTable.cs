using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParameterizationOracle
{
    public class ParameterizationTable
    {
        public ParameterizationTable()
        {
            Keywords = new List<Keyword>();
            Srms = new List<StepRunMode>();
            Protocols = new List<ScriptProtocol>();
        }

        public KeywordsTable KTable { get; set; }

        public ScriptsTable STable { get; set; }

        public List<Keyword> Keywords { get; set; }

        public List<StepRunMode> Srms { get; set; }

        public List<ScriptProtocol> Protocols { get; set; }
    }
}
