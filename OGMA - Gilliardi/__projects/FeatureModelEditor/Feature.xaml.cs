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

namespace FeatureModelEditor {
    /// <summary>
    /// Interaction logic for FeatureElement.xaml
    /// </summary>
    public partial class Feature : UserControl {

        #region Constructors
        /// <summary>
        /// Deafult Constructor.
        /// </summary>
        public Feature() {
            InitializeComponent();
            this.FeatureName = "New Feature"; 
            this.SetOr(); //Or is the default value for Type.
            this.DisableGrouping();
        }
        
        /// <summary>
        /// Custom contructor. Allos to set the ownerEditor at instantiation.
        /// </summary>
        /// <param name="editor"></param>
        public Feature(FeatureModel editor){
            InitializeComponent();
            this.OwnerEditor = editor;
            this.FeatureName = "New Feature";
            this.SetOr(); //Or is the default value for Type.
            this.DisableGrouping();
        }

        /// <summary>
        /// Custom constructor.
        /// </summary>
        public Feature(FeatureModel editor, String name){
            InitializeComponent();
            this.OwnerEditor = editor;
            this.FeatureName = name;
            this.SetOr(); //Or is the default value for Type.
            this.DisableGrouping();
        }
                
        /// <summary>
        /// Custom constructor.
        /// </summary>
        public Feature(String name){
            InitializeComponent();
            this.FeatureName = name;
            this.SetOr(); //Or is the default value for Type.
            this.DisableGrouping();
        }

        #endregion
        #region Fields and Properties
        
        private FeatureType type;
        private string featureName;
        private FeatureModel ownerEditor;
        private bool isSelected;
        private bool isAbstract;

        /// <summary>
        /// Marks this feature as abstract;
        /// </summary>
        public bool  IsAbstract{
             get{return this.isAbstract;}
            set{ this.SetAbstract(value);}
        }

        /// <summary>
        /// Marks the "Selected" state - is this feature selected in the 
        /// ownerEditor?
        /// </summary>
        public bool IsSelected { 
            get{return this.isSelected;}
            set{this.Select(); }
        }

        /// <summary>
        /// Represents the current feature Type. 
        /// </summary>
        public FeatureType Type{
            get { return type; }
            set { this.SetType(value); }
        }

        /// <summary>
        /// Represents the feature name. It is used as unique identifier between features.
        /// </summary>
        public String FeatureName{
            get{ return this.featureName; }
            set{
                this.textBlockName.Text = value;
                this.featureName = value;
            }
        }

        /// <summary>
        /// Contains the parent diagram of this feature.
        /// </summary>
        public FeatureModel OwnerEditor { 
            get{return this.ownerEditor; }
            set{this.ownerEditor = value;}
        }

        #endregion
        #region Events and Delegates

        /// <summary>
        /// Starts drag operation.
        /// </summary>
        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e) {

            if(this.OwnerEditor.IsLocking){
                this.OwnerEditor.LockFeatures(this.OwnerEditor.SelectedFeature, this);
                this.OwnerEditor.IsLocking = false;
            }else{
                Object[] data = new Object[] { this, Mouse.GetPosition(this) };
                DragDrop.DoDragDrop(this, data, DragDropEffects.Move);
            }

            this.Select();
        }

        #endregion
        #region Other Methods

        /// <summary>
        /// Set this feature as abstract.
        /// </summary>
        private void SetAbstract(bool value) {
            this.isAbstract = value;

            if(value){
                this.gridSquareBox.Background = new SolidColorBrush(Color.FromRgb(0x99, 0xAA, 0x77));
            }else{
                this.gridSquareBox.Background = new SolidColorBrush(Color.FromRgb(0x73, 0xA3, 0xC4));
            }
        }

        /// <summary>
        /// Sets the Type property to given FeatureType. It works as shortcut to
        /// SetOr, SetAlternative, SetMandatory and SetOptional methods.
        /// </summary>
        public void SetType(FeatureType value) {

            switch(value){  
                case FeatureType.Or:   
                    this.SetOr();   
                    break;
                case FeatureType.Optional:
                    this.SetOptional();
                    break;
                case FeatureType.Mandatory:
                    this.SetMandatory();
                    break;
                case FeatureType.Alternative:
                    this.SetAlternative();
                    break;
            }
        }
        
