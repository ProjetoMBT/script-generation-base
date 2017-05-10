using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace FunctionalTool.Modeling.Uml
{
    public class UmlBase
    {
       
        public String Id { get; set; }
        public String Name { get; set; }
        public Dictionary<String,UmlStereotype> Stereotypes { get; set; }
        public Dictionary<String, UmlTag> dictionaryTag { get; set; }
        
        private const string XMI_NAMESPACE_URI = "http://schema.omg.org/spec/XMI/1.3";
        private const string JUDE_NAMESPACE_URI = "http://objectclub.esm.co.jp/Jude/namespace/";
        private const string UML_NAMESPACE_URI = "org.omg.xmi.namespace.UML";
      //  private const string uml
        public static XmlNamespaceManager ns = InitializeNamespaceManager();


        private static XmlNamespaceManager InitializeNamespaceManager()
        {
            XmlNamespaceManager ns = new XmlNamespaceManager(new NameTable());
            ns.AddNamespace("XMI",XMI_NAMESPACE_URI);
            ns.AddNamespace("UML", UML_NAMESPACE_URI);
            ns.AddNamespace("JUDE", JUDE_NAMESPACE_URI);

            return ns;
        }

        public UmlBase()
        {
            dictionaryTag = new Dictionary<string, UmlTag>();
            Stereotypes = new Dictionary<string, UmlStereotype>();


        }
    }
}
