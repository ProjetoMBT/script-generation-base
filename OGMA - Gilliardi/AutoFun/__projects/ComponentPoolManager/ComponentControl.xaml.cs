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
namespace ComponentPoolManager {
    /// <summary>
    /// Interaction logic for ComponentControl.xaml
    /// </summary>
    public partial class ComponentControl : UserControl {
        public ComponentControl() {
            InitializeComponent();
        }

        public PoolManager ParentManager { get; set; }
        public DanuComponent SourceComponent { get; set; }
        
        internal void UpdateInfo(DanuComponent d) {
            this.SourceComponent = d;
            this.textBlockName.Text = d.Name;
            foreach(DanuInterfaceObject i in d.Interfaces){
                ComponentControlInterface newInterface = new ComponentControlInterface();
                newInterface.ParentControl = d;
                newInterface.Interface = i;
                newInterface.MouseDoubleClick += new MouseButtonEventHandler(newInterface_MouseDoubleClick);
                this.stackPanelInterfaces.Children.Add(newInterface);
            }
        }

        
        void newInterface_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            InterfaceEditor editor = new InterfaceEditor();
            editor.InterfaceObject = (sender as ComponentControlInterface).Interface;
            this.ParentManager.LoadInterfaceEditor(editor);
        }
    }
}
