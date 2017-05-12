using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Coc.Modeling.Uml;
using System.Reflection;
using System.Web;
using Coc.Data.Interfaces;
using Coc.Data.ControlStructure;
using Coc.Data.ControlAndConversionStructures;

namespace Coc.Data.Xmi.PerformanceValidator
{
    /*
    /// <summary>
    /// <img src="images/Xmi.PerformanceValidator.PNG"/>
    /// </summary>
    public static class NamespaceDoc
    {
    }*/


    public class PerformanceValidator : Validator
    {
        //Store errors
        //1 - message
        //2 - warning
        //3 - error
        private List<KeyValuePair<String, Int32>> errors = null;
        private List<string> listLog = null;

        public PerformanceValidator()
        {
            errors = null;
            listLog = null;
        }

        private void log(String s, Int32 i)
        {
            if (errors != null)
            {
                errors.Add(new KeyValuePair<string, int>(s, i));
            }
        }

        /// <summary>
        /// Inicia a validacao de um ou mais diagramas.
        /// </summary>
        public override List<KeyValuePair<String, Int32>> Validate(List<GeneralUseStructure> listModelStructure, String fileName)
        {
            //TODO Create an UmlModel reference variable, cast model to UmlModel type and refactor method to use new variable.
            errors = new List<KeyValuePair<String, Int32>>();
            UmlModel model = listModelStructure.OfType<UmlModel>().FirstOrDefault();

            if (listModelStructure.OfType<UmlUseCaseDiagram>().Count() > 1)
            {
                log("[WARNING] There must be only one UseCase Diagram.", 2);
            }

            //Looking for diagrams inside given model
            foreach (UmlDiagram diagram in model.Diagrams)
            {
                //validate by diagram type
                if (diagram is UmlUseCaseDiagram)
                {
                    ValidateUseCaseDiagram(model, diagram);
                }
                else if (diagram is UmlActivityDiagram)
                {
                    ValidateActivityDiagram(diagram);
                }
            }

            //locate reference cycles
            foreach (UmlActivityDiagram diagram in model.Diagrams.OfType<UmlActivityDiagram>())
            {
                Stack<UmlActivityDiagram> visitedDiagrams = new Stack<UmlActivityDiagram>();
                visitedDiagrams.Push(diagram);
                HasCircularReference(visitedDiagrams, model, diagram);
            }

            //list log is null when model doesn't have a loop
            if (listLog != null)
            {
                foreach (string item in listLog)
                {
                    log(item, 3);
                }
                listLog = null;
            }
            else
            {
                log("[ERROR] An error was found while parsing XML file.", 3);
            }
            return errors;
        }

        private void ValidateUseCaseDiagram(UmlModel model, UmlDiagram diagram)
        {
            //checks for actors
            if (diagram.UmlObjects.OfType<UmlActor>().Count() < 1)
            {
                log("[ERROR] Missing actor in " + HttpUtility.UrlDecode(diagram.Name) + ". At least 1 actor is required to continue.", 3);
            }

            //checks for Use Cases 
            if (diagram.UmlObjects.OfType<UmlUseCase>().Count() < 1)
            {
                log("[ERROR] No use case found in " + HttpUtility.UrlDecode(diagram.Name) + ". At least 1 use case is required to continue.", 3);
            }

            //validate diagram's elements
            foreach (UmlBase item in diagram.UmlObjects)
            {
                //validate by type
                if (item is UmlUseCase)
                {
                    ValidateUseCase(model, diagram, item);
                }

                if (item is UmlAssociation)
                {
                    ValidateAssociation(diagram, item);
                }

                if (item is UmlActor)
                {
                    ValidateActor(diagram, item);
                }
            }
        }

        private void ValidateUseCase(UmlModel model, UmlDiagram diagram, UmlBase item)
        {
            UmlUseCase uCase = (UmlUseCase)item;

            bool existeAC = false;
            foreach (UmlActivityDiagram actD in model.Diagrams.OfType<UmlActivityDiagram>())
            {
                if (actD.Name == uCase.Name)
                {
                    existeAC = true;
                    break;
                }
            }

            if (!existeAC)
            {
                log("[ERROR] Missing activity diagram for \"" + HttpUtility.UrlDecode(uCase.Name) + "\" use case.", 3);
            }
        }

