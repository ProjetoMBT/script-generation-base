using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Forms;

namespace Coc.Data.LoadRunner.ParameterEditor
{
    public class ImageTreeViewItem : TreeViewItem
    {
        public ScriptParameterizationData Data = null;
        public ParameterEditorWindow ParentWindow = null;
        public Image Image { get; private set; }
        public List<string> nextRows = new List<string>();
        private TextBlock block;
        public string selectedItem;
        public String Text { get { return block.Text; } set { block.Text = value; } }
        public CustomCheckbox checkbox { get; set; }
        Grid g;

        public ImageTreeViewItem() : base()
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

            this.ContextMenu = new System.Windows.Controls.ContextMenu();
            System.Windows.Controls.MenuItem mm = new System.Windows.Controls.MenuItem() { Header = "Replicate" };
            System.Windows.Controls.MenuItem mm2 = new System.Windows.Controls.MenuItem() { Header = "Delete" };
            ContextMenu.Items.Add(mm);
            ContextMenu.Items.Add(mm2);
            mm.Click += new System.Windows.RoutedEventHandler(mm_Click);
            mm2.Click += new System.Windows.RoutedEventHandler(mm2_Click);
            this.selectedItem = "Random";
            this.Header = g;
        }

        void mm2_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                foreach (ImageTreeViewItem scenario in ParentWindow.treeViewScenarios.Items)
                {
                    foreach (ImageTreeViewItem script in scenario.Items)
                    {
                        foreach (ImageTreeViewItem parameter in script.Items)
                        {
                            if (parameter == this)
                            {
                                script.Items.Remove(parameter);
                                break;
                            }
                        }
                    }
                }
                foreach (ImageTreeViewItem scenario in ParentWindow.treeViewScenarios.Items)
                {
                    foreach (ImageTreeViewItem script in scenario.Items)
                    {
                        foreach (ImageTreeViewItem parameter in script.Items)
                        {
                            parameter.nextRows.Remove(this.Text);
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("Unable to delete information for the other parameters.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        void mm_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                foreach (ImageTreeViewItem scenario in ParentWindow.treeViewScenarios.Items)
                {
                    foreach (ImageTreeViewItem script in scenario.Items)
                    {
                        foreach (ImageTreeViewItem parameter in script.Items)
                        {
                            if (parameter.Text == this.Text && parameter != this)
                            {
                                parameter.Data = new ScriptParameterizationData()
                                {
                                    Name = this.Data.Name,
                                    Delimiter = this.Data.Delimiter,
                                    TableLocation = this.Data.TableLocation,
                                    ColumnName = this.Data.ColumnName,
                                    GenerateNewValue = this.Data.GenerateNewValue,
                                    Table = this.Data.Table,
                                    Type = this.Data.Type,
                                    ValueForEachVirtualUser = this.Data.ValueForEachVirtualUser,
                                    OriginalValue = this.Data.OriginalValue,
                                    AutoAllocateBlockSize = this.Data.AutoAllocateBlockSize,
                                    SelectNextRow = this.Data.SelectNextRow,
                                    StartRow = this.Data.StartRow,
                                    OutOfRangePolicy = this.Data.OutOfRangePolicy
                                };
                            }
                        }
                    }
                }

                MessageBox.Show("The parameters of " + this.Text + " has been replicated successfully", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch (Exception )
            {
                MessageBox.Show("Unable to replicate information for the other parameters.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
    }
}
