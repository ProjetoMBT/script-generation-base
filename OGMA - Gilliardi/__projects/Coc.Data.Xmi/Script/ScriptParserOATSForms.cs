using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coc.Modeling.Uml;
using System.Web;
using System.Globalization;

namespace Coc.Data.Xmi.Script
{
    public class ScriptParserOATSForms : ScriptParser
    {
        public ScriptParserOATSForms(TabHelper tabHelper, ScriptSequence sequence) : base(tabHelper, sequence)
        {
            this.PROTOCOL = "forms";
        }

        public string parse_(UmlTransition transition)
        {
            string script = "";
            try
            {
                Dictionary<TagNames, string> tags = extractTransitionTags(transition);
                string protocol = tags[TagNames.TDProtocol];
                
                if (!protocol.Equals(PROTOCOL))
                {
                    throw new Exception("Invalid protocol: " + protocol);
                }

                string tagTDObject = tags[TagNames.TDObject];
                string tagTDAction = tags[TagNames.TDAction];
                string tagTDProperties = tags[TagNames.TDProperties];
                string tagTDwait = tags[TagNames.TDWait];
                string tagTDIterations = tags[TagNames.TDIterations];
                bool tagTDVerify = bool.Parse(tags[TagNames.TDVerify]);

                //captura o objeto
                Tuple<Components, string> objParams = extractObject(tagTDObject);
                Components obj = objParams.Item1;
                string tdObject = objParams.Item2;
                string tdAction = tagTDAction;
                string properties = tagTDProperties;
                string[] actionParam = {};

                LinkedList<Tupla> actions = extractActionParameters(tdAction);

                //invoca o parsing para cada acao do objeto
                foreach (Tupla action in actions)
                {
                    tdAction = action.actionName;// acao
                    actionParam = action.parameters;//parametro da acao
                    
                    if (tagTDVerify)
                    {
                        //constroi o script para o objeto sem a acao pois a acao de verificacao 
                        //deve ser o metodo ".exists()"
                        /*script += verifyObject(
                            buildObjScript(obj.ToString(), properties, tdObject, buildAction("exists", null))
                        );*/

                        //verifica este objeto apenas uma vez para todas suas acoes
                        tagTDVerify = false;

                    }
                    /*
                    script += buildObjScript(obj.ToString(), properties, tdObject, buildAction(tdAction, actionParam));
                    */
                    if (!(obj == Components.statusBar | obj == Components.alertDialog))
                    {
                        script = addThink(script, Configuration.getInstance().WaitTime);
                    }
                    
                }
            }
            catch (Exception)
            {
                script = "";
            }

            return script;
        }

        #region TRATAMENTO ESPECIFICO PARA CADA OBJETO
        /*
        private string FORMS_INVALID(Components TDObject, string properties, string objectName, string[] actionParameter)
        {
            return "";
        }

        private string FORMS_SCREENSHOT(Components TDObject, string properties, string objectName, string[] actionParameter)
        {
            string script = "";
            try
            {
                //forms.captureScreenshot(41);
                script = buildObjScript("captureScreenshot", properties, objectName, buildAction(objectName, actionParameter));

            }
            catch (Exception e)
            {
                script = "";
            }

            return script;
        }

        private string FORMS_TEXT(Components TDObject, string properties, string objectName, string[] actionParameter)
        {
            string script = "";
            try
            {


            }
            catch (Exception e)
            {

            }

            return script;
        }

        private string FORMS_BUTTON(Components TDObject, string properties, string objectName, string[] actionParameters)
        {
            string script = "";
            try
            {
                //forms.button(7, "{{obj.Test.forms_button_PROJECT_FIND_FIND_0}}").click();
                script = buildObjScript("button", properties, objectName, buildAction(objectName, new string[] { "" }));

            }
            catch (Exception e)
            {
                script = "";
            }

            return script;
        }

        private string FORMS_CALENDAR(Components TDObject, string properties, string objectName, string[] actionParameters)
        {
            string script = "";
            try
            {
                //forms.calendar(24, "{{obj.Test.forms_calendar}}").enter("29-MAY-2015");
                script = buildObjScript("calendar", properties, objectName, buildAction(objectName, actionParameters));

            }
            catch (Exception e)
            {
                script = "";
            }

            return script;
        }

        private string FORMS_WINDOW(Components TDObject, string properties, string objectName, string[] actionParameters)
        {

            string script = "";
            try
            {
                //forms.window(8, "{{obj.Test.forms_window_PROJECT_FIND}}").activate(true);
                script = buildObjScript("window", properties, objectName, buildAction(objectName, actionParameters));

            }
            catch (Exception e)
            {
                script = "";
            }

            return script;
        }

        private string FORMS_LISTVALUES(Components TDObject, string properties, string objectName, string[] actionParameters)
        {
            string script = "";
            try
            {
                //forms.listOfValues(9, "{{obj.Test.forms_listOfValues}}").select("Contract Generic US|T.Cont.Generic.US.USD|US-US_CORP_USD_OU");
                script = buildObjScript("listOfValues", properties, objectName, buildAction(objectName, actionParameters));

            }
            catch (Exception e)
            {
                script = "";
            }

            return script;
        }

        private string FORMS_STATUSBAR(Components TDObject, string properties, string objectName, string[] actionParameters)
        {
            string script = "";
            try
            {
                //forms.statusBar(98, "{{obj.Error.forms_statusBar}}")
                //.assertText("FormsFT AutoValidation: Verify StatusBar text value", 
                //"FRM-40400: Transaction complete: 1 records applied and saved.", MatchOption.Exact);
                script = buildObjScript("statusBar", properties, objectName, buildAction(objectName, actionParameters));

            }
            catch (Exception e)
            {
                script = "";
            }

            return script;
        }

        private string FORMS_DIALOG(Components TDObject, string properties, string objectName, string[] actionParameters)
        {
            string script = "";
            try
            {
                //forms.alertDialog(103, "{{obj.Error.forms_alertDialog}}").clickYes();
                script = buildObjScript("alertDialog", properties, objectName, buildAction(objectName, actionParameters));

            }
            catch (Exception e)
            {
                script = "";
            }

            return script;
        }
        */
        #endregion
    }
}
