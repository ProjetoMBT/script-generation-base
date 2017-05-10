using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coc.Modeling.Uml;
using System.Web;

namespace Coc.Data.Xmi.Script
{
    public class ScriptParserOATSWeb : ScriptParser
    {
        public ScriptParserOATSWeb(TabHelper tabHelper, ScriptSequence sequence) : base(tabHelper, sequence)
        {
            this.PROTOCOL = "web";
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
                string[] actionParam = { };
                
                LinkedList<Tupla> actions = extractActionParameters(tdAction);
                

                //invoca o parsing para cada acao do objeto
                foreach (Tupla action in actions)
                {
                    tdAction = action.actionName;// acao
                    actionParam = action.parameters;//parametro da acao
                    
                    /*
                    * 1 - buildStep()
                    * 2 - buildDataBank()
                    * 3 - buildLoop()
                    * 4 - verifyObject()
                    * 5 - buildObjScript()
                    * 6 - addThink()
                    */

                    foreach (string param in actionParam)
                    {
                        script += buildDataBank(param);
                    }
                                        

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
                    script += buildObjScript(protocol, obj.ToString(), properties, tdObject, buildAction(tdAction, actionParam));
                    */
                    if (!(obj == Components.statusBar | obj == Components.alertDialog))
                    {
                        script = addThink(script, Configuration.getInstance().WaitTime);
                    }
                    

                }
            }
            catch (Exception e)
            {
                script = "";
            }

            return script;
        }
    }
}
