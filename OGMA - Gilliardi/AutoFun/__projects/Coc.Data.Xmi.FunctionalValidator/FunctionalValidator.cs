using System;
using System.Collections.Generic;
using System.Linq;
using Coc.Modeling.Uml;
using System.Web;
using Coc.Data.Interfaces;
using Coc.Data.ControlStructure;
using Coc.Data.ControlAndConversionStructures;

namespace Coc.Data.Xmi.FunctionalValidator
{
    /*
    /// <summary>
    /// <img src="images/Xmi.FunctionalValidator.PNG"/>
    /// </summary>
    public static class NamespaceDoc
    {
    }*/


    public class FunctionalValidator : Validator
    {
        #region Attributes
        //Store errors
        //1 - message
        //2 - warning
        //3 - error
        private List<KeyValuePair<String, Int32>> errors;
        private List<String> listLog;
        private Boolean popUp = false;
        #endregion

        #region Constructor
        public FunctionalValidator()
        {

        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Inicia a validacao de um ou mais diagramas.
        /// </summary>
        public override List<KeyValuePair<String, Int32>> Validate(List<GeneralUseStructure> listModelStructure, String fileName)
        {
            //TODO Create an UmlModel reference variable, cast model to UmlModel type and refactor method to use new variable.
            errors = new List<KeyValuePair<String, Int32>>();
            UmlModel model = listModelStructure.OfType<UmlModel>().FirstOrDefault();

            //if (model.Diagrams.OfType<UmlUseCaseDiagram>().Count() > 1)
            //{
            //    log("There must be only one UseCase Diagram.", 3);
            //}

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
                    ValidateActivityDiagram(diagram, model, fileName);
                }
            }

            //locate reference cycles
            foreach (UmlActivityDiagram diagram in model.Diagrams.OfType<UmlActivityDiagram>())
            {
                Stack<UmlActivityDiagram> visitedDiagrams = new Stack<UmlActivityDiagram>();
                visitedDiagrams.Push(diagram);
                HasCircularReference(visitedDiagrams, model, diagram);
            }

            //list log is null when model don't have a loop
            if (listLog != null)
            {
                foreach (String item in listLog)
                {
                    log(item, 3);
                }
                listLog = null;
            }
            else
            {
                log("An error was found while parsing XML file.", 3);
            }
            return errors;
        }
        #endregion

        #region Private Methods
        private void log(String s, Int32 i)
        {
            if (errors != null)
            {
                errors.Add(new KeyValuePair<String, int>(s, i));
            }
        }

        private void ValidateUseCaseDiagram(UmlModel model, UmlDiagram diagram)
        {
            foreach (UmlUseCase uCase in diagram.UmlObjects.OfType<UmlUseCase>())
            {
                ValidateUseCase(model, diagram, uCase);
            }

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

        private void ValidateUseCase(UmlModel model, UmlDiagram diagram, UmlUseCase uCase)
        {
            Boolean existeAC = false;
            String aux = uCase.GetTaggedValue("jude.hyperlink");

            if (aux != null)
            {
                foreach (UmlActivityDiagram actD in model.Diagrams.OfType<UmlActivityDiagram>())
                {
                    if ((aux == uCase.Name) && (aux == actD.Name))
                    {
                        existeAC = true;
                        break;
                    }
                }
            }

            if (!existeAC)
            {
                log("[WARNING] Missing activity diagram for { " + HttpUtility.UrlDecode(uCase.Name) + " } use case.", 2);
            }
            if (String.IsNullOrEmpty(uCase.GetTaggedValue("TDPRECONDITIONS")))
            {
                log("[WARNING] Missing TDpreConditions in " + HttpUtility.UrlDecode(diagram.Name) + " # " + HttpUtility.UrlDecode(uCase.Name) + " element.", 2);
            }
            if (String.IsNullOrEmpty(uCase.GetTaggedValue("TDPOSTCONDITIONS")))
            {
                log("[WARNING] Missing TDpostConditions in " + HttpUtility.UrlDecode(diagram.Name) + " # " + HttpUtility.UrlDecode(uCase.Name) + " element.", 2);
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
                log("[ERROR] Actors cannot be connected together. Found at" + HttpUtility.UrlDecode(association.End1.Name) + "#" + HttpUtility.UrlDecode(association.End2.Name), 3);
            }

            else if (association.End1 is UmlUseCase && association.End2 is UmlUseCase)
            {
                Boolean isIncExt = false;
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
                    log("[ERROR] Use cases cannot be connected together. Found at" + HttpUtility.UrlDecode(association.End1.Name) + "#" + HttpUtility.UrlDecode(association.End2.Name), 3);
                }
            }
        }

