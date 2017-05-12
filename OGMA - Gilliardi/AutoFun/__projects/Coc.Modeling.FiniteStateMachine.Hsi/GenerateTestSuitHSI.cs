using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coc.Modeling.TestSuitStructure;
using Coc.Modeling.FiniteStateMachine;
using Coc.Data.ControlAndConversionStructures;
using Coc.Modeling.Uml;
using System.Globalization;

namespace Coc.Data.HSI
{
    public class GenerateTestSuit
    {
        TestSuit suit = new TestSuit(DateTime.Now.ToString());
        Scenario scenario;
        float actualTestCaseProb = 0;
        int equalsTcCount = 0;

        #region Public Methods
        public TestSuit PopulateTestSuit(String[][] matriz, FiniteStateMachine machine, GeneralUseStructure modelGeneralUseStructure)
        {
            UmlModel model = (UmlModel)modelGeneralUseStructure;
            foreach (UmlUseCaseDiagram ucDiagram in model.Diagrams.OfType<UmlUseCaseDiagram>())
            {
                UmlUseCase equivalentUC = ucDiagram.UmlObjects.OfType<UmlUseCase>().Where(x => x.Name.Equals(machine.Name)).FirstOrDefault();

                foreach (UmlActor actor in ucDiagram.UmlObjects.OfType<UmlActor>())
                {
                    foreach (UmlAssociation association in ucDiagram.UmlObjects.OfType<UmlAssociation>())
                    {
                        if ((association.End1.Equals(actor) && association.End2.Equals(equivalentUC)) || (association.End1.Equals(equivalentUC) && association.End2.Equals(actor)))
                        {
                            try
                            {
                                actualTestCaseProb = float.Parse(association.GetTaggedValue("TDprob"), CultureInfo.InvariantCulture);
                            }
                            catch
                            {
                                actualTestCaseProb = 0;
                            }
                            if (suit.Scenarios.Count < 1)
                            {
                                scenario = new Scenario();
                                scenario.Name = actor.Name;
                                AddScenarioInformation(actor);
                                suit.Scenarios.Add(scenario);
                            }

                            foreach (Scenario scenarioAlreadyAdded in suit.Scenarios)
                            {
                                if (actor.Name.Equals(scenarioAlreadyAdded.Name))
                                {
                                    scenario = scenarioAlreadyAdded;
                                }
                                else
                                {
                                    scenario = new Scenario();
                                    scenario.Name = actor.Name;
                                    AddScenarioInformation(actor);
                                    suit.Scenarios.Add(scenario);
                                }
                            }
                        }
                    }
                }
            }

            for (int k = 0; k < matriz.Length; k++)
            {
                List<Transition> listTransition = new List<Transition>();
                String[] arraySequence = matriz[k];

                foreach (String input in arraySequence)
                {
                    Transition tran = new Transition();
                    tran = GetTransitionFSM(input, machine, ref listTransition);

                    if (tran != null)
                    {
                        listTransition.Add(tran);
                    }
                }
                scenario.TestCases.Add(FillTestCase(machine, listTransition));
            }
            //WriteStructureForDebug();

            return suit;
        }
        #endregion

        #region Private Methods
        private void WriteStructureForDebug()
        {
            String text = suit.Name;
            String newLine = "\n";
            String tab = "\t";
            String splitLine = "------------------------------------------------------------------------------------------";

            foreach (Scenario scenario in suit.Scenarios)
            {
                text += newLine + suit.Scenarios.Count + " SCENARIOS";
                text += newLine + "SCENARIO: " + scenario.Name;
                text += newLine + tab + scenario.TestCases.Count + " TESTCASES";
                foreach (TestCase testCase in scenario.TestCases)
                {
                    text += newLine + splitLine;
                    text += newLine + tab + "TESTCASE: " + testCase.ToString();
                    text += newLine + tab + tab + testCase.Transactions.Count + " TRANSACTIONS";
                    foreach (Transaction transaction in testCase.Transactions)
                    {
                        text += newLine + tab + tab + "TRANSACTION: " + transaction.ToString();
                        text += newLine + tab + tab + tab + transaction.Subtransactions.Count + " SUBTRANSACTIONS";
                        foreach (Subtransaction subtransaction in transaction.Subtransactions)
                        {
                            text += newLine + tab + tab + tab + "SUBTRANSACTION: " + subtransaction.Name;
                        }
                    }
                    //text += newLine + tab + tab + testCase.Requests.Count + " REQUESTS";
                    //foreach (Request request in testCase.Requests)
                    {
                        //text += newLine + tab + tab + "REQUEST: " + request.Name;
                    }
                }
            }
            String pathToWrite = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
            System.IO.File.WriteAllText(pathToWrite + @"\arquivo321.txt", text);
        }

        private Transition GetTransitionFSM(String input, FiniteStateMachine fsm, ref List<Transition> listTransition)
        {
            List<Transition> transition = fsm.Transitions.Where(x => x.Input.Equals(input)).ToList();

            foreach (Transition t in transition)
            {
                if (!listTransition.Contains(t))
                {
                    return t;
                }
            }

            return null;
        }

        private TestCase FillTestCase(FiniteStateMachine machine, List<Transition> listTransition)
        {
            TestCase testCase = new TestCase();
            testCase.Name = machine.Name;
            testCase.Probability = actualTestCaseProb;
            Transaction transaction = null;
            bool existsLane = false;

            for (int i = 0; i < listTransition.Count; i++)
            {
                Transition t = listTransition[i];

                State s = machine.States.Where(x => x.Name.Equals(t.SourceState.Name)).FirstOrDefault();
                try
                {
                    existsLane = !String.IsNullOrEmpty(s.TaggedValues["Lane"]);
                }
                catch
                {
                    existsLane = false;
                }
                if (existsLane)
                {
                    //List<Transaction> transactions = testCase.Transactions.Where(x => x.Name.Equals(s.TaggedValues["Lane"])).ToList();
                    //if (transactions.Count == 0)
                    //{
                    if (transaction == null || !transaction.Name.Equals(s.TaggedValues["Lane"]))
                    {
                        transaction = new Transaction();
                        transaction.Name = s.TaggedValues["Lane"];
                        Subtransaction subtransaction = null;
                        if (!t.SourceState.Name.Contains("InitialNode"))
                        {
                            subtransaction = new Subtransaction();
                            subtransaction.Name = t.SourceState.Name;
                        }
                        Request request = new Request();
                        request.Name = t.SourceState.Name;
                        GetRequestTags(t, request);
                        if (subtransaction != null)
                        {
                            subtransaction.Begin = request;
                            subtransaction.End = request;
                            transaction.Subtransactions.Add(subtransaction);
                        }
                        testCase.Transactions.Add(transaction);
                    }
                    else
                    {
                        //transaction = transactions.FirstOrDefault();
                        Subtransaction subtransaction = null;
                        if (!t.SourceState.Name.Contains("InitialNode"))
                        {
                            subtransaction = new Subtransaction();
                            subtransaction.Name = t.SourceState.Name;
                        }
                        Request request = new Request();
                        request.Name = t.SourceState.Name;
                        GetRequestTags(t, request);
                        if (subtransaction != null)
                        {
                            subtransaction.Begin = request;
                            subtransaction.End = request;
                            transaction.Subtransactions.Add(subtransaction);
                        }
                    }
                }
                else
                {
                    transaction = new Transaction();
                    transaction.Name = t.SourceState.Name;
                    Request request = new Request();
                    GetRequestTags(t, request);
                    transaction.Begin = request;
                    transaction.End = request;
                    testCase.Transactions.Add(transaction);
                }
                AddRequestsToTestCase(testCase, t);

                if (i == (listTransition.Count - 1))
                {
                    s = machine.States.Where(x => x.Name.Equals(t.TargetState.Name)).FirstOrDefault();
                    try
                    {
                        existsLane = !String.IsNullOrEmpty(s.TaggedValues["Lane"]);
                    }
                    catch
                    {
                        existsLane = false;
                    }
                    if (existsLane)
                    {
                        List<Transaction> transactions = testCase.Transactions.Where(x => x.Name.Equals(s.TaggedValues["Lane"])).ToList();
                        if (transactions.Count == 0)
                        {
                            transaction = new Transaction();
                            transaction.Name = s.TaggedValues["Lane"];
                            Subtransaction subtransaction = null;
                            if (!t.TargetState.Name.Contains("InitialNode"))
                            {
                                subtransaction = new Subtransaction();
                                subtransaction.Name = t.TargetState.Name;
                            }
                            Request request = new Request();
                            request.Name = t.TargetState.Name;
                            GetRequestTags(t, request);
                            if (subtransaction != null)
                            {
                                subtransaction.Begin = request;
                                subtransaction.End = request;
                                transaction.Subtransactions.Add(subtransaction);
                            }
                            testCase.Transactions.Add(transaction);
                        }
                        else
                        {
                            transaction = transactions.FirstOrDefault();
                            Subtransaction subtransaction = null;
                            if (!t.TargetState.Name.Contains("InitialNode"))
                            {
                                subtransaction = new Subtransaction();
                                subtransaction.Name = t.TargetState.Name;
                            }
                            Request request = new Request();
                            request.Name = t.TargetState.Name;
                            GetRequestTags(t, request);
                            if (subtransaction != null)
                            {
                                subtransaction.Begin = request;
                                subtransaction.End = request;
                                transaction.Subtransactions.Add(subtransaction);
                            }
                        }
                    }
                    else
                    {
                        transaction = new Transaction();
                        transaction.Name = t.SourceState.Name;
                        Request request = new Request();
                        GetRequestTags(t, request);
                        transaction.Begin = request;
                        transaction.End = request;
                        testCase.Transactions.Add(transaction);
                    }
                    AddRequestsToTestCase(testCase, t);
                }
            }

            foreach (Transaction t in testCase.Transactions)
            {
                if (t.Subtransactions.Count > 0)
                {
                    t.Begin = t.Subtransactions.ElementAt(0).Begin;
                    t.End = t.Subtransactions.ElementAt(t.Subtransactions.Count - 1).Begin;
                }
            }

            return testCase;
        }

        private void AddRequestsToTestCase(TestCase testCase, Transition t)
        {
            Request request = new Request();
            GetRequestTags(t, request);
            if (!testCase.Requests.Contains(request))
            {
                testCase.Requests.Add(request);
            }
        }

        private void GetRequestTags(Transition t, Request request)
        {
            try
            {
                request.Action = t.TaggedValues["TDACTION"];
            }
            catch
            {

            }
            try
            {
                request.Body = t.TaggedValues["TDBODY"];
            }
            catch
            {

            }
            try
            {
                //request.Cookies = t.TaggedValues["TDCOOKIES"];
            }
            catch
            {

            }
            try
            {
                request.ExpectedTime = Convert.ToDouble(t.TaggedValues["TDEXPECTEDTIME"]);
            }
            catch
            {

            }
            try
            {
                request.Method = t.TaggedValues["TDMETHOD"];
            }
            catch
            {

            }
            try
            {
                if (String.IsNullOrEmpty(request.Name))
                {
                    request.Name = t.SourceState.Name;
                }
            }
            catch
            {

            }
            try
            {
                //request.OptimisticTime = Convert.ToDouble(t.TaggedValues["TDOPTIMISTICTIME"]);
            }
            catch
            {

            }
            try
            {
                //request.Parameters = t.TaggedValues["TDPARAMETERS"];
            }
            catch
            {

            }
            try
            {
                //request.PessimisticTime = Convert.ToDouble(t.TaggedValues["TDPESSIMISTICTIME"]);
            }
            catch
            {

            }
            try
            {
                request.Referer = t.TaggedValues["TDREFERER"];
            }
            catch
            {

            }
            try
            {
                //request.SaveParameters = t.TaggedValues["TDSAVEPARAMETERS"];
            }
            catch
            {

            }
            try
            {
                request.ThinkTime = Convert.ToDouble(t.TaggedValues["TDTHINKTIME"]);
            }
            catch
            {

            }
        }

        private void AddScenarioInformation(UmlActor actor)
        {
            try
            {
                //scenario.HostSUT.Name = actor.GetTaggedValue("TDHOST");
                scenario.ExecutionTime = Convert.ToInt32(actor.GetTaggedValue("TDTIME"));
                scenario.RampUpTime = Convert.ToInt32(actor.GetTaggedValue("TDRAMPUPTIME"));
                scenario.RampUpUser = Convert.ToInt32(actor.GetTaggedValue("TDRAMPUPUSER"));
                scenario.RampDownTime = Convert.ToInt32(actor.GetTaggedValue("TDRAMPDOWNTIME"));
                scenario.RampDownUser = Convert.ToInt32(actor.GetTaggedValue("TDRAMPDOWNUSER"));
                scenario.Population = Convert.ToInt32(actor.GetTaggedValue("TDPOPULATION"));
            }
            catch
            {
                //do something
            }
        }
        #endregion
    }
}
