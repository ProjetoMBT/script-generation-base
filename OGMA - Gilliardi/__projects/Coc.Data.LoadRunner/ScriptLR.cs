using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Resources;
using System.Reflection;
using System.Web;
using System.Diagnostics;
using Coc.Testing.Performance.AbstractTestCases;

namespace Coc.Data.LoadRunner
{
    //Class responsible for generating a LoadRunner Script
    public static class ScriptLR
    {
        /// <summary>
        /// Generates a LoadRunner script from a TestCase object.
        /// </summary>
        /// <param name="testCase">TestCase object</param>
        /// <param name="destinationPath">Destination path (folder) to the generated script</param>
        public static void GenerateScript(TestCase testCase, string destinationPath, string scriptName, SaveParameter saveParam)
        {
            //Creates the script directory
            DirectoryInfo directory = new DirectoryInfo(destinationPath);
            directory.Create();
            Assembly asm = Assembly.GetExecutingAssembly();

            #region Copies the necessary files to the script folder

            TextReader reader;
            TextWriter writer;

            //Create folder WSDL
            String pathWSDL = destinationPath + "\\WSDL";
            Directory.CreateDirectory(pathWSDL);

            reader = new StreamReader(asm.GetManifestResourceStream("PerformanceTool.Templates.web_services.ini"));
            writer = new StreamWriter(pathWSDL + "\\web_services.ini");
            writer.Write(reader.ReadToEnd());
            writer.Close();

            reader = new StreamReader(asm.GetManifestResourceStream("PerformanceTool.Templates.root_wsdl.ini"));
            writer = new StreamWriter(pathWSDL + "\\root_wsdl.ini");
            writer.Write(reader.ReadToEnd());
            writer.Close();

            reader = new StreamReader(asm.GetManifestResourceStream("PerformanceTool.Templates.FileNotUpdate.htm"));
            writer = new StreamWriter(pathWSDL + "\\FileNotUpdate.htm");
            writer.Write(reader.ReadToEnd());
            writer.Close();

            reader = new StreamReader(asm.GetManifestResourceStream("PerformanceTool.Templates.default.cfg"));
            writer = new StreamWriter(destinationPath + "\\default.cfg");
            writer.Write(reader.ReadToEnd());
            writer.Close();

            reader = new StreamReader(asm.GetManifestResourceStream("PerformanceTool.Templates.globals.h"));
            writer = new StreamWriter(destinationPath + "\\globals.h");
            writer.Write(reader.ReadToEnd());
            writer.Close();

            reader = new StreamReader(asm.GetManifestResourceStream("PerformanceTool.Templates.vuser_init.c"));
            writer = new StreamWriter(destinationPath + "\\vuser_init.c");
            writer.Write(reader.ReadToEnd());
            writer.Close();

            reader = new StreamReader(asm.GetManifestResourceStream("PerformanceTool.Templates.vuser_end.c"));
            writer = new StreamWriter(destinationPath + "\\vuser_end.c");
            writer.Write(reader.ReadToEnd());
            writer.Close();

            reader = new StreamReader(asm.GetManifestResourceStream("PerformanceTool.Templates.script.usr"));
            writer = new StreamWriter(destinationPath + "\\" + scriptName + ".usr");
            writer.Write(reader.ReadToEnd().Replace("@prmPath", scriptName + ".prm"));
            writer.Close();

            reader = new StreamReader(asm.GetManifestResourceStream("PerformanceTool.Templates.default.usp"));
            writer = new StreamWriter(destinationPath + "\\default.usp");
            writer.Write(reader.ReadToEnd());
            writer.Close();

            #endregion


            //StreamWriter to the final script file (Action.c)
            StreamWriter sw = new StreamWriter(destinationPath + "\\Action.c");

            //Writes the script file
            sw.WriteLine("Action()");
            sw.WriteLine("{");
            sw.WriteLine("web_set_max_html_param_len(\"9999999\");");
            sw.WriteLine("lr_start_transaction(\"" + testCase.Name + "\");");
            sw.WriteLine("\n");


            foreach (Transaction transaction in testCase.Transactions)
            {
                #region subtransaction
                if (transaction.Subtransactions.Count() > 0)
                {
                    bool parallel = false;
                    if (transaction.End.ThinkTime != 0)
                        sw.WriteLine("\tlr_think_time(" + transaction.Begin.ThinkTime + ");");
                    sw.WriteLine("\n");
                    sw.WriteLine("\tlr_start_transaction(\"" + transaction.Name + "\");");
                    foreach (Subtransaction sub in transaction.Subtransactions)
                    {
                        Request request = sub.Begin;
                        sub.Name = sub.Name.Replace('.', '_');
                        if (sub.Begin.IsParallel == true && parallel == false)
                        {
                            sw.WriteLine("\tweb_concurrent_start(NULL);");
                            sw.WriteLine("\n");
                            sw.WriteLine("\t\tlr_start_sub_transaction(\"" + sub.Name + "\", \"" + transaction.Name + "\");");
                            parallel = true;
                        }
                        else if (sub.Begin.IsParallel == false && parallel == true)
                        {
                            sw.WriteLine("\tweb_concurrent_end(NULL);");
                            sw.WriteLine("\n");
                            sw.WriteLine("\t\tlr_start_sub_transaction(\"" + sub.Name + "\", \"" + transaction.Name + "\");");
                            parallel = false;
                        }
                        else
                        {
                            sw.WriteLine("\t\tlr_start_sub_transaction(\"" + sub.Name + "\", \"" + transaction.Name + "\");");
                        }

                        if (sub.Begin.Method.Equals("POST"))
                        {
                            if (sub.Begin.Parameters != null && sub.Begin.Parameters.Count > 0)
                            {

                                sw.WriteLine("\t\t\tweb_submit_data(\"" + sub.Begin.Name + "\",");
                                int begin = 0, cursor = 0;
                                while (cursor < sub.Begin.Action.Length)
                                {
                                    if (sub.Begin.Action[cursor] == '{')
                                    {
                                        begin = cursor;
                                    }
                                    else if (sub.Begin.Action[cursor] == '}' && sub.Begin.Action[cursor - 1] != '}')
                                    {
                                        string replace = sub.Begin.Action.Substring(begin + 1, cursor - begin - 1);
                                        if (replace.Contains('.'))
                                        {
                                            string aux = sub.Begin.Action.Substring(begin + 1, cursor - begin - 1).ToLower().Replace('.', '_');
                                            sub.Begin.Action = sub.Begin.Action.Replace(replace, aux);
                                        }
                                    }
                                    cursor++;
                                }
                                sw.WriteLine("\t\t\t\t\"Action=" + sub.Begin.Action + "\",");
                                sw.WriteLine("\t\t\t\t\"Method=POST\",");
                                sw.WriteLine("\t\t\t\t\"RecContentType=text/html\",");


                                if (sub.Begin.Referer != "")
                                {
                                    begin = 0;
                                    cursor = 0;

                                    while (cursor < sub.Begin.Referer.Length)
                                    {
                                        if (sub.Begin.Referer[cursor] == '{')
                                        {
                                            begin = cursor;
                                        }
                                        else if (sub.Begin.Referer[cursor] == '}' && sub.Begin.Referer[cursor - 1] != '}')
                                        {
                                            string replace = sub.Begin.Referer.Substring(begin + 1, cursor - begin - 1);
                                            if (replace.Contains('.'))
                                            {
                                                string aux = sub.Begin.Referer.Substring(begin + 1, cursor - begin - 1).ToLower().Replace('.', '_');
                                                sub.Begin.Referer = sub.Begin.Referer.Replace(replace, aux);
                                            }
                                        }
                                        cursor++;
                                    }

                                }
                                sw.WriteLine("\t\t\t\t\"Referer=" + sub.Begin.Referer + "\",");

                                sw.WriteLine("\t\t\t\t\"Mode=HTTP\",");
                                if (sub.Begin.Body != "")
                                {
                                    begin = 0;
                                    cursor = 0;

                                    while (cursor < sub.Begin.Body.Length)
                                    {
                                        if (sub.Begin.Body[cursor] == '{')
                                        {
                                            begin = cursor;
                                        }
                                        else if (sub.Begin.Body[cursor] == '}' && sub.Begin.Body[cursor - 1] != '}')
                                        {
                                            string replace = sub.Begin.Body.Substring(begin + 1, cursor - begin - 1);
                                            if (replace.Contains('.'))
                                            {
                                                string aux = sub.Begin.Body.Substring(begin + 1, cursor - begin - 1).ToLower().Replace('.', '_');
                                                sub.Begin.Body = sub.Begin.Body.Replace(replace, aux);
                                            }
                                        }
                                        cursor++;
                                    }
                                    sw.WriteLine("\t\t\t\t\"Body=" + sub.Begin.Body + "\",");
                                }
                                sw.WriteLine("\t\t\t\tITEMDATA,");

                                foreach (Parameter param in request.Parameters)
                                {

                                    begin = 0;
                                    cursor = 0;
                                    while (cursor < param.Name.Length)
                                    {
                                        if (param.Name[cursor] == '{')
                                        {
                                            begin = cursor;
                                        }
                                        else if (param.Name[cursor] == '}' && param.Name[cursor - 1] != '}')
                                        {
                                            string replace = param.Name.Substring(begin + 1, cursor - begin - 1);
                                            if (replace.Contains('.'))
                                            {
                                                string aux = param.Name.Substring(begin + 1, cursor - begin - 1).ToLower().Replace('.', '_');
                                                param.Name = param.Name.Replace(replace, aux);
                                            }
                                        }
                                        cursor++;
                                    }

                                    begin = 0;
                                    cursor = 0;
                                    while (cursor < param.Value.Length)
                                    {
                                        if (param.Value[cursor] == '{')
                                        {
                                            begin = cursor;
                                        }
                                        else if (param.Value[cursor] == '}' && param.Value[cursor - 1] != '}')
                                        {
                                            string replace = param.Value.Substring(begin + 1, cursor - begin - 1);
                                            if (replace.Contains('.'))
                                            {
                                                string aux = param.Value.Substring(begin + 1, cursor - begin - 1).ToLower().Replace('.', '_');
                                                param.Value = param.Value.Replace(replace, aux);
                                            }
                                        }
                                        cursor++;
                                    }

                                    sw.WriteLine("\t\t\t\t\"Name=" + param.Name + "\", \"Value=" + HttpUtility.UrlDecode(param.Value) + "\", ENDITEM,");
                                }
                                sw.WriteLine("\t\t\t\tLAST);");
                                bool exists = false;

                                if (sub.Begin.SaveParameters.Count > 0)
                                {
                                    foreach (SaveParameter sp in sub.Begin.SaveParameters)
                                    {
                                        foreach (Rule ru in saveParam.Rules)
                                        {
                                            if (sp.Name.ToLower() == ru.Name.ToLower() && ru.Enabled)
                                            {
                                                exists = true;
                                            }

                                        }
                                    }

                                }

                            }
                            else
                            {
                                sw.WriteLine("\t\t\tweb_custom_request(\"" + sub.Begin.Name + "\",");

                                int begin = 0, cursor = 0;
                                while (cursor < sub.Begin.Action.Length)
                                {
                                    if (sub.Begin.Action[cursor] == '{')
                                    {
                                        begin = cursor;
                                    }
                                    else if (sub.Begin.Action[cursor] == '}' && sub.Begin.Action[cursor - 1] != '}')
                                    {
                                        string replace = sub.Begin.Action.Substring(begin + 1, cursor - begin - 1);
                                        if (replace.Contains('.'))
                                        {
                                            string aux = sub.Begin.Action.Substring(begin + 1, cursor - begin - 1).ToLower().Replace('.', '_');
                                            sub.Begin.Action = sub.Begin.Action.Replace(replace, aux);
                                        }
                                    }
                                    cursor++;
                                }

                                sw.WriteLine("\t\t\t\t\"URL=" + sub.Begin.Action + "\",");
                                sw.WriteLine("\t\t\t\t\"Method=POST\",");
                                sw.WriteLine("\t\t\t\t\"RecContentType=text/html\",");

                                begin = 0;
                                cursor = 0;

                                while (cursor < sub.Begin.Referer.Length)
                                {
                                    if (sub.Begin.Referer[cursor] == '{')
                                    {
                                        begin = cursor;
                                    }
                                    else if (sub.Begin.Referer[cursor] == '}' && sub.Begin.Referer[cursor - 1] != '}')
                                    {
                                        string replace = sub.Begin.Referer.Substring(begin + 1, cursor - begin - 1);
                                        if (replace.Contains('.'))
                                        {
                                            string aux = sub.Begin.Referer.Substring(begin + 1, cursor - begin - 1).ToLower().Replace('.', '_');
                                            sub.Begin.Referer = sub.Begin.Referer.Replace(replace, aux);
                                        }
                                    }
                                    cursor++;
                                }

                                begin = 0;
                                cursor = 0;

                                if (sub.Begin.Referer != "")
                                {
                                    begin = 0;
                                    cursor = 0;

                                    while (cursor < sub.Begin.Referer.Length)
                                    {
                                        if (sub.Begin.Referer[cursor] == '{')
                                        {
                                            begin = cursor;
                                        }
                                        else if (sub.Begin.Referer[cursor] == '}' && sub.Begin.Referer[cursor - 1] != '}')
                                        {
                                            string replace = sub.Begin.Referer.Substring(begin + 1, cursor - begin - 1);
                                            if (replace.Contains('.'))
                                            {
                                                string aux = sub.Begin.Referer.Substring(begin + 1, cursor - begin - 1).ToLower().Replace('.', '_');
                                                sub.Begin.Referer = sub.Begin.Referer.Replace(replace, aux);
                                            }
                                        }
                                        cursor++;
                                    }
                                }
                                sw.WriteLine("\t\t\t\t\"Referer=" + sub.Begin.Referer + "\",");

                                sw.WriteLine("\t\t\t\t\"Mode=HTTP\",");
                                if (sub.Begin.Body != "")
                                {
                                    begin = 0;
                                    cursor = 0;

                                    while (cursor < sub.Begin.Body.Length)
                                    {
                                        if (sub.Begin.Body[cursor] == '{')
                                        {
                                            begin = cursor;
                                        }
                                        else if (sub.Begin.Body[cursor] == '}' && sub.Begin.Body[cursor - 1] != '}')
                                        {
                                            string replace = sub.Begin.Body.Substring(begin + 1, cursor - begin - 1);
                                            if (replace.Contains('.'))
                                            {
                                                string aux = sub.Begin.Body.Substring(begin + 1, cursor - begin - 1).ToLower().Replace('.', '_');
                                                sub.Begin.Body = sub.Begin.Body.Replace(replace, aux);
                                            }
                                        }
                                        cursor++;
                                    }
                                    sw.WriteLine("\t\t\t\t\"Body=" + sub.Begin.Body + "\",");
                                }
                                sw.WriteLine("LAST);");

                                bool exists = false;

                                if (sub.Begin.SaveParameters.Count > 0)
                                {
                                    foreach (SaveParameter sp in sub.Begin.SaveParameters)
                                    {
                                        foreach (Rule ru in saveParam.Rules)
                                        {
                                            if (sp.Name.ToLower() == ru.Name.ToLower() && ru.Enabled)
                                            {

                                                exists = true;
                                            }

                                        }
                                    }
                                }

                            }

                            if (sub.Begin.Cookies != null && sub.Begin.Cookies.Count > 0)
                            {
                                foreach (Cookie co in request.Cookies)
                                {
                                    sw.WriteLine("\t\t\tweb_add_cookie(\"" + co.Name + "\");");
                                }
                            }
                        }

                        else
                        {
                            if (sub.Begin.Parameters.Count == 0)
                            {
                                sw.WriteLine("\t\t\tweb_url(\"" + sub.Begin.Name + "\",");
                                int begin = 0, cursor = 0;
                                while (cursor < sub.Begin.Action.Length)
                                {
                                    if (sub.Begin.Action[cursor] == '{')
                                    {
                                        begin = cursor;
                                    }
                                    else if (sub.Begin.Action[cursor] == '}' && sub.Begin.Action[cursor - 1] != '}')
                                    {
                                        string replace = sub.Begin.Action.Substring(begin + 1, cursor - begin - 1);
                                        if (replace.Contains('.'))
                                        {
                                            string aux = sub.Begin.Action.Substring(begin + 1, cursor - begin - 1).ToLower().Replace('.', '_');
                                            sub.Begin.Action = sub.Begin.Action.Replace(replace, aux);
                                        }
                                    }
                                    cursor++;
                                }
                                sw.WriteLine("\t\t\t\t\"URL=" + sub.Begin.Action + "\",");
                                sw.WriteLine("\t\t\t\t\"Resource=0\",");
                                sw.WriteLine("\t\t\t\t\"RecContentType=text/html\",");

                                begin = 0;
                                cursor = 0;
                                if (sub.Begin.Referer != "")
                                {
                                    begin = 0;
                                    cursor = 0;

                                    while (cursor < sub.Begin.Referer.Length)
                                    {
                                        if (sub.Begin.Referer[cursor] == '{')
                                        {
                                            begin = cursor;
                                        }
                                        else if (sub.Begin.Referer[cursor] == '}' && sub.Begin.Referer[cursor - 1] != '}')
                                        {
                                            string replace = sub.Begin.Referer.Substring(begin + 1, cursor - begin - 1);
                                            if (replace.Contains('.'))
                                            {
                                                string aux = sub.Begin.Referer.Substring(begin + 1, cursor - begin - 1).ToLower().Replace('.', '_');
                                                sub.Begin.Referer = sub.Begin.Referer.Replace(replace, aux);
                                            }
                                        }
                                        cursor++;
                                    }

                                }
                                sw.WriteLine("\t\t\t\t\"Referer=" + sub.Begin.Referer + "\",");

                                sw.WriteLine("\t\t\t\t\"Mode=HTTP\",");
                                if (sub.Begin.Body != "")
                                {
                                    begin = 0;
                                    cursor = 0;

                                    while (cursor < sub.Begin.Body.Length)
                                    {
                                        if (sub.Begin.Body[cursor] == '{')
                                        {
                                            begin = cursor;
                                        }
                                        else if (sub.Begin.Body[cursor] == '}' && sub.Begin.Body[cursor - 1] != '}')
                                        {
                                            string replace = sub.Begin.Body.Substring(begin + 1, cursor - begin - 1);
                                            if (replace.Contains('.'))
                                            {
                                                string aux = sub.Begin.Body.Substring(begin + 1, cursor - begin - 1).ToLower().Replace('.', '_');
                                                sub.Begin.Body = sub.Begin.Body.Replace(replace, aux);
                                            }
                                        }
                                        cursor++;
                                    }
                                    sw.WriteLine("\t\t\t\t\"Body=" + sub.Begin.Body + "\",");
                                }
                                sw.WriteLine("\t\t\t\tLAST);");

                                bool exists = false;

                                if (sub.Begin.SaveParameters.Count > 0)
                                {
                                    foreach (SaveParameter sp in sub.Begin.SaveParameters)
                                    {
                                        foreach (Rule ru in saveParam.Rules)
                                        {
                                            if (sp.Name.ToLower() == ru.Name.ToLower() && ru.Enabled)
                                            {
                                                exists = true;
                                            }

                                        }
                                    }
                                }


                                if (sub.Begin.Cookies != null && sub.Begin.Cookies.Count > 0)
                                {
                                    foreach (Cookie co in request.Cookies)
                                    {
                                        sw.WriteLine("\t\t\tweb_add_cookie(\"" + co.Name + "\");");
                                    }
                                }
                            }


                            else
                            {
                                if (sub.Begin.Parameters != null && sub.Begin.Parameters.Count > 0)
                                {
                                    sw.WriteLine("\t\t\tweb_submit_data(\"" + sub.Begin.Name + "\",");

                                    int begin = 0, cursor = 0;
                                    while (cursor < sub.Begin.Action.Length)
                                    {
                                        if (sub.Begin.Action[cursor] == '{')
                                        {
                                            begin = cursor;
                                        }
                                        else if (sub.Begin.Action[cursor] == '}' && sub.Begin.Action[cursor - 1] != '}')
                                        {
                                            string replace = sub.Begin.Action.Substring(begin + 1, cursor - begin - 1);
                                            if (replace.Contains('.'))
                                            {
                                                string aux = sub.Begin.Action.Substring(begin + 1, cursor - begin - 1).ToLower().Replace('.', '_');
                                                sub.Begin.Action = sub.Begin.Action.Replace(replace, aux);
                                            }
                                        }
                                        cursor++;
                                    }
                                    sw.WriteLine("\t\t\t\t\"Action=" + sub.Begin.Action + "\",");
                                    sw.WriteLine("\t\t\t\t\"Method=GET\",");
                                    sw.WriteLine("\t\t\t\t\"RecContentType=text/html\",");

                                    begin = 0;
                                    cursor = 0;

                                    while (cursor < sub.Begin.Referer.Length)
                                    {
                                        if (sub.Begin.Referer[cursor] == '{')
                                        {
                                            begin = cursor;
                                        }
                                        else if (sub.Begin.Referer[cursor] == '}' && sub.Begin.Referer[cursor - 1] != '}')
                                        {
                                            string replace = sub.Begin.Referer.Substring(begin + 1, cursor - begin - 1);
                                            if (replace.Contains('.'))
                                            {
                                                string aux = sub.Begin.Referer.Substring(begin + 1, cursor - begin - 1).ToLower().Replace('.', '_');
                                                sub.Begin.Referer = sub.Begin.Referer.Replace(replace, aux);
                                            }
                                        }
                                        cursor++;
                                    }

                                    sw.WriteLine("\t\t\t\t\"Referer=" + sub.Begin.Referer + "\",");
                                    sw.WriteLine("\t\t\t\t\"Mode=HTTP\",");
                                    if (sub.Begin.Body != "")
                                    {
                                        begin = 0;
                                        cursor = 0;

                                        while (cursor < sub.Begin.Body.Length)
                                        {
                                            if (sub.Begin.Body[cursor] == '{')
                                            {
                                                begin = cursor;
                                            }
                                            else if (sub.Begin.Body[cursor] == '}' && sub.Begin.Body[cursor - 1] != '}')
                                            {
                                                string replace = sub.Begin.Body.Substring(begin + 1, cursor - begin - 1);
                                                if (replace.Contains('.'))
                                                {
                                                    string aux = sub.Begin.Body.Substring(begin + 1, cursor - begin - 1).ToLower().Replace('.', '_');
                                                    sub.Begin.Body = sub.Begin.Body.Replace(replace, aux);
                                                }
                                            }
                                            cursor++;
                                        }
                                        sw.WriteLine("\t\t\t\t\"Body=" + sub.Begin.Body + "\",");
                                    }

                                    sw.WriteLine("\t\t\t\tITEMDATA,");

                                    foreach (Parameter param in request.Parameters)
                                    {
                                        begin = 0;
                                        cursor = 0;
                                        while (cursor < param.Name.Length)
                                        {
                                            if (param.Name[cursor] == '{')
                                            {
                                                begin = cursor;
                                            }
                                            else if (param.Name[cursor] == '}' && param.Name[cursor - 1] != '}')
                                            {
                                                string replace = param.Name.Substring(begin + 1, cursor - begin - 1);
                                                if (replace.Contains('.'))
                                                {
                                                    string aux = param.Name.Substring(begin + 1, cursor - begin - 1).ToLower().Replace('.', '_');
                                                    param.Name = param.Name.Replace(replace, aux);
                                                }
                                            }
                                            cursor++;
                                        }

                                        begin = 0;
                                        cursor = 0;
                                        while (cursor < param.Value.Length)
                                        {
                                            if (param.Value[cursor] == '{')
                                            {
                                                begin = cursor;
                                            }
                                            else if (param.Value[cursor] == '}' && param.Value[cursor - 1] != '}')
                                            {
                                                string replace = param.Value.Substring(begin + 1, cursor - begin - 1);
                                                if (replace.Contains('.'))
                                                {
                                                    string aux = param.Value.Substring(begin + 1, cursor - begin - 1).ToLower().Replace('.', '_');
                                                    param.Value = param.Value.Replace(replace, aux);
                                                }
                                            }
                                            cursor++;
                                        }
                                        sw.WriteLine("\t\t\t\t\"Name=" + param.Name + "\", \"Value=" + HttpUtility.UrlDecode(param.Value) + "\", ENDITEM,");

                                    }
                                    sw.WriteLine("\t\t\t\tLAST);");
                                    bool exists = false;

                                    if (sub.Begin.SaveParameters.Count > 0)
                                    {
                                        foreach (SaveParameter sp in sub.Begin.SaveParameters)
                                        {
                                            foreach (Rule ru in saveParam.Rules)
                                            {
                                                if (sp.Name.ToLower() == ru.Name.ToLower() && ru.Enabled)
                                                {

                                                    exists = true;
                                                }

                                            }
                                        }
                                    }

                                    if (sub.Begin.Cookies != null && sub.Begin.Cookies.Count > 0)
                                    {
                                        foreach (Cookie co in request.Cookies)
                                        {
                                            sw.WriteLine("\t\t\tweb_add_cookie(\"" + co.Name + "\");");
                                        }
                                    }
                                }
                                else
                                {
                                    sw.WriteLine("\t\t\tweb_custom_request(\"" + sub.Begin.Name + "\",");
                                    int begin = 0, cursor = 0;
                                    while (cursor < sub.Begin.Action.Length)
                                    {
                                        if (sub.Begin.Action[cursor] == '{')
                                        {
                                            begin = cursor;
                                        }
                                        else if (sub.Begin.Action[cursor] == '}' && sub.Begin.Action[cursor - 1] != '}')
                                        {
                                            string replace = sub.Begin.Action.Substring(begin + 1, cursor - begin - 1);
                                            if (replace.Contains('.'))
                                            {
                                                string aux = sub.Begin.Action.Substring(begin + 1, cursor - begin - 1).ToLower().Replace('.', '_');
                                                sub.Begin.Action = sub.Begin.Action.Replace(replace, aux);
                                            }
                                        }
                                        cursor++;
                                    }
                                    sw.WriteLine("\t\t\t\t\"URL=" + sub.Begin.Action + "\",");
                                    sw.WriteLine("\t\t\t\t\"Method=GET\",");
                                    sw.WriteLine("\t\t\t\t\"RecContentType=text/html\",");

                                    begin = 0;
                                    cursor = 0;

                                    while (cursor < sub.Begin.Referer.Length)
                                    {
                                        if (sub.Begin.Referer[cursor] == '{')
                                        {
                                            begin = cursor;
                                        }
                                        else if (sub.Begin.Referer[cursor] == '}' && sub.Begin.Referer[cursor - 1] != '}')
                                        {
                                            string replace = sub.Begin.Referer.Substring(begin + 1, cursor - begin - 1);
                                            if (replace.Contains('.'))
                                            {
                                                string aux = sub.Begin.Referer.Substring(begin + 1, cursor - begin - 1).ToLower().Replace('.', '_');
                                                sub.Begin.Referer = sub.Begin.Referer.Replace(replace, aux);
                                            }
                                        }
                                        cursor++;
                                    }

                                    sw.WriteLine("\t\t\t\t\"Referer=" + sub.Begin.Referer + "\",");
                                    sw.WriteLine("\t\t\t\t\"Mode=HTTP\",");
                                    if (sub.Begin.Body != "")
                                    {
                                        begin = 0;
                                        cursor = 0;

                                        while (cursor < sub.Begin.Body.Length)
                                        {
                                            if (sub.Begin.Body[cursor] == '{')
                                            {
                                                begin = cursor;
                                            }
                                            else if (sub.Begin.Body[cursor] == '}' && sub.Begin.Body[cursor - 1] != '}')
                                            {
                                                string replace = sub.Begin.Body.Substring(begin + 1, cursor - begin - 1);
                                                if (replace.Contains('.'))
                                                {
                                                    string aux = sub.Begin.Body.Substring(begin + 1, cursor - begin - 1).ToLower().Replace('.', '_');
                                                    sub.Begin.Body = sub.Begin.Body.Replace(replace, aux);
                                                }
                                            }
                                            cursor++;
                                        }
                                        sw.WriteLine("\t\t\t\t\"Body=" + sub.Begin.Body + "\",");
                                    }
                                    sw.WriteLine("LAST);");

                                    bool exists = false;

                                    if (sub.Begin.SaveParameters.Count > 0)
                                    {
                                        foreach (SaveParameter sp in sub.Begin.SaveParameters)
                                        {
                                            foreach (Rule ru in saveParam.Rules)
                                            {
                                                if (sp.Name.ToLower() == ru.Name.ToLower() && ru.Enabled)
                                                {
                                                    exists = true;
                                                }
                                            }
                                        }
                                    }

                                    if (sub.Begin.Cookies != null && sub.Begin.Cookies.Count > 0)
                                    {
                                        foreach (Cookie co in request.Cookies)
                                        {
                                            sw.WriteLine("\t\t\tweb_add_cookie(\"" + co.Name + "\");");
                                        }
                                    }
                                }
                            }
                        }
                        sw.WriteLine("\t\tlr_end_sub_transaction(\"" + sub.Name + "\", LR_AUTO);");
                        sw.WriteLine("\n");
                        foreach (SaveParameter sp in sub.Begin.SaveParameters)
                        {
                            foreach (Rule ru in saveParam.Rules)
                            {
                                if (sp.Name.ToLower().Trim() == ru.Name.ToLower().Trim() && ru.Enabled)
                                {
                                    sw.WriteLine("\t\t\t\t\tweb_reg_save_param(\"" + ru.Name.ToLower() + "\"");
                                    sw.WriteLine("\t\t\t\t,\"LB/IC=" + ru.LeftBoundary + "\",\"RB/IC=" + ru.RightBoundary + "\",\"Ord=" + ru.Order + "\",\"Search=Body\",LAST);");
                                }
                            }
                        }
                    }
                    sw.WriteLine("\tlr_end_transaction(\"" + transaction.Name + "\",LR_AUTO);");
                    sw.WriteLine("\n");
                }
                #endregion

                else
                {
                    //Begins with the first request on the transaction
                    Request request = transaction.Begin;

                    //int requestIndex = testCase.Requests.IndexOf(request) - 1;
                    int requestIndex = 0;

                    foreach (Request req in testCase.Requests)
                        if (req.Name == request.Name)
                            requestIndex = testCase.Requests.IndexOf(req);

                    #region transaction

                    do
                    {
                        requestIndex++;
                        //Updates to the next request                    
                        bool parallel = false;
                        request = testCase.Requests.ElementAt(requestIndex - 1);
                        transaction.Name = transaction.Name.Replace('.', '_');
                        if (request.Method == null || request.Action == null) break;

                        if (request.IsParallel == true && parallel == false)
                        {
                            sw.WriteLine("lr_start_transaction(\"" + transaction.Name + "\");");
                            sw.WriteLine("web_concurrent_start(NULL);");
                            parallel = true;
                        }
                        else if (request.IsParallel = false && parallel == true)
                        {
                            sw.WriteLine("web_concurrent_end(NULL);");
                            sw.WriteLine("lr_start_transaction(\"" + transaction.Name + "\");");
                            parallel = false;
                        }
                        else
                        {
                            sw.WriteLine("lr_start_transaction(\"" + transaction.Name + "\");");
                        }

                        if (request.Method.Equals("POST"))
                        {
                            if (request.Parameters != null && request.Parameters.Count > 0)
                            {
                                sw.WriteLine("web_submit_data(\"" + request.Name + "\",");
                                int begin = 0, cursor = 0;
                                while (cursor < request.Action.Length)
                                {
                                    if (request.Action[cursor] == '{')
                                    {
                                        begin = cursor;
                                    }
                                    else if (request.Action[cursor] == '}' && request.Action[cursor - 1] != '}')
                                    {
                                        string replace = request.Action.Substring(begin + 1, cursor - begin - 1);
                                        if (replace.Contains('.'))
                                        {
                                            string aux = request.Action.Substring(begin + 1, cursor - begin - 1).ToLower().Replace('.', '_');
                                            request.Action = request.Action.Replace(replace, aux);
                                        }
                                    }
                                    cursor++;
                                }
                                sw.WriteLine("\"Action=" + request.Action + "\",");
                                sw.WriteLine("\"Method=POST\",");
                                sw.WriteLine("\"RecContentType=text/html\",");

                                begin = 0;
                                cursor = 0;

                                while (cursor < request.Referer.Length)
                                {
                                    if (request.Referer[cursor] == '{')
                                    {
                                        begin = cursor;
                                    }
                                    else if (request.Referer[cursor] == '}' && request.Referer[cursor - 1] != '}')
                                    {
                                        string replace = request.Referer.Substring(begin + 1, cursor - begin - 1);
                                        if (replace.Contains('.'))
                                        {
                                            string aux = request.Referer.Substring(begin + 1, cursor - begin - 1).ToLower().Replace('.', '_');
                                            request.Referer = request.Referer.Replace(replace, aux);
                                        }
                                    }
                                    cursor++;
                                }

                                sw.WriteLine("\"Referer=" + request.Referer + "\",");
                                sw.WriteLine("\t\t\t\t\"Mode=HTTP\",");
                                if (request.Body != "")
                                {
                                    begin = 0;
                                    cursor = 0;

                                    while (cursor < request.Body.Length)
                                    {
                                        if (request.Body[cursor] == '{')
                                        {
                                            begin = cursor;
                                        }
                                        else if (request.Body[cursor] == '}' && request.Body[cursor - 1] != '}')
                                        {
                                            string replace = request.Body.Substring(begin + 1, cursor - begin - 1);
                                            if (replace.Contains('.'))
                                            {
                                                string aux = request.Body.Substring(begin + 1, cursor - begin - 1).ToLower().Replace('.', '_');
                                                request.Body = request.Body.Replace(replace, aux);
                                            }
                                        }
                                        cursor++;
                                    }
                                    sw.WriteLine("\"Body=" + request.Body + "\",");
                                }
                                //Writes the request parameters

                                sw.WriteLine("ITEMDATA,");

                                foreach (Parameter param in request.Parameters)
                                {
                                    begin = 0;
                                    cursor = 0;
                                    while (cursor < param.Name.Length)
                                    {
                                        if (param.Name[cursor] == '{')
                                        {
                                            begin = cursor;
                                        }
                                        else if (param.Name[cursor] == '}' && param.Name[cursor - 1] != '}')
                                        {
                                            string replace = param.Name.Substring(begin + 1, cursor - begin - 1);
                                            if (replace.Contains('.'))
                                            {
                                                string aux = param.Name.Substring(begin + 1, cursor - begin - 1).ToLower().Replace('.', '_');
                                                param.Name = param.Name.Replace(replace, aux);
                                            }
                                        }
                                        cursor++;
                                    }

                                    begin = 0;
                                    cursor = 0;
                                    while (cursor < param.Value.Length)
                                    {
                                        if (param.Value[cursor] == '{')
                                        {
                                            begin = cursor;
                                        }
                                        else if (param.Value[cursor] == '}' && param.Value[cursor - 1] != '}')
                                        {
                                            string replace = param.Value.Substring(begin + 1, cursor - begin - 1);
                                            if (replace.Contains('.'))
                                            {
                                                string aux = param.Value.Substring(begin + 1, cursor - begin - 1).ToLower().Replace('.', '_');
                                                param.Value = param.Value.Replace(replace, aux);
                                            }
                                        }
                                        cursor++;
                                    }
                                    sw.WriteLine("\"Name=" + param.Name + "\", \"Value=" + HttpUtility.UrlDecode(param.Value) + "\", ENDITEM,");

                                }
                                sw.WriteLine("LAST);");


                                bool exists = false;

                                if (request.SaveParameters.Count > 0)
                                {
                                    foreach (SaveParameter sp in request.SaveParameters)
                                    {
                                        foreach (Rule ru in saveParam.Rules)
                                        {
                                            if (sp.Name.ToLower() == ru.Name.ToLower() && ru.Enabled)
                                            {
                                                exists = true;
                                            }

                                        }
                                    }
                                }

                                if (request.Cookies != null && request.Cookies.Count > 0)
                                {
                                    foreach (Cookie co in request.Cookies)
                                    {
                                        sw.WriteLine("\tweb_add_cookie(\"" + co.Name + "\");");
                                    }
                                }
                            }

                            else
                            {
                                sw.WriteLine("web_custom_request(\"" + request.Name + "\",");
                                int begin = 0, cursor = 0;
                                while (cursor < request.Action.Length)
                                {
                                    if (request.Action[cursor] == '{')
                                    {
                                        begin = cursor;
                                    }
                                    else if (request.Action[cursor] == '}' && request.Action[cursor - 1] != '}')
                                    {
                                        string replace = request.Action.Substring(begin + 1, cursor - begin - 1);
                                        if (replace.Contains('.'))
                                        {
                                            string aux = request.Action.Substring(begin + 1, cursor - begin - 1).ToLower().Replace('.', '_');
                                            request.Action = request.Action.Replace(replace, aux);
                                        }
                                    }
                                    cursor++;
                                }
                                sw.WriteLine("\"URL=" + request.Action + "\",");
                                sw.WriteLine("\"Method=POST\",");
                                sw.WriteLine("\"RecContentType=text/html\",");
                                begin = 0;
                                cursor = 0;
                                while (cursor < request.Referer.Length)
                                {
                                    if (request.Referer[cursor] == '{')
                                    {
                                        begin = cursor;
                                    }
                                    else if (request.Referer[cursor] == '}' && request.Referer[cursor - 1] != '}')
                                    {
                                        string replace = request.Referer.Substring(begin + 1, cursor - begin - 1);
                                        if (replace.Contains('.'))
                                        {
                                            string aux = request.Referer.Substring(begin + 1, cursor - begin - 1).ToLower().Replace('.', '_');
                                            request.Referer = request.Referer.Replace(replace, aux);
                                        }
                                    }
                                    cursor++;
                                }

                                sw.WriteLine("\"Referer=" + request.Referer + "\",");
                                sw.WriteLine("\t\t\t\t\"Mode=HTTP\",");
                                if (request.Body != "")
                                {
                                    begin = 0;
                                    cursor = 0;

                                    while (cursor < request.Body.Length)
                                    {
                                        if (request.Body[cursor] == '{')
                                        {
                                            begin = cursor;
                                        }
                                        else if (request.Body[cursor] == '}' && request.Body[cursor - 1] != '}')
                                        {
                                            string replace = request.Body.Substring(begin + 1, cursor - begin - 1);
                                            if (replace.Contains('.'))
                                            {
                                                string aux = request.Body.Substring(begin + 1, cursor - begin - 1).ToLower().Replace('.', '_');
                                                request.Body = request.Body.Replace(replace, aux);
                                            }
                                        }
                                        cursor++;
                                    }

                                    sw.WriteLine("\"Body=" + request.Body + "\",");
                                }
                                sw.WriteLine("LAST);");

                                bool exists = false;

                                if (request.SaveParameters.Count > 0)
                                {
                                    foreach (SaveParameter sp in request.SaveParameters)
                                    {
                                        foreach (Rule ru in saveParam.Rules)
                                        {
                                            if (sp.Name.ToLower() == ru.Name.ToLower())
                                            {
                                                exists = true;
                                            }
                                        }
                                    }

                                }


                                if (request.Cookies != null && request.Cookies.Count > 0)
                                {
                                    foreach (Cookie co in request.Cookies)
                                    {
                                        sw.WriteLine("\tweb_add_cookie(\"" + co.Name + "\");");
                                    }
                                }

                            }
                        }

                        else
                        {
                            if (request.Parameters.Count == 0)
                            {
                                sw.WriteLine("web_url(\"" + request.Name + "\",");
                                int begin = 0, cursor = 0;
                                while (cursor < request.Action.Length)
                                {
                                    if (request.Action[cursor] == '{')
                                    {
                                        begin = cursor;
                                    }
                                    else if (request.Action[cursor] == '}' && request.Action[cursor - 1] != '}')
                                    {
                                        string replace = request.Action.Substring(begin + 1, cursor - begin - 1);
                                        if (replace.Contains('.'))
                                        {
                                            string aux = request.Action.Substring(begin + 1, cursor - begin - 1).ToLower().Replace('.', '_');
                                            request.Action = request.Action.Replace(replace, aux);
                                        }
                                    }
                                    cursor++;
                                }
                                sw.WriteLine("\"URL=" + request.Action + "\",");
                                sw.WriteLine("\"Resource=0\",");
                                sw.WriteLine("\"RecContentType=text/html\",");
                                begin = 0;
                                cursor = 0;
                                while (cursor < request.Referer.Length)
                                {
                                    if (request.Referer[cursor] == '{')
                                    {
                                        begin = cursor;
                                    }
                                    else if (request.Referer[cursor] == '}' && request.Referer[cursor - 1] != '}')
                                    {
                                        string replace = request.Referer.Substring(begin + 1, cursor - begin - 1);
                                        if (replace.Contains('.'))
                                        {
                                            string aux = request.Referer.Substring(begin + 1, cursor - begin - 1).ToLower().Replace('.', '_');
                                            request.Referer = request.Referer.Replace(replace, aux);
                                        }
                                    }
                                    cursor++;
                                }

                                sw.WriteLine("\"Referer=" + request.Referer + "\",");
                                sw.WriteLine("\t\t\t\t\"Mode=HTTP\",");
                                if (request.Body != "")
                                {
                                    begin = 0;
                                    cursor = 0;

                                    while (cursor < request.Body.Length)
                                    {
                                        if (request.Body[cursor] == '{')
                                        {
                                            begin = cursor;
                                        }
                                        else if (request.Body[cursor] == '}' && request.Body[cursor - 1] != '}')
                                        {
                                            string replace = request.Body.Substring(begin + 1, cursor - begin - 1);
                                            if (replace.Contains('.'))
                                            {
                                                string aux = request.Body.Substring(begin + 1, cursor - begin - 1).ToLower().Replace('.', '_');
                                                request.Body = request.Body.Replace(replace, aux);
                                            }
                                        }
                                        cursor++;
                                    }

                                    sw.WriteLine("\"Body=" + request.Body + "\",");
                                }
                                sw.WriteLine("LAST);");

                                bool exists = false;

                                if (request.SaveParameters.Count > 0)
                                {
                                    foreach (SaveParameter sp in request.SaveParameters)
                                    {
                                        foreach (Rule ru in saveParam.Rules)
                                        {
                                            if (sp.Name.ToLower() == ru.Name.ToLower() && ru.Enabled)
                                            {

                                                exists = true;
                                            }

                                        }
                                    }
                                }

                                if (request.Cookies != null && request.Cookies.Count > 0)
                                {
                                    foreach (Cookie co in request.Cookies)
                                    {
                                        sw.WriteLine("\tweb_add_cookie(\"" + co.Name + "\");");
                                    }
                                }
                            }
                            else
                            {
                                if (request.Parameters != null && request.Parameters.Count > 0)
                                {
                                    sw.WriteLine("web_submit_data(\"" + request.Name + "\",");
                                    int begin = 0, cursor = 0;
                                    while (cursor < request.Action.Length)
                                    {
                                        if (request.Action[cursor] == '{')
                                        {
                                            begin = cursor;
                                        }
                                        else if (request.Action[cursor] == '}' && request.Action[cursor - 1] != '}')
                                        {
                                            string replace = request.Action.Substring(begin + 1, cursor - begin - 1);
                                            if (replace.Contains('.'))
                                            {
                                                string aux = request.Action.Substring(begin + 1, cursor - begin - 1).ToLower().Replace('.', '_');
                                                request.Action = request.Action.Replace(replace, aux);
                                            }
                                        }
                                        cursor++;
                                    }
                                    sw.WriteLine("\"Action=" + request.Action + "\",");
                                    sw.WriteLine("\"Method=GET\",");
                                    sw.WriteLine("\"RecContentType=text/html\",");
                                    begin = 0;
                                    cursor = 0;
                                    while (cursor < request.Referer.Length)
                                    {
                                        if (request.Referer[cursor] == '{')
                                        {
                                            begin = cursor;
                                        }
                                        else if (request.Referer[cursor] == '}' && request.Referer[cursor - 1] != '}')
                                        {
                                            string replace = request.Referer.Substring(begin + 1, cursor - begin - 1);
                                            if (replace.Contains('.'))
                                            {
                                                string aux = request.Referer.Substring(begin + 1, cursor - begin - 1).ToLower().Replace('.', '_');
                                                request.Referer = request.Referer.Replace(replace, aux);
                                            }
                                        }
                                        cursor++;
                                    }

                                    sw.WriteLine("\"Referer=" + request.Referer + "\",");
                                    sw.WriteLine("\t\t\t\t\"Mode=HTTP\",");
                                    if (request.Body != "")
                                    {

                                        begin = 0;
                                        cursor = 0;

                                        while (cursor < request.Body.Length)
                                        {
                                            if (request.Body[cursor] == '{')
                                            {
                                                begin = cursor;
                                            }
                                            else if (request.Body[cursor] == '}' && request.Body[cursor - 1] != '}')
                                            {
                                                string replace = request.Body.Substring(begin + 1, cursor - begin - 1);
                                                if (replace.Contains('.'))
                                                {
                                                    string aux = request.Body.Substring(begin + 1, cursor - begin - 1).ToLower().Replace('.', '_');
                                                    request.Body = request.Body.Replace(replace, aux);
                                                }
                                            }
                                            cursor++;
                                        }

                                        sw.WriteLine("\"Body=" + request.Body + "\",");
                                    }

                                    sw.WriteLine("ITEMDATA,");

                                    foreach (Parameter param in request.Parameters)
                                    {
                                        begin = 0;
                                        cursor = 0;
                                        while (cursor < param.Name.Length)
                                        {
                                            if (param.Name[cursor] == '{')
                                            {
                                                begin = cursor;
                                            }
                                            else if (param.Name[cursor] == '}' && param.Name[cursor - 1] != '}')
                                            {
                                                string replace = param.Name.Substring(begin + 1, cursor - begin - 1);
                                                if (replace.Contains('.'))
                                                {
                                                    string aux = param.Name.Substring(begin + 1, cursor - begin - 1).ToLower().Replace('.', '_');
                                                    param.Name = param.Name.Replace(replace, aux);
                                                }
                                            }
                                            cursor++;
                                        }

                                        begin = 0;
                                        cursor = 0;
                                        while (cursor < param.Value.Length)
                                        {
                                            if (param.Value[cursor] == '{')
                                            {
                                                begin = cursor;
                                            }
                                            else if (param.Value[cursor] == '}' && param.Value[cursor - 1] != '}')
                                            {
                                                string replace = param.Value.Substring(begin + 1, cursor - begin - 1);
                                                if (replace.Contains('.'))
                                                {
                                                    string aux = param.Value.Substring(begin + 1, cursor - begin - 1).ToLower().Replace('.', '_');
                                                    param.Value = param.Value.Replace(replace, aux);
                                                }
                                            }
                                            cursor++;
                                        }
                                        sw.WriteLine("\"Name=" + param.Name + "\", \"Value=" + HttpUtility.UrlDecode(param.Value) + "\", ENDITEM,");
                                    }
                                    sw.WriteLine("LAST);");

                                    if (request.Cookies != null && request.Cookies.Count > 0)
                                    {
                                        foreach (Cookie co in request.Cookies)
                                        {
                                            sw.WriteLine("\tweb_add_cookie(\"" + co.Name + "\");");
                                        }
                                    }
                                }
                                else
                                {
                                    sw.WriteLine("web_custom_request(\"" + request.Name + "\",");
                                    int begin = 0, cursor = 0;
                                    while (cursor < request.Action.Length)
                                    {
                                        if (request.Action[cursor] == '{')
                                        {
                                            begin = cursor;
                                        }
                                        else if (request.Action[cursor] == '}' && request.Action[cursor - 1] != '}')
                                        {
                                            string replace = request.Action.Substring(begin + 1, cursor - begin - 1);
                                            if (replace.Contains('.'))
                                            {
                                                string aux = request.Action.Substring(begin + 1, cursor - begin - 1).ToLower().Replace('.', '_');
                                                request.Action = request.Action.Replace(replace, aux);
                                            }
                                        }
                                        cursor++;
                                    }
                                    sw.WriteLine("\"URL=" + request.Action + "\",");
                                    sw.WriteLine("\"Method=GET\",");
                                    sw.WriteLine("\"RecContentType=text/html\",");

                                    begin = 0;
                                    cursor = 0;
                                    while (cursor < request.Referer.Length)
                                    {
                                        if (request.Referer[cursor] == '{')
                                        {
                                            begin = cursor;
                                        }
                                        else if (request.Referer[cursor] == '}' && request.Referer[cursor - 1] != '}')
                                        {
                                            string replace = request.Referer.Substring(begin + 1, cursor - begin - 1);
                                            if (replace.Contains('.'))
                                            {
                                                string aux = request.Referer.Substring(begin + 1, cursor - begin - 1).ToLower().Replace('.', '_');
                                                request.Referer = request.Referer.Replace(replace, aux);
                                            }
                                        }
                                        cursor++;
                                    }

                                    sw.WriteLine("\"Referer=" + request.Referer + "\",");
                                    sw.WriteLine("\t\t\t\t\"Mode=HTTP\",");
                                    if (request.Body != "")
                                    {

                                        begin = 0;
                                        cursor = 0;

                                        while (cursor < request.Body.Length)
                                        {
                                            if (request.Body[cursor] == '{')
                                            {
                                                begin = cursor;
                                            }
                                            else if (request.Body[cursor] == '}' && request.Body[cursor - 1] != '}')
                                            {
                                                string replace = request.Body.Substring(begin + 1, cursor - begin - 1);
                                                if (replace.Contains('.'))
                                                {
                                                    string aux = request.Body.Substring(begin + 1, cursor - begin - 1).ToLower().Replace('.', '_');
                                                    request.Body = request.Body.Replace(replace, aux);
                                                }
                                            }
                                            cursor++;
                                        }

                                        sw.WriteLine("\"Body=" + request.Body + "\",");
                                    }
                                    sw.WriteLine("LAST);");


                                    bool exists = false;

                                    if (request.SaveParameters.Count > 0)
                                    {
                                        foreach (SaveParameter sp in request.SaveParameters)
                                        {
                                            foreach (Rule ru in saveParam.Rules)
                                            {
                                                if (sp.Name.ToLower() == ru.Name.ToLower() && ru.Enabled)
                                                {

                                                    exists = true;
                                                }

                                            }
                                        }

                                    }


                                    if (request.Cookies != null && request.Cookies.Count > 0)
                                    {
                                        foreach (Cookie co in request.Cookies)
                                        {
                                            sw.WriteLine("\tweb_add_cookie(\"" + co.Name + "\");");
                                        }
                                    }

                                }
                            }
                        }
                        if (request.ThinkTime != 0)
                            sw.WriteLine("\n");
                        sw.WriteLine("lr_end_transaction(\"" + transaction.Name + "\",LR_AUTO);");


                        sw.WriteLine("lr_think_time(" + request.ThinkTime + ");");

                        foreach (SaveParameter sp in request.SaveParameters)
                        {

                            foreach (Rule ru in saveParam.Rules)
                            {
                                if (sp.Name.ToLower() == ru.Name.ToLower() && ru.Enabled)
                                {
                                    sw.WriteLine("\t\t\t\t\tweb_reg_save_param(\"" + ru.Name.ToLower() + "\"");
                                    sw.WriteLine("\t\t\t\t,\"LB/IC=" + ru.LeftBoundary + "\",\"RB/IC=" + ru.RightBoundary + "\",\"Ord=" + ru.Order + "\",\"Search=Body\",LAST);");
                                }
                            }
                        }

                    }
                    while (request.Name != transaction.End.Name);
                    #endregion
                }
            }
            sw.WriteLine("\tlr_end_transaction(\"" + testCase.Name + "\",LR_AUTO);");
            sw.WriteLine("\n\treturn 0;");
            sw.WriteLine("}");

            sw.Close();
        }
    }
}