using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coc.Modeling.Uml;
using System.IO;
using Coc.Data.Interfaces;
using Coc.Data.ControlAndConversionStructures;
using Coc.Data.ControlAndConversionStructures;

namespace Coc.Data.ReadLR
{
    public class LoadRunnerToXMI : Parser
    {
        public override StructureCollection ParserMethod(String path, ref String name, Tuple<String, Object>[] args)
        {
            return null;
        }

        public UmlModel Converter(String path, String prm)
        {
            #region Variables
            List<String> functions = new List<string>();
            UmlModel model = new UmlModel("55");
            String line = "";
            //String test = "";
            String auxAux = ""; ;
            String auxMethod = "";
            String auxReferer = "";
            String auxBody = "";
            String parameters = "";
            String auxCookies = "";
            String auxCookies2 = "";
            String auxSaveParam = "";
            String auxSaveParam2 = "";
            String auxThinkTime = "";
            String auxAction = "";
            String name = "";
            String tempoBody = "";
            //int w = 0;
            bool trans = false;
            bool entro = false;
            bool bloco = false;
            bool existeLane = false;
            bool firstLine = false;
            bool putInitial = false;
            bool blocoItemData = false;
            bool createdLane = false;
            bool subbloco = false;
            bool existsThinkTime = false;
            bool blocoExtares = false;
            bool comentado = false;
            bool blocoBody = false;
            UmlTransition transition = null;
            UmlInitialState initial = new UmlInitialState();
            UmlFinalState final = new UmlFinalState();
            UmlElement antActionState = new UmlActionState();
            UmlActionState actionState = null;
            UmlActionState subActionState = null;
            UmlLane lane = null;
            UmlTransition antTransition = new UmlTransition();
            UmlFork fork = null;
            UmlDecision decision = null;
            initial.Name = "Initial0";
            final.Name = "Final0";
            UmlActivityDiagram activityDiagram = new UmlActivityDiagram("ActivityDiagram");
            activityDiagram.UmlObjects.Add(initial);
            List<String> namesParameter = new List<String>();
            #endregion
            String[] lines = File.ReadAllLines(path);
            String[] prmFile = File.ReadAllLines(prm);
            #region LoadPrmFile
            Dictionary<String, String> dicPrm = new Dictionary<String, String>();
            String prmName = "";
            String columnName = "";

            foreach (String linha in prmFile)
            {
                if (linha.Contains("[parameter:"))
                {
                    prmName = linha.Substring(11);
                    prmName = prmName.Substring(0, prmName.Length - 1);
                    columnName = String.Empty;
                }

                if (linha.Contains("ColumnName="))
                {
                    columnName = linha.Substring(12);
                    columnName = columnName.Substring(0, columnName.Length - 1);
                    dicPrm.Add(prmName, columnName);
                }
            }

            foreach (String linha in lines)
            {
                if (linha.Trim().StartsWith("//"))
                {
                    continue;
                }


                if (linha.Contains("web_reg_save_param("))
                {
                    String parameter = linha.Replace("\t", "").Trim().Substring(20);
                    parameter = parameter.Substring(0, parameter.Length - 2);
                    namesParameter.Add(parameter);
                }
            }
            #endregion
            for (int i = 0; i < lines.Length; i++)
            {
                line = lines[i].Replace("\t", "").Trim();
                if (line.Trim().StartsWith("//"))
                {
                    continue;
                }

                if (line.Trim().StartsWith("/*"))
                {
                    continue;
                }

                //if (line.Trim().EndsWith("*/") && comentado)
                //{
                //    comentado = false;
                //    continue;
                //}
                //else if (comentado)
                //{
                //    continue;
                //}

                #region ThinkTime
                if (line.Contains("lr_think_time("))
                {
                    auxThinkTime = line.Replace("\t", "").Substring(14).Trim();
                    auxThinkTime = auxThinkTime.Substring(0, auxThinkTime.Length - 2);
                    existsThinkTime = true;
                }
                #endregion

                if (!firstLine && line.EndsWith("()"))
                {
                    activityDiagram.Name = line.Replace("\t", "").Trim();
                    activityDiagram.Name = activityDiagram.Name.Substring(0, activityDiagram.Name.Length - 2);
                    firstLine = true;
                    name = activityDiagram.Name;
                    continue;
                }

                if (line.Contains("lr_start_transaction(\""))
                {
                    lane = new UmlLane();
                    line = line.Replace("\t", "").Trim();
                    line = line.Substring(22);
                    line = line.Substring(0, line.Length - 3);
                    lane.Name = line;
                    existeLane = true;
                    createdLane = true;
                    trans = true;
                    continue;
                }

                if (trans)
                {
                    #region SaveParam
                    if (line.Contains("web_reg_save_param("))
                    {
                        auxSaveParam = line.Replace("\t", "").Trim().Substring(20);
                        auxSaveParam = auxSaveParam.Substring(0, auxSaveParam.Length - 2);
                        auxSaveParam = auxSaveParam + "|";
                        auxSaveParam2 += auxSaveParam;
                        if (!lines[i + 1].Contains("web_reg_save_param("))
                        {
                            //SÓ DEVE OCORRER NO FINAL
                            // auxSaveParam2 += auxSaveParam2.Substring(0, auxSaveParam2.Length - 1);
                            auxSaveParam2 = ParseParameter(auxSaveParam2, dicPrm, namesParameter);
                            transition.SetTaggedValue("TDsaveParam", auxSaveParam2);
                        }
                    }
                    #endregion
                }

                if (line.Contains("web_custom_request(\"") || line.Contains("web_url(\"") || line.Contains("web_submit_data(\""))
                {
                    actionState = new UmlActionState();
                    subActionState = new UmlActionState();
                    transition = new UmlTransition();
                    trans = false;

                    if (line.Contains("web_custom_request(\""))
                    {
                        line = line.Substring(20);
                        line = line.Substring(0, line.Length - 2);
                        //bloco = true;
                        if (line.Contains("{Server}"))
                        {
                            continue;
                        }
                    }
                    else if (line.Contains("web_url(\""))
                    {
                        line = line.Substring(9);
                        line = line.Substring(0, line.Length - 2);
                        //bloco = true;
                        if (line.Contains("{Server}"))
                        {
                            continue;
                        }
                    }
                    else if (line.Contains("web_submit_data(\""))
                    {
                        line = line.Substring(17);
                        line = line.Substring(0, line.Length - 2);
                        //bloco = true;
                        if (line.Contains("{Server}"))
                        {
                            continue;
                        }
                    }

                    if (createdLane)
                    {
                        createdLane = false;
                        activityDiagram.Lanes.Add(lane);
                    }

                    if (lane != null)
                    {
                        lane.AddElement(actionState);
                    }
                    else if (lane == null)
                    {
                        lane = new UmlLane();
                        lane.Name = "";
                        lane.AddElement(actionState);
                        activityDiagram.Lanes.Add(lane);
                    }

                    if (!putInitial)
                    {
                        transition.Source = initial;
                        transition.Target = actionState;
                        antActionState = transition.Target;
                        putInitial = true;
                    }
                    else
                    {
                        subActionState = actionState;
                        transition.Source = antActionState;
                        transition.Target = subActionState;
                        antActionState = transition.Target;
                    }

                    bloco = true;
                    actionState.Name = line;

                    if (line.Contains("lr_end_transaction(\""))
                    {
                        existeLane = false;
                    }
                    activityDiagram.UmlObjects.Add(transition);
                    activityDiagram.UmlObjects.Add(actionState);
                }

                if (bloco)
                {
                    if (line.Contains("EXTRARES"))
                    {
                        blocoExtares = true;
                    }

                    if (blocoExtares && line.Contains("LAST);"))
                    {
                        blocoExtares = false;
                    }
                    else if (blocoExtares)
                    {
                        continue;
                    }

                    #region URL
                    if (line.Contains("URL="))
                    {
                        auxAux = line.Replace("\t", "").Trim().Substring(5); ;
                        auxAux = auxAux.Substring(0, auxAux.Length - 2);

                        if (line.Contains("_"))
                        {
                            auxAux = auxAux.Replace("_", ".");
                        }

                        if (line.Contains("{Server}"))
                        {
                            auxAux = auxAux.Replace("{Server}", "{Server.Server}");
                        }

                        auxAux = ParseParameter(auxAux, dicPrm, namesParameter);
                        transition.SetTaggedValue("TDaction", auxAux);
                    }
                    #endregion
                    #region Action
                    else if (line.Contains("Action="))
                    {
                        auxAction = line.Replace("\t", "").Trim().Substring(8);
                        auxAction = auxAction.Substring(0, auxAction.Length - 2);
                        transition.SetTaggedValue("TDaction", auxAction);
                    }
                    #endregion
                    #region Method
                    else if (line.Contains("Method="))
                    {
                        auxMethod = line.Replace("\t", "").Trim().Substring(8);
                        auxMethod = auxMethod.Substring(0, auxMethod.Length - 2);
                        transition.SetTaggedValue("TDmethod", auxMethod);
                    }
                    #endregion
                    #region Body
                    if (line.Contains("Body="))
                    {
                        auxBody = line.Replace("\t", "").Trim().Substring(6);
                        auxBody = auxBody.Substring(0, auxBody.Length - 1);
                        auxBody = ParseParameter(auxBody, dicPrm, namesParameter);
                        tempoBody = auxBody;
                        blocoBody = true;
                        continue;
                    }

                    if (blocoBody && line.Contains("LAST);"))
                    {
                        //auxBody = auxBody.Substring(0, auxBody.Length - 1);
                        tempoBody = tempoBody.Substring(0, tempoBody.Length - 1);
                        transition.SetTaggedValue("TDbody", tempoBody);
                        tempoBody = "";
                        blocoBody = false;
                    }

                    if (blocoBody)
                    {
                        auxBody = line.Replace("\t", "").Trim().Substring(1);
                        auxBody = auxBody.Substring(0, auxBody.Length - 1);
                        auxBody = ParseParameter(auxBody, dicPrm, namesParameter);
                        tempoBody += auxBody;
                        continue;
                    }
                    #endregion
                    #region Referer
                    else if (line.Contains("Referer="))
                    {
                        auxReferer = line.Replace("\t", "").Trim().Substring(9);
                        auxReferer = auxReferer.Substring(0, auxReferer.Length - 2);

                        if (line.Contains("_"))
                        {
                            auxReferer = auxReferer.Replace("_", ".");
                        }

                        if (line.Contains("{Server}"))
                        {
                            auxReferer = auxReferer.Replace("{Server}", "{Server.Server}");
                        }

                        auxReferer = ParseParameter(auxReferer, dicPrm, namesParameter);

                        if (auxReferer != "")
                        {
                            transition.SetTaggedValue("TDreferer", auxReferer);
                        }
                    }
                    #endregion
                    #region itemData
                    else if (line.Contains("ITEMDATA"))
                    {
                        blocoItemData = true;
                        continue;
                    }
                    else if (line.Contains("LAST);") && blocoItemData)
                    {
                        transition.SetTaggedValue("TDparameters", parameters);
                        blocoItemData = false;
                        parameters = parameters.Substring(0, parameters.Length - 1);
                        continue;
                    }

                    if (blocoItemData)
                    {
                        parameters = ItemData(line, parameters);
                    }
                    #endregion
                    #region ThinkTime
                    else if (existsThinkTime)
                    {
                        transition.SetTaggedValue("TDthinkTime", auxThinkTime);
                        existsThinkTime = false;
                    }
                    #endregion
                    #region Cookie
                    else if (line.Contains("web_add_cookie("))
                    {
                        auxCookies = line.Replace("\t", "").Trim().Substring(16);
                        auxCookies = auxCookies.Substring(0, auxCookies.Length - 3);
                        auxCookies = auxCookies + "|";
                        auxCookies2 += auxCookies;
                        
                        if (!lines[i + 1].Contains("web_add_cookie("))
                        {
                            //SÓ DEVE OCORRER NO FINAL
                            auxCookies2 = auxCookies2.Substring(0, auxCookies2.Length - 1);
                            transition.SetTaggedValue("TDcookies", auxCookies2);
                        }
                    }
                    #endregion

                    if (line.Contains("LAST);"))
                    {
                        bloco = false;
                    }
                }
            }
            transition = new UmlTransition();
            transition.Source = antActionState;
            transition.Target = final;
            activityDiagram.UmlObjects.Add(transition);
            activityDiagram.UmlObjects.Add(final);
            model.AddDiagram(activityDiagram);
            
            return model;
        }

