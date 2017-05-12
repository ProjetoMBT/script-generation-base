using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FunctionalTool.Testing.Functional {
    public class TestPlan {
        public String Name { set; get; }
        public List<TestCase> TestCases{ get; set; }

        public TestPlan(){
            this.Name = "";
            this.TestCases = new List<TestCase>();
        }
    }
}
