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
using PlugSpl.DataStructs.ComponentPoolManager;

namespace ComponentPoolManager {
    /// <summary>
    /// Interaction logic for MethodEditor.xaml
    /// </summary>
    public partial class MethodEditor : UserControl {
        private TextBox tempBox = new TextBox();
        public MethodEditor() {
            InitializeComponent();

            this.tempBox.KeyDown += new KeyEventHandler(tempBox_KeyDown);
        }

        public MethodEditor(EshuMethod method)
            :this()
        {
            // TODO: Complete member initialization
            //this.m = m;
            this.textBlockMethod.Text = method.Name;
            this.textBlockType.Text = method.ReturnType;
            
            foreach (EshuProperty p in method.Parameters)
            {
                this.listBoxParameters.Items.Add(new ParameterEditor(p));
            }
        }

        private void buttonAdd_Click(object sender, RoutedEventArgs e) {
            ParameterEditor p = new ParameterEditor();
            this.listBoxParameters.Items.Add(p);
        }



        /// <summary>
        /// Start editing mode.
        /// </summary>
        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            this.tempBox.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            this.tempBox.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            this.tempBox.Text = this.textBlockType.Text + " " + this.textBlockMethod.Text;
            //this.tempBox.Margin = this.textBlockCaption.Margin;
            this.textBlockType.Visibility = System.Windows.Visibility.Collapsed;
            this.textBlockMethod.Visibility = System.Windows.Visibility.Collapsed;
            this.gridMainContainer.Children.Add(this.tempBox);
        }

        void tempBox_KeyDown(object sender, KeyEventArgs e) {
            try
            {
                switch (e.Key)
                {
                    case Key.Enter:
                    case Key.Tab:
                        if (this.tempBox.Text.Split(' ').Count() != 2)
                            throw new Exception("The method must have only one return type and one name.");

                        this.textBlockType.Text = this.tempBox.Text.Split(' ')[0];
                        this.textBlockMethod.Text = this.tempBox.Text.Split(' ')[1];
                        goto case Key.Escape;
                    case Key.Escape:
                        this.gridMainContainer.Children.Remove(this.tempBox);
                        this.textBlockType.Visibility = System.Windows.Visibility.Visible;
                        this.textBlockMethod.Visibility = System.Windows.Visibility.Visible;
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "PlugSPL Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void buttonRemove_Click(object sender, RoutedEventArgs e) {
            if(this.listBoxParameters.SelectedItem != null)
                this.listBoxParameters.Items.Remove(this.listBoxParameters.SelectedItem);
            if(this.listBoxParameters.Items.Count > 0)
                this.listBoxParameters.SelectedIndex = 0;
        }

        private void buttonRemoveMethod_Click(object sender, RoutedEventArgs e) {
            StackPanel panel = this.Parent as StackPanel;
            panel.Children.Remove(this);
        }



        public EshuMethod GetEshuMethod() {
            EshuMethod eshuMethod = new EshuMethod(this.MethodName, this.MethodReturnType);
            eshuMethod.Parameters = new List<EshuProperty>();
            foreach(ParameterEditor parameterEditor in this.listBoxParameters.Items.OfType<ParameterEditor>()){
                EshuProperty eshuProperty = parameterEditor.GetEshuProperty();
                eshuMethod.Parameters.Add(eshuProperty);
            }
            return eshuMethod;
        }

        public string MethodName { get{return this.textBlockMethod.Text;} }
        public string MethodReturnType { get{return this.textBlockType.Text;} }
    }
}
