using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coc.Testing.Functional
{
    public class TestStep
    {
        public String Description { get; set; }
        public String ExpectedResult { get; set; }
        //public List<TestStep> CompositeSteps { get; set; }
        public String Index { get; set; }
        public String Title { get; set; }
        public String workItemIdString { get; set; }
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
        //public String Id { get; set; }

        public TestStep()
        {
            this.Description = "";
            this.ExpectedResult = "";
            this.Index = "";
            this.Title = "";
            this.workItemIdString = "";
            //this.Id = Guid.NewGuid().ToString();

            //CompositeSteps = new List<TestStep>();
        }
    }
}
