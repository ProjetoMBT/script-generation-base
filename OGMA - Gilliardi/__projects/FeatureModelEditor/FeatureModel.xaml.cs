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
using System.Collections;
using System.Reflection;
using PlugSpl.Atlas;
using PlugSpl;
using System.Xml.Serialization;

namespace FeatureModelEditor
{
    /// <summary>
    /// Interaction logic for FeatureModelEditor.xaml
    /// </summary>
    public partial class FeatureModel : UserControl, PlugSpl.Modules.IFeatureModelEditor
    {


        public string sourceDirectory { get; set; }

        /// <summary>
        /// Represents legend object on diagram.
        /// </summary>
        private Legend editorLegend { get; set; }

        /// <summary>
        /// Feature Model Editor default constructor.
        /// </summary>
        public FeatureModel()
        {
            InitializeComponent();
            this.Associations = new List<KeyValuePair<Feature, Feature>>();
            this.loadConstraintPanel();
            this.SnapToGrid = true;
            sourceDirectory = "";
            this.editorLegend = new Legend();
            this.rectanglePlaceholder.Visibility = System.Windows.Visibility.Hidden;
        }

        #region Fields and Properties

        /// <summary>
        /// Auxiliar field. Stores a brush to reset diagram background when grid mode is turned off.
        /// For more information, check ~buttonGrid_Click~ method.
        /// </summary>
        public Brush grid = null;

        /// <summary>
        /// Stores current constraint editor. Constraint editor can be changed at runtime.
        /// </summary>
        public IConstraintEditor constraintEditor { get; set; }

        /// <summary>
        /// Is enabled when some feature is about to be added to the diagram.
        /// Triggered by the ADD button, on the toolbar.
        /// </summary>
        public bool IsAdding { get; set; }

        /// <summary>
        /// Returns current selected feature. 
        /// If no feature is selected, returns null.
        /// </summary>
        public Feature SelectedFeature
        {
            get { return selectedFeature; }
            set
            {
                this.selectedFeature = value;
                this.selectedFeature.Select();
                this.updateTypeToolbar(value);
            }
        }


        /// <summary>
        /// Stores current selected feature.
        /// </summary>
        private Feature selectedFeature;

        /// <summary>
        /// Stores value for Snap-to-grid mode. 
        /// Snap-to-grid mode auto align features on diagram.
        /// </summary>
        public bool SnapToGrid
        {
            get;
            set;
        }

        /// <summary>
        /// Enables when a feature is about to be locked with another one.
        /// Active it by clicking the LockButton.
        /// </summary>
        public bool IsLocking { get; set; }

        /// <summary>
        /// Enables when a feature is about to be unlocked from another one.
        /// Active it by clicking the UnlockButton.
        /// </summary>
        public bool IsUnlocking { get; set; }

        /// <summary>
        /// Stores the connections between features. The first feature is the parent feature
        /// and the last feature is the child feature.
        /// </summary>
        public List<KeyValuePair<Feature, Feature>> Associations
        {
            get;
            set;
        }

        /// <summary>
        /// Status of legend. Active or not.
        /// </summary>
        private bool activeLegend { get; set; }

        /// <summary>
        /// Stores a reference to the main application window.
        /// TODO investigate usage. 
        /// </summary>
        public MainWindow CurrentApplicationWindow { get; set; }

        #endregion

        #region Interface Implementation

        public void SetApplicationWindow(MainWindow window)
        {
            this.CurrentApplicationWindow = window;
        }

        /// <summary>
        /// Returns this control, so it can be attached to the main application.
        /// </summary>
        public Control GetControl()
        {
            return this;
        }

        /// <summary>
        /// Export a FeatureModelAtlas to be used by main application.
        /// </summary>
        public AtlasFeatureModel ExportStructure()
        {
            return this.GetAtlasFromDiagram();
        }

        /// <summary>
        /// Imports a FeatureModelAtlas to be used in this modeler.
        /// </summary>
        /// <param name="f"></param>
        public void ImportStructure(AtlasFeatureModel f)
        {
            this.loadModelFromAtlas(f);
        }

        #endregion

        #region Toolbar Buttons


        /// <summary>
        /// Set selected feature type to Optional.
        /// </summary>
        private void radioButtonOptional_Checked(object sender, RoutedEventArgs e)
        {
            if (this.selectedFeature == null)
                return;
            this.selectedFeature.Type = FeatureType.Optional;
            this.UpdateFeatureGroup(this.selectedFeature);
        }

        /// <summary>
        /// Set selected feature type to Mandatory
        /// </summary>
        private void radioButtonMandatory_Checked(object sender, RoutedEventArgs e)
        {
            if (this.selectedFeature == null)
                return;
            this.selectedFeature.Type = FeatureType.Mandatory;
            this.UpdateFeatureGroup(this.selectedFeature);
        }

        /// <summary>
        /// Set selected feature type to Or
        /// </summary>
        private void radionButtonOr_Checked(object sender, RoutedEventArgs e)
        {
            if (this.selectedFeature == null)
                return;

            this.selectedFeature.Type = FeatureType.Or;
            this.UpdateFeatureGroup(this.selectedFeature);
        }

        /// <summary>
        /// Set selected feature type to Alternative.
        /// </summary>
        private void radioButtonAlternative_Checked(object sender, RoutedEventArgs e)
        {
            if (this.selectedFeature == null)
                return;

            this.selectedFeature.Type = FeatureType.Alternative;
            this.UpdateFeatureGroup(this.selectedFeature);
        }

