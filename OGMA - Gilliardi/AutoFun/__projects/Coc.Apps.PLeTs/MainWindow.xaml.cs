//#define PL_FUNCTIONAL_TESTING
//#define PL_PERFORMANCE_TESTING -- Não foi coberta por nenhuma configuração de solution
//#define PL_DFS
//#define PL_HSI
//#define PL_WP -- Não foi coberta por nenhuma configuração de solution
//#define PL_GRAPH
//#define PL_FSM
//#define PL_LR -- Não foi coberta por nenhuma configuração de solution
//#define PL_OATS
//#define PL_MTM
//#define PL_XMI


#define AUX


using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Xml;
using System.Text;
using Microsoft.Win32;
using Coc.Data.ControlStructure;
using System.Web;
using Coc.Apps.PleTs;
using Coc.Data.Xmi.Script;

namespace Coc.Apps.PLeTs
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public enum ErrorLevel
        {
            Critical,
            Warning,
            Message,
            Green
        }
        private ControlUnit.ControlUnit control;
        private Boolean fatalError = false;
        private String cFilePath;
        private String prmFilePath;
        private String value = "";
        private String parserType = "";
        private StructureType type;
        /// <summary>
        /// Removes encapsulation from status field. Allows
        /// to modify status field from anywhere.
        /// </summary>
        private static TextBlock textBlockStatus;
        /// <summary>
        /// Stores current exe folder.
        /// </summary>
        private static String CurrentPath;
        /// <summary>
        /// Static reference for internal text block. Used by Log* functions.
        /// </summary>
        private static RichTextBox textBlockLog;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            MainWindow.SetInstance(this);
            MainWindow.CurrentPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            MainWindow.SetStatus("Ready.");
            ButtonsInitialization();

            Configuration.getInstance().setConfiguration(
                Configuration.Fields.softwareversion,
                Assembly.GetExecutingAssembly().GetName().Version.ToString(3)
            );

            #region TITLE HANDLING
#if PL_FUNCTIONAL_TESTING
            //this.Title = "Functional Testing Tool for ";
            this.Title = "PLeTsFunc - A Model-Based Functional Testing Tool for ";
#elif PL_PERFORMANCE_TESTING
            this.Title = "Performance Testing Tool for ";
#endif
            #endregion

            #region MTM
#if PL_MTM
            //this.buttonMTM.Click += new RoutedEventHandler(this.buttonMTM_Click);
            GenerateFile.Items.Insert(1, "MTM - Script");
            this.Title += "Microsoft Test Manager";
            this.GenerateFile.IsEnabled = false;
#endif
            #endregion
            #region OATS
#if PL_OATS
            //this.buttonOpenScript.Click += new RoutedEventHandler(this.buttonOpenScript_Click);
            //this.buttonParseXMItoXLS.Click += new RoutedEventHandler(this.buttonParseXMItoXLS_Click);
            GenerateFile.Items.Insert(1, "OATS - Excel");
            this.Title += "Oracle ATM Open Script";
            this.GenerateFile.IsEnabled = false;
#endif
            #endregion
            #region LOADRUNNER
#if PL_LR
            //this.buttonLoadRunner.Click += new RoutedEventHandler(this.buttonLoadRunner_Click);
            //this.buttonParseLRtoXMI.Click += new RoutedEventHandler(this.buttonParseLRtoXMI_Click);
            GenerateFile.Items.Insert(1, "LoadRunner Script");
            this.Title += "HP Load Runner ";
            this.GenerateFile.IsEnabled = false;
#else
            //this.toolBarShortcuts.Items.Remove(this.buttonLoadRunner);
            ((MenuItem)this.buttonParseLRtoXMI.Parent).Items.Remove(this.buttonParseLRtoXMI);
#endif
            #endregion

            #region Sequence Generation Methods
#if PL_OATS
            SequenceGeneratorType.Items.Add("OATS");
#endif
#if PL_DFS
            SequenceGeneratorType.Items.Add("DFS Method");
#endif
#if PL_HSI
            SequenceGeneratorType.Items.Add("HSI Method");
#endif
#if PL_WP
            SequenceGeneratorType.Items.Add("Wp Method");
#endif
            #endregion
            #region Parser Type
#if PL_LR
            Parser.Items.Add("Astah XML");
            //Alterar a atual implementação do LRtoXMI
            //Parser.Items.Add("LoadRunnerToXMI");
