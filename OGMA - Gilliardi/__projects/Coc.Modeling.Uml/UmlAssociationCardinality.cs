using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coc.Modeling.Uml
{
    public enum UmlAssociationCardinality
    {
        One,
        One_To_N,
        Zero_To_One,
        Zero_To_N,
        N,
        Undefined
    }
}
