using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using Coc.Testing.Performance.AbstractTestCases;

namespace Coc.Data.LoadRunner

{
    //Class responsible for generating a LoadRunner Scenario
    public class ScenarioLR {
        /// <summary>
        /// Generates a LoadRunner scenario (.lrs file) from a Scenario object.
        /// </summary>
        /// <param name="scenario">Scenario object from the Sequence Model</param>
        public static void GenerateScenario(Scenario scenario, string scenarioName, string destinationPath)
        {
            
            //StringBuilder used to rewrite the template files into the scenario file
            StringBuilder newFile = new StringBuilder();
            Assembly asm = Assembly.GetExecutingAssembly();
            TextReader reader;
            reader = new StreamReader(asm.GetManifestResourceStream("PerformanceTool.Templates.scenarioTemplate.lrs"));
            string[] file = reader.ReadToEnd().Split('\n');
            reader.Close();

            //string location = Path.GetDirectoryName(asm.Location) + "\\" +  destinationPath;
            string location = destinationPath;
            //Temporary string to replace the tags in the template files
            string temp;

            //Generates the Scenario file
            foreach (string line in file) {
                temp = line;
                temp = temp.Replace("<<ScenarioPath>>", location + "\\" + scenarioName + ".lrs");
                temp = temp.Replace("<<Subject>>", scenarioName);
                temp = temp.Replace("<<GroupsNum>>", scenario.TestCases.Count.ToString());
                temp = temp.Replace("<<Vusers>>", scenario.Population.ToString());
                temp = temp.Replace("<<Scripts>>", scenario.TestCases.Count.ToString());
                temp = temp.Replace("<<ResultFilePath>>", location + "\\results\\" + scenarioName + ".lrr");
                temp = temp.Replace("<<HostGenerator>>", "localhost"); //not found?
                temp = temp.Replace("<<RampUp_Users>>", scenario.RampUpUser.ToString());
                temp = temp.Replace("<<RampUp_Time>>", scenario.RampUpTime.ToString());
                temp = temp.Replace("<<RampDown_Users>>", scenario.RampDownUser.ToString());
                temp = temp.Replace("<<RampDown_Time>>", scenario.RampDownTime.ToString());

                #region testChief
                //Part of the file where there's the scripts information
                if (temp.Contains("<<TestChief>>")) {
                    StringBuilder testChief = new StringBuilder();
                    
                    //Foreach script in the scenario
                    List<String> existsNames = new List<String>();
                    foreach (TestCase script in scenario.TestCases) {

                        string scriptName = returnValidName(script.Name.Replace(" ", ""), existsNames, 0);
                        existsNames.Add(scriptName);
                        string scriptPath = location + "\\" + scriptName + "\\" + scriptName + ".usr";
                        int scriptUsers = (int)(script.Probability * scenario.Population);

                        testChief.Append(
                            "\n{" + scriptName + "\n" +
                            "UiName=" + scriptName + "\n" +
                            "Type=1\n" +
                            "SubType=Multi+QTWeb\n" +
                            "Path=" + scriptPath + "\n" + //"Path=C:\Users\Administrador\Desktop\gerenciarhabilidades6\gerenciarhabilidades6.usr\n" +
                            "Config=[WEB]\\r\\nStartRecordingIsDst=\"0\"\\r\\nScreenAvailWidth=\"1152\"\\r\\nNavigatorBrowserLanguage=\"en-us\"\\r\\nUTF8InputOutput=\"0\"\\r\\nNavigatorUserLanguage=\"en-us\"\\r\\nRecorderWinCodePage=\"1252\"\\r\\nUseCustomAgent=\"1\"\\r\\nProxyUseBrowser=\"0\"\\r\\nProxyAutoConfigScriptURL=\"\"\\r\\nProxyPassword=\"\"\\r\\nSaveSnapshotResources=0\\r\\nSearchForImages=\"1\"\\r\\nProxyUseProxyServer=\"0\"\\r\\nHttpVer=\"1.1\"\\r\\nProxyUseProxy=\"0\"\\r\\nAnalogMode=\"0\"\\r\\nResetContext=\"True\"\\r\\nStartRecordingGMT=\"2010/07/27 14:00:00\"\\r\\nProxyUseAutoConfigScript=\"0\"\\r\\nProxyUseSame=\"1\"\\r\\nUserHomePage=\"res" + "://shdoclc.dll/hardAdmin.htm\\\"\\r\\nScreenWidth=\"1152\"\\r\\nScreenAvailHeight=\"834\"\\r\\nBrowserAcceptLanguage=\"en-us\"\\r\\nProxyHTTPSHost=\"\"\\r\\nBrowserType=\"Microsoft Internet Explorer 4.0\"\\r\\nProxyNoLocal=\"0\"\\r\\nProxyHTTPHost=\"\"\\r\\nNavigatorSystemLanguage=\"en-us\"\\r\\nEnableChecks=\"0\"\\r\\nProxyHTTPPort=\"443\"\\r\\nWebRecorderVersion=\"10\"\\r\\nBrowserAcceptEncoding=\"gzip, deflate\"\\r\\nProxyBypass=\"\"\\r\\nKeepAlive=\"Yes\"\\r\\nProxyUserName=\"\"\\r\\nProxyHTTPSPort=\"443\"\\r\\nScreenHeight=\"864\"\\r\\nCustomUserAgent=\"Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729)\"\\r\\n" +
                            "[Log]\\r\\nMsgClassParameters=\"0\"\\r\\nMsgClassData=\"0\"\\r\\nLogOptions=\"LogBrief\"\\r\\nMsgClassFull=\"0\"\\r\\nAutoLog=1\\r\\n"+
                            "[General]\\r\\"+
                            "nXlBridgeTimeout=\"120\"\\r\\nDefaultRunLogic=\"uspnED.483\"\\r\\nautomatic_nested_transactions=\"1\"\\r\\nAutomaticTransactions=\"1\"\\r\\n[Iterations]\\r\\nRandomMax=\"90\"\\r\\nRandomMin=\"60\"\\r\\nIterationPace=\"IterationASAP\"\\r\\nStartEvery=\"60\"\\r\\nNumOfIterations=\"1\"\\r\\n"+
                            "[ThinkTime]\\r\\"+
                            "\\r\\nOptions=RECORDED\\r\\nFactor=\"1\"\\r\\nLimit=\"1\"\\r\\nLimitFlag=\"0\"\\r\\n\n" +
                            "ConfigUsp=[RunLogicEndRoot]\\r\\nName=\"End\"\\r\\nMercIniTreeSectionName=\"RunLogicEndRoot\"\\r\\nRunLogicNumOfIterations=\"1\"\\r\\nRunLogicObjectKind=\"Group\"\\r\\nRunLogicActionType=\"VuserEnd\"\\r\\nMercIniTreeFather=\"\"\\r\\nRunLogicRunMode=\"Sequential\"\\r\\nRunLogicActionOrder=\"vuser_end\"\\r\\nMercIniTreeSons=\"vuser_end\"\\r\\n[RunLogicInitRoot:vuser_init]\\r\\nName=\"vuser_init\"\\r\\nMercIniTreeSectionName=\"vuser_init\"\\r\\nRunLogicObjectKind=\"Action\"\\r\\nRunLogicActionType=\"VuserInit\"\\r\\nMercIniTreeFather=\"RunLogicInitRoot\"\\r\\n[RunLogicEndRoot:vuser_end]\\r\\nName=\"vuser_end\"\\r\\nMercIniTreeSectionName=\"vuser_end\"\\r\\nRunLogicObjectKind=\"Action\"\\r\\nRunLogicActionType=\"VuserEnd\"\\r\\nMercIniTreeFather=\"RunLogicEndRoot\"\\r\\n[RunLogicRunRoot:Action]\\r\\nName=\"Action\"\\r\\nMercIniTreeSectionName=\"Action\"\\r\\nRunLogicObjectKind=\"Action\"\\r\\nRunLogicActionType=\"VuserRun\"\\r\\nMercIniTreeFather=\"RunLogicRunRoot\"\\r\\n[RunLogicRunRoot]\\r\\nName=\"Run\"\\r\\nMercIniTreeSectionName=\"RunLogicRunRoot\"\\r\\nRunLogicNumOfIterations=\"1\"\\r\\nRunLogicObjectKind=\"Group\"\\r\\nRunLogicActionType=\"VuserRun\"\\r\\nMercIniTreeFather=\"\"\\r\\nRunLogicRunMode=\"Sequential\"\\r\\nRunLogicActionOrder=\"Action\"\\r\\nMercIniTreeSons=\"Action\"\\r\\n[RunLogicInitRoot]\\r\\nName=\"Init\"\\r\\nMercIniTreeSectionName=\"RunLogicInitRoot\"\\r\\nRunLogicNumOfIterations=\"1\"\\r\\nRunLogicObjectKind=\"Group\"\\r\\nRunLogicActionType=\"VuserInit\"\\r\\nMercIniTreeFather=\"\"\\r\\nRunLogicRunMode=\"Sequential\"\\r\\nRunLogicActionOrder=\"vuser_init\"\\r\\nMercIniTreeSons=\"vuser_init\"\\r\\n[Profile Actions]\\r\\nProfile Actions name=vuser_init,Action,vuser_end\\r\\nMercIniTreeSectionName=Profile Actions\\r\\n[RunLogicErrorHandlerRoot]\\r\\nMercIniTreeSectionName=\"RunLogicErrorHandlerRoot\"\\r\\nRunLogicNumOfIterations=\"1\"\\r\\nRunLogicActionOrder=\"vuser_errorhandler\"\\r\\nRunLogicObjectKind=\"Group\"\\r\\nName=\"ErrorHandler\"\\r\\nRunLogicRunMode=\"Sequential\"\\r\\nRunLogicActionType=\"VuserErrorHandler\"\\r\\nMercIniTreeSons=\"vuser_errorhandler\"\\r\\nMercIniTreeFather=\"\"\\r\\n[RunLogicErrorHandlerRoot:vuser_errorhandler]\\r\\nMercIniTreeSectionName=\"vuser_errorhandler\"\\r\\nRunLogicObjectKind=\"Action\"\\r\\nName=\"vuser_errorhandler\"\\r\\nRunLogicActionType=\"VuserErrorHandler\"\\r\\nMercIniTreeFather=\"RunLogicErrorHandlerRoot\"\\r\\n\\r\\n\n" +
                            "Param=\n" +
                            "TDPath=\n" +
                            "TDServer=\n" +
                            "TDDatabase=\n" +
                            "Platform=All\n" +
                            "AstraSubType=0\n" +
                            "}");
                    }
                    temp = temp.Replace("<<TestChief>>", testChief.ToString());
                }
                #endregion
                #region groupChief
                if (temp.Contains("<<GroupChief>>")) {
                    StringBuilder groupChief = new StringBuilder();


                    List<String> existsNames = new List<String>();
                    //For each script in the scenario
                    foreach (TestCase script in scenario.TestCases) {


                        string scriptName = returnValidName(script.Name.Replace(" ", ""), existsNames, 0);
                        existsNames.Add(scriptName);
                        string scriptPath = location + "\\" + scriptName + "\\" + scriptName + ".usr";
                        int scriptUsers = (int)(script.Probability * (scenario.Population / scenario.TestCases.Count()));

                        int mod = scenario.Population % scenario.TestCases.Count();
                        if (mod != 0)
                        {
                            int index = scenario.TestCases.IndexOf(script);

                            if (index < mod)
                            {
                                scriptUsers = scriptUsers + 1;
                            }
                        }
                       

                        groupChief.Append("\n{" + scriptName);

                        //For each user in the script
                        //for (int user = 1; user <= (totalUsers * script.Probability); user++) {
                        for (int user = 1; user <= scriptUsers; user++) {
                            groupChief.Append("\n{" + user +
                                "\n5=" + scriptName +
                                "\n9=" + "localhost" +
                                "\nSEED_NUM=0" +
                                "\n}\n\n");
                        }

                        groupChief.Append("\n{ChiefSettings" +
                            "\n5=" + scriptName +
                            "\n9=" + "localhost" +
                            "\nGroupParam=" +
                            "\nEnabled=1" +
                            "\n}\n");
                        
                        groupChief.Append("}"); //closes groupA
                    }
                    temp = temp.Replace("<<GroupChief>>", groupChief.ToString());
                }

                #endregion


                if (temp.Contains("<<GroupScheduler>>")) {

                    StringBuilder groupInfo = new StringBuilder();

                    List<String> existsNames = new List<String>();
                    //Writes the group info for each script in the scenario
                    foreach (TestCase script in scenario.TestCases) {
                        
                        int scriptUsers = (int)(script.Probability * scenario.Population);
                        string scriptName = returnValidName(script.Name.Replace(" ", ""), existsNames, 0);
                        existsNames.Add(scriptName);
                        string scriptPath = location + "\\" + scriptName + "\\" + scriptName + ".usr";

                        groupInfo.Append("\n          <GroupScheduler>\n            " +
                        "<GroupName>" + scriptName + "</GroupName>\n            " +
                        "<StartupMode>\n              <StartAtScenarioBegining />\n            </StartupMode>\n            " +
                        "<Scheduling>\n              <IsDefaultScheduler>true</IsDefaultScheduler>\n              " +
                        "<DynamicScheduling>\n                <RampUp>\n                  <StartCondition>\n                    <PrevAction />" +
                        "\n                  </StartCondition>\n                  <Batch>\n                    <Count>" + scenario.RampUpUser +
                        "</Count>\n                    <Interval>" + scenario.RampUpTime + "</Interval>\n                  " +
                        "</Batch>\n                  <TotalVusersNumber>" + scriptUsers +
                        "</TotalVusersNumber>\n                </RampUp>\n                <Duration>\n                  <StartCondition>" +
                        "\n                    <PrevAction />\n                  </StartCondition>\n                  <RunFor>300</RunFor>\n                </Duration>" +
                        "\n                <RampDownAll>\n                  <StartCondition>\n                    <PrevAction />\n                  </StartCondition>" +
                        "\n                  <Batch>\n                    <Count>" + scenario.RampDownUser + "</Count>\n                    <Interval>" + scenario.RampDownTime + "</Interval>\n                  </Batch>" +
                        "\n                </RampDownAll>\n              </DynamicScheduling>\n            </Scheduling>\n          </GroupScheduler>");
                      
                    }
                    temp = temp.Replace("<<GroupScheduler>>", groupInfo.ToString());                  
                }
                newFile.Append(temp + "\n");
            }
            File.WriteAllText(destinationPath + "\\" + scenarioName + ".lrs", newFile.ToString(), Encoding.UTF8);
        }

        private static string returnValidName(string p, List<String> names, int count)
        {
            if (names.Contains(p))
            {
                if (!names.Contains(p + count))
                {
                    p = p + count;
                }
                else
                {
                    count = count + 1;
                    p = returnValidName(p, names, count);
                    return p;
                }
            }
            return p;
        }
    }
}