#endif
#if PL_OATS
#if AUX
            Parser.Items.Add("Xmi to OATS");
#endif
            Parser.Items.Add("Script JAVA");
            Parser.Items.Add("Astah XML");
#endif
#if PL_MTM
            Parser.Items.Add("Astah XML");
            Parser.Items.Add("Argo XML");
            Parser.Items.Add("Enterprise Architect");
#endif
            #endregion
            //adds build number to title bar
            Version v = Assembly.GetExecutingAssembly().GetName().Version;
            this.Title += (" - v" + v.Major + "." + v.Minor);//+ " " + v.Build + "." + v.Revision);
            this.buttonClearLog_Click(null, null);
        }

        private void ButtonsInitialization()
        {
            buttonLoadData.IsEnabled = false;
            buttonGenerateTestCases.IsEnabled = false;
            GenerateFile.IsEnabled = false;
            buttonXmiExport.IsEnabled = false;
            //buttonMTM.IsEnabled = false;
            //buttonParseXMItoXLS.IsEnabled = false;
            //buttonOpenScript.IsEnabled = false;
        }

        /// <summary>
        /// Singleton implementation.
        /// </summary>
        private static void SetInstance(MainWindow mainWindow)
        {
            MainWindow.textBlockLog = mainWindow.textBlockLogContainer;
            MainWindow.textBlockStatus = mainWindow.textBlockStatusContainer;
        }

        /// <summary>
        /// Update status bar information.
        /// </summary>
        private static void SetStatus(String p)
        {
            MainWindow.textBlockStatus.Text = p;
        }

        private void SequenceGeneratorType_SelectionChanged(Object sender, SelectionChangedEventArgs e)
        {
            value = SequenceGeneratorType.SelectedValue.ToString();

            switch (value)
            {
#if PL_OATS
                case "OATS":
                    control = new ControlUnit.ControlUnit(StructureType.OATS);
                    type = StructureType.OATS;
                    Parser.IsEnabled = true;
                    break;
#endif
#if PL_DFS
                case "DFS Method":
                    control = new ControlUnit.ControlUnit(StructureType.DFS);
                    type = StructureType.DFS;
                    Parser.IsEnabled = true;
                    break;
#endif
#if PL_HSI
                case "HSI Method":
                    control = new ControlUnit.ControlUnit(StructureType.HSI);
                    type = StructureType.HSI;
                    Parser.IsEnabled = true;
                    GenerateFile.IsEnabled = false;
                    break;
#endif
#if PL_WP
                case "Wp Method":
                    control = new ControlUnit.ControlUnit(StructureType.Wp);
                    type = StructureType.Wp;
                    Parser.IsEnabled = true;
                    break;
#endif
            }
        }

        /// <summary>
        /// Load an UML model from given XMI file.
        /// </summary>
        private void buttonLoadData_Click(Object sender, RoutedEventArgs e)
        {
            //shows file dialog
            OpenFileDialog dialog = new OpenFileDialog();

            switch (parserType)
            {
#if PL_OATS
#if AUX
                case "Xmi to OATS":
                    dialog.Filter = "Open XMI project files (*.xmi, *.xml) |*.xmi;*.xml|All files (*.*)|*.*";
                    break;
#endif
                case "Script JAVA":
                    dialog.Filter = "Open Java files (*.java) |*.java|All files (*.*)|*.*";
                    break;
#if PL_XMI
                case "Astah XML":
                    dialog.Filter = "Open XMI project files (*.xmi, *.xml) |*.xmi;*.xml|All files (*.*)|*.*";
                    break;
#endif
#elif PL_XMI
                case "Astah XML":
                    dialog.Filter = "Open XMI project files (*.xmi, *.xml) |*.xmi;*.xml|All files (*.*)|*.*";
                    break;
                case "Argo XML":
                    dialog.Filter = "Open XMI project files (*.xmi, *.xml) |*.xmi;*.xml|All files (*.*)|*.*";
                    break;
                case "Enterprise Architect":
                    dialog.Filter = "Open XMI project files (*.xmi, *.xml) |*.xmi;*.xml|All files (*.*)|*.*";
                    break;
#endif
#if PL_LR
                case "LoadRunnerToXMI":
                    dialog.Filter = "All files (*.*)|*.*";
                    break;
#endif
            }

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            Boolean parsedSuccessfully = true;
            MainWindow.LogAppend(dialog.FileName);
            MainWindow.LogAppend("Initializing file parsing...");

            try
            {
                GenerateFile.SelectedIndex = 0;
                control.LoadModelingStructure(dialog.FileName, parserType);
                Validate(dialog.FileName, parsedSuccessfully);
                SequenceGeneratorType.IsEnabled = true;
            }
            catch (IOException ioe)
            {
                parsedSuccessfully = false;
                SequenceGeneratorType.IsEnabled = false;
                GenerateFile.IsEnabled = false;
                buttonXmiExport.IsEnabled = true;
                //buttonParseXMItoXLS.IsEnabled = false;
                MainWindow.LogAppend(ioe.Message, ErrorLevel.Critical);
                MainWindow.LogAppend("Correlation file is being used by another process. Please close it and reload the XMI file.", ErrorLevel.Critical);
            }
            catch (Exception ex)
            {
                MainWindow.LogAppend("[ERROR] " + ex.Message, ErrorLevel.Critical);
                parsedSuccessfully = false;
                SequenceGeneratorType.IsEnabled = false;
                GenerateFile.IsEnabled = false;
                buttonXmiExport.IsEnabled = true;
                //buttonParseXMItoXLS.IsEnabled = false;
            }
        }

        private void GenerateFile_SelectionChanged(Object sender, SelectionChangedEventArgs e)
        {
            value = GenerateFile.SelectedValue.ToString();

            switch (value)
            {
#if PL_MTM
                case "MTM - Script":
                    buttonMTM_Click(sender, e);
                    break;
#endif
#if PL_OATS
                case "OATS - Excel":
                    buttonParseXMItoXLS_Click(sender, e);  
                    break;
#endif
#if PL_LR
                case "LoadRunner Script":
                    //NOT IMPLEMENTED
                    break;
#endif
            }
        }

        private void Parser_SelectionChanged(Object sender, SelectionChangedEventArgs e)
        {
            parserType = Parser.SelectedValue.ToString();

            switch (parserType)
            {
#if PL_OATS
#if AUX
                case "Xmi to OATS":
                    buttonLoadData.IsEnabled = true;
                    Parser.IsEnabled = true;
                    break;
#endif
                case "Script JAVA":
                    buttonLoadData.IsEnabled = true;
                    Parser.IsEnabled = true;
                    break;
#endif
#if PL_XMI
                case "Astah XML":
                    buttonLoadData.IsEnabled = true;
                    Parser.IsEnabled = true;
                    break;
                case "Argo XML":
                    buttonLoadData.IsEnabled = true;
                    Parser.IsEnabled = true;
                    break;
                case "Enterprise Architect":
                    buttonLoadData.IsEnabled = true;
                    Parser.IsEnabled = true;
                    break;
#endif
#if PL_LR
                case "LoadRunnerToXMI":
                    buttonLoadData.IsEnabled = true;
                    Parser.IsEnabled = true;
                    break;
#endif
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="parsedSuccessfully"></param>
        private void Validate(String filename, Boolean parsedSuccessfully)
        {
            if (parsedSuccessfully)
            {
                MainWindow.LogAppend("Validating...");

                List<KeyValuePair<String, Int32>> errors = control.ValidateModel(filename);

                int mess = 0, warn = 0;
                foreach (KeyValuePair<String, Int32> error in errors)
                {
                    if (error.Value == 3) //error
                    {
                        MainWindow.LogAppend(error.Key, ErrorLevel.Critical);
                        mess++;
                    }
                    else //warning
                    {
                        MainWindow.LogAppend(error.Key, ErrorLevel.Warning);
                        warn++;
                    }
                }
                if (mess > 0)
                {
                    buttonGenerateTestCases.IsEnabled = false;
                    GenerateFile.IsEnabled = false;
                    buttonXmiExport.IsEnabled = true;
                    //buttonParseXMItoXLS.IsEnabled = false;
                    //buttonMTM.IsEnabled = false;
                    //buttonLoadRunner.IsEnabled = false;   
                    fatalError = true;
                }
                else if (warn > 0)
                {
                    //MainWindow.LogAppend("The next steps may not generate the expected results.", ErrorLevel.Warning);
                    buttonGenerateTestCases.IsEnabled = true;
                    GenerateFile.IsEnabled = false;
#if PL_OATS
                    GenerateFile.IsEnabled = true;
#endif
                    buttonXmiExport.IsEnabled = true;
                    //buttonParseXMItoXLS.IsEnabled = true;
                    //buttonMTM.IsEnabled = false;
                    //buttonLoadRunner.IsEnabled = false;
                    fatalError = false;
                }
                else
                {
                    MainWindow.LogAppend("Parsing finished without issues.", ErrorLevel.Green);
                    buttonGenerateTestCases.IsEnabled = false;

                    switch (parserType)
                    {
#if PL_OATS
#if AUX
                        case "Xmi to OATS":
                            buttonGenerateTestCases.IsEnabled = false;
                            GenerateFile.IsEnabled = true;
                            break;
#endif
                        case "Script JAVA":
                            buttonGenerateTestCases.IsEnabled = false;
                            GenerateFile.IsEnabled = true;
                            break;
                        case "Astah XML":
                            buttonGenerateTestCases.IsEnabled = false;
                            GenerateFile.IsEnabled = true;
                            break;
#elif PL_XMI
                        case "Astah XML":
                            buttonGenerateTestCases.IsEnabled = true;
                            GenerateFile.IsEnabled = false;
                            break;
                        case "Argo XML":
                            buttonGenerateTestCases.IsEnabled = true;
                            GenerateFile.IsEnabled = false;
                            break;
                        case "Enterprise Architect":
                            buttonGenerateTestCases.IsEnabled = true;
                            GenerateFile.IsEnabled = false;
                            break;
#endif
#if PL_LR
                        case "LoadRunnerToXMI":
                            buttonGenerateTestCases.IsEnabled = false;
                            break;
#endif
                    }
                    buttonXmiExport.IsEnabled = true;
                    //buttonParseXMItoXLS.IsEnabled = true;
                    //buttonMTM.IsEnabled = false;
                    //buttonLoadRunner.IsEnabled = false;
                    fatalError = false;
                }
            }
            else
            {
                MainWindow.LogAppend("[ERROR] Some critical error was found while parsing XMI file.", ErrorLevel.Critical);
                buttonGenerateTestCases.IsEnabled = false;
                GenerateFile.IsEnabled = false;
                //buttonParseXMItoXLS.IsEnabled = false;
                //buttonMTM.IsEnabled = false;
                //buttonLoadRunner.IsEnabled = false;
                buttonXmiExport.IsEnabled = false;
                return;
            }
        }

        /// <summary>
        /// Appends a message to log window.
        /// </summary>
        public static void LogAppend(String s)
        {
            MainWindow.LogAppend(s, ErrorLevel.Message);
        }

        /// <summary>
        /// Appends a message to log window.
        /// </summary>
        public static void LogAppend(String s, MainWindow.ErrorLevel level)
        {
            //as users are unforeseen beings, we need to have sure that the pointer is at the log's end
            MainWindow.textBlockLog.CaretPosition = MainWindow.textBlockLog.Document.ContentEnd;
            MainWindow.textBlockLog.ScrollToEnd();
            TextRange range = new TextRange(textBlockLog.CaretPosition, textBlockLog.Document.ContentEnd);

            //adjust coloring according to log level.
            switch (level)
            {
                case ErrorLevel.Critical:
                    range.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.OrangeRed);
                    break;
                case ErrorLevel.Warning:
                    range.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Brown);
                    break;
                case ErrorLevel.Green:
                    range.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Green);
                    break;
                default:
                    range.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Black);
                    break;
            }

            //appends a new line to current document.
            MainWindow.textBlockLog.AppendText("[" + DateTime.Now.ToShortTimeString() + "] " + s + Environment.NewLine);
            MainWindow.textBlockLog.CaretPosition = MainWindow.textBlockLog.Document.ContentEnd;
            MainWindow.textBlockLog.ScrollToEnd();
        }

        /// <summary>
        /// Save log to file.
        /// </summary>
        private void buttonSaveLog_Click(Object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Text file (*.txt) | *.txt";

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            TextRange textRange = new TextRange(
               this.textBlockLogContainer.Document.ContentStart,//TextPointer to the start of content in the RichTextBox.
               this.textBlockLogContainer.Document.ContentEnd   //TextPointer to the end of content in the RichTextBox.
            );
            File.WriteAllText(dialog.FileName, textRange.Text);
        }

        /// <summary>
        /// Clears log window.
        /// </summary> 
        private void buttonClearLog_Click(Object sender, RoutedEventArgs e)
        {
            TextRange textRange = new TextRange(
               this.textBlockLogContainer.Document.ContentStart,//TextPointer to the start of content in the RichTextBox.
               this.textBlockLogContainer.Document.ContentEnd   //TextPointer to the end of content in the RichTextBox.
            );

            this.textBlockLogContainer.Document.Blocks.Clear();
            MainWindow.LogAppend("Waiting for file containing test data.\n");
        }

        /// <summary>
        /// Opens configuration file.
        /// </summary>
        private void buttonConfigure_Click(Object sender, RoutedEventArgs e)
        {
            Process p = new Process();

            ProcessStartInfo i = new ProcessStartInfo();
            i.Arguments = System.IO.Path.Combine(MainWindow.CurrentPath + "\\Configuration.cfg");
            i.FileName = "notepad.exe";

            p.StartInfo = i;
            p.Start();
        }

        private void buttonOption_Click(Object sender, RoutedEventArgs e)
        {
            ConfigurationForm cf = new ConfigurationForm();
            cf.Show();

        }

        /// <summary>
        /// Quits.
        /// </summary>
        private void buttonClose_Click(Object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Generate test cases from loaded structure. Note that
        /// generation depends on active features.
        /// </summary>
        private void buttonGenerateTestCases_Click(Object sender, RoutedEventArgs e)
        {
            try
            {
                control.GenerateSequence(type);
                GenerateFile.IsEnabled = true;
                //buttonMTM.IsEnabled = true;
                buttonGenerateTestCases.IsEnabled = false;
                buttonXmiExport.IsEnabled = false;
                MainWindow.LogAppend(control.TestCaseCount + " test cases have been generated.", ErrorLevel.Message);
                MainWindow.LogAppend("There are test cases ready to be generated. Press {Export...} to proceed.", ErrorLevel.Green);
            }
            catch (Exception)
            {
                MainWindow.LogAppend("Error generating test plans.", ErrorLevel.Critical);
                buttonGenerateTestCases.IsEnabled = false;
                buttonXmiExport.IsEnabled = false;
            }
        }

#if PL_MTM
        private void buttonMTM_Click(Object sender, RoutedEventArgs e)
        {
            String destPath = null;
            try
            {
                if (control.TestCaseCount < 1)
                {
                    MainWindow.LogAppend("No test plan is ready to be generated. Aborting.", ErrorLevel.Critical);
                    return;
                }

                destPath = String.Empty;
                destPath = saveFile(HttpUtility.UrlDecode(control.Name));
                //If is a valid path
                if (destPath != null)
                {
                    String archiveName = verifyOpenFiles(destPath);

                    if (archiveName == "dontHave")
                    {
                        control.GenerateScript(destPath);
                        buttonXmiExport.IsEnabled = false;
                        MainWindow.LogAppend("Saved in " + destPath, ErrorLevel.Green);
                        System.Diagnostics.Process.Start("explorer.exe", destPath);
                    }
                    else
                    {
                        MessageBox.Show(archiveName + " is opened");
                    }
                }
            }
            catch (Exception ex)
            {
                Directory.Delete(destPath, true);
                MainWindow.LogAppend("Error exporting files: " + ex.Message, ErrorLevel.Critical);
            }
        }

        /// <summary>
        /// Save File dialog
        /// </summary>
        /// <param name="modelName"></param>
        /// <returns></returns>
        private String saveFile(String modelName)
        {
            String directory = Environment.CurrentDirectory + "\\Result Files";
            String directory_aux = "";
            //verifica se diretório não existe
            //control.listScriptGenerator

            if (!Directory.Exists(directory + "\\" + modelName))
            {
                directory_aux = directory + "\\" + modelName;
                Directory.CreateDirectory(directory_aux);
            }
            else
            {
                String name;
                String[] sub;
                int i = 1;
                //se ele existe cria um só que com numeração diferente
                String[] filePaths = Directory.GetDirectories(directory);
                foreach (String path in filePaths)
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
                            {
                                i = Convert.ToInt16(sub[1]);
                            }
                        }
                    }
                }
                directory_aux = Environment.CurrentDirectory + "\\Result Files\\" + modelName + "(" + (i + 1) + ")";
                Directory.CreateDirectory(directory_aux);
                i++;
            }

            var dlg = new System.Windows.Forms.FolderBrowserDialog();
            dlg.Description = "Choose destiny folder";
            dlg.ShowNewFolderButton = true;
            dlg.Reset();
            dlg.SelectedPath = directory;

            if (System.Windows.Forms.DialogResult.OK == dlg.ShowDialog())
            {
                String folder = dlg.SelectedPath;
                if (folder == directory)
                {
                    return directory_aux;
                }
                else
                {
                    Directory.Delete(directory_aux, true);

                    if (Directory.GetDirectories(directory).Length == 0)
                    {
                        Directory.Delete(directory, true);
                    }

                    if (!folder.Contains("Result Files"))
                    {
                        folder = folder + "\\Result Files";
                    }

                    String folder_aux = "";

                    //verifica se diretório não existe
                    if (!Directory.Exists(folder + "\\" + modelName))
                    {
                        folder_aux = folder + "\\" + modelName;
                        Directory.CreateDirectory(folder_aux);
                        return folder_aux;
                    }
                    else
                    {
                        String name;
                        String[] sub;
                        int i = 1;
                        //se ele existe cria um só que com numeração diferente
                        String[] filePaths = Directory.GetDirectories(folder);
                        foreach (String path in filePaths)
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
                                    {
                                        i = Convert.ToInt16(sub[1]);
                                    }
                                }
                            }
                        }
                        folder_aux = folder + "\\" + modelName + "(" + (i + 1) + ")";
                        Directory.CreateDirectory(folder_aux);
                        i++;

                        return folder_aux;
                    }
                }
            }
            else
            {
                Directory.Delete(directory_aux);
            }
            return null;
        }

        /// <summary>
        /// Verify if have equals open files in system
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private String verifyOpenFiles(String filePath)
        {
            String fileOpened = "dontHave";

            if (Directory.GetFiles(filePath) == null)
            {
                return fileOpened;
            }

            String[] filePaths = Directory.GetFiles(filePath);

            foreach (String path in filePaths)
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
#endif
#if PL_OATS
        private void buttonParseXMItoXLS_Click(Object sender, RoutedEventArgs e)
        {
            try
            {
                String directory = Environment.CurrentDirectory + "\\Result Files\\";
                //verifica se diretório não existe
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                control.GenerateSequence(type);
                control.GenerateScript("");
                MainWindow.LogAppend("XLS files saved at " + directory, ErrorLevel.Green);
                MainWindow.SetStatus("Done.");
                System.Diagnostics.Process.Start("explorer.exe", directory);
            }
            catch (Exception ex)
            {
                MainWindow.LogAppend(ex.Message, ErrorLevel.Critical);
            }
        }
#endif
        private void buttonParseLRtoXMI_Click(Object sender, RoutedEventArgs e)
        {
            Boolean cSelected = false;
            Boolean prmSelected = false;
            OpenFileDialog cDialog = new OpenFileDialog();
            cDialog.Filter = "Open .C files (*.c) |*.c;";

            if (cDialog.ShowDialog() != true)
            {
                cFilePath = null;
                return;
            }
            else
            {
                cFilePath = cDialog.FileName;
                cSelected = true;
                MessageBox.Show(".c file successfully loaded.", "Load .c file", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }

            if (cSelected)
            {
                OpenFileDialog prmDialog = new OpenFileDialog();
                prmDialog.Filter = "Open .PRM files (*.prm) |*.prm;";

                if (prmDialog.ShowDialog() != true)
                {
                    cFilePath = null;
                    prmFilePath = null;
                    return;
                }
                else
                {
                    prmFilePath = prmDialog.FileName;
                    prmSelected = true;
                    MessageBoxResult result = MessageBox.Show(".prm file successfully loaded. Do you want to generate the .xmi file?", "Load .prm File", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);
                    switch (result)
                    {
                        case MessageBoxResult.Yes:
                            //do something
                            break;
                        case MessageBoxResult.No:
                            //do something
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void buttonXmiExport_Click(Object sender, RoutedEventArgs e)
        {
            XmlDocument document = new XmlDocument();
            XmlWriterSettings settings = new XmlWriterSettings();

            document = control.ExportParsedStructure();

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "XML file (*.xml) | *.xml";
            if (dialog.ShowDialog() != true)
            {
                return;
            }

            settings.Encoding = new UTF8Encoding(false);
            settings.Indent = true;
            settings.CheckCharacters = true;
            using (XmlWriter writer = XmlWriter.Create(dialog.FileName, settings))
                document.Save(writer);

            MainWindow.LogAppend("XMI file saved in " + dialog.FileName, ErrorLevel.Green);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

    }
}