        /// <summary>
        /// Enable or disable snap to grid mode.
        /// </summary>
        private void buttonSnapToGrid_Click(object sender, RoutedEventArgs e)
        {
            this.SnapToGrid = !this.SnapToGrid;

            if (!this.SnapToGrid)
                return;

            //recalculate coordinates as snap-to-grid
            foreach (Feature f in this.gridEditorContainer.Children.OfType<Feature>())
                f.SetPosition(f.GetPosition());

        }

        /// <summary>
        /// Clean the diagram.
        /// </summary>
        private void buttonClear_Click(object sender, RoutedEventArgs e)
        {

            if (MessageBox.Show("Do you want to start a new diagram? Unsaved changes will be lost.",
                "PlugSPL: Creating a new diagram", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                return;

            this.ClearEditor();
        }

        /// <summary>
        /// Prints current diagram. 
        /// TODO: A fix is needed in order to print full diagram. Today, only the visible slice is printable.
        /// </summary>
        private void buttonPrint_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog p = new PrintDialog();
            if (p.ShowDialog() == true)
                p.PrintVisual(this.gridEditorContainer, "Generated byPlugSPL");
            //p.PrintVisual((UserControl)this.constraintEditor, "Generated by Plug");

        }

        /// <summary>
        /// Enable/diable grid mode.
        /// </summary>
        private void buttonGrid_Click(object sender, RoutedEventArgs e)
        {

            if (this.grid == null)
            {
                this.grid = this.gridEditorContainer.Background;
                this.gridEditorContainer.Background = new SolidColorBrush(Colors.White);
            }
            else
            {
                this.gridEditorContainer.Background = this.grid;
                this.grid = null;
            }
        }

        /// <summary>
        /// Shows an open file dialog. Selected file will be loaded as a feature model.
        /// </summary>
        private void buttonOpenFile_Click(object sender, RoutedEventArgs e)
        {

            //stores old Environment.CurrentDirectory property.
            string oldCurrentDir = Environment.CurrentDirectory;

            OpenFileDialog dialog = new OpenFileDialog();

            String[] availableFiles = Directory.GetFiles("./", "FeatureModelEditor.*.dll");
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
                    if (interfaces.Contains(typeof(IFileFormat)))
                    {
                        IFileFormat extension = (IFileFormat)Activator.CreateInstance(t);
                        string filter = extension.GetFilter();
                        if (!dialog.Filter.Equals(""))
                            filter = "|" + filter;
                        dialog.Filter += filter;
                        extensions.Add(t);
                    }
                }
            }

            if (dialog.ShowDialog() != true)
            {
                //restores current directory.
                Environment.CurrentDirectory = oldCurrentDir;
                return;
            }

            //restores current directory.
            Environment.CurrentDirectory = oldCurrentDir;

            string fileToLoad = dialog.FileName;
            this.sourceDirectory = dialog.FileName;
            string ext = fileToLoad.Split('.').Last();

            //Searches amongst valid extensions for the one selected
            foreach (Type t in extensions)
            {
                IFileFormat extension = (IFileFormat)Activator.CreateInstance(t);
                if (extension.GetFilter().Contains(ext))
                {
                    AtlasFeatureModel newAtlas = extension.LoadFrom(fileToLoad);
                    this.loadModelFromAtlas(newAtlas);
                    bool loadLayout = this.loadLayoutFile(fileToLoad);

                    if (!loadLayout) this.PerformLayout();

                    this.UpdateAssociations();

                    if (this.constraintEditor != null)
                    {
                        try
                        {
                            this.constraintEditor.SetFeatureModel(newAtlas);
                            foreach (AtlasConstraint contraint in newAtlas.Constraints)
                            {
                                this.constraintEditor.AddConstraint(contraint);
                            }

                            this.textBlockStatus.Text = "Constraint editor updated at " + DateTime.Now.ToLongTimeString();
                        }
                        catch (Exception)
                        {
                            this.textBlockStatus.Text = "Current feature model contains errors.";
                        }
                    }

                    return;
                }
            }
        }

        /// <summary>
        /// Performs tha layout of the Feature Model diagram
        /// </summary>
        public void PerformLayout()
        {
            Feature root = this.GetRootFeature();

            //this.SnapToGrid = false;
            double outx;
            this.PerformLayout(root, 0, 0, 25, out outx);

            this.UpdateAssociations();
        }

        /// <summary>
        /// Performs the layout of the children of the given source Feature
        /// </summary>
        /// <param name="source">The source from the children to perform layout</param>
        /// <param name="minX">minimum X value to the children position</param>
        /// <param name="y">Y value of the children</param>
        /// <param name="spacing">spacing between two children of the source</param>
        /// <param name="maxX">the maximum value X of the children</param>
        private void PerformLayout(Feature source, double minX, double y, double spacing, out double maxX)
        {
            List<Feature> children = this.GetChildrenFeatures(source);

            if (children.Count == 0)
            {
                maxX = 0;
                //return;
            }

            double inc = 0;
            Point p;
            if (children.Count > 0)
            {
                double childY = y + source.ActualHeight * 2;
                double childX = minX;
                foreach (Feature f in children)
                {
                    PerformLayout(f, childX, childY, spacing, out inc);
                    childX = inc + spacing;
                }
                p = new Point((minX + (inc - minX) / 2 - (source.ActualWidth / 2))
                                            , y);
            }
            else
            {
                p = new Point(minX, y);
            }
            
            source.SetPosition(p);
            System.Diagnostics.Debug.WriteLine("F: " + source.FeatureName + " (x,y): " + p.ToString());
            
            maxX = source.GetPosition().X + source.ActualWidth;
            if (inc > maxX)
                maxX = inc;
        }


