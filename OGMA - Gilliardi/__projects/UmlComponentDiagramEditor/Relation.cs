using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;


namespace UmlComponentDiagramEditor
{
    public class Relation: SelectObject
    {

        public enum ConnectionType { Mutex, Requires };
        public enum RequiresPosition { Up, Down, Left, Right };
        public ConnectionType connectionType {get; set;}
        public Component Source { get; set; }
        public Component Target { get; set; }
        private ComponentDiagram Parent { get; set; }
        public List<Line> relationLines { get; set; }
        public Label relationDescription { get; set; }
        //public Arrow RequiresArrow { get; set; }
        private bool isSelected;
        public bool IsSelected { get { return isSelected;} }

        public Relation(ConnectionType ct, ComponentDiagram parent, Component source, Component target)
        {
            this.connectionType = ct;
            this.Parent = parent;
            this.Source = source;
            this.Target = target;
            relationDescription = new Label();
            this.isSelected = false;

            this.relationLines = new List<Line>();
            if (ct == ConnectionType.Mutex)
            {
                drawMutexConnection();
            }
            if (ct == ConnectionType.Requires)
            {
                drawRequiresConnection();
            }
        }

        private void drawMutexConnection()
        {
            double midCoordinate = Source.GetPosition().Y + Source.Width + 10;   
            
            Point startMidPoint = new Point(Source.GetPosition().X + Source.Width / 2, midCoordinate); 
            Point endMidPoint = new Point(Target.GetPosition().X + Target.Width / 2, midCoordinate);
            Point startSourcePoint = new Point(Source.GetPosition().X + Source.Width / 2, Source.GetPosition().Y + Source.Height - 3);
            Point endSourcePoint = new Point(Source.GetPosition().X + Source.Width / 2, startMidPoint.Y);
            Point startTargetPoint = new Point(Target.GetPosition().X + Target.Width / 2, endMidPoint.Y);
            Point endTargetPoint = new Point(endMidPoint.X, Target.GetPosition().Y + Target.Height - 3);

            Line sourceLine = new Line();
            Line midLine = new Line();
            Line targetLine = new Line();

            sourceLine.MouseDown += new System.Windows.Input.MouseButtonEventHandler(line_MouseDown);
            midLine.MouseDown += new System.Windows.Input.MouseButtonEventHandler(line_MouseDown);
            targetLine.MouseDown += new System.Windows.Input.MouseButtonEventHandler(line_MouseDown);
            relationDescription.MouseDown += new System.Windows.Input.MouseButtonEventHandler(line_MouseDown);

            relationLines.Add(sourceLine);
            relationLines.Add(midLine);
            relationLines.Add(targetLine);

            sourceLine.X1 = startSourcePoint.X;
            sourceLine.Y1 = startSourcePoint.Y;
            sourceLine.X2 = endSourcePoint.X;
            sourceLine.Y2 = endSourcePoint.Y;

            midLine.X1 = startMidPoint.X;
            midLine.Y1 = startMidPoint.Y;
            midLine.X2 = endMidPoint.X;
            midLine.Y2 = endMidPoint.Y;

            targetLine.X1 = startTargetPoint.X;
            targetLine.Y1 = startTargetPoint.Y;
            targetLine.X2 = endTargetPoint.X;
            targetLine.Y2 = endTargetPoint.Y;

            sourceLine.Stroke = new SolidColorBrush(Colors.Black);
            sourceLine.StrokeDashCap = PenLineCap.Triangle;
            sourceLine.StrokeDashArray = new DoubleCollection() { 6, 6 };
            sourceLine.UpdateLayout();
            targetLine.Stroke = new SolidColorBrush(Colors.Black);
            targetLine.StrokeDashCap = PenLineCap.Triangle;
            targetLine.StrokeDashArray = new DoubleCollection() { 6, 6 };
            targetLine.UpdateLayout();
            midLine.Stroke = new SolidColorBrush(Colors.Black);
            midLine.StrokeDashCap = PenLineCap.Triangle;
            midLine.StrokeDashArray = new DoubleCollection() { 6, 6 };
            midLine.UpdateLayout();
            midLine.Tag = "relation";
            sourceLine.Tag = "relation";
            targetLine.Tag = "relation";
            relationDescription.Content = "<<Mutex>>";
            relationDescription.FontSize = 12;
            relationDescription.VerticalAlignment = VerticalAlignment.Top;
            relationDescription.HorizontalAlignment = HorizontalAlignment.Left;
            relationDescription.Margin = new Thickness(startMidPoint.X + (midLine.ActualWidth / 2), startMidPoint.Y, 0, 0);

            this.Parent.containerEditor.Children.Add(sourceLine);
            this.Parent.containerEditor.Children.Add(midLine);
            this.Parent.containerEditor.Children.Add(targetLine);
            this.Parent.containerEditor.Children.Add(relationDescription);

            Grid.SetZIndex(sourceLine, -1);
            Grid.SetZIndex(targetLine, -1);
            Grid.SetZIndex(midLine, -1);
        }

