using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlugSpl.Atlas;
using PlugSpl.DataStructs.UmlComponentDiagram;

namespace ClassicProductConfigurator {
    public interface IConfigurationFormat {
        void SaveTo(ComponentDiagramBragi atlas, string filename);
        ComponentDiagramBragi LoadFrom(string filename);
        string GetFilter();
    }
}
