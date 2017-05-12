using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coc.Data.Xmi.Script
{
    public class ScriptSequence
    {
        private int sequence;

        public ScriptSequence(int sequence){
            this.sequence = sequence;
        }

        public void incrementSequence()
        {
            sequence++;
        }

        public void decrementSequence()
        {
            sequence--;
        }

        public int Sequence
        {
            get { return sequence; }
            set { 
                sequence = value;
            }
        }

    }
}
