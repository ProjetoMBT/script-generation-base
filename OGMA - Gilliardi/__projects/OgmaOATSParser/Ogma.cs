using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Coc.Data.ControlStructure;
using Coc.Data.Interfaces;
using Coc.Modeling.Uml;
using System.Text.RegularExpressions;
using Coc.Data.ControlAndConversionStructures;

namespace OgmaOATSParser
{
    /*
     * This is Ogma, an Oracle ATS script parser built through the use of a simple Context-Free Grammar. 
     * In this header we will explain how Ogma functions in two over-arching steps. Within the code, you will
     * find more detailed descriptions of each step.
     * 
     * It is important to note that Ogma is 'not' built for performance. Our goal was to make it as readable
     * as possible so as to enable the comprehension of our approach. Therefore, bear in mind that if applied
     * to extensive scripts in daily use, it may be found lacking.
     * 
     * The reason for this component to be documented in this manner, rather than in the usual documentation 
     * format used by the development team responsible for the overarching PLeTs tool, is the development team's
     * understanding that the use of Formal Languages is not as widely understood as other areas of programming.
     * Therefore, we hope that anyone who is assigned to maintain or expand this component will be able to
     * quickly come to grips with it through these instructions.
     * 
     * 1º: The first step in Ogma's parsing method is the tokenization of a given Oracle ATS script. To do this,
     * the class Lexical Analyser is used. Within it, a single, continuous string representing the entire script
     * is broken down into tokens that are then analysed one by one, following a basic CFG description. As a result
     * of this phase, a tree structure is built with the entire derivation of each token within the script.
     * 
     * 2º: Given that the Lexical and Syntactic analyses are put together in the Lexical Analyser phase, the
     * structure that is returned from it is assumed to be syntactically correct. Therefore, all that is left is the
     * Semantic analysis of the script. By making use of the built lexicon, as well as the input script itself,
     * Ogma goes on to analyse each token in the script in regards to its semantic representation, that is, the actual
     * values are assigned to an UML model structure.
     * 
     * It is important to note that Ogma does NOT validate the values of the script. That is, given a script with a 
     * correct lexical and syntactic structure, the semantic analysis is done merely in regards of assigning values
     * from the script to an UML model. The validation of these values must be done on the UML model structure itself.
     */
    public class Ogma : Parser
    {
        private LexicalAnalyser analyser;
        private InputQueue entry;
        private String CurrentStep;
        private int StepPosition;
        private String CurrentObject;
        UmlModel model;
        UmlActivityDiagram acdiagram;
        UmlTransition lastAssociation;
        UmlActionState lastActivity;

        #region Control Methods
        public Ogma()
        {
            analyser = new LexicalAnalyser();
            CurrentStep = "";
            StepPosition = 1;
            CurrentObject = "";
        }

        /*
         * This simple method is the starting point of Ogma's parsing process, building the lexicon (lex) and
         * making a call to the first rule of the CFG.
         */
        public override StructureCollection ParserMethod(String path, ref String name, Tuple<String, Object>[] args)
        {
            StructureCollection modelAux = new StructureCollection();
            LoadFile(path);
            Tuple<ClassNode, InputQueue> result = analyser.BuildLexicon(entry);
            ClassNode lex = result.Item1;
            entry = result.Item2;
            entry.Pointer = 0;

            OGMA(lex);
            modelAux.listGeneralStructure.Add(model);
            
            //return new List<GeneralUseStructure>() { (GeneralUseStructure)model };
            return modelAux;
        }

        private void LoadFile(string path)
        {
            TextReader reader = new StreamReader(path);
            entry = new InputQueue(reader.ReadToEnd());
        }

        private void GenerateUseCaseDiagram()
        {
            UmlUseCaseDiagram ucdiagram = new UmlUseCaseDiagram();
            ucdiagram.Name = "UseCase Diagram0";
            UmlActor actor = new UmlActor();
            actor.Name = "Actor0";
            UmlUseCase uc = new UmlUseCase();
            uc.Name = "UseCase0";
            UmlAssociation assoc = new UmlAssociation();
            assoc.End1 = actor;
            assoc.End2 = uc;
            ucdiagram.UmlObjects.Add(actor);
            ucdiagram.UmlObjects.Add(uc);
            ucdiagram.UmlObjects.Add(assoc);
            model.AddDiagram(ucdiagram);
        }
        #endregion

