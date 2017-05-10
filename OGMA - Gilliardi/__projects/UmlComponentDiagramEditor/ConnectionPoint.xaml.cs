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
    /// Interaction logic for ConnectionPoint.xaml
    /// </summary>
    public partial class ConnectionPoint : UserControl
    {


        public ConnectionPosition ConnectionPosition { get; set; }


        public SelectObject MyComponent;
        private SelectObject myComponent
        {
            get{ return MyComponent;}
            set { MyComponent = value; }
        }

        public ConnectionPoint()
        {
            InitializeComponent();
        }

        public void SetPosition(Point target)
        {
            this.Margin = new Thickness(target.X, target.Y, 0, 0);
        }

        public Point GetPosition()
        {
            Point position = this.MyComponent.GetPosition();
            switch (this.ConnectionPosition)
            {
                case ConnectionPosition.Up:
                    position.X += ((Control)this.MyComponent).ActualWidth / 2 - (this.Width/2);
                    break;
                case ConnectionPosition.Down:
                    position.X += ((Control)this.MyComponent).ActualWidth / 2 - (this.Width/2);
                    position.Y += ((Control)this.MyComponent).ActualHeight - (this.Height);
                    break;
                case ConnectionPosition.Left:
                    position.Y += ((Control)this.MyComponent).ActualHeight /2 - (this.Height/2);
                    break;
                case ConnectionPosition.Right:
                    position.X += ((Control)this.MyComponent).ActualWidth - (this.Width);
                    position.Y += ((Control)this.MyComponent).ActualHeight / 2 - (this.Height/2);
                    break;
                case ConnectionPosition.Center:
                    position.X += ((Control)this.MyComponent).ActualWidth / 2 - (this.Width / 2);
                    position.Y += ((Control)this.MyComponent).ActualHeight / 2 - (this.Height / 2);  
                    break;
            }
            return position;
            
        }

        private void connectionPoint_MouseDown(object sender, MouseButtonEventArgs e)
        {
     
            foreach (ConnectionPoint c in ((Grid)this.Parent).Children.OfType<ConnectionPoint>())
            {
                if (!c.Equals(this))
                {
                    c.connectionPoint.Stroke = new SolidColorBrush(Colors.Black);
                }
                else
                {
                    c.myComponent.setSelectedPoint(this);
                    c.connectionPoint.Stroke = new SolidColorBrush(Colors.Red);

                }
            }

            ComponentDiagram d = ((SelectObject)this.MyComponent).getParentDiagram();

            if (!d.IsLocking)
            {
                d.SourceConnectionPoint = this;
            }
            else
            {
                if (d.SourceConnectionPoint != null && d.isAdding == true)
                {
                    if (this.myComponent is Component && d.SourceConnectionPoint.myComponent is Component)
                    {
                        MessageBox.Show("You can not connect a component in another component");
                        d.IsLocking = false;
                        d.SourceConnectionPoint = null;
                        ComponentDiagram cd = this.MyComponent.getParentDiagram();
                        cd.invalidOperation = true;
                        return;
                    }
                    if (this.myComponent is Ball && d.SourceConnectionPoint.myComponent is Ball)
                    {
                        MessageBox.Show("You can not connect a interface in another interface");
                        d.IsLocking = false;
                        d.SourceConnectionPoint = null;
                        ComponentDiagram cd = this.MyComponent.getParentDiagram();
                        cd.invalidOperation = true;
                        return;
                    }

                    if (this.MyComponent is Component && (d.SourceConnectionPoint.myComponent as Ball).RelatedComponent != null)
                    {
                        MessageBox.Show("You can not connect two components in one interface or socket");
                        d.IsLocking = false;
                        d.SourceConnectionPoint = null;
                        ComponentDiagram cd = this.MyComponent.getParentDiagram();
                        cd.invalidOperation = true;
                        return;
                    }
                    if (this.myComponent is Ball)
                    {
                        if ((this.myComponent as Ball).RelatedComponent != null)
                        {
                            MessageBox.Show("You can not connect two components in one interface or socket");
                            d.IsLocking = false;
                            d.SourceConnectionPoint = null;
                            ComponentDiagram cd = this.MyComponent.getParentDiagram();
                            cd.invalidOperation = true;
                            return;
                        }
                    
                    }

                    if (this.MyComponent is Component && d.SourceConnectionPoint.MyComponent is Ball)
                    {
                        (this.myComponent as Component).interfaceList.Add(d.SourceConnectionPoint.myComponent as Ball);
                    }
                    
                    addAssociation(d);
                }
                else
                {
                    d.isRemoving = false;
                    d.SourceConnectionPoint = null;
                    d.isAdding = false;
                    d.invalidOperation = false;
                }
            }
        }

        public void addAssociation(ComponentDiagram d)
        {

            //d.Association.Add(new KeyValuePair<ConnectionPoint, ConnectionPoint>(d.SourceConnectionPoint, this));
            //this.myComponent.setRelatedComponent(d.SourceConnectionPoint.myComponent);
            //d.SourceConnectionPoint.myComponent.setRelatedComponent(this.myComponent);
            //d.IsLocking = false;
            //d.UpdateConnections();
            //d.SourceConnectionPoint = null;
            //d.isAdding = false;
        
        }

        public override string ToString()
        {
            return this.ConnectionPosition.ToString();
        }
    }
}
