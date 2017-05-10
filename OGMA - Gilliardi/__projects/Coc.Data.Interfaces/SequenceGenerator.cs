using System.Collections.Generic;
using Coc.Data.ControlStructure;
using Coc.Data.ConversionUnit;
using Coc.Data.ControlAndConversionStructures;

namespace Coc.Data.Interfaces
{
    public class SequenceGenerator
    {
        public List<GeneralUseStructure> ConvertStructure(List<GeneralUseStructure> listGeneralUseStructure, StructureType type)
        {

            // switch (generalUseStructure.GetModelType())
            //  {
            //    case StructureType.UmlModel:
            ModelingStructureConverterFactory msf = new ModelingStructureConverterFactory();
            ModelingStructureConverter msc = msf.CreateModelingStructureConverter(type);
            List<GeneralUseStructure> sgs = msc.Converter(listGeneralUseStructure, StructureType.None);
            return sgs;

            //   case StructureType.None:
            // break;
            //}
            // return null;

        }

        // public abstract List<GeneralUseStructure> GenerateSequence(GeneralUseStructure model, ref int tcCount, StructureType type);
        public virtual List<GeneralUseStructure> GenerateSequence(List<GeneralUseStructure> listGeneralStructure, ref int tcCount, StructureType type)
        {
            return listGeneralStructure;
        }

    }
}
