using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlugSpl.Atlas;
using PlugSpl.DataStructs.UmlComponentDiagram;
using FeatureModelEditor;
using System.Xml.Serialization;
using System.IO;

namespace FeatureModelEditor.PlugComponentDiagram {
    public class PlugComponentDiagramParser :  IFileFormat{

        private AtlasFeatureModel model;
        private ComponentDiagramBragi bragi;
        private int counter = 0;


        public void SaveTo(AtlasFeatureModel atlas, string filename) {
         
            this.model = atlas;
            this.bragi = new ComponentDiagramBragi();

            AtlasFeature rootFeature = atlas.GetFeature(atlas.RootFeatureName);
            this.ParseComponent(null, rootFeature);


            this.bragi.AddStereotype(new Stereotype("Mutex"));
            this.bragi.AddStereotype(new Stereotype("Requires"));
            XmlSerializer serializer = new XmlSerializer(typeof(ComponentDiagramBragi));

            using(TextWriter writer = new StreamWriter(filename)){
                serializer.Serialize(writer, this.bragi);
            }

        }

        public void ParseComponent(InterfaceObject parentInterface, AtlasFeature feature){
            Component comp = new Component(feature.Name);
            this.bragi.AddComponent(comp);

            if(parentInterface != null){
                Socket so = new Socket(comp, parentInterface, (++counter).ToString());
                bragi.AddSocket(so);
            }

            foreach(AtlasFeature fchild in this.model.GetChildren(feature)){
                if(fchild.IsAbstract){
                    this.ParseInterface(comp, fchild);
                }else{
                    string iname = (++counter).ToString();
                    InterfaceObject o = new InterfaceObject(iname, comp);
                    this.bragi.AddInterface(o);

                    SMarty m = new SMarty((++counter).ToString(), SMartyBindingTimeTypes.CompileTime, o);
                    this.bragi.AddAttachment(m);
                                        
                    this.ParseComponent(o, fchild);
                }
            }
        }

        public void ParseInterface(Component parentComponent, AtlasFeature feature){
            string iname = (++counter).ToString();
            
            InterfaceObject o = new InterfaceObject(iname, parentComponent);
            this.bragi.AddInterface(o);

            SMarty m = new SMarty((++counter).ToString(), SMartyBindingTimeTypes.CompileTime, o);
            this.bragi.AddAttachment(m);

            foreach(AtlasFeature child in this.model.GetChildren(feature)){
                this.ParseComponent(o, child);
            }
        }

        public AtlasFeatureModel LoadFrom(string filename) {
            return new AtlasFeatureModel();
        }

        public string GetFilter() {
            return "PlugSPL Component Diagram file (*.plugcd) | *.plugcd";
        }
    }
}
