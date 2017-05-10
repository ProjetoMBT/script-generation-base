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
using PlugSpl.Modules;
using PlugSpl.DataStructs.UmlComponentDiagram;
using PlugSpl.DataStructs.ProductConfigurator;
using PlugSpl.DataStructs.ComponentPoolManager;
using PlugSpl;
using Microsoft.Win32;
using System.IO;
using System.Xml.Serialization;
using System.Reflection;

namespace ComponentPoolManager
{
    /// <summary>
    /// Interaction logic for PoolManager.xaml
    /// </summary>
    public partial class PoolManager : UserControl, IComponentPoolManager
    {

        private MainWindow CurrentApplicationWindow { get; set; }
        private DanuProductConfigurator danu = null;
        private List<Type> codeHandlers = new List<Type>();
        private string lastPlugCD;
        #region constructors

        public PoolManager()
        {
            InitializeComponent();
            lastPlugCD = "";
        }

        #endregion
        #region interface implementation
        public void SetApplicationWindow(PlugSpl.MainWindow mainWindow)
        {
            this.CurrentApplicationWindow = mainWindow;
        }

        #endregion
        #region internal and private methods

        /// <summary>
        /// Loads Danu data into display;
        /// </summary>
        private void loadDanuIntoGui()
        {
            this.stackPanelComponents.Children.Clear();
            foreach (DanuComponent d in this.danu.Components)
            {
                ComponentControl c = new ComponentControl();
                c.ParentManager = this;
                c.UpdateInfo(d);
                this.stackPanelComponents.Children.Add(c);
            }
        }


        #endregion
        #region event handlers and delegates

        /// <summary>
        /// Save Danu struct to disk.
        /// </summary>
        private void buttonSaveConfig_Click(object sender, RoutedEventArgs e)
        {

            string oldDirectory = Environment.CurrentDirectory;

            try
            {
                //structure must be loaded before saving actions.
                if (this.danu == null)
                    return;

                //adds default extension to the open file dialog.
                SaveFileDialog dialog = new SaveFileDialog() { Filter = "PlugSPL Component Metadata (*.plugcm)|*.plugcm" };

                ////dialog.InitialDirectory = Environment.CurrentDirectory;
                if (lastPlugCD.Length > 0)
                {
                    dialog.FileName = lastPlugCD + ".plugcm";
                }
                else
                {
                    dialog.FileName = "newConfig.plugcm";
                }

                if (dialog.ShowDialog() != true)
                    return;

                FileInfo f = new FileInfo(dialog.FileName);
                DirectoryInfo d = f.Directory;
                if (!d.Name.Equals(f.Name.Substring(0, f.Name.LastIndexOf('.'))))
                    throw new Exception("File name must be the same name of the save directory, i.e. " + d.Name);

                //if selected file already exists, dele it before create a new one.
                if (File.Exists(dialog.FileName))
                    File.Delete(dialog.FileName);

                //Instantiate serializer and reader.
                XmlSerializer serializer = new XmlSerializer(typeof(DanuProductConfigurator));
                TextWriter writer = null;
                try
                {
                    writer = new StreamWriter(dialog.FileName);

                    List<string> aux = dialog.FileName.Split('.').Reverse().ToList();
                    string projectLowerName = aux[1];
                    aux = projectLowerName.Split('\\').ToList();
                    projectLowerName = aux.Last();

                    string newCurrent = Path.Combine(oldDirectory, "./Projects/" + projectLowerName);

                    if (!Directory.Exists(newCurrent))
                        Directory.CreateDirectory(newCurrent);

                    Environment.CurrentDirectory = newCurrent;

                    string cacheDir = Path.Combine(Environment.CurrentDirectory, "Cache");
                    if (!Directory.Exists(cacheDir))
                        Directory.CreateDirectory(cacheDir);

                    //Do serialization and dispose reader.
                    serializer.Serialize(writer, this.danu);
                    writer.Close();
                }
                catch (Exception ex)
                {
                    if (writer != null)
                        writer.Close();
                    
                    throw ex;
                }

                Environment.CurrentDirectory = oldDirectory;

                //Locate work directory (dir where the file was saved)
                char[] pathChars = dialog.FileName.Reverse().ToArray();
                string pathName = new String(pathChars);
                pathName = pathName.Substring(pathName.IndexOf("\\"), pathName.Length - pathName.IndexOf("\\"));
                pathChars = pathName.Reverse().ToArray();
                pathName = new String(pathChars);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "PlugSPL Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Environment.CurrentDirectory = oldDirectory;
            }
        }

