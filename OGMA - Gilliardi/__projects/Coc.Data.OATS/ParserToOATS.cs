using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Coc.Modeling.Uml;
using System.Xml;
using Coc.Data.Xmi;
using Coc.Modeling.FiniteStateMachine;
using Coc.Data.ConversionUnit;
using Coc.Data.Interfaces;

namespace Coc.Data.OATS
{
    /*
    /// <summary>
    /// <img src="images/OATS.PNG"/>
    /// </summary>
    public static class NamespaceDoc
    {
    }*/



    public class ParserToOATS
    {
        StreamWriter sw = null;
        FiniteStateMachine fsm = null;
        State currState = null;
        UmlActivityDiagram curActDiag = null;
        int tabs = 0;
        
        public void ParseXmiToOATS(string XmiPath, string OATSPath)
        {
            UmlModel model = new UmlModel("");
            XmlDocument xmiDoc = new XmlDocument();
            xmiDoc.Load(XmiPath);
            XmiImporter importer = new XmiImporter();
            String name = "";
            model = importer.FromXmi(xmiDoc, ref name);
           

            if (model.Diagrams.Count(X => X is UmlActivityDiagram) > 1)
                throw new Exception("Only one Activity diagram allowed! (For now)");
            try
            {
                UmlToFsm umlToFsm = new UmlToFsm();
                foreach (UmlActivityDiagram actDiag in model.Diagrams)
                {
                    tabs = 0;
                    sw = new StreamWriter(OATSPath + "\\script.java");
                    fsm = umlToFsm.ActivityDiagramToFsm(actDiag, model);
                    //fsm.
                    currState = fsm.InitialState;
                    curActDiag = actDiag;
                    S();
                    sw.Close();
                    break;//only one
                }
            }
            catch (Exception ex)
            {
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
            sw.WriteLine("import oracle.oats.scripting.modules.basic.api.*;");
            sw.WriteLine("import oracle.oats.scripting.modules.browser.api.*;");
            sw.WriteLine("import oracle.oats.scripting.modules.functionalTest.api.*;");
            sw.WriteLine("import oracle.oats.scripting.modules.utilities.api.*;");
            sw.WriteLine("import oracle.oats.scripting.modules.utilities.api.sql.*;");
            sw.WriteLine("import oracle.oats.scripting.modules.utilities.api.xml.*;");
            sw.WriteLine("import oracle.oats.scripting.modules.utilities.api.file.*;");
            sw.WriteLine("import oracle.oats.scripting.modules.webdom.api.*;");
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
        }

        private void METHODS()
        {
            tabs++;
            String s = new String('\t', tabs);
            sw.WriteLine(s + "public void initialize() throws Exception {");
            INIT();
            sw.WriteLine(s + "}");
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
            //sw.WriteLine(s + "browser.launch();");
            //sw.WriteLine(s + "web.window(\"/web:window[@index='0']\").navigate(\"http://www.cepes.pucrs.br/epesi/#1\");");
            tabs--;
        }

        private void nextState()
        {
            IEnumerable<Transition> ts = fsm.Transitions.Where(x => x.SourceState.Equals(currState));

            State next = null;

            foreach (Transition t in ts)
            {
                next = t.TargetState;
                break;//first transition
            }

            currState = next;
        }

        private void STEPS()
        {
            tabs++;
            String s = new String('\t', tabs);
            sw.WriteLine(s + "beginStep(\"" + currState.Name + "\", 0);");
            sw.WriteLine(s + "{");
            IEnumerable<UmlTransition> trans = null;
            while (currState != null)
            {
                UmlActionState action = null;
                action = (UmlActionState)curActDiag.UmlObjects.Single(x => x.Name.Equals(currState.Name));
                trans = curActDiag.UmlObjects.OfType<UmlTransition>().Where(x => x.Target.Equals(action));
                foreach (UmlTransition t in trans)
                {
                    String TDobject = t.TaggedValues.FirstOrDefault(x => x.Key.Equals("TDOBJECT")).Value;
                    if (TDobject != null && !TDobject.Equals(""))
                    {
                        if (TDobject.Equals("browser"))
                            WEB_BROWSER(t.TaggedValues);
                        else if (TDobject.Equals("button"))
                            WEB_BUTTON(t.TaggedValues);
                        else if (TDobject.Equals("input#text"))
                            WEB_TEXT("Text", t.TaggedValues);
                        else if (TDobject.Equals("input#password"))
                            WEB_TEXT("Password", t.TaggedValues);
                        else if (TDobject.Equals("web.window"))
                            WEB_WINDOW(t.TaggedValues);
                        else if (TDobject.Equals("link"))
                            WEB_LINK(t.TaggedValues);
                    }
                }
                nextState();
            }
            sw.WriteLine(s + "}");
            sw.WriteLine(s + "endStep();");
            tabs--;
        }

        private void WEB_BROWSER(Dictionary<string, string> tags)
        {
            tabs++;
            String s = new String('\t', tabs);

            String TDaction = tags.FirstOrDefault(x => x.Key.Equals("TDACTION")).Value;
            String TDobject = tags.FirstOrDefault(x => x.Key.Equals("TDOBJECT")).Value;

            String web_browser = s + TDobject + "." + TDaction + "();";

            sw.WriteLine(web_browser);
            tabs--;
        }

        private void WEB_LINK(Dictionary<string, string> tags)
        {
            tabs++;
            String s = new String('\t', tabs);

            String TDaction = tags.FirstOrDefault(x => x.Key.Equals("TDACTION")).Value;
            String TDtext = tags.FirstOrDefault(x => x.Key.Equals("TDTEXT")).Value;
            String TDhref = tags.FirstOrDefault(x => x.Key.Equals("TDHREF")).Value;
            String TDobject = tags.FirstOrDefault(x => x.Key.Equals("TDOBJECT")).Value;

            String web_link = s + "web.link(\"//web:a[@text='" + TDtext + "' or @href='" + TDhref + "']\")";
            if (TDaction != null)
            {
                web_link += "." + TDaction + "()";
            }
            web_link += ";";

            sw.WriteLine(web_link);
            tabs--;
        }

        private void WEB_WINDOW(Dictionary<string, string> tags)
        {
            tabs++;
            String s = new String('\t', tabs);
            String TDaction = tags.FirstOrDefault(x => x.Key.Equals("TDACTION")).Value;
            String TDurl = tags.FirstOrDefault(x => x.Key.Equals("TDURL")).Value;
            String TDtitle = tags.FirstOrDefault(x => x.Key.Equals("TDTITLE")).Value;
            String TDobject = tags.FirstOrDefault(x => x.Key.Equals("TDOBJECT")).Value;
            String TDthink = tags.FirstOrDefault(x => x.Key.Equals("TDTHINK")).Value;
            String web_window = s + "web.window(\"" + TDtitle + "\")";

            if (TDaction != null)
            {
                if (TDaction.Equals("navigate"))
                {
                    web_window += ".navigate(\"" + TDurl + "\");";
                }
                else if (TDaction.Equals("close"))
                {
                    web_window += ".close();";
                }
                else if (TDaction.Equals("waitPage"))
                {
                    if (TDurl.Equals("null"))
                        web_window += ".waitForPage(null);";
                    else
                        web_window += ".waitForPage(\"" + TDurl + "\");";
                }
                else
                {
                    web_window += ";";
                }
            }
            if (TDthink != null)
            {
                web_window += Environment.NewLine;
                web_window += s + "{";
                web_window += Environment.NewLine;
                web_window += s + "\tthink(" + TDthink + ");";
                web_window += Environment.NewLine;
                web_window += s + "}";
            }
            sw.WriteLine(web_window);
            tabs--;
        }

        private void WEB_TEXT(string type, Dictionary<string, string> tags)
        {
            tabs++;
            String s = new String('\t', tabs);
            String TDobject = tags.FirstOrDefault(x => x.Key.Equals("TDOBJECT")).Value;
            String TDname = tags.FirstOrDefault(x => x.Key.Equals("TDNAME")).Value;
            String TDvalue = tags.FirstOrDefault(x => x.Key.Equals("TDVALUE")).Value;
            String TDthink = tags.FirstOrDefault(x => x.Key.Equals("TDTHINK")).Value;
            String TDaction = tags.FirstOrDefault(x => x.Key.Equals("TDACTION")).Value;
            String web_input = s + "web.textBox(\"//web:input_" + type.ToLower() + "[@id='" + TDname + "' or @name='" + TDname + "']\")";

            if (TDvalue != null)
            {
                //char[] t = TDobject.Split('#')[1].ToCharArray();
                //String input = (new String(t[0], 1)).ToUpper();
                //for (int i = 1; i < t.Length; i++)
                //{
                //    input += t[i];
                //}
                web_input += ".set" + type + "(" + TDvalue + ")";
            }
            if (TDaction != null)
            {
                web_input += "." + TDaction + "()";
            }
            web_input += ";";
            if (TDthink != null)
            {
                web_input += Environment.NewLine;
                web_input += s + "{";
                web_input += Environment.NewLine;
                web_input += s + "\tthink(" + TDthink + ");";
                web_input += Environment.NewLine;
                web_input += s + "}";
            }
            sw.WriteLine(web_input);
            tabs--;
        }

        private void WEB_BUTTON(Dictionary<string, string> tags)
        {
            tabs++;
            String s = new String('\t', tabs);
            String TDobject = tags.FirstOrDefault(x => x.Key.Equals("TDOBJECT")).Value;
            String TDname = tags.FirstOrDefault(x => x.Key.Equals("TDNAME")).Value;
            String TDvalue = tags.FirstOrDefault(x => x.Key.Equals("TDVALUE")).Value;
            String TDaction = tags.FirstOrDefault(x => x.Key.Equals("TDACTION")).Value;
            String web_button =s + "web.button(\"//web:input_submit[@name='" + TDname + "'";

            if (TDvalue != null)
            {
                web_button += " or  @value='" + TDvalue + "'";
            }
            web_button += "]\")";

            if (TDaction != null)
            {
                web_button += "." + TDaction + "()";
            }

            web_button += ";";
            sw.WriteLine(web_button);
            tabs--;
        }
    }
    }

