using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using FunctionalTool.Exceptions;

namespace FunctionalTool.Modeling.Uml
{
    public class UmlUseCase : UmlComponent
    {
        

        public Dictionary<String, UmlActionStateDiagram> dicTagHyperLinkUseCase;
        public List<String> includeList { get; set; }
        public String posCondition;
        public string FTpreConditions;
        public string FTstate;
        public string FTassigned;
        public string FTreason;
        public string FTiterationPath;
        public string FTareaPath;
        public string FTapplication;
        public string FTcomplexity;
        public string FTrisks;
        public string FTtcLifecycle;
        public string FTlifecycleType;
        public string FTtcTeamUsage;

        public UmlUseCase()
        {
            includeList = new List<string>();
            dicTagHyperLinkUseCase = new Dictionary<String, UmlActionStateDiagram>();
        }

        public UmlActionStateDiagram getActionStateDiagram(String key, Dictionary<String, UmlActionStateDiagram> dicActionDiagram)
        {
            String keyDiagram = UmlActionStateDiagram.dicJudeHyperLinks[key];
            return dicActionDiagram[keyDiagram];
        }

        public static Dictionary<String, UmlUseCase> ParseUseCase(XmlNode node, Dictionary<String, UmlActionStateDiagram> dicActionDiagram, XmlDocument doc)
        {
            Dictionary<String, UmlUseCase> dictionaryUseCase = new Dictionary<string, UmlUseCase>();
            foreach (XmlNode useCaseNode in node.SelectNodes("//UML:Model/*/UML:UseCase", ns))
            {
                UmlUseCase usecase = new UmlUseCase();
                usecase.Name = useCaseNode.Attributes["name"].Value;
                usecase.Id = useCaseNode.Attributes["xmi.id"].Value;

                foreach (XmlNode NodeTag in node.SelectNodes("//UML:Model/*/UML:UseCase[@xmi.id='" + usecase.Id + "']//UML:TaggedValue", ns))
                {
                    try
                    {
                        UmlTag tag = new UmlTag();
                        tag.id = NodeTag.Attributes["xmi.id"].Value;
                        tag.name = NodeTag.Attributes["tag"].Value;

                        if (NodeTag.Attributes["value"] != null)
                        {
                            tag.value = NodeTag.Attributes["value"].Value;
                        }

                        switch (NodeTag.Attributes["tag"].Value)
                        {
                            case "FTpostConditions":
                                if (NodeTag.Attributes["value"] != null)
                                {
                                    usecase.posCondition = NodeTag.Attributes["value"].Value;
                                }
                                break;
                            case "FTpreConditions":
                                usecase.dictionaryTag.Add(tag.id, tag);
                                break;
                            case "FTstate":
                                if (NodeTag.Attributes["value"] != null)
                                {
                                    usecase.FTstate = NodeTag.Attributes["value"].Value;
                                }
                                break;
                            case "FTassigned":
                                if (NodeTag.Attributes["value"] != null)
                                {
                                    usecase.FTassigned = NodeTag.Attributes["value"].Value;
                                }
                                break;
                            case "FTreason":
                                if (NodeTag.Attributes["value"] != null)
                                {
                                    usecase.FTreason = NodeTag.Attributes["value"].Value;
                                }
                                break;
                            case "FTiterationPath":
                                if (NodeTag.Attributes["value"] != null)
                                {
                                    usecase.FTiterationPath = NodeTag.Attributes["value"].Value;
                                }
                                break;
                            case "FTareaPath":
                                if (NodeTag.Attributes["value"] != null)
                                {
                                    usecase.FTareaPath = NodeTag.Attributes["value"].Value;
                                }
                                break;
                            case "FTapplication":
                                if (NodeTag.Attributes["value"] != null)
                                {
                                    usecase.FTapplication = NodeTag.Attributes["value"].Value;
                                }
                                break;
                            case "FTcomplexity":
                                if (NodeTag.Attributes["value"] != null)
                                {
                                    usecase.FTcomplexity = NodeTag.Attributes["value"].Value;
                                }
                                break;
                            case "FTrisks":
                                if (NodeTag.Attributes["value"] != null)
                                {
                                    usecase.FTrisks = NodeTag.Attributes["value"].Value;
                                }
                                break;
                            case "FTtcLifecycle":
                                if (NodeTag.Attributes["value"] != null)
                                {
                                    usecase.FTtcLifecycle = NodeTag.Attributes["value"].Value;
                                }
                                break;
                            case "FTlifecycleType":
                                if (NodeTag.Attributes["value"] != null)
                                {
                                    usecase.FTlifecycleType = NodeTag.Attributes["value"].Value;
                                }
                                break;
                            case "FTtcTeamUsage":
                                if (NodeTag.Attributes["value"] != null)
                                {
                                    usecase.FTtcTeamUsage = NodeTag.Attributes["value"].Value;
                                }
                                break;
                            case "jude.hyperlink":
                                tag.value = retiraCabecalhoECauda(tag.value);
                                UmlActionStateDiagram umlTD = usecase.getActionStateDiagram(tag.value, dicActionDiagram);
                                usecase.dicTagHyperLinkUseCase.Add(tag.id, umlTD);
                                break;
                            default:
                                DialogResult dialogResult = MessageBox.Show("Use case" + usecase.Name + " has a invalid Tag. Are you sure you want to continue executing a parsing method?", "Error", MessageBoxButtons.YesNo);

                                if (dialogResult == DialogResult.No)
                                {
                                    throw new InvalidTag();
                                }
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("Error"+e.Message);
                    }
                }

                foreach (XmlNode include in node.SelectNodes("//UML:Model/*/UML:UseCase[@xmi.id='" + usecase.Id + "']//UML:UseCase.include//UML:Include", ns))
                {
                    usecase.includeList.Add(include.Attributes["xmi.idref"].Value);
                }

                dictionaryUseCase.Add(usecase.Id, usecase);

            }

            return dictionaryUseCase;
        }

        private static String retiraCabecalhoECauda(String s)
        {
            String result = s.Substring(22);
            String[] aux = result.Split('%');
            return aux[0];
        }
    }

}
