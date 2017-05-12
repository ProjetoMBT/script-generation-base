using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using PlugSpl.DataStructs.UmlComponentDiagram;
using PlugSpl.Atlas;

namespace ClassicProductConfigurator.SerializedBragiFormat {
    public class Class1 : IConfigurationFormat{
        
        public void SaveTo(ComponentDiagramBragi atlas, string filename) {
            using(TextWriter writer = new StreamWriter(filename)){
                XmlSerializer serializer = new XmlSerializer(typeof(AtlasFeatureModel));
                serializer.Serialize(writer, atlas);
            }
        }

        public ComponentDiagramBragi LoadFrom(string filename){
            TextReader reader = new StreamReader(filename);
            XmlSerializer serializer = new XmlSerializer(typeof(ComponentDiagramBragi));
            ComponentDiagramBragi atlas = (ComponentDiagramBragi)serializer.Deserialize(reader);
            reader.Close();
            return atlas;
        }

        public string GetFilter() {
            return "UML Component Diagram / Smarty (*.plugcd)|*.plugcd";
        }
    }
}
