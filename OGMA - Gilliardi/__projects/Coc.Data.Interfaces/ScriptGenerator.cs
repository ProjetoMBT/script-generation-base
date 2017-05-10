using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coc.Data.ControlStructure;
using Coc.Data.ControlAndConversionStructures;

namespace Coc.Data.Interfaces
{
    public interface ScriptGenerator
    {
        void GenerateScript(List<GeneralUseStructure> listPlan, String path);//, List<object> arguments = null

       
    }
}
