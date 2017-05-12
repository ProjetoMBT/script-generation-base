using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coc.Modeling.TestPlanStructure
{
    public class TestStep
    {
        public String Description { get; set; }
        public String ExpectedResult { get; set; }
        public String Index { get; set; }
        public String Title { get; set; }
        public String workItemIdString { get; set; }

        public TestStep()
        {
            this.Description = "";
            this.ExpectedResult = "";
            this.Index = "";
            this.Title = "";
            this.workItemIdString = "";
        }
    }
}
