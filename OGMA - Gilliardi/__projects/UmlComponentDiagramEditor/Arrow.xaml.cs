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

namespace UmlComponentDiagramEditor
{
    /// <summary>
    /// Interaction logic for Arrow.xaml
    /// </summary>
    public partial class Arrow : UserControl
    {

        
        public Arrow()
        {
            InitializeComponent();
        }



        public Point GetPosition()
        {
            return new Point(this.Margin.Left, this.Margin.Top);
        }

        public void SetPosition(Point p)
        {
            this.Margin = new Thickness(p.X, p.Y, 0, 0);
        }

        public void Select()
        {
            //ArrowImage.Source = global::UmlComponentDiagramEditor.Arrow.
        }
        public void UnSelect()
        {
        }
    }
}
