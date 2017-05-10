using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FeatureModelEditor.ConstraintEditor
{
    class PropositionCountException : Exception
    {
        public PropositionCountException()
            : base()
        {

        }

        public PropositionCountException(String s)
            : base(s)
        {

        }
    }
}