        /// <summary>
        /// Shows a save file dialog. Current feature model will be save in selected path.
        /// </summary>
        private void buttonSaveFileAs_Click(object sender, RoutedEventArgs e)
        {

            //stores old current directory.
            string oldCurrentDir = Environment.CurrentDirectory;
            try
            {
                AtlasFeatureModel atlas = this.GetAtlasFromDiagram();
                atlas = this.constraintEditor.UpdateConstraintEditor(atlas);

                SaveFileDialog dialog = new SaveFileDialog();

                String[] availableFiles = Directory.GetFiles("./", "FeatureModelEditor.*.dll");
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
                        if (interfaces.Contains(typeof(IFileFormat)))
                        {
                            IFileFormat extension = (IFileFormat)Activator.CreateInstance(t);
                            string filter = extension.GetFilter();

                            if (dialog.Filter != String.Empty)
                                filter = "|" + filter;

                            dialog.Filter = dialog.Filter + filter;
                            extensions.Add(t);
                        }
                    }
                }

                dialog.FileName = atlas.RootFeatureName;

                if (dialog.ShowDialog() != true)
                {
                    Environment.CurrentDirectory = oldCurrentDir;
                    return;
                }


                string fileToSave = dialog.FileName;
                string ext = fileToSave.Split('.').Last();

                //Searches amongst valid extensions for the one selected
                foreach (Type t in extensions)
                {
                    IFileFormat extension = (IFileFormat)Activator.CreateInstance(t);
                    if (extension.GetFilter().Contains(ext))
                    {
                        FileInfo f = new FileInfo(fileToSave);

                        //if (!atlas.RootFeatureName.Equals(fileToSave.Substring(fileToSave.LastIndexOf('\\') + 1, fileToSave.Length - fileToSave.LastIndexOf('.') - 1)))
                        if (!(atlas.RootFeatureName.ToLower() + f.Extension).Equals(f.Name.ToLower()))
                        {
                            throw new Exception("File name must be the same of the root feature, i.e., " + atlas.RootFeatureName);
                        }

                        extension.SaveTo(atlas, fileToSave);

                        if (File.Exists(fileToSave))
                            this.saveLayoutFile(atlas, fileToSave);

                        //restores currend directory.
                        Environment.CurrentDirectory = oldCurrentDir;
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "PlugSPL Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                //restores currend directory.
                Environment.CurrentDirectory = oldCurrentDir;
            }
        }

        /// <summary>
        /// Adds a feature to diagram.
        /// </summary>
        private void buttonAddFeature_Click(object sender, RoutedEventArgs e)
        {
            this.IsLocking = false;
            this.IsAdding = true;
            this.IsUnlocking = false;
            this.rectanglePlaceholder.Visibility = System.Windows.Visibility.Visible;
        }

