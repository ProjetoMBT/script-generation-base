using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coc.Data.ControlStructure;
using Coc.Data.ControlAndConversionStructures;

namespace Coc.Data.ConversionUnit
{
    /*
    /// <summary>
    /// <img src="images/ConversionUnit.PNG"/>
    /// </summary>
    public static class NamespaceDoc
    {
    }*/


    public interface ModelingStructureConverter
    {
        List<GeneralUseStructure> Converter(List<GeneralUseStructure> listModel, StructureType type);
    }
}
