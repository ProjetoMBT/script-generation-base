using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PlugSpl;
using PlugSpl.Modules;
using System.Diagnostics;
using System.IO;
using ComponentPoolManager.CodeHandler;
using PlugSpl.DataStructs.ProductConfigurator;
using Microsoft.Win32;
using System.Xml.Serialization;
using System.Reflection;
using System.Globalization;
using System.Xml;


namespace ProductInstantiator
{
    /// <summary>
    /// Interaction logic for Instantiator.xaml
    /// </summary>
    public partial class Instantiator : UserControl, IProductInstantiator
    {
        string configuratorPath;
        DanuProductConfigurator danu;
        private string compilerPath;
        private String path;

        #region Interface Implementation
        public MainWindow CurrentApplicationWindow { get; set; }

        public Control GetControl()
        {
            return this;
        }

        public void ExportStructure()
        {

            string oldDirectory = Environment.CurrentDirectory;

            OpenFileDialog open = new OpenFileDialog();
            open.Filter = open.Filter = "PlugSPL Product Configuration (*.plugpc) | *.plugpc";
            if (open.ShowDialog() != true)
            {
                Environment.CurrentDirectory = oldDirectory;
                return;
            }
            configuratorPath = open.FileName;
            this.textBoxGreen.Text = configuratorPath;
            Environment.CurrentDirectory = oldDirectory;
            path = oldDirectory;
        }

        public void ImportStructure()
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "PlugSPL Component Metadata (*.plugcm) | *.plugcm";

            if (open.ShowDialog() != true)
                return;

            //change directory. must to be fixed.
            string oldLocation = Environment.CurrentDirectory;
            string lowerFileName = open.FileName.Split('\\').Last().Split('.').First();
            string appPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Environment.CurrentDirectory = System.IO.Path.Combine(appPath, "./Projects/" + lowerFileName);

            StreamReader reader = new StreamReader(open.FileName);
            this.textBoxRed.Text = open.FileName;
            XmlSerializer serializer = new XmlSerializer(typeof(PlugSpl.DataStructs.ProductConfigurator.DanuProductConfigurator));
            danu = (PlugSpl.DataStructs.ProductConfigurator.DanuProductConfigurator)serializer.Deserialize(reader);

            //change current dir to old location
            Environment.CurrentDirectory = oldLocation;
            path = oldLocation;

        }

        public void SetApplicationWindow(MainWindow mainWindow)
        {
            this.CurrentApplicationWindow = mainWindow;
        }
        #endregion
        #region Constructors


        public Instantiator()
        {
            InitializeComponent();
        }

        #endregion
        #region Events and Delegates
        /// <summary>
        /// Button Compile interaction logic.
        /// </summary>
        private void buttonCompile_Click(object sender, RoutedEventArgs i)
        {
            try
            {
                // String pathFull = @"C:\Users\COC-DELL-20\Desktop\head\__output\Coc.Apps.PlugSpl\Projects\plets\Repository";
                string source = "./Source/";
                string target = "./Repository/";
                //string oldEnvDir = Environment.CurrentDirectory;
                if (!File.Exists(textBoxCompilerPath.Text))
                {
                    MessageBox.Show("Compiler not found. Invalid property value or file do not exists.");
                    return;
                }
                String subDir = @"Source";
                // String pathFull = System.IO.Path.GetFullPath(repository);
                Assembly caminho = Assembly.GetExecutingAssembly();
                String s = caminho.Location;
                String pathFull;
                pathFull = s.Replace("ProductInstantiator.dll", "Source");
                //pathFull = configuratorPath.Substring(0, configuratorPath.LastIndexOf("\\"))+ "\\"+repository;
                DirectoryInfo pasta = new DirectoryInfo(pathFull);
                List<String> list = ListaDiretorios(pasta, pathFull, subDir);
                //XmlDocument document = new XmlDocument();
                //document = Generator.ToXmi(list);
                //document.Save(pathFull);
                /*
                FileInfo cm = new FileInfo(textBoxRed.Text);
                FileInfo pc = new FileInfo(textBoxGreen.Text);
                if (!cm.DirectoryName.Equals(pc.DirectoryName))
                {
                    MessageBox.Show("Component metadata and product configuration must be in the same directory.");
                    return;
                }
                */
                //change directory pro project path
                /*
                  string oldlocation = Environment.CurrentDirectory;
                  string lowername = oldlocation + @"\Projects\" + textBoxRed.Text.Split('\\').Last().Split('.').First();
                  Environment.CurrentDirectory = lowername;
                  */
                //set up compiler
                Process p = new Process();
                p.StartInfo.FileName = textBoxCompilerPath.Text;
                if (checkBoxOptmize.IsChecked == true)
                    p.StartInfo.Arguments += " /optmize";
                p.StartInfo.Arguments = target + "PlugSPLCompileProject.csproj";
                textBoxLog.Text += Environment.NewLine + "Trying to call compiler with command line \"" + p.StartInfo.FileName + " " +
                    p.StartInfo.Arguments + "\" at " + DateTime.Now.ToShortTimeString();
                List<DanuComponent> chosenOnes = null;
                try
                {
                    //Gera CSPROJ
                    chosenOnes = FreyrCSharpCodeGenerator.GenerateProductProject(danu, configuratorPath, list);
                    textBoxLog.Text += Environment.NewLine + "[" + DateTime.Now.ToShortTimeString() + "] Project file generated successfully.";
                }
                catch (Exception e)
                {
                    textBoxLog.Text += Environment.NewLine + "[" + DateTime.Now.ToShortTimeString() + "] Project file NOT generated.";
                    textBoxLog.Text += Environment.NewLine + e.ToString();
                }
                try
                {
                    //Copy files
                    Utils.CopyDirectory(source, target);
                    textBoxLog.Text += Environment.NewLine + "[" + DateTime.Now.ToShortTimeString() + "] Source code copyied to work directory.";
                }
                catch (Exception ee)
                {
                    textBoxLog.Text += Environment.NewLine + "[" + DateTime.Now.ToShortTimeString() + "] Source code NOT copyied to work directory.";
                    textBoxLog.Text += Environment.NewLine + ee.ToString();
                }
                //Replace Tags
                try
                {
                    ReplaceTags(chosenOnes);
                    textBoxLog.Text += Environment.NewLine + "[" + DateTime.Now.ToShortTimeString() + "] Tags replaced successfully.";
                }
                catch (Exception eee)
                {
                    textBoxLog.Text += Environment.NewLine + "[" + DateTime.Now.ToShortTimeString() + "] Tags NOT replaced.";
                    textBoxLog.Text += Environment.NewLine + eee.ToString();
                }
                //call compiler
                try
                {
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.ErrorDialog = false;
                    p.StartInfo.RedirectStandardError = true;
                    p.StartInfo.RedirectStandardInput = true;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.Start();
                    textBoxLog.Text += p.StandardOutput.ReadToEnd();
                    textBoxLog.Text += p.StandardError.ReadToEnd();
                }
                catch (Exception eeee)
                {
                    textBoxLog.Text += Environment.NewLine + eeee.ToString();
                }
                finally
                {
                    p.WaitForExit();
                }

                //launch application
                if (checkBoxLaunch.IsChecked == true && File.Exists(source + "product.exe"))
                {
                    p.StartInfo.FileName += "product.exe";
                    p.StartInfo.Arguments = "";

                    try
                    {
                        p.Start();
                    }
                    catch (Exception eeeee)
                    {
                        textBoxLog.Text += Environment.NewLine + eeeee.ToString();
                    }
                }

                //return directory to original value
                //    Environment.CurrentDirectory = oldlocation;
            }
            catch (Exception ex)
            {
                textBoxLog.Text += Environment.NewLine + ex.ToString();
            }
        }

