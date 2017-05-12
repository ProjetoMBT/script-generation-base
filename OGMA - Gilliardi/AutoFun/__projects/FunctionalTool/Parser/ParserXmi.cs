using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using FunctionalTool.Modeling.Uml;
using FunctionalTool.Testing.Functional;
using System.IO;
using System.Windows.Forms;

namespace FunctionalTool
{
    public class ParserXmi
    {
        public static Dictionary<String, UmlActionStateDiagram> dicActivityDiagram;
        private static XmlDocument XmlDoc;
        public static UmlUseCaseDiagram useCaseDiagram;

        public static void Parser(string filePath)
        {
           XmlDocument XmlDoc = new XmlDocument();
           XmlDoc.Load(filePath);         
           dicActivityDiagram = UmlActionStateDiagram.ParseActivityDiagram(XmlDoc);
           useCaseDiagram = UmlUseCaseDiagram.ParseUmlUseCaseDiagram(XmlDoc, dicActivityDiagram);
           
        }
        public static void Clear()
        {
            dicActivityDiagram = new Dictionary<string, UmlActionStateDiagram>();
            XmlDoc = new XmlDocument();
            UmlActionStateDiagram.cleardicJudeHyperLinks();
        }
    }
}
