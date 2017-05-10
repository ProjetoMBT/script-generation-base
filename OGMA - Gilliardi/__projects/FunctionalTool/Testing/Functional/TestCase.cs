using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FunctionalTool.Testing.Functional {
    public class TestCase {

        public static int contTestCaseId = 001;
        public static int contWorkItemId = 1000;
        public int TestCaseId { get; set; }
        public int WorkItemId { get; set; }
        public String Title {get;set;}
        public String Summary {get;set;}
        public List<TestStep> TestSteps {get;set;}
        
        public TestCase(String title){
            this.Title = title;
            Summary = "";
            this.TestSteps = new List<TestStep>();
        }
    }
}
