using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Coc.Data.Xmi;
using Coc.Modeling.Uml;
using System.Xml;
using System.Text.RegularExpressions;
using Coc.Data.ControlAndConversionStructures;

namespace Coc.Data.OATS
{
    public class ParserToXmi
    {
        private String line;
        private StreamReader sr;
        private UmlModel model;
        private UmlActivityDiagram actDiagram;
        private UmlActionState activity;
        private UmlActionState lastActivity;
        private Dictionary<string, string> taggedValues;
        private Regex ANY;
        private Regex IMPORT;
        private Regex OPEN_BRACKET;
        private Regex CLOSE_BRACKET;
        private Regex CLASS;
        private Regex SCR_SERV;
        private Regex MET;
        private Regex BEGIN_STEP;
        private Regex END_STEP;
        private Regex WWINDOW;
        private Regex WTEXT_BOX;
        private Regex WBUTTON;
        private Regex WLINK;
        private Regex WB_CLICK;
        private Regex WW_CLOSE;
        private Regex OPEN_COMM;
        private Regex CLOSE_COMM;
        private Regex THINKTIME;
        private Regex BROWSER_LCH;
        private long lc;

        //public bool ParseOATSToXMI(String pathOATSScript, String XMISavePath)
        //{
        //    String SPC = @"[\t\ ]";
        //    ANY = new Regex(@"([A-Za-z0-9,\(\)\[\]/\*&_#\- ""]*)");
        //    IMPORT = new Regex(@"import ([A-Za-z0-9.\*]+);");
        //    OPEN_BRACKET = new Regex(@"([\t\ ]*){");
        //    CLOSE_BRACKET = new Regex(@"([\t\ ]*)}");
        //    CLASS = new Regex(@"public class ([A-Za-z][A-Za-z0-9_]*) extends IteratingVUserScript {");
        //    SCR_SERV = new Regex(@"^" + SPC + @"*@ScriptService ([A-Za-z0-9\*\.\ ]+);$");
        //    MET = new Regex(@"public void ([A-Za-z][A-Za-z0-9_]*)\(\) throws Exception {");
        //    BEGIN_STEP = new Regex(@"(" + SPC + "*)beginStep" + ANY + ";");
        //    END_STEP = new Regex(@"(" + SPC + @"*)endStep\(\);");
        //    WWINDOW = new Regex(@"web.window\(" + ANY);
        //    WTEXT_BOX = new Regex(@"web.textBox\(" + ANY);
        //    WBUTTON = new Regex(@"web.button\(" + ANY);
        //    WLINK = new Regex(@"web.link\(" + ANY);
        //    WB_CLICK = new Regex(@"(" + SPC + @"*).click\(\);");
        //    WW_CLOSE = new Regex(@"(" + SPC + @"*).close\(\);");
        //    OPEN_COMM = new Regex(@"(" + SPC + @"*)/\*\*");
        //    CLOSE_COMM = new Regex(@"(" + SPC + @"*)*[\ ]*/");
        //    THINKTIME = new Regex(@"think\([0-9]+(.[0-9])*\)");
        //    BROWSER_LCH = new Regex(@"browser.launch\(\)");

        //    using (sr = new StreamReader(pathOATSScript))
        //    {
        //        lc = 0;
        //        line = "";
        //        model = new UmlModel("OATS Parsed Model");
        //        actDiagram = new UmlActivityDiagram("Test Diagram");
        //        UmlInitialState initial = new UmlInitialState();
        //        UmlFinalState final = new UmlFinalState();
        //        activity = null;
        //        lastActivity = null;
        //        actDiagram.UmlObjects.Add(initial);
        //        lastActivity = initial;
        //        taggedValues = null;
        //        S();

        //        if (sr.Peek() > 0)
        //            throw new Exception("Parser error. Not EOF");

        //        activity = final;
        //        actDiagram.UmlObjects.Add(final);
        //        addTransition();

        //        model.AddDiagram(actDiagram);
        //        XmlDocument xd = model.ToXmi();

        //        XmlWriterSettings settings = new XmlWriterSettings();
        //        settings.Encoding = new UTF8Encoding(false);
        //        settings.Indent = true;
        //        settings.CheckCharacters = true;

        //        using (XmlWriter writer = XmlWriter.Create(XMISavePath + "\\" + model.Name + ".xml", settings))
        //            xd.Save(writer);

        //        return true;
        //    }
        //}

