﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using Coc.Data.ControlAndConversionStructures;
using Coc.Data.Interfaces;
using Coc.Modeling.Uml;
using Coc.Modeling.FiniteStateMachine;
using Coc.Data.ConversionUnit;
using Coc.Modeling.Graph;
using System.Web;
using System.Text.RegularExpressions;
using Coc.Data.Xmi.Script;
using System.Reflection;

namespace Coc.Data.Xmi
{
    public class XmiToOATS : Parser
    {
        #region Attributes
        private List<GeneralUseStructure> listModelingStructure;
        private Regex param;
        private Regex activityName;
        private StreamWriter sw;
        private DirectedGraph dg;
        private Node currState;
        private UmlActivityDiagram curActDiag;
        private Boolean loop;
        private int tabs;
        private int i;

        #endregion



        public XmiToOATS()
        {
            this.i = 0;
            this.tabs = 0;
            this.loop = false;
            this.listModelingStructure = new List<GeneralUseStructure>();
            this.activityName = new Regex("^[\\w|\\W]+[^\\d]+");
            this.param = new Regex(@"(?<param>\[(?<file>[ZéúíóáÉÚÍÓÁèùìòàÈÙÌÒÀõãñÕÃÑêûîôâÊÛÎÔÂëÿüïöäËYÜÏÖÄçÇ\sa-zA-Z0-9_!#$%&'+\/=?^`{|}~-]*).(?<column>[ZéúíóáÉÚÍÓÁèùìòàÈÙÌÒÀõãñÕÃÑêûîôâÊÛÎÔÂëÿüïöäËYÜÏÖÄçÇ\sa-zA-Z0-9_!#$%&'+\/=?^`{|}~-]*)\])", RegexOptions.IgnoreCase);
        }


        #region Public Methods
        public override StructureCollection ParserMethod(String path, ref String name, Tuple<String, Object>[] args)
        {
            StructureCollection model = new StructureCollection();
            XmlDocument document = new XmlDocument();
            document.Load(path);
            ToOATS(document);
            model.listGeneralStructure.AddRange(listModelingStructure);

            return model;
        }
        #endregion

        #region Private Methods
        private void ToOATS(XmlDocument xmiDoc)
        {
            String toTestRegex = "{click},{setText;[data1.username]},{pressTab}";
            Boolean testedRegex = RegexTest(toTestRegex);

            UmlModel model = new UmlModel("");
            XmiImporter importer = new XmiImporter();
            String name = "";
            model = importer.FromXmi(xmiDoc, ref name);

            string[] parts = xmiDoc.BaseURI.Split(new char[]{'/'});
            string fName = parts[parts.Length-1];
            fName = fName.Substring(0, fName.IndexOf('.'));

            try
            {
                foreach (UmlActivityDiagram actDiag in model.Diagrams)
                {
                    tabs = 0;
                    UmlToGraphOATS umlToGraphOATS = new UmlToGraphOATS();
                    sw = new StreamWriter(Configuration.getInstance().getConfiguration(Configuration.Fields.workspacepath) + fName + "_OATS.java");
                    dg = umlToGraphOATS.ActivityDiagramToGraph(actDiag, model);
                    OrderEdges(dg);
                    OrderActDiagramTransitions(actDiag);
                    currState = dg.RootNode;
                    curActDiag = actDiag;
                    S();
                    sw.Close();
                    break;
                }
            }
            catch (Exception e)
            {
                String message = e.Message;
                if (sw != null)
                {
                    sw.Close();
                }
                throw;
            }
        }

        private void S()
        {
            LIMP();
            CLS();
        }

        private void LIMP()
        {
            sw.WriteLine("/*");
            sw.WriteLine("SCRIPT AUTOMATICALLY GENERATED BY PLETS v" + Configuration.getInstance().getConfiguration(Configuration.Fields.softwareversion));
            sw.WriteLine("*/");
            sw.WriteLine();
            sw.WriteLine("import oracle.oats.scripting.modules.basic.api.*;");
            sw.WriteLine("import oracle.oats.scripting.modules.browser.api.*;");
            sw.WriteLine("import oracle.oats.scripting.modules.functionalTest.api.*;");
            sw.WriteLine("import oracle.oats.scripting.modules.utilities.api.*;");
            sw.WriteLine("import oracle.oats.scripting.modules.utilities.api.sql.*;");
            sw.WriteLine("import oracle.oats.scripting.modules.utilities.api.xml.*;");
            sw.WriteLine("import oracle.oats.scripting.modules.utilities.api.file.*;");
            sw.WriteLine("import oracle.oats.scripting.modules.webdom.api.*;");
            sw.WriteLine("import oracle.oats.scripting.modules.formsFT.api.*;");
            sw.WriteLine("import oracle.oats.scripting.modules.applet.api.*;");
            
            
            sw.WriteLine();
        }

