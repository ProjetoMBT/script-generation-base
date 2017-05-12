using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Xml;
using Coc.Data.LoadRunner.SequenceModel;

namespace Coc.Data.LoadRunner.ParameterEditor
{
    /// <summary>
    /// Interaction logic for ParameterEditorWindow.xaml
    /// </summary>
    public partial class ParameterEditorWindow : Window
    {
        private List<Scenario> scenarios = null;
        private DataTable td = new DataTable();
        private ItemCollection parameterScenarios;
        //public List<SaveParameter> saveParameters = new List<SaveParameter>();
        public SaveParameter saveParameters;
        private XmlDocument actualInformations = new XmlDocument();
        private ImageTreeViewItemCor actualRootItem;
        private ImageTreeViewItemCor root;
        List<ImageTreeViewItem> param = new List<ImageTreeViewItem>();
        private ImageTreeViewItem actualSelectedItem;
        //private MainWindow parentWindow;
        public static List<string> enableRulesList;
        public static List<string> disableRulesList;
        private bool treeHeadFlag = true;

        //public MainWindow ParentWindow
        //{
        //    get { return parentWindow; }
        //    set { parentWindow = value; }
        //}

        public ItemCollection ParameterScenarios
        {
            get { return parameterScenarios; }
            set { parameterScenarios = value; }
        }

        public ParameterEditorWindow()
        {
            InitializeComponent();
            this.treeViewScenarios.SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>(treeViewScenarios_SelectedItemChanged);
            this.treeViewSParameters.SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>
            (treeViewSParameters_SelectedItemChanged);
            actualInformations.AppendChild(actualInformations.CreateProcessingInstruction("xml", "version=\"1.0\"encoding=\"UTF-8\""));
            XmlElement root = actualInformations.CreateElement("CorrelationSettings");
            actualInformations.AppendChild(root);
        }

        /// <summary>
        /// Update form with information got from ImageItemTreeView selected.
        /// </summary>
        void treeViewScenarios_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if ((cbNextRow.SelectedItem != null) && (this.actualSelectedItem != null))
            {
                this.actualSelectedItem.selectedItem = (string)cbNextRow.SelectedItem;
            }
            ImageTreeViewItem item = this.treeViewScenarios.SelectedItem as ImageTreeViewItem;
            this.actualSelectedItem = item;
            if (item.Data != null)
            {
                cbNextRow.Items.Clear();
                cbNextRow.Items.Add("Unique");
                cbNextRow.Items.Add("Random");
                cbNextRow.Items.Add("Sequential");
                foreach (string s in item.nextRows)
                {
                    this.cbNextRow.Items.Add("Same line as " + s);
                }
                cbNextRow.SelectedItem = item.selectedItem;
                this.LoadInformationFrom(item.Data);
            }
        }

