using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FeatureModelEditor.ConstraintEditor
{
    class ParentesisMismatchException : Exception
    {
        private string p;

        public ParentesisMismatchException(string p)
        {
            // TODO: Complete member initialization
            this.p = p;
        }
    }
}