        private void OGMA(ClassNode lex)
        {
            model = new UmlModel("script");

            GenerateUseCaseDiagram();

            acdiagram = new UmlActivityDiagram("UseCase0");
            UmlInitialState initial = new UmlInitialState();
            initial.Name = "InitialNode";
            acdiagram.UmlObjects.Add(initial);

            model.AddDiagram(acdiagram);

            LIST_IMPORT(lex.Derivations[0]);
            CLASS(lex.Derivations[1]);

            String currentActionValue = lastAssociation.GetTaggedValue("TDACTION");
            lastAssociation.SetTaggedValue("TDACTION", currentActionValue + "");
            acdiagram.UmlObjects.Add(lastAssociation);

            UmlFinalState final = new UmlFinalState();
            final.Name = "FinalNode";
            acdiagram.UmlObjects.Add(final);
            lastAssociation = new UmlTransition();
            lastAssociation.Source = lastActivity;
            lastAssociation.Target = (UmlElement)acdiagram.UmlObjects[acdiagram.UmlObjects.Count - 1];
            acdiagram.UmlObjects.Add(lastAssociation);
        }

        private void LIST_IMPORT(ClassNode lex)
        {
            if (lex.Derivations[0].Type != Classes.Epsilon)
            {
                IMPORT(lex.Derivations[0]);
                LIST_IMPORT(lex.Derivations[1]);
            }
        }

        private void IMPORT(ClassNode lex)
        {
            entry.Pop();
            IDENTIFIER(lex.Derivations[1]);
            IDENTIFIER2(lex.Derivations[2]);
            entry.Pop();
        }

        private void IDENTIFIER(ClassNode lex)
        {
            entry.Pop();
        }

        private void IDENTIFIER2(ClassNode lex)
        {
            if (lex.Derivations[0].Type != Classes.Epsilon)
            {
                entry.Pop();
                IDENTIFIER(lex.Derivations[1]);
                IDENTIFIER3(lex.Derivations[2]);
            }
        }

        private void IDENTIFIER3(ClassNode lex)
        {
            if (lex.Derivations[0].Type != Classes.Identifier2)
            {
                entry.Pop();
                entry.Pop();
            }
            else
                IDENTIFIER2(lex.Derivations[0]);
        }

        private void CLASS(ClassNode lex)
        {
            CLASS_DECLARATION(lex.Derivations[0]);
            entry.Pop();
            BODY(lex.Derivations[2]);
            entry.Pop();
        }

        private void CLASS_DECLARATION(ClassNode lex)
        {
            for (int i = 0; i < 5; i++)
            {
                entry.Pop();
            }
        }

        private void BODY(ClassNode lex)
        {
            SCRIPT_SERVICE(lex.Derivations[0]);
            METHODS(lex.Derivations[1]);
        }

        private void SCRIPT_SERVICE(ClassNode lex)
        {
            if (lex.Derivations[0].Type != Classes.Epsilon)
            {
                entry.Pop();
                IDENTIFIER(lex.Derivations[1]);
                IDENTIFIER2(lex.Derivations[2]);
                IDENTIFIER(lex.Derivations[3]);
                entry.Pop();
                SCRIPT_SERVICE(lex.Derivations[5]);
            }
        }

        private void METHODS(ClassNode lex)
        {
            JAVADOC(lex.Derivations[0]);
            METHODS2(lex.Derivations[1]);
        }

        private void METHODS2(ClassNode lex)
        {
            if (lex.Derivations[0].Type != Classes.Epsilon)
            {
                METHOD(lex.Derivations[0]);
                METHODS(lex.Derivations[1]);
            }
        }

        private String ANY(ClassNode lex)
        {
            return entry.Pop();
        }

