using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Coc.Data.ControlAndConversionStructures;

namespace Coc.Data.Interfaces
{
    public interface ParsedStructureExporter
    {
        XmlDocument ToXmi(List<GeneralUseStructure> model);
    }
}