        private void ValidateActor(UmlDiagram diagram, UmlBase item)
        {
            UmlActor actor = (UmlActor)item;

            //store required tags for Actor element
            String[] requiredTags = new String[] { "TDHOST", "TDRAMPUPTIME", "TDRAMPDOWNTIME", "TDPOPULATION" };
            foreach (String tag in requiredTags)
            {
                String value = actor.GetTaggedValue(tag);

                if (value == null)
                {
                    log("[ERROR] Missing " + tag + " in " + HttpUtility.UrlDecode(diagram.Name) + " # " + HttpUtility.UrlDecode(actor.Name) + " element. Using empty string as value.", 3);
                }
                else
                {
                    switch (tag)
                    {
                        case "TDHOST":
                            if (tag.Length < 1)
                                log("[ERROR] Tag {" + tag + "} has no valid value for Actor {" + HttpUtility.UrlDecode(actor.Name) + "}. Found at {" + HttpUtility.UrlDecode(diagram.Name) + "}.", 3);
                            break;
                        default:
                            Double val = 0;
                            try
                            {
                                val = Convert.ToDouble(actor.GetTaggedValue(tag));
                            }
                            catch
                            {
                                log("[ERROR] Tag {" + tag + "} has no valid value for Actor {" + HttpUtility.UrlDecode(actor.Name) + "}. Found at {" + HttpUtility.UrlDecode(diagram.Name) + "}.", 3);
                            }
                            if (val <= 0)
                            {
                                log("[ERROR] Tag {" + tag + "} has no valid value for Actor {" + HttpUtility.UrlDecode(actor.Name) + "}. Found at {" + HttpUtility.UrlDecode(diagram.Name) + "}.", 3);
                            }
                            break;
                    }
                }
            }
        }

        private void ValidateAssociation(UmlDiagram diagram, UmlBase item)
        {
            UmlAssociation association = (UmlAssociation)item;

            if (association.End1 == null || association.End2 == null)
            {
                log("[ERROR] Association " + HttpUtility.UrlDecode(diagram.Name) + "#" + HttpUtility.UrlDecode(association.Name) + " is invalid.", 3);
            }
            else if (association.End1 is UmlActor && association.End2 is UmlActor)
            {
                log("[ERROR] Actors cannot be connected together. Found at " + HttpUtility.UrlDecode(association.End1.Name) + "#" + HttpUtility.UrlDecode(association.End2.Name), 3);
            }
            else if (association.End1 is UmlUseCase && association.End2 is UmlUseCase)
            {
                bool isIncExt = false;
                foreach (String s in association.Stereotypes)
                {
                    if (s.Equals("Extend") || s.Equals("Include"))
                    {
                        isIncExt = true;
                        break;
                    }
                }

                if (!isIncExt)
                {
                    log("[ERROR] Use cases cannot be connected together. Found at " + HttpUtility.UrlDecode(association.End1.Name) + "#" + HttpUtility.UrlDecode(association.End2.Name), 3);
                }
            }
            //TODO: Change TDprob validation
            //else if (association.GetTaggedValue("TDprob") == null)
            //{
            //    log("[ERROR] Missing TDprob in " + HttpUtility.UrlDecode(diagram.Name) + " # " + HttpUtility.UrlDecode(association.Name) + " element.", 3);
            //}
        }

        private void ValidateActivityDiagram(UmlDiagram diagram)
        {
            //initial state must appear only once in activity diagrams.
            ValidateInitialStateCount(diagram);

            //final state must appear only once in activity diagrams.
            ValidateFinalStateCount(diagram);

            //ActionState must not have tagged values, unless jude.hyperlink
            ValidateActionStateTransitionsTags(diagram);

            //ActionState must have at least one outgoing OR incoming transition
            ValidateActionStateTransitions(diagram);

            //transition validation
            ValidateTransition(diagram);
        }

        private void ValidateActionStateTransitionsTags(UmlDiagram diagram)
        {
            foreach (UmlActionState s in diagram.UmlObjects.OfType<UmlActionState>())
            {
                if (s.TaggedValues.Count > 1)
                {
                    log("[WARNING] Invalid tagged values at {" + HttpUtility.UrlDecode(s.Name) + "}. Found at {" + HttpUtility.UrlDecode(diagram.Name) + "}.", 2);
                }
                else if (s.TaggedValues.Count == 0)
                {
                    continue;
                }
                else if (!s.TaggedValues.Keys.Contains("jude.hyperlink"))
                {
                    log("[WARNING] Invalid tagged values at {" + HttpUtility.UrlDecode(s.Name) + "}. Found at {" + HttpUtility.UrlDecode(diagram.Name) + "}.", 2);
                }
            }
        }