        private void ValidateActor(UmlDiagram diagram, UmlBase item)
        {
            UmlActor actor = (UmlActor)item;

            //store requires tags for Actor element
            String[] requiresTags = new String[] { "TDHOST" };
            foreach (String tag in requiresTags)
            {
                String value = actor.GetTaggedValue(tag);

                if (value == null)
                {
                    log("[ERROR] Missing " + tag + " in " + HttpUtility.UrlDecode(diagram.Name) + " # " + HttpUtility.UrlDecode(actor.Name) + " element.", 3);
                }
                else
                {
                    switch (tag)
                    {
                        case "TDHOST":
                            if (tag.Length < 1)
                                log("[ERROR] Tag {" + tag + "} has no valid value for Actor {" + HttpUtility.UrlDecode(actor.Name) + "}. Found at {" + HttpUtility.UrlDecode(diagram.Name) + "}.", 3);
                            break;
                    }
                }
            }
        }

        private void ValidateActivityDiagram(UmlDiagram diagram, UmlModel model, String fileName)
        {
            //initial state must appear only once in activity diagrams.
            ValidateInitialStateCount(diagram);

            //final state must appear only once in activity diagrams.
            ValidateFinalStateCount(diagram);

            //ActionState must not have tagged values, unless jude.hyperlink
            ValidateActionStateTransitionsTags(diagram, model, fileName);

            //ActionState must have at least one outgoing OR incoming transition
            ValidateActionStateTransitions(diagram);

            //transition validation
            ValidateTransition(diagram);
        }

        private void ValidateActionStateTransitions(UmlDiagram diagram)
        {
            foreach (UmlActionState s in diagram.UmlObjects.OfType<UmlActionState>())
            {
                if (!(s is UmlFinalState))
                {
                    int cnt = diagram.UmlObjects.OfType<UmlTransition>().Where(x => x.Source.Id.Equals(s.Id)).Count();

                    if (s is UmlDecision)
                    {
                        List<UmlTransition> decisionOutgoingList = new List<UmlTransition>();
                        List<String> decisionOutgoingInputList = new List<String>();
                        decisionOutgoingList = diagram.UmlObjects.OfType<UmlTransition>().Where(x => x.Source.Name.Equals(s.Name)).ToList();

                        foreach (UmlTransition t in decisionOutgoingList)
                        {
                            if (!decisionOutgoingInputList.Contains(t.GetTaggedValue("TDACTION")))
                            {
                                decisionOutgoingInputList.Add(t.GetTaggedValue("TDACTION"));
                            }
                            else
                            {
                                log("[ERROR] Decision element { " + HttpUtility.UrlDecode(s.Name) + " } has the same TDaction { " + HttpUtility.UrlDecode(t.GetTaggedValue("TDACTION")) + " } value for the outgoing transitions. Found at { " + HttpUtility.UrlDecode(diagram.Name) + " } diagram.", 3);
                            }
                        }
                    }

                    switch (cnt)
                    {
                        case 0:
                            log("[ERROR] { " + HttpUtility.UrlDecode(s.Name) + " } doesn't have outgoing transition.", 3);
                            break;
                        case 1://default...
                            break;
                        default:
                            if (!(s is UmlDecision))
                                log("[ERROR] { " + HttpUtility.UrlDecode(s.Name) + " } has {" + cnt + "} outgoing transitions.", 3);
                            break;
                    }
                }
                if (!(s is UmlInitialState) && diagram.UmlObjects.OfType<UmlTransition>().Where(x => x.Target.Name.Equals(s.Name)).Count() == 0)
                {
                    log("[ERROR] { " + HttpUtility.UrlDecode(s.Name) + " } is unreacheable.", 3);
                }
            }
        }

