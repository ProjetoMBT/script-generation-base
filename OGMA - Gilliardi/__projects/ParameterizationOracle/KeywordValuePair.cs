using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParameterizationOracle
{
    public class KeywordValuePair
    {
        public KeywordValuePair()
        {

        }

        public KeywordObject KObject { get; set; }

        public KeywordAction KAction { get; set; }

        public Keyword Key { get; set; }

        public StepRunMode Srm { get; set; }
    }
}