        private void SetTaggedValues(Dictionary<string, string> values, UmlBase umlElement)
        {
            if (values == null)
                return;
            foreach (KeyValuePair<string, string> kp in values)
                umlElement.SetTaggedValue(kp.Key, kp.Value);
        }

        private void addTransition()
        {
            if (activity != null)
            {
                if (lastActivity != null)
                {
                    UmlTransition tran = new UmlTransition();
                    tran.End1 = lastActivity;
                    tran.End2 = activity;
                    SetTaggedValues(taggedValues, tran);
                    actDiagram.UmlObjects.Add(tran);
                    lastActivity = null;
                    taggedValues = null;
                }
            }
        }

        private void S()
        {
            check(ANY);
            LIMP();
            WHITES();
            CLS();
        }

        private void WHITES()
        {
            Regex whites = new Regex(@"^([\t\ \n\r]*)$");
            //Regex coment = new Regex(@"^([\t\ \n\r]*[//]" + ANY + ")$");
            while (whites.Match(line).Success)// || coment.Match(line).Success)
            {
                check(ANY);
            }
        }

        private void LIMP()
        {
            if (line.Trim().StartsWith("import") && line.EndsWith(";"))
            {
                //IMP()
                check(IMPORT);
                LIMP();
            }
        }

        private void CLS()
        {
            //public....{
            check(CLASS);
            BODY();
            //}
            check(CLOSE_BRACKET);
        }

        private void BODY()
        {
            WHITES();
            SCRS();
            WHITES();
            METHODS();
            WHITES();
        }

        private void SCRS()
        {
            if (line.Trim().StartsWith("@ScriptService") && line.EndsWith(";"))
            {
                //SCR();
                check(SCR_SERV);
                SCRS();
            }
        }

        private void METHODS()
        {
            JAVADOC();
            if (line.Replace("\t", "").Trim().StartsWith("public void"))
            {
                METHOD();
                WHITES();
                METHODS();
                WHITES();
            }
        }

        private void JAVADOC()
        {
            if (line.Replace("\t", "").Trim().StartsWith("/**"))
            {
                check(OPEN_COMM);
                while (!CLOSE_COMM.Match(line).Success)
                {
                    check(ANY);
                }
                //*/
                check(CLOSE_COMM);
            }
        }

        private void METHOD()
        {
            //public void NAME()...
            check(MET);
            BLOCK();
        }

        private void BLOCK()
        {
            STEPS();
            WHITES();
            COM_BLOCK();
            //while (!CLOSE_BRACKET.Match(line).Success)
            //check(ANY);
            //}
            check(CLOSE_BRACKET);
        }

        private void STEPS()
        {
            if ((line.Trim().StartsWith("beginStep") && line.EndsWith(";"))
                || (line.Trim().StartsWith("endStep") && line.EndsWith(";")))
            {
                //criar umlLane, 

                STEP();
                STEPS();
            }
        }

        private void STEP()
        {
            if (line.Trim().StartsWith("beginStep") && line.EndsWith(";"))
            {
                check(BEGIN_STEP);
                SBLOCK();
            }
            else if (line.Trim().StartsWith("endStep") && line.EndsWith(";"))
            {
                if (activity != null)
                {
                    actDiagram.UmlObjects.Add(activity);
                    lastActivity = activity;
                    activity = null;
                }
                check(END_STEP);
            }
        }

        private void SBLOCK()
        {
            check(OPEN_BRACKET);
            COM_BLOCK();
            check(CLOSE_BRACKET);
        }

        private void COM_BLOCK()
        {
            while (!CLOSE_BRACKET.Match(line).Success)
            {
                if (BROWSER_LCH.Match(line).Success)
                {
                    iniActivity();
                    taggedValues.Add("TDOBJECT", "browser.launch");
                    endActivity("Browser Launch");
                    check(BROWSER_LCH);
                }
                else if (line.Trim().StartsWith("web.window"))
                {
                    WEB_WINDOW();
                }
                else if (line.Trim().StartsWith("web.textBox"))
                {
                    WEB_TEXT();
                }
                else if (line.Trim().StartsWith("web.button"))
                {
                    WEB_BUTTON();
                }
                else if (line.Trim().StartsWith("web.link"))
                {
                    WEB_LINK();
                }
                //Gr{
                //else if (line.Trim().StartsWith("web.element"))
                //{
                //    WEB_ELEMENT();
                //}
                else
                {
                    check(ANY);
                }
            }
        }

