using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coc.Testing.Functional
{
    public class TestCase
    {
        public static int contTestCaseId = 001;
        public static int contWorkItemId = 1000;
        public int TestCaseId { get; set; }
        public int WorkItemId { get; set; }
        public String Title { get; set; }
        public String Summary { get; set; }
        public List<TestStep> TestSteps { get; set; }
        public String TDpreConditions;
        public String TDpostConditions;
        public String TDstate;
        public String TDassigned;
        public String TDreason;
        public String TDiterationPath;
        public String TDareaPath;
        public String TDapplication;
        public String TDcomplexity;
        public String TDrisks;
        public String TDtcLifecycle;
        public String TDlifecycleType;
        public String TDtcTeamUsage;
        public Boolean WriteFirstLine;

        public TestCase(String title)
        {
            this.Title = title;
            Summary = "";
            this.TestSteps = new List<TestStep>();
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
