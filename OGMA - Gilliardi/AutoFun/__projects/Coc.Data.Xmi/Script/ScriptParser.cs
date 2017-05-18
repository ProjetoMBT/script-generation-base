using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using Coc.Modeling.Uml;
using System.Web;
using System.Text.RegularExpressions;
using System.Threading;

namespace Coc.Data.Xmi.Script
{
    /*
    /// <summary>
    /// <img src="images/Xmi.Script.PNG"/>
    /// </summary>
    public static class NamespaceDoc
    {
    }*/




    /// <summary>
    /// Classe responsavel por construir o script com as tags que foram anotadas no modelo.
    /// </summary>
    public class ScriptParser
    {
        public struct Tupla
        {
            public string actionName;
            public string[] parameters;
        }


        public struct Databank
        {
            public string name;
            public string path;
            public string repoName;
        }



        /// <summary>
        /// Esta enumeracao contem o nome das tags presentes no modelo
        /// </summary>
        public enum TagNames
        {
            TDAction,
            TDAssert,
            TDLoopCondition,
            TDIterations,
            TDObject,
            TDProperties,
            TDProtocol,
            TDVerify,
            TDWait,
            paramcycle
        }


        /// <summary>
        /// Esta enumeracao contem os nomes dos objetos que existem no script do OATS.
        /// </summary>
        public enum Components
        {
            Invalid,
            browser,
            captureScreenshot,
            forms,
            
            abstractWindow,
            alertDialog,
            appletAdapter,
            blockScroller,
            button,
            calendar,
            checkBox,
            choiceBox,
            comboBox,
            editBox,
            editorDialog,
            flexWindow,
            hGridApp,
            helpDialog,
            imageItem,
            infoBox,
            list,
            listOfValues,
            logonDialog,
            otsHGrid,
            oracleForms,
            radioButton,
            responseBox,
            schedulingDataClient,
            spreadTable,
            statusBar,
            tab,
            tableBox,
            textField,
            tree,
            treeList,
            window,

            element,
            link,
            textBox
        }

        protected string PROTOCOL;
        protected TabHelper tabHelper;
        protected ScriptSequence sequence;

        private int counter;
        private Dictionary<string, string> counters;
        private Dictionary<string, Databank> databanks;

        //([\\[|\\{][\\W\\w]+.[\\W\\w]+[\\]|\\}])
        private Regex dataBankRegExp = new Regex("[\\[|\\{]db\\.[\\w|\\d|]+\\.[\\w|\\d|]+[\\]|\\}]");
        

        /// <summary>
        /// Instancia um objeto ScriptParser informando o nivel de tabs e sequencia 
        /// do script que devem ser utilizados para a construcao do script.
        /// </summary>
        /// <param name="tabHelper">Objeto que ira auxiliar na identacao do codigo do script.</param>
        /// <param name="sequence">Objeto que ira informar e controlar 
        /// a sequencia das acoes e passos no script.</param>
        public ScriptParser(TabHelper tabHelper, ScriptSequence sequence)
        {
            this.tabHelper = tabHelper;
            this.sequence = sequence;
            this.counter = 0;
            this.databanks = new Dictionary<string, Databank>();
        }

        public TabHelper TabHelper
        {
            get { return tabHelper; }
        }

        public ScriptSequence Sequence
        {
            get { return sequence; }
        }

        protected string addThink(string script, double time)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                if (time <= 0)
                {
                    throw new Exception("Parametro time invalido: " + time);
                }

                tabHelper.incrementTabs();
                sb.Append(script);
                sb.AppendLine();
                sb.Append(tabHelper.TabText);
                sb.AppendLine("{");
                tabHelper.incrementTabs();
                sb.Append(tabHelper.TabText);
                sb.AppendLine(string.Format("think({0});", time.ToString(CultureInfo.GetCultureInfo("EN-US"))));
                tabHelper.decrementTabs();
                sb.Append(tabHelper.TabText);
                sb.AppendLine("}");
                tabHelper.decrementTabs();//volta o tab a posicao inicial
            }
            catch (Exception e)
            {
                sb = new StringBuilder();
            }

