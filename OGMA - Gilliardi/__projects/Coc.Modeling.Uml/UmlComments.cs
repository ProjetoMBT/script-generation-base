using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coc.Modeling.Uml
{
    /// <summary>
    /// Represents a UML annotation. It must be attached
    /// to an Uml Object in order to be part of a model.
    /// </summary>
    public class UmlComments
    {
        public String Text { get; set; }
    }
}
