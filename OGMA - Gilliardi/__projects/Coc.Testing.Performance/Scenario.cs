using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Coc.Testing.Performance
{
    /// <summary>
    /// Models the entity that results from the parsing of an Actor.
    /// </summary>
    [Serializable]
    public class Scenario {

        private int population;
        /// <summary>
        /// Number of users.
        /// </summary>
        [XmlAttribute("Population")]
        public int Population {
            get { return population; }
            set { population = value; }
        }


        private int executionTime;
        /// <summary>
        /// Execution Time (in seconds).
        /// </summary>
        [XmlAttribute("ExecutionTime")]
        public int ExecutionTime {
            get { return executionTime; }
            set { executionTime = value; }
        }


        private List<TestCase> testCases;
        [XmlElement("TestCases")]
        public List<TestCase> TestCases {
            get { return testCases; }
            set { testCases = value; }
        }


        private double rampUpTime;
        /// <summary>
        /// Initialization time (in seconds).
        /// </summary>
        [XmlAttribute("RampUpTime")]
        public double RampUpTime {
            get { return rampUpTime; }
            set { rampUpTime = value; }
        }


        private int rampUpUser;
        /// <summary>
        /// Number of users during Initialization time.
        /// </summary>
        [XmlAttribute("RampUpUser")]
        public int RampUpUser {
            get { return rampUpUser; }
            set { rampUpUser = value; }
        }


        private double rampDownTime;
        /// <summary>
        /// Finalization time (in seconds).
        /// </summary>
        [XmlAttribute("RampDownTime")]
        public double RampDownTime {
            get { return rampDownTime; }
            set { rampDownTime = value; }
        }


        private int rampDownUser;
        /// <summary>
        /// Number of users in Finalization.
        /// </summary>
        [XmlAttribute("RampDownUser")]
        public int RampDownUser {
            get { return rampDownUser; }
            set { rampDownUser = value; }
        }


        private List<Host> additionalHosts;
        /// <summary>
        /// Additional Hosts list (for use with monitoring and external resources).
        /// </summary>
        [XmlElement("AdittionalHosts")]
        public List<Host> AdditionalHosts {
            get { return additionalHosts; }
            set { additionalHosts = value; }
        }


        private Host hostSUT;
        /// <summary>
        /// Host for the System Under Test (SUT).
        /// </summary>
        [XmlElement("HostSut") ]
        public Host HostSUT {
            get { return hostSUT; }
            set { hostSUT = value; }
        }

        private string name;
        [XmlAttribute("Name")]
        public string Name {
            get { return name; }
            set { name = value; }
        }

        public Scenario()
        {
            this.TestCases = new List<TestCase>();
        }
    }
}