        private String ParseParameter(String par, Dictionary<String, String> dicPrm, List<String> saveParNames)
        {
            int begin = 0; int cursor = 0;

            while (cursor < par.Length)
            {
                if (par[cursor] == '{')
                {
                    begin = cursor;
                }
                else if (par[cursor] == '}' && par[cursor - 1] != '}')
                {
                    //Set end point
                    int end1 = cursor - begin - 1;
                    //Get substring
                    string replace = par.Substring(begin + 1, cursor - begin - 1);
                    //ToLower magic
                    string aux = par.Substring(begin + 1, cursor - begin - 1);

                    if (!saveParNames.Contains(aux))
                    {
                        if (dicPrm.Keys.Contains(aux))
                        {
                            aux += "." + dicPrm[aux];
                        }
                    }

                    //Replace only the same text
                    var aStringBuilder = new StringBuilder(par);
                    //Remove old string
                    aStringBuilder.Remove(begin + 1, end1);
                    //Replace for new string
                    aStringBuilder.Insert(begin + 1, aux);
                    //Set string 
                    par = aStringBuilder.ToString();


                }
                cursor++;
            }

            return par;
        }
        
        private String ItemData(String line, String auxTDparameters2)
        {
            String auxItemData;
            String auxTDparameters = "";

            auxItemData = line.Replace("\t", "").Replace("\"", "");

            if (line.StartsWith("\"N") && line.Contains("ENDITEM"))
            {
                String[] auxItemData2 = auxItemData.Split(',');

                auxItemData2[0] = auxItemData2[0].Substring(5);
                auxItemData2[1] = auxItemData2[1].Substring(7);

                if (auxItemData2[0].Contains("|") || auxItemData2[1].Contains("|"))
                {
                    auxItemData2[0] = auxItemData2[0].Replace("|", @"\");
                    auxItemData2[1] = auxItemData2[1].Replace("|", @"\");
                }


                auxTDparameters = auxItemData2[0] + "@@" + auxItemData2[1] + "|";
                auxTDparameters2 += auxTDparameters;
            }

            return auxTDparameters2;
        }
    }
}