        private void endActivity(String name)
        {
            activity.Name = name;
            addTransition();
            actDiagram.UmlObjects.Add(activity);
            lastActivity = activity;
            activity = null;
        }

        private void iniActivity()
        {
            activity = new UmlActionState();
            taggedValues = new Dictionary<string, string>();
        }

        private void WEB_BUTTON()
        {
            Regex nameId = new Regex(@"(?<nameid>@(id|name)='[A-Za-z]+[A-Za-z0-9_]*')");
            Regex value = new Regex(@"(?<value>@value='[A-Za-z]+[A-Za-z0-9_]*')");
            Regex startNameId = new Regex(@"^" + nameId);
            Regex input = new Regex(@"(?<websub>web:input_(submit)\[(" + nameId + "|" + value + @"|[A-Za-z\ @=0-9\'])+\])");
            Regex rClick = new Regex(@".click\(\)");
            Match m = null;
            String name = "";
            String tagName = null;
            String tagValue = null;
            iniActivity();
            check(WBUTTON);
            tagName = "TDOBJECT";
            tagValue = "button";
            name = "Button ";
            if (!taggedValues.ContainsKey(tagName))
                taggedValues.Add(tagName, tagValue);

            while (!WB_CLICK.Match(line).Success)
            {
                m = input.Match(line);
                if (m.Success)
                {
                    if (m.Groups["nameid"].Success)
                    {
                        tagName = "TDNAME";
                        tagValue = cutBetween('\'', m.Groups["nameid"].Value);
                        taggedValues.Add(tagName, tagValue);
                    }
                    if (m.Groups["value"].Success)
                    {
                        tagName = "TDVALUE";
                        tagValue = cutBetween('\'', m.Groups["value"].Value);
                        taggedValues.Add(tagName, tagValue);
                    }
                }

                check(ANY);
            }
            m = rClick.Match(line);
            if (m.Success)
            {
                tagName = "TDACTION";
                tagValue = "click";
                if (!taggedValues.ContainsKey(tagName))
                    taggedValues.Add(tagName, tagValue);
            }
            check(WB_CLICK);
            endActivity(name);
        }

        private void WEB_TEXT()
        {
            Regex nameId = new Regex(@"(@(id|name)='[A-Za-z]+[A-Za-z0-9_]*')");
            Regex startNameId = new Regex(@"^" + nameId);
            Regex text = new Regex(@"[t|T]ext");
            Regex password = new Regex(@"[p|P]assword");
            Regex input = new Regex(@"web:input_(" + text + "|" + password + @")\[(" + nameId + @"|[A-Za-z\ @=0-9\']*)+\]");
            Regex value = new Regex(@".set(" + text + "|" + password + @")\([A-Za-z\ +/\(\)=0-9""]*\);");
            Regex pTab = new Regex(@".pressTab\(\)");
            Match m = null;
            //web.textBox(
            check(WTEXT_BOX);
            String name = null;
            String tagName = null;
            String tagValue = null;
            iniActivity();
            while (!OPEN_BRACKET.Match(line).Success)
            {
                tagName = null;
                tagValue = null;
                //		59,
                /*		"/web:window[@index='0' or @title='EPESI']/web:document[@index='0']
                 * /web:form[@id='libs_qf_1bbae4d400dd068c367ef84d19555f40' or @name='libs_qf_1bbae4d400dd068c367ef84d19555f40' or @index='0']
                 * /web:input_password[@name='password' or @index='0']")
                 * /web:input_text[@id='username' or @name='username' or @index='0']"
                //*/
                m = input.Match(line);
                if (m.Success)
                    foreach (Group g in m.Groups)
                        if (g.Success)
                            foreach (Capture c in g.Captures)
                                if (startNameId.Match(c.Value).Success)
                                {
                                    tagName = "TDNAME";
                                    tagValue = cutBetween('\'', c.Value);
                                    if (!taggedValues.ContainsKey(tagName))
                                    {
                                        name = name + tagValue;
                                        taggedValues.Add(tagName, tagValue);
                                    }
                                }
                                else if (text.Match(c.Value).Success)
                                {
                                    tagName = "TDOBJECT";
                                    tagValue = "input#text";
                                    if (!taggedValues.ContainsKey(tagName))
                                    {
                                        name = "Input ";
                                        taggedValues.Add(tagName, tagValue);
                                    }
                                }
                                else if (password.Match(c.Value).Success)
                                {
                                    tagName = "TDOBJECT";
                                    tagValue = "input#password";
                                    if (!taggedValues.ContainsKey(tagName))
                                    {
                                        name = "Input ";
                                        taggedValues.Add(tagName, tagValue);
                                    }
                                }
                /*		.setPassword(deobfuscate("CzMwzF+2+fwXkAwc6LSY/g=="));
                 *  .setText("admin");
                //*/
                m = value.Match(line);
                if (m.Success)
                    foreach (Group g in m.Groups)
                        if (g.Success)
                            foreach (Capture c in g.Captures)
                                if (value.Match(c.Value).Success)
                                {
                                    tagName = "TDVALUE";
                                    tagValue = cutBetween('(', ')', c.Value);
                                    if (!taggedValues.ContainsKey(tagName))
                                        taggedValues.Add(tagName, tagValue);
                                }
                if (pTab.Match(line).Success)
                {
                    tagName = "TDACTION";
                    tagValue = "pressTab";
                    if (!taggedValues.ContainsKey(tagName))
                        taggedValues.Add(tagName, tagValue);
                }
                thinkTime();
                check(ANY);
            }
            //{
            check(OPEN_BRACKET);
            while (!CLOSE_BRACKET.Match(line).Success)
            {
                //think(1.348);
                thinkTime();
                check(ANY);
                //}
            }
            check(CLOSE_BRACKET);
            tagName = "TDACTION";
            taggedValues.TryGetValue(tagName, out tagValue);
            endActivity(name + " " + tagValue);
        }

