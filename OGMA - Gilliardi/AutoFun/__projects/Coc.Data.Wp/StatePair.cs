using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coc.Modeling.FiniteStateMachine;

namespace Coc.Data.Wpartial
{
    public class StatePair
    {
        #region Attributes
        public State Si { get; set; }
        public State Sj { get; set; }
        public String wi { get; set; }
        #endregion

        #region Public Methods
        public override string ToString()
        {
            return Si.Name + " - " + wi + " - " + Sj.Name;
        }
        #endregion
    }
}
