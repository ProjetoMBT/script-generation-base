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
    /// Interaction logic for Notation.xaml
    /// </summary>
    public partial class Notation : UserControl, SelectObject
    {

        public string Text
        {
            get
            {
                return this.TextBlock_Text.Text;
            }

            set
            {
                this.TextBlock_Text.Text = value;
            }
        }



        public Notation(Ball b)
        {
            InitializeComponent();
            this.ParentBall = b;
            UpdateVisualInformation();
            MaxSelection = 0;
            MinSelection = 0;
        }


        public Point GetPosition()
        {
            Point p = new Point();
            p.X = this.Margin.Left;
            p.Y = this.Margin.Top;

            return p;
        }

        public void SetPosition(Point p)
        {
            this.Margin = new Thickness(p.X, p.Y, 0, 0);
        }

        public int MinSelection { get; set; }
        public int MaxSelection { get; set; }
        public List<string> Variants { get; set; }
        public string BindingTime { get; set; }
        public ComponentDiagram ParentDiagram { get; set; }
        public Ball ParentBall { get; set; }
        public Line relatedLine { get; set; }

        internal void UpdateVisualInformation()
        {
            int minIndex = this.lblMinSel.Content.ToString().IndexOf('=');
            this.lblMinSel.Content = this.lblMinSel.Content.ToString().Substring(0, minIndex + 1) + " " + this.MinSelection;
            int maxIndex = this.lblMaxSel.Content.ToString().IndexOf('=');
            this.lblMaxSel.Content = this.lblMaxSel.Content.ToString().Substring(0, maxIndex + 1) + " " + this.MaxSelection;

            if (this.ParentBall.Variants.Count > 0)
            {
                int variantIndex = this.lblVariants.Content.ToString().IndexOf('=');
                string variantsName = "[\n";
                foreach (string s in this.ParentBall.getVariantsName())
                {
                    variantsName += s + "\n";
                }

                variantsName += "]";
                this.lblVariants.Content = this.lblVariants.Content.ToString().Substring(0, variantIndex + 1) + " " + variantsName;
            }

            else
            {
                int variantIndex = this.lblVariants.Content.ToString().IndexOf('=');
                string variants = "";
                this.lblVariants.Content = this.lblVariants.Content.ToString().Substring(0, variantIndex + 1) + variants;
            }
           
        }

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Object[] data = new Object[] { this, Mouse.GetPosition(this) };
            DragDrop.DoDragDrop(this, data, DragDropEffects.Move);

        }

        public void UpdateLineCoordinate()
        {
            Line l = new Line();
            this.ParentDiagram.containerEditor.Children.Remove(this.relatedLine);          
            l.X1 = this.ParentBall.GetPosition().X + this.ParentBall.Width / 2;
            l.Y1 = this.ParentBall.GetPosition().Y + this.ParentBall.Height / 2;
            l.X2 = this.GetPosition().X + this.ActualWidth / 2;
            l.Y2 = this.GetPosition().Y + this.ActualHeight / 2;
            l.Stroke = new SolidColorBrush(Colors.Black);
            l.StrokeDashCap = PenLineCap.Triangle;
            l.StrokeDashArray = new DoubleCollection() { 6, 6 };
            this.relatedLine = l;
            this.relatedLine.Tag = "noteLine";
            this.ParentDiagram.containerEditor.Children.Add(l);
            Grid.SetZIndex(relatedLine, -1);

        }

        public ComponentDiagram getParentDiagram()
        {
            return this.ParentDiagram;
        }

        public void Select()
        {
            return;
        }
        public void Unselect()
        {
            return;
        }

        public ConnectionPoint getSelectedPoint()
        {
            return null;
        }
        public void setSelectedPoint(ConnectionPoint newValue)
        {
            return;
        }

        public void setRelatedComponent(SelectObject so)
        {
            return;
        }

        private void lblMaxSel_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TextBox renameBox = new TextBox();
            renameBox.Width = 20;
            renameBox.Height = 20;
            renameBox.VerticalAlignment = VerticalAlignment.Top;
            renameBox.HorizontalAlignment = HorizontalAlignment.Left;
            renameBox.Margin = new Thickness(lblMaxSel.Margin.Left + 85, lblMaxSel.Margin.Top,0,0);
            ((Grid)this.lblMaxSel.Parent).Children.Add(renameBox);
            int index = this.lblMaxSel.Content.ToString().IndexOf("=");
            string newText = this.lblMaxSel.Content.ToString().Substring(index + 2).Trim();
            renameBox.Text = newText;
            renameBox.KeyDown += new KeyEventHandler(renameBox_KeyDown);
        }

        private void renameBox_KeyDown(object o, KeyEventArgs args)
        {
            if (args.Key == Key.Enter)
            {
                int index = this.lblMaxSel.Content.ToString().IndexOf("=");
                this.lblMaxSel.Content = this.lblMaxSel.Content.ToString().Replace(this.lblMaxSel.Content.ToString().Substring(index + 2).Trim(), ((TextBox)o).Text);
                //this.lblMaxSel.Content = ((TextBox)o).Text;
                try
                {
                    MaxSelection = Int32.Parse(((TextBox)o).Text);
                }
                catch (Exception)
                {
                    return;
                }
                ((Grid)this.lblMaxSel.Parent).Children.Remove((TextBox)o);
            }
            else if (args.Key == Key.Escape)
            {
                ((Grid)this.lblMaxSel.Parent).Children.Remove((TextBox)o);
                this.lblMaxSel.Visibility = Visibility.Visible;
            }
        }

        private void lblMinSel_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TextBox renameBox = new TextBox();
            renameBox.Width = 20;
            renameBox.Height = 20;
            renameBox.VerticalAlignment = VerticalAlignment.Top;
            renameBox.HorizontalAlignment = HorizontalAlignment.Left;
            renameBox.Margin = new Thickness(lblMinSel.Margin.Left + 85, lblMinSel.Margin.Top, 0, 0);
            ((Grid)this.lblMinSel.Parent).Children.Add(renameBox);
            int index = this.lblMinSel.Content.ToString().IndexOf("=");
            string newText = this.lblMinSel.Content.ToString().Substring(index + 2).Trim();
            renameBox.Text = newText;
            renameBox.KeyDown += new KeyEventHandler(renameBoxMin_KeyDown);
        }
        private void renameBoxMin_KeyDown(object o, KeyEventArgs args)
        {
            if (args.Key == Key.Enter)
            {
                int index = this.lblMinSel.Content.ToString().IndexOf("=");
                this.lblMinSel.Content = this.lblMinSel.Content.ToString().Replace(this.lblMinSel.Content.ToString().Substring(index + 2).Trim(), ((TextBox)o).Text);
                //this.lblMaxSel.Content = ((TextBox)o).Text;
                try
                {
                    MinSelection = Int32.Parse(((TextBox)o).Text);
                }
                catch(Exception)
                {
                    return;
                }
                ((Grid)this.lblMinSel.Parent).Children.Remove((TextBox)o);
            }
            else if (args.Key == Key.Escape)
            {
                ((Grid)this.lblMinSel.Parent).Children.Remove((TextBox)o);
                this.lblMinSel.Visibility = Visibility.Visible;
            }
        }
        public string getName()
        {
            return "";
        }

        public void setName(string name)
        {

        }
    }
}