        private void ValidateActionStateTransitions(UmlDiagram diagram)
        {
            foreach (UmlActionState s in diagram.UmlObjects.OfType<UmlActionState>())
            {
                if (!(s is UmlFinalState))
                {
                    int cnt = diagram.UmlObjects.OfType<UmlTransition>().Where(x => x.Source.Name.Equals(s.Name)).Count();

                    switch (cnt)
                    {
                        case 0:
                            log("[ERROR] {" + HttpUtility.UrlDecode(s.Name) + "} doesn't have outgoing transition.", 3);
                            break;
                        case 1:
                            //default
                            break;
                        default:
                            if (!(s is UmlDecision || s is UmlFork))
                            {
                                log("[ERROR] {" + HttpUtility.UrlDecode(s.Name) + "} has " + cnt + " outgoing transitions.", 3);
                            }
                            break;
                    }
                }

                int cnt2 = diagram.UmlObjects.OfType<UmlTransition>().Where(x => x.Target.Name.Equals(s.Name)).Count();

                if (!(s is UmlInitialState) && (cnt2 == 0))
                {
                    log("[ERROR] {" + HttpUtility.UrlDecode(s.Name) + "} is unreacheable.", 3);
                }
            }
        }

        private void ValidateFinalStateCount(UmlDiagram diagram)
        {
            switch (diagram.UmlObjects.OfType<UmlFinalState>().Count())
            {
                case 0:
                    log("[ERROR] Missing Final State for {" + HttpUtility.UrlDecode(diagram.Name) + "} diagram.", 3);
                    break;
                case 1:
                    //default
                    break;
                default:
                    log("[ERROR] Duplicated Final State found in {" + HttpUtility.UrlDecode(diagram.Name) + "} diagram.", 3);
                    break;
            }
        }

        private void ValidateInitialStateCount(UmlDiagram diagram)
        {
            switch (diagram.UmlObjects.OfType<UmlInitialState>().Count())
            {
                case 0:
                    log("[ERROR] Missing Initial State for {" + HttpUtility.UrlDecode(diagram.Name) + "} diagram.", 3);
                    break;
                case 1:
                    //default
                    break;
                default:
                    log("[ERROR] Duplicated Initial State found in {" + HttpUtility.UrlDecode(diagram.Name) + "} diagram.", 3);
                    break;
            }
        }

        private void ValidateTransition(UmlDiagram diagram)
        {
            foreach (UmlTransition transition in diagram.UmlObjects.OfType<UmlTransition>())
            {
                //target cannot be null
                if (transition.Target == null)
                {
                    log("[ERROR] Missing target of transition {" + HttpUtility.UrlDecode(transition.Name) + "}. Found at {" + HttpUtility.UrlDecode(diagram.Name) + "}.", 3);
                }

                //source cannot be null
                if (transition.Source == null)
                {
                    log("[ERROR] Missing source of transition {" + HttpUtility.UrlDecode(transition.Name) + "}. Found at {" + HttpUtility.UrlDecode(diagram.Name) + "}.", 3);
                }

                //transition pointing to pseudo-state cannot be tagged
                if (transition.Target is UmlPseudoState)
                {
                    if (transition.TaggedValues.Count > 0)
                    {
                        log("[ERROR] Transition {" + HttpUtility.UrlDecode(transition.Name) + "} points to pseudo-state and may not be tagged. Found at {" + HttpUtility.UrlDecode(diagram.Name) + "}.", 3);
                    }
                }
                //transition pointing to linked states cannot be tagged
                else if (transition.Target is UmlActionState && !String.IsNullOrEmpty(transition.Target.GetTaggedValue("jude.hyperlink")))
                {
                    if (transition.TaggedValues.Count > 0)
                    {
                        log("[ERROR] Transition {" + HttpUtility.UrlDecode(transition.Name) + "} points to linked state and may not be tagged. Found at {" + HttpUtility.UrlDecode(diagram.Name) + "}.", 3);
                    }
                }
                //transition cannot point to initial state
                else if (transition.Target is UmlInitialState)
                {
                    if (transition.TaggedValues.Count > 0)
                    {
                        log("[ERROR] Transition {" + HttpUtility.UrlDecode(transition.Name) + "} points to initial state. Found at {" + HttpUtility.UrlDecode(diagram.Name) + "}.", 3);
                    }
                }
                //validate tagged values
                else if (!(transition.Target is UmlFinalState))
                {
                    ValidateTransitionTags(diagram, transition);
                }
            }
        }

