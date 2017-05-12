using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows;

namespace FunctionalTool.Modeling.Uml
{
    public class UmlStereotype : UmlBase {

        private static String refToObject { get; set; }

        public static void ParserStereotype(XmlDocument doc, ref Dictionary<String, UmlStereotype> dictionaryStereotype)
        {

            foreach (XmlNode StereotypeNode in doc.SelectNodes("//UML:Model/*/UML:Stereotype", ns))
            {                                                   
                
                UmlStereotype stereotype = new UmlStereotype();
                stereotype.Name = StereotypeNode.Attributes["name"].Value;
                stereotype.Id = StereotypeNode.Attributes["xmi.id"].Value;
                foreach (XmlNode NodeStereotype in doc.SelectNodes("//UML:Model/*/UML:Stereotype[@xmi.id='" + stereotype.Id + "']//JUDE:ModelElement", ns))
                {                                                        

                    refToObject = NodeStereotype.Attributes["xmi.idref"].Value;
                    
                }
                dictionaryStereotype.Add(stereotype.Id, stereotype);
            }
          
        }
      

       






        
    }
}
