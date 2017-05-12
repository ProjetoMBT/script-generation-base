using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.XPath;
using System.Reflection;
using System.Collections;
using Coc.Modeling.Uml;
using Coc.Testing.Performance.IntermediateStruct;

namespace Coc.Testing.Performance.Importers
{
    public static class UmlModelImporter
    {
        public static TestSuit XmiToTestSuit(UmlModel model)
        {
            //TestSuit Initialization
            TestSuit testSuit = new TestSuit();
            testSuit.Name = model.Name;

            UmlUseCaseDiagram useCaseDiagram = model.Diagrams.OfType<UmlUseCaseDiagram>().FirstOrDefault();

            if (useCaseDiagram == null)
            {
                return testSuit;
            }

            //Actors to Scenarios
            foreach (UmlActor actor in useCaseDiagram.UmlObjects.OfType<UmlActor>())
            {
                Scenario scenario = new Scenario();
                try
                {
                    scenario.Name = HttpUtility.UrlDecode(actor.Name);
                    scenario.Population = Convert.ToInt32(actor.TaggedValues["TDPOPULATION"]);
                    scenario.ExecutionTime = Convert.ToInt32(actor.TaggedValues["TDEXECUTIONTIME"]);
                    scenario.HostSUT = hostSUTParse(actor.TaggedValues, "TDHOST");
                    scenario.RampUpUser = Convert.ToInt32(actor.TaggedValues["TDRAMPUPUSER"]);
                    scenario.RampDownUser = Convert.ToInt32(actor.TaggedValues["TDRAMPDOWNUSER"]);
                    scenario.RampUpTime = Convert.ToDouble(actor.TaggedValues["TDRAMPUPTIME"]);
                    scenario.RampDownTime = Convert.ToDouble(actor.TaggedValues["TDRAMPDOWNTIME"]);
                    scenario.AdditionalHosts = additionalHostsParse(actor.TaggedValues, "TDHOST");
                }
                catch { }

                //UseCases to TestCases
                #region Scenario Test Cases
                scenario.TestCases = new List<TestCase>();
                foreach (UmlUseCaseDiagram ucDiagram in model.Diagrams.OfType<UmlUseCaseDiagram>())
                {
                    foreach (UmlUseCase uc in ucDiagram.UmlObjects.OfType<UmlUseCase>())
                    {
                        TestCase testCase = new TestCase();
                        testCase.Name = HttpUtility.UrlDecode(uc.Name);

                        foreach (UmlAssociation association in ucDiagram.UmlObjects.OfType<UmlAssociation>())
                        {
                            if ((association.End1.Equals(actor) && association.End2.Equals(uc)) || (association.End1.Equals(uc) && association.End2.Equals(actor)))
                            {
                                testCase.Probability = (float)Convert.ToDouble(association.GetTaggedValue("TDprob"));
                            }
                        }

                        List<Request> listRequests = new List<Request>();
                        List<Transaction> listTransactions = new List<Transaction>();
                        
                        foreach (UmlActivityDiagram actDiagram in model.Diagrams.OfType<UmlActivityDiagram>())
                        {
                            if (uc.Name.Equals(actDiagram.Name))
                            {
                                #region Test Case Requests
                                getRequests(actDiagram, listRequests, model);
                                testCase.Requests = listRequests;
                                #endregion
                                #region Test Case Transactions
                                getTransactions(actDiagram, listTransactions, listRequests, model);
                                testCase.Transactions = listTransactions;
                                #endregion
                                getParallelState(actDiagram, listRequests, model);

                                break;
                            }
                        }
                        scenario.TestCases.Add(testCase);
                    }
                }
                testSuit.Scenarios.Add(scenario);
                #endregion
            }
            return testSuit;
        }


        public static List<Transition> parallelTransitions(UmlTransition t, UmlActivityDiagram actDiagram)
        {
            List<UmlTransition> listUmlTransition = new List<UmlTransition>();
            List<Transition> listTransition = new List<Transition>();
            Transition tran1 = new Transition();
            foreach (UmlTransition tran in actDiagram.UmlObjects.OfType<UmlTransition>())
            {
                if (t.Target.Id.Equals(tran.Source.Id))
                {
                    listUmlTransition.Add(tran);
                }
            }

            Transition tran2 = new Transition();
            tran2.Action = listUmlTransition[0].GetTaggedValue("TDACTION");
            tran2.Referer = listUmlTransition[0].GetTaggedValue("TDREFERER");
            tran2.Source = t.Source.Name;
            tran2.Target = listUmlTransition[0].Target.Name;
            listTransition.Add(tran2);

            UmlTransition transition = listUmlTransition[0];
            tran1.Action = transition.GetTaggedValue("TDACTION");
            tran1.Referer = transition.GetTaggedValue("TDREFERER");
            tran1.Source = transition.Target.Name;
            for (int i = 1; i < listUmlTransition.Count; i++)
            {
                transition = listUmlTransition[i];
                tran1.Action = transition.GetTaggedValue("TDACTION");
                tran1.Referer = transition.GetTaggedValue("TDREFERER");
                tran1.Target = transition.Target.Name;
                listTransition.Add(tran1);
                tran1 = new Transition();
                tran1.Source = listTransition[listTransition.Count - 1].Target;
            }
            Transition tran3 = new Transition();
            UmlTransition transitionAux = getNextState(listTransition[listTransition.Count - 1].Target, actDiagram);
            tran3.Action = transitionAux.GetTaggedValue("TDACTION");
            tran3.Referer = transitionAux.GetTaggedValue("TDREFERER");
            tran3.Source = listTransition[listTransition.Count - 1].Target;
            tran3.Target = transitionAux.Target.Name;
            listTransition.Add(tran3);
            return listTransition;
        }

