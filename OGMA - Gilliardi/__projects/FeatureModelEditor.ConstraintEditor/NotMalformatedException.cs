using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FeatureModelEditor.ConstraintEditor
{
    class NotMalformatedException : Exception
    {
        public NotMalformatedException()
            : base()
        {

        }

        public NotMalformatedException(String s) : base(s)
        {

        }
    }
}
