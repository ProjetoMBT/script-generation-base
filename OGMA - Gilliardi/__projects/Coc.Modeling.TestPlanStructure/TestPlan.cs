using System;
using System.Collections.Generic;
using Coc.Data.ControlAndConversionStructures;

namespace Coc.Modeling.TestPlanStructure
{
    public class TestPlan : GeneralUseStructure
    {
        public String Name { set; get; }
        public String NameUseCase { set; get; }
        //public String Id { set; get; }
        public List<TestCase> TestCases { get; set; }

        public TestPlan()
        {
            this.Name = "";
            this.NameUseCase = "";
            //this.Id = Guid.NewGuid().ToString();
            this.TestCases = new List<TestCase>();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
