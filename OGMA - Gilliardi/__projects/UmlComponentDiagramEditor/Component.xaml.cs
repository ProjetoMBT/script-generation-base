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
using System.Windows.Ink;

namespace UmlComponentDiagramEditor
{
    /// <summary>
    /// Interaction logic for Component.xaml
    /// </summary>
    public partial class Component : UserControl, SelectObject
    {

        public ComponentDiagram ParentDiagram { get; set; }

        public string componentName { get; set; }
        public List<Ball> interfaceList { get; set; }
        private bool isLock { get; set; }
        private ConnectionPoint SelectedPoint;
        public List<ConnectionPoint> connectionList { get; set; }
        public SelectObject RelatedComponent { get; set; }
        public Component ComponentParent { get; set; }
        public List<Component> Childrens { get; set; }

        public Component()
        {

            InitializeComponent();
            interfaceList = new List<Ball>();
            connectionList = new List<ConnectionPoint>();
            //connectionParent();
            Childrens = new List<Component>();
        }

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.ParentDiagram.SelectedComponent != null && this.ParentDiagram.SelectedComponent != this)
            {
                ((SelectObject)this.ParentDiagram.SelectedComponent).Unselect();
            }
            this.ParentDiagram.SelectedComponent = this;
            if (this.ParentDiagram.mutexRelationClick == true){
                if(this.ParentDiagram.relationSourceLocked == false)
                {
                    this.ParentDiagram.shadowRelationLine.Stroke = new SolidColorBrush(Colors.Orange);
                    this.ParentDiagram.shadowRelationLine.StrokeDashCap = PenLineCap.Triangle;
                    this.ParentDiagram.shadowRelationLine.StrokeDashArray = new DoubleCollection() { 6, 6 };
                    this.ParentDiagram.shadowRelationLine.UpdateLayout();
                    this.ParentDiagram.shadowRelationLine.Opacity = 1;
                    this.ParentDiagram.shadowRelationLine.X1 = this.GetPosition().X + this.Width / 2;
                    this.ParentDiagram.shadowRelationLine.Y1 = this.GetPosition().Y + this.Height / 2;
                    this.ParentDiagram.containerEditor.Children.Add(this.ParentDiagram.shadowRelationLine);
                    this.ParentDiagram.relationLockedForMutex = this;
                    this.ParentDiagram.relationSourceLocked = true;
                    this.ParentDiagram.actionBar.Foreground = new SolidColorBrush(Colors.Red);
                    this.ParentDiagram.actionBar.Items[0] = "Select mutex target component.";
                }
                else
                {
                    if (this.ParentDiagram.relationLockedForMutex == this)
                    {
                        MessageBox.Show("You can not connect a component with itself");
                        this.ParentDiagram.relationSourceLocked = false;
                        this.ParentDiagram.mutexRelationClick = false;
                        this.ParentDiagram.relationLockedForRequires = null;
                        this.ParentDiagram.actionBar.Foreground = new SolidColorBrush(Colors.Black);
                        this.ParentDiagram.actionBar.Items[0] = "No Actions.";
                        return;
                    }
                    if (this.ParentDiagram.validRelation(this.ParentDiagram.relationLockedForMutex, this) == false)
                    {
                        MessageBox.Show("Related components are already connected");
                        this.ParentDiagram.relationSourceLocked = false;
                        this.ParentDiagram.mutexRelationClick = false;
                        this.ParentDiagram.relationLockedForMutex = null;
                        this.ParentDiagram.actionBar.Foreground = new SolidColorBrush(Colors.Black);
                        this.ParentDiagram.actionBar.Items[0] = "No Actions.";
                    }
                    else
                    {
                        Relation mutexRelation = new Relation(Relation.ConnectionType.Mutex, this.ParentDiagram, this.ParentDiagram.relationLockedForMutex, this);
                        this.ParentDiagram.RelationList.Add(mutexRelation);
                        this.ParentDiagram.relationSourceLocked = false;
                        this.ParentDiagram.mutexRelationClick = false;
                        this.ParentDiagram.relationLockedForMutex = null;
                        this.ParentDiagram.actionBar.Foreground = new SolidColorBrush(Colors.Black);
                        this.ParentDiagram.actionBar.Items[0] = "No Actions.";
                    }
                }
            }

