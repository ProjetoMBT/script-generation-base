//#define HSI
//#define DFS
//#define Wp

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coc.Data.ControlStructure;

namespace Coc.Data.ConversionUnit
{
    public class ModelingStructureConverterFactory
    {

        public ModelingStructureConverter CreateModelingStructureConverter(StructureType type)
        {
            switch (type)
            {
#if HSI
                case StructureType.HSI:
                   return new UmlToFsm();
#endif
#if DFS
                case StructureType.DFS:
                    return new UmlToGraph();
#endif
#if WP
                case StructureType.Wp:
                    return new UmlToFsm();
#endif
            }
            return null;
        }
    }
}
