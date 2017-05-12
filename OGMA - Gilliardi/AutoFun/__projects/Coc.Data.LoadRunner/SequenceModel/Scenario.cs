using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coc.Data.LoadRunner.SequenceModel
{
    /// <summary>
    /// Class that represents a generic Scenario
    /// </summary>
    public class Scenario
    {
        public Scenario()
        {
            this.testCases = new List<TestCase>();
        }

        private int population;
        /// <summary>
        /// Number of users.
        /// </summary>
        public int Population
        {
            get { return population; }
            set { population = value; }
        }

        private int executionTime;
        /// <summary>
        /// Scenario execution time.
        /// </summary>
        public int ExecutionTime
        {
            get { return executionTime; }
            set { executionTime = value; }
        }

        private int thinkTime;
        /// <summary>
        /// Global think time used in all transactions whitin the scenario.
        /// </summary>
        public int ThinkTime
        {
            get { return thinkTime; }
            set { thinkTime = value; }
        }

        private int warmUpTime;
        /// <summary>
        /// Scenario warmup time.
        /// </summary>
        public int WarmUpTime
        {
            get { return warmUpTime; }
            set { warmUpTime = value; }
        }

        private List<TestCase> testCases;
        /// <summary>
        /// Test Cases list.
        /// </summary>
        public List<TestCase> TestCases
        {
            get { return testCases; }
            set { testCases = value; }
        }

        private double rampUpTime;
        /// <summary>
        /// RampUp time. (as seconds)
        /// </summary>
        public double RampUpTime
        {
            get { return rampUpTime; }
            set { rampUpTime = value; }
        }

        private int rampUpUser;
        /// <summary>
        /// Ramp up number of users.
        /// </summary>
        public int RampUpUser
        {
            get { return rampUpUser; }
            set { rampUpUser = value; }
        }

        private double rampDownTime;
        /// <summary>
        /// RampDown time. (as seconds)
        /// </summary>
        public double RampDownTime
        {
            get { return rampDownTime; }
            set { rampDownTime = value; }
        }

        private int rampDownUser;
        /// <summary>
        /// Ramp down number of users.
        /// </summary>
        public int RampDownUser
        {
            get { return rampDownUser; }
            set { rampDownUser = value; }
        }

        private List<Host> additionalHosts;
        /// <summary>
        /// Adicional Host list.
        /// </summary>
        public List<Host> AdditionalHosts
        {
            get { return additionalHosts; }
            set { additionalHosts = value; }
        }

        private Host hostSUT;
        /// <summary>
        /// Host sut.
        /// </summary>
        public Host HostSUT
        {
            get { return hostSUT; }
            set { hostSUT = value; }
        }

        private string name;
        /// <summary>
        /// Host sut.
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }
}
