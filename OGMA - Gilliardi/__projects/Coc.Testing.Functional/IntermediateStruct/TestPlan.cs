using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coc.Data.ControlStructure;

namespace Coc.Testing.Functional
{
    public class TestPlan : SequenceGenerationStructure
    {
        public String Name { set; get; }
        //public String Id { set; get; }
        public List<TestCase> TestCases { get; set; }

        public TestPlan()
        {
            this.Name = "";
            //this.Id = Guid.NewGuid().ToString();
            this.TestCases = new List<TestCase>();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}