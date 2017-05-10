using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

using Coc.Data.Xmi;
using Coc.Modeling.Uml;

namespace Coc.Data.Xmi
{
    public static class LoadRunnerImporter
    {
        public static UmlModel FromLoadRunnerScript(this UmlModel model, String filepath)
        {
            filepath = "Action.c";

            UmlActivityDiagram activityDiagram = new UmlActivityDiagram("ActivityDiagram");
            bool blocoTransaction = false;
            bool blocoItemData = false;
            bool blocoSubtransaction = false;
            bool blocoSaveParameter = false;
            bool existsLane = false;
            bool createdLane = false;
            bool putInitial = false;
            bool existsThinkTime = false;
            int i = 0;
            UmlTransition transition = null;
            UmlInitialState initial = new UmlInitialState();
            UmlFinalState final = new UmlFinalState();
            UmlElement antActionState = new UmlActionState();
            UmlActionState transaction = null;
            UmlActionState subtransaction = null;
            UmlLane lane = null;
            UmlTransition antTransition = new UmlTransition();
            String auxMethod = null;
            String auxReferer = null;
            String auxBody = null;
            String auxUrl = null;
            String auxAction = null;
            String auxThinkTime = null;
            String auxSaveParam = null;
            String auxSaveParam2 = null;
            String auxCookies = null;
            String auxCookies2 = null;
            String line = null;
            String auxTDparameters2 = null;
            String[] auxSubtransaction = null;
            String[] allLines = File.ReadAllLines(filepath);

            IEnumerable<String> allLines2 = from String s in allLines
                                            where s.Trim() != String.Empty
                                            select s;
            allLines = allLines2.ToArray();
            activityDiagram.UmlObjects.Add(initial);

            for (int k = 0; k < allLines.Length; k++)
            {
                line = allLines[k];

                #region ThinkTime
                if (line.Contains("lr_think_time("))
                {
                    auxThinkTime = line.Replace("\t", "").Substring(14).Trim();
                    auxThinkTime = auxThinkTime.Substring(0, auxThinkTime.Length - 2);
                    existsThinkTime = true;
                }
                #endregion

                if (line.Contains("lr_start_transaction("))
                {
                    if (line.Contains("lr_start_transaction(\"uc"))
                    {
                        activityDiagram.Name = line.Replace("\t", "").Trim().Substring(22);
                        activityDiagram.Name = activityDiagram.Name.Substring(0, activityDiagram.Name.Length - 3);
                        continue;
                    }

                    blocoTransaction = true;

                    if (allLines[k + 1].Contains("lr_start_sub_transaction("))
                    {
                        existsLane = true;
                        continue;
                    }
                }
                else if (line.Contains("lr_end_transaction("))
                {
                    if (line.Contains("lr_end_transaction(\"uc"))
                    {
                        continue;
                    }
                    String auxLine = line.Substring(21);
                    auxLine = auxLine.Substring(0, auxLine.Length - 11);

                    if (auxLine.Equals(lane.Name))
                    {
                        createdLane = false;
                    }

                    if (!activityDiagram.Lanes.Contains(lane))
                    {
                        activityDiagram.Lanes.Add(lane);
                    }
                    blocoTransaction = false;
                    existsLane = false;
                    continue;
                }

                if (blocoTransaction)
                {
                    if (line.Contains("lr_start_sub_transaction("))
                    {
                        blocoSubtransaction = true;
                    }
                    else if (line.Contains("lr_end_sub_transaction("))
                    {
                        lane.AddElement(subtransaction);
                        blocoSubtransaction = false;
                        continue;
                    }

                    if (existsLane)
                    {
                        if (!createdLane)
                        {
                            lane = new UmlLane();

                            lane.Index = i;
                            i++;
                            createdLane = true;
                        }
                        if (blocoSubtransaction)
                        {
                            #region SubTransaction
                            if (line.Contains("lr_start_sub_transaction("))
                            {
                                subtransaction = new UmlActionState();
                                transition = new UmlTransition();

                                subtransaction.Name = line.Replace("\t", "").Replace("\"", "").Substring(25);
                                auxSubtransaction = subtransaction.Name.Split(',');
                                auxSubtransaction[0] = auxSubtransaction[0].Substring(0, auxSubtransaction[0].Length).Trim();
                                auxSubtransaction[1] = auxSubtransaction[1].Substring(0, auxSubtransaction[1].Length - 2).Trim();

                                subtransaction.Name = auxSubtransaction[0];

                                if (!activityDiagram.Lanes.Contains(lane))
                                {
                                    lane.Name = auxSubtransaction[1];
                                }
                                initial.Name = "Initial0";
                                final.Name = "Final0";

                                #region PutInitial
                                if (!putInitial)
                                {
                                    transition.Source = initial;
                                    transition.Target = subtransaction;
                                    antActionState = transition.Target;
                                    putInitial = true;
                                }
                                else
                                {
                                    transition.Source = antActionState;
                                    transition.Target = subtransaction;
                                    antActionState = transition.Target;
                                }
                                #endregion

                                activityDiagram.UmlObjects.Add(subtransaction);
                                activityDiagram.UmlObjects.Add(transition);
                            }
                            #endregion
                            #region Tagged Values
                            #region URL
                            else if (line.Contains("URL="))
                            {
                                auxUrl = Url(transition, auxUrl, line);
                            }
                            #endregion
                            #region Action
                            else if (line.Contains("Action="))
                            {
                                auxAction = Action(transition, auxAction, line);
                            }
                            #endregion
                            #region Referer
                            else if (line.Contains("Referer="))
                            {
                                auxReferer = Referer(transition, auxReferer, line);
                            }
                            #endregion
                            #region Method
                            else if (line.Contains("Method="))
                            {
                                auxMethod = Method(transition, auxMethod, line);
                            }
                            #endregion
                            #region Body
                            else if (line.Contains("Body="))
                            {
                                auxBody = Body(transition, auxBody, line);
                            }
                            #endregion
                            #region Cookies
                            if (line.Contains("web_add_cookie("))
                            {
                                auxCookies = line.Replace("\t", "").Trim().Substring(16);
                                auxCookies = auxCookies.Substring(0, auxCookies.Length - 3);
                                auxCookies = auxCookies + "|";

                                auxCookies2 += auxCookies;
                                if (!allLines[k + 1].Contains("web_add_cookie("))
                                {
                                    //SÓ DEVE OCORRER NO FINAL
                                    auxCookies2 = auxCookies2.Substring(0, auxCookies2.Length - 1);
                                    transition.SetTaggedValue("TDcookies", auxCookies2);
                                }
                            }
                            #endregion
                            #region ItemData
                            else if (line.Contains("ITEMDATA"))
                            {
                                blocoItemData = true;
                                continue;
                            }
                            else if (line.Contains("LAST);") && blocoItemData)
                            {
                                transition.SetTaggedValue("TDparameters", auxTDparameters2);
                                blocoItemData = false;
                                continue;
                            }

                            if (blocoItemData)
                            {
                                auxTDparameters2 = ItemData(line, auxTDparameters2);
                            }
                            #endregion
                            #region ThinkTime
                            if (existsThinkTime)
                            {
                                transition.SetTaggedValue("TDthinkTime", auxThinkTime);
                                existsThinkTime = false;
                            }
                            #endregion
                            #endregion
                        }
                    }
                    else
                    {
                        #region Transaction
                        if (line.Contains("lr_start_transaction("))
                        {
                            transaction = new UmlActionState();
                            transition = new UmlTransition();
                            transaction.Name = line.Replace("\t", "").Trim().Substring(22);
                            transaction.Name = transaction.Name.Substring(0, transaction.Name.Length - 3);

                            if (!(allLines[k + 1].Contains("lr_start_sub_transaction(")))
                            {
                                if (lane.Name == "")
                                {
                                    lane.AddElement(transaction);
                                }
                                else
                                {
                                    lane = new UmlLane();
                                    lane.Name = "";
                                    lane.Index = i;
                                    i++;
                                    lane.AddElement(transaction);
                                }
                            }

                            #region PutInitial
                            if (!putInitial)
                            {
                                transition.Source = initial;
                                transition.Target = transaction;
                                antActionState = transition.Target;
                                putInitial = true;
                            }
                            else
                            {
                                transition.Source = antActionState;
                                transition.Target = transaction;
                                antActionState = transition.Target;
                            }
                            #endregion

                            activityDiagram.UmlObjects.Add(transaction);
                            activityDiagram.UmlObjects.Add(transition);
                        }
                        #endregion
                        #region Tagged Values
                        #region URL
                        else if (line.Contains("URL="))
                        {
                            auxUrl = Url(transition, auxUrl, line);
                        }
                        #endregion
                        #region Action
                        else if (line.Contains("Action="))
                        {
                            auxAction = Action(transition, auxAction, line);
                        }
                        #endregion
                        #region Referer
                        else if (line.Contains("Referer="))
                        {
                            auxReferer = Referer(transition, auxReferer, line);
                        }
                        #endregion
                        #region Method
                        else if (line.Contains("Method="))
                        {
                            auxMethod = Method(transition, auxMethod, line);
                        }
                        #endregion
                        #region Body
                        else if (line.Contains("Body="))
                        {
                            auxBody = Body(transition, auxBody, line);
                        }
                        #endregion
                        #region Cookies
                        if (line.Contains("web_add_cookie("))
                        {
                            auxCookies = line.Replace("\t", "").Trim().Substring(16);
                            auxCookies = auxCookies.Substring(0, auxCookies.Length - 3);
                            auxCookies = auxCookies + "|";

                            auxCookies2 += auxCookies;
                            if (!allLines[k + 1].Contains("web_add_cookie("))
                            {
                                //SÓ DEVE OCORRER NO FINAL
                                auxCookies2 = auxCookies2.Substring(0, auxCookies2.Length - 1);
                                transition.SetTaggedValue("TDcookies", auxCookies2);
                            }
                        }
                        #endregion
                        #region ItemData
                        else if (line.Contains("ITEMDATA"))
                        {
                            blocoItemData = true;
                            continue;
                        }
                        else if (line.Contains("LAST);") && blocoItemData)
                        {
                            transition.SetTaggedValue("TDparameters", auxTDparameters2);
                            blocoItemData = false;
                            continue;
                        }

                        if (blocoItemData)
                        {
                            auxTDparameters2 = ItemData(line, auxTDparameters2);
                        }
                        #endregion
                        #region ThinkTime
                        if (existsThinkTime)
                        {
                            transition.SetTaggedValue("TDthinkTime", auxThinkTime);
                            existsThinkTime = false;
                        }
                        #endregion
                        #endregion
                    }
                }
                #region SaveParameters
                if (line.Contains("web_reg_save_param("))
                {
                    blocoSaveParameter = true;

                    auxSaveParam = line.Replace("\t", "").Trim().Substring(20);
                    auxSaveParam = auxSaveParam.Substring(0, auxSaveParam.Length - 1);
                    auxSaveParam = auxSaveParam + "|";

                    auxSaveParam2 += auxSaveParam;
                }
                else if (line.Contains("LAST);") && blocoSaveParameter)
                {
                    if (allLines[k + 1].Contains("web_reg_save_param("))
                    {
                        continue;
                    }
                    else
                    {
                        //SÓ DEVE OCORRER NO FINAL
                        auxSaveParam2 = auxSaveParam2.Substring(0, auxSaveParam2.Length - 1);
                        transition.SetTaggedValue("TDsaveParameters", auxSaveParam2);
                        auxSaveParam2 = null;
                        blocoSaveParameter = false;
                    }
                }
                #endregion
            }
            transition = new UmlTransition();
            transition.Source = antActionState;
            transition.Target = final;

            activityDiagram.UmlObjects.Add(transition);
            activityDiagram.UmlObjects.Add(final);

            if (activityDiagram.Lanes.Count() > 0)
            {
                activityDiagram.Lanes[0].AddElement(initial);
                lane.AddElement(final);
            }

            model.AddDiagram(activityDiagram);

            return model;
        }

