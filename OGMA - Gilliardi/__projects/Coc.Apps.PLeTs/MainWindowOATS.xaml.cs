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
    /// Interaction logic for MainWindowOATS.xaml
    /// </summary>
    public partial class MainWindowOATS : Window
    {
        public enum ErrorLevel
        {
            Critical,
            Warning,
            Message,
            Green
        }


        public enum SoftwareNames
        {
            OATS, Excel, Astah
        }



        private ControlUnit.ControlUnit control;
        private Boolean fatalError = false;
        private String cFilePath;
        private String prmFilePath;
        private String value = "";
        private String parserType = "";
        private StructureType type;
        private string importedFileName;


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
        public MainWindowOATS()
        {
            InitializeComponent();
            MainWindowOATS.SetInstance(this);
            MainWindowOATS.CurrentPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            MainWindowOATS.SetStatus("Ready.");
            cmbExportType.IsEnabled = false;
            btnExport.IsEnabled = false;
            importedFileName = "";

            Configuration.getInstance().setConfiguration(
                Configuration.Fields.softwareversion, 
                Assembly.GetExecutingAssembly().GetName().Version.ToString(3)
            );

            #region TITLE HANDLING
#if PL_FUNCTIONAL_TESTING
            //this.Title = "Functional Testing Tool for ";
            this.Title = "PLeTsFunc - A Model-Based Functional Testing Tool for ";
#endif
            #endregion


            #region OATS
#if PL_OATS
            this.Title += "Oracle ATM Open Script";
            
            control = new ControlUnit.ControlUnit(StructureType.OATS);
            type = StructureType.OATS;

#endif
            #endregion
                        
            
            //adds build number to title bar
            this.Title += (" - v" + Configuration.getInstance().getConfiguration(Configuration.Fields.softwareversion));
            this.buttonClearLog_Click(null, null);
        }

        /// <summary>
        /// Singleton implementation.
        /// </summary>
        private static void SetInstance(MainWindowOATS mainWindow)
        {
            MainWindowOATS.textBlockLog = mainWindow.textBlockLogContainer;
            MainWindowOATS.textBlockStatus = mainWindow.textBlockStatusContainer;
        }

        /// <summary>
        /// Update status bar information.
        /// </summary>
        private static void SetStatus(String p)
        {
            MainWindowOATS.textBlockStatus.Text = p;
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
                MainWindowOATS.LogAppend("Validating...");

                List<KeyValuePair<String, Int32>> errors = control.ValidateModel(filename);

                int mess = 0, warn = 0;
                foreach (KeyValuePair<String, Int32> error in errors)
                {
                    if (error.Value == 3) //error
                    {
                        MainWindowOATS.LogAppend(error.Key, ErrorLevel.Critical);
                        mess++;
                    }
                    else //warning
                    {
                        MainWindowOATS.LogAppend(error.Key, ErrorLevel.Warning);
                        warn++;
                    }
                }
                if (mess > 0)
                {
                    buttonGenerateTestCases.IsEnabled = false;
                    fatalError = true;
                }
                else if (warn > 0)
                {
                    //MainWindowOATS.LogAppend("The next steps may not generate the expected results.", ErrorLevel.Warning);
                    buttonGenerateTestCases.IsEnabled = true;
                    fatalError = false;
                }
                else
                {
                    MainWindowOATS.LogAppend("Parsing finished without issues.", ErrorLevel.Green);
                    buttonGenerateTestCases.IsEnabled = false;

                    switch (parserType)
                    {

#if AUX
                        case "Xmi to OATS":
                            buttonGenerateTestCases.IsEnabled = false;
                            break;
#endif
                        case "Script JAVA":
                            buttonGenerateTestCases.IsEnabled = false;
                            break;
                        case "Astah XML":
                            buttonGenerateTestCases.IsEnabled = false;
                            break;

                    }

                    fatalError = false;
                }
            }
            else
            {
                MainWindowOATS.LogAppend("[ERROR] Some critical error was found while parsing XMI file.", ErrorLevel.Critical);
                buttonGenerateTestCases.IsEnabled = false;
                return;
            }
        }

        /// <summary>
        /// Appends a message to log window.
        /// </summary>
        public static void LogAppend(String s)
        {
            MainWindowOATS.LogAppend(s, ErrorLevel.Message);
        }

        /// <summary>
        /// Appends a message to log window.
        /// </summary>
        public static void LogAppend(String s, MainWindowOATS.ErrorLevel level)
        {
            //as users are unforeseen beings, we need to have sure that the pointer is at the log's end
            MainWindowOATS.textBlockLog.CaretPosition = MainWindowOATS.textBlockLog.Document.ContentEnd;
            MainWindowOATS.textBlockLog.ScrollToEnd();
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
            MainWindowOATS.textBlockLog.AppendText("[" + DateTime.Now.ToShortTimeString() + "] " + s + Environment.NewLine);
            MainWindowOATS.textBlockLog.CaretPosition = MainWindowOATS.textBlockLog.Document.ContentEnd;
            MainWindowOATS.textBlockLog.ScrollToEnd();
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
            MainWindowOATS.LogAppend("Waiting for file containing test data.\n");
        }

        /// <summary>
        /// Opens configuration file.
        /// </summary>
        private void buttonConfigure_Click(Object sender, RoutedEventArgs e)
        {
            Process p = new Process();

            ProcessStartInfo i = new ProcessStartInfo();
            i.Arguments = System.IO.Path.Combine(MainWindowOATS.CurrentPath + "\\Configuration.cfg");
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

        private void buttonHelp_Click(Object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.Help.ShowHelp(null, "Help\\Documentation.chm");
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
                //buttonMTM.IsEnabled = true;
                buttonGenerateTestCases.IsEnabled = false;
                MainWindowOATS.LogAppend(control.TestCaseCount + " test cases have been generated.", ErrorLevel.Message);
                MainWindowOATS.LogAppend("There are test cases ready to be generated. Press {Export...} to proceed.", ErrorLevel.Green);
            }
            catch (Exception)
            {
                MainWindowOATS.LogAppend("Error generating test plans.", ErrorLevel.Critical);
                buttonGenerateTestCases.IsEnabled = false;
            }
        }


        private void buttonParseLRtoXMI_Click(Object sender, RoutedEventArgs e)
        {
            Boolean cSelected = false;
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




        private void lockExport()
        {
            cmbExportType.IsEnabled = false;
            btnExport.IsEnabled = false;
        }

        
        private void btnImportJava_Click(object sender, RoutedEventArgs e)
        {
            importedFileName = openDialog("Open Java files (*.java) |*.java|All files (*.*)|*.*");

            if (parseFile(importedFileName, "Script JAVA"))
            {
                //cmbExportType.IsEnabled = true;
                exportXmi();
            }

        }

        private void btnImportAstahXmi_Click(object sender, RoutedEventArgs e)
        {

            importXmi();
            /*
            importedFileName = openDialog("Open XMI project files (*.xmi, *.xml) |*.xmi;*.xml|All files (*.*)|*.*");

            if (parseFile(importedFileName, "Astah XML"))
            {
                //cmbExportType.IsEnabled = true;
            }*/
        }


        private void cmbExportType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (cmbExportType.SelectedIndex >= 0)
            {
                btnExport.IsEnabled = true;
            }
            else
            {
                btnExport.IsEnabled = false;
            }
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            int idx = cmbExportType.SelectedIndex;
            switch(idx){
                case 0:
                    exportXmi();
                    break;
                    
                case 1:
                    exportXls();
                    break;

                case 2:
                    importXmi();
                    break;
            }

        }

        private void exportXmi()
        {
            string fName = null;
            try
            {
                XmlDocument document = new XmlDocument();
                XmlWriterSettings settings = new XmlWriterSettings();

                document = control.ExportParsedStructure();

                fName = importedFileName.Substring(0, importedFileName.LastIndexOf("."));
                fName = Configuration.getInstance().getConfiguration(Configuration.Fields.workspacepath) + fName.Substring(fName.LastIndexOf("\\") + 1) + ".xml";
                
                settings.Encoding = new UTF8Encoding(false);
                settings.Indent = true;
                settings.CheckCharacters = true;
                using (XmlWriter writer = XmlWriter.Create(fName, settings))
                    document.Save(writer);

                MainWindowOATS.LogAppend("XMI file saved in " + fName, ErrorLevel.Green);
            }
            catch (Exception e)
            {
                MainWindowOATS.LogAppend("XMI file error: " + e.StackTrace + " - " + fName, ErrorLevel.Critical);
            }
        }

        private void exportXls()
        {
            try
            {
                // Environment.CurrentDirectory
                String directory = Configuration.getInstance().getConfiguration(Configuration.Fields.workspacepath) + "Result Files\\";
                //verifica se diretório não existe
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                control.GenerateSequence(type);
                control.GenerateScript("");
                MainWindowOATS.LogAppend("XLS files saved at " + directory, ErrorLevel.Green);
                MainWindowOATS.SetStatus("Done.");
                System.Diagnostics.Process.Start("explorer.exe", directory);
            }
            catch (Exception ex)
            {
                MainWindowOATS.LogAppend(ex.Message, ErrorLevel.Critical);
            }
        }

        private void importXmi()
        {
            string fName = openDialog("Open XMI project files (*.xmi, *.xml) |*.xmi;*.xml|All files (*.*)|*.*");

            if (parseFile(fName, "Xmi to OATS"))
            {
               
            }
        }

        private void btnOpenExcel_Click(object sender, RoutedEventArgs e)
        {
            openSoftware(SoftwareNames.Excel);
        }

        private void btnOpenAstah_Click(object sender, RoutedEventArgs e)
        {
            openSoftware(SoftwareNames.Astah);
        }

        private void btnOpenOATS_Click(object sender, RoutedEventArgs e)
        {
            openSoftware(SoftwareNames.OATS);
        }


        private void openSoftware(SoftwareNames software)
        {
            try
            {
                string path = "";
                switch (software)
                {
                    case SoftwareNames.Astah:
                        path = Configuration.getInstance().getConfiguration(Configuration.Fields.astahpath);
                        if (!path.EndsWith("\\"))
                        {
                            path += "\\";
                        }
                        path += "astah-pro.exe";

                        break;

                    case SoftwareNames.Excel:
                        path = "Excel";
                        break;

                    case SoftwareNames.OATS:
                        path = Configuration.getInstance().getConfiguration(Configuration.Fields.oatspath);
                        if (!path.EndsWith("\\"))
                        {
                            path += "\\";
                        }
                        path += "OpenScript.exe";

                        break;
                }

                Process.Start(path);
            }
            catch (Exception e)
            {
            }

        }
        

        private string openDialog(string filter)
        {
            string fileName = "";
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = filter;
            if (dialog.ShowDialog() == true)
            {
                fileName = dialog.FileName;
            }

            return fileName;
        }

        private bool parseFile(string fileName, string parserType)
        {
            bool parsedSuccessfully = true;
            MainWindowOATS.LogAppend(fileName);
            MainWindowOATS.LogAppend("Initializing file parsing...");

            try
            {
                control.LoadModelingStructure(fileName, parserType);
                Validate(fileName, parsedSuccessfully);
            }
            catch (IOException ioe)
            {
                parsedSuccessfully = false;
                MainWindowOATS.LogAppend(ioe.Message, ErrorLevel.Critical);
                MainWindowOATS.LogAppend("Correlation file is being used by another process. Please close it and reload the XMI file.", ErrorLevel.Critical);

            }
            catch (Exception ex)
            {
                MainWindowOATS.LogAppend("[ERROR] " + ex.Message, ErrorLevel.Critical);
                parsedSuccessfully = false;
            }

            return parsedSuccessfully;
        }

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }

    }
}