        private void ValidateActionStateTransitionsTags(UmlDiagram diagram, UmlModel model, String fileName)
        {
            String[] path = fileName.Split('\\');
            path = path.Where(w => w != path[path.Length - 1]).ToArray();
            String pathNew = String.Join(@"\", path);
            foreach (UmlActionState s in diagram.UmlObjects.OfType<UmlActionState>())
            {
                if (s.TaggedValues.Count > 1)
                {
                    if (s.TaggedValues.Keys.Contains("cycles"))
                    {
                        String aux = s.GetTaggedValue("jude.hyperlink");

                        if (s.TaggedValues.Count - 1 > 1)
                        {
                            log("[WARNING] Invalid tagged values at {" + HttpUtility.UrlDecode(s.Name) + "}. Found at {" + HttpUtility.UrlDecode(diagram.Name) + "}.", 2);
                        }

                        //ValidateLineExcel(diagram, model, fileName, populate, s);
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
        }

        private void ValidateFinalStateCount(UmlDiagram diagram)
        {
            switch (diagram.UmlObjects.OfType<UmlFinalState>().Count())
            {
                case 0:
                    log("[ERROR] Missing Final State for {" + HttpUtility.UrlDecode(diagram.Name) + "} diagram.", 3);
                    break;
                case 1: //default. no statement.
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
                case 1: //default. no statement.
                    break;
                default:
                    log("[ERROR] Duplicated Initial State found in {" + HttpUtility.UrlDecode(diagram.Name) + "} diagram.", 3);
                    break;
            }
        }

        private void ValidateTransition(UmlDiagram diagram)
        {
            //transition validation
            foreach (UmlTransition transition in diagram.UmlObjects.OfType<UmlTransition>())
            {
                //target cannot be null
                if (transition.Target == null)
                {
                    log("[ERROR] Missing target of Transition  {" + HttpUtility.UrlDecode(transition.Name) + "}. Found at {" + HttpUtility.UrlDecode(diagram.Name) + "}.", 3);
                }

                //source cannot be null
                if (transition.Source == null)
                {
                    log("[ERROR] Missing source of Transition {" + HttpUtility.UrlDecode(transition.Name) + "}. Found at {" + HttpUtility.UrlDecode(diagram.Name) + "}.", 3);
                }

                //transition pointing to pseudo-state cannot be tagged
                if (transition.Target is UmlPseudoState)
                {
                    if (transition.TaggedValues.Count > 0)
                    {
                        log("[WARNING] Transition {" + HttpUtility.UrlDecode(transition.Source.Name + "->" + transition.Target.Name) + "} points to pseudo-state and may not be tagged. Found at {" + HttpUtility.UrlDecode(diagram.Name) + "}.", 2);
                    }
                }
                //transition pointing to linked states cannot be tagged
                else if (transition.Target is UmlActionState && !String.IsNullOrEmpty(transition.Target.GetTaggedValue("jude.hyperlink")))
                {
                    if (transition.TaggedValues.Count > 0)
                    {
                        log("[WARNING] Transition {" + HttpUtility.UrlDecode(transition.Source.Name + "->" + transition.Target.Name) + "} points to linked state and may not be tagged. Found at {" + HttpUtility.UrlDecode(diagram.Name) + "}.", 2);
                    }
                }
                //no transition can point to initial state
                else if (transition.Target is UmlInitialState)
                {
                    if (transition.TaggedValues.Count > 0)
                    {
                        log("[WARNING] Transition {" + HttpUtility.UrlDecode(transition.Source.Name + "->" + transition.Target.Name) + "} points to initial state. Found at {" + HttpUtility.UrlDecode(diagram.Name) + "}.", 2);
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
            String[] validTagNames = { "TDACTION", "TDEXPECTEDRESULT" };
            String[] mandatoryTagNames = { "TDACTION" };
            String[] optionalTag = { "TDEXPECTEDRESULT" };

            //Acuse any unexpected tagged value.
            foreach (KeyValuePair<String, String> tagvalue in transition.TaggedValues)
            {
                if (!validTagNames.Contains(tagvalue.Key))
                {
                    log("[WARNING] Unexpected tag {" + tagvalue.Key + "} tagged in transition {" + HttpUtility.UrlDecode(transition.Source.Name + "->" + transition.Target.Name) + "}. Found at {" + HttpUtility.UrlDecode(diagram.Name) + "}.", 2);
                }

            }
            //Acuse any missing tag.
            foreach (String tagvalue in mandatoryTagNames)
            {
                String value = transition.GetTaggedValue(tagvalue);
                if (value == null)
                {
                    log("[ERROR] Missing TDaction in {" + HttpUtility.UrlDecode(transition.Source.Name + "->" + transition.Target.Name) + "}. Found at diagram {" + HttpUtility.UrlDecode(diagram.Name) + "}.", 3);
                }
                else
                {
                    //valid value by tag
                    switch (tagvalue)
                    {
                        case "TDACTION":
                            if (value.Length < 1)
                                log("[ERROR] Tag {TDaction} has no valid value for transition {" + HttpUtility.UrlDecode(transition.Source.Name + "->" + transition.Target.Name) + "}. Found at diagram {" + HttpUtility.UrlDecode(diagram.Name) + "}.", 3);
                            break;
                    }
                }
            }
            foreach (String tagvalue in optionalTag)
            {
                String value = transition.GetTaggedValue(tagvalue);
                if (value == null)
                {
                    log("[WARNING] Missing TDexpectedResult in {" + HttpUtility.UrlDecode(transition.Source.Name + "->" + transition.Target.Name) + "}. Found at diagram {" + HttpUtility.UrlDecode(diagram.Name) + "}.", 2);
                }
                else
                {
                    //valid value by tag
                    switch (tagvalue)
                    {
                        case "TDEXPECTEDRESULT":
                            if (value.Length < 1)
                                log("[WARNING] Tag {TDexpectedResult} has no valid value for transition {" + HttpUtility.UrlDecode(transition.Source.Name + "->" + transition.Target.Name) + "}. Found at diagram {" + HttpUtility.UrlDecode(diagram.Name) + "}.", 2);

                            break;
                    }
                }
            }
        }

        private void ValidateDuplicatedActivities(UmlModel model, UmlDiagram diagram)
        {
            UmlUseCaseDiagram ucDiagram = (UmlUseCaseDiagram)diagram;
            foreach (UmlUseCase useCase in ucDiagram.UmlObjects.OfType<UmlUseCase>())
            {
                UmlActivityDiagram actDiagram = model.Diagrams.OfType<UmlActivityDiagram>().Where(x => x.Name.Equals(useCase.Name)).FirstOrDefault();
                if (actDiagram != null && ContainsInclude(ucDiagram, useCase) == false)
                {
                    List<String> listActionForSingleUseCase = new List<String>();
                    ValidateDuplicatedActivitiesRecCall(actDiagram, model, listActionForSingleUseCase, useCase);
                }
            }
        }

        private void ValidateDuplicatedActivitiesRecCall(UmlActivityDiagram actDiagram, UmlModel model, List<String> listActionForSingleUseCase, UmlUseCase useCase)
        {
            List<String> repeatedActivities = new List<String>();
            foreach (UmlActionState act in actDiagram.UmlObjects.OfType<UmlActionState>())
            {
                if (!String.IsNullOrEmpty(act.GetTaggedValue("jude.hyperlink")))
                {
                    UmlActivityDiagram actDiagramSub = model.Diagrams.OfType<UmlActivityDiagram>().Where(x => x.Name.Equals(act.Name)).FirstOrDefault();
                    ValidateDuplicatedActivitiesRecCall(actDiagramSub, model, listActionForSingleUseCase, useCase);
                }
                else
                {
                    if (!(act is UmlInitialState || act is UmlFinalState))
                    {
                        if (!listActionForSingleUseCase.Contains(act.Name))
                        {
                            listActionForSingleUseCase.Add(act.Name);
                        }
                        else
                        {
                            if (!repeatedActivities.Contains(act.Name))
                            {
                                log("[ERROR] Action state { " + HttpUtility.UrlDecode(act.Name) + " } has its name duplicated. Found at { " + HttpUtility.UrlDecode(useCase.Name) + " } diagram.", 3);
                                repeatedActivities.Add(act.Name);
                            }
                        }
                    }
                }
            }
        }

        private Boolean ContainsInclude(UmlUseCaseDiagram diagram, UmlUseCase useCase)
        {
            bool IsInclude = true;
            foreach (UmlAssociation item in diagram.UmlObjects.OfType<UmlAssociation>())
            {
                if (item.End1.Id.Equals(useCase.Id) && item.End2 is UmlActor || item.End2.Id.Equals(useCase.Id) && item.End1 is UmlActor)
                {
                    IsInclude = false;
                }
            }

            return IsInclude;
        }

        private Boolean HasCircularReference(Stack<UmlActivityDiagram> visitedDiagrams, UmlModel model, UmlActivityDiagram actDiagram)
        {
            listLog = new List<String>();
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
                    String temp = "Circular reference found at {" + referenced.Name + "}.";
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
        #endregion
    }
}