        private String ANY2(ClassNode lex)
        {
            String toReturn = "";

            if (lex.Derivations[0].Type != Classes.Epsilon)
            {
                toReturn = ANY_JAVADOC(lex.Derivations[0]);
                toReturn += ANY2(lex.Derivations[1]);
            }

            return toReturn;
        }

        private String ANY3(ClassNode lex)
        {
            String toReturn = "";

            if (lex.Derivations[0].Type != Classes.Epsilon)
            {
                toReturn = ANY_JAVADOC(lex.Derivations[0]);
                toReturn += ANY3(lex.Derivations[1]);
            }

            return toReturn;
        }

        private String ANY_JAVADOC(ClassNode lex)
        {
            return entry.Pop();
        }

        private void JAVADOC(ClassNode lex)
        {
            if (lex.Derivations[0].Type != Classes.Epsilon)
            {
                entry.Pop();
                ANY2(lex.Derivations[1]);
                entry.Pop();
            }
        }

        private void METHOD(ClassNode lex)
        {
            METHOD_DECLARATION(lex.Derivations[0]);
            entry.Pop();
            BLOCK(lex.Derivations[2]);
            entry.Pop();
        }

        private void METHOD_DECLARATION(ClassNode lex)
        {
            for (int i = 0; i < 3; i++)
            {
                entry.Pop();
            }
            METHOD_DECLARATION2(lex.Derivations[3]);
        }

        private void METHOD_DECLARATION2(ClassNode lex)
        {
            if (lex.Derivations[0].Type != Classes.Epsilon)
            {
                entry.Pop();
                entry.Pop();
            }
        }

        private void BLOCK(ClassNode lex)
        {
            switch (lex.Derivations[0].Type)
            {
                case Classes.Script_Element:
                    SCRIPT_ELEMENT(lex.Derivations[0]);
                    THINK(lex.Derivations[1]);
                    BLOCK(lex.Derivations[2]);
                    break;

                case Classes.Step:
                    STEP(lex.Derivations[0]);
                    BLOCK(lex.Derivations[1]);
                    break;
            }
        }

        private void THINK(ClassNode lex)
        {
            if (lex.Derivations[0].Type != Classes.Epsilon)
            {
                for (int i = 0; i < 3; i++)
                {
                    entry.Pop();
                }

                NUMBER(lex.Derivations[3]);

                for (int i = 0; i < 3; i++)
                {
                    entry.Pop();
                }
            }
        }

        private void NUMBER(ClassNode lex)
        {
            entry.Pop();
            NUMBER2(lex.Derivations[1]);
        }

        private void NUMBER2(ClassNode lex)
        {
            if (lex.Derivations[0].Type != Classes.Epsilon)
            {
                entry.Pop();
                entry.Pop();
            }
        }

        private void STEP(ClassNode lex)
        {
            BEGIN_STEP(lex.Derivations[0]);

            entry.Pop();

            ELEMENT_SEQUENCE(lex.Derivations[2]);

            entry.Pop();

            CLOSE_STEP(lex.Derivations[4]);
        }

        private void BEGIN_STEP(ClassNode lex)
        {
            entry.Pop();
            entry.Pop();

            STEP_NAME(lex.Derivations[2]);

            entry.Pop();

            NUMBER(lex.Derivations[4]);

            entry.Pop();
            entry.Pop();
        }

        private void STEP_NAME(ClassNode lex)
        {
            entry.Pop();

            CurrentStep = ANY3(lex.Derivations[1]);

            entry.Pop();
        }

        private void CLOSE_STEP(ClassNode lex)
        {
            entry.Pop();
            CurrentStep = "";
            StepPosition = 1;
        }

        private void ELEMENT_SEQUENCE(ClassNode lex)
        {
            if (lex.Derivations[0].Type != Classes.Epsilon)
            {
                SCRIPT_ELEMENT(lex.Derivations[0]);
                THINK(lex.Derivations[1]);
                ELEMENT_SEQUENCE(lex.Derivations[2]);
            }
        }

