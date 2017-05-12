using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlugSpl.Atlas;
using FeatureModelEditor;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace FeatureModelEditor.Atlas {
    public class AtlasFileFormat : IFileFormat {

        public void SaveTo(AtlasFeatureModel atlas, string filename) {
            using(TextWriter writer = new StreamWriter(filename)){
                XmlSerializer serializer = new XmlSerializer(typeof(AtlasFeatureModel));
                serializer.Serialize(writer, atlas);
            }
        }

        public AtlasFeatureModel LoadFrom(string filename){
            TextReader reader = new StreamReader(filename);
            XmlSerializer serializer = new XmlSerializer(typeof(AtlasFeatureModel));
            AtlasFeatureModel atlas = (AtlasFeatureModel)serializer.Deserialize(reader);
            reader.Close();
            return atlas;
        }

        public string GetFilter() {
            return "PlugSPL Feature Model Project (*.plugfm)|*.plugfm";
        }
    }
}