        private void drawRequiresConnection()
        {
            double midCoordinateX = ((Source.GetPosition().X + (Source.Width/2)) + (Target.GetPosition().X + (Target.Width/2))) / 2;
            double midCoordinateY = ((Source.GetPosition().Y + (Source.Height / 2)) + (Target.GetPosition().Y + (Target.Height / 2))) / 2;

            Point startMidPoint = new Point();
            Point endMidPoint = new Point();
            Point startSourcePoint = new Point();
            Point endSourcePoint = new Point();
            Point startTargetPoint = new Point();
            Point endTargetPoint = new Point();
            


            switch (calculateEndLinePoint())
            {
            
                case RequiresPosition.Right:
                    startMidPoint = new Point(midCoordinateX, Source.GetPosition().Y + Source.Height / 2);
                    endMidPoint = new Point(midCoordinateX, Target.GetPosition().Y + Target.Height / 2 + 20);
                    startSourcePoint = new Point(Source.GetPosition().X + Source.Width / 2, Source.GetPosition().Y + Source.Height / 2);
                    endSourcePoint = new Point(startMidPoint.X, Source.GetPosition().Y + Source.Height / 2);
                    startTargetPoint = new Point(endMidPoint.X, endMidPoint.Y);
                    endTargetPoint = new Point(Target.GetPosition().X, endMidPoint.Y);

                    RotateTransform rt = new RotateTransform(90);
                    //RequiresArrow.RenderTransform = rt;
                    break;
                case RequiresPosition.Left:
                    startMidPoint = new Point(midCoordinateX, Source.GetPosition().Y + Source.Height / 2);
                    endMidPoint = new Point(midCoordinateX, Target.GetPosition().Y + Target.Height / 2 + 20);
                    startSourcePoint = new Point(Source.GetPosition().X + Source.Width / 2, Source.GetPosition().Y + Source.Height / 2);
                    endSourcePoint = new Point(startMidPoint.X, Source.GetPosition().Y + Source.Height / 2);
                    startTargetPoint = new Point(endMidPoint.X, endMidPoint.Y);
                    endTargetPoint = new Point(Target.GetPosition().X + Target.Width, endMidPoint.Y);
                    //RequiresArrow.SetPosition(new Point(endTargetPoint.X - 3, endTargetPoint.Y + (RequiresArrow.Height / 2)));
                    RotateTransform rtR = new RotateTransform(270);
                    //RequiresArrow.RenderTransform = rtR;
                    break;
                case RequiresPosition.Down:
                    startMidPoint = new Point(Source.GetPosition().X + Source.Width/2, midCoordinateY);
                    endMidPoint = new Point(Target.GetPosition().X + Target.Width/2  - 20, midCoordinateY);
                    startSourcePoint = new Point(Source.GetPosition().X + Source.Width / 2, Source.GetPosition().Y + Source.Height / 2);
                    endSourcePoint = new Point(startMidPoint.X, startMidPoint.Y);
                    startTargetPoint = new Point(endMidPoint.X, endMidPoint.Y);
                    endTargetPoint = new Point(endMidPoint.X, Target.GetPosition().Y);
                    //RequiresArrow.SetPosition(new Point(endTargetPoint.X + (RequiresArrow.Width/2), endTargetPoint.Y + 3));
                     RotateTransform rtD = new RotateTransform(180);
                    //RequiresArrow.RenderTransform = rtD;
                    break;
                case RequiresPosition.Up:
                     startMidPoint = new Point(Source.GetPosition().X + Source.Width/2, midCoordinateY);
                    endMidPoint = new Point(Target.GetPosition().X + Target.Width/2  - 20, midCoordinateY);
                    startSourcePoint = new Point(Source.GetPosition().X + Source.Width / 2, Source.GetPosition().Y + Source.Height / 2);
                    endSourcePoint = new Point(startMidPoint.X, startMidPoint.Y);
                    startTargetPoint = new Point(endMidPoint.X, endMidPoint.Y);
                    endTargetPoint = new Point(endMidPoint.X, Target.GetPosition().Y + Target.Height);
                    //RequiresArrow.SetPosition(new Point(endTargetPoint.X - (RequiresArrow.Width/2), endTargetPoint.Y - 2));
                     RotateTransform rtU = new RotateTransform(0);
                    //RequiresArrow.RenderTransform = rtU;
                    break;
            }

            Line sourceLine = new Line();
            Line midLine = new Line();
            Line targetLine = new Line();
            
            sourceLine.MouseDown += new System.Windows.Input.MouseButtonEventHandler(line_MouseDown);
            midLine.MouseDown += new System.Windows.Input.MouseButtonEventHandler(line_MouseDown);
            targetLine.MouseDown += new System.Windows.Input.MouseButtonEventHandler(line_MouseDown);
            //RequiresArrow.MouseDown += new System.Windows.Input.MouseButtonEventHandler(line_MouseDown);
            relationDescription.MouseDown += new System.Windows.Input.MouseButtonEventHandler(line_MouseDown);

            relationLines.Clear();
            relationLines.Add(sourceLine);
            relationLines.Add(midLine);
            relationLines.Add(targetLine);

            sourceLine.X1 = startSourcePoint.X;
            sourceLine.Y1 = startSourcePoint.Y;
            sourceLine.X2 = endSourcePoint.X;
            sourceLine.Y2 = endSourcePoint.Y;

            midLine.X1 = startMidPoint.X;
            midLine.Y1 = startMidPoint.Y;
            midLine.X2 = endMidPoint.X;
            midLine.Y2 = endMidPoint.Y;

            targetLine.X1 = startTargetPoint.X;
            targetLine.Y1 = startTargetPoint.Y;
            targetLine.X2 = endTargetPoint.X;
            targetLine.Y2 = endTargetPoint.Y;

            sourceLine.Stroke = new SolidColorBrush(Colors.Black);
            sourceLine.StrokeDashCap = PenLineCap.Triangle;
            sourceLine.StrokeDashArray = new DoubleCollection() { 6, 6 };
            sourceLine.UpdateLayout();
            targetLine.Stroke = new SolidColorBrush(Colors.Black);
            targetLine.StrokeDashCap = PenLineCap.Triangle;
            targetLine.StrokeDashArray = new DoubleCollection() { 6, 6 };
            targetLine.UpdateLayout();
            midLine.Stroke = new SolidColorBrush(Colors.Black);
            midLine.StrokeDashCap = PenLineCap.Triangle;
            midLine.StrokeDashArray = new DoubleCollection() { 6, 6 };
            midLine.UpdateLayout();
            midLine.Tag = "relation";
            sourceLine.Tag = "relation";
            targetLine.Tag = "relation";

            relationDescription.Content = "<<Requires>>";
            relationDescription.FontSize = 12;
            relationDescription.VerticalAlignment = VerticalAlignment.Top;
            relationDescription.HorizontalAlignment = HorizontalAlignment.Left;
            relationDescription.Margin = new Thickness(startMidPoint.X + (midLine.ActualWidth/2), startMidPoint.Y, 0 ,0);

          
            this.Parent.containerEditor.Children.Add(sourceLine);
            this.Parent.containerEditor.Children.Add(midLine);
            this.Parent.containerEditor.Children.Add(targetLine);
            this.Parent.containerEditor.Children.Add(relationDescription);

            //this.Parent.containerEditor.Children.Add(RequiresArrow);
            Grid.SetZIndex(sourceLine, -1);
            Grid.SetZIndex(targetLine, -1);
            Grid.SetZIndex(midLine, -1);
            //Grid.SetZIndex(RequiresArrow, -1);
        }

