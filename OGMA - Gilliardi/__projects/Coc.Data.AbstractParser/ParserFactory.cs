//#define XMI
//#define OATS
//#define LR

#define AUX


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coc.Data.Interfaces;

#if XMI
using Coc.Data.Xmi;
#endif
#if OATS
//using OgmaOATSParser;
using OgmaJOATSParser;
#endif
#if LR
using Coc.Data.ReadLR;
#endif

namespace Coc.Data.AbstractParser
{
    /*
    /// <summary>
    /// <img src="images/AbstractParser.PNG"/>
    /// </summary>
    public static class NamespaceDoc
    {
    }*/


    public class ParserFactory
    {
        public static Parser CreateParser(String parserType)
        {
            switch (parserType)
            {
#if AUX
                case "Xmi to OATS":
                    return new XmiToOATS();
#endif
#if OATS
                case "Script JAVA":
                    return new OgmaJ();
#endif
#if XMI
                case "Astah XML":
                    return new XmiImporter();
                case "Argo XML":
                    return new XmlArgoUml();
                case "Enterprise Architect":
                    return new Enterprise_ArchitectImporter2();
#endif
#if LR
                case "LoadRunnerToXMI":
                    return new LoadRunnerToXMI();
#endif
            }
            return null;
        }
    }
}