            return sb.ToString();
        }

        /// <summary>
        /// Extrai, do conteudo da tag passado por parametro, o objeto e seu respectivo nome.
        /// </summary>
        /// <param name="tagValue">Conteudo de uma tag.</param>
        /// <returns></returns>
        protected Tuple<Components, string> extractObject(string tagValue)
        {
            string objName = "";
            Components component = Components.Invalid;
            try
            {
                //TDOBJECT	{textField;forms_textField_AGREEMENT_CUSTOMER_NUMBER}
                string[] values = tagValue.Split(new char[] { ';' });

                foreach (Components enu in Enum.GetValues(typeof(Components)))
                {
                    if (enu.ToString().Equals(values[0]))
                    {
                        component = enu;
                        break;
                    }
                }

                objName = values.Length > 1 ? values[1] : component.ToString();

            }
            catch (Exception e)
            {
                objName = "";
                component = Components.Invalid;
            }

            return Tuple.Create<Components, string>(component, objName);
        }

        /// <summary>
        /// Extrai da transicao passada por parametro, todas as tags 
        /// que foram anotadas no modelo.
        /// </summary>
        /// <param name="transition">Transicao do diagrama de atividades no modelo.</param>
        /// <returns></returns>
        protected Dictionary<TagNames, string> extractTransitionTags(UmlTransition transition)
        {
            Dictionary<TagNames, string> tags = new Dictionary<TagNames, string>();

            string tagTDAction = transition.GetTaggedValue("TDACTION");
            string tagTDAssert = transition.GetTaggedValue("TDASSERT");
            string tagTDIterations = transition.GetTaggedValue("TDITERATIONS");
            string tagTDLoopCondition = transition.GetTaggedValue("TDLOOPCONDITION");
            string tagTDObject = transition.GetTaggedValue("TDOBJECT");
            string tagTDProtocol = transition.GetTaggedValue("TDPROTOCOL");
            string tagTDProperties = transition.GetTaggedValue("TDPROPERTIES");
            string tagTDWait = transition.GetTaggedValue("TDWAIT");
            string tagTDVerify = transition.GetTaggedValue("TDVERIFY");
            string tagParamcycle = transition.GetTaggedValue("paramcycle");

            try
            {
                tagTDAction = HttpUtility.UrlDecode(tagTDAction);
                tagTDAssert = HttpUtility.UrlDecode(tagTDAssert);
                tagTDIterations = HttpUtility.UrlDecode(tagTDIterations);
                tagTDLoopCondition = HttpUtility.UrlDecode(tagTDLoopCondition);
                tagTDObject = HttpUtility.UrlDecode(tagTDObject);
                tagTDProtocol = HttpUtility.UrlDecode(tagTDProtocol);
                tagTDProperties = HttpUtility.UrlDecode(tagTDProperties);
                tagTDWait = HttpUtility.UrlDecode(tagTDWait);
                tagTDVerify = HttpUtility.UrlDecode(tagTDVerify);

                double auxTDwait = 0.0f;
                if (!string.IsNullOrEmpty(tagTDWait))
                {
                    auxTDwait = float.Parse(tagTDWait);
                }
                else
                {
                    try
                    {
                        auxTDwait = Convert.ToDouble(Configuration.getInstance().getConfiguration(Configuration.Fields.waittime));
                    }
                    catch (Exception e) { }
                }

                if (!string.IsNullOrEmpty(tagTDObject))
                {
                    tagTDObject = tagTDObject.Substring(0, tagTDObject.Length - 1).Substring(1);
                }
                else
                {
                    tagTDObject = "";
                }

                tagTDIterations = string.IsNullOrEmpty(tagTDIterations) || tagTDIterations.Equals("null") ? "0" : tagTDIterations;
                /*
                string verifyAll = Configuration.getInstance().getConfiguration("verifyall");
                verifyAll = verifyAll.Equals("") ? "false" : verifyAll;
                */
                bool tagVerifyAux =  false;
                try
                {
                    tagVerifyAux = bool.Parse(tagTDVerify);
                }
                catch (ArgumentNullException e) { }
                catch (FormatException e) { }
                /*
                try
                {
                    bool verifyAllAux = bool.Parse(verifyAll);
                    if (verifyAllAux)
                    {
                        tagVerifyAux = verifyAllAux;
                    }
                }
                catch (ArgumentNullException e) { }
                catch (FormatException e) { }
                */
                tagTDVerify = tagVerifyAux.ToString();

            }
            catch (Exception e)
            {
                tagTDAction = "";
                tagTDAssert = "";
                tagTDIterations = "";
                tagTDLoopCondition = "";
                tagTDObject = "";
                tagTDProtocol = "";
                tagTDProperties = "";
                tagTDWait = "";
                tagTDVerify = "";
            }

            tags.Add(TagNames.TDAction, tagTDAction);
            tags.Add(TagNames.TDAssert, tagTDAssert);
            tags.Add(TagNames.TDLoopCondition, tagTDLoopCondition);
            tags.Add(TagNames.TDIterations, tagTDIterations);
            tags.Add(TagNames.TDObject, tagTDObject);
            tags.Add(TagNames.TDProperties, tagTDProperties);
            tags.Add(TagNames.TDProtocol, tagTDProtocol);
            tags.Add(TagNames.TDWait, tagTDWait);
            tags.Add(TagNames.TDVerify, tagTDVerify);
            tags.Add(TagNames.paramcycle, tagParamcycle);

            return tags;
        }


        /// <summary>
        /// A partir do valor da tag TDAction, extrai todas as acoes e seus respectivos parametros,
        /// retornando uma lista no formato <ACAO, PARAMETROS[]>, caso a acao nao possua parametro
        /// o valor do parametro para o item acao sera uma string vazia.
        /// </summary>
        /// <param name="TDActionValue">Valor relacionado a tag TDAction.</param>
        /// <returns>Lista contendo as acoes e seus respectivos valores de parametro.</returns>
        protected LinkedList<Tupla> extractActionParameters(string TDActionValue)
        {
            LinkedList<Tupla> extract = new LinkedList<Tupla>();

            //TDACTION	{},{setText;"HHH"},{invokeSoftKey;"NEXT_FIELD";"test"}
            try
            {
                //separa as acoes
                Regex reg = new Regex("({[\\w|\\W|.]+?}{1,2})");
                string[] actions = reg.Split(TDActionValue);
                //string[] actions = {TDActionValue};


                //**********************************************
                foreach (string action in actions)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(action))
                        {
                            string aux_action = "";// action.Substring(0, action.Length - 1).Substring(1);
                            
                            
                            
                            if (action.StartsWith("{"))
                            {
                                aux_action = action.Substring(1, action.Length-1);
                            }

                            if (action.EndsWith("}"))
                            {
                                aux_action = aux_action.Substring(0, aux_action.Length-1);
                            }



                            //separa os parametros
                            string[] act_param = aux_action.Split(new char[] { ';' });
                            Tupla tupla;
                            tupla.actionName = act_param[0];
                            tupla.parameters = new string[act_param.Length - 1];

                            Array.Copy(act_param, 1, tupla.parameters, 0, act_param.Length - 1);

                            //se o parametro for databank coloca o parametro no formato correto
                            for (int i = 0; i < tupla.parameters.Length;i++)
                            {
                                if (dataBankRegExp.IsMatch(tupla.parameters[i]))
                                {
                                    tupla.parameters[i] = "\"{" + tupla.parameters[i].Replace("\"", "") + "}\"";
                                }
                            }
                            
                            extract.AddLast(tupla);
                        }
                    }
                    catch (Exception e)
                    {

                    }
                }
            }
            catch (Exception e)
            {
                extract = new LinkedList<Tupla>();
            }

            return extract;
        }
        
        /// <summary>
        /// Este metodo retorna uma lista contendo todos os databanks encontrados
        /// no valor da tag TDACTION.
        /// </summary>
        /// <param name="TDActionValue">Conteudo da tag TDACTION.</param>
        /// <returns></returns>
        protected List<string> extractDataBanks(string TDActionValue)
        {
            List<string> banks = new List<string>();
            LinkedList<Tupla> actions = extractActionParameters(TDActionValue);

            //adiciona a referencia para os arquivos no dicionario para que nao existam 
            //referencias para ARQUIVO.COLUNA repetidos
            foreach (Tupla t in actions)
            {
                foreach (string param in t.parameters)
                {
                    //caso o parametro seja referencia para arquivo
                    //ira retornar a chamada para um databank
                    banks.Add(buildDataBank(param));
                }
            }

            banks = banks.Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();

            return banks;
        }

        /// <summary>
        /// Este metodo realiza a construcao de um trecho do script 
        /// referente a acao a ser executada, que no caso sera a 
        /// chamada para algum metodo de um objeto qualquer. Utiliza o array de string
        /// para construir a lista de parametros que o metodo recebera como argumento. 
        /// </summary>
        /// <param name="action">Nome do metodo de um objeto que sera invocado no script.</param>
        /// <param name="actionParameters">array de string contendo os argumentos 
        /// que o metodo devera receber como parametro.</param>
        /// <returns></returns>
        protected string buildAction(string action, string[] actionParameters)
        {
            string actionReturn = "";
            try
            {
                if (!string.IsNullOrEmpty(action))
                {
                    string parameters = "";
                    if (actionParameters!= null && actionParameters.Length > 0)
                    {
                        parameters = actionParameters[0];
                        for (int i = 1; i < actionParameters.Length; i++)
                        {
                            parameters += ", " + actionParameters[i];
                        }
                    }
                    
                    if (action.Equals("captureScreenshot"))
                    {
                        actionReturn = "." + action + "(" + sequence.Sequence.ToString() + ")";
                        sequence.incrementSequence();
                    }
                    else if (action.Equals("setPassword") && actionParameters.Length == 2)
                    {
                        actionReturn = "." + action + "(" + actionParameters[0] + "(" + actionParameters[1] + "))";
                    }
                    else
                    {
                        actionReturn = "." + action + "(" + parameters + ")";
                    }
                }
            }
            catch (Exception e)
            {
                actionReturn = "";
            }

            return actionReturn;
        }

        /// <summary>
        /// Este metodo constroi o codigo que sera executado no script OATS.
        /// </summary>
        /// <param name="protocol">Protocolo do objeto(web, forms, applet).</param>
        /// <param name="obj">Tipo do objeto que ira executar a acao.</param>
        /// <param name="properties">Nome do arquivo properties que contem a biblioteda de objetos.</param>
        /// <param name="objectName">Nome do objeto que ira executar a acao.</param>
        /// <param name="action">Acao que devera ser executada pelo objeto.</param>
        /// <returns>Trecho de codigo referente aos parametros informados ou string vazia caso ocorra algum erro.</returns>
        protected string buildObjScript(string protocol, string obj, string properties, string objectName, string action)
        {
            string script = "";
            try
            {
                //forms.captureScreenshot(41);
                if (action.Contains("captureScreenshot") || (obj.Equals("browser") && action.Contains("launch")))
                {
                    tabHelper.incrementTabs();
                    script = string.Format("{0}{1}{2};",
                        tabHelper.TabText, protocol, action);
                }
                else
                {
                    //forms.textField(42,"{{obj.Error.forms_textField_AGREEMENT_CUSTOMER_NAME_0}}").invokeSoftKey("ENTER_QUERY");
                    tabHelper.incrementTabs();
                    script = string.Format("{0}{1}.{2}({3}, {4}{5}){6};",
                    //script = string.Format("{0}{1}.{2}({3}, \"{{{{obj.{4}.{5}}}}}\"){6};",
                        tabHelper.TabText, protocol, obj, sequence.Sequence, properties, objectName, action);
                    sequence.incrementSequence();
                }
            }
            catch (Exception e)
            {
                script = "";
            }
            finally
            {
                tabHelper.decrementTabs();
            }

            return script;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stepName"></param>
        /// <returns></returns>
        public string buildStep(string stepName)
        {
            sequence.incrementSequence();
            string step = string.Format("{0}beginStep(\"{1}\", {2});\n", 
                tabHelper.TabText, stepName, sequence.Sequence);

            step += tabHelper.TabText + "{{\n{0}\n" + tabHelper.TabText + "}}\n";
            step += tabHelper.TabText + "endStep();\n";
            
            return step;
        }

        /// <summary>
        /// Este metodo extrai o nome do arquivo contido no parametro de uma acao 
        /// para entao construir o script referente ao acesso do databank.
        /// </summary>
        /// <param name="actionParameter"></param>
        /// <returns>string contendo o script construido para acessar um databank 
        /// de acordo com o nome do arquivo presente no parametro da acao passada por parametro</returns>
        protected string buildDataBank(string actionParameter)
        {
            string databank = "";
            try
            {
                tabHelper.incrementTabs();
                if (dataBankRegExp.IsMatch(actionParameter))
                {
                    string fileName = dataBankRegExp.Match(actionParameter).Value;
                    //captura o nome do arquivo
                    fileName = fileName.Replace("[", "").Replace("]", "").Replace("{", "").Replace("}", "").Split(new char[] { '.' })[1];
                                                            
                    /*
                    if (!databanks.ContainsKey(fileName))
                    {
                        new Thread(() => new DatabankConfigForm(fileName).Show()).Start();

                        //new DatabankConfigForm(fileName).Show();
                        //waitForDatabankPath();

                        //databanks.Add(db.name, db);
                    }*/

                    databank = string.Format(
                        "{0}getDatabank(\"{1}\").load(\"Default\", \"DataBank/{1}.csv\", null);\n" +
                        "{0}getDatabank(\"{1}\").getNextDatabankRecord();\n",
                        tabHelper.TabText, fileName);

                }
            }
            catch (Exception e)
            {
                databank = "";
            } 
            finally
            {
                tabHelper.decrementTabs();
            }

            return databank;
        }



        private string buildAssert(string protocol, Components obj, string objName, string properties, string assertText)
        {
            string assertScript = "";
            try
            {
                tabHelper.incrementTabs();
                //web e forms suportam assert atraves de verifyAttibute
                if (!string.IsNullOrEmpty(assertText))
                {
                    assertText = assertText.Replace("\"", "");

                    if (dataBankRegExp.IsMatch(assertText))
                    {
                        assertText = "{" + assertText + "}";
                    }
                    assertScript = string.Format(
                        "\n{0}{1}.{2}({3}, \"{{{{obj.{4}.{5}}}}}\").verifyAttribute(\"Assert Test\", \"value\", \"{6}\", TestOperator.StringExact);\n\n",
                        tabHelper.TabText, protocol, obj, sequence.Sequence, properties, objName, assertText);
                    
                    sequence.incrementSequence();
                }
            }
            catch (Exception e)
            {
                assertScript = "";
            }
            finally
            {
                tabHelper.decrementTabs();
            }

            return assertScript;
        }

        /// <summary>
        /// Constroi um script para realizar a verificacao de existencia do objeto.
        /// </summary>
        /// <param name="objectScript"></param>
        /// <returns>script contendo a verificacao de existencia do objeto.</returns>
        protected string verifyObject(string objectScript)
        {   
            if (string.IsNullOrEmpty(objectScript)) return "";

            string verifyScrpt = "";
            try
            {
                tabHelper.incrementTabs();
                objectScript = objectScript.Replace(";", "");
                verifyScrpt = buildWhile("!" + objectScript, "").Replace("\t", "").Replace("\n", "");
                verifyScrpt = tabHelper.TabText + verifyScrpt + "\n";//
            }
            catch (Exception e)
            {
                verifyScrpt = "";
            }
            finally
            {
                tabHelper.decrementTabs();
            }

            return verifyScrpt;
        }


        private string buildDoWhile(string condition)
        {
            string doWhile = tabHelper.TabText + "do\n{{\n{0}\n" + tabHelper.TabText + "}}\n";
            doWhile += tabHelper.TabText + "while(" + condition + ");";

            return doWhile;
        }


        private string buildWhile(string condition, string script)
        {
            string strWhile = "";
            try
            {
                tabHelper.incrementTabs();
                strWhile = tabHelper.TabText + "while(" + condition + ")\n";
                strWhile += tabHelper.TabText + "{{\n{0}\n" + tabHelper.TabText + "}}\n";
                
                script = "\t" + script.Replace("\n", "\n\t");

                strWhile = string.Format(strWhile, script);

            }
            catch (Exception e)
            {
                strWhile = "";
            }
            finally
            {
                tabHelper.decrementTabs();
            }

            return strWhile;
        }


        private string buildFor(int cicles, string script)
        {
            string strFor = "";
            try
            {
                tabHelper.incrementTabs();

                string counterVar = "count_" + counter++;

                strFor = string.Format("{0}for(int {1}=0;{1}<" + cicles + ";{1}++)\n", tabHelper.TabText, counterVar);
                strFor += tabHelper.TabText + "{{\n{0}\n" + tabHelper.TabText + "}}\n";

                script = "\t" + script.Replace("\n", "\n\t");
                
                strFor = string.Format(strFor, script);
            }
            catch (Exception e)
            {
                strFor = "";
            }
            finally
            {
                tabHelper.decrementTabs();
            }

            return strFor;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public string parse(GroupNode node)
        {
            string scrpt = "";
            try
            {
                if (node != null)
                {
                    tabHelper.incrementTabs();

                    bool containsDtBnk = false;
                    bool containsCondition = false;
                    int iterations = 0;
                    string dataBank = "";
                    string loopCondition = "";
                    string stepScrpt = buildStep(node.GroupName);
                    foreach (UmlTransition transition in node.Transitions)
                    {
                        /*
                         * 1 - buildStep()
                         * 2 - buildDataBank()
                         * 3 - buildLoop()
                         * 4 - verifyObject()
                         * 5 - buildObjScript()
                         * 6 - addThink()
                         */
                        Dictionary<TagNames, string> tags = extractTransitionTags(transition);

                        try
                        {
                            if (string.IsNullOrEmpty(loopCondition))
                            {
                                loopCondition = tags[TagNames.TDLoopCondition];
                                containsCondition = !string.IsNullOrEmpty(loopCondition);
                            }

                            if (iterations == 0)
                            {
                                iterations = Convert.ToInt32(tags[TagNames.TDIterations]);
                            }


                            if (!containsDtBnk)
                            {
                                string actions_Assert = tags[TagNames.TDAction] + ",{;" + tags[TagNames.TDAssert]+"}";

                                extractDataBanks(actions_Assert).ForEach(S => dataBank += S);
                                containsDtBnk = !string.IsNullOrEmpty(dataBank);
                            }

                        }
                        catch (FormatException e) { }
                        catch (Exception e) { }

                        //realiza a construcao do script para as tags desta transicao
                        scrpt += parse(tags);
                    }

                    //subgrupos
                    string subScrpt = "";
                    string cond = "";
                    for (int j = 0; j < node.SubGroups.Count; j++ )
                    {
                        GroupNode sub = node.SubGroups[j];
                        //caso a primeira transicao seja loopcond, remove 
                        UmlTransition transition = null;
                        if (j == 0 && sub.Transitions.Count > 0)
                        {
                            transition = node.SubGroups[0].Transitions[0];
                            cond = transition.GetTaggedValue("TDLOOPCONDITION");
                            transition.SetTaggedValue("TDLOOPCONDITION", "");
                        }
                        subScrpt += parse(sub);

                        if (!string.IsNullOrEmpty(cond) && transition != null)
                        {
                            transition.SetTaggedValue("TDLOOPCONDITION", cond);
                        }

                    }

                    //caso tenha subgrupos e tenha loop na primeira transicao
                    if (node.SubGroups.Count > 0)
                    {
                        try
                        {
                            UmlTransition transition = node.SubGroups[0].Transitions[0];
                            Dictionary<TagNames, string> tags = extractTransitionTags(transition);
                            string loopCond = "";
                            try
                            {
                                loopCond = tags[TagNames.TDLoopCondition];
                            }
                            catch (Exception e) { }

                            //caso tenha condicao para o loop, adiciona no while o subscript
                            if (!string.IsNullOrEmpty(loopCond))
                            {
                                subScrpt = buildWhile(loopCond, subScrpt);
                            }
                        }
                        catch (Exception e)
                        {
                        }
                    }
                    scrpt += subScrpt;


                    //verifica entre estas transicoes se deve adicionar um loop...
                    if (iterations > 0)
                    {
                        scrpt = buildFor(iterations, scrpt);
                    }

                    //caso tenha que adicionar as transicoes no databank
                    if (containsDtBnk)
                    {
                        scrpt = dataBank + scrpt;
                    }

                    //step's loop
                    if (containsCondition)
                    {
                        scrpt = buildWhile(loopCondition, scrpt);
                    }

                    scrpt = string.Format(stepScrpt, scrpt);
                }
            }
            catch (Exception e)
            {
                scrpt = "";
            }

            finally
            {
                tabHelper.decrementTabs();
            }

            return scrpt;
        }

        
        public string parse(Dictionary<TagNames, string> tags)
        {
            string script = "";
            try
            {   
                string protocol = tags[TagNames.TDProtocol];
                string tagTDObject = tags[TagNames.TDObject];
                string tagTDAction = tags[TagNames.TDAction];
                string tagTDProperties = tags[TagNames.TDProperties];
                string tagTDwait = tags[TagNames.TDWait];
                string tagTDIterations = tags[TagNames.TDIterations];
                bool tagTDVerify = bool.Parse(tags[TagNames.TDVerify]);
                string tagTDAssert = tags[TagNames.TDAssert];

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

                    if (tagTDVerify && !(tdAction.Contains("captureScreenshot") || (tdObject.Equals("browser") && tdAction.Contains("launch")))) 
                    {
                        //constroi o script para o objeto sem a acao pois a acao de verificacao 
                        //deve ser o metodo ".exists()"
                        script += verifyObject(
                            buildObjScript(protocol, obj.ToString(), properties, tdObject, buildAction("exists", null))
                        );

                        //verifica este objeto apenas uma vez para todas suas acoes
                        tagTDVerify = false;
                    }

                    script += buildObjScript(protocol, obj.ToString(), properties, tdObject, buildAction(tdAction, actionParam));

                    if (!(obj == Components.statusBar | obj == Components.alertDialog))
                    {
                        //mudei de 0 para 1
                        double thinkTime = 1;

                        try{
                            thinkTime = Convert.ToDouble(
                                Configuration.getInstance().getConfiguration(Configuration.Fields.waittime), 
                                CultureInfo.GetCultureInfo("EN-US"));
                            double tagTime = Convert.ToDouble(tagTDwait, CultureInfo.GetCultureInfo("EN-US"));
                            if (tagTime > 0 && (thinkTime != tagTime))
                            {
                                thinkTime = tagTime;
                            }

                        }catch(Exception e){}

                        script = addThink(script, thinkTime);
                    }

                }

                script += buildAssert(protocol, obj, tdObject, properties, tagTDAssert);
                
            }
            catch (Exception e)
            {
                script = "";
            }

            return script;
        }

    }
}