        private String ELEMENT_NAME(ClassNode lex)
        {
            String toReturn = "";

            for (int i = 0; i < 3; i++)
            {
                entry.Pop();
            }

            ANY(lex.Derivations[3]);
            entry.Pop();
            ANY(lex.Derivations[5]);
            entry.Pop();

            toReturn = ANY(lex.Derivations[7]);

            for (int i = 0; i < 3; i++)
            {
                entry.Pop();
            }

            return toReturn;
        }

        private String ACTION(ClassNode lex)
        {
            String action = ANY(lex.Derivations[0]);
            String tdAction = "{";

            Regex toCheck = new Regex(@"^set");
            if (toCheck.Match(action).Success)
            {
                String[] dtName = CurrentObject.Split(';')[1].Split('}')[0].Split('_');
                tdAction += "set;dt_" + dtName[dtName.Length - 1] + "}";
            }
            else
            {
                tdAction += action + "}";
            }

            entry.Pop();
            ACTION2(lex.Derivations[2]);
            entry.Pop();

            return tdAction;
        }

        private String ACTION(ClassNode lex, String newCurrentObject)
        {
            String action = ANY(lex.Derivations[0]);
            String tdAction = "{";

            Regex toCheck = new Regex(@"^set");
            if (toCheck.Match(action).Success)
            {
                String[] dtName = newCurrentObject.Split(';')[1].Split('}')[0].Split('_');
                tdAction += "set;dt_" + dtName[dtName.Length - 1] + "}";
            }
            else
            {
                tdAction += action + "}";
            }

            entry.Pop();
            ACTION2(lex.Derivations[2]);
            entry.Pop();

            return tdAction;
        }

        private void ACTION2(ClassNode lex)
        {
            switch (lex.Derivations[0].Type)
            {
                case Classes.Action:
                    ACTION(lex.Derivations[0]);
                    break;

                case Classes.Any:
                    ANY(lex.Derivations[0]);
                    break;

                case Classes.Reserved_Word:
                    entry.Pop();
                    ANY3(lex.Derivations[1]);
                    entry.Pop();
                    break;
            }
        }

        private void SCRIPT_ELEMENT(ClassNode lex)
        {
            switch (lex.Derivations[0].Type)
            {
                case Classes.Web_Window:
                    WEB_WINDOW(lex.Derivations[0]);
                    break;
                case Classes.Web_Textbox:
                    WEB_TEXTBOX(lex.Derivations[0]);
                    break;
                case Classes.Web_Button:
                    WEB_BUTTON(lex.Derivations[0]);
                    break;
                case Classes.Web_Image:
                    WEB_IMAGE(lex.Derivations[0]);
                    break;
                case Classes.Web_Alert_Dialog:
                    WEB_ALERT_DIALOG(lex.Derivations[0]);
                    break;
                case Classes.Web_Link:
                    WEB_LINK(lex.Derivations[0]);
                    break;
                case Classes.Browser_Launch:
                    BROWSER_LAUNCH(lex.Derivations[0]);
                    break;
                case Classes.Web_Element:
                    WEB_ELEMENT(lex.Derivations[0]);
                    break;
                case Classes.Web_Select_Box:
                    WEB_SELECT_BOX(lex.Derivations[0]);
                    break;
                case Classes.Web_Dialog:
                    WEB_DIALOG(lex.Derivations[0]);
                    break;
                case Classes.Web_Radio_Button:
                    WEB_RADIO_BUTTON(lex.Derivations[0]);
                    break;
                case Classes.Web_Check_Box:
                    WEB_CHECK_BOX(lex.Derivations[0]);
                    break;
            }
        }