        private void ValidateTransitionTags(UmlDiagram diagram, UmlTransition transition)
        {
            String[] validTagNames = { "TDACTION", "TDMETHOD", "TDEXPTIME", "TDTHINKTIME", "TDSAVEPARAMETERS", "TDCOOKIES", "TDBODY", "TDREFERER", "TDPARAMETERS" };
            //TODO: Confirm if TDreferer is a mandatory tag
            //String[] mandatoryTagNames = { "TDACTION", "TDREFERER" };
            String[] mandatoryTagNames = { "TDACTION" };
            String auxValue = "";

            //Acuse any unexpected tagged value.
            foreach (KeyValuePair<String, String> taggedValue in transition.TaggedValues)
            {
                if (!validTagNames.Contains(taggedValue.Key))
                {
                    log("[WARNING] Unexpected tag {" + taggedValue.Key + "} tagged in transition {" + transition.Source.Name + "->" + transition.Target.Name + "}. Found at {" + HttpUtility.UrlDecode(diagram.Name) + "}.", 2);
                }
                else
                {
                    switch (taggedValue.Key)
                    {
                        //TODO: Review all tags' error messages
                        case "TDMETHOD":
                            Boolean noErrors = new Boolean();
                            noErrors = ValidateTDmethod(diagram, transition, taggedValue);

                            if (!noErrors)
                            {
                                log("[WARNING] Tag {" + taggedValue.Key + "} has no valid value for transition {" + transition.Source.Name + "->" + transition.Target.Name + "}. Found at {" + HttpUtility.UrlDecode(diagram.Name) + "}.", 2);
                            }
                            break;

                        case "TDPARAMETERS":
                            noErrors = new Boolean();
                            noErrors = ValidateTDparameters(auxValue, taggedValue);

                            if (!noErrors)
                            {
                                log("[WARNING] Tag {" + taggedValue.Key + "} has no valid value for transition {" + transition.Source.Name + "->" + transition.Target.Name + "}. Found at {" + HttpUtility.UrlDecode(diagram.Name) + "}.", 2);
                            }
                            break;

                        case "TDEXPTIME":
                            noErrors = new Boolean();
                            noErrors = ValidateDoubleValue(taggedValue);

                            if (!noErrors)
                            {
                                log("[WARNING] Tag {" + taggedValue.Key + "} has no valid value for transition {" + transition.Source.Name + "->" + transition.Target.Name + "}. Found at {" + HttpUtility.UrlDecode(diagram.Name) + "}.", 2);
                            }
                            break;

                        case "TDTHINKTIME":
                            noErrors = new Boolean();
                            noErrors = ValidateDoubleValue(taggedValue);

                            if (!noErrors)
                            {
                                log("[WARNING] Tag {" + taggedValue.Key + "} has no valid value for transition {" + transition.Source.Name + "->" + transition.Target.Name + "}. Found at {" + HttpUtility.UrlDecode(diagram.Name) + "}.", 2);
                            }
                            break;

                        case "TDSAVEPARAMETERS":
                            noErrors = new Boolean();
                            noErrors = ValidateStringValue(taggedValue);

                            if (!noErrors)
                            {
                                log("[WARNING] Tag {" + taggedValue.Key + "} has no valid value for transition {" + transition.Source.Name + "->" + transition.Target.Name + "}. Found at {" + HttpUtility.UrlDecode(diagram.Name) + "}.", 2);
                            }
                            break;

                        case "TDCOOKIES":
                            noErrors = new Boolean();
                            noErrors = ValidateStringValue(taggedValue);

                            if (!noErrors)
                            {
                                log("[WARNING] Tag {" + taggedValue.Key + "} has no valid value for transition {" + transition.Source.Name + "->" + transition.Target.Name + "}. Found at {" + HttpUtility.UrlDecode(diagram.Name) + "}.", 2);
                            }
                            break;

                        case "TDBODY":
                            noErrors = new Boolean();
                            noErrors = ValidateStringValue(taggedValue);

                            if (!noErrors)
                            {
                                log("[WARNING] Tag {" + taggedValue.Key + "} has no valid value for transition {" + transition.Source.Name + "->" + transition.Target.Name + "}. Found at {" + HttpUtility.UrlDecode(diagram.Name) + "}.", 2);
                            }
                            break;
                    }
                }
            }

            //String auxURL = "http://{Server.Server}";

            //Acuse any missing tag.
            foreach (String tagName in mandatoryTagNames)
            {
                //Boolean noErrors = new Boolean();
                String value = transition.GetTaggedValue(tagName);
                if (value == null)
                {
                    log("[ERROR] Tag {" + tagName + "} is meant to be tagged in transition {" + HttpUtility.UrlDecode(transition.Name) + "}. Found at {" + HttpUtility.UrlDecode(diagram.Name) + "}.", 3);
                }
                //else
                //{
                //    noErrors = ValidateTDaction(diagram, transition, auxURL, tagName, value);
                //    if (!noErrors)
                //    {
                //        log("[ERROR] Tag {" + tagName + "} has no valid value for transition {" + HttpUtility.UrlDecode(transition.Name) + "}. Found at {" + HttpUtility.UrlDecode(diagram.Name) + "}.", 3);
                //    }
                //}
            }
        }

        //TODO: Review TDACTION and TDREFERER validation
        private Boolean ValidateTDaction(UmlDiagram diagram, UmlTransition transition, String auxURL, String tagName, String value)
        {
            String auxTD = "";
            auxTD = HttpUtility.UrlDecode(value);
            try
            {
                auxTD = auxTD.Substring(0, 22);
            }
            catch
            {
                return false;
            }

            if (!auxTD.Equals(auxURL))
            {
                return false;
            }
            return true;
        }

        private Boolean ValidateStringValue(KeyValuePair<String, String> taggedValue)
        {
            if (taggedValue.Value.Length < 1)
            {
                return false;
            }
            return true;
        }

        private Boolean ValidateDoubleValue(KeyValuePair<String, String> taggedValue)
        {
            Double val = 0;
            //TODO: Verify if there must be a different message in this case (non-numeric value in the tag) 
            try
            {
                val = Convert.ToDouble(taggedValue.Value);
            }
            catch (FormatException)
            {
                return false;
            }

            if (val <= 0)
            {
                return false;
            }
            return true;
        }

        private Boolean ValidateTDmethod(UmlDiagram diagram, UmlTransition transition, KeyValuePair<String, String> taggedValue)
        {
            if ((taggedValue.Value != "POST") && (taggedValue.Value != "GET"))
            {
                return false;
            }
            return true;
        }

        //TODO: Review TDparameters validation
        private Boolean ValidateTDparameters(String auxValue, KeyValuePair<String, String> taggedValue)
        {
            int countErrors = 0;
            String[] parameters = HttpUtility.UrlDecode(taggedValue.Value).Replace(@"\|", "#*$%&*!@7").Split('|');
            foreach (String parameter in parameters)
            {
                if (!(parameter.Contains("@@")))
                {
                    countErrors++;
                }
                else
                {
                    String aux = parameter.Replace("@@", "|");
                    String paramName = aux.Split('|')[0];
                    String paramValue = aux.Split('|')[1].Replace("#*$%&*!@7", "|");

                    if (paramName.Length < 1)
                    {
                        countErrors++;
                    }
                    else
                    {
                        auxValue = paramName + "@@" + paramValue;
                    }
                }
            }

            if (countErrors > 0)
            {
                return false;
            }

            return true;
        }

        private Boolean HasCircularReference(Stack<UmlActivityDiagram> visitedDiagrams, UmlModel model, UmlActivityDiagram actDiagram)
        {
            listLog = new List<string>();
            foreach (UmlActionState state in visitedDiagrams.Peek().UmlObjects.OfType<UmlActionState>())
            {
                if (state.GetTaggedValue("jude.hyperlink") == null)
                {
                    continue;
                }

                UmlActivityDiagram referenced = model.Diagrams.Where(x => x is UmlActivityDiagram && x.Name.Equals(state.GetTaggedValue("jude.hyperlink"))).FirstOrDefault() as UmlActivityDiagram;

                if (referenced == null)
                {
                    continue;
                }

                if (visitedDiagrams.Contains(referenced))
                {
                    string temp = "[ERROR] Circular reference found at {" + referenced.Name + "}.";
                    if (!listLog.Contains(temp))
                    {
                        listLog.Add(temp);
                    }
                    return true;
                }

                visitedDiagrams.Push(referenced);

                if (HasCircularReference(visitedDiagrams, model, referenced))
                {
                    return true;
                }
                visitedDiagrams.Pop();
            }

            return false;
        }
    }
}