        private void WEB_WINDOW()
        {
            //WARNING!!!! YOU NEED TO LEARN REGEX BEFORE CONTINUING AND UNDERSTAND WHAT HAPPENED IN THIS METHOD
            String name = "window ";
            String wNumber = "";
            String title = "";
            String action = "";
            String begin = @"(?<begin>web.window\()";

            String number = @"(?<number>[0-9]+,)";
            String path = @"(?<path>""[a-zA-Z0-9:\[@=/\ \'\]-]*""[\)]*)";
            String both = @"(?<both>" + number + @"\ " + path + ")";

            String endL = @"(?<endLine>\)$)|(?<endCall>\);$)";
            String end = @"(" + endL + @"|(?<closeP>\)^;))*";
            String navigate = @"(?<navigate>\.navigate\(""[a-zA-Z:/\.#0-9]+""(" + endL + ")+)";
            String waitPage = @"(?<waitPage>\.waitForPage\([""a-zA-Z:/\.#0-9]+(" + endL + ")+)";
            String close = @"(?<close>\.close\((" + endL + ")+)";
            String met = @"(?<met>" + navigate + "|" + close + "|" + waitPage + ")";

            Regex rgTitle = new Regex(@"(?<title>@title='[A-Za-z]+[A-Za-z0-9_\-\ :]*')");

            Regex rg = new Regex("((" + begin + ")*(?<param>" + number + "$|" + path + "|" + both + @")*" + end + "){0,1}(" + met + "){0,1}");

            Match m = rg.Match(line);
            iniActivity();
            bool cnt = true;
            while (cnt)
            {
                m = rg.Match(line);
                foreach (Capture c in m.Groups["navigate"].Captures)
                {
                    taggedValues.Add("TDACTION", "navigate");
                    taggedValues.Add("TDURL", cutBetween('"', '"', c.Value));
                    action = "navigate";
                    cnt = false;
                }
                foreach (Capture c in m.Groups["close"].Captures)
                {
                    taggedValues.Add("TDACTION", "close");
                    action = "close";
                    cnt = false;
                }
                foreach (Capture c in m.Groups["waitPage"].Captures)
                {
                    taggedValues.Add("TDACTION", "waitPage");
                    taggedValues.Add("TDurl", cutBetween('(', ')', c.Value));
                    action = "wait page";
                }
                foreach (Capture c in m.Groups["path"].Captures)
                {
                    taggedValues.Add("TDTITLE", cutBetween('"', '"', c.Value));
                    Match t = rgTitle.Match(c.Value);
                    if (t.Success)
                        foreach (Capture ct in t.Groups["title"].Captures)
                            title = cutBetween('\'', ct.Value);
                }
                if (m.Groups["begin"].Success)
                    taggedValues.Add("TDOBJECT", "web.window");
                if (m.Groups["number"].Success)
                    wNumber = m.Groups["number"].Value;
                check(ANY);
                if (OPEN_BRACKET.Match(line).Success | m.Groups["endCall"].Success) cnt = false;
            }
            //{
            if (OPEN_BRACKET.Match(line).Success)
            {
                check(OPEN_BRACKET);
                while (!CLOSE_BRACKET.Match(line).Success)
                {
                    //think(1.348);
                    thinkTime();
                    check(ANY);
                }
                //}
                check(CLOSE_BRACKET);
            }

            endActivity(name + " " + wNumber + " " + title + " " + action);
        }