        private void ELEMENT_DETAILS(ClassNode lex, String tdObject)
        {
            entry.Pop();
            NUMBER(lex.Derivations[1]);
            entry.Pop();
            tdObject += ELEMENT_NAME(lex.Derivations[3]) + "}";
            entry.Pop();
            entry.Pop();

            String tdAction = ACTION(lex.Derivations[6], tdObject);

            if (!tdObject.Split(';')[0].Equals("{window"))
            {
                if (tdObject.Equals(CurrentObject))
                {
                    String currentActionValue = lastAssociation.GetTaggedValue("TDACTION");

                    lastAssociation.SetTaggedValue("TDACTION", currentActionValue + "," + tdAction);
                }
                else
                {
                    UmlActionState newActivity = new UmlActionState();
                    newActivity.Name = CurrentStep + StepPosition;
                    StepPosition++;
                    CurrentObject = tdObject;
                    acdiagram.UmlObjects.Add(newActivity);

                    if (lastAssociation == null)
                    {
                        lastAssociation = new UmlTransition();
                        lastAssociation.Source = (UmlElement)acdiagram.UmlObjects[0];
                    }
                    else
                    {
                        String currentActionValue = lastAssociation.GetTaggedValue("TDACTION");
                        lastAssociation.SetTaggedValue("TDACTION", currentActionValue + "");
                        acdiagram.UmlObjects.Add(lastAssociation);

                        lastAssociation = new UmlTransition();
                        lastAssociation.Source = (UmlElement)lastActivity;
                    }

                    lastAssociation.Target = (UmlElement)newActivity;
                    lastAssociation.SetTaggedValue("TDOBJECT", tdObject);
                    lastAssociation.SetTaggedValue("TDACTION", "" + tdAction);

                    lastActivity = newActivity;
                }
            }
        }

        private void WEB_WINDOW(ClassNode lex)
        {
            entry.Pop();
            String tdObject = "{window;";
            ELEMENT_DETAILS(lex.Derivations[1], tdObject);
            entry.Pop();
        }

        private void WEB_TEXTBOX(ClassNode lex)
        {
            entry.Pop();
            String tdObject = "{textbox;";
            ELEMENT_DETAILS(lex.Derivations[1], tdObject);
            entry.Pop();
        }

        private void WEB_BUTTON(ClassNode lex)
        {
            entry.Pop();
            String tdObject = "{button;";
            ELEMENT_DETAILS(lex.Derivations[1], tdObject);
            entry.Pop();
        }

        private void WEB_IMAGE(ClassNode lex)
        {
            entry.Pop();
            String tdObject = "{image;";
            ELEMENT_DETAILS(lex.Derivations[1], tdObject);
            entry.Pop();
        }

        private void WEB_ALERT_DIALOG(ClassNode lex)
        {
            entry.Pop();
            String tdObject = "{alertdialog;";
            ELEMENT_DETAILS(lex.Derivations[1], tdObject);
            entry.Pop();
        }

        private void WEB_LINK(ClassNode lex)
        {
            entry.Pop();
            String tdObject = "{link;";
            ELEMENT_DETAILS(lex.Derivations[1], tdObject);
            entry.Pop();
        }

        private void WEB_ELEMENT(ClassNode lex)
        {
            entry.Pop();
            String tdObject = "{element;";
            ELEMENT_DETAILS(lex.Derivations[1], tdObject);
            entry.Pop();
        }

        private void WEB_SELECT_BOX(ClassNode lex)
        {
            entry.Pop();
            String tdObject = "{selectbox;";
            ELEMENT_DETAILS(lex.Derivations[1], tdObject);
            entry.Pop();
        }

        private void WEB_DIALOG(ClassNode lex)
        {
            entry.Pop();
            String tdObject = "{dialog;";
            ELEMENT_DETAILS(lex.Derivations[1], tdObject);
            entry.Pop();
        }

        private void WEB_RADIO_BUTTON(ClassNode lex)
        {
            entry.Pop();
            String tdObject = "{radiobutton;";
            ELEMENT_DETAILS(lex.Derivations[1], tdObject);
            entry.Pop();
        }

        private void WEB_CHECK_BOX(ClassNode lex)
        {
            entry.Pop();
            String tdObject = "{checkbox;";
            ELEMENT_DETAILS(lex.Derivations[1], tdObject);
            entry.Pop();
        }

        private void BROWSER_LAUNCH(ClassNode lex)
        {
            for (int i = 0; i < 4; i++)
            {
                entry.Pop();
            }
        }
    }
}