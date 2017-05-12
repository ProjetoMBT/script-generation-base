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
using PlugSpl.DataStructs.UmlComponentDiagram;
using Microsoft.Win32;
using System.IO;
using System.Reflection;
using System.Xml;


namespace ClassicProductConfigurator
{
    /// <summary>
    /// Interaction logic for ProductConfigurator.xaml
    /// </summary>
    public partial class ProductConfigurator : UserControl, IProductConfigurator
    {

        /// <summary>
        /// Stores PlugSPL main window reference.
        /// </summary>
        private MainWindow MainWindow;

        /// <summary>
        /// Stores internal Bragi data.
        /// </summary>
        public ComponentDiagramBragi BragiStruct { get; private set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ProductConfigurator()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Update the TreeView control with given Bragi data. 
        /// Data is not validated by this control and an external control must to be 
        /// implemented in order to do it.
        /// </summary>
        public void UpdateVisualTree()
        {

            // Gets core component. 
            // NOTE: "x => x.Socket.Count() == 0" is a lambda expression that means 
            // "every component with no sockets."
            Component core = this.BragiStruct.Components.Where(x => x.Sockets.Count() == 0).FirstOrDefault();

            if (core == null)
                return;

            this.treeViewComponentsContainer.Items.Clear();

            //Adds component to tree recursivelly.
            this.addComponentToTree(core, this.treeViewComponentsContainer.Items);

        }

        /// <summary>
        /// Adds a component to the TreeView. This method is called recursivelly.
        /// </summary>
        private void addComponentToTree(Component component, ItemCollection collection)
        {

            //Adds component to TreeView.
            TreeViewItem item = new TreeViewItem();
            CheckBox chk = new CheckBox()
            {
                Content = component.Name,
                Margin = new Thickness(2),
            };
            chk.Click += new RoutedEventHandler(this.component_OnClick);
            item.Header = chk;
            item.IsExpanded = true;

            collection.Add(item);

            //Locate available interface and components attached to it.
            List<Component> children = new List<Component>();

            foreach (InterfaceObject i in component.Interfaces)
            {

                TreeViewItem itemInterface = new TreeViewItem();
                itemInterface.Header = new TextBlock()
                {
                    Text = i.Name,
                    Foreground = new SolidColorBrush(Colors.Red)
                };
                itemInterface.IsExpanded = true;
                item.Items.Add(itemInterface);

                // NOTE: "x => x.AttachedInterface.Equals(i)" is a lambda expression and means
                // "Every Socket which has i as AttachedInterface"
                Socket[] usesGivenInterface = this.BragiStruct.Sockets.Where(x => x.AttachedInterface.Equals(i)).ToArray();

                foreach (Socket s in usesGivenInterface)
                    addComponentToTree(s.Parent, itemInterface.Items);
            }
        }

        private void component_OnClick(object o, RoutedEventArgs e)
        {
            if (o is CheckBox)
                checkIfParent(this.treeViewComponentsContainer.Items, (CheckBox)o);
        }

        /// <summary>
        /// Check if checkBox is in a collection
        /// </summary>
        /// <param name="collection">collection to search for checkBox</param>
        /// <param name="checkBox">the check box to be searched</param>
        /// <returns>true if checkBox is a descendent in collection, otherwise false</returns>
        private bool checkIfParent(ItemCollection collection, CheckBox checkBox)
        {
            foreach (ItemsControl item in collection)
                if (item is TreeViewItem)
                {
                    if (((TreeViewItem)item).Header is CheckBox)
                        if (((CheckBox)((TreeViewItem)item).Header).Content.Equals(checkBox.Content))
                            return true;

                    if (checkIfParent(item.Items, checkBox))
                    {
                        if (((TreeViewItem)item).Header is CheckBox)
                            ((CheckBox)((TreeViewItem)item).Header).IsChecked = this.GetSelectedChildren((TreeViewItem)item, true).Count > 0;

                        return true;
                    }
                }

            return false;
        }

        #region Interface implementation
        public Control GetControl()
        {
            return this;
        }

        public void ExportStructure()
        {
            throw new NotImplementedException();
        }

        public void ImportStructure()
        {
            throw new NotImplementedException();
        }

        public void SetApplicationWindow(PlugSpl.MainWindow mainWindow)
        {
            this.MainWindow = mainWindow;
        }

        #endregion
        #region Events and Delegates

        /// <summary>
        /// Open file button logic.
        /// </summary>
        private void buttonOpenFile_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                OpenFileDialog dialog = new OpenFileDialog();

                String[] availableFiles = Directory.GetFiles("./", "*.dll");
                List<String> featureModelModules = new List<String>();
                List<Type> extensions = new List<Type>();

                //Searches amongst available libraries for valid Feature Model extensions
                foreach (String s in availableFiles)
                {
                    Assembly asm = Assembly.LoadFrom(s);
                    Type[] types = asm.GetTypes();

                    foreach (Type t in types)
                    {
                        Type[] interfaces = t.GetInterfaces();
                        if (interfaces.Contains(typeof(IConfigurationFormat)))
                        {
                            IConfigurationFormat extension = (IConfigurationFormat)Activator.CreateInstance(t);
                            string filter = extension.GetFilter();
                            if (!dialog.Filter.Equals(""))
                                filter = "|" + filter;
                            dialog.Filter += filter;
                            extensions.Add(t);
                        }
                    }
                }

                if (dialog.ShowDialog() == false)
                {
                    return;
                }

                string fileToLoad = dialog.FileName;
                string ext = fileToLoad.Split('.').Last();

                //Searches amongst valid extensions for the one selected
                foreach (Type t in extensions)
                {
                    IConfigurationFormat extension = (IConfigurationFormat)Activator.CreateInstance(t);
                    if (extension.GetFilter().Contains(ext))
                    {
                        ComponentDiagramBragi bragi = extension.LoadFrom(fileToLoad);

                        this.BragiStruct = bragi;
                        this.UpdateVisualTree();
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "PlugSPL Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        private void buttonSaveFile_Click(object sender, RoutedEventArgs e)
        {
            XmlTextWriter writer = null;
            try
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.AddExtension = true;
                dialog.DefaultExt = ".plugpc";
                dialog.Filter = "PlugSPL Product Configuration (*.plugpc)|*.plugpc";

                if (dialog.ShowDialog() != true)
                    return;

                writer = new XmlTextWriter(dialog.FileName, Encoding.UTF8);

                List<TreeViewItem> selectedComponents = this.GetSelectedChildren(
                    treeViewComponentsContainer.Items.OfType<TreeViewItem>().FirstOrDefault());

                writer.WriteStartElement("Root");

                foreach (TreeViewItem t in selectedComponents)
                {
                    writer.WriteElementString("Component", ((t.Header as CheckBox).Content as string));
                }

                writer.WriteEndElement();

                writer.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "PlugSPL Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }


        private List<TreeViewItem> GetSelectedChildren(TreeViewItem tree)
        {
            return GetSelectedChildren(tree, false);
        }

        private List<TreeViewItem> GetSelectedChildren(TreeViewItem tree, bool ignoreHeader)
        {
            List<TreeViewItem> items = new List<TreeViewItem>();

            if (tree == null)
                return items;

            if (tree.Header is CheckBox && ((CheckBox)tree.Header).IsChecked == true && !ignoreHeader)
                items.Add(tree);

            //locate selected children
            foreach (TreeViewItem t in tree.Items.OfType<TreeViewItem>())
                items.AddRange(this.GetSelectedChildren(t));

            return items;
        }

    }
}
