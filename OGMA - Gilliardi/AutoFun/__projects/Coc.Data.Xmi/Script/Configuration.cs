using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using System.Reflection;

namespace Coc.Data.Xmi.Script
{


    /// <summary>
    /// Esta classe e responsavel pelo gerenciamento das configuracoes da ferramenta.
    /// </summary>
    public class Configuration
    {

        /// <summary>
        /// Esta enumeracao guarda os nomes dos campos que deverao ser salvos/recuperados
        /// no arquivo de configuracao da ferramenta.
        /// </summary>
        public enum Fields
        {
            waittime,
            astahpath,
            oatspath,
            verifyall,
            workspacepath,
            softwareversion,
            actioncolor,
            actionlist
        }



        private Dictionary<Fields, object> configurations;

        private static Configuration instance;


        private Configuration()
        {
            init();
        }

        /// <summary>
        /// Inicializa as configuracoes.
        /// </summary>
        public void init()
        {
            this.configurations = new Dictionary<Fields, object>();
            readConfigFile();
            checkPath();
        }

        /// <summary>
        /// Realiza a leitura do arquivo de configuracao.
        /// </summary>
        public void readConfigFile()
        {
            StreamReader sr = null;
            try
            {
                sr = new StreamReader("Config.cfg");
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if (!string.IsNullOrEmpty(line))
                    {
                        string field = line.Split(new char[] { '=' })[0].ToLower();
                        string value = line.Split(new char[] { '=' })[1];

                        switch (field)
                        {
                            case "actioncolor":
                                setConfiguration(Fields.actioncolor, value);
                                break;
                            case "actionlist":
                                setConfiguration(Fields.actionlist, value);
                                break;
                            case "astahpath":
                                setConfiguration(Fields.astahpath, value);
                                break;
                            case "oatspath":
                                setConfiguration(Fields.oatspath, value);
                                break;
                            case "softwareversion":
                                setConfiguration(Fields.softwareversion, value);
                                break;
                            case "verifyall":
                                setConfiguration(Fields.verifyall, value);
                                break;
                            case "waittime":
                                setConfiguration(Fields.waittime, value);
                                break;
                            case "workspacepath":
                                setConfiguration(Fields.workspacepath, value);
                                break;
                            
                        }
                    }

                }
                sr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                }
            }
        }


        /// <summary>
        /// Verifica se a string passada por parametro e igual ao nome
        /// de algum campo de configuracao.
        /// </summary>
        /// <param name="fildName">Nome do campo que se quer verificar a existencia.</param>
        /// <returns></returns>
        private Tuple<bool,object> isFieldName(string fildName)
        {
            bool isField = false;
            object ret = null;

            foreach (Fields f in (Fields[])Enum.GetValues(typeof(Fields)))
            {
                if (isField = f.ToString().Equals(fildName))
                {
                    ret = f;
                    break;
                }
            }

            return Tuple.Create<bool, object>(isField, ret);
        }


        /// <summary>
        /// Salva todas as configuracoes em um arquivo, retornando o status da operacao.
        /// </summary>
        /// <returns><code>true</code>, caso ocorra a escrita sem erros ou 
        /// <code>false</code> caso contrario.</returns>
        public bool writeConfigFile()
        {
            bool write = false;
            try
            {
                StreamWriter sw = new StreamWriter("Config.cfg");
                foreach(Fields field in configurations.Keys)
                {
                    if (field == Fields.waittime)
                    {
                        sw.WriteLine(Fields.waittime + "=" + configurations[field]);
                    }
                    else
                    {
                        sw.WriteLine(field + "=" + configurations[field]);
                    }

                }

                sw.Close();
                write = true;
            }
            catch (Exception e)
            {
                write = false;
                Console.WriteLine(e.StackTrace);
            }
            return write;
        }


        public static Configuration getInstance(){
            if(instance == null)
            {
                instance = new Configuration();
            }
            return instance;
        }


        public bool configurationExists(Fields field)
        {
            return configurations.ContainsKey(field);
        }


        /// <summary>
        /// Realiza a recuperacao de uma configuracao, de acordo como o campo passado por parametro.
        /// </summary>
        /// <param name="field"></param>
        /// <returns>O valor da configuracao ou string vazia caso nao exista.</returns>
        public string getConfiguration(Fields field)
        {
            string val = "";
            if (configurations.ContainsKey(field))
            {
                val = configurations[field] + "";
            }
            return val;
        }


        /// <summary>
        /// Altera o valor da configuracao passada por parametro.
        /// </summary>
        /// <param name="field">Campo que devera receber o novo valor.</param>
        /// <param name="configValue">Novo valor para a configuracao.</param>
        public void setConfiguration(Fields field, string configValue)
        {
            try
            {

                if (configValue.Length>0 && (field == Fields.astahpath || field == Fields.oatspath || field == Fields.workspacepath))
                {
                    if (!configValue.EndsWith("\\"))
                    {
                        configValue += "\\";
                    }
                }


                if (configurations.ContainsKey(field))
                {
                    configurations[field] = configValue;
                }
                else
                {
                    configurations.Add(field, configValue);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }


        private void checkPath()
        {
            try
            {
                if (!Directory.Exists(getConfiguration(Fields.astahpath)))
                {
                    setConfiguration(Fields.astahpath, AssemblyDirectory);
                }

                if (!Directory.Exists(getConfiguration(Fields.oatspath)))
                {
                    setConfiguration(Fields.oatspath, AssemblyDirectory);
                }

                if (!Directory.Exists(getConfiguration(Fields.workspacepath)))
                {
                    setConfiguration(Fields.workspacepath, AssemblyDirectory);
                }

            }
            catch (Exception e)
            {

            }

        }

        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

    }
}