        private RequiresPosition calculateEndLinePoint()
        {
            if (Source != null && Target != null)
            {
                if (Source.GetPosition().X < Target.GetPosition().X)
                {
                    double diferenceX = Target.GetPosition().X - Source.GetPosition().X;
                    double diferenceY;
                    if (Source.GetPosition().Y < Target.GetPosition().Y)
                    {
                        diferenceY = Target.GetPosition().Y - Source.GetPosition().Y;
                    }
                    else
                    {
                        diferenceY = Source.GetPosition().Y - Target.GetPosition().Y;
                    }

                    if (diferenceY < diferenceX)
                    {
                        return RequiresPosition.Right;
                    }
                    else
                    {
                        if (Source.GetPosition().Y < Target.GetPosition().Y)
                        {
                            return RequiresPosition.Down;
                        }
                        else
                        {
                            return RequiresPosition.Up;
                        }
                    }
                }

                else if (Source.GetPosition().X > Target.GetPosition().X)
                {
                    double diferenceX = Source.GetPosition().X - Target.GetPosition().X;
                    double diferenceY;
                    if (Source.GetPosition().Y < Target.GetPosition().Y)
                    {
                        diferenceY = Target.GetPosition().Y - Source.GetPosition().Y;
                    }
                    else
                    {
                        diferenceY = Source.GetPosition().Y - Target.GetPosition().Y;
                    }

                    if (diferenceY < diferenceX)
                    {
                        return RequiresPosition.Left;
                    }
                    else
                    {
                        if (Source.GetPosition().Y < Target.GetPosition().Y)
                        {
                            return RequiresPosition.Down;
                        }
                        else
                        {
                            return RequiresPosition.Up;

                        }
                    }
                }
            }
            return RequiresPosition.Right;
        }

