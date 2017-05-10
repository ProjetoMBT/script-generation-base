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

using Coc.Modeling.Uml;
using Coc.Modeling.Uml.Xmi;
//using Coc.Data.Xmi;

using Fiddler;
using System.Xml;
using System.Reflection;
using System.IO;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace Coc.Apps.Capturer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Interop - Used remove proxy from system
        [DllImport("wininet.dll")]
        public static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);
        public const int INTERNET_OPTION_SETTINGS_CHANGED = 39;
        public const int INTERNET_OPTION_REFRESH = 37;
        static bool settingsReturn, refreshReturn;
        #endregion
        
        /// <summary>
        /// Stores current proxy state.
        /// </summary>
        private bool isProxyRunning = false;

        /// <summary>
        /// Store requested URLs
        /// </summary>
        private List<String> lista = new List<String>();

        /// <summary>
        /// Delegate used by fiddler. Should not be manipulated "by hand".
        /// </summary>
        delegate void UpdateUI();

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Event listener to fiddler application.
        /// </summary>
        /// <param name="session"></param>
        private void FiddlerApplication_AfterSessionComplete(Fiddler.Session session)
        {
            myListBox.Dispatcher.BeginInvoke(new UpdateUI(() =>
            {

                String fullURL = session.fullUrl;
                if (fullURL.Length != 0)
                {
                    fullURL = Filter.iShttps(fullURL);
                    if (Filter.FitrarPictures(fullURL) == false)
                    {
                        lista.Add(fullURL);
                        myListBox.Items.Add(fullURL);
                    }
                }
            }));
        }

        /// <summary>
        /// Adds the handler to Fiddler application at window loading. 
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FiddlerApplication.AfterSessionComplete += FiddlerApplication_AfterSessionComplete;
            this.isProxyRunning = false;
        }

        /// <summary>
        /// Stops Fiddler proxy, restores proxy settings and prompt to save generated file.
        /// </summary>
        private void buttonStop_Click(object sender, RoutedEventArgs e)
        {
            this.isProxyRunning = false;

            //Stops Fiddler application.
            Fiddler.FiddlerApplication.Shutdown();

            //Disable proxy in windows register.
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);
            regKey.SetValue("ProxyEnable", 0);

            //Refresh system info. (So we don't need to restart windows).
            settingsReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
            refreshReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);

            //Exports data to Uml Model.
            UmlModel model = this.generateXmlFromNavigationData(lista);
            
            //Creates dialog to set save path
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "XMI Document (*.xmi) | *.xmi";

            //Abort saving if user close or cancel.
            if(dialog.ShowDialog() != true)
                return;

            //Call api to export XMI data.
            XmlDocument document = model.ToXmi();
            
            //XmlDocument doc = new XmlDocument();
            //doc.Load(@"C:\Users\COC-DELL-20\Desktop\teste.xmi");
            //UmlModel modelNew = model.FromXmi(doc);

            //Initializer a new XML writer.
            XmlWriterSettings settings = new XmlWriterSettings(){
                Encoding = new UTF8Encoding(false),
                Indent = true,
                CheckCharacters = true
            };
            
            //Save generated XML file to disk.
            using (XmlWriter writer = XmlWriter.Create(dialog.FileName, settings))
                document.Save(writer);

            //Send message to screen.
            MessageBox.Show(
                File.Exists(dialog.FileName) //check for file existance.
                ? "File succesfully exported."
                : "Couldn't export file.");        
        }

        /// <summary>
        /// Converts a list of URLs into a UML Model.
        /// </summary>
        public UmlModel generateXmlFromNavigationData(List<String> lista)
        {
            List<String> listaUrl = new List<string>();
            UmlModel model = new UmlModel("Activity+Diagram0");
            UmlActivityDiagram diagram = new UmlActivityDiagram("Activity0");
            model.AddDiagram(diagram);
            UmlInitialState initial = new UmlInitialState() { Name = "Initial0" };
            UmlTransition transition = new UmlTransition();
            transition.End1 = initial;
            diagram.UmlObjects.Add(initial);
            diagram.UmlObjects.Add(transition);
            
            UmlLane lane = new UmlLane();
       
            for (int i = 0; i < lista.Count; i++){

                String url = Filter.iShttps(lista[i]);
                String urlParam = url;

                if (Filter.isGet(url)==true)
                    transition.SetTaggedValue(new UmlTag(null, "TDmethod"), "GET");

                urlParam = Filter.filtraParam(urlParam);
                url = Filter.TDactionValue;

                transition.SetTaggedValue(new UmlTag(null, "TDaction"), url);
                    string urlFilter=Filter.filtrarNome(url);
                    listaUrl.Add(urlFilter);
                UmlElement element = new UmlActionState() {

                    Name = urlFilter+Filter.contadorUrl(listaUrl,urlFilter)
                };
                lane.Name = "Partition" + i;
                lane.Elements.Add(element);
                transition.End2 = element;

                if (!urlParam.Equals(""))
                    transition.SetTaggedValue(new UmlTag(null, "TDparameter") , urlParam);

                transition.SetTaggedValue(new UmlTag(null, "TDthinkTime"), "5");

                UmlTransition t = new UmlTransition();
                t.End1 = element;
                transition = t;
                
                diagram.UmlObjects.Add(element);
                diagram.UmlObjects.Add(t);
             
            }

            UmlFinalState final = new UmlFinalState() { Name = "Final0" };
            transition.End2 = final;
            diagram.UmlObjects.Add(final);
           
            diagram.UmlObjects.Add(lane);
            return model;
        }

        /// <summary>
        /// Stop fiddler before closing.
        /// </summary>
        private void Window_Closed(object sender, EventArgs e) {
            if(this.isProxyRunning)
                Fiddler.FiddlerApplication.Shutdown();
             this.Close();
        }

        /// <summary>
        /// Starts Fiddler proxy and capturing
        /// </summary>
        private void buttonStart_Click(object sender, RoutedEventArgs e) {

            this.isProxyRunning = true;

            FiddlerApplication.Startup(0, Fiddler.FiddlerCoreStartupFlags.Default);            
        }

        /// <summary>
        /// Exists application
        /// </summary>
        private void MenuItem_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

    }
}