        /// <summary>
        /// Load a danu.xml file.
        /// </summary>
        private void buttonOpenConfig_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "PlugSPL Component Metadata (*.plugcm)|*.plugcm";

                if (dialog.ShowDialog() == true)
                {
                    using (TextReader reader = new StreamReader(dialog.FileName))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(DanuProductConfigurator));

                        string projectLowerDirectory = dialog.FileName.Split('\\').Last().Split('.').First();
                        string oldDirectory = Environment.CurrentDirectory;
                        Environment.CurrentDirectory = oldDirectory + "/Projects/" + projectLowerDirectory;

                        DanuProductConfigurator productConfigurator = null;
                        try
                        {
                            productConfigurator = (DanuProductConfigurator)serializer.Deserialize(reader);
                        }
                        catch (Exception ex)
                        {
                            productConfigurator = null;
                            MessageBox.Show(ex.Message, "PlugSPL Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }

                        if (productConfigurator != null)
                        {
                            this.danu = productConfigurator;
                            this.loadDanuIntoGui();
                        }
                        else
                        {
                            this.textBlockStatus.Text = "Unable to parser [" + dialog.FileName + "] file.";
                        }

                        reader.Close();

                        Environment.CurrentDirectory = oldDirectory;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "PlugSPL Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        /// <summary>
        /// Import bragi file and builds an Danu struct with it.
        /// </summary>
        private void buttonLoadDanu_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "UML Component Diagram / Smarty (*.plugcd)|*.plugcd";

            if (dialog.ShowDialog() != true)
                return;

            ComponentDiagramBragi plugcd = null;
            XmlSerializer serializer = new XmlSerializer(typeof(ComponentDiagramBragi));
            using (TextReader reader = new StreamReader(dialog.FileName))
            {
                try
                {
                    plugcd = (ComponentDiagramBragi)serializer.Deserialize(reader);
                }
                catch (Exception)
                {
                    MessageBox.Show("Wrong data format. Given file has no valid structure inside it.",
                        "PlugSPL Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            if (plugcd == null)
                return;

            lastPlugCD = dialog.FileName;
            lastPlugCD = lastPlugCD.Split('\\').Last().Split('.')[0];

            this.danu = new DanuProductConfigurator(plugcd);
            this.loadDanuIntoGui();
        }

        /// <summary>
        /// Load combo box values. Search app assemblies for types which implements 
        /// IPoolManagerCodeHandler interface.
        /// </summary>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.codeHandlers.Count() > 0)
                return;

            try
            {
                string[] asmFiles = Directory.GetFiles("./", "ComponentPoolManager.*.dll");
                foreach (string s in asmFiles)
                {
                    Assembly asm = Assembly.LoadFrom(s);
                    foreach (Type p in asm.GetTypes())
                    {
                        Type type = typeof(IPoolManagerCodeHandler);
                        if (type.IsAssignableFrom(p))
                            this.codeHandlers.Add(p);
                    }
                }

                List<String> handlerNames = new List<string>();
                this.comboBoxCodeHandlers.Items.Clear();
                foreach (Type p in this.codeHandlers)
                {
                    IPoolManagerCodeHandler handler = (IPoolManagerCodeHandler)Activator.CreateInstance(p);
                    if (handler != null)
                        this.comboBoxCodeHandlers.Items.Add(handler.GetName());
                }

                this.comboBoxCodeHandlers.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "PlugSPL Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion


        internal void LoadInterfaceEditor(InterfaceEditor editor)
        {
            this.interfaceEditorContainer.Children.Clear();
            this.interfaceEditorContainer.Children.Add(editor);
        }

        private void buttonUpdateHandler_Click(object sender, RoutedEventArgs e)
        {

            if (this.danu == null)
                return;
            try
            {
                foreach (Type p in this.codeHandlers)
                {
                    IPoolManagerCodeHandler handler = (IPoolManagerCodeHandler)Activator.CreateInstance(p);

                    if (handler != null && handler.GetName() == comboBoxCodeHandlers.Text)
                    {

                        string oldDirectory = Environment.CurrentDirectory;
                        Environment.CurrentDirectory = Path.Combine(oldDirectory, "./Projects/" + this.danu.Root.Name.ToLower());

                        handler.GenerateCode(this.danu);
                        textBlockStatus.Text = "Code stub generated succefully.";

                        Environment.CurrentDirectory = oldDirectory;

                        return;
                    }
                }
                textBlockStatus.Text = "Unable to generate code. A suitable code generator could not be found.";
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "PlugSPL Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
           

        }
    }
}
