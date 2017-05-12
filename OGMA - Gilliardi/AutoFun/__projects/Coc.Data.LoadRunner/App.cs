//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.IO;
//using Coc.Testing.Performance.AbstractTestCases;
//using Coc.Data.LoadRunner.ParameterEditor;

//namespace Coc.Data.LoadRunner
//{
//    /// <summary>
//    /// Executa um teste específico
//    /// </summary>
//    public static class App
//    {

//        public static void Execute(Scenario scenario, string destinationPath, string scenarioName, ParameterEditorWindow parametersScenarios)
//        {
//            foreach (TestCase script in scenario.TestCases)

//            //?? 
//            script.Name = script.Name.ToLower()
//                .Replace('ç', 'c')
//                .Replace('ã', 'a')
//                .Replace('á', 'a')
//                .Replace('à', 'a')
//                .Replace('â', 'a')
//                .Replace('è', 'e')
//                .Replace('é', 'e')
//                .Replace('ê', 'e')
//                .Replace('ì', 'i')
//                .Replace('í', 'i')
//                .Replace('î', 'i')
//                .Replace('õ', 'o')
//                .Replace('ó', 'o')
//                .Replace('ò', 'o')
//                .Replace('ô', 'o');

//            List<String> existsName = new List<string>();
//            //Generates the Action.c files of the Scripts (Test Cases) from the given Scenario
//            foreach (TestCase script in scenario.TestCases)
//            {
//                string scriptName = returnValidName(script.Name.Replace(" ", ""), existsName, 0);
//                ScriptLR.GenerateScript(script, destinationPath + "\\" + scriptName, scriptName, parametersScenarios.saveParameters);
//                existsName.Add(scriptName);
//            }

//            existsName.Clear();
//            foreach (ImageTreeViewItem scenarioPrm in parametersScenarios.ParameterScenarios.OfType<ImageTreeViewItem>())
//                foreach (ImageTreeViewItem script in scenarioPrm.Items.OfType<ImageTreeViewItem>())
//                {
//                    string scriptName = returnValidName(script.Text, existsName, 0);
//                    using (TextWriter writerPrm = new StreamWriter(destinationPath + "\\" + scriptName.ToLower() + "\\" + scriptName.ToLower() + ".prm"))
//                        foreach (ImageTreeViewItem param in script.Items.OfType<ImageTreeViewItem>())
//                            parametersScenarios.writeAsText(writerPrm, param.Data, param.Text);
//                }
//            ScenarioLR.GenerateScenario(scenario, scenario.Name, destinationPath);

//            Directory.CreateDirectory(destinationPath + "\\WSDL");


//        }

//        private static string returnValidName(string p, List<String> names, int count)
//        {
//            if(names.Contains(p))
//            {
//                if (!names.Contains(p + count))
//                {
//                    p = p + count;
//                }
//                else
//                {
//                    count = count + 1;
//                    p = returnValidName(p, names, count);
//                    return p;
//                }
//            }
//            return p;
//        }
//    }
//}
