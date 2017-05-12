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
using PlugSpl.DataStructs.ProductConfigurator;
using PlugSpl.DataStructs.ComponentPoolManager;

namespace ComponentPoolManager
{
    /// <summary>
    /// Interaction logic for InterfaceEditor.xaml
    /// </summary>
    public partial class InterfaceEditor : UserControl
    {
        public InterfaceEditor()
        {
            InitializeComponent();
        }

        private string caption;
        public string Caption
        {
            get { return caption; }
            set { caption = value; this.textBoxInterfaceName.Text = value; }
        }

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            this.stackPanelMethodEditor.Children.Add(new MethodEditor());
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.InterfaceObject.Name = this.textBoxInterfaceName.Text;
                EshuInterface eshuInterface = this.InterfaceObject.Eshu;
                eshuInterface.ClearMethods();

                foreach (MethodEditor methodEditor in this.stackPanelMethodEditor.Children.OfType<MethodEditor>())
                {
                    EshuMethod eshuMethod = methodEditor.GetEshuMethod();
                    eshuInterface.AddMethod(eshuMethod);
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "PlugSPL Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private DanuInterfaceObject interfaceObject;
        public DanuInterfaceObject InterfaceObject
        {
            get { return this.interfaceObject; }
            set
            {
                this.interfaceObject = value;
                this.Caption = this.interfaceObject.Name;
                foreach (EshuMethod m in this.interfaceObject.Eshu.Signature)
                {
                    MethodEditor me = new MethodEditor(m);
                    this.stackPanelMethodEditor.Children.Add(me);
                }
            }
        }
    }
}
