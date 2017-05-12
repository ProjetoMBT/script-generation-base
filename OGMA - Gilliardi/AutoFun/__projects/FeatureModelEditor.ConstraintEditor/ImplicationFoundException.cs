using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FeatureModelEditor.ConstraintEditor
{
    class ImplicationFoundException : Exception
    {
        public ImplicationFoundException()
            : base()
        {

        }

        public ImplicationFoundException(String s)
            : base(s)
        {

        }
    }
}