        private static String ItemData(String line, String auxTDparameters2)
        {
            String auxItemData;
            String auxTDparameters;

            auxItemData = line.Replace("\t", "").Replace("\"", "");

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
            return auxTDparameters2;
        }
        private static String Body(UmlTransition transition, String auxBody, String line)
        {
            auxBody = line.Replace("\t", "").Trim().Substring(6);
            auxBody = auxBody.Substring(0, auxBody.Length - 2);
            if (auxBody.Contains("|"))
            {
                auxBody = auxBody.Replace("|", @"\");
            }

            transition.SetTaggedValue("TDbody", auxBody);
            return auxBody;
        }
        private static String Method(UmlTransition transition, String auxMethod, String line)
        {
            auxMethod = line.Replace("\t", "").Trim().Substring(8);
            auxMethod = auxMethod.Substring(0, auxMethod.Length - 2);

            transition.SetTaggedValue("TDmethod", auxMethod);
            return auxMethod;
        }
        private static String Referer(UmlTransition transition, String auxReferer, String line)
        {
            auxReferer = line.Replace("\t", "").Trim().Substring(9);
            auxReferer = auxReferer.Substring(0, auxReferer.Length - 2);

            if (auxReferer.Length > 0)
            {
                if (auxReferer.Contains("_"))
                {
                    auxReferer = auxReferer.Replace("_", ".");
                }
                transition.SetTaggedValue("TDreferer", auxReferer);
            }
            return auxReferer;
        }
        private static String Action(UmlTransition transition, String auxAction, String line)
        {
            auxAction = line.Replace("\t", "").Trim().Substring(8);
            auxAction = auxAction.Substring(0, auxAction.Length - 2);

            if (auxAction.Contains("_"))
            {
                auxAction = auxAction.Replace("_", ".");
            }
            transition.SetTaggedValue("TDACTION", auxAction);
            return auxAction;
        }
        private static String Url(UmlTransition transition, String auxUrl, String line)
        {
            auxUrl = line.Replace("\t", "").Trim().Substring(5);
            auxUrl = auxUrl.Substring(0, auxUrl.Length - 2);

            if (auxUrl.Contains("_"))
            {
                auxUrl = auxUrl.Replace("_", ".");
            }
            transition.SetTaggedValue("TDACTION", auxUrl);
            return auxUrl;
        }
    }
}