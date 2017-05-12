using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coc.Modeling.Uml;
using Coc.Data.ControlStructure;
using Coc.Data.ControlAndConversionStructures;

namespace Coc.Data.Interfaces
{
    public class Validator
    {
        public virtual List<KeyValuePair<String, Int32>> Validate(List<GeneralUseStructure> ListModel, String fileName)
        {
            return new List<KeyValuePair<String, Int32>>();
        }
    }
}
