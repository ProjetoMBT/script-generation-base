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
    /// Interaction logic for ParameterEditor.xaml
    /// </summary>
    public partial class ParameterEditor : UserControl {
        public ParameterEditor() {
            InitializeComponent();
        }

        public ParameterEditor(EshuProperty property):this()
        {
            this.textBlockVariableName.Text=property.Name;
            this.textBlockType.Text = property.Type;
        }

        private void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            this.textBoxInput.Text = textBlockType.Text + " " + textBlockVariableName.Text;
            this.textBlockVariableName.Visibility = System.Windows.Visibility.Collapsed;
            this.textBlockType.Visibility = System.Windows.Visibility.Collapsed;
            this.textBoxInput.Visibility = System.Windows.Visibility.Visible;
        }

        private void textBoxInput_KeyDown(object sender, KeyEventArgs e) {
            switch(e.Key){
                case Key.Enter:
                case Key.Tab:
                    try{
                        this.textBlockType.Text = this.textBoxInput.Text.Split(' ')[0];
                        this.textBlockVariableName.Text = this.textBoxInput.Text.Split(' ')[1];
                    }catch(Exception){
                        MessageBox.Show("Invalid parameter assignature.","PlugSPL Error: Invalid parameter definition",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    goto case Key.Escape;
                case Key.Escape:
                    this.textBlockVariableName.Visibility = System.Windows.Visibility.Visible;
                    this.textBlockType.Visibility = System.Windows.Visibility.Visible;
                    this.textBoxInput.Visibility = System.Windows.Visibility.Collapsed;
                    break;
            }
        }

        internal EshuProperty GetEshuProperty() {
            EshuProperty eshuProperty = new EshuProperty(this.ParameterName, this.ParameterType);
            return eshuProperty;
        }

        
        public string ParameterName { get{return this.textBlockVariableName.Text;} }
        public string ParameterType { get{return this.textBlockType.Text;} }
    }
}
