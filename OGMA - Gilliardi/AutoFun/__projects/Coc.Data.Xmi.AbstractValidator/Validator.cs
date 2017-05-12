using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Coc.Modeling.Uml;

namespace Coc.Data.Xmi.AbstractValidator
{
    public abstract class Validator
    {
        public abstract List<KeyValuePair<String, Int32>> Validate(UmlModel model);
    }
}