        private void CopySourceCodeFiles()
        {

        }
        #endregion

        private void ReplaceTags(List<DanuComponent> chosenOnes)
        {
            foreach (DanuComponent comp in chosenOnes)
            {
                foreach (DanuComponent compChild in chosenOnes)
                {
                    foreach (DanuInterfaceObject io in comp.Interfaces)
                    {
                        foreach (DanuSocket so in compChild.Sockets)
                        {
                            if (io.PossibleSockets.Contains(so.Parent))
                            {
                                string newCode =
                                    FreyrCSharpCodeGenerator.ReplaceInterface(comp.Name + "/" +
                                    comp.Name, compChild.Name, io.Name);

                                File.Delete("./Repository/" + comp.Name + "/" + comp.Name + ".cs");

                                TextWriter writer = new StreamWriter("./Repository/" +
                                    comp.Name + "/" + comp.Name + ".cs");

                                writer.Write(newCode);

                                writer.Close();
                            }
                        }
                    }
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.ImportStructure();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.ExportStructure();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            string msBuildPath = @"C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe";
            this.compilerPath = msBuildPath;

            if (!File.Exists(msBuildPath))
            {
                this.textBoxLog.Text = "Cannot find default MSBuild application. PlugSPL is unable to compile any C# project without a valid MSBuild location. You can set MSBuild location from Compiler path box.";
            }
            else
            {
                this.textBoxLog.Text = "MSBuild compiler found at \"" + msBuildPath + "\". PlugSPL can now compile C# projects for .NET Framework 4.0.X applications.";
                textBoxCompilerPath.Text = msBuildPath;
            }
        }

        // METODO RECURSIVO PARA LISTA DIRETORIOS E SUB-DIRETORIOS
        private List<String> ListaDiretorios(DirectoryInfo diretorioPai, String pathFull, String subDir)
        {
            List<String> list = new List<String>();

            // para cada sub-diretorio 
            foreach (DirectoryInfo dir in diretorioPai.GetDirectories())
            {
                // lista diretorios do diretorio corrente
                list.AddRange(ListaDiretorios(dir, pathFull, subDir));
                FileInfo[] arquivos = dir.GetFiles();
                //Varre o arquivo dentro da pasta
                foreach (FileInfo file in arquivos)
                {
                    //faz a comparação se o arquivo tem a estensão procurada
                    if (file.Extension.Equals(".cs") || file.Extension.Equals(".xaml"))
                    {
                        CompareInfo compare = CultureInfo.InvariantCulture.CompareInfo;
                        int index = compare.IndexOf(pathFull, subDir, CompareOptions.IgnoreCase);
                        String nomeArquivo = file.FullName.Substring(index).Replace(subDir + @"\", "");
                        list.Add(nomeArquivo);
                    }
                }
            }
            return list;
        }
    }

}
