using System;

namespace Coc.Data.VFSMachine
{
    public class Variable
    {
        #region Attributes
        public String Condition { get; set; }
        #endregion

        #region Constructor
        public Variable(String n)
        {
            Condition = n;
        }
        #endregion

        #region Public Methods
        public override string ToString()
        {
            return Condition;
        }
        #endregion
    }
}
