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
using PlugSpl.Modules;
using System.Xml.Serialization;
using System.IO;
using Microsoft.Win32;
using PlugSpl.Atlas;

namespace UmlComponentDiagramEditor
{
    /// <summary>
    /// Interaction logic for ComponentDiagram.xaml
    /// </summary>
    public partial class ComponentDiagram : UserControl, IUmlComponentDiagramEditor
    {
        private Object selectedComponent = null;
        public Object SelectedComponent
        {
            get
            {
                return this.selectedComponent;
            }
            set
            {
                this.selectedComponent = value;
                (value as SelectObject).Select();
            }
        }

        private Object isLocked { get; set; }

        public List<KeyValuePair<Component, Ball>> Association { get; internal set; }
        public Component relationLockedForMutex { get; set; }
        public Component relationLockedForRequires { get; set; }
        public List<Relation> RelationList { get; set; }
        public List<Component> Components { get; set; }
        public List<Ball> Interfaces { get; set; }
        public List<Ball> Sockets { get; set; }
        public bool invalidOperation { get; set; }
        private bool componentClickAdd { get; set; }
        private bool ballClickAdd { get; set; }
        public bool reconnecting { get; set; }
        private bool socketClickAdd { get; set; }
        public bool mutexRelationClick { get; set; }
        public bool requiresRelationClick { get; set; }
        public bool associationAction { get; set; }
        private Component componentMouseMove { get; set; }
        private Ball ballMouseMove { get; set; }
        private Ball socketMouseMove { get; set; }
        public Line shadowRelationLine { get; set; }
        public Ball associationBall { get; set; }
        public bool relationSourceLocked { get; set; }
        public string sourceDirectory { get; set; }

        public ComponentDiagram()
        {
            InitializeComponent();
            componentMouseMove = new Component();
            ballMouseMove = new Ball();
            socketMouseMove = new Ball();
            shadowRelationLine = new Line();
            shadowRelationLine.IsHitTestVisible = false;
            this.Association = new List<KeyValuePair<Component, Ball>>();
            this.RelationList = new List<Relation>();
            this.Components = new List<Component>();
            this.Interfaces = new List<Ball>();
            this.Sockets = new List<Ball>();
            sourceDirectory = "";

        }

        public bool isAdding { get; set; }
        public bool isRemoving { get; set; }

        private void containerEditor_Drop(object sender, DragEventArgs e)
        {
            Object[] data = (Object[])e.Data.GetData(typeof(Object[]));
            SelectObject f = (SelectObject)(data[0]);
            Point orientation = (Point)data[1];

            if (f == null) return;

            Point dropPoint = e.GetPosition(f as Control);

            f.SetPosition(new Point()
            {
                X = f.GetPosition().X + dropPoint.X - orientation.X,
                Y = f.GetPosition().Y + dropPoint.Y - orientation.Y
            });

            if (f is Ball)
            {
                if ((f as Ball).isSocket)
                {
                    (f as Ball).rotateSocket(f as Ball);
                }
                else
                {
                    (f as Ball).setPositionNameEditor();
                    if ((f as Ball).Sockets.Count > 0)
                    {
                        foreach (Ball so in (f as Ball).Sockets)
                        {
                            (f as Ball).rotateSocket(so);
                        }
                    }
                }
            }
            else if (f is Component)
            {
                foreach (Ball b in ((Component)f).interfaceList)
                {
                    if (b.isSocket)
                    {
                        b.rotateSocket(b);
                    }
                }
            }
            if (f is Notation)
            {
                (f as Notation).UpdateLineCoordinate();

            }
            setLines(f);
        }

        public void setLines(SelectObject f)
        {
            if (f is Notation)
                return;

            if (f is Ball && ((Ball)f).Sockets.Count > 0)
            {
                Ball b = new Ball();
                b = (f as Ball);
                if (b.isInterface)
                {
                    foreach (Ball socket in (f as Ball).Sockets)
                    {
                        Point p = updateInterfaceCoordinates(b, socket.socketAngle);
                        socket.SetPosition(p);
                    }
                }
            }

            Line[] x = this.containerEditor.Children.OfType<Line>().ToArray<Line>();

            for (int i = 0; i < x.Length; i++)
            {
                if (((string)x[i].Tag) != "noteLine" && ((string)x[i].Tag) != "relation")
                    this.containerEditor.Children.Remove(x[i]);
            }

            UpdateConnections();

            if (f is Component)
            {
                foreach (Relation relation in RelationList)
                {
                    relation.updateLines();
                }
            }

            if (f is Ball && (f as Ball).isInterface)
            {
                (f as Ball).note.UpdateLineCoordinate();
            }

        }

        public Point updateInterfaceCoordinates(Ball b, Ball.SocketPosition sp)
        {
            switch (sp)
            {
                case Ball.SocketPosition.Up:
                    Point p = new Point(b.GetPosition().X - 1, b.GetPosition().Y + 12);
                    return p;
                case Ball.SocketPosition.Down:
                    Point p1 = new Point(b.GetPosition().X, b.GetPosition().Y - 12);
                    return p1;
                case Ball.SocketPosition.Left:
                    Point p2 = new Point(b.GetPosition().X + 12, b.GetPosition().Y);
                    return p2;
                case Ball.SocketPosition.Right:
                    Point p3 = new Point(b.GetPosition().X - 12, b.GetPosition().Y);
                    return p3;
            }
            return new Point();
        }

        private Point connectingPoint(Ball b, Ball.SocketPosition sp)
        {
            switch (sp)
            {
                case Ball.SocketPosition.Up:
                    Point p = new Point(b.GetPosition().X + 1, b.GetPosition().Y - 12);
                    return p;
                case Ball.SocketPosition.Down:
                    Point p1 = new Point(b.GetPosition().X, b.GetPosition().Y + 12);
                    return p1;
                case Ball.SocketPosition.Left:
                    Point p2 = new Point(b.GetPosition().X - 12, b.GetPosition().Y);
                    return p2;
                case Ball.SocketPosition.Right:
                    Point p3 = new Point(b.GetPosition().X + 12, b.GetPosition().Y);
                    return p3;
            }
            return new Point();
        }

        public void addComponent(Component c)
        {

            this.containerEditor.Children.Add(c);
            this.containerEditor.UpdateLayout();
            c.ParentDiagram = this;
            if (this.containerEditor.Children.Count > 2)
            {
                if (this.selectedComponent != null)
                {
                    (this.selectedComponent as SelectObject).Unselect();
                }
            }
            this.Components.Add(c);
            c.txtComponentName.Content = c.componentName;
            this.selectedComponent = c;
        }

        public void addInterface(Ball b)
        {
            b.ParentDiagram = this;
            b.addNote();
            this.Interfaces.Add(b);
            b.editionNameLabel.Content = b.InterfaceName;
            b.editionNameLabel.Visibility = Visibility.Visible;
            this.containerEditor.Children.Add(b.editionNameLabel);
            b.setPositionNameEditor();
            this.containerEditor.Children.Add(b);
            this.containerEditor.UpdateLayout();
            b.Select();
        }

