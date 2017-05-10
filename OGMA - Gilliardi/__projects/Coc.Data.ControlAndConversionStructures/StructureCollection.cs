using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coc.Data.ControlStructure;
using Coc.Data.ControlAndConversionStructures;

namespace Coc.Data.ControlAndConversionStructures
{
    public class StructureCollection
    {
        public StructureType type { get; set; }
        public List<GeneralUseStructure> listGeneralStructure { get; set; }
        public StructureCollection()
        {
            listGeneralStructure = new List<GeneralUseStructure>();
        }
    }
}
