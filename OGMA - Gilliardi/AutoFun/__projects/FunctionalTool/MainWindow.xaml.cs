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
using Microsoft.Win32;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;
using System.Diagnostics;
using FunctionalTool.Testing.Functional;
using FunctionalTool.Exceptions;
using Coc.Apps.FunctionalTestingTool;
using System.Xml;
using Coc.Modeling.Uml;
using Coc.Testing.Functional;
using Coc.Apps.PleTs;

namespace FunctionalTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static ProgressBar ProgressBar { get; set; }
        public string modelName;

        public static void Main(string[] args)
        {
            ParameterizedThreadStart start =
                new ParameterizedThreadStart(Execute);

            Thread t = new Thread(start);
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            
        }

        public static void Execute(object o)
        {
            MainWindow m = new MainWindow();
            m.ShowDialog();
        }

        public MainWindow()
        {
            InitializeComponent();
            ProgressBar = progressBar;
            ProgressBar.Minimum = 0;
        }

        /// <summary>
        /// this is the event method for import button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnImporta_Click(object sender, RoutedEventArgs e)
        {
            String filePath = this.openFile();
            if (filePath != "")
            {
                try
                {
              
                    {
                      //  TestPlan testPlan = PopulateTestPlan.PopulateTP(ParserXmi.useCaseDiagram, ParserXmi.dicActivityDiagram);
                        XmlDocument doc= new XmlDocument();
                        doc.Load(filePath);
                        UmlModel model = new UmlModel("tre");
                        Coc.Data.Xmi.XmiImporter.FromXmi(model,doc);
                        Coc.Testing.Functional.TestPlan testPlan = PopulateTestPlanV1.PopulateTP(model);
                        String destPath = this.saveFile();
                        if (destPath != null)
                        {
                            String archiveName = verifyOpenFiles(destPath);

                            if (archiveName == "dontHave")
                            {
                                //Data.Excel.GenerateXlsFromTestPlan(testPlan, destPath);
                                Coc.Apps.PLeTs.Excel.GenerateXlsFromTestPlan(testPlan, destPath);
                                //Data.Excel.

                                MessageBox.Show("Saved in " + destPath);
                                System.Diagnostics.Process.Start("explorer.exe", destPath);
                            }
                            else
                            {
                                MessageBox.Show(archiveName + " is opened");
                            }
                        }
                    }
                }
                catch (InvalidBeginNode beginEx)
                {
                    MessageBox.Show("Operation canceled. Begin node not found. Error: [" + beginEx.Message + "]");
                }
                catch (InvalidEndNode endEx)
                {
                    MessageBox.Show("Operation canceled. End node  not found" + endEx.Message + "]");
                }
                catch (InvalidTag tagEx)
                {
                    MessageBox.Show("Operation canceled. Tag not found. Error: [" + tagEx.Message + "]");
                }
                catch (InvalidTransition transEx)
                {
                    MessageBox.Show("Operation canceled. Transition not found. Error: [" + transEx.Message + "]");
                }
                catch (InvalidDescription)
                {
                    MessageBox.Show("Operation canceled.");
                }
                catch (InvalidExpectedResult)
                {
                    MessageBox.Show("Operation canceled.");
                }
                catch (Exception genericEx)
                {
                    MessageBox.Show(genericEx.Message);
                }
            }
        }

        public String openFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.CurrentDirectory + "\\Models";
            openFileDialog.DefaultExt = "XML";
            openFileDialog.Title = "Import XMI file";
            openFileDialog.CheckPathExists = true;
            openFileDialog.Filter = "(*.XML)|*.XML";
            openFileDialog.FilterIndex = 2;
            string nameValidator = @".xml$";
            if (openFileDialog.ShowDialog() == true)
            {
                if (!Regex.Match(openFileDialog.SafeFileName, nameValidator).Success)
                {
                    MessageBox.Show("Invalid file!");
                    return null;
                }
            }

            modelName = System.IO.Path.GetFileNameWithoutExtension(openFileDialog.FileName);

            string str = "@,!#$%\"^&:*<>?|//\\";
            char[] ch = str.ToCharArray();

            foreach (char c in ch)
            {
                if (modelName.Contains(c.ToString()))
                {
                    modelName = modelName.Replace(c.ToString(),"");
                }
            }


            return openFileDialog.FileName;
        }

        public String saveFile()
        {
            
            String directory = Environment.CurrentDirectory + "\\Result Files\\" ;
            //verifica se diretório não existe
            if (!Directory.Exists(directory + modelName))
            {
                directory = directory + modelName;
                Directory.CreateDirectory(directory); 
            }
            else
            {
                String name;
                String[] sub;
                int i = 1;
                //se ele existe cria um só que com numeração diferente
                string[] filePaths = Directory.GetDirectories(directory);
                foreach (string path in filePaths)
                {
                    name = System.IO.Path.GetFileNameWithoutExtension(path);
                    if (name.Contains("("))
                    {
                        char[] splitTokens = { '(',')' };

                        sub = name.Split(splitTokens);

                        if (sub[0] == modelName)
                        {
                            //descubro o maior index
                            if (Convert.ToInt16(sub[1]) > i)
                                i = Convert.ToInt16(sub[1]);
                        }
                    }
                }
                directory = Environment.CurrentDirectory + "\\Result Files\\" + modelName + "(" + (i + 1) + ")";
                Directory.CreateDirectory(directory);
                i++;
            }


            var dlg = new System.Windows.Forms.FolderBrowserDialog();
            dlg.Description = "Choose destiny folder";
            dlg.ShowNewFolderButton = true;
            dlg.Reset();
            dlg.SelectedPath = directory;
            
            if ( System.Windows.Forms.DialogResult.OK == dlg.ShowDialog())
            {
                String folder = dlg.SelectedPath;
                if (folder == directory)
                {
                    return directory;
                }
                else
                {
                    Directory.Delete(directory);
                    if (!Directory.Exists(folder+"\\"+modelName))
                    {
                        Directory.CreateDirectory(folder + "\\"+modelName);
                        return folder + "\\" + modelName;
                    }
                    else
                    {
                        String name;
                        String[] sub;
                        int i = 1;
                        //se ele existe cria um só que com numeração diferente
                        string[] filePaths = Directory.GetDirectories(folder);
                        foreach (string path in filePaths)
                        {
                            name = System.IO.Path.GetFileNameWithoutExtension(path);
                            if (name.Contains("("))
                            {
                                char[] splitTokens = { '(', ')' };

                                sub = name.Split(splitTokens);

                                if (sub[0] == modelName)
                                {
                                    //descubro o maior index
                                    if (Convert.ToInt16(sub[1]) > i)
                                        i = Convert.ToInt16(sub[1]);
                                }
                            }
                        }
                        directory = folder + "\\" +  modelName + "(" + (i + 1) + ")";
                        Directory.CreateDirectory(directory);
                        i++;

                        return directory;
                    }
                }
            }
            return null;
        }

        public String verifyOpenFiles(String filePath)
        {
            String fileOpened = "dontHave";

            string[] filePaths = Directory.GetFiles(filePath);
            foreach (string path in filePaths)
            {
                try
                {
                    System.IO.FileStream fs = System.IO.File.OpenWrite(path);
                    fs.Close();
                }
                catch (System.IO.IOException)
                {
                    return path;
                }
            }
            return fileOpened;
        }

    }
}