        /// <summary>
        /// Removes a feature from diagram.
        /// </summary>
        private void buttonRemoveFeature_Click(object sender, RoutedEventArgs e)
        {
            if (selectedFeature == null)
                return;

            if (MessageBox.Show("Do you want to remove selected feature from diagram? \nNote that:\n- Feature associations will be lost. \n- This operation cannot be undone.", "Really remove that?!",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                return;

            this.selectedFeature.Deselect();
            this.gridEditorContainer.Children.Remove(selectedFeature);

            for (int i = 0; i < this.Associations.Count(); i++)
            {
                KeyValuePair<Feature, Feature> pair = this.Associations[i];
                if (selectedFeature == pair.Key || selectedFeature == pair.Value)
                {
                    this.Associations.Remove(pair);
                }
            }

            if (this.gridEditorContainer.Children.OfType<Feature>().Count() > 0)
            {
                this.gridEditorContainer.Children.OfType<Feature>().First().Select();
            }
            else
            {
                this.selectedFeature = null;
                this.textBoxSelected.Text = "";
            }

            this.UpdateAssociations();
        }

        /// <summary>
        /// Connects a feature to another. 
        /// Former is the parent feature and the last one is the child feature.
        /// </summary>
        private void buttonAddChildFeature_Click(object sender, RoutedEventArgs e)
        {
            this.IsLocking = true;
            this.IsAdding = false;
            this.IsUnlocking = false;
        }

        /// <summary>
        /// Unlocks selected feature from its parent.
        /// </summary>
        private void buttonRemoveChildFeature_Click(object sender, RoutedEventArgs e)
        {
            foreach (KeyValuePair<Feature, Feature> pair in this.Associations)
            {
                if (pair.Value.Equals(this.selectedFeature))
                {
                    this.Associations.Remove(pair);
                    this.UpdateAssociations();
                    this.UpdateGroupDecoration(pair.Key);
                    this.UpdateGroupDecoration(pair.Value);
                    return;
                }
            }
        }

        #endregion

        /// <summary>
        /// Returns an atlas generated from currently displayed diagram.
        /// </summary>
        private AtlasFeatureModel GetAtlasFromDiagram()
        {
            AtlasFeatureModel atlas = new AtlasFeatureModel();

            List<Feature> features = this.GetFeatures();
            List<KeyValuePair<Feature, Feature>> associations = this.Associations;

            //finding root features
            List<Feature> parentFeatures = new List<Feature>();

            foreach (Feature f in features)
            {
                bool hasParent = false;
                foreach (KeyValuePair<Feature, Feature> pair in associations)
                {
                    if (pair.Value == f)
                    {
                        hasParent = true;
                        break;
                    }
                }
                if (!hasParent)
                    parentFeatures.Add(f);
            }

            //checks for amount of root features.
            if (parentFeatures.Count == 0)
            {
                return atlas;
            }
            else if (parentFeatures.Count > 1)
            {
                throw new Exception("Unable to build Atlas due to more than one root feature found.");
            }

            Feature rootFeature = parentFeatures.First();
            AtlasFeature feature = new AtlasFeature(rootFeature.FeatureName);

            feature.IsAbstract = rootFeature.IsAbstract;
            atlas.CreateFeatureModel(feature);

            List<Feature> queue = new List<Feature>();
            queue.Add(rootFeature);

            while (queue.Count > 0)
            {
                List<Feature> children = this.GetChildrenFeatures(queue.First());
                if (children.Count == 0)
                    if (queue.First().IsAbstract)
                        throw new Exception("A feature that is abstract must have child feature. Verify the feature named " + queue.First().FeatureName);

                queue.AddRange(children);
                queue.Remove(queue.First());

                if (queue.Count > 0)
                {
                    CreateAtlasFeature(queue.First(), atlas);
                }
            }
            return atlas;
        }

        /// <summary>
        /// Integrates GetAtlasFromDiagram logic (Support Method)
        /// </summary>
        public void CreateAtlasFeature(Feature feature, AtlasFeatureModel atlas)
        {
            Feature parent = this.GetParentFeature(feature);
            AtlasFeature parentFeature = atlas.GetFeature(parent.FeatureName);
            AtlasFeature childFeature = new AtlasFeature(feature.FeatureName);

            childFeature.IsAbstract = feature.IsAbstract;

            AtlasConnectionType type;
            switch (feature.Type)
            {
                case FeatureType.Alternative:
                    type = AtlasConnectionType.Alternative;
                    break;
                case FeatureType.Mandatory:
                    type = AtlasConnectionType.Mandatory;
                    break;
                case FeatureType.Or:
                    type = AtlasConnectionType.OrRelation;
                    break;
                case FeatureType.Optional:
                default:
                    type = AtlasConnectionType.Optional;
                    break;
            }

            atlas.AddFeature(childFeature, parentFeature, type);
        }

        /// <summary>
        /// Save layout together the data file. Regardless of data file format, the layout file
        /// is saved in the same format.
        /// </summary>
        private void saveLayoutFile(AtlasFeatureModel atlas, string fileToSave)
        {
            FileStream fileStream = File.Create(fileToSave + ".layout");
            TextWriter b = new StreamWriter(fileStream);
            b.WriteLine("# This layout file was generated by PlugSPL Environment.");
            b.WriteLine("# DO NOT MODIFY THIS FILE BY HAND!");

            Feature[] features = this.GetFeatures().ToArray();
            for (int i = 0; i < features.Count(); i++)
            {

                Feature f = features[i];

                string s = String.Format("{0}~{1}~{2}", f.FeatureName, f.GetPosition().X, f.GetPosition().Y);

                if (features.Last().Equals(f))
                    b.Write(s);
                else
                    b.WriteLine(s);
            }

            b.Close();
            fileStream.Close();
        }

        /// <summary>
        /// Adds given feature to grid container.
        /// </summary>
        /// <param name="feature"></param>
        private void AddFeature(Feature feature)
        {
            if (feature.OwnerEditor != this)
                feature.OwnerEditor = this;
            this.GetContainer().Children.Add(feature);
        }

        /// <summary>
        /// Associate last feature as child of former one.
        /// </summary>
        public void LockFeatures(Feature source, Feature target)
        {

            //feature cant lock with itself.
            if (source.Equals(target))
            {
                MessageBox.Show("Cannot attach a feature to itself.", "PlugSPL - Feature association failure.",
                    MessageBoxButton.OK, MessageBoxImage.Hand);
                return;
            }

            foreach (KeyValuePair<Feature, Feature> p in this.Associations)
            {

                //feature cannot have two parent.
                if (p.Value.Equals(target))
                {
                    MessageBox.Show("Selected feature already has a parent attached to it.\nUnlock this feature or select another one to lock.",
                        "PlugSPL - Feature association failure.", MessageBoxButton.OK, MessageBoxImage.Hand);
                    return;
                }
            }
            //locate source children
            List<Feature> children = this.GetChildrenFeatures(source);

            if (children.Count == 0)
            { //this is the first children.
                switch (target.Type)
                {
                    case FeatureType.Mandatory:
                    case FeatureType.Optional:
                        source.DisableGrouping();
                        break;
                    case FeatureType.Or:
                        source.EnableOrGrouping();
                        break;
                    case FeatureType.Alternative:
                        source.EnableAlternativeGrouping();
                        break;
                }
            }
            else
            { //set type of new child to reflect previous type.
                target.SetType(children.First().Type);
            }

            KeyValuePair<Feature, Feature> pair = new KeyValuePair<Feature, Feature>(source, target);
            this.UpdateFeatureGroup(target);
            this.Associations.Add(pair);
            this.UpdateAssociations();
        }

        /// <summary>
        /// Returns a list of associations that has the given Feature as source.
        /// </summary>
        private List<Feature> GetChildrenFeatures(Feature source)
        {
            List<Feature> children = new List<Feature>();
            foreach (KeyValuePair<Feature, Feature> pair in this.Associations)
            {
                if (pair.Key == source)
                    children.Add(pair.Value);
            }
            return children;
        }

        /// <summary>
        /// Update visual lines for associations.
        /// </summary>
        private void UpdateAssociations()
        {
            List<Line> oldLines = this.gridEditorContainer.Children.OfType<Line>().ToList();
            foreach (Line l in oldLines)
            {
                this.gridEditorContainer.Children.Remove(l);
            }
            foreach (KeyValuePair<Feature, Feature> f in this.Associations)
            {
                Point a = f.Key.GetLowerAttachPoint();
                Point b = f.Value.GetUpperAttachPoint();

                Line newLine = new Line();
                newLine.X1 = a.X;
                newLine.X2 = b.X;
                newLine.Y1 = a.Y;
                newLine.Y2 = b.Y;

                newLine.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                newLine.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;

                newLine.Stroke = new SolidColorBrush(Colors.Black);

                this.gridEditorContainer.Children.Add(newLine);
                Grid.SetZIndex(newLine, -1);
            }
        }

        /// <summary>
        /// Returns the container.
        /// </summary>
        /// <returns></returns>
        public Grid GetContainer()
        {
            return this.gridEditorContainer;
        }

        /// <summary>
        /// Update a group of faetures to match a specific FeatureType.
        /// </summary>
        public void UpdateFeatureGroup(Feature feature)
        {

            Feature parent = this.GetParentFeature(feature);

            if (parent == null) //no features at same level to update
                return;

            foreach (KeyValuePair<Feature, Feature> pair in this.Associations)
            {
                if (pair.Key.Equals(parent) && !pair.Value.Equals(feature))
                { //locate features at same level

                    if (feature.Type == FeatureType.Alternative || feature.Type == FeatureType.Or)
                    { //feature must reflect group changes
                        pair.Value.SetType(feature.Type);
                    }
                    else
                    {  //feature is mandatory or optional
                        if (pair.Value.Type != FeatureType.Optional) //change to AND grouping
                            pair.Value.SetType(FeatureType.Mandatory);
                    }
                }
            }

            //enable container on parent
            switch (feature.Type)
            {
                case FeatureType.Alternative:
                    parent.EnableAlternativeGrouping();
                    break;
                case FeatureType.Or:
                    parent.EnableOrGrouping();
                    break;
                default:
                    parent.DisableGrouping();
                    break;
            }
        }

        /// <summary>
        /// Returns parent feature of a feature. Returns null if given feature has no parent.
        /// </summary>
        public Feature GetParentFeature(Feature f)
        {
            foreach (KeyValuePair<Feature, Feature> p in this.Associations)
            {
                if (p.Value.FeatureName == f.FeatureName)
                {
                    return p.Key;
                }
            }
            return null;
        }

        /// <summary>
        /// Update toolbar button states. 
        /// </summary>
        private void updateTypeToolbar(Feature value)
        {

            //Update type
            switch (value.Type)
            {
                case FeatureType.Alternative:
                    this.radioButtonAlternative.IsChecked = true;
                    break;
                case FeatureType.Mandatory:
                    this.radioButtonMandatory.IsChecked = true;
                    break;
                case FeatureType.Optional:
                    this.radioButtonOptional.IsChecked = true;
                    break;
                case FeatureType.Or:
                    this.radionButtonOr.IsChecked = true;
                    break;
            }

            //update name
            this.textBoxSelected.Text = value.FeatureName;
            buttonAbstract.IsChecked = value.IsAbstract;
        }

        /// <summary>
        /// Return feature containing given name (if exists).
        /// </summary>
        private Feature GetFeatureByName(string p)
        {
            List<Feature> ff = this.GetFeatures();
            foreach (Feature f in ff)
            {
                if (f.FeatureName == p)
                    return f;
            }
            return null;
        }

        /// <summary>
        /// Return a list of features from diagram container.
        /// </summary>
        public List<Feature> GetFeatures()
        {
            return this.gridEditorContainer.Children.OfType<Feature>().ToList();
        }

        /// <summary>
        /// Loads a feature model from given atlas and update the feature model editor with it.
        /// </summary>
        private void loadModelFromAtlas(AtlasFeatureModel newAtlas)
        {
            this.ClearEditor();
            foreach (AtlasFeature f in newAtlas.Features)
            {
                Feature newFeature = new Feature();
                newFeature.IsAbstract = f.IsAbstract;
                newFeature.FeatureName = f.Name;
                this.AddFeature(newFeature);
            }
            foreach (AtlasConnection c in newAtlas.Connections)
            {
                Feature source = this.GetFeatureByName(c.Parent.Name);
                Feature target = this.GetFeatureByName(c.Child.Name);
                switch (c.Type)
                {
                    case AtlasConnectionType.Mandatory:
                        target.Type = FeatureType.Mandatory;
                        break;
                    case AtlasConnectionType.Optional:
                        target.Type = FeatureType.Optional;
                        break;
                    case AtlasConnectionType.Alternative:
                        target.Type = FeatureType.Alternative;
                        break;
                    case AtlasConnectionType.OrRelation:
                    case AtlasConnectionType.Excludes:
                    case AtlasConnectionType.Requires:
                    default:
                        target.Type = FeatureType.Or;
                        break;
                }
                this.Associations.Add(new KeyValuePair<Feature, Feature>(source, target));
                this.UpdateGroupDecoration(this.GetRootFeature());
            }
            this.UpdateAssociations();
        }

        /// <summary>
        /// Refreshs visual grouping for features.
        /// </summary>
        private void UpdateGroupDecoration(Feature feature)
        {
            if (feature == null)
                return;

            List<Feature> children = this.GetChildrenFeatures(feature);

            if (children.Count == 0)
            {
                feature.DisableGrouping();
                return;
            }
            else
            {
                switch (children.First().Type)
                {
                    case FeatureType.Alternative:
                        feature.EnableAlternativeGrouping();
                        break;
                    case FeatureType.Optional:
                        feature.EnableOrGrouping();
                        break;
                }
            }

            foreach (Feature f in children)
            {
                this.UpdateGroupDecoration(f);
            }
        }

        /// <summary>
        /// Returns top-level feature from current diagram. 
        /// </summary>
        private Feature GetRootFeature()
        {
            List<Feature> hasParent = new List<Feature>();
            foreach (Feature f in this.GetFeatures())
            {
                foreach (KeyValuePair<Feature, Feature> pair in this.Associations)
                {
                    if (!hasParent.Contains(pair.Value))
                        hasParent.Add(pair.Value);
                }
            }

            foreach (Feature f in this.GetFeatures())
            {
                if (!hasParent.Contains(f))
                    return f;
            }

            return null;
        }

        /// <summary>
        /// Removes all controls from diagram.
        /// </summary>
        private void ClearEditor()
        {
            this.Associations.Clear();
            List<Feature> features = this.GetFeatures();
            foreach (Feature f in features)
                this.gridEditorContainer.Children.Remove(f);

            this.constraintEditor.Clear();

            this.UpdateAssociations();

        }

        /// <summary>
        /// Loads a layout file from given path and adjust features to retrieved positions.
        /// </summary>
        private bool loadLayoutFile(string fileToLoad)
        {

            if (!File.Exists(fileToLoad + ".layout"))
            {
                MessageBox.Show("No layout file found. A brand file will be generated in the next time you save this project.",
                    "PlugSPL warning: No layout file found.",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return false;
            }

            fileToLoad += ".layout";

            try
            {
                using (TextReader reader = new StreamReader(fileToLoad))
                {
                    string[] lines = reader.ReadToEnd().Split('\n');
                    foreach (string line in lines)
                    {
                        if (line == "" || line.StartsWith("#"))
                            continue;

                        string featureName = line.Split('~')[0];
                        string x = line.Split('~')[1];
                        string y = line.Split('~')[2];

                        Feature f = this.GetFeatureByName(featureName);

                        if (f == null)
                        {
                            continue;
                        }
                        else
                        {
                            Point position = new Point(Convert.ToInt32(x), Convert.ToInt32(y));
                            f.SetPosition(position);
                        }
                    }
                }
                return true;
            }
            catch (Exception)
            {
                MessageBox.Show("Layout file is corrupted. Save this project to generate a new brand layout file.",
                    "PlugSPL error: Layout file is corrupted.",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return false;
            }
        }

        /// <summary>
        /// Initialize constraint editor into grid.
        /// </summary>
        private void loadConstraintPanel()
        {
            return;
            //TODO Figure out a config file for this.
            if (!File.Exists("./Configuration/FeatureModelEditor.xml"))
                MessageBox.Show("Unable to load FeatureModelEditor configuration file. \nFile do not exists: <./Configuration/FeatureModelEditor.xml>",
                    "PlugSPL Error: Unable to find configuration file.", MessageBoxButton.OK, MessageBoxImage.Error);

            TextReader reader = new StreamReader("./Configuration/FeatureModelEditor.xml");
            XmlSerializer serializer = new XmlSerializer(typeof(List<Module>));
            List<Module> modules = (List<Module>)serializer.Deserialize(reader);
            reader.Close();

            /* USE IT TO GENERATE A NEW CONFIGURATION FILE
            TextWriter writer = new StreamWriter("./Configuration/FeatureModelEditor.xml");
            XmlSerializer serializer = new XmlSerializer(typeof(List<Module>));
            serializer.Serialize(writer, listofmodules);
            writer.Close();
            */

            foreach (Module l in modules)
            {
                if (l.Interface != "IConstraintEditor")
                    continue;
                if (!File.Exists(l.Assembly))
                    continue;
                Assembly asm = Assembly.LoadFrom(l.Assembly);
                Type type = asm.GetType(l.Type);

                if (type == null)
                    continue;

                //instantiate constraint editor
                IConstraintEditor editor = (IConstraintEditor)Activator.CreateInstance(type);

                //put constraint editor into grid
                gridContraintEditor.Children.Clear();
                gridContraintEditor.Children.Add((UserControl)editor.GetControl());

                this.constraintEditor = editor;
            }
        }


        #region Events and Delegates

        /// <summary>
        /// Switches over pressed keys and calls respective method.
        /// </summary>
        private void gridEditorContainer_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left: //moves feature to left
                    this.SelectedFeature.SetPosition(new Point()
                    {
                        X = this.SelectedFeature.GetPosition().X - 12,
                        Y = this.SelectedFeature.GetPosition().Y
                    });
                    break;
                case Key.Right: //moves feature to right
                    this.SelectedFeature.SetPosition(new Point()
                    {
                        X = this.SelectedFeature.GetPosition().X + 12,
                        Y = this.SelectedFeature.GetPosition().Y
                    });
                    break;
                case Key.Up: //moves feature to up
                    this.SelectedFeature.SetPosition(new Point()
                    {
                        X = this.SelectedFeature.GetPosition().X,
                        Y = this.SelectedFeature.GetPosition().Y - 12
                    });
                    break;
                case Key.Down: //moves feature to down
                    this.SelectedFeature.SetPosition(new Point()
                    {
                        X = this.SelectedFeature.GetPosition().X,
                        Y = this.SelectedFeature.GetPosition().Y + 12
                    });
                    break;
                case Key.Delete: //removes feature from diagram
                    this.buttonRemoveFeature_Click(this.buttonRemoveFeature, new RoutedEventArgs());
                    break;
                case Key.C: //connects two features
                    this.buttonAddChildFeature_Click(this.buttonAddChildFeature, new RoutedEventArgs());
                    break;
                case Key.A: //adds feature to diagram
                    this.buttonAddFeature_Click(this.buttonAddFeature, new RoutedEventArgs());
                    break;
            }

            this.UpdateAssociations();
        }

        /// <summary>
        /// Set focus to diagram and update placeholder coordinates when mouse is moving over diagram.
        /// </summary>
        private void gridEditorContainer_MouseMove(object sender, MouseEventArgs e)
        {

            this.gridEditorContainer.Focus();

            //updates placeholder coordinates
            if (this.IsAdding)
            {

                //calculate feature position
                Point p = Mouse.GetPosition(this); //mouse location

                p.X = p.X - 85; //mouse_position_X - feature_width * 0.5 - margin_X 
                p.Y = p.Y - 90; //mouse_position_Y - feature_height * 0.5 - margin_Y

                //snap-to-grid calculation
                if (this.SnapToGrid)
                {
                    int x = (int)p.X;
                    x = (x / 24) * 24;
                    p.X = x;

                    //adding "mandatory circle" height value
                    int y = (int)p.Y;// + 7;
                    y = (y / 24) * 24;
                    p.Y = y;// - 7;
                }
                else
                {
                    p.Y += 7;
                }


                //adjust placeholder margins
                this.rectanglePlaceholder.Margin = new Thickness(Math.Abs(p.X), Math.Abs(p.Y), 0, 0);
            }
        }

        /// <summary>
        /// Restore cursor to pointer when mouse leaves diagram.
        /// </summary>
        private void gridEditorContainer_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
        }

        /// <summary>
        /// Changes cursor when mouse is over diagram.
        /// </summary>
        private void gridEditorContainer_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        /// <summary>
        /// Set selected feature as abstract.
        /// </summary>
        private void buttonAbstract_Click(object sender, RoutedEventArgs e)
        {
            if (this.selectedFeature == null)
                return;
            this.selectedFeature.IsAbstract = (this.buttonAbstract.IsChecked == true);
        }

        /// <summary>
        /// Triggers on MouseDown event for gridContainerEditor. Process several diagraming actions.
        /// </summary>
        private void gridEditorContainer_MouseDown(object sender, MouseButtonEventArgs e)
        {

            //check if it is on "add mode".
            if (this.IsAdding)
            {

                //generate generic name
                int counter = 0;
                while (this.GetFeatureByName("Feature " + counter) != null)
                    counter++;

                //creates a new feature
                Feature f = new Feature("Feature " + counter);

                //adds feature to diagram
                this.AddFeature(f);

                //calculate feature position
                Point p = Mouse.GetPosition(this); //mouse location

                p.X = p.X - 85; //mouse_position_X - feature_width * 0.5 - margin_X 
                p.Y = p.Y - 90; //mouse_position_Y - feature_height * 0.5 - margin_Y

                f.SetPosition(p);
                f.Select();

                this.IsAdding = false; //disable "add mode".
                this.buttonAddFeature.IsChecked = false; //allow button to be actived again.
                this.rectanglePlaceholder.Visibility = System.Windows.Visibility.Hidden;
            }

            if (this.constraintEditor != null)
            {
                try
                {
                    this.constraintEditor.SetFeatureModel(this.GetAtlasFromDiagram());
                    this.textBlockStatus.Text = "Constraint editor updated at " + DateTime.Now.ToLongTimeString();
                }
                catch (Exception)
                {
                    this.textBlockStatus.Text = "Current feature model contains errors.";
                }

            }
        }

        /// <summary>
        /// OnDrop event. Trigger when sometinhg is droped on diagram.
        /// </summary>
        private void gridEditorContainer_Drop(object sender, DragEventArgs e)
        {

            Object[] data = (Object[])e.Data.GetData(typeof(Object[]));
            Feature f = (Feature)(data[0]);
            Point orientation = (Point)data[1];

            if (f == null) return;

            Point dropPoint = e.GetPosition(f as Control);

            f.SetPosition(new Point()
            {
                X = f.GetPosition().X + dropPoint.X - orientation.X,
                Y = f.GetPosition().Y + dropPoint.Y - orientation.Y
            });

            this.UpdateAssociations();
            this.rectanglePlaceholder.Visibility = System.Windows.Visibility.Hidden;

            if (this.constraintEditor != null)
            {
                try
                {
                    this.constraintEditor.SetFeatureModel(this.GetAtlasFromDiagram());
                    this.textBlockStatus.Text = "Constraint editor updated at " + DateTime.Now.ToLongTimeString();
                }
                catch (Exception)
                {
                    this.textBlockStatus.Text = "Current feature model contains errors.";
                }

            }
        }

        /// <summary>
        /// Renames selected feature. Triggered on ConfirmButton click (toolbar).
        /// </summary>
        private void textBoxSelected_KeyDown(object sender, KeyEventArgs e)
        {

            switch (e.Key)
            {
                case Key.Enter:
                    if (this.GetFeatureByName(textBoxSelected.Text) != null)
                    {
                        MessageBox.Show("Unable to rename selected feature. A feature with given name already exists.", "PlugSPL - Feature renaming exception.", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (this.SelectedFeature == null || String.IsNullOrEmpty(textBoxSelected.Text))
                        return;

                    this.SelectedFeature.FeatureName = textBoxSelected.Text;
                    break;
                case Key.Escape:
                    this.textBoxSelected.Text = this.selectedFeature.FeatureName;
                    break;
            }
        }

        #endregion

        private void buttonPerformLayout_Click(object sender, RoutedEventArgs e)
        {
            PerformLayout();
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            //stores old current directory.

            if (this.sourceDirectory != "")
            {
                try
                {
                    if (MessageBox.Show("Do you really want overwrite the old save file from current diagram?", "Confirmation", MessageBoxButton.YesNoCancel) == MessageBoxResult.Yes)
                    {
                        AtlasFeatureModel atlas = this.GetAtlasFromDiagram();
                        atlas = this.constraintEditor.UpdateConstraintEditor(atlas);


                        String[] availableFiles = Directory.GetFiles("./", "FeatureModelEditor.*.dll");
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
                                if (interfaces.Contains(typeof(IFileFormat)))
                                {
                                    IFileFormat extension = (IFileFormat)Activator.CreateInstance(t);
                                    string filter = extension.GetFilter();

                                    extensions.Add(t);
                                }
                            }
                        }


                        string fileToSave = this.sourceDirectory;
                        string ext = fileToSave.Split('.').Last();

                        //Searches amongst valid extensions for the one selected
                        foreach (Type t in extensions)
                        {
                            IFileFormat extension = (IFileFormat)Activator.CreateInstance(t);
                            if (extension.GetFilter().Contains(ext))
                            {
                                FileInfo f = new FileInfo(fileToSave);

                                //if (!atlas.RootFeatureName.Equals(fileToSave.Substring(fileToSave.LastIndexOf('\\') + 1, fileToSave.Length - fileToSave.LastIndexOf('.') - 1)))
                                if (!(atlas.RootFeatureName.ToLower() + f.Extension).Equals(f.Name.ToLower()))
                                {
                                    this.buttonSaveFileAs_Click(sender, e);
                                }

                                extension.SaveTo(atlas, fileToSave);

                                if (File.Exists(fileToSave))
                                    this.saveLayoutFile(atlas, fileToSave);

                            }
                        }
                    }
                    else
                    {
                        return;
                    }
                }

                catch (Exception ex)
                {

                    MessageBox.Show(ex.Message, "PlugSPL Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
            else
            {
                this.buttonSaveFileAs_Click(sender, e);
            }
        }


        #region legend implemetation
        //private void buttonActivateLegend_Click(object sender, RoutedEventArgs e)
        //{

        //    if (!this.activeLegend)
        //    {

        //        this.gridEditorContainer.Children.Add(this.editorLegend);
        //        this.activeLegend = true;
        //        double position = 0;

        //        foreach (UIElement f in this.GetContainer().Children)
        //        {
        //            if (f is Feature)
        //            {
        //                if (((Feature)f).GetPosition().X > position)
        //                {
        //                    position = ((Feature)f).GetPosition().X;
        //                }
        //            }
        //        }

        //        this.editorLegend.Margin = new Thickness(position - 10, 10, 0, 0);
        //        this.editorLegend.VerticalAlignment = System.Windows.VerticalAlignment.Top;
        //        this.editorLegend.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
        //        ListView lv = new ListView();
        //        ListView lv2 = new ListView();

        //        foreach (object lvi in this.constraintEditor.contraintList().Items)
        //        {
        //            ListViewItem l2 = new ListViewItem();
        //            l2.Content = lvi;

        //            if (lv.Items.Count < 5)
        //            {

        //                lv.Items.Add(l2);
        //            }
        //            else
        //            {
        //                lv2.Items.Add(l2);
        //            }
        //        }

        //        double maxY = 0;
        //        foreach (UIElement f in this.GetContainer().Children)
        //        {
        //            if (f is Feature)
        //            {
        //                if (((Feature)f).GetPosition().Y > maxY)
        //                {
        //                    maxY = ((Feature)f).GetPosition().Y;
        //                }
        //            }
        //        }

        //        lv.BorderBrush = new SolidColorBrush(Colors.White);
        //        lv.Width = 600;
        //        lv.Height = 400;
        //        lv.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
        //        lv.VerticalAlignment = System.Windows.VerticalAlignment.Top;
        //        lv.Margin = new Thickness((this.gridEditorContainer.Margin.Left + this.gridEditorContainer.ActualWidth / 2) - (lv.Width / 2 + lv.Margin.Left) + 60, maxY + 45, 0, 0);
        //        lv.Background = new SolidColorBrush(Colors.Transparent);


        //        lv2.BorderBrush = new SolidColorBrush(Colors.White);
        //        lv2.Width = 600;
        //        lv2.Height = 400;
        //        lv2.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
        //        lv2.VerticalAlignment = System.Windows.VerticalAlignment.Top;
        //        lv2.Margin = new Thickness(lv.Margin.Left + (lv.Width / 2) + 60, lv.Margin.Top, 0, 0);
        //        lv2.Background = new SolidColorBrush(Colors.Transparent);

        //        this.gridEditorContainer.Children.Add(lv);
        //        this.gridEditorContainer.Children.Add(lv2);
        //    }

        //    else
        //    {
        //        this.gridEditorContainer.Children.Remove(this.editorLegend);
        //        this.activeLegend = false;
        //    }
        //}
        #endregion
    }

}
