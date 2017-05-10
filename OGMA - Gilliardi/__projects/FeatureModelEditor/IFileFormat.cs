using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlugSpl.Atlas;

namespace FeatureModelEditor {
    public interface IFileFormat {
        void SaveTo(AtlasFeatureModel atlas, string filename);
        AtlasFeatureModel LoadFrom(string filename);
        string GetFilter(); 
    }
}