        void treeViewSParameters_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            this.previewInformationGrid.Children.Clear();
            ImageTreeViewItemCor item = this.treeViewSParameters.SelectedItem as ImageTreeViewItemCor;
            this.LoadInformationFromSaveParam(item);
        }

        private void LoadInformationFromSaveParam(ImageTreeViewItemCor item)
        {
            if (item.rule == null)
            {
                this.actualRootItem = item;
                updatePreviewInformation();
            }
            else
            {
                TextBlock tb = new TextBlock();
                tb.Text = "Action: ";
                this.previewInformationGrid.Children.Add(tb);
                tb.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                tb.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                tb.Margin = new Thickness(60, 12, 0, 0);

                tb = new TextBlock();
                tb.Text = "Order: ";
                this.previewInformationGrid.Children.Add(tb);
                tb.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                tb.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                tb.Margin = new Thickness(60, 42, 0, 0);

                tb = new TextBlock();
                tb.Text = "Left boundary: ";
                this.previewInformationGrid.Children.Add(tb);
                tb.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                tb.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                tb.Margin = new Thickness(60, 72, 0, 0);

                tb = new TextBlock();
                tb.Text = "Right boundary: ";
                this.previewInformationGrid.Children.Add(tb);
                tb.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                tb.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                tb.Margin = new Thickness(60, 102, 0, 0);

                tb = new TextBlock();
                tb.Text = "Parameter Prefix: ";
                this.previewInformationGrid.Children.Add(tb);
                tb.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                tb.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                tb.Margin = new Thickness(60, 132, 0, 0);

                this.previewInformationGrid.Children.Add(item.action);
                this.previewInformationGrid.Children.Add(item.order);
                this.previewInformationGrid.Children.Add(item.leftBound);
                this.previewInformationGrid.Children.Add(item.rightBound);
                this.previewInformationGrid.Children.Add(item.paramPrefix);
            }
        }

        /// <summary>
        /// Loads parameterization data from a *.dat file. 
        /// </summary>
        private void buttonLoadDataFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.ShowDialog();

            if (!File.Exists(dialog.FileName))
            {
                textBoxDataFileName.Text = "<<unable to load selected file>>";
                return;
            }

            textBoxDataFileName.Text = dialog.FileName;
            loadDataFromDat(dialog.FileName);
        }

        /// <summary>
        /// Fills datagrid with values from DAT
        /// </summary>
        /// <param name="filename"></param>
        private void loadDataFromDat(String filename)
        {
            td = new DataTable();
            String[] lines = File.ReadAllLines(filename);

            comboBoxName.Items.Clear();
            td.Columns.Clear();
            foreach (String c in lines[0].Split(','))
            {
                td.Columns.Add(c);
                comboBoxName.Items.Add(c);
            }

            comboBoxIndex.Items.Clear();
            for (int j = 0; j < lines[0].Split(',').Count(); j++)
            {
                comboBoxIndex.Items.Add(j);
            }

            comboBoxName.IsEnabled = true;
            comboBoxName.Visibility = System.Windows.Visibility.Visible;
            comboBoxIndex.Visibility = System.Windows.Visibility.Collapsed;

            for (int i = 1; i < lines.Count() && i < 50; i++)
            {
                String[] line = lines[i].Split(',');

                MethodInfo[] methods = td.Rows.GetType().GetMethods();

                foreach (MethodInfo m in methods)
                {
                    ParameterInfo[] inf = m.GetParameters();
                    if (m.Name == "Add" && m.GetParameters().First().Name == "values")
                    {
                        int lastParamPosition = inf.Length - 1;

                        object[] realParams = new object[inf.Length];

                        for (int ii = 0; ii < lastParamPosition; ii++)
                            realParams[ii] = line[ii];

                        Type paramsType = inf[lastParamPosition].ParameterType.GetElementType();
                        Array extra = Array.CreateInstance(paramsType, line.Length - lastParamPosition);

                        for (int ij = 0; ij < extra.Length; ij++)
                        {
                            extra.SetValue(line[ij + lastParamPosition], ij);
                        }
                        realParams[lastParamPosition] = extra;
                        m.Invoke(td.Rows, realParams);
                    }
                }
            }

            this.dataGridFileValues.Columns.Clear();
            this.dataGridFileValues.AutoGenerateColumns = true;
            this.dataGridFileValues.DataContext = td;
            this.ParameterizationData.SourcePath = filename;
            string[] splited = filename.Split('\\');
            string tableName = splited.Last();
            this.ParameterizationData.Table = @"..\Data\" + tableName;
        }

        /// <summary>
        /// Display "Name" comboBox button when checked.
        /// </summary>
        private void RadioButton_Checked_3(object sender, RoutedEventArgs e)
        {
            try
            {
                this.comboBoxIndex.Visibility = System.Windows.Visibility.Collapsed;
                this.comboBoxName.Visibility = System.Windows.Visibility.Visible;
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// Displays "Index" comboBox button when checked.
        /// </summary>
        private void RadioButton_Checked_4(object sender, RoutedEventArgs e)
        {
            try
            {
                this.comboBoxIndex.Visibility = System.Windows.Visibility.Visible;
                this.comboBoxName.Visibility = System.Windows.Visibility.Collapsed;
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// Sets current List"Scenario" object. Used for script parameterization.
        /// </summary>
        /// <param name="scenarios">Object to be used.</param>
        public void SetTestData(List<Scenario> scenarios)
        {
            this.scenarios = scenarios;
        }

        /// <summary>
        /// Fill this.treeView with scenario list data.
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (Scenario scenario in this.scenarios)
            {
                loadParameterizeInformation(scenario);
                loadSaveParameterInformation(scenario);
                this.updatePreviewInformation();
            }
        }

        private void loadParameterizeInformation(Scenario scenario)
        {
            ImageTreeViewItem item = new ImageTreeViewItem();
            item.ParentWindow = this;
            item.IsExpanded = true;
            item.Image.Source = new BitmapImage(new Uri(@"pack://application:,,,/PerformanceTool;component/Images/ParameterEditor/Gnome-Folder-Visiting-32.png"));
            item.Text = scenario.Name;
            this.treeViewScenarios.Items.Add(item);

            List<String> lis = new List<String>();
            foreach (TestCase testCase in scenario.TestCases)
            {
                ImageTreeViewItem item2 = new ImageTreeViewItem();
                item2.ParentWindow = this;
                item2.IsExpanded = true;
                item2.Image.Source = new BitmapImage(new Uri(@"pack://application:,,,/PerformanceTool;component/Images/ParameterEditor/Gnome-Accessories-Text-Editor-32.png"));
                item2.Text = testCase.Name;
                item.Items.Add(item2);
                List<String> parameters = new List<String>();

                foreach (Request request in testCase.Requests)
                {
                    parseRequestToParameter(request, parameters);
                }

                parameters.Sort();
                IEnumerable<String> list = parameters.Distinct<String>();
                List<String> removeNames = new List<string>();
                for (int i = 0; i < list.Count(); i++)
                {
                    String s = list.ElementAt(i);

                    ImageTreeViewItem item4 = new ImageTreeViewItem();
                    item4.ParentWindow = this;
                    item4.IsExpanded = true;
                    item4.Image.Source = new BitmapImage(new Uri(@"pack://application:,,,/PerformanceTool;component/Images/ParameterEditor/Gnome-Format-Text-Italic-32.png"));
                    item4.Text = s;
                    item4.Data = new ScriptParameterizationData();

                    if (s.Contains('_'))
                    {
                        item4.Data.ColumnName = s.Split('_')[1];
                        item4.Data.Table = "..\\Data\\" + s.Split('_')[0] + ".dat";
                        item4.Data.Table = item4.Data.Table.Replace(" ", "");
                        item4.Data.GenerateNewValue = "EachIteration";
                    }
                    item4.Data.SelectNextRow = "Random";
                    item2.Items.Add(item4);

                    if (item4.Text.Length > 8)
                    {
                        if (item4.Text[8] == '-' && item4.Text[13] == '-' && item4.Text[18] == '-' && item4.Text[23] == '-')
                        {
                            removeNames.Add(item4.Text);
                        }
                    }

                    foreach (string names in list)
                    {
                        if (item4.Text != names)
                        {
                            item4.nextRows.Add(names);
                        }
                    }
                }
            }
        }

        private void parseRequestToParameter(Request request, List<string> parameters)
        {
            int begin = 0, cursor = 0;

            while (cursor < request.Action.Length)
            {
                if (request.Action[cursor] == '{')
                {
                    begin = cursor;
                }
                else if (request.Action[cursor] == '}' && request.Action[cursor - 1] != '}')
                {
                    if (request.Action.Substring(begin + 1, cursor - begin - 1).ToLower().Contains('.'))
                    {
                        parameters.Add(request.Action.Substring(begin + 1, cursor - begin - 1).ToLower().Replace('.', '_').Trim());
                        request.Action = request.Action.Replace('.', '_').Trim();
                    }
                }
                cursor++;
            }

            begin = 0;
            cursor = 0;

            if (request.Body.StartsWith("{"))
            {
                request.Body = request.Body.Substring(1, request.Body.Length - 2);
            }

            while (cursor < request.Body.Length)
            {
                if (request.Body[cursor] == '{')
                {
                    begin = cursor;
                }
                else if (request.Body[cursor] == '}' && request.Body[cursor - 1] != '}')
                {
                    if (request.Body.Substring(begin + 1, cursor - begin - 1).ToLower().Contains('.'))
                    {

                        parameters.Add(request.Body.Substring(begin + 1, cursor - begin - 1).ToLower().Replace('.', '_').Trim());
                        request.Body = request.Body.Replace('.', '_').Trim();
                    }
                }
                cursor++;
            }

            request.Body = "{" + request.Body + "}";

            foreach (Parameter par in request.Parameters)
            {
                begin = 0;
                cursor = 0;

                while (cursor < par.Value.Length)
                {
                    if (par.Value[cursor] == '{')
                    {
                        begin = cursor;
                    }
                    else if (par.Value[cursor] == '}' && par.Value[cursor - 1] != '}')
                    {
                        if (par.Value.Substring(begin + 1, cursor - begin - 1).ToLower().Contains('.'))
                        {
                            parameters.Add(par.Value.Substring(begin + 1, cursor - begin - 1).ToLower().Replace('.', '_').Trim());
                            par.Value = par.Value.Replace('.', '_').Trim();
                        }
                    }
                    cursor++;
                }
            }

            begin = 0;
            cursor = 0;

            while (cursor < request.Referer.Length)
            {
                if (request.Referer[cursor] == '{')
                {
                    begin = cursor;
                }
                else if (request.Referer[cursor] == '}' && request.Referer[cursor - 1] != '}')
                {
                    if (request.Referer.Substring(begin + 1, cursor - begin - 1).ToLower().Contains('.'))
                    {
                        parameters.Add(request.Referer.Substring(begin + 1, cursor - begin - 1).ToLower().Replace('.', '_').Trim());
                        request.Referer = request.Referer.Replace('.', '_').Trim();
                    }
                }
                cursor++;
            }
        }

        private void loadSaveParameterInformation(Scenario scenario)
        {
            ImageTreeViewItemCor itemCor = new ImageTreeViewItemCor();
            root = itemCor;
            actualRootItem = root;
            actualRootItem.Information = "User";
            itemCor.ParentWindow = this;
            itemCor.checkbox.Type = "header";
            itemCor.IsExpanded = true;
            itemCor.Image.Source = new BitmapImage(new Uri(@"pack://application:,,,/PerformanceTool;component/Images/ParameterEditor/Gnome-Folder-Visiting-32.png"));
            itemCor.Text = scenario.Name;
            itemCor.checkbox.Checked += new RoutedEventHandler(Checkbox_Changed);
            itemCor.checkbox.Unchecked += new RoutedEventHandler(Checkbox_Changed);
            this.treeViewSParameters.Items.Add(itemCor);
            SaveParameter spCor = new SaveParameter(itemCor.Name);
            itemCor.saveParameter = spCor;
            this.saveParameters = spCor;
        }

        /// <summary>
        /// Fill form with information got from ScriptParameterization data
        /// </summary>
        /// <param name="data">ScriptParameterization to get information from</param>
        public void LoadInformationFrom(ScriptParameterizationData data)
        {
            this.ParameterizationData = data;
            #region DAT data
            this.textBoxDataFileName.Text = data.SourcePath;
            if (!data.SourcePath.Equals(String.Empty))
            {
                this.loadDataFromDat(data.SourcePath);
            }
            else
            {
                this.td = new DataTable();
                this.td.Columns.Add("#");
                this.td.Rows.Add("");
                this.dataGridFileValues.DataContext = td;
            }
            #endregion
            #region ColumnName
            try
            {
                int x;
                if (!Int32.TryParse(data.ColumnName, out x))
                {
                    IEnumerable<String> o = from String s in this.comboBoxName.Items.OfType<String>()
                                            where s.Equals(data.ColumnName)
                                            select s;
                    this.comboBoxName.SelectedItem = o.First();
                    this.comboBoxIndex.Visibility = System.Windows.Visibility.Collapsed;
                    this.comboBoxName.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    this.comboBoxIndex.SelectedIndex = x;
                    this.comboBoxIndex.Visibility = System.Windows.Visibility.Visible;
                    this.comboBoxName.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
            catch (Exception)
            {
                this.comboBoxIndex.Visibility = System.Windows.Visibility.Collapsed;
                this.comboBoxName.Visibility = System.Windows.Visibility.Visible;
                this.comboBoxName.SelectedIndex = -1;
            }

            #endregion
            #region Update Value on
            switch (data.GenerateNewValue)
            {
                case "Once":
                    this.radioButtonOnce.IsChecked = true;
                    break;
                case "Ocurrence":
                    this.radioButtonOcurrence.IsChecked = true;
                    break;
                default: //EachInteraction
                    this.radioButtonIteration.IsChecked = true;
                    break;
            }
            #endregion
            #region When out of range
            switch (data.OutOfRangePolicy)
            {
                case "About":
                    this.radioButtonAbout.IsChecked = true;
                    break;
                case "Cyclic":
                    this.radioButtonCyclic.IsChecked = true;
                    break;
                default: //continue last
                    this.radioButtonContinueLast.IsChecked = true;
                    break;
            }
            #endregion
        }

        public ScriptParameterizationData ParameterizationData = new ScriptParameterizationData();

        /// <summary>
        /// Generate PRM file use with given script (name).
        /// </summary>
        /// <param name="writer">textWriter to write to.</param>
        /// <param name="data">data to be written</param>
        /// <param name="itemName">script name</param>
        public void writeAsText(TextWriter writer, ScriptParameterizationData data, String itemName)
        {
            writer.WriteLine("[parameter:{0}]", itemName);
            writer.WriteLine(@"Delimiter=""{0}""", data.Delimiter);
            writer.WriteLine(@"ParamName=""{0}""", itemName);
            writer.WriteLine(@"TableLocation=""Local""");
            writer.WriteLine(@"ColumnName=""{0}""", data.ColumnName);
            writer.WriteLine(@"Table=""{0}""", data.Table);
            writer.WriteLine(@"GenerateNewVal=""{0}""", data.GenerateNewValue);
            writer.WriteLine(@"Type=""{0}""", data.Type);
            writer.WriteLine(@"value_for_each_vuser=""{0}""", data.ValueForEachVirtualUser);
            writer.WriteLine(@"OriginalValue=""{0}""", data.OriginalValue);
            writer.WriteLine(@"auto_allocate_block_size=""{0}""", data.AutoAllocateBlockSize);
            writer.WriteLine(@"SelectNextRow=""{0}""", data.SelectNextRow);
            writer.WriteLine(@"StartRow=""{0}""", data.StartRow);
            writer.WriteLine(@"OutOfRangePolicy=""{0}""", data.OutOfRangePolicy);
        }

        /*
         * ============================================
         * BEGIN SAFE AREA - CODE HAS TO BE REFACTORED
         * ============================================
         */

        #region RadioGroup SelectNextRow {Unique, Random, Sequential}
        private void radioButtonUnique_Checked(object sender, RoutedEventArgs e)
        {
            this.ParameterizationData.SelectNextRow = "Unique";
        }

        private void radioButtonRandom_Checked(object sender, RoutedEventArgs e)
        {
            this.ParameterizationData.SelectNextRow = "Random";
        }

        private void radioButtonSequential_Checked(object sender, RoutedEventArgs e)
        {
            this.ParameterizationData.SelectNextRow = "Sequential";
        }
        #endregion
        #region RadioGroup UpdateValue {Once, Iteration, Ocurrence}

        private void radioButtonOnce_Click(object sender, RoutedEventArgs e)
        {
            this.ParameterizationData.GenerateNewValue = "Once";
        }

        private void radioButtonIteration_Click(object sender, RoutedEventArgs e)
        {
            this.ParameterizationData.GenerateNewValue = "EachIteration";
        }

        private void radioButtonOcurrence_Click(object sender, RoutedEventArgs e)
        {
            this.ParameterizationData.GenerateNewValue = "Occurrence";
        }
        #endregion
        #region RadioGroup WhenOutOfValues {About, Cyclic, ContinueLast}

        private void radioButtonAbout_Checked(object sender, RoutedEventArgs e)
        {
            this.ParameterizationData.OutOfRangePolicy = "About";
        }

        private void radioButtonCyclic_Checked(object sender, RoutedEventArgs e)
        {
            this.ParameterizationData.OutOfRangePolicy = "Cyclic";
        }

        private void radioButtonContinueLast_Checked(object sender, RoutedEventArgs e)
        {
            this.ParameterizationData.OutOfRangePolicy = "ContinueWithLast";
        }
        #endregion

        #region Toolbar Buttons Triggers
        /// <summary>
        /// Write all available script parameters to selected folder as PRM files.
        /// </summary>
        private void buttonExportAll_Click(object sender, RoutedEventArgs e)
        {
            parameterScenarios = this.treeViewScenarios.Items;
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }

            //this.parentWindow.loadRunnerScenario = dialog.SelectedPath;

            foreach (Scenario s in this.scenarios)
            {
                Directory.CreateDirectory(dialog.SelectedPath + "\\" + s.Name + "\\Data");
            }

            System.Windows.MessageBox.Show("Script parameters have been set successfully.", "Script parameters", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }
        /// <summary>
        /// Export selected parameter to a PRM file.
        /// </summary>
        private void buttonExport_Click(object sender, RoutedEventArgs e)
        {
            ImageTreeViewItem item = null;

            try
            {
                item = this.treeViewScenarios.SelectedItem as ImageTreeViewItem;

                if (item.Data == null)
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show("Select a parameter to export");
                return;
            }

            SaveFileDialog dialog = new SaveFileDialog();

            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }
            ScriptParameterizationData data = item.Data;

            using (TextWriter writer = new StreamWriter(dialog.FileName))
            {
                this.writeAsText(writer, data, item.Name);
            }
        }

        #endregion
        /// <summary>
        /// Is a import correlation event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="item"></param>
        public void btnImportCor_Click(object sender, RoutedEventArgs e, ImageTreeViewItemCor item)
        {
            //Open import file dialog
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Correlation Files (*.cor)|*.cor";
            dialog.ShowDialog();
            const string xmiNamespaceUri = "http://schema.omg.org/spec/XMI/1.3";
            const string umlNamespaceUri = "org.omg.xmi.namespace.UML";
            XmlNamespaceManager ns = new XmlNamespaceManager(new NameTable());
            ns.AddNamespace("xmi", xmiNamespaceUri);
            ns.AddNamespace("UML", umlNamespaceUri);
            //Create a new xml document
            XmlDocument doc = new XmlDocument();
            //if file name is diferent that empty
            if (dialog.FileName != "")
            {
                try
                {
                    //Load file .cor
                    doc.Load(dialog.FileName);
                    //Parse parameters and load informations in memory
                    if (parseCorrelationParameters(doc, ns))
                    {
                        loadCorrelationInformation(doc, ns);
                    }

                    foreach (XmlNode g in doc.SelectNodes("//Group", ns))
                    {
                        if (actualInformations.InnerXml.Equals("<?xml version=\"1.0\"encoding=\"UTF-8\"?><CorrelationSettings />"))
                        {
                            actualInformations.DocumentElement.AppendChild(actualInformations.ImportNode(g, true));
                        }
                        else
                        {
                            foreach (XmlNode g2 in actualInformations.SelectNodes("//Group", ns))
                            {
                                //if exists
                                if (g.Attributes["Name"].InnerText == g2.Attributes["Name"].InnerText)
                                {
                                    foreach (XmlNode rule in doc.SelectNodes("//Rule", ns))
                                    {
                                        //if rule dont exist
                                        if (!ExistRule(rule, g, ns))
                                        {
                                            XmlNode imported = g2.OwnerDocument.ImportNode(rule, true);
                                            g2.AppendChild(imported);
                                        }
                                    }
                                }
                                else
                                {
                                    actualInformations.DocumentElement.AppendChild(actualInformations.ImportNode(g, true));
                                }
                            }
                        }
                    }

                    actualRootItem = item;
                    checkParametersAndSaveParameters();
                    string name = "";

                    foreach (ImageTreeViewItem img in this.param)
                    {
                        if (img == this.param.Last())
                        {
                            name += img.Text + ".";
                        }
                        else
                        {
                            name += img.Text + ", ";
                        }
                    }

                    if (this.param.Count > 0)
                    {
                        MessageBoxResult mbr = System.Windows.MessageBox.Show("The following parameters have been found in the save parameters list: " + name + "Do you want to delete these parameters of the parameters list?", "", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                        if (mbr == MessageBoxResult.Yes)
                        {
                            this.deleteParameters();
                        }
                    }
                }
                catch (Exception ex)
                {

                    System.Windows.MessageBox.Show(ex.Message);
                }
            }
        }
        /// <summary>
        /// Compare if exists a equal rule in grupe g and g2, used by btnImportCor_Click
        /// </summary>
        /// <param name="r"></param>
        /// <param name="r2"></param>
        /// <param name="ns"></param>
        /// <returns></returns>
        private bool ExistRule(XmlNode rule, XmlNode g, XmlNamespaceManager ns)
        {
            //XmlNode nodeRef = null;
            foreach (XmlNode r in g.SelectNodes("//Rule", ns))
            {
                if (rule.Attributes["Name"].InnerText == r.Attributes["Name"].InnerText)
                {
                    return false;
                }
            }
            return true;
        }

        private void deleteParameters()
        {
            try
            {
                for (int i = 0; i < this.treeViewScenarios.Items.Count; i++)
                {
                    ImageTreeViewItem scenarios = (ImageTreeViewItem)this.treeViewScenarios.Items[i];
                    for (int j = 0; j < scenarios.Items.Count; j++)
                    {
                        ImageTreeViewItem scripts = (ImageTreeViewItem)scenarios.Items[j];
                        for (int k = 0; k < scripts.Items.Count; k++)
                        {
                            if (param.Contains(scripts.Items[k]))
                            {
                                foreach (ImageTreeViewItem parame in scripts.Items)
                                {
                                    parame.nextRows.Remove(((ImageTreeViewItem)scripts.Items[k]).Text);
                                }
                                scripts.Items.RemoveAt(k);
                                k--;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Parse the correlation file and create rules
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="ns"></param>
        /// <returns></returns>
        private bool parseCorrelationParameters(XmlDocument doc, XmlNamespaceManager ns)
        {
            foreach (XmlNode node in doc.SelectNodes("CorrelationSettings", ns))
            {
                foreach (XmlNode groupNode in node.SelectNodes("Group", ns))
                {
                    foreach (XmlNode rul in groupNode.SelectNodes("Rule", ns))
                    {
                        //Instatiate a new rule
                        Coc.Data.LoadRunner.SequenceModel.Rule r = new Coc.Data.LoadRunner.SequenceModel.Rule(rul.Attributes["Name"].InnerText.ToLower().Trim());

                        if (!ContainsRule(r))
                        {
                            //Root ImageTreeViewItemCor
                            for (int i = 0; i < root.saveParameter.Rules.Count(); i++)
                            {
                                if (r.Name.ToLower().Trim() == root.saveParameter.Rules[i].Name.ToLower().Trim())
                                {
                                    //References r
                                    root.saveParameter.Rules[i] = r;
                                    //Search for equal name, if find, remove item
                                    for (int j = 0; j < root.Items.Count; j++)
                                    {
                                        if (((ImageTreeViewItemCor)root.Items[j]).Text.ToLower().Trim() == r.Name.ToLower().Trim())
                                        {
                                            root.Items.RemoveAt(j);
                                            j--;
                                        }
                                    }
                                }
                            }
                            //Set rule atributes
                            r.Enabled = true;
                            r.LeftBoundary = rul.Attributes["LeftBoundText"].InnerText;
                            r.RightBoundary = rul.Attributes["RightBoundText"].InnerText;
                            r.Prefix = rul.Attributes["ParamPrefix"].InnerText;
                            r.Parent = this.saveParameters;
                            r.Action = rul.Attributes["Type"].InnerText;
                            this.saveParameters.Rules.Add(r);
                        }
                    }
                }
            }
            return true;
        }

        private bool ContainsRule(Coc.Data.LoadRunner.SequenceModel.Rule r)
        {
            foreach (Coc.Data.LoadRunner.SequenceModel.Rule item in saveParameters.Rules)
            {
                if (r.Name == item.Name)
                {
                    return true;
                }
            }
            return false;
        }

        private void checkParametersAndSaveParameters()
        {
            foreach (ImageTreeViewItem item in this.treeViewScenarios.Items)
            {
                foreach (ImageTreeViewItem item2 in item.Items)
                {
                    foreach (ImageTreeViewItem item3 in item2.Items)
                    {
                        if (searchExistsParam(item3.Text))
                        {
                            param.Add(item3);
                        }
                    }
                }
            }
        }

        private bool searchExistsParam(string p)
        {
            foreach (Coc.Data.LoadRunner.SequenceModel.Rule r in this.saveParameters.Rules)
            {
                if (p.ToUpper() == r.Name.ToUpper())
                {
                    return true;
                }
            }
            return false;
        }

        private bool existSaveParameter(SaveParameter sp)
        {
            if (this.saveParameters.Name == sp.Name)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Load informations about coorrelations and add (TextBlock format) in grid
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="ns"></param>
        private void loadCorrelationInformation(XmlDocument doc, XmlNamespaceManager ns)
        {
            TextBlock tb = new TextBlock();
            tb.Tag = "info";
            string information = "";

            foreach (XmlNode node in doc.SelectNodes("CorrelationSettings", ns))
            {
                String enableRules = String.Empty, disableRules = String.Empty;
                foreach (XmlNode groupNode in node.SelectNodes("Group", ns))
                {
                    information = "Application : " + this.root.saveParameter.Name + " - Enabled\n";
                    this.root.Information = information;

                    //Add .cor rules
                    this.actualRootItem = root;
                    foreach (Coc.Data.LoadRunner.SequenceModel.Rule r in this.actualRootItem.saveParameter.Rules)
                    {
                        if (r.Enabled)
                        {
                            createSaveParams(root, r);
                            enableRules += "\t\t" + r.Name + "\n";
                        }
                        else
                        {
                            createSaveParams(root, r);
                            disableRules += "\t\t" + r.Name + "\n";
                        }
                    }

                    information += "Enabled Rules: \n";
                    information += enableRules;
                    information += "Disabled Rules: \n";
                    information += disableRules;
                }
            }
            this.previewInformationGrid.Children.Clear();
            tb.Text += information;
            this.previewInformationGrid.Children.Add(tb);
        }

        private void createSaveParams(ImageTreeViewItemCor root, Coc.Data.LoadRunner.SequenceModel.Rule r)
        {
            if (!ExistItem(r.Name))
            {
                ImageTreeViewItemCor item2 = new ImageTreeViewItemCor();
                item2.IsExpanded = true;
                item2.checkbox.Checked += new RoutedEventHandler(Checkbox_Changed);
                item2.checkbox.Unchecked += new RoutedEventHandler(Checkbox_Changed);
                item2.checkbox.RelatedRule = r;
                item2.Image.Source = new BitmapImage(new Uri(@"pack://application:,,,/PerformanceTool;component/Images/ParameterEditor/Gnome-Accessories-Text-Editor-32.png"));
                item2.Text = r.Name;
                root.Items.Add(item2);
                item2.rule = r;
                item2.saveParameter = this.saveParameters;
                item2.paramPrefix.Text = r.Prefix;
                item2.leftBound.Text = r.LeftBoundary;
                item2.rightBound.Text = r.RightBoundary;
            }
        }

        private bool ExistItem(String name)
        {
            foreach (ImageTreeViewItemCor item in root.Items)
            {
                if (item.Text == name)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// This method return true if treeViewParameters.itens contais a rule
        /// </summary>
        /// <param name="r">Is a Rule</param>
        /// <returns>True if exist and false if not exist</returns>
        private bool ExistsRule(Coc.Data.LoadRunner.SequenceModel.Rule r)
        {
            foreach (ImageTreeViewItemCor item in this.treeViewSParameters.Items)
            {
                foreach (ImageTreeViewItemCor item2 in item.Items)
                {
                    if (item2.Text == r.Name)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// This method return true if treeViewParameters.itens contais a rule
        /// </summary>
        /// <param name="r">Rule name</param>
        /// <returns>True if exist and false if not exist</returns>
        private bool ExistsRule(String ruleName)
        {
            foreach (Coc.Data.LoadRunner.SequenceModel.Rule r in this.actualRootItem.saveParameter.Rules)
            {
                if (r.Name == ruleName)
                {
                    return true;
                }
            }
            return false;
        }

        void Checkbox_Changed(object sender, RoutedEventArgs e)
        {
            if (((CustomCheckbox)sender).Type == "header")
            {
                if (((CustomCheckbox)sender).IsChecked == true)
                {
                    foreach (TreeViewItem item in root.Items)
                    {
                        if (item != actualRootItem)
                            item.IsEnabled = true;
                        treeHeadFlag = true;
                    }
                }
                else if (((CustomCheckbox)sender).IsChecked == false)
                {
                    foreach (TreeViewItem item in root.Items)
                    {
                        if (item != actualRootItem)
                            item.IsEnabled = false;
                        treeHeadFlag = false;
                    }
                    TextBlock tb = new TextBlock();
                    tb.Text = "Enabled Rules:\nDisabled Rules:\n";
                    this.previewInformationGrid.Children.Clear();
                    this.previewInformationGrid.Children.Add(tb);
                    return;
                }
            }
            else if (((CustomCheckbox)sender).RelatedSaveParam != null)
            {
                if (((CustomCheckbox)sender).IsChecked == true)
                {
                    ((CustomCheckbox)sender).RelatedSaveParam.Enabled = true;
                    actualRootItem.Information = actualRootItem.Information.Replace("Disabled", "Enabled");
                    TextBlock tb = GetTextBlockInformation();

                }
                else if (((CustomCheckbox)sender).IsChecked == false)
                {
                    ((CustomCheckbox)sender).RelatedSaveParam.Enabled = false;
                    actualRootItem.Information = actualRootItem.Information.Replace("Enabled", "Disabled");

                }
            }
            else
            {
                if (((CustomCheckbox)sender).IsChecked == true)
                {
                    ((CustomCheckbox)sender).RelatedRule.Enabled = true;
                    actualRootItem = searchRuleParent(((CustomCheckbox)sender).RelatedRule);
                }
                else if (((CustomCheckbox)sender).IsChecked == false)
                {
                    ((CustomCheckbox)sender).RelatedRule.Enabled = false;
                    actualRootItem = searchRuleParent(((CustomCheckbox)sender).RelatedRule);
                }
            }
            updatePreviewInformation();
        }

        private ImageTreeViewItemCor searchRuleParent(Coc.Data.LoadRunner.SequenceModel.Rule rule)
        {
            foreach (ImageTreeViewItemCor it in this.treeViewSParameters.Items)
            {
                if (rule.Parent.Name == it.Text)
                {
                    return it;
                }
            }
            return new ImageTreeViewItemCor();
        }

        private void updatePreviewInformation()
        {
            string newInformation = actualRootItem.Information;
            newInformation += "Enabled Rules:\n";

            actualRootItem.saveParameter = saveParameters;

            foreach (Coc.Data.LoadRunner.SequenceModel.Rule r in actualRootItem.saveParameter.Rules)
            {
                if (r.Enabled)
                {
                    newInformation += "\t\t" + r.Name + "\n";
                }
            }
            newInformation += "Disabled Rules:\n";

            foreach (Coc.Data.LoadRunner.SequenceModel.Rule r in actualRootItem.saveParameter.Rules)
            {
                if (!r.Enabled)
                {
                    newInformation += "\t\t" + r.Name + "\n";
                }
            }
            this.previewInformationGrid.Children.Clear();
            TextBlock tb = new TextBlock();
            tb.Text = newInformation;
            this.previewInformationGrid.Children.Add(tb);
        }

        private TextBlock GetTextBlockInformation()
        {
            foreach (var item in previewInformationGrid.Children)
            {
                if (item.GetType() == typeof(TextBlock))
                {
                    if ((item as TextBlock).Tag.Equals("info"))
                    {
                        return item as TextBlock;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// This is a save button event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            //if left tree is not selected
            if (treeHeadFlag == false)
            {
                foreach (ImageTreeViewItemCor item in root.Items)
                {
                    item.IsEnabled = false;
                }
            }
            if (!validateValues())
            {
                return;
            }
            else
            {
                PopuleteRulesValues();
                const string xmiNamespaceUri = "http://schema.omg.org/spec/XMI/1.3";
                const string umlNamespaceUri = "org.omg.xmi.namespace.UML";
                XmlNamespaceManager ns = new XmlNamespaceManager(new NameTable());
                ns.AddNamespace("xmi", xmiNamespaceUri);
                ns.AddNamespace("UML", umlNamespaceUri);

                //create a bew document
                XmlDocument doc = new XmlDocument();
                string path = "";

                //Configure directories
                //if (this.parentWindow.loadRunnerPath != "" && this.parentWindow.loadRunnerPath != null)
                //{
                //path = System.IO.Path.GetDirectoryName(this.parentWindow.loadRunnerPath);
                path = @"C:\Users\11202217\Desktop";
                //}
                //else
                //{
                //    System.Windows.MessageBox.Show("Please set you LoadRunner's executable.", "Select path", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                //    return;
                //}
                path = System.IO.Path.Combine(path, "../");

                path += "/config/CorrelationSettings.xml";

                try
                {
                    //load the archiver .chor selected by user
                    doc.Load(path);
                    //ride treeViewSParameters
                    foreach (ImageTreeViewItemCor it in this.treeViewSParameters.Items)
                    {
                        //for each treeViewSParameters into ImageTreeViewItemCor 
                        foreach (ImageTreeViewItemCor it2 in it.Items)
                        {
                            //read in correlation archive
                            foreach (XmlNode c in actualInformations.SelectNodes("CorrelationSettings", ns))
                            {
                                foreach (XmlNode child in c.SelectNodes("Group", ns))
                                {
                                    if (this.actualRootItem.Text == child.Attributes["Name"].InnerText)
                                    {
                                        foreach (XmlNode rule in child.SelectNodes("Rule", ns))
                                        {
                                            if (it2.rule != null)
                                            {
                                                //Set rule atributes
                                                if (rule.Attributes["Name"].InnerText == it2.rule.Name)
                                                {
                                                    rule.Attributes["LeftBoundText"].InnerText = it2.leftBound.Text;
                                                    rule.Attributes["RightBoundText"].InnerText = it2.rightBound.Text;
                                                    rule.Attributes["ParamPrefix"].InnerText = it2.paramPrefix.Text;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    //If correlation has informations
                    if (actualInformations != null)
                    {
                        foreach (XmlNode c in actualInformations.SelectNodes("CorrelationSettings", ns))
                        {
                            foreach (XmlNode child in c.SelectNodes("Group", ns))
                            {
                                try
                                {
                                    XmlNode cop = doc.ImportNode(child, true);
                                    doc.DocumentElement.AppendChild(cop);
                                }
                                catch (Exception ex)
                                {
                                    string erro = ex.Message;
                                    System.Windows.MessageBox.Show("Impossible to copy correlation file.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                        }
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Please select one correlation file.", "Empty file", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }
                    //Save doc
                    doc.Save(path);
                    System.Windows.MessageBox.Show("Correlations have been configured successfully", "", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                catch (Exception)
                {
                    System.Windows.MessageBox.Show("Please set a valid LoadRunner's executable.", "Select path", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
        }

        /// <summary>
        /// Populate ImageTreeViewItemCor with respectives values, according to the treeViewParameters
        /// </summary>
        private void PopuleteRulesValues()
        {
            //itvc is a left tree in the interface
            foreach (ImageTreeViewItemCor itvc in this.treeViewSParameters.Items)
            {
                //itvc hsave many itens
                foreach (ImageTreeViewItemCor it in itvc.Items)
                {
                    //set itens according to the selected parameters
                    it.rule.Action = it.action.SelectedItem.ToString();
                    it.rule.Order = it.order.Text;
                    it.rule.LeftBoundary = it.leftBound.Text;
                    it.rule.RightBoundary = it.rightBound.Text;
                    it.rule.Prefix = it.paramPrefix.Text;
                }
            }
        }

        /// <summary>
        /// Validate if rules in the tree has valid atributes
        /// </summary>
        /// <returns></returns>
        private bool validateValues()
        {
            ///foreach component tree
            foreach (ImageTreeViewItemCor cor in this.treeViewSParameters.Items)
            {
                //ride your rules
                foreach (ImageTreeViewItemCor rules in cor.Items)
                {
                    if (rules.leftBound.Text == "" | rules.rightBound.Text == "" | rules.paramPrefix.Text == ""
                        | rules.action.Text == "")
                    {
                        System.Windows.MessageBox.Show("You must fill in all fields of all the rules");
                        return false;
                    }
                }
            }
            return true;
        }

        private void btnExportCor_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //If exist a loaded correlation file
                if (this.actualRootItem == null)
                {
                    System.Windows.MessageBox.Show("Please select one application to export Correlation file.", "", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                //Save dialog 
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Filter = "Correlation files (*.cor)|*.cor";
                const string xmiNamespaceUri = "http://schema.omg.org/spec/XMI/1.3";
                const string umlNamespaceUri = "org.omg.xmi.namespace.UML";
                XmlNamespaceManager ns = new XmlNamespaceManager(new NameTable());
                ns.AddNamespace("xmi", xmiNamespaceUri);
                ns.AddNamespace("UML", umlNamespaceUri);

                //Create a new XMl document
                XmlDocument singleCor = new XmlDocument();

                //Show save as dialog
                dialog.ShowDialog();
                XmlNode singleNode = null;

                XmlDocument tempActualInformations = new XmlDocument();
                tempActualInformations = (XmlDocument)actualInformations.Clone();

                if (dialog.FileName != "")
                {
                    foreach (ImageTreeViewItemCor it in this.root.Items)
                    {
                        foreach (XmlNode c in tempActualInformations.SelectNodes("CorrelationSettings", ns))
                        {
                            foreach (XmlNode child in c.SelectNodes("Group", ns))
                            {
                                singleNode = child;
                                foreach (XmlNode rule in child.SelectNodes("Rule", ns))
                                {
                                    if (it.rule != null)
                                    {
                                        String name = rule.Attributes["Name"].InnerText;
                                        if (name.Equals(it.rule.Name, StringComparison.InvariantCultureIgnoreCase))
                                        {
                                            if (it.rule.Enabled)
                                            {
                                                rule.Attributes["LeftBoundText"].InnerText = it.leftBound.Text;
                                                rule.Attributes["RightBoundText"].InnerText = it.rightBound.Text;
                                                rule.Attributes["ParamPrefix"].InnerText = it.paramPrefix.Text;
                                            }
                                            else
                                            {
                                                child.RemoveChild(rule);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    singleCor.AppendChild(singleCor.CreateProcessingInstruction("xml", "version=\"1.0\""));
                    XmlElement xmlElement = singleCor.CreateElement("CorrelationSettings");
                    singleCor.AppendChild(xmlElement);
                    singleCor.DocumentElement.AppendChild(singleCor.ImportNode(singleNode, true));
                    singleCor.Save(dialog.FileName);
                    System.Windows.MessageBox.Show("Correlation file saved in" + dialog.FileName);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error =" + ex.Message);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Visibility = System.Windows.Visibility.Hidden;
            e.Cancel = true;
        }

        private void cbNextRow_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.actualSelectedItem != null)
            {
                this.actualSelectedItem.Data.SelectNextRow = (string)((System.Windows.Controls.ComboBox)sender).SelectedItem;
            }
        }

        private void comboBoxName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.actualSelectedItem != null)
            {
                this.actualSelectedItem.Data.ColumnName = (string)((System.Windows.Controls.ComboBox)sender).SelectedItem;
            }
        }
    }
}