        private static UmlTransition getNextState(String act, UmlActivityDiagram actDiagram)
        {
            foreach (UmlTransition transition in actDiagram.UmlObjects.OfType<UmlTransition>())
            {
                if (transition.Source.Name.Equals(act))
                {
                    foreach (UmlTransition tran in actDiagram.UmlObjects.OfType<UmlTransition>())
                    {
                        if (transition.Target.Id.Equals(tran.Source.Id))
                        {
                            return tran;
                        }
                    }
                }
            }
            return null;
        }

        private static void getParallelState(UmlActivityDiagram actDiagram, List<Request> requests, UmlModel model)
        {
            foreach (UmlActionState act in actDiagram.UmlObjects.OfType<UmlActionState>())
            {
                try
                {
                    if (!((act is UmlInitialState) || (act is UmlFinalState)))
                    {
                        String diagramName = act.TaggedValues["jude.hyperlink"];
                        foreach (UmlActivityDiagram actDiagram2 in model.Diagrams.OfType<UmlActivityDiagram>())
                        {
                            if (actDiagram2.Name.Equals(diagramName))
                            {
                                getParallelState(actDiagram2, requests, model);
                            }
                        }
                    }
                }
                catch
                {
                    foreach (UmlTransition t in actDiagram.UmlObjects.OfType<UmlTransition>())
                    {
                        if (t.Source is UmlFork)
                        {
                            foreach (Request req in requests)
                            {
                                if (req.Name.Equals(t.Target.Name))
                                {
                                    req.ParallelState = true;
                                    break;
                                }
                            }
                        }
                        else if (t.Target is UmlJoin)
                        {
                            foreach (Request req in requests)
                            {
                                if (req.Name.Equals(t.Source.Name))
                                {
                                    req.ParallelState = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void getTransactions(UmlActivityDiagram actDiagram, List<Transaction> transactions, List<Request> requests, UmlModel model)
        {
            Transaction transaction = new Transaction();
            UmlActionState aux_first = new UmlActionState();
            UmlActionState aux_last = new UmlActionState();

            foreach (UmlActionState act in actDiagram.UmlObjects.OfType<UmlActionState>())
            {
                try
                {
                    if (!((act is UmlInitialState) || (act is UmlFinalState) || (act is UmlPseudoState)))
                    {
                        String diagramName = act.TaggedValues["jude.hyperlink"];
                        foreach (UmlActivityDiagram actDiagram2 in model.Diagrams.OfType<UmlActivityDiagram>())
                        {
                            if (actDiagram2.Name.Equals(diagramName))
                            {
                                getTransactions(actDiagram2, transactions, requests, model);
                            }
                        }
                    }
                }
                catch
                {
                    if (act.ParentLane == null || act.ParentLane.Name.Equals(""))
                    {
                        transaction = new Transaction();
                        foreach (Request req in requests)
                        {
                            if (req.Name.Equals(act.Name))
                            {
                                transaction.Name = act.Name;
                                transaction.Begin = req;
                                transaction.End = req;
                                transactions.Add(transaction);
                                break;
                            }
                        }
                    }
                    else
                    {
                        Boolean exist = false;
                        if (transactions.Count() > 0)
                        {
                            for (int i = 0; i < transactions.Count; i++)
                            {
                                Transaction auxTransaction = transactions[i];
                                if (auxTransaction.Name.Equals(act.ParentLane.Name))
                                {
                                    foreach (Subtransaction sub in auxTransaction.Subtransactions)
                                    {
                                        foreach (UmlActionState element in act.ParentLane.GetElements())
                                        {
                                            if (element.Name.Equals(sub.Name))
                                            {
                                                exist = true;
                                            }
                                        }
                                    }
                                    if (!(auxTransaction.Name.Equals(transaction.Name)))
                                    {
                                        transaction = auxTransaction;
                                    }
                                }
                            }
                        }

                        if (!exist)
                        {
                            transaction = new Transaction();
                            transactions.Add(transaction);

                            transaction.Name = act.ParentLane.Name;
                            transaction.Requests = new List<Request>();
                            transaction.Subtransactions = new List<Subtransaction>();

                            aux_first = getLaneFirstState(act.ParentLane, actDiagram);
                            aux_last = getLaneLastState(act.ParentLane, actDiagram);
                        }

                        Subtransaction subtransaction = new Subtransaction();

                        foreach (Request req in requests)
                        {
                            if (req.Name.Equals(act.Name))
                            {
                                if (req.Name.Equals(aux_first.Name))
                                {
                                    transaction.Begin = req;
                                }
                                if (req.Name.Equals(aux_last.Name))
                                {
                                    transaction.End = req;
                                }

                                subtransaction.Name = act.Name;
                                subtransaction.Begin = req;
                                subtransaction.End = req;
                                transaction.Requests.Add(req);
                                transaction.Subtransactions.Add(subtransaction);

                                break;
                            }
                        }
                    }
                }
            }
        }

        private static void getRequests(UmlActivityDiagram actDiagram, List<Request> requests, UmlModel model)
        {
            foreach (UmlActionState act in actDiagram.UmlObjects.OfType<UmlActionState>())
            {
                try
                {
                    if (!((act is UmlInitialState) || (act is UmlFinalState) || (act is UmlPseudoState)))
                    {
                        String diagramName = act.TaggedValues["jude.hyperlink"];
                        foreach (UmlActivityDiagram actDiagram2 in model.Diagrams.OfType<UmlActivityDiagram>())
                        {
                            if (actDiagram2.Name.Equals(diagramName))
                            {
                                getRequests(actDiagram2, requests, model);
                            }
                        }
                    }
                }
                catch
                {
                    bool exist = false;
                    foreach (var item in requests)
                    {
                        if (item.Name.Equals(act.Name))
                        {
                            exist = true;
                        }
                    }
                    if (!exist)
                    {
                        Request r = new Request();
                        r.Name = act.Name;
                        r.Transitions = getTransitions(act, actDiagram);
                        if (r.Transitions.Count > 1)
                        {
                            foreach (Transition item in r.Transitions)
                            {
                                Request request = new Request();
                                request.Name = item.Source;
                                request.Transitions.Add(item);
                                requests.Add(request);
                            }
                        }
                        else
                        {
                            requests.Add(r);
                        }
                    }
                }
            }
        }

        private static List<Transition> getTransitions(UmlActionState act, UmlActivityDiagram actDiagram)
        {
            List<Transition> list = new List<Transition>();
            foreach (UmlTransition t in actDiagram.UmlObjects.OfType<UmlTransition>())
            {
                if (t.Source.Id.Equals(act.Id))
                {
                    if (t.Source is UmlActionState && t.Target is UmlFork)
                    {
                        list.AddRange(parallelTransitions(t, actDiagram));
                    }
                    else if (t.Source is UmlActionState && t.Target is UmlActionState)
                    {
                        if (!(t.Source is UmlPseudoState))
                        {
                            Transition transition = new Transition();
                            transition.Action = t.GetTaggedValue("TDACTION");
                            transition.Body = t.GetTaggedValue("TDBODY");
                            if (t.GetTaggedValue("TDCOOKIES") != null)
                            {
                                transition.Cookies = getCookies(t.GetTaggedValue("TDTDCOOKIES"));
                            }
                            transition.EstimatedTime = Convert.ToDouble(t.GetTaggedValue("TDEXPTIME"));
                            transition.Method = t.GetTaggedValue("TDMETHOD");
                            if (t.GetTaggedValue("TDPARAMETERS") != null)
                            {
                                transition.Parameters = getParameters(t.GetTaggedValue("TDTDPARAMETERS"));
                            }
                            transition.Referer = t.GetTaggedValue("TDREFERER");
                            if (t.GetTaggedValue("TDSAVEPARAMETERS") != null)
                            {
                                transition.SaveParameters = getSaveParameters(t.GetTaggedValue("TDSAVEPARAMETERS"));
                            }
                            transition.ThinkTime = Convert.ToDouble(t.GetTaggedValue("TDTHINKTIME"));
                            transition.Source = t.Source.Name;
                            transition.Target = t.Target.Name;
                            list.Add(transition);
                        }
                    }
                }
            }
            return list;
        }

        public static List<Parameter> getParameters(String tr)
        {
            List<Parameter> parameters = new List<Parameter>();
            String[] tagsArray = null;
            String s = HttpUtility.UrlDecode(tr);

            s = s.Replace(@"\|", "()");
            tagsArray = s.Split('|');

            for (int i = 0; i < tagsArray.Count(); i++)
            {
                if (tagsArray[i].Contains("()"))
                {
                    tagsArray[i] = tagsArray[i].Replace("()", "|");
                }
            }

            foreach (String t in tagsArray)
            {
                string key = t.Substring(0, t.IndexOf("@@")).Trim();
                string value = t.Substring(t.IndexOf("@@") + 2, t.Length - t.IndexOf("@@") - 2).Trim();
                parameters.Add(new Parameter() { Name = key, Value = value });
            }
            return parameters;
        }

        public static List<SaveParameter> getSaveParameters(String tr)
        {
            List<SaveParameter> saveParameters = new List<SaveParameter>();
            String[] tagsArray = null;
            tr = HttpUtility.UrlDecode(tr);

            tagsArray = tr.Split('|');

            foreach (String t in tagsArray)
            {
                saveParameters.Add(new SaveParameter() { Name = t });
            }
            return saveParameters;
        }

        private static List<Cookie> getCookies(String tr)
        {
            List<Cookie> cookies = new List<Cookie>();
            String[] tagsArray = null;
            tr = tr.Replace(@"\|", "()");
            tagsArray = tr.Split('|');

            for (int i = 0; i < tagsArray.Count(); i++)
            {
                if (tagsArray[i].Contains("()"))
                {
                    tagsArray[i] = tagsArray[i].Replace("()", @"\|");
                }
            }

            foreach (String t in tagsArray)
            {
                cookies.Add(new Cookie() { Name = t });
            }
            return cookies;
        }

        public static Host hostSUTParse(Dictionary<String, String> objectPath, string tagName)
        {
            var path = from tag in objectPath
                       where tag.Key.Equals(tagName)
                       select tag.Value.ToString();

            Host hostSut = new Host();
            hostSut.Name = HttpUtility.UrlDecode((path.Count() > 0) ? path.First() : "");

            return hostSut;
        }

        public static List<Host> additionalHostsParse(Dictionary<String, String> objectPath, string tagName)
        {
            List<Host> additionalHostsList = new List<Host>();
            var additionalHosts = from tag in objectPath
                                  where tag.Key.Equals(tagName)
                                  select tag.Value;

            foreach (String hostName in additionalHosts)
            {
                additionalHostsList.Add(new Host() { Name = HttpUtility.UrlDecode(hostName) });
            }
            return additionalHostsList;
        }

        public static UmlActionState getLaneFirstState(UmlLane lane, UmlActivityDiagram activityDiagram)
        {
            foreach (UmlActionState act in lane.GetElements().OfType<UmlActionState>())
            {
                if (!(act is UmlInitialState))
                {
                    foreach (UmlTransition t in activityDiagram.UmlObjects.OfType<UmlTransition>())
                    {
                        if (t.Target.Id.Equals(act.Id))
                        {
                            if (t.Source is UmlInitialState)
                            {
                                return act;
                            }
                            else if (t.Source is UmlActionState)
                            {
                                if (!((UmlActionState)t.Source).ParentLane.Equals(act.ParentLane))
                                {
                                    return act;
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        public static UmlActionState getLaneLastState(UmlLane lane, UmlActivityDiagram activityDiagram)
        {
            foreach (UmlActionState act in lane.GetElements().OfType<UmlActionState>())
            {
                if (!(act is UmlFinalState))
                {
                    foreach (UmlTransition t in activityDiagram.UmlObjects.OfType<UmlTransition>())
                    {
                        if (t.Source.Id.Equals(act.Id))
                        {
                            if (t.Target is UmlFinalState)
                            {
                                return act;
                            }
                            else if (t.Target is UmlActionState)
                            {
                                if (!((UmlActionState)t.Target).ParentLane.Equals(act.ParentLane))
                                {
                                    return act;
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        public static UmlActionState getDiagramFirstState(UmlActivityDiagram activityDiagram)
        {
            foreach (UmlActionState act in activityDiagram.UmlObjects.OfType<UmlActionState>())
            {
                foreach (UmlTransition t in activityDiagram.UmlObjects.OfType<UmlTransition>())
                {
                    if (t.Source is UmlInitialState)
                    {
                        return ((UmlActionState)t.Target);
                    }
                }
            }
            return null;
        }

        public static UmlActionState getDiagramLastState(UmlActivityDiagram activityDiagram)
        {
            foreach (UmlActionState act in activityDiagram.UmlObjects.OfType<UmlActionState>())
            {
                foreach (UmlTransition t in activityDiagram.UmlObjects.OfType<UmlTransition>())
                {
                    if (t.Target is UmlFinalState)
                    {
                        return ((UmlActionState)t.Source);
                    }
                }
            }
            return null;
        }
    }
}
