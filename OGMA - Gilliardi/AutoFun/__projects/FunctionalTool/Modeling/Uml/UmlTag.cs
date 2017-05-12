using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web;
using System.Windows.Forms;

namespace FunctionalTool.Modeling.Uml
{
    public class UmlTag 
    {
        public class eInvalidTag : Exception { }

        public String id { get; set; }
        public String name { get; set; }
        public String value { get; set; }

        public static void ParserTag(XmlNode node, XmlNamespaceManager ns, UmlBase UmlElement)
        {
            foreach (XmlNode XmlNode in node.SelectNodes("//UML:" + UmlElement.GetType().Name.Substring(3) + "[@xmi.id='" + UmlElement.Id + "']//UML:TaggedValue", ns))
            {
                UmlTag tag = new UmlTag();
                tag.name = XmlNode.Attributes["tag"].Value;
                tag.id = XmlNode.Attributes["xmi.id"].Value;
                tag.value = HttpUtility.UrlDecode(XmlNode.Attributes["value"].Value).Replace("+", " ");

                switch (UmlElement.GetType().Name)
                {
                    case "UmlTransition":
                        switch (tag.name)
                        {
                            case "FTexpectedResult":
                                UmlTransition transition = UmlElement as UmlTransition;
                                transition.listExpectedResults.Add(tag);
                                break;
                            case "FTaction":
                                UmlElement.dictionaryTag.Add(tag.id, tag);
                                break;
                            default:
                                DialogResult dialogResult = MessageBox.Show("Use case" + tag.name + " has a invalid Tag. Are you sure you want to continue executing a parsing method?", "Error", MessageBoxButtons.YesNo);

                                if (dialogResult == DialogResult.No)
                                {
                                    throw new eInvalidTag();
                                }
                                break;
                        }
                        break;
                    default:
                        break;
                }
                if (tag.name.Equals("jude.hyperlink"))
                {
                    UmlActionState umlActivity = UmlElement as UmlActionState;
                    tag.value = getHyperLink(XmlNode.Attributes["value"].Value);
                    umlActivity.dicJudeHyperLink.Add(tag.value, tag.value);
                }        
            }
        }

        private static String getHyperLink(String s)
        {
            String result = s.Substring(22);
            String[] aux = result.Split('%');
            return aux[0];
        }

    }
}
