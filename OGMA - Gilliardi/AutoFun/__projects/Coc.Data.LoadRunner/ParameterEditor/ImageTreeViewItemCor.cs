using System;
using System.Windows;
using System.Windows.Controls;
using Coc.Data.LoadRunner.SequenceModel;

namespace Coc.Data.LoadRunner.ParameterEditor
{
    public class ImageTreeViewItemCor : TreeViewItem
    {

        public ScriptParameterizationData Data = null;
        public ParameterEditorWindow ParentWindow = null;
        public Image Image { get; private set; }
        private TextBlock block;
        public String Text { get { return block.Text; } set { block.Text = value; } }
        public CustomCheckbox checkbox { get; set; }
        Grid g;
        public TextBox leftBound { get; set; }
        public TextBox rightBound { get; set; }
        public TextBox paramPrefix { get; set; }
        public ComboBox action { get; set; }
        public ComboBox order { get; set; }
        public SaveParameter saveParameter;
        public Rule rule;

        public ImageTreeViewItemCor() : base()
        {
            g = new Grid();
            g.ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(20) });
            g.ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(20) });
            g.ColumnDefinitions.Add(new ColumnDefinition());

            Image m = new Image();
            this.Image = m;
            this.Image.Width = 16;
            this.Image.Height = 16;
            this.Image.Margin = new System.Windows.Thickness(-50, 0, 0, 0);
            this.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            this.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            Grid.SetColumn(this.Image, 1);
            g.Children.Add(m);

            TextBlock b = new TextBlock();
            this.block = b;
            this.block.Margin = new System.Windows.Thickness(-25, 0, 0, 0);
            Grid.SetColumn(this.block, 2);
            g.Children.Add(b);

            checkbox = new CustomCheckbox(this);
            checkbox.Margin = new System.Windows.Thickness(-20, 0, 0, 0);
            this.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            this.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            Grid.SetColumn(this.checkbox, 0);
            checkbox.IsChecked = true;
            g.Children.Add(checkbox);
            this.Header = g;

            //Text Box Left Boundary
            leftBound = new System.Windows.Controls.TextBox();
            leftBound.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            leftBound.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            leftBound.Margin = new Thickness(140, 72, 0, 0);
            leftBound.Name = "tbLfBound";
            leftBound.Width = 150;
            //Text Box Right Boundary
            rightBound = new System.Windows.Controls.TextBox();
            rightBound.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            rightBound.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            rightBound.Margin = new Thickness(152, 102, 0, 0);
            rightBound.Name = "tbRgBound";
            rightBound.Width = 150;
            //Text Box Prefix
            paramPrefix = new System.Windows.Controls.TextBox();
            paramPrefix.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            paramPrefix.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            paramPrefix.Margin = new Thickness(152, 132, 0, 0);
            paramPrefix.Name = "tbPrefix";
            paramPrefix.Width = 150;

            //Instantiate Action ComboBox
            this.action = new System.Windows.Controls.ComboBox();
            action.Name = "cbAction";

            ComboBoxItem cbi = new ComboBoxItem();
            cbi.Content = "Search for parameters in all of the body text";
            action.Items.Add(cbi);
            action.SelectedItem = cbi;

            cbi = new ComboBoxItem();
            cbi.Content = "Search for parameters in links and form actions";
            action.Items.Add(cbi);

            cbi = new ComboBoxItem();
            cbi.Content = "Search for parameters in cookie headers";
            action.Items.Add(cbi);

            cbi = new ComboBoxItem();
            cbi.Content = "Parameterize form field value";
            action.Items.Add(cbi);

            cbi = new ComboBoxItem();
            cbi.Content = "Text to enter a web_reg_add_cookie function by";
            action.Items.Add(cbi);

            action.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            action.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            action.Margin = new Thickness(100, 12, 0, 0);

            //Instantiate Order ComboBox
            this.order = new System.Windows.Controls.ComboBox();
            order.Name = "cbOrder";
            //Instatiate new Itens into ComboBox
            cbi = new ComboBoxItem();
            cbi.Content = "ALL";
            order.Items.Add(cbi);
            order.SelectedItem = cbi;
            cbi = new ComboBoxItem();
            cbi.Content = "1";
            order.Items.Add(cbi);
            order.SelectedItem = cbi;
            //Set vertical and horizontal Aligments
            order.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            order.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            order.Margin = new Thickness(100, 42, 0, 0);

            this.ContextMenu = new System.Windows.Controls.ContextMenu();
            System.Windows.Controls.MenuItem mm = new System.Windows.Controls.MenuItem() { Header = "Import Correlation" };
            //relate menu item with event mm_click
            mm.Click += new System.Windows.RoutedEventHandler(mm_Click);
            ContextMenu.Items.Add(mm);
            this.Header = g;
        }
        /// <summary>
        /// Magic method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mm_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //call import event
            this.ParentWindow.btnImportCor_Click(sender, e, this);
        }

        public string Information { get; set; }
    }
}