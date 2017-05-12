using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Coc.Modeling.FiniteStateMachine
{
    /// <summary>
    /// Represents a transition into a fsm.
    /// </summary>
    [Serializable]
    public class Transition : IComparable
    {


        public Transition()
        {
            TaggedValues = new Dictionary<string, string>();
        }
        /// <summary>
        /// From where the transition comes.
        /// </summary>
        [XmlElement()]
        public State SourceState
        {
            get;
            set;
        }
        /// <summary>
        /// To where the transitions goes.
        /// </summary>
        [XmlElement()]
        public State TargetState
        {
            get;
            set;
        }
        /// <summary>
        /// Input property.
        /// </summary>
        [XmlAttribute()]
        public string Input
        {
            get;
            set;
        }
        /// <summary>
        /// Output property.
        /// </summary>
        [XmlAttribute()]
        public string Output
        {
            get;
            set;
        }

        /// <summary>
        /// Determines if this transition was part of a loop.
        /// </summary>
        [XmlAttribute()]
        public bool CycleTransition { get; set; }

        /// <summary>
        /// Determines if this transition was the end of a loop.
        /// </summary>
        [XmlAttribute()]
        public bool EndCycle { get; set; }
        
        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        public Transition(State sourceState, State targetState, string input, string output)
        {
            if (sourceState == null)
                throw new ArgumentNullException("Source state cannot be null");
            if (targetState == null)
                throw new ArgumentNullException("Target state cannot be null");
            if (input == null)
                throw new ArgumentNullException("Input cannot be null");
            if (output == null)
                throw new ArgumentNullException("Output cannot be null");

            SourceState = sourceState;
            TargetState = targetState;
            Input = input;
            Output = output;
            CycleTransition = false;
        }

        public Transition(State sourceState, State targetState, string input, string output, bool cycleTransition)
            : this(sourceState, targetState, input, output)
        {
            TaggedValues = new Dictionary<string, string>();
            CycleTransition = cycleTransition;
        }

        public Transition(State sourceState, State targetState, string input, string output, bool cycleTransition, bool lastCycleTrans)
            : this(sourceState, targetState, input, output,cycleTransition)
        {
            TaggedValues = new Dictionary<string, string>();
            EndCycle = lastCycleTrans;
        }

            
        #endregion

        #region Methods
        /// <summary>
        /// CompareTo implementation.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            Transition t;
            try
            {
                t = (Transition)obj;
            }
            catch (Exception ee)
            {
                Console.WriteLine("Unable to compare Transitions. Given object isnt a Machine Transition. " + ee.Source);
                return -1;
            }

            return (
                this.SourceState.Equals(t.SourceState) &&
                this.TargetState.Equals(t.TargetState) &&
                this.Input.Equals(t.Input) &&
                this.Output.Equals(t.Output))
                ? 0 : 1;
        }

        public override string ToString()
        {
            return SourceState.Name + " -> " + TargetState.Name;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            Transition otr = (Transition)obj;
            if (!otr.SourceState.Equals(this.SourceState))
                return false;
            if (!otr.TargetState.Equals(this.TargetState))
                return false;
            if (!otr.Input.Equals(this.Input))
                return false;
            if (!otr.Output.Equals(this.Output))
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            return Input.GetHashCode() ^ Output.GetHashCode() ^ SourceState.GetHashCode() ^ TargetState.GetHashCode();
        }

        public Dictionary<String, String> TaggedValues { get;  set; }

        public String GetTaggedValue(string tag)
        {
            if (this.TaggedValues.Keys.Contains(tag))
            {
                return this.TaggedValues[tag];
            }
            return null;
        }
        #endregion

        public bool Isvisited { get; set; }
    }
}