        private void CLS()
        {
            //public....{
            sw.WriteLine("public class script extends IteratingVUserScript {");
            BODY();
            //}
            sw.WriteLine("}");
        }

        private void BODY()
        {
            SCRS();
            METHODS();
        }

        private void SCRS()
        {
            String s = new String('\t', tabs + 1);
            sw.WriteLine(s + "@ScriptService oracle.oats.scripting.modules.utilities.api.UtilitiesService utilities;");
            sw.WriteLine(s + "@ScriptService oracle.oats.scripting.modules.browser.api.BrowserService browser;");
            sw.WriteLine(s + "@ScriptService oracle.oats.scripting.modules.functionalTest.api.FunctionalTestService ft;");
            sw.WriteLine(s + "@ScriptService oracle.oats.scripting.modules.webdom.api.WebDomService web;");
	        sw.WriteLine(s + "@ScriptService oracle.oats.scripting.modules.applet.api.AppletService applet;");
            sw.WriteLine(s + "@ScriptService oracle.oats.scripting.modules.formsFT.api.FormsService forms;");
            sw.WriteLine();
        }

        private void METHODS()
        {
            tabs++;
            String s = new String('\t', tabs);
            sw.WriteLine(s + "public void initialize() throws Exception {");
            INIT();
            sw.WriteLine(s + "}");
            sw.WriteLine();
            sw.WriteLine(s + "public void run() throws Exception {");
            STEPS();
            sw.WriteLine(s + "}");
            sw.WriteLine(s + "public void finish() throws Exception {");
            sw.WriteLine(s + "}");
            tabs--;
        }

        private void INIT()
        {
            tabs++;
            String s = new String('\t', tabs);
            sw.WriteLine(s + "browser.closeAllBrowsers();");
            sw.WriteLine(s + "browser.launch();");
            //sw.WriteLine(s + "web.window(\"/web:window[@index='0']\").navigate(\"http://www.cepes.pucrs.br/epesi/#1\");");
            tabs--;
        }



        private void STEPS()
        {
            TabHelper tab = new TabHelper(tabs);
            ScriptSequence sequence = new ScriptSequence(i);

            ScriptParser scriptParser = new ScriptParser(tab, sequence);

            //agrupa as transicoes de acordo com o nome...
            IEnumerable<IGrouping<string, UmlTransition>> groups = 
                curActDiag.UmlObjects.OfType<UmlTransition>()
                .GroupBy(t => getFriendlyName(t.Source.Name), t => t);


            //organiza em GroupNodes
            GroupNode root = new GroupNode();
            root.GroupName = groups.First().Key;
            root.Transitions = groups.First().ToList();

            groups = groups.Where(x => !x.Key.Equals(root.GroupName));

            foreach (IGrouping<string, UmlTransition> group in groups)
            {
                GroupNode gn = new GroupNode();
                gn.GroupName = group.Key;
                group.ToList().ForEach(
                    x => fixName(x)
                );

                gn.Transitions = group.ToList();

                root.SubGroups.Add(gn);
            }


            GroupNode father = null;
            GroupNode next = null;
            GroupNode prev = null;

            for(int j=0; j < root.SubGroups.Count; j++)
            {
                if (root.SubGroups[j] != null)
                {
                    //ajusta os passos que estao desalinhados ate este ponto
                    if ((j + 1 < root.SubGroups.Count) && root.SubGroups[j].Transitions.Last().Target.Name.Contains(root.SubGroups[j + 1].GroupName))
                    {
                        root.SubGroups[j + 1].Transitions.Insert(0, root.SubGroups[j].Transitions.Last());
                        root.SubGroups[j].Transitions.RemoveAt(root.SubGroups[j].Transitions.Count - 1);
                    }

                    //seleciona o pai, este pai, pois caso o proximo seja filho, este ja é o pai dos proximos
                    if (root.SubGroups[j].Transitions.Count > 0 &&
                        !root.SubGroups[j].Transitions.Last().TaggedValues.ContainsKey("paramcycle"))
                    {
                        //ajusta a ref. dos irmaos pois quando trocar o pai, os irmaos nao terao a a mesma descendencia...
                        prev = next = null;
                        father = root.SubGroups[j];
                    }


                    //identifica os filhos no subdiagrama...
                    if (root.SubGroups[j].Transitions.Count > 0 &&
                        root.SubGroups[j].Transitions.Last().TaggedValues.ContainsKey("paramcycle"))
                    {
                        root.SubGroups[j].Father = father;
                        root.SubGroups[j].NextSibling = next;
                        root.SubGroups[j].PrevSibling = prev;

                        //ajusta a ref. do diagrama principal
                        father.SubGroups.Add(root.SubGroups[j]);
                        root.SubGroups[j] = null;
                    }
                }

            }

            //remove os realocados
            root.SubGroups.RemoveAll(g => g == null);
            
            sw.WriteLine(scriptParser.parse(root));

        }


