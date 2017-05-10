using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FunctionalTool.Testing.Functional {
    public class TestStep{
        
        public String Description {get;set;}
        public String ExpectedResult {get;set;}
        public List<TestStep> CompositeSteps{get;set;}
        public String  Index { get; set; }
        public string Title { get; set; }
        public string workItemIdString { get; set; }
        public string FTstate;
        public string FTassigned;
        public string FTreason;
        public string FTiterationPath;
        public string FTareaPath;
        public string FTapplication;
        public string FTcomplexity;
        public string FTrisks;
        public string FTtcLifecycle;
        public string FTlifecycleType;
        public string FTtcTeamUsage;
        
        public TestStep() {
            this.Description = "";
            this.ExpectedResult = "";
            this.Index = "";
            this.Title = "";
            this.workItemIdString = "";

            CompositeSteps = new List<TestStep>();
        }

    }
}