        private void WEB_ELEMENT()
        {
            /*web.element(
					93,
					"/web:window[@index='0' or @title='EPESI - Dashboard']/web:document[@index='0']/web:div[@id='dashboard' or @text=\"\r\n\r\n\r\n\r\nWatchdog - All        \r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\nCat.\r\n\r\n\r\nTitle\r\n\r\n\r\n\r\n\r\n   No records found   \r\n\r\n\r\n\r\n\r\n\r\nShoutbox          \r\n\r\n\r\n\r\n    Start typing to search... [All] \r\n \r\n \r\n\r\n\r\n\r\n\r\n\r\n\r\nTasks - Todo - Short-term          \r\n\r\nYour user doesn&apos;t have contact, please assign one\r\n\r\n\r\n\r\n\r\n\r\nAgenda          \r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\nStart\r\n\r\n\r\n\r\nTitle\r\n\r\n\r\n\r\n\r\n   No records found   \r\n\r\n\r\n\r\n\r\n\r\n\r\nClock        \r\n\r\n\r\n\r\nMay 08, 2014\" or @index='93']")
					.click();
			{
				think(11.656);
			}
             
			web.element(
					94,
					"/web:window[@index='0' or @title='EPESI - Dashboard']/web:document[@index='0']/web:span[@text='Contacts' or @index='10']")
					.click();
             
             */
            String name = "element ";
            String wNumber = "";
            String title = "";
            String action = "";
            String begin = @"(?<begin>web.element\()";
            String id = @"(?<id>@id='[a-zA-Z]+[a-zA-Z0-9]*')";
            String text = @"(?<text>@text='[a-zA-Z]+[a-zA-Z0-9]*')";
            String number = @"(?<number>[0-9]+,)";
            String str = @"[a-zA-Z0-9:\[@=/\ \'\]-]";
            String path = @"(?<path>""(?<div>/web:div\[(" + id + @"|[a-zA-Z0-9:@=/\ \'-])*\])|(?<span>/web:span\[(" + text + @"|[a-zA-Z0-9:@=/\ \'-])*\]|" + str + @")*""[\)]*)";
            String both = @"(?<both>" + number + @"\ " + path + ")";

            String endL = @"(?<endLine>\)$)|(?<endCall>\);$)";
            String end = @"(" + endL + @"|(?<closeP>\)^;))*";
            String navigate = @"(?<navigate>\.navigate\(""[a-zA-Z:/\.#0-9]+""(" + endL + ")+)";
            String waitPage = @"(?<waitPage>\.waitForPage\([""a-zA-Z:/\.#0-9]+(" + endL + ")+)";
            String close = @"(?<close>\.close\((" + endL + ")+)";
            String met = @"(?<met>" + navigate + "|" + close + "|" + waitPage + ")";

            Regex rgTitle = new Regex(@"(?<title>@title='[A-Za-z]+[A-Za-z0-9_\-\ :]*')");

            Regex rg = new Regex("((" + begin + ")*(?<param>" + number + "$|" + path + "|" + both + @")*" + end + "){0,1}(" + met + "){0,1}");

            Match m = rg.Match(line);
            iniActivity();
            bool cnt = true;
            while (cnt)
            {
                m = rg.Match(line);
                foreach (Capture c in m.Groups["navigate"].Captures)
                {
                    taggedValues.Add("TDACTION", "navigate");
                    taggedValues.Add("TDURL", cutBetween('"', '"', c.Value));
                    action = "navigate";
                    cnt = false;
                }
                foreach (Capture c in m.Groups["close"].Captures)
                {
                    taggedValues.Add("TDACTION", "close");
                    action = "close";
                    cnt = false;
                }
                foreach (Capture c in m.Groups["waitPage"].Captures)
                {
                    taggedValues.Add("TDACTION", "waitPage");
                    taggedValues.Add("TDURL", cutBetween('(', ')', c.Value));
                    action = "wait page";
                }
                foreach (Capture c in m.Groups["path"].Captures)
                {
                    taggedValues.Add("TDTITLE", cutBetween('"', '"', c.Value));
                    Match t = rgTitle.Match(c.Value);
                    if (t.Success)
                        foreach (Capture ct in t.Groups["title"].Captures)
                            title = cutBetween('\'', ct.Value);
                }
                if (m.Groups["begin"].Success)
                    taggedValues.Add("TDOBJECT", "web.window");
                if (m.Groups["number"].Success)
                    wNumber = m.Groups["number"].Value;
                check(ANY);
                if (OPEN_BRACKET.Match(line).Success | m.Groups["endCall"].Success) cnt = false;
            }
            if (OPEN_BRACKET.Match(line).Success)
            {
                check(OPEN_BRACKET);
                while (!CLOSE_BRACKET.Match(line).Success)
                {
                    //think(1.348);
                    thinkTime();
                    check(ANY);
                }
                //}
                check(CLOSE_BRACKET);
            }

            endActivity(name + " " + wNumber + " " + title + " " + action);
        }