        private void fixName(UmlTransition transition)
        {
            transition.Source.Name = getFriendlyName(transition.Source.Name);
            transition.Target.Name = getFriendlyName(transition.Target.Name);
        }


        private string getFriendlyName(string obscureName)
        {
            obscureName = HttpUtility.UrlDecode(obscureName);
            obscureName = activityName.Match(obscureName).Value;
            return obscureName;
        }


        /*
        private Boolean FinishedFOR(UmlTransition t)
        {
            UmlElement target = t.Target;
            UmlTransition transition = (curActDiag.UmlObjects.OfType<UmlTransition>().Where(x => x.Source.Name.Equals(target.Name))).FirstOrDefault();

            if (!String.IsNullOrEmpty(transition.GetTaggedValue("paramcycle")))
            {
                return false;
            }
            return true;
        }

        private void FinishFOR()
        {
            String s = new String('\t', tabs);
            sw.WriteLine(s + "}");
            tabs--;
        }

        private void WriteHeaderFOR(UmlTransition t)
        {
            tabs++;
            String s = new String('\t', tabs);
            String fileName = "";
            String tdAction = HttpUtility.UrlDecode(t.GetTaggedValue("TDACTION"));

            if (RegexTest(tdAction))
            {
                int aux1 = tdAction.IndexOf('[');
                int aux2 = tdAction.IndexOf(']');
                int aux3 = aux2 - aux1;

                String parameter = tdAction.Substring(aux1, aux3 + 1);
                fileName = parameter.Split('.')[0].Substring(1);
            }

            sw.WriteLine();
            sw.WriteLine(s + "getDatabank(\"" + fileName + "\").load(\"Default\", \"DataBank/" + fileName + ".csv\", null);");
            String loop = t.GetTaggedValue("paramcycle");
            sw.WriteLine(s + "for(int i=0; i<" + loop + ";i++)");
            sw.WriteLine(s + "{");
        }

        
        private void WEB_BROWSER(Dictionary<String, String> tags)
        {
            tabs++;
            String s = new String('\t', tabs);

            String TDaction = tags.FirstOrDefault(x => x.Key.Equals("TDACTION")).Value;
            TDaction = HttpUtility.UrlDecode(TDaction);
            String TDobject = tags.FirstOrDefault(x => x.Key.Equals("TDOBJECT")).Value;
            TDobject = HttpUtility.UrlDecode(TDobject);

            String web_browser = s + TDobject + "." + TDaction + "();";

            sw.WriteLine(web_browser);
            tabs--;
        }

        private void WEB_LINK(Dictionary<String, String> tags)
        {
            tabs++;
            String s = new String('\t', tabs);

            String TDaction = tags.FirstOrDefault(x => x.Key.Equals("TDACTION")).Value;
            TDaction = HttpUtility.UrlDecode(TDaction);
            TDaction = TDaction.Substring(1);
            TDaction = TDaction.Substring(0, TDaction.Length - 1);
            String TDobject = tags.FirstOrDefault(x => x.Key.Equals("TDOBJECT")).Value;
            TDobject = HttpUtility.UrlDecode(TDobject);
            String auxTDobject = TDobject.Split(';')[1];
            auxTDobject = auxTDobject.Substring(0, auxTDobject.Length - 1);

            String web_link = s + "web.link(" + i + ", \"{{obj." + auxTDobject + "}}\")";
            i++;

            if (TDaction != null)
            {
                web_link += "." + TDaction + "()";
            }
            web_link += ";";

            sw.WriteLine(web_link);
            tabs--;
        }

        private void WEB_WINDOW(Dictionary<String, String> tags)
        {
            tabs++;
            String s = new String('\t', tabs);

            String TDaction = tags.FirstOrDefault(x => x.Key.Equals("TDACTION")).Value;
            TDaction = HttpUtility.UrlDecode(TDaction);
            TDaction = TDaction.Substring(1);
            TDaction = TDaction.Substring(0, TDaction.Length - 1);

            String[] splittedAction = TDaction.Split(';');
            TDaction = splittedAction[0];
            String TDactionParam = "";
            if (splittedAction.Length > 1)
            {
                TDactionParam = splittedAction[1];
            }
            
            String TDobject = tags.FirstOrDefault(x => x.Key.Equals("TDOBJECT")).Value;
            TDobject = HttpUtility.UrlDecode(TDobject);
            
            String auxTDobject = TDobject.Split(';')[1];
            auxTDobject = auxTDobject.Substring(0, auxTDobject.Length - 1);

            String web_window = s + "web.window(" + i + ", \"{{obj." + auxTDobject + "}}\")";
            i++;

            if (TDaction != null)
            {
                if (TDaction.Equals("navigate"))
                {
                    if (!TDactionParam.ElementAt(0).Equals("\""))
                    {
                        TDactionParam = "\"" + TDactionParam;
                    }
                    if (!TDactionParam.ElementAt(TDactionParam.Length-1).Equals("\""))
                    {
                        TDactionParam = TDactionParam + "\"";
                    }

                    web_window += ".navigate(" + TDactionParam + ");";
                }
                else if (TDaction.Equals("close"))
                {
                    web_window += ".close();";
                }
                else if (TDaction.Equals("waitForPage"))
                {
                    web_window += ".waitForPage(" + TDactionParam + ");";
                }
                else
                {
                    web_window += ";";
                }
            }

            web_window += Environment.NewLine;
            web_window += s + "{";
            web_window += Environment.NewLine;
            web_window += s + "\tthink(" + "1" + ");";
            web_window += Environment.NewLine;
            web_window += s + "}";

            sw.WriteLine(web_window);
            tabs--;
        }

        private void WEB_TEXT(Dictionary<String, String> tags)
        {
            tabs++;
            String s = new String('\t', tabs);
            String TDaction = tags.FirstOrDefault(x => x.Key.Equals("TDACTION")).Value;
            TDaction = HttpUtility.UrlDecode(TDaction);
            String TDobject = tags.FirstOrDefault(x => x.Key.Equals("TDOBJECT")).Value;
            TDobject = HttpUtility.UrlDecode(TDobject);
            String auxTDobject;

            String web_textbox = "";
            try
            {
                String[] actions = TDaction.Split(',');
                for (int j = 0; j < actions.Length; j++)
                {
                    String singleAction = actions[j];
                    singleAction = singleAction.Substring(1);
                    singleAction = singleAction.Substring(0, singleAction.Length - 1);

                    auxTDobject = TDobject.Split(';')[1];
                    auxTDobject = auxTDobject.Substring(0, auxTDobject.Length - 1);

                    if (singleAction.Contains(';'))
                    {
                        String[] actionParams = singleAction.Split(';');
                        String singleActionWithParam = actionParams[0];
                        String singleActionParam = actionParams[1];

                        //{setPassword;deobfuscate;" 7Qav0cxWTQA0OpbEx8 dw=="}
                        if (singleActionWithParam.Equals("setPassword") && (actionParams.Length > 2))
                        {
                            String singleActionSecondParam = singleAction.Split(';')[2];

                            web_textbox = s + "web.textBox(" + i + ", \"{{obj." + auxTDobject + "}}\")" + "." + singleActionWithParam + "(" + singleActionParam + "(" + singleActionSecondParam + "));";
                            i++;
                            web_textbox += Environment.NewLine;
                            web_textbox += s + "{";
                            web_textbox += Environment.NewLine;
                            web_textbox += s + "\tthink(" + "1" + ");";
                            web_textbox += Environment.NewLine;
                            web_textbox += s + "}";
                            sw.WriteLine(web_textbox);
                            continue;
                        }
                        else if (singleActionWithParam.Equals("setPassword") && (actionParams.Length == 2))
                        {
                            String fileName = "";
                            String columnName = "";

                            //{setPassword;[data1.password]}
                            if (RegexTest(singleActionParam))
                            {
                                int aux1 = singleActionParam.IndexOf('[');
                                int aux2 = singleActionParam.IndexOf(']');
                                int aux3 = aux2 - aux1;

                                String parameter = singleActionParam.Substring(aux1, aux3 + 1);
                                fileName = parameter.Split('.')[0];
                                fileName = fileName.Substring(1);
                                columnName = parameter.Split('.')[1];
                                columnName = columnName.Substring(0, columnName.Length - 1);

                                web_textbox = s + "web.textBox(" + i + ", \"{{obj." + auxTDobject + "}}\")" + "." + singleActionWithParam + "(\"{{db." + fileName + "." + columnName + "}}\");";
                                i++;
                                web_textbox += Environment.NewLine;
                                web_textbox += s + "{";
                                web_textbox += Environment.NewLine;
                                web_textbox += s + "\tthink(" + "1" + ");";
                                web_textbox += Environment.NewLine;
                                web_textbox += s + "}";
                                sw.WriteLine(web_textbox);
                                continue;
                            }
                            //{setPassword;123456}
                            else
                            {
                                web_textbox = s + "web.textBox(" + i + ", \"{{obj." + auxTDobject + "}}\")" + "." + singleActionWithParam + "(" + singleActionParam + ");";
                                i++;
                                web_textbox += Environment.NewLine;
                                web_textbox += s + "{";
                                web_textbox += Environment.NewLine;
                                web_textbox += s + "\tthink(" + "1" + ");";
                                web_textbox += Environment.NewLine;
                                web_textbox += s + "}";
                                sw.WriteLine(web_textbox);
                                continue;
                            }
                        }

                        String fileName2 = "";
                        String columnName2 = "";

                        //{setText;[data1.password]}
                        if (RegexTest(singleActionParam))
                        {
                            int aux1 = singleActionParam.IndexOf('[');
                            int aux2 = singleActionParam.IndexOf(']');
                            int aux3 = aux2 - aux1;

                            String parameter = singleActionParam.Substring(aux1, aux3 + 1);
                            fileName2 = parameter.Split('.')[0];
                            fileName2 = fileName2.Substring(1);
                            columnName2 = parameter.Split('.')[1];
                            columnName2 = columnName2.Substring(0, columnName2.Length - 1);

                            web_textbox = s + "web.textBox(" + i + ", \"{{obj." + auxTDobject + "}}\")" + "." + singleActionWithParam + "(\"{{db." + fileName2 + "." + columnName2 + "}}\");";
                            i++;
                            web_textbox += Environment.NewLine;
                            web_textbox += s + "{";
                            web_textbox += Environment.NewLine;
                            web_textbox += s + "\tthink(" + "1" + ");";
                            web_textbox += Environment.NewLine;
                            web_textbox += s + "}";
                            sw.WriteLine(web_textbox);
                            continue;
                        }
                        //{setText;user}
                        else
                        {
                            web_textbox = s + "web.textBox(" + i + ", \"{{obj." + auxTDobject + "}}\")" + "." + singleActionWithParam + "(" + singleActionParam + ");";
                            i++;
                            web_textbox += Environment.NewLine;
                            web_textbox += s + "{";
                            web_textbox += Environment.NewLine;
                            web_textbox += s + "\tthink(" + "1" + ");";
                            web_textbox += Environment.NewLine;
                            web_textbox += s + "}";
                            sw.WriteLine(web_textbox);
                            continue;
                        }
                    }
                    //click, pressTab...
                    web_textbox = s + "web.textBox(" + i + ", \"{{obj." + auxTDobject + "}}\")" + "." + singleAction + "();";
                    i++;
                    web_textbox += Environment.NewLine;
                    web_textbox += s + "{";
                    web_textbox += Environment.NewLine;
                    web_textbox += s + "\tthink(" + "1" + ");";
                    web_textbox += Environment.NewLine;
                    web_textbox += s + "}";
                    sw.WriteLine(web_textbox);
                }
            }
            //single action..
            catch
            {
                TDaction = TDaction.Substring(1);
                TDaction = TDaction.Substring(0, TDaction.Length - 1);
                auxTDobject = TDobject.Split(';')[1];
                auxTDobject = auxTDobject.Substring(0, auxTDobject.Length - 1);
                web_textbox = s + "web.textBox(" + i + ", \"{{obj." + auxTDobject + "}}\")" + "." + TDaction + "();";
                i++;
                web_textbox += Environment.NewLine;
                web_textbox += s + "{";
                web_textbox += Environment.NewLine;
                web_textbox += s + "\tthink(" + "1" + ");";
                web_textbox += Environment.NewLine;
                web_textbox += s + "}";
                sw.WriteLine(web_textbox);
            }
            tabs--;
        }

        private void WEB_BUTTON(Dictionary<String, String> tags)
        {
            tabs++;
            String s = new String('\t', tabs);
            String TDaction = tags.FirstOrDefault(x => x.Key.Equals("TDACTION")).Value;
            TDaction = HttpUtility.UrlDecode(TDaction);
            String TDobject = tags.FirstOrDefault(x => x.Key.Equals("TDOBJECT")).Value;
            TDobject = HttpUtility.UrlDecode(TDobject);

            TDaction = TDaction.Substring(1);
            TDaction = TDaction.Substring(0, TDaction.Length - 1);

            String auxTDobject = TDobject.Split(';')[1];
            auxTDobject = auxTDobject.Substring(0, auxTDobject.Length - 1);

            String web_button = s + "web.button(" + i + ", \"{{obj." + auxTDobject + "}}\")";
            i++;

            if (TDaction != null)
            {
                web_button += "." + TDaction + "()";
            }

            web_button += ";";
            sw.WriteLine(web_button);
            tabs--;
        }
        */