        private void buttonAddComponent_Click(object sender, RoutedEventArgs e)
        {
            if (this.componentClickAdd == true)
            {
                this.containerEditor.Children.Remove(componentMouseMove);

            }
            if (this.ballClickAdd == true)
            {
                this.containerEditor.Children.Remove(ballMouseMove);
            }
            if (this.socketClickAdd == true)
            {
                this.containerEditor.Children.Remove(socketMouseMove);
            }

            this.actionBar.Foreground = new SolidColorBrush(Colors.Red);
            this.actionBar.Items[0] = "Click the position where you want to add the component.";
            this.componentClickAdd = true;
            this.ballClickAdd = false;
            this.mutexRelationClick = false;
            this.requiresRelationClick = false;
            this.relationLockedForRequires = null;
            this.relationLockedForMutex = null;
            this.relationSourceLocked = false;
            this.socketClickAdd = false;
            this.containerEditor.Children.Remove(shadowRelationLine);
            componentMouseMove.SetPosition(new Point(-100, -100));
            this.containerEditor.Children.Add(componentMouseMove);
            componentMouseMove.Opacity = 0.5;
        }

        private void buttonRemoveComponent_Click(object sender, RoutedEventArgs e)
        {
            this.componentClickAdd = false;
            this.ballClickAdd = false;
            this.mutexRelationClick = false;
            this.requiresRelationClick = false;
            this.relationLockedForRequires = null;
            this.relationLockedForMutex = null;
            this.relationSourceLocked = false;
            this.socketClickAdd = false;
            if (this.componentMouseMove != null)
            {
                this.containerEditor.Children.Remove(this.componentMouseMove);
            }
            if (this.ballMouseMove != null)
            {
                this.containerEditor.Children.Remove(this.ballMouseMove);
            }
            if (this.socketMouseMove != null)
            {
                this.containerEditor.Children.Remove(this.socketMouseMove);
            }

            if (selectedComponent != null)
            {
                if (selectedComponent is Component)
                {
                    if (MessageBox.Show("Do you want to remove selected component? Note that any interface or socket attached to it will be removed too.", "Do you really want remove?", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                    {
                        return;
                    }
                    removeComponent(selectedComponent as Component);
                }
                else if (selectedComponent is Ball)
                {
                    if ((selectedComponent as Ball).isInterface)
                    {
                        removeInterface(selectedComponent as Ball);
                    }
                    else
                    {
                        removeSocket(selectedComponent as Ball);
                    }
                }
                else if (selectedComponent is Relation)
                {
                    if (((Relation)selectedComponent).connectionType == Relation.ConnectionType.Requires)
                    {
                        removeRequires((Relation)selectedComponent);
                    }
                    else
                    {
                        removeMutex((Relation)selectedComponent);
                    }
                }

                if (containerEditor.Children.OfType<Component>().Count() > 0)
                {
                    if (this.SelectedComponent != null && this.SelectedComponent is SelectObject)
                        ((SelectObject)this.SelectedComponent).Unselect();

                    this.selectedComponent = containerEditor.Children.OfType<Component>().First();
                    (selectedComponent as SelectObject).Select();
                }

                Line[] x = this.containerEditor.Children.OfType<Line>().ToArray<Line>();

                for (int i = 0; i < x.Length; i++)
                {
                    if (((string)x[i].Tag) != "noteLine" && ((string)x[i].Tag) != "relation")
                        this.containerEditor.Children.Remove(x[i]);
                }

                UpdateConnections();
            }
        }

        public void removeComponent(Component component)
        {
            this.containerEditor.Children.Remove(component);

            List<KeyValuePair<Component, Ball>> associations = this.Association.Where(assoc => assoc.Key == component).ToList();

            foreach (KeyValuePair<Component, Ball> assoc in associations)
            {
                if (assoc.Value.isInterface)
                {
                    //desconnect interface
                    assoc.Value.setRelatedComponent(null);
                }
                else if (assoc.Value.isSocket)
                {
                    //remove socket
                    if (assoc.Value.RelatedBall != null)
                    {
                        Ball i = assoc.Value.RelatedBall;
                        i.Sockets.Remove(assoc.Value);
                        assoc.Value.RelatedBall = null;
                        i.Variants.Remove(component);
                        if (i.note != null)
                            i.note.UpdateVisualInformation();
                    }
                    this.Interfaces.Remove(assoc.Value);
                    this.containerEditor.Children.Remove(assoc.Value);
                }
                else
                {
                    throw new Exception("Unknow Ball type!!");
                }

                this.Association.Remove(assoc);
            }

            List<Relation> rels = this.RelationList.Where(r => r.Source == component || r.Target == component).ToList();

            foreach (Relation r in rels)
            {
                if (r.connectionType == Relation.ConnectionType.Requires)
                    //this.containerEditor.Children.Remove(r.RequiresArrow);

                this.containerEditor.Children.Remove(r.relationDescription);
                foreach (Line l in r.relationLines)
                    this.containerEditor.Children.Remove(l as UIElement);

                this.RelationList.Remove(r);
            }

            this.Components.Remove(component);
        }

        public void removeInterface(Ball ball)
        {
            this.containerEditor.Children.Remove(ball.note.relatedLine as UIElement);

            this.containerEditor.Children.Remove(ball.note as UIElement);

            this.containerEditor.Children.Remove(ball.editionNameLabel as UIElement);

            List<KeyValuePair<Component, Ball>> associations = this.Association.Where(assoc => assoc.Value == ball).ToList();

            foreach (KeyValuePair<Component, Ball> assoc in associations)
            {
                assoc.Key.interfaceList.Remove(ball);
                this.Association.Remove(assoc);
            }

            List<Ball> balls = (from assoc in this.Association
                                where assoc.Value.RelatedBall == ball
                                select assoc.Value).ToList();

            foreach (Ball b in balls)
            {
                if (b.RelatedBall != null)
                {
                    Ball i = b.RelatedBall;
                    i.Sockets.Remove(b);
                    b.RelatedBall = null;
                    i.Variants.Remove((Component)b.RelatedComponent);
                    if (i.note != null)
                        i.note.UpdateVisualInformation();
                }
            }

            this.containerEditor.Children.Remove(ball);
            this.Interfaces.Remove(ball);
        }

        public Control GetControl()
        {
            return this;
        }

        public void ExportStructure()
        {
            throw new NotImplementedException();
        }

        public void ImportStructure()
        {
            throw new NotImplementedException();
        }
        private void buttonAddInterface_Click(object sender, RoutedEventArgs e)
        {
            if (this.associationAction == true)
            {
                this.containerEditor.Children.Remove(this.shadowRelationLine);
                if (selectedComponent is Ball)
                {
                    if (((Ball)this.selectedComponent).note != null)
                    {
                        if (((Ball)this.selectedComponent).isInterface)
                        {
                            this.Interfaces.Remove((Ball)this.selectedComponent);
                        }
                        this.containerEditor.Children.Remove(((Ball)selectedComponent).editionNameLabel);
                        ((Ball)selectedComponent).deleteNote();
                    }
                }
                this.containerEditor.Children.Remove((UIElement)this.selectedComponent);
                this.associationAction = false;
                this.mutexRelationClick = false;
                this.actionBar.Foreground = new SolidColorBrush(Colors.Black);
                this.actionBar.Items[0] = "No Actions.";
            }
            if (this.ballClickAdd == true)
            {
                return;
            }
            if (this.socketClickAdd == true)
            {
                this.containerEditor.Children.Remove(socketMouseMove);
            }
            if (this.componentClickAdd == true)
            {
                this.containerEditor.Children.Remove(componentMouseMove);
            }
            this.actionBar.Foreground = new SolidColorBrush(Colors.Red);
            this.actionBar.Items[0] = "Click the position where you want to add the Interface.";
            this.ballClickAdd = true;
            this.componentClickAdd = false;
            this.relationLockedForRequires = null;
            this.relationLockedForMutex = null;
            this.relationSourceLocked = false;
            this.mutexRelationClick = false;
            this.requiresRelationClick = false;
            this.socketClickAdd = false;
            this.associationAction = false;
            this.containerEditor.Children.Remove(shadowRelationLine);
            this.ballMouseMove.SetPosition(new Point(-100, -100));
            this.containerEditor.Children.Add(ballMouseMove);
            this.ballMouseMove.Opacity = 0.5;

        }

        /// <summary>
        /// 
        /// </summary>
        private void buttonAddLink_Click(object sender, RoutedEventArgs e)
        {
            if (this.selectedComponent == null || ((SelectObject)this.selectedComponent).getSelectedPoint() == null)
            {
                return;
            }
            if (!this.IsLocking)

                this.IsLocking = true;
            this.isAdding = true;
        }

        public void UpdateConnections()
        {

            foreach (KeyValuePair<Component, Ball> asso in this.Association)
            {

                Line l = new Line();
                l.X1 = asso.Key.GetPosition().X + (asso.Key.Width / 2);
                l.Y1 = asso.Key.GetPosition().Y + (asso.Key.Height / 2);
                l.X2 = asso.Value.GetPosition().X + (asso.Value.Width / 2);
                l.Y2 = asso.Value.GetPosition().Y + (asso.Value.Height / 2);

                l.Stroke = new SolidColorBrush(Colors.Black);
                l.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                l.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;

                Grid.SetZIndex(l, -1);
                this.containerEditor.Children.Add(l);
            }

        }

        public ConnectionPoint SourceConnectionPoint { get; set; }

        public bool IsLocking { get; set; }

        private void buttonAddRelation_Click(object sender, RoutedEventArgs e)
        {
            if (!this.IsLocking)
                if (this.SourceConnectionPoint != null)
                {
                    this.IsLocking = true;
                }

            if (selectedComponent is Ball)
            {
                isLocked = this.selectedComponent;
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.associationAction == true && !this.reconnecting)
            {             
                if (sender is Grid)
                {
                    this.containerEditor.Children.Remove(this.shadowRelationLine);
                    if (selectedComponent is Ball)
                    {
                        if (((Ball)this.selectedComponent).note != null)
                        {
                            if (((Ball)this.selectedComponent).isInterface)
                            {
                                this.Interfaces.Remove((Ball)this.selectedComponent);
                            }
                            this.containerEditor.Children.Remove(((Ball)selectedComponent).editionNameLabel);
                            ((Ball)selectedComponent).deleteNote();
                        }
                    }
                    this.containerEditor.Children.Remove((UIElement)this.selectedComponent);
                    this.associationAction = false;
                    this.mutexRelationClick = false;
                    this.actionBar.Foreground = new SolidColorBrush(Colors.Black);
                    this.actionBar.Items[0] = "No Actions.";

                }
            }

            if (componentClickAdd == true)
            {
                componentClickAdd = false;
                Component c = new Component();
                Point p = new Point();
                Grid grid = sender as Grid;
                p.X = Mouse.GetPosition(grid).X;
                p.Y = Mouse.GetPosition(grid).Y;
                c.SetPosition(p);
                int counter = 0;
                //auto-generated names.

                while (this.GetComponent("Component" + counter) != null)
                {
                    counter++;
                }
                c.componentName = "Component" + counter;
                addComponent(c);
                if (this.selectedComponent != null && this.selectedComponent != c)
                {
                    (selectedComponent as SelectObject).Unselect();
                }
                c.Select();
                this.actionBar.Foreground = new SolidColorBrush(Colors.Black);
                this.actionBar.Items[0] = "No Actions.";
                this.containerEditor.Children.Remove(componentMouseMove);
                return;
            }
            if (ballClickAdd == true)
            {
                ballClickAdd = false;
                Ball b = new Ball();
                int counter = 0;
                Grid grid = sender as Grid;
                Point p = new Point();
                p.X = Mouse.GetPosition(grid).X;
                p.Y = Mouse.GetPosition(grid).Y;
                b.SetPosition(p);
                //auto-generated names.

                while (this.GetInterface("Interface" + counter) != null)
                {
                    counter++;
                }
                b.InterfaceName = "Interface" + counter;
                addInterface(b);
                if (this.selectedComponent != null && this.selectedComponent != b)
                {
                    (selectedComponent as SelectObject).Unselect();
                }
                this.selectedComponent = b;
                b.Select();
                this.shadowRelationLine.Stroke = new SolidColorBrush(Colors.Green);
                this.shadowRelationLine.StrokeDashCap = PenLineCap.Triangle;
                this.shadowRelationLine.StrokeDashArray = new DoubleCollection() { 1, 1 };
                this.shadowRelationLine.UpdateLayout();
                this.shadowRelationLine.X1 = b.GetPosition().X + b.Width / 2;
                this.shadowRelationLine.Y1 = b.GetPosition().Y + b.Height / 2;
                this.shadowRelationLine.X2 = b.GetPosition().X + b.Width / 2;
                this.shadowRelationLine.Y2 = b.GetPosition().Y + b.Height / 2;
                this.containerEditor.Children.Add(shadowRelationLine);
                this.associationAction = true;
                this.associationBall = b;
                this.actionBar.Foreground = new SolidColorBrush(Colors.Red);
                this.actionBar.Items[0] = "Select the component that will provide the interface.";
                this.containerEditor.Children.Remove(ballMouseMove);
                return;
            }
            if (socketClickAdd == true)
            {

                socketClickAdd = false;
                Ball b = new Ball();
                int counter = 0;
                Grid grid = sender as Grid;
                Point p = new Point();
                p.X = Mouse.GetPosition(grid).X;
                p.Y = Mouse.GetPosition(grid).Y;
                b.SetPosition(p);
                //auto-generated names.
                while (this.GetInterface("Socket" + counter) != null)
                {
                    counter++;
                }
                b.InterfaceName = "Socket" + counter;
                addSocket(b);
                if (this.selectedComponent != null && this.selectedComponent != b)
                {
                    (selectedComponent as SelectObject).Unselect();
                }
                this.selectedComponent = b;
                b.Select();
                this.shadowRelationLine.Stroke = new SolidColorBrush(Colors.Green);
                this.shadowRelationLine.StrokeDashCap = PenLineCap.Triangle;
                this.shadowRelationLine.StrokeDashArray = new DoubleCollection() { 1, 1 };
                this.shadowRelationLine.UpdateLayout();
                this.shadowRelationLine.X1 = b.GetPosition().X + b.Width / 2;
                this.shadowRelationLine.Y1 = b.GetPosition().Y + b.Height / 2;
                this.shadowRelationLine.X2 = b.GetPosition().X + b.Width / 2;
                this.shadowRelationLine.Y2 = b.GetPosition().Y + b.Height / 2;
                this.containerEditor.Children.Add(shadowRelationLine);
                this.associationAction = true;
                this.associationBall = b;
                this.actionBar.Foreground = new SolidColorBrush(Colors.Red);
                this.actionBar.Items[0] = "Select the component that will provide the interface.";
                this.containerEditor.Children.Remove(socketMouseMove);
                return;
            }


            if (isLocked != null)
            {
                if (this.selectedComponent is Ball)
                {
                    this.LoadInterfaceSocketConnection((isLocked as Ball), (selectedComponent as Ball));
                }
            }


        }

        private void addSocket(Ball b)
        {
            b.ParentDiagram = this;
            b.addNote();
            this.Interfaces.Add(b);
            b.editionNameLabel.Content = b.InterfaceName;
            b.editionNameLabel.Visibility = Visibility.Visible;
            this.containerEditor.Children.Add(b.editionNameLabel);
            b.setPositionNameEditor();
            this.containerEditor.Children.Add(b);
            b.ChangeSocket();
            this.containerEditor.UpdateLayout();
            b.Select();
        }

        private void LoadInterfaceSocketConnection(Ball isLockedBall, Ball selectBall)
        {
            if ((isLockedBall.RelatedComponent != null && selectBall.RelatedComponent != null) && isLockedBall.RelatedComponent == selectBall.RelatedComponent)
            {
                selectBall = null;
                this.isLocked = null;
                this.IsLocking = false;
                return;
            }
            if (isLockedBall.isInterface && selectBall.isInterface)
            {
                MessageBox.Show("You can not connect an interface to another interface.");
                isLocked = null;
                return;
            }
            Point p = connectingPoint(selectBall, selectBall.socketAngle);
            isLockedBall.SetPosition(p);
            selectBall.RelatedBall = isLockedBall;

            if (isLockedBall.isInterface)
            {
                isLockedBall.DesconectSocket.IsEnabled = true;
                isLockedBall.Sockets.Add(selectBall);
                selectBall.RelatedBall = isLockedBall;
                if (isLockedBall.RelatedComponent != null)
                {
                    (isLockedBall.RelatedComponent as Component).Childrens.Add((selectBall.RelatedComponent as Component));
                    isLockedBall.Variants.Add((selectBall.RelatedComponent as Component));
                }

                if (selectBall.RelatedComponent != null && isLockedBall.RelatedComponent != null)
                {
                    ((this.selectedComponent as Ball).RelatedComponent as Component).ComponentParent = (isLockedBall.RelatedComponent as Component);
                }

                Grid.SetZIndex(selectBall, -1);
                foreach (Ball so in (isLockedBall as Ball).Sockets)
                {
                    (isLockedBall as Ball).rotateSocket(so);
                }

                isLockedBall.note.UpdateLineCoordinate();
                isLockedBall.note.UpdateVisualInformation();
            }
            else if (isLockedBall.isSocket)
            {
                selectBall.DesconectSocket.IsEnabled = true;
                isLockedBall.RelatedBall = selectBall;
                selectBall.Sockets.Add(isLockedBall);
                if (isLockedBall.RelatedComponent != null && selectBall.RelatedComponent != null)
                {
                    (isLockedBall.RelatedComponent as Component).ComponentParent = (selectBall.RelatedComponent as Component);
                    (isLockedBall.RelatedComponent as Component).Childrens.Add((selectBall.RelatedComponent as Component));
                    selectBall.Variants.Add(isLockedBall.RelatedComponent as Component);
                    selectBall.note.UpdateVisualInformation();
                }

                Grid.SetZIndex(isLockedBall, -1);
                foreach (Ball so in (selectBall as Ball).Sockets)
                {
                    (selectBall as Ball).rotateSocket(so);
                }
            }

            if (isLockedBall.isSocket)
            {
                switch (isLockedBall.socketAngle)
                {

                    case Ball.SocketPosition.Up:
                        Point po = new Point(isLockedBall.GetPosition().X, isLockedBall.GetPosition().Y);
                        isLockedBall.SetPosition(po);
                        break;

                    case Ball.SocketPosition.Down:
                        Point pod = new Point(isLockedBall.GetPosition().X, isLockedBall.GetPosition().Y);
                        isLockedBall.SetPosition(pod);
                        break;
                    case Ball.SocketPosition.Right:
                        Point por = new Point(isLockedBall.GetPosition().X, isLockedBall.GetPosition().Y);
                        isLockedBall.SetPosition(por);
                        break;
                    case Ball.SocketPosition.Left:
                        Point pol = new Point(isLockedBall.GetPosition().X, isLockedBall.GetPosition().Y);
                        isLockedBall.SetPosition(pol);
                        break;
                }
            }

            isLocked = null;
            Line[] x = this.containerEditor.Children.OfType<Line>().ToArray<Line>();

            for (int i = 0; i < x.Length; i++)
            {
                if ((string)(x[i].Tag) != "noteLine" && (string)(x[i].Tag) != "relation")
                    this.containerEditor.Children.Remove(x[i]);
            }
            UpdateConnections();
        }

        private void addMutexRelation_Click(object sender, RoutedEventArgs e)
        {
            if (mutexRelationClick == true)
            {
                return;
            }
            else
            {
                if (selectedComponent != null)
                {
                    ((SelectObject)selectedComponent).Unselect();
                }
                this.componentClickAdd = false;
                this.relationLockedForRequires = null;
                this.relationLockedForMutex = null;
                this.relationSourceLocked = false;
                this.containerEditor.Children.Remove(shadowRelationLine);
                this.ballClickAdd = false;
                this.socketClickAdd = false;
                this.requiresRelationClick = false;
                this.mutexRelationClick = true;
                this.actionBar.Foreground = new SolidColorBrush(Colors.Red);
                this.actionBar.Items[0] = "Select mutex source component.";
            }
        }

        private void addRequiresRelation_Click(object sender, RoutedEventArgs e)
        {
            if (requiresRelationClick == true)
            {
                return;
            }
            else
            {
                if (selectedComponent != null)
                {
                    ((SelectObject)selectedComponent).Unselect();
                }
                this.componentClickAdd = false;
                this.ballClickAdd = false;
                this.socketClickAdd = false;
                this.relationLockedForRequires = null;
                this.relationLockedForMutex = null;
                this.relationSourceLocked = false;
                this.containerEditor.Children.Remove(shadowRelationLine);
                this.requiresRelationClick = true;
                this.mutexRelationClick = false;
                this.actionBar.Foreground = new SolidColorBrush(Colors.Red);
                this.actionBar.Items[0] = "Select requires source component.";
            }
        }

        private void removeRequires(Relation r)
        {

            int position = -1;
            foreach (Relation rel in this.RelationList)
            {
                if (r.Source == rel.Source && r.Target == rel.Target)
                {
                    foreach (Line l in rel.relationLines)
                    {
                        this.containerEditor.Children.Remove(l);
                    }
                    this.containerEditor.Children.Remove(rel.relationDescription);
                   // this.containerEditor.Children.Remove(rel.RequiresArrow);
                    position = this.RelationList.IndexOf(rel);
                }
            }
            this.RelationList.RemoveAt(position);

        }

        private void removeMutex(Relation r)
        {

            int position = -1;
            foreach (Relation rel in this.RelationList)
            {
                if (r.Source == rel.Source && r.Target == rel.Target)
                {
                    foreach (Line l in rel.relationLines)
                    {
                        this.containerEditor.Children.Remove(l);
                    }
                    this.containerEditor.Children.Remove(rel.relationDescription);
                    position = this.RelationList.IndexOf(rel);
                }
            }
            this.RelationList.RemoveAt(position);

        }

        private PlugSpl.DataStructs.UmlComponentDiagram.ComponentDiagramBragi saveStructure()
        {
           
                PlugSpl.DataStructs.UmlComponentDiagram.ComponentDiagramBragi cdb =
                    new PlugSpl.DataStructs.UmlComponentDiagram.ComponentDiagramBragi();
                PlugSpl.DataStructs.UmlComponentDiagram.Stereotype s =
                    new PlugSpl.DataStructs.UmlComponentDiagram.Stereotype("Mutex");
                cdb.AddStereotype(s);
                s = new PlugSpl.DataStructs.UmlComponentDiagram.Stereotype("Requires");
                cdb.AddStereotype(s);

                foreach (Component c in this.Components)
                {
                    PlugSpl.DataStructs.UmlComponentDiagram.Component com =
                        new PlugSpl.DataStructs.UmlComponentDiagram.Component(c.componentName);
                    cdb.AddComponent(com);
                }

                foreach (Ball b in this.Interfaces)
                {
                    if (b.isInterface)
                    {
                        Component c = new Component();


                        //interface, tem que estar conectada em alguem....
                        bool connected = false;

                        if (b.Variants.Count == 0)
                            throw new Exception("A interface must have at least one socket.");

                        foreach (KeyValuePair<Component, Ball> cc in this.Association)
                        {
                            if (cc.Value == b)
                            {

                                c = cc.Key;
                                PlugSpl.DataStructs.UmlComponentDiagram.InterfaceObject into =
                                new PlugSpl.DataStructs.UmlComponentDiagram.InterfaceObject(b.InterfaceName,
                                    (PlugSpl.DataStructs.UmlComponentDiagram.Component)cdb.GetUmlObject(c.componentName));
                                cdb.AddInterface(into);

                                PlugSpl.DataStructs.UmlComponentDiagram.SMarty smr = new PlugSpl.DataStructs.UmlComponentDiagram.SMarty(into.Name, PlugSpl.DataStructs.UmlComponentDiagram.SMartyBindingTimeTypes.LinkingTime, cdb.GetUmlObject(into.Name));
                                if (b.note != null)
                                {
                                    smr.MaxSelection = b.note.MaxSelection;
                                    smr.MinSelection = b.note.MinSelection;
                                }
                                cdb.AddAttachment(smr);
                                connected = true;
                            }
                        }
                        if (!connected)
                            throw new Exception("Interface " + b.InterfaceName + " must be connected to a component.");
                    }
                }

                foreach (Ball so in this.Interfaces)
                {
                    if (so.isSocket)
                    {
                        Component c = new Component();
                        foreach (KeyValuePair<Component, Ball> cc in this.Association)
                        {
                            if (cc.Value == so)
                            {
                                if (cc.Value.RelatedBall == null)
                                {
                                    throw new Exception("All sockets must be connected to an interface.");
                                    //Ball ball = new Ball();
                                    //cc.Value.RelatedBall = ball;                                    
                                }
                                c = cc.Key;
                                PlugSpl.DataStructs.UmlComponentDiagram.Socket sock =
                                new PlugSpl.DataStructs.UmlComponentDiagram.Socket((PlugSpl.DataStructs.UmlComponentDiagram.Component)cdb.GetUmlObject(c.componentName), (PlugSpl.DataStructs.UmlComponentDiagram.InterfaceObject)cdb.GetUmlObject(so.RelatedBall.InterfaceName), so.InterfaceName);
                                cdb.AddSocket(sock);
                            }
                        }
                    }

                }

                List<PlugSpl.DataStructs.UmlComponentDiagram.Stereotype> stereo = cdb.CopyStereotypes();

                foreach (Relation re in this.RelationList)
                {
                    foreach (PlugSpl.DataStructs.UmlComponentDiagram.Stereotype st in stereo)
                    {
                        if (st.Name.Equals(re.connectionType.ToString()))
                        {
                            s = st;
                        }
                    }
                    PlugSpl.DataStructs.UmlComponentDiagram.Association asso = new PlugSpl.DataStructs.UmlComponentDiagram.Association(cdb.GetUmlObject(re.Source.componentName), cdb.GetUmlObject(re.Target.componentName));
                    asso.Type = s;
                    cdb.AddAssociation(asso);
                }
                return cdb;
            

        }

        private void buttonSaveFileAs_Click(object sender, RoutedEventArgs e)
        {
            StreamWriter stre = null;
            try
            {
                SaveFileDialog save = new SaveFileDialog();
                save.DefaultExt = "plugcd";
                save.AddExtension = true;

                PlugSpl.DataStructs.UmlComponentDiagram.ComponentDiagramBragi cdb = saveStructure();

                if (cdb.RootName == "")
                {
                    return;
                }

                string rootName = cdb.RootName;
                save.FileName = rootName;

                if (save.ShowDialog() == false)
                {
                    return;
                }

                FileInfo f = new FileInfo(save.FileName);
                if (!rootName.Equals(f.Name.Substring(0, f.Name.LastIndexOf('.'))))
                {

                    throw new Exception("File name must be the same of the root feature, i.e., " + rootName);
                }

                saveLayoutFile(save.FileName);
                XmlSerializer serializer = new XmlSerializer(cdb.GetType());
                stre = new StreamWriter(save.FileName);
                serializer.Serialize(stre, cdb);
                stre.Close();
            }
            catch (Exception ex)
            {
                if (stre != null)
                {
                    stre.Close();
                }
                MessageBox.Show(ex.Message, "PlugSPL Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        public void SetApplicationWindow(PlugSpl.MainWindow mainWindow)
        {
            this.MainWindow = mainWindow;
        }

        public PlugSpl.MainWindow MainWindow { get; set; }

        private void buttonOpenFile_Click(object sender, RoutedEventArgs e)
        {
            StreamReader reader = null;
            try
            {
                OpenFileDialog open = new OpenFileDialog();
                open.Filter = "UML Component Diagram / Smarty (*.plugcd)|*.plugcd";
                if (open.ShowDialog() == false)
                {
                    return;
                }

                this.cleanDiagram();
                string fileToLoad = open.FileName;
                sourceDirectory = open.FileName;
                string ext = fileToLoad.Split('.').Last();

                if (ext != "plugcd")
                {
                    return;
                }

                reader = new StreamReader(open.FileName);

                XmlSerializer serializer = new XmlSerializer(typeof(PlugSpl.DataStructs.UmlComponentDiagram.ComponentDiagramBragi));

                PlugSpl.DataStructs.UmlComponentDiagram.ComponentDiagramBragi cdb = (PlugSpl.DataStructs.UmlComponentDiagram.ComponentDiagramBragi)serializer.Deserialize(reader);

                foreach (PlugSpl.DataStructs.UmlComponentDiagram.Component com in cdb.Components)
                {
                    loadVisualComponents(com);
                }

                foreach (PlugSpl.DataStructs.UmlComponentDiagram.InterfaceObject interf in cdb.Interfaces)
                {
                    loadVisualInterfaces(interf);
                }

                foreach (PlugSpl.DataStructs.UmlComponentDiagram.Socket sock in cdb.Sockets)
                {

                    loadVisualSockets(sock);
                }

                //foreach(PlugSpl.DataStructs.UmlComponentDiagram.Association
                loadLayout(open.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "PlugSPL Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }

        private void loadVisualSockets(PlugSpl.DataStructs.UmlComponentDiagram.Socket sock)
        {
            Ball visualSocket = new Ball();
            visualSocket.InterfaceName = sock.Name;
            visualSocket.ParentDiagram = this;
            addInterface(visualSocket);
            visualSocket.ChangeSocket();
        }

        private void loadVisualComponents(PlugSpl.DataStructs.UmlComponentDiagram.Component comp)
        {
            Component visualComponent = new Component();
            visualComponent.componentName = comp.Name;
            visualComponent.txtComponentName.Content = visualComponent.componentName;
            addComponent(visualComponent);

            if (this.selectedComponent != null && this.selectedComponent != visualComponent)
            {
                (selectedComponent as SelectObject).Unselect();
            }
            visualComponent.Select();
        }

        private void loadVisualInterfaces(PlugSpl.DataStructs.UmlComponentDiagram.InterfaceObject intr)
        {
            Ball VisualBall = new Ball();
            VisualBall.ParentDiagram = this;
            VisualBall.InterfaceName = intr.Name;
            VisualBall.editionNameLabel.Content = VisualBall.InterfaceName;
            addInterface(VisualBall);
            PlugSpl.DataStructs.UmlComponentDiagram.SMarty notation =
             (PlugSpl.DataStructs.UmlComponentDiagram.SMarty)intr.Attachments.First();
            VisualBall.note.MaxSelection = notation.MaxSelection;
            VisualBall.note.MinSelection = notation.MinSelection;
            int alterIndexMax = VisualBall.note.lblMaxSel.Content.ToString().IndexOf('=') + 2;
            int alterIndexMin = VisualBall.note.lblMinSel.Content.ToString().IndexOf('=') + 2;
            VisualBall.note.lblMaxSel.Content = VisualBall.note.lblMaxSel.Content.ToString().Replace(VisualBall.note.lblMaxSel.Content.ToString().Substring(alterIndexMax).Trim(), notation.MaxSelection.ToString());
            VisualBall.note.lblMinSel.Content = VisualBall.note.lblMinSel.Content.ToString().Replace(VisualBall.note.lblMinSel.Content.ToString().Substring(alterIndexMin).Trim(), notation.MinSelection.ToString());
            if (this.selectedComponent != null && this.selectedComponent != VisualBall)
            {
                (selectedComponent as SelectObject).Unselect();
            }
            this.selectedComponent = VisualBall;
            VisualBall.note.UpdateLineCoordinate();
            VisualBall.Select();
        }


        private void saveLayoutFile(string fileToSave)
        {

            //private void saveLayout(

            FileStream fileStream = File.Create(fileToSave + ".layout");
            TextWriter b = new StreamWriter(fileStream);
            foreach (Component c in this.Components)
            {
                string s = String.Format("{0}\t{1}~{2}", c.componentName, c.GetPosition().X, c.GetPosition().Y);
                if (s.Contains(','))
                {
                    s = s.Split(',')[0];
                }
                b.Write(s + "\n");
            }
            foreach (Ball ba in this.Interfaces)
            {
                if (ba.note != null)
                {
                    string s = String.Format("{0}\t{1}~{2}~{3}", ba.InterfaceName, ba.GetPosition().X, ba.GetPosition().Y, ba.note.GetPosition());
                    if (s.Contains(','))
                    {
                        //string interfacePostion = s.Split(',')[0];
                        //string notePostion = s.Split(',')[1].Split('~')[1].Split(',')[0];
                        //s = interfacePostion + "~" + notePostion;
                        s = s.Replace(',', ';');
                    }
                    b.Write(s + "\n");
                }
                else
                {
                    string s = String.Format("{0}\t{1}~{2}", ba.InterfaceName, ba.GetPosition().X, ba.GetPosition().Y);
                    if (s.Contains(','))
                    {
                        s = s.Split(',')[0];
                    }
                    b.Write(s + "\n");

                }
            }
            foreach (Ball so in this.Sockets)
            {
                string s = String.Format("{0}\t{1}~{2}", so.InterfaceName, so.GetPosition().X, so.GetPosition().Y);
                if (s.Contains(','))
                {
                    s = s.Split(',')[0];
                }
                b.Write(s + "\n");
            }
            foreach (KeyValuePair<Component, Ball> cc in this.Association)
            {
                string s = cc.Key.getName() + "-" + cc.Value.getName();

                b.Write(s + "\n");
            }
            foreach (Ball inte in this.Interfaces)
            {
                if (inte.Sockets.Count > 0)
                {
                    foreach (Ball so in inte.Sockets)
                    {
                        string s = inte.InterfaceName + ">" + so.InterfaceName;
                        b.Write(s + "\n");
                    }
                }
            }

            foreach (Relation r in this.RelationList)
            {
                string s = r.Source.componentName + "|" + r.connectionType + "|" + r.Target.componentName;
                b.Write(s + "\n");
            }
            b.Close();
            fileStream.Close();
        }

        private void loadLayout(string fileToLoad)
        {
            if (!File.Exists(fileToLoad + ".layout"))
            {
                MessageBox.Show("No layout file found. A new file will be generated in the next time you save this project.",
                    "PlugSPL warning: No layout file found.",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            fileToLoad += ".layout";

            try
            {
                using (TextReader reader = new StreamReader(fileToLoad))
                {
                    string[] lines = reader.ReadToEnd().Split('\n');
                    foreach (string line in lines)
                    {
                        Console.WriteLine(line);
                        if (line != "")
                        {
                            if (!line.Contains("-") && !line.Contains("|") && !line.Contains(">"))
                            {
                                string ElementName = line.Split('\t')[0];
                                string x = line.Split('\t')[1].Split('~')[0];
                                string y = line.Split('\t')[1].Split('~')[1].Split(new char[] { '.', ';' })[0];

                                Component c = this.GetComponent(ElementName);
                                Ball interf;

                                if (c == null)
                                {
                                    interf = this.GetInterface(ElementName);

                                    if (interf.isInterface)
                                    {
                                        Point position = new Point(Convert.ToInt32(x), Convert.ToInt32(y));
                                        interf.SetPosition(position);
                                        string notex = line.Split('\t')[1].Split('~')[2].Split(';')[0];
                                        string notey = line.Split('\t')[1].Split('~')[2].Split(';')[1].Trim();
                                        Point notePosition = new Point(Convert.ToInt32(notex), Convert.ToInt32(notey));
                                        interf.note.SetPosition(notePosition);
                                        interf.DrawNoteLine(interf.note);
                                        interf.note.UpdateLineCoordinate();
                                        interf.setPositionNameEditor();
                                    }
                                    else
                                    {
                                        Point position = new Point(Convert.ToInt32(x), Convert.ToInt32(y));
                                        interf.SetPosition(position);
                                    }
                                }
                                else
                                {
                                    Point position = new Point(Convert.ToInt32(x), Convert.ToInt32(y));
                                    c.SetPosition(position);
                                }

                            }
                            else if (line.Contains('-'))
                            {

                                this.loadConnections(line);

                            }
                            else if (line.Contains('>'))
                            {

                                this.LoadConnectionSocket(line);

                            }

                            else if (line.Contains('|'))
                            {
                                this.loadRelations(line);
                            }

                            else
                            {
                                return;
                            }

                        }
                        else
                        {
                            foreach (Component c in this.Components)
                            {
                                c.Unselect();
                            }
                            foreach (Ball b in this.Interfaces)
                            {
                                b.Unselect();
                            }
                            reader.Close();
                            return;
                        }
                    }

                    reader.Close();
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                MessageBox.Show("Layout file is corrupted. Save this project to generate a new brand layout file.",
                    "PlugSPL error: Layout file is corrupted.",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }
        }

        private void LoadConnectionSocket(string line)
        {
            string sourceName = line.Split('>')[0];
            string targetName = line.Split('>')[1];

            Ball b = this.GetInterface(sourceName.Trim());

            if (b != null)
            {
                Ball target = this.GetInterface(targetName.Trim());
                this.LoadInterfaceSocketConnection(b, target);
            }
            else
            {
                b = this.GetSocket(sourceName.Trim());
                if (b != null)
                {
                    Ball target = this.GetInterface(targetName.Trim());
                    this.LoadInterfaceSocketConnection(b, target);
                }
                else
                {
                    return;
                }
            }

        }

        private void loadConnections(string line)
        {

            string sourceName = line.Split('-')[0].Trim();
            Component c = this.GetComponent(sourceName);


            string targetName = line.Split('-')[1].Trim();

            Ball b = this.GetInterface(targetName);
            this.Association.Add(new KeyValuePair<Component, Ball>(c, b));
            c.interfaceList.Add(b);
            b.RelatedComponent = c;
            b.rotateSocket(b);
            this.UpdateConnections();

        }

        private void loadRelations(string line)
        {

            string source = line.Split('|')[0].Trim();
            string target = line.Split('|')[2].Trim();
            string relation = line.Split('|')[1].Trim();

            Component sourceComponent = this.GetComponent(source);
            Component targetComponent = this.GetComponent(target);

            Relation.ConnectionType ct = Relation.ConnectionType.Mutex;

            if (relation == "Requires")
            {
                ct = Relation.ConnectionType.Requires;
            }

            Relation rel = new Relation(ct, this, sourceComponent, targetComponent);
            this.RelationList.Add(rel);
        }

        public Component GetComponent(string componentName)
        {
            foreach (Component c in this.Components)
            {
                if (c.componentName == componentName)
                {
                    return c;
                }
            }

            return null;
        }

        public Ball GetSocket(string socketName)
        {
            foreach (Ball b in this.Sockets)
            {
                if (b.InterfaceName == socketName)
                {
                    return b;
                }
            }
            return null;
        }

        public Ball GetInterface(string interfaceName)
        {
            foreach (Ball b in this.Interfaces)
            {
                if (b.InterfaceName == interfaceName)
                {
                    return b;
                }
            }

            return null;
        }



        private ConnectionPoint ComponentConnectionPoint(Component c, string s)
        {

            ConnectionPosition con = ConnectionPosition.Center;

            switch (s)
            {
                case "Up":
                    con = ConnectionPosition.Up;
                    break;

                case "Down":
                    con = ConnectionPosition.Down;
                    break;
                case "Left":
                    con = ConnectionPosition.Left;
                    break;
                case "Right":
                    con = ConnectionPosition.Right;
                    break;
            }

            foreach (ConnectionPoint cp in c.connectionList)
            {
                if (cp.ConnectionPosition == con)
                {
                    return cp;
                }
            }

            return null;

        }

        private void buttonRemoveLink_Click(object sender, RoutedEventArgs e)
        {

            if (this.selectedComponent == null || ((SelectObject)this.selectedComponent).getSelectedPoint() == null)
            {
                return;
            }
            if (!this.IsLocking)
            {
                this.IsLocking = true;
                this.isRemoving = true;
            }
        }

        private void removeSocket(Ball b)
        {

            List<KeyValuePair<Component, Ball>> associations = this.Association.Where(assoc => assoc.Value == b).ToList();
            //List<Component> comp = (from assoc in this.Association
            //where assoc.Value == b
            //select assoc.Key).ToList();
            if (b.RelatedBall != null)
            {
                Ball i = b.RelatedBall;
                i.Sockets.Remove(b);
                b.RelatedBall = null;
                foreach (KeyValuePair<Component, Ball> c in associations)
                    i.Variants.Remove(c.Key);
                if (i.note != null)
                    i.note.UpdateVisualInformation();
            }
            this.Interfaces.Remove(b);
            this.containerEditor.Children.Remove(b);
            foreach (KeyValuePair<Component, Ball> c in associations)
                this.Association.Remove(c);
            /*
            this.containerEditor.Children.Remove(b as UIElement);

            for (int i = this.Association.Count() - 1; i >= 0; i--)
            {
                if (this.Association[i].Value == b)
                {
                    this.Association.Remove(this.Association[i]);
                }
            }
            //*/
        }

        private void buttonNewSolution_Click(object sender, RoutedEventArgs e)
        {

            if (MessageBox.Show("If you continue to lose all your unsaved progress. You really want to continue?", "Really remove that?!",
                  MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            {
                return;
            }

            cleanDiagram();
        }

        private void cleanDiagram()
        {
            UIElement[] ui = this.containerEditor.Children.OfType<UIElement>().ToArray<UIElement>();

            for (int i = 0; i < ui.Length; i++)
            {
                if (ui[i] is TextBlock)
                {
                    TextBlock tb = (ui[i] as TextBlock);
                    if (tb.Name == "tbGridText1" || tb.Name == "tbGridText2")
                    {
                        continue;
                    }
                }
                else
                {
                    this.containerEditor.Children.Remove(ui[i]);
                }
            }


            this.Components.Clear();
            this.Interfaces.Clear();
            this.Sockets.Clear();
            this.Association.Clear();
            this.RelationList.Clear();
            this.UpdateConnections();
        }

        private void containerEditor_MouseMove(object sender, MouseEventArgs e)
        {
            if (componentClickAdd == true)
            {
                Grid grid = sender as Grid;

                componentMouseMove.SetPosition(new Point(Mouse.GetPosition(grid).X, Mouse.GetPosition(grid).Y));
                this.containerEditor.UpdateLayout();
                componentMouseMove.UpdateLayout();
            }
            if (ballClickAdd == true)
            {
                Grid grid = sender as Grid;
                ballMouseMove.SetPosition(new Point(Mouse.GetPosition(grid).X, Mouse.GetPosition(grid).Y));
                this.containerEditor.UpdateLayout();
                ballMouseMove.UpdateLayout();

            }
            if (socketClickAdd == true)
            {
                Grid grid = sender as Grid;
                socketMouseMove.SetPosition(new Point(Mouse.GetPosition(grid).X, Mouse.GetPosition(grid).Y));
                this.containerEditor.UpdateLayout();
                socketMouseMove.UpdateLayout();
            }

            if (mutexRelationClick == true && this.relationSourceLocked == true)
            {
                Grid grid = sender as Grid;
                shadowRelationLine.X2 = Mouse.GetPosition(grid).X;
                shadowRelationLine.Y2 = Mouse.GetPosition(grid).Y;
                this.containerEditor.UpdateLayout();
                shadowRelationLine.UpdateLayout();
            }

            if (requiresRelationClick == true && this.relationSourceLocked == true)
            {
                Grid grid = sender as Grid;
                shadowRelationLine.X2 = Mouse.GetPosition(grid).X;
                shadowRelationLine.Y2 = Mouse.GetPosition(grid).Y;
                this.containerEditor.UpdateLayout();
                shadowRelationLine.UpdateLayout();
            }

            if (this.associationAction == true)
            {
                Grid grid = sender as Grid;
                shadowRelationLine.X2 = Mouse.GetPosition(grid).X;
                shadowRelationLine.Y2 = Mouse.GetPosition(grid).Y;
                this.containerEditor.UpdateLayout();
                shadowRelationLine.UpdateLayout();
            }

        }

        private void buttonAddSocket_Click(object sender, RoutedEventArgs e)
        {
            if (this.socketClickAdd == true)
            {
                this.containerEditor.Children.Remove(socketMouseMove);
            }
            if (this.ballClickAdd == true)
            {
                this.containerEditor.Children.Remove(ballMouseMove);
            }
            if (this.componentClickAdd == true)
            {
                this.containerEditor.Children.Remove(componentMouseMove);
            }
            this.actionBar.Foreground = new SolidColorBrush(Colors.Red);
            this.actionBar.Items[0] = "Click the position where you want to add the socket.";
            this.socketClickAdd = true;
            this.ballClickAdd = false;
            this.componentClickAdd = false;
            this.relationLockedForRequires = null;
            this.relationLockedForMutex = null;
            this.relationSourceLocked = false;
            this.requiresRelationClick = false;
            this.mutexRelationClick = false;
            this.containerEditor.Children.Remove(shadowRelationLine);
            socketMouseMove.SetPosition(new Point(-100, -100));
            this.containerEditor.Children.Add(socketMouseMove);
            socketMouseMove.ParentDiagram = this;
            socketMouseMove.ChangeSocket();
            socketMouseMove.Opacity = 0.5;

        }

        private void tbRenameSelected_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    if (this.GetComponent(tbRenameSelected.Text) != null)
                    {
                        MessageBox.Show("Unable to rename selected feature. A feature with given name already exists.", "PlugSPL - Feature renaming exception.", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (this.selectedComponent == null || String.IsNullOrEmpty(tbRenameSelected.Text))
                        return;
                    ((SelectObject)this.selectedComponent).setName(tbRenameSelected.Text);
                    if (this.selectedComponent is Component)
                    {
                        foreach (KeyValuePair<Component, Ball> cb in this.Association)
                        {
                            if (cb.Key == ((Component)this.selectedComponent))
                            {
                                if (cb.Value.isSocket)
                                {
                                    cb.Value.RelatedBall.note.UpdateVisualInformation();
                                }
                            }
                        }
                    }
                    break;

                case Key.Escape:
                    this.tbRenameSelected.Text = ((SelectObject)this.selectedComponent).getName();
                    break;
            }
        }

        private void containerEditor_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.mutexRelationClick = false;
            this.requiresRelationClick = false;
            this.relationLockedForRequires = null;
            this.relationLockedForMutex = null;
            this.containerEditor.Children.Remove(this.shadowRelationLine);
            this.associationAction = false;
            this.associationBall = null;

            if (this.componentClickAdd == true)
            {
                this.containerEditor.Children.Remove(this.componentMouseMove);
                this.componentClickAdd = false;
            }

            this.actionBar.Foreground = new SolidColorBrush(Colors.Black);
            this.actionBar.Items[0] = "No Actions.";
            return;
        }

        public bool validRelation(Component source, Component target)
        {
            foreach (Relation r in this.RelationList)
            {
                if (r.Source == source && r.Target == target || r.Source == target && r.Target == source)
                {
                    return false;
                }
            }

            return true;
        }

        private void buttonPrint_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog p = new PrintDialog();
            if (p.ShowDialog() == true)
                p.PrintVisual(this.containerEditor, "Generated byPlugSPL");
        }

        private void buttonLoadFeatureModel_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "UML Component Diagram / Smarty (*.plugfm)|*.plugfm";
            if (open.ShowDialog() == false)
            {
                return;
            }
            string ext = open.FileName.Split('.')[1];
            if (ext != "plugfm")
            {
                return;
            }

            StreamReader reader = new StreamReader(open.FileName);

            XmlSerializer serializer = new XmlSerializer(typeof(PlugSpl.Atlas.AtlasFeatureModel));

            AtlasFeatureModel atl = (PlugSpl.Atlas.AtlasFeatureModel)serializer.Deserialize(reader);

            foreach (AtlasFeature af in atl.Features)
            {
                if (af.IsAbstract)
                {
                    Ball b = new Ball();
                    b.setName("I" + af.Name);
                    this.addInterface(b);
                }
                else
                {

                    Component c = new Component();
                    c.setName(af.Name);
                    this.addComponent(c);
                }
            }

            foreach (AtlasConnection ac in atl.Connections)
            {
                if (ac.Child.IsAbstract)
                {
                    Component c = new Component();
                    c = this.GetComponent(ac.Parent.Name);
                    Ball b = new Ball();
                    b = this.GetInterface("I" + ac.Child.Name);

                    KeyValuePair<Component, Ball> cc = new KeyValuePair<Component, Ball>(c, b);
                    this.Association.Add(cc);
                }
            }

          
        }

        private void buttonSaveFile_Click(object sender, RoutedEventArgs e)
        {
            StreamWriter stre = null;
            if (this.sourceDirectory != "")
            {
                try
                {
                    if (MessageBox.Show("Do you really want overwrite the old save file from current diagram?", "Confirmation", MessageBoxButton.YesNoCancel) == MessageBoxResult.Yes)
                    {
                        PlugSpl.DataStructs.UmlComponentDiagram.ComponentDiagramBragi cdb = saveStructure();

                        if (cdb.RootName == "")
                        {
                            return;
                        }

                        string rootName = cdb.RootName;

                        FileInfo f = new FileInfo(sourceDirectory);
                        if (!rootName.Equals(f.Name.Substring(0, f.Name.LastIndexOf('.'))))
                        {

                            this.buttonSaveFileAs_Click(sender, e);
                        }
                        saveLayoutFile(this.sourceDirectory);
                        XmlSerializer serializer = new XmlSerializer(cdb.GetType());
                        stre = new StreamWriter(sourceDirectory);
                        serializer.Serialize(stre, cdb);
                        stre.Close();
                    }
                    else
                    {
                        return;
                    }
                }
                catch (Exception ex)
                {
                    if (stre != null)
                    {
                        stre.Close();
                    }
                    MessageBox.Show(ex.Message, "PlugSPL Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                this.buttonSaveFileAs_Click(sender, e);
            }
        
        }




    }
}