            else if (this.ParentDiagram.requiresRelationClick == true)
            {
                if (this.ParentDiagram.relationSourceLocked == false)
                {
                    this.ParentDiagram.shadowRelationLine.Stroke = new SolidColorBrush(Colors.Blue);
                    this.ParentDiagram.shadowRelationLine.StrokeDashCap = PenLineCap.Triangle;
                    this.ParentDiagram.shadowRelationLine.StrokeDashArray = new DoubleCollection() { 6, 6 };
                    this.ParentDiagram.shadowRelationLine.UpdateLayout();
                    this.ParentDiagram.shadowRelationLine.Opacity = 1;
                    this.ParentDiagram.shadowRelationLine.X1 = this.GetPosition().X + this.Width / 2;
                    this.ParentDiagram.shadowRelationLine.Y1 = this.GetPosition().Y + this.Height / 2;
                    this.ParentDiagram.containerEditor.Children.Add(this.ParentDiagram.shadowRelationLine);
                    this.ParentDiagram.relationLockedForRequires = this;
                    this.ParentDiagram.relationSourceLocked = true;
                    this.ParentDiagram.actionBar.Foreground = new SolidColorBrush(Colors.Red);
                    this.ParentDiagram.actionBar.Items[0] = "Select requires target component.";
                }
                else
                {
                    if (this.ParentDiagram.relationLockedForRequires == this)
                    {
                        MessageBox.Show("You can not connect a component with itself");
                        this.ParentDiagram.relationSourceLocked = false;
                        this.ParentDiagram.requiresRelationClick = false;
                        this.ParentDiagram.relationLockedForRequires = null;
                        this.ParentDiagram.actionBar.Foreground = new SolidColorBrush(Colors.Black);
                        this.ParentDiagram.actionBar.Items[0] = "No Actions.";
                        return;
                    }
                    if (this.ParentDiagram.validRelation(this.ParentDiagram.relationLockedForRequires, this) == false)
                    {
                        MessageBox.Show("Related components are already connected");
                        this.ParentDiagram.relationSourceLocked = false;
                        this.ParentDiagram.requiresRelationClick = false;
                        this.ParentDiagram.relationLockedForRequires = null;
                        this.ParentDiagram.actionBar.Foreground = new SolidColorBrush(Colors.Black);
                        this.ParentDiagram.actionBar.Items[0] = "No Actions.";
                    }
                    else
                    {
                        Relation requiresRelation = new Relation(Relation.ConnectionType.Requires, this.ParentDiagram, this.ParentDiagram.relationLockedForRequires, this);
                        this.ParentDiagram.RelationList.Add(requiresRelation);
                        this.ParentDiagram.relationSourceLocked = false;
                        this.ParentDiagram.requiresRelationClick = false;
                        this.ParentDiagram.relationLockedForRequires = null;
                        this.ParentDiagram.actionBar.Foreground = new SolidColorBrush(Colors.Black);
                        this.ParentDiagram.actionBar.Items[0] = "No Actions.";
                    }
                }
            }
            else if (this.ParentDiagram.associationAction == true)
            {
                this.ParentDiagram.Association.Add(new KeyValuePair<Component, Ball>(this, this.ParentDiagram.associationBall));
                this.ParentDiagram.UpdateConnections();
                this.interfaceList.Add(this.ParentDiagram.associationBall);
                if (this.ParentDiagram.associationBall.isInterface)
                {
                    this.ParentDiagram.associationBall.note.UpdateLineCoordinate();
                    
                }
                this.ParentDiagram.setLines(this);
                this.ParentDiagram.associationBall.RelatedComponent = this;
                this.ParentDiagram.associationBall.rotateSocket(this.ParentDiagram.associationBall);
                this.ParentDiagram.associationBall = null;
                this.ParentDiagram.associationAction = false;
                this.ParentDiagram.reconnecting = false;
                this.ParentDiagram.containerEditor.Children.Remove(this.ParentDiagram.shadowRelationLine);
                this.ParentDiagram.actionBar.Foreground = new SolidColorBrush(Colors.Black);
                this.ParentDiagram.actionBar.Items[0] = "No Actions.";
            }
            
         }
        public void Unselect()
        {
            foreach (ConnectionPoint c in this.connectionList)
            {
                c.connectionPoint.Visibility = Visibility.Hidden;
                c.connectionPoint.Stroke = new SolidColorBrush(Colors.Black);
            }
            this.SelectedPoint = null;
            this.recComponent.Stroke = new SolidColorBrush(Colors.Blue);
        }

        public void Select()
        {
            if (this.IsSelected)
                return;
            foreach (ConnectionPoint c in this.connectionList)
            {
                c.connectionPoint.Visibility = Visibility.Visible;
            }
            this.getParentDiagram().tbRenameSelected.Text = this.componentName;
            this.recComponent.Stroke = new SolidColorBrush(Colors.Red);
            
        }


        /// <summary>
        /// returns current position of this control into its editor.
        /// </summary>
        /// <returns></returns>
        public Point GetPosition()
        {
            return new Point(this.Margin.Left, this.Margin.Top);
        }

        /// <summary>
        /// Moves this control through its parent by modifying its margin attr.
        /// </summary>
        /// <param name="target"></param>
        public void SetPosition(Point target)
        {
            this.Margin = new Thickness(target.X, target.Y, 0, 0);
        }

        private void recComponent_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Object[] data = new Object[] { this, Mouse.GetPosition(this) };
            DragDrop.DoDragDrop(this, data, DragDropEffects.Move);
            this.Select();
        }

        public ConnectionPoint getSelectedPoint()
        {
            return this.SelectedPoint;
        }
   
        public void setSelectedPoint(ConnectionPoint newValue)
        {
            this.SelectedPoint = newValue;
        }

        public bool IsSelected { get; set; }
       
        public ComponentDiagram getParentDiagram()
        {
            return this.ParentDiagram;
        }

        public void setRelatedComponent(SelectObject so)
        {
            this.RelatedComponent = so;
        }

        public string getName()
        {
            return this.componentName;
        }

        public void setName(string name)
        {
            this.componentName = name;
            this.txtComponentName.Content = name;
        }
    }
}