        /// <summary>
        /// Selects current feature. Its border color turns to red and it becomes draggable.
        /// </summary>
        internal void Select(){

            if(this.IsSelected)
                return;

            this.isSelected = true;
            borderSelection.BorderBrush = new SolidColorBrush(Colors.Red);

            //remove selection from other features
            foreach(Feature f in this.OwnerEditor.GetContainer().Children.OfType<Feature>()){
                if(f != this)
                    f.Deselect();
            }
            this.OwnerEditor.SelectedFeature = this;
        }

        /// <summary>
        /// Remove selection from current feature.
        /// </summary>
        internal void Deselect(){
            this.isSelected = false;
            borderSelection.BorderBrush = new SolidColorBrush(Colors.Transparent);
        }

        /// <summary>
        /// Set this features as Alternative feature.
        /// </summary>
        public void SetAlternative(){
            this.ellipseMandatory.Visibility = System.Windows.Visibility.Hidden;
            this.type = FeatureType.Alternative;
        }

        /// <summary>
        /// Set this features as Or feature.
        /// </summary>
        public void SetOr(){
            this.ellipseMandatory.Visibility = System.Windows.Visibility.Hidden;
            this.type = FeatureType.Or;
        }

        /// <summary>
        /// Set this features as Mandatory feature.
        /// </summary>
        public void SetMandatory(){
            this.ellipseMandatory.Visibility = System.Windows.Visibility.Visible;
            this.ellipseMandatory.Fill = new SolidColorBrush(Colors.Black);
            this.type = FeatureType.Mandatory;
        }

        /// <summary>
        /// Set this features as Optional feature.
        /// </summary>
        public void SetOptional(){
            this.ellipseMandatory.Visibility = System.Windows.Visibility.Visible;
            this.ellipseMandatory.Fill = new SolidColorBrush(Colors.White);
            this.type = FeatureType.Optional;
        }
        #endregion
        #region Positional Methods

        /// <summary>
        /// returns current position of this control into its editor.
        /// </summary>
        /// <returns></returns>
        internal Point GetPosition() {
            return new Point(this.Margin.Left, this.Margin.Top);
        }

        /// <summary>
        /// Moves this control through its parent by modifying its margin attr.
        /// </summary>
        public void SetPosition(Point target) {
            
            if(this.OwnerEditor == null)
                return;
        
            //snap to grid per 0.12 inches
            if(this.OwnerEditor.SnapToGrid){
                int x = (int)target.X;
                x = (x / 12) * 12;
                target.X = x;

                // 7 => height of "mandatory signal" above feature
                int y = (int)target.Y + 7;
                y = (y / 12) * 12;
                target.Y = y - 7;
            }

            //Abs prevent user from move elements to negative position
            this.Margin = new Thickness(Math.Abs(target.X), Math.Abs(target.Y), 0, 0);
        }

        /// <summary>
        /// Returns a point where a line must attach to link it with a parent feature.
        /// </summary>
        public Point GetUpperAttachPoint(){
            this.UpdateLayout();
            Point p = this.GetPosition();
            p.X += this.ActualWidth / 2;
            p.Y += 8;
            return p;
        }
        
        /// <summary>
        /// Returns a point where a line must attach to link it with a child feature.
        /// </summary>
        public Point GetLowerAttachPoint(){
            this.UpdateLayout();
            Point p = this.GetPosition();
            p.X += this.ActualWidth / 2;
            p.Y += this.Height - 8;
            return p;
        }

        #endregion
        #region Enable/Disable grouping

        public void EnableAlternativeGrouping() {
            this.ellipseOr.Visibility = System.Windows.Visibility.Visible;
            this.ellipseOr.Stroke = new SolidColorBrush(Colors.Black);
            this.ellipseOr.Fill = new SolidColorBrush(Colors.White);
        }

        public void EnableOrGrouping() {
            this.ellipseOr.Visibility = System.Windows.Visibility.Visible;
            this.ellipseOr.Stroke = new SolidColorBrush(Colors.Black);
            this.ellipseOr.Fill = new SolidColorBrush(Colors.Black);
        }

        public void DisableGrouping() {
            this.ellipseOr.Visibility = System.Windows.Visibility.Hidden;
        }

        #endregion

        public override string ToString() {
            return this.FeatureName;
        }
    }
}
