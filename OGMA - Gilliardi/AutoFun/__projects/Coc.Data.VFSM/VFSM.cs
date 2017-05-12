using System;
using System.Collections.Generic;
using Coc.Modeling.FiniteStateMachine;

namespace Coc.Data.VFSMachine
{
    /*
    /// <summary>
    /// <img src="images/VFSMachine.PNG"/>
    /// </summary>
    public static class NamespaceDoc
    {
    }*/



    public class VFSM
    {
        #region Attributes
        public State StateInitial { get; set; }
        public List<State> ListStates { get; set; }
        public List<TransitionVFSM> ListTransition { get; set; }
        public List<String> ListAlphabetInput { get; set; }
        public List<String> ListAlphabetOutput { get; set; }
        public List<Variable> ListOfGuardian { get; set; }
        public List<Variable> ListOfVariableInitial { get; set; }
        public String Name { get; set; }
        #endregion

        #region Constructor
        public VFSM(String n)
        {
            Name = n;
            ListTransition = new List<TransitionVFSM>();
            ListAlphabetInput = new List<string>();
            ListAlphabetOutput = new List<string>();
            ListStates = new List<State>();
            ListOfVariableInitial = new List<Variable>();
            ListOfGuardian = new List<Variable>();
        }
        #endregion
    }
}