        private void NextState()
        {
            List<Edge> ts = (dg.Edges.Where(x => x.NodeA.Equals(currState))).ToList();

            Node next = null;

            foreach (Edge t in ts)
            {
                next = t.NodeB;
                break;//first transition
            }

            currState = next;
        }

        private Node NextStateAux(Node state)
        {
            List<Edge> ts = (dg.Edges.Where(x => x.NodeA.Equals(state))).ToList();

            Node next = null;

            foreach (Edge t in ts)
            {
                next = t.NodeB;
                break;
            }

            return next;
        }

        private Edge GetEquivalentTransition(UmlTransition transition)
        {
            foreach (Edge edge in dg.Edges)
            {
                if ((edge.NodeA.Name.Equals(transition.Source.Name)) && (edge.NodeB.Name.Equals(transition.Target.Name)))
                {
                    return edge;
                }
            }
            return null;
        }

        private Boolean RegexTest(String testedString)
        {
            return param.IsMatch(testedString);
        }

        private void OrderEdges(DirectedGraph dg)
        {
            List<Edge> orderedEdges = new List<Edge>();
            Edge initialEdge = (dg.Edges.Where(x => x.NodeA.Equals(dg.RootNode))).FirstOrDefault();
            Node actual = initialEdge.NodeB;

            orderedEdges.Add(initialEdge);

            for (int i = 0; i < dg.Edges.Count; i++)
            {
                Edge edge = (dg.Edges.Where(x => x.NodeA.Equals(orderedEdges[i].NodeB))).FirstOrDefault();
                if (edge != null)
                {
                    orderedEdges.Add(edge);
                }
            }
            dg.Edges.Clear();
            dg.Edges.AddRange(orderedEdges);
        }

        private void OrderActDiagramTransitions(UmlActivityDiagram actDiagram)
        {
            List<UmlTransition> orderedTransitions = new List<UmlTransition>();
            UmlElement initialNode = actDiagram.UmlObjects.OfType<UmlInitialState>().FirstOrDefault();
            UmlTransition initialTransition = (actDiagram.UmlObjects.OfType<UmlTransition>().Where(x => x.Source.Equals(initialNode))).FirstOrDefault();
            UmlElement actual = initialTransition.Target;

            orderedTransitions.Add(initialTransition);

            for (int i = 0; i < actDiagram.UmlObjects.OfType<UmlTransition>().ToList().Count(); i++)
            {
                UmlTransition transition = (actDiagram.UmlObjects.OfType<UmlTransition>().Where(x => x.Source.Equals(orderedTransitions[i].Target))).FirstOrDefault();
                if (transition != null)
                {
                    orderedTransitions.Add(transition);
                }
            }

            actDiagram.UmlObjects.RemoveAll(IsTransition);
            actDiagram.UmlObjects.AddRange(orderedTransitions);
        }

        private Boolean IsTransition(UmlBase element)
        {
            return element is UmlTransition;
        }
        
        #endregion
    }
}