        private void WEB_LINK()
        {
            Regex text = new Regex(@"(?<text>@text='[A-Za-z]+[A-Za-z0-9_]*')");
            Regex href = new Regex(@"(?<href>@href='[A-Za-z]+[A-Za-z0-9_:\(\)/%]*')");
            Regex input = new Regex(@"(?<weba>web:a\[(" + text + "|" + href + @"|[A-Za-z\ @=0-9\']*)+\])");
            Regex rClick = new Regex(@".click\(\)");
            Match m = null;
            String name = "";
            String tagName = null;
            String tagValue = null;
            iniActivity();
            check(WLINK);
            tagName = "TDOBJECT";
            tagValue = "link";
            name = "Link ";
            if (!taggedValues.ContainsKey(tagName))
                taggedValues.Add(tagName, tagValue);

            while (!WB_CLICK.Match(line).Success)
            {
                m = input.Match(line);
                if (m.Success)
                    if (m.Groups["weba"].Success)
                    {
                        if (m.Groups["text"].Success)
                        {
                            taggedValues.Add("TDTEXT", cutBetween('\'', m.Groups["text"].Value));
                            name += " " + cutBetween('\'', m.Groups["text"].Value);
                        }
                        if (m.Groups["href"].Success)
                            taggedValues.Add("TDHREF", cutBetween('\'', m.Groups["href"].Value));
                    }

                check(ANY);
            }
            m = rClick.Match(line);
            if (m.Success)
            {
                tagName = "TDACTION";
                tagValue = "click";
                if (!taggedValues.ContainsKey(tagName))
                {
                    taggedValues.Add(tagName, tagValue);
                }
            }
            check(WB_CLICK);
            endActivity(name);
        }

        private void thinkTime()
        {
            Match m;
            String tagName;
            String tagValue;
            m = THINKTIME.Match(line);
            if (m.Success)
                foreach (Group g in m.Groups)
                    if (g.Success)
                        foreach (Capture c in g.Captures)
                            if (c.Value.StartsWith("think("))
                            {
                                tagName = "TDTHINK";
                                tagValue = cutBetween('(', ')', c.Value);
                                if (!taggedValues.ContainsKey(tagName))
                                    taggedValues.Add(tagName, tagValue);
                            }
        }

        private void check(Regex expected)
        {
            Match match = expected.Match(line);
            if (match.Success)
            {
                line = sr.ReadLine();
                if (line != null)
                    line = line.Trim();
                lc++;
            }
            else
                throw new Exception("Error.\nExpected " + expected.ToString() + "\nBut found " + line + "\nAt line " + lc);
        }
        private static string groupsToString(GroupCollection gpc)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < gpc.Count; i++)
            {
                sb.AppendLine(i + " " + gpc[i]);
                foreach (Capture c in gpc[i].Captures)
                {
                    sb.AppendLine("\t" + c.Value);
                }
            }
            return sb.ToString();
        }
        private static string cutBetween(char cutChar, string input)
        {
            return cutBetween(cutChar, cutChar, input);
        }
        private static string cutBetween(char cutCharStart, char cutCharEnd, string input)
        {
            int s = input.IndexOf(cutCharStart) + 1;
            int l = input.LastIndexOf(cutCharEnd) - s;
            return input.Substring(s, l);
        }
    }
}
