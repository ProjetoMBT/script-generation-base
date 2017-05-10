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
    /// Interaction logic for ComponentControlInterface.xaml
    /// </summary>
    public partial class ComponentControlInterface : UserControl {
        public ComponentControlInterface() {
            InitializeComponent();
        }

        public DanuComponent ParentControl { get; set; }

        private DanuInterfaceObject interfaceObject;
        public DanuInterfaceObject Interface {
            get{return this.interfaceObject;}
            set{this.interfaceObject = value; this.InterfaceName = this.interfaceObject.Name;}
        }

        public String InterfaceName{
            get{return this.Interface.Name;}
            set{this.Interface.Name = value; this.textBlockInterfaceName.Text = value;}
        }
    }
}
