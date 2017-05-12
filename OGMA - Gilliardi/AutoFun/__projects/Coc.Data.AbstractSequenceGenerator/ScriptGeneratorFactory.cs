//#define MTM
//#define OATS

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coc.Data.Interfaces;
using Coc.Data.Excel;
using Coc.Data.OATS;

namespace Coc.Data.AbstractSequenceGenerator
{
    public class ScriptGeneratorFactory
    {
        public static ScriptGenerator CreateScriptGenerator()
        {

#if MTM
            return new MTMScriptGenerator();
#endif
#if OATS
            return new ParserToExcelOATS();
#endif
            return null;
        }
    }
}
