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
    /// Interaction logic for Interface.xaml
    /// </summary>
    public partial class Ball : UserControl, SelectObject
    {

        public ComponentDiagram ParentDiagram { get; set; }
        private ConnectionPoint selectedPoint { get; set; }
        public bool isInterface { get; set; }
        public bool isSocket { get; set; }
        public Ball RelatedBall { get; set; }
        public List<Ball> Sockets { get; set; }
        public List<Component> Variants { get; set; }
        public SelectObject RelatedComponent { get; set; }
        public ConnectionPoint connectionPoint { get; set; }
        public Label editionNameLabel { get; set; }
        private TextBox editionNameTextBox { get; set; }
        private string interfaceName { get; set; }
        public Notation note { get; set; }
        public int socketCount { get; set; }
        public Line linkComponent { get; set; }
        public string InterfaceName
        {
            get { return interfaceName; }
            set { interfaceName = value; }
        }

        

        public enum SocketPosition { Up, Down, Left, Right }

        private ConnectionPosition actualPosition;

        public ConnectionPosition ActualPosition
        {
            get
            {
                return actualPosition;
            }
            set
            {
                actualPosition = value;
            }

        }

        public SocketPosition socketAngle { get; set; }

        public Point Position
        {
            get { return GetPosition(); }
            set
            {
                this.Margin = new Thickness(value.X, value.Y, 0, 0);
            }
        }

        public Ball()
        {
            InitializeComponent();
            this.Variants = new List<Component>();
            isInterface = true;
            isSocket = false;
            InstantiateNameEditor();
            this.Sockets = new List<Ball>();

        }

        public void Unselect()
        {

            this.centerConnectionPoint.Visibility = Visibility.Hidden;
            //this.centerConnectionPoint.connectionPoint.Stroke = new SolidColorBrush(Colors.Black);
            this.recInterface.Stroke = new SolidColorBrush(Colors.Blue);
            this.selectedPoint = null;

        }

        public void Select()
        {
            if (this.IsSelected)
                return;
            this.centerConnectionPoint.Visibility = Visibility.Visible;
            this.recInterface.Stroke = new SolidColorBrush(Colors.Red);
            this.getParentDiagram().tbRenameSelected.Text = this.InterfaceName;
        }

        public Point GetPosition()
        {
            Point p = new Point();
            p.X = this.Margin.Left;
            p.Y = this.Margin.Top;

            return p;
        }

        public void SetPosition(Point target)
        {
            this.Margin = new Thickness(target.X, target.Y, 0, 0);
        }

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.isSocket == true && this.RelatedBall != null)
            {
                return;
            }

            if (this.ParentDiagram.invalidOperation == true)
            {
                this.ParentDiagram.invalidOperation = false;
                return;
            }

            Object[] data = new Object[] { this, Mouse.GetPosition(this) };
            DragDrop.DoDragDrop(this, data, DragDropEffects.Move);
            this.ParentDiagram.containerEditor.Children.Remove(selectedPoint);

            if (this.ParentDiagram.SelectedComponent != null && this.ParentDiagram.SelectedComponent != this)
            {
                ((SelectObject)this.ParentDiagram.SelectedComponent).Unselect();
            }
            if (!this.ParentDiagram.associationAction)
                this.ParentDiagram.SelectedComponent = this;

            if (this.isInterface == true && this.RelatedComponent == null)
            {
                this.ParentDiagram.relationLockedForRequires = null;
                this.ParentDiagram.relationLockedForMutex = null;
                this.ParentDiagram.relationSourceLocked = false;
                this.ParentDiagram.mutexRelationClick = false;
                this.ParentDiagram.requiresRelationClick = false;
                this.ParentDiagram.associationAction = true;
                this.ParentDiagram.reconnecting = true;
                this.ParentDiagram.containerEditor.Children.Remove(this.ParentDiagram.shadowRelationLine);
                this.ParentDiagram.shadowRelationLine.Stroke = new SolidColorBrush(Colors.Green);
                this.ParentDiagram.shadowRelationLine.StrokeDashCap = PenLineCap.Triangle;
                this.ParentDiagram.shadowRelationLine.StrokeDashArray = new DoubleCollection() { 1, 1 };
                this.ParentDiagram.shadowRelationLine.UpdateLayout();
                this.ParentDiagram.shadowRelationLine.X1 = this.GetPosition().X + this.Width / 2;
                this.ParentDiagram.shadowRelationLine.Y1 = this.GetPosition().Y + this.Height / 2;
                this.ParentDiagram.shadowRelationLine.X2 = this.GetPosition().X + this.Width / 2;
                this.ParentDiagram.shadowRelationLine.Y2 = this.GetPosition().Y + this.Height / 2;
                this.ParentDiagram.containerEditor.Children.Add(this.ParentDiagram.shadowRelationLine);
                this.ParentDiagram.associationBall = this;
                this.ParentDiagram.actionBar.Foreground = new SolidColorBrush(Colors.Red);
                this.ParentDiagram.actionBar.Items[0] = "Select the component that will provide the interface.";
            }
        }

        private void ChangeToSocket_Click(object sender, RoutedEventArgs e)
        {

            ChangeSocket();
        }

        public void ChangeSocket()
        {
            if (this.RelatedBall == null)
            {
                this.recInterface.Visibility = Visibility.Hidden;
                this.Socket.Visibility = Visibility.Visible;
                isInterface = false;
                isSocket = true;
                this.deleteNote();
                this.editionNameLabel.Visibility = Visibility.Hidden;

                rotateSocket(this);
            }
            else
            {
                MessageBox.Show("You can not switch to an interface with Socket connection. Disconnect the socket interface!");
                return;
            }
        }

        public void deleteNote()
        {
            if (this.note != null)
            {
                this.ParentDiagram.containerEditor.Children.Remove(this.note.relatedLine);
                this.ParentDiagram.containerEditor.Children.Remove(this.note);
                this.note = null;
            }
        }

        public void deleteInterfaceVisualElements()
        {
            this.ParentDiagram.containerEditor.Children.Remove(this.note.relatedLine);
            this.ParentDiagram.containerEditor.Children.Remove(this.note);
            this.ParentDiagram.containerEditor.Children.Remove(this.editionNameLabel);
            this.ParentDiagram.containerEditor.Children.Remove(this);
        }

        public void rotateSocket(Ball b)
        {

            switch (b.verifyPosition())
            {
                case SocketPosition.Up:
                    RotateTransform rt = new RotateTransform(270, 2, 13);
                    this.Socket.RenderTransform = rt;
                    this.socketAngle = SocketPosition.Up;
                    break;
                case SocketPosition.Down:
                    RotateTransform rtDown = new RotateTransform(90, 2, 13);
                    this.Socket.RenderTransform = rtDown;
                    this.socketAngle = SocketPosition.Down;
                    break;
                case SocketPosition.Left:
                    RotateTransform rtLeft = new RotateTransform(180, 2, 13);
                    this.Socket.RenderTransform = rtLeft;
                    this.socketAngle = SocketPosition.Left;
                    break;
                case SocketPosition.Right:
                    RotateTransform rtRight = new RotateTransform(0);
                    this.Socket.RenderTransform = rtRight;
                    this.socketAngle = SocketPosition.Right;
                    return;
            }
        }

        private void InstantiateNameEditor()
        {
            editionNameLabel = new Label();
            editionNameTextBox = new TextBox();
            editionNameLabel.VerticalAlignment = VerticalAlignment.Top;
            editionNameTextBox.VerticalAlignment = VerticalAlignment.Top;
            editionNameLabel.HorizontalAlignment = HorizontalAlignment.Left;
            editionNameTextBox.HorizontalAlignment = HorizontalAlignment.Left;
            editionNameTextBox.Width = 80;
            editionNameTextBox.Height = 25;

        }

        public SocketPosition verifyPosition()
        {

            if (RelatedComponent != null)
            {
                if (this.GetPosition().X < RelatedComponent.GetPosition().X)
                {
                    double diferenceX = RelatedComponent.GetPosition().X - this.GetPosition().X;
                    double diferenceY;
                    if (this.GetPosition().Y < RelatedComponent.GetPosition().Y)
                    {
                        diferenceY = RelatedComponent.GetPosition().Y - this.GetPosition().Y;
                    }
                    else
                    {
                        diferenceY = this.GetPosition().Y - RelatedComponent.GetPosition().Y;
                    }

                    if (diferenceY < diferenceX)
                    {
                        return SocketPosition.Left;
                    }
                    else
                    {
                        if (this.GetPosition().Y < RelatedComponent.GetPosition().Y)
                        {
                            return SocketPosition.Up;
                        }
                        else
                        {
                            return SocketPosition.Down;
                        }
                    }
                }

                else if (this.GetPosition().X > RelatedComponent.GetPosition().X)
                {
                    double diferenceX = this.GetPosition().X - this.RelatedComponent.GetPosition().X;
                    double diferenceY;
                    if (this.GetPosition().Y < RelatedComponent.GetPosition().Y)
                    {
                        diferenceY = RelatedComponent.GetPosition().Y - this.GetPosition().Y;
                    }
                    else
                    {
                        diferenceY = this.GetPosition().Y - RelatedComponent.GetPosition().Y;
                    }

                    if (diferenceY < diferenceX)
                    {
                        return SocketPosition.Right;
                    }
                    else
                    {
                        if (this.GetPosition().Y < RelatedComponent.GetPosition().Y)
                        {
                            return SocketPosition.Up;
                        }
                        else
                        {
                            return SocketPosition.Down;
                        }
                    }
                }


                else
                {
                    if (RelatedComponent.GetPosition().X < this.GetPosition().X)
                    {
                        double diferenceX = this.GetPosition().X - RelatedComponent.GetPosition().X;
                        double diferenceY;
                        if (RelatedComponent.GetPosition().Y < this.GetPosition().Y)
                        {
                            diferenceY = this.GetPosition().Y - RelatedComponent.GetPosition().Y;
                        }
                        else
                        {
                            diferenceY = RelatedComponent.GetPosition().Y - this.GetPosition().Y;
                        }

                        if (diferenceY < diferenceX)
                        {
                            return SocketPosition.Left;
                        }
                        else
                        {
                            if (RelatedComponent.GetPosition().Y < this.GetPosition().Y)
                            {
                                return SocketPosition.Up;
                            }
                            else
                            {
                                return SocketPosition.Down;
                            }
                        }
                    }
                }

                if (this.isSocket)
                {
                    Point p = new Point(this.GetPosition().X - this.Width / 2, this.GetPosition().Y);
                    this.SetPosition(p);
                }
            }
            return SocketPosition.Right;

        }

        public void ChangeBall()
        {
            if (RelatedBall != null)
            {
                MessageBox.Show("You can not change the state of a socket connected to an interface!");
                return;
            }
            this.recInterface.Visibility = Visibility.Visible;
            this.Socket.Visibility = Visibility.Hidden;
            isInterface = true;
            this.addNote();
            isSocket = false;
            int counter = 0;
            while (this.ParentDiagram.GetInterface("Interface" + counter) != null)
            {
                counter++;
            }
            this.interfaceName = "Interface" + counter;
            this.editionNameLabel.Visibility = Visibility.Visible;
            setPositionNameEditor();
            this.editionNameLabel.Content = interfaceName;

        }

        private void ChangeToBall_Click(object sender, RoutedEventArgs e)
        {
            ChangeBall();
        }

        public ConnectionPoint getSelectedPoint()
        {
            return this.selectedPoint;
        }

        public void setSelectedPoint(ConnectionPoint newValue)
        {
            return;
        }

        public ComponentDiagram getParentDiagram()
        {
            return this.ParentDiagram;
        }

        public void setRelatedComponent(SelectObject so)
        {
            this.RelatedComponent = so;
        }

        public bool IsSelected { get; set; }

        private void DesconectSocket_Click(object sender, RoutedEventArgs e)
        {

            MessageBoxResult mbr = MessageBox.Show("Do you really want to disconnect the socket interface?", "Attention!", MessageBoxButton.YesNoCancel);

            if (mbr == MessageBoxResult.Yes)
            {
                Point p = new Point(this.GetPosition().X, this.GetPosition().Y + 20);
                this.SetPosition(p);
                if (this.isInterface)
                {
                    if (this.Sockets.Count > 0)
                    {
                        this.Sockets[0].RelatedBall = null;
                        if (this.Sockets[0].RelatedComponent != null && this.RelatedComponent != null)
                        {
                            (this.RelatedComponent as Component).Childrens.Remove(this.Sockets[0].RelatedComponent as Component);
                            this.Variants.Remove(this.Sockets[0].RelatedComponent as Component);
                            this.note.UpdateVisualInformation();
                        }

                        this.Sockets.Remove(Sockets[0]);
                    }
                    if (this.Sockets.Count == 0)
                    {
                        this.DesconectSocket.IsEnabled = false;
                    }
                }
                this.ParentDiagram.UpdateConnections();
                this.ParentDiagram.setLines(this);
                this.setPositionNameEditor();
            }
            else
            {
                return;
            }
        }

        private void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            if (!this.ParentDiagram.containerEditor.Children.Contains(editionNameTextBox))
            {
                if (this.isInterface)
                {

                    setPositionNameEditor();
                    this.ParentDiagram.containerEditor.Children.Add(editionNameTextBox);
                    editionNameTextBox.KeyDown += new KeyEventHandler(renameBox_KeyDown);
                }
            }

        }

        private void renameBox_KeyDown(object o, KeyEventArgs args)
        {

            this.editionNameLabel.Content = ((TextBox)o).Text;
            editionNameLabel.VerticalAlignment = VerticalAlignment.Top;
            editionNameLabel.HorizontalAlignment = HorizontalAlignment.Left;


            if (args.Key == Key.Enter)
            {

                if (editionNameLabel.Parent == null)
                {
                    this.ParentDiagram.containerEditor.Children.Add(editionNameLabel);
                }
                ((Grid)editionNameLabel.Parent).Children.Remove((TextBox)o);
                editionNameLabel.Visibility = Visibility.Visible;
                editionNameLabel.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
                this.interfaceName = editionNameLabel.Content.ToString();
                this.ParentDiagram.tbRenameSelected.Text = editionNameLabel.Content.ToString();
            }

            else if (args.Key == Key.Escape)
            {
                ((Grid)editionNameLabel.Parent).Children.Remove((TextBox)o);
                editionNameLabel.Visibility = Visibility.Visible;
            }
        }


        public void setPositionNameEditor()
        {
            this.editionNameLabel.Margin = new Thickness(this.GetPosition().X, this.GetPosition().Y - 20, 0, 0);
            this.editionNameTextBox.Margin = new Thickness(this.GetPosition().X, this.GetPosition().Y - 20, 0, 0);
        }

        internal void addNote()
        {
            Notation n = new Notation(this);
            n.ParentDiagram = this.ParentDiagram;
            n.SetPosition(new Point(this.GetPosition().X + 30, this.GetPosition().Y + 30));
            DrawNoteLine(n);
            this.note = n;
            this.ParentDiagram.containerEditor.Children.Add(n);
        }

        internal void DrawNoteLine(Notation n)
        {
            if (n.relatedLine != null)
            {
                this.ParentDiagram.containerEditor.Children.Remove(n.relatedLine);
            }
            n.relatedLine = new Line();
            n.relatedLine.HorizontalAlignment = HorizontalAlignment.Left;
            n.relatedLine.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            n.relatedLine.X1 = this.GetPosition().X + this.Width / 2;
            n.relatedLine.Y1 = this.GetPosition().Y + this.Height / 2;
            n.relatedLine.X2 = n.GetPosition().X + n.ActualWidth / 2;
            n.relatedLine.Y2 = n.GetPosition().Y + n.ActualHeight / 2;
            n.relatedLine.Stroke = new SolidColorBrush(Colors.Black);
            n.relatedLine.StrokeDashCap = PenLineCap.Triangle;
            n.relatedLine.StrokeDashArray = new DoubleCollection() { 6, 6 };
            n.relatedLine.Tag = "is notation.";
            this.ParentDiagram.containerEditor.Children.Add(n.relatedLine);
            Grid.SetZIndex(n.relatedLine, -1);
        }

        public string getName()
        {
            return this.InterfaceName;
        }

        public void setName(string name)
        {
            this.interfaceName = name;
            this.editionNameLabel.Content = name;
        }

        public List<string> getVariantsName()
        {
            List<string> names = new List<string>();
            foreach (Component c in this.Variants)
            {
                names.Add(c.getName());
            }

            return names;
        }
    }
}
