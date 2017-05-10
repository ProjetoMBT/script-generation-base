using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Coc.Modeling.Uml;
using System.Reflection;


namespace Coc.Data.Xmi
{
    public class FuntionalValidator
    {
        
        private List<String> errors = new List<string>();
        /// <summary>
        /// Inicia a validacao de um ou mais diagramas.
        /// </summary>
        public Boolean Validator(UmlModel model)
        {
            foreach (UmlDiagram diagram in model.Diagrams)
            {

                if (diagram is UmlUseCase)
                {
                    if (diagram.UmlObjects.OfType<UmlActor>().Count() == 0)
                    {
                        errors.Add("There is no Actor(" + diagram.Name + ")");
                    }
                }
                else if(diagram is UmlActivityDiagram){

                    if (diagram.UmlObjects.OfType<UmlInitialState>().Count() != 1)
                    {
                        errors.Add("There is no Initial State  or There are two or more of them (" + diagram.Name + ")");
                    }

                    if (diagram.UmlObjects.OfType<UmlFinalState>().Count() != 1)
                    {
                        errors.Add("There is no Final State  or There are two or more of them (" + diagram.Name + ")");
                    }


                }
                    foreach (UmlBase element in diagram.UmlObjects)
                {
                   this.ValidateElement(diagram, element);
                }
            }
            foreach (String s in errors)
            {
                Console.WriteLine(s);
            }

            return this.errors.Count() == 0;
        }

        public void ValidateElement(UmlDiagram diagram, UmlBase element)
        {
            foreach (KeyValuePair<string, string> pair in element.TaggedValues)
            {
                
                
                
                
                String name =  diagram.GetType().Name + "_" + element.GetType().Name + "_" + pair.Key;

                if (pair.Key.Contains("jude.hyperlink"))
                {
                    name = pair.Key.Replace('.', '_');
                }
                
                MethodInfo method = this.GetType().GetMethod(name);

             

                String info;
                if (element is UmlUseCase)
                {
                    UmlUseCase aux = (UmlUseCase)element;
                    info = diagram.Name + " >> " + aux.Name + " >> " + pair.Key;
                }
                else
                {
                    info = diagram.Name + " >> " + element.Name + " >> " + pair.Key;
                }
                if (method != null)
                {
                    method.Invoke(this, new object[] { pair.Value, info });
                }
            }
        }



        public bool UmlActionStateDiagram_UmlActionState_TDmethod(string p, String f)
        {
            if (new string[] { "POST", "GET" }.Contains(p))
                return true;
            else
            {
                errors.Add(f + " >> " + p);
                return false;
            }
        }


        public bool UmlActivityDiagram_UmlTransition_TDmethod(string p, String f)
        {
            if (new string[] { "POST", "GET" }.Contains(p))
                return true;
            else
            {
                errors.Add(f + " >> " + p);
                return false;
            }
        }

        public bool UmlActivityDiagram_UmlTransition_TDexpTime(string p, String f)
        {
            if (String.IsNullOrEmpty(p))
                return false;

            try
            {
                double val = Convert.ToDouble(p);

                if (val > 0)
                    return true;
            }
            catch (Exception) { }
            return false;
        }

        public bool UmlActivityDiagram_UmlTransition_TDbody(string p, String f)
        {
            if (String.IsNullOrEmpty(p))
            {
                errors.Add(f + " >> " + p);
                return false;
            }
            return true;
        }

        public bool UmlActivityDiagram_UmlTransition_TDcookies(string p, String f)
        {
            if (String.IsNullOrEmpty(p))
            {
                errors.Add(f + " >> " + p);
                return false;
            }
            return true;
        }

        public bool UmlActivityDiagram_UmlTransition_TDsaveParameters(string p, String f)
        {
            if (String.IsNullOrEmpty(p))
            {
                errors.Add(f + " >> " + p);
                return false;
            }
            return true;
        }

        public bool UmlActivityDiagram_UmlTransition_TDparameters(string p, String f)
        {
            if (String.IsNullOrEmpty(p))
                return false;

            String[] parameters = p.Split('|');

            foreach (String parameter in parameters)
            {
                string param_name;
                string param_value;

                try
                {
                    param_name = parameter.Replace("@@", "|").Split('|')[0];
                    param_value = parameter.Replace("@@", "|").Split('|')[1];

                    if (param_name.Length < 1 || param_value.Length < 1)
                    {
                        errors.Add(f + " >> " + param_name);
                        return false;
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return true;
        }

        public bool UmlActivityDiagram_UmlTransition_TDthinkTime(string p, String f)
        {
            try
            {
                double val = Convert.ToDouble(p);

                if (val > 0)
                    return true;
            }
            catch (Exception) { }
            errors.Add("BUGADO!");
            return false;
        }

        public bool UmlActivityDiagram_UmlTransition_TDaction(string p, String f)
        {
            if (String.IsNullOrEmpty(p))
            {
                errors.Add(f + " >> " + p);
                return false;
            }
            return true;
        }




        public bool UmlActivityDiagram_UmlActionState_jude_hyperlink(string p, String f)
        {
            if (String.IsNullOrEmpty(p))
            {
                errors.Add(f + " >> " + p);
                return false;
            }
            

            return true;
        }






    
    }
}
