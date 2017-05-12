//#define XMI

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coc.Data.Interfaces;
using Coc.Data.Xmi;

namespace Coc.Data.AbstractParser
{
    public class ParsedStructureExporterFactory
    {
        public static ParsedStructureExporter CreateExporter()
        {
#if XMI
            return new XmiExporter();
           
#endif
        }
    }
}