        public void updateLines()
        {

            foreach (Line l in relationLines)
            {
                this.Parent.containerEditor.Children.Remove(l);
            }

            this.relationLines.Clear();
            this.Parent.containerEditor.Children.Remove(relationDescription);
            if (this.connectionType == ConnectionType.Mutex)
            {
                drawMutexConnection();
            }
            else
            {
                drawRequiresConnection();
            }
        }

        public void removeRelation()
        {
            foreach (Line l in relationLines)
            {
                this.Parent.containerEditor.Children.Remove(l);
            }
            this.Parent.containerEditor.Children.Remove(relationDescription);
            this.Parent.RelationList.Remove(this);
        }

        private void line_MouseDown(object o, System.Windows.Input.MouseEventArgs e)
        {
            if (this.IsSelected)
                this.Unselect();
            else
            {
                if (this.Parent.SelectedComponent != null && this.Parent.SelectedComponent != this)
                    ((SelectObject)this.Parent.SelectedComponent).Unselect();
            
                this.Parent.SelectedComponent = this;
            }
        }

        #region SelectObject members

        public void SetPosition(Point p)
        {
            //throw new NotImplementedException();
        }

        public Point GetPosition()
        {
            //return RequiresArrow.GetPosition();
            return new Point(relationLines[2].X2, relationLines[2].Y2);
        }

        public void Select()
        {
            if (this.IsSelected)
                return;

            foreach (Line l in relationLines)
            {
                l.Stroke = new SolidColorBrush(Colors.Red);
            }

            this.isSelected = true;
        }

        public void Unselect()
        {
            foreach (Line l in relationLines)
            {
                l.Stroke = new SolidColorBrush(Colors.Black);
            }

            this.isSelected = false;
        }

        public string getName()
        {
            return "requires relation";
        }

        public void setName(string name)
        {
            //throw new NotImplementedException();
        }

        public ConnectionPoint getSelectedPoint()
        {
            return null;
        }

        public void setSelectedPoint(ConnectionPoint newValue)
        {
            //throw new NotImplementedException();
        }

        public ComponentDiagram getParentDiagram()
        {
            return this.Parent;
        }

        public void setRelatedComponent(SelectObject so)
        {
            //throw new NotImplementedException();
        }
        #endregion
    }
}
