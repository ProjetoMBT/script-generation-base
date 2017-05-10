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
using PlugSpl.Atlas;
namespace FeatureModelEditor.ConstraintEditor
{
    /// <summary>
    /// Interaction logic for ConstraintEditorControl.xaml
    /// </summary>
    public partial class ConstraintEditorControl : UserControl, IConstraintEditor
    {

        #region Constructors



        /// <summary>
        /// Default constructor.
        /// </summary>
        public ConstraintEditorControl()
        {
            InitializeComponent();

        }

        #endregion

        #region Fields and Properties

        /// <summary>
        /// Stores current feature model atlas. All information must be retrieved from here.
        /// </summary>
        AtlasFeatureModel atlas = null;



        #endregion

        #region Interface Methods

        /// <summary>
        /// Returns this control. Used by host control.
        /// </summary>
        /// <returns></returns>
        public IConstraintEditor GetControl()
        {
            return this;
        }

        /// <summary>
        /// Sets current FeatureModelAtlas.
        /// </summary>
        public void SetFeatureModel(AtlasFeatureModel model)
        {
            this.atlas = model;
        }

        /// <summary>
        /// Adds a constraint to this constraint editor.
        /// </summary>        
        public void AddConstraint(AtlasConstraint constraint)
        {
            ConstraintGrid cGrid = new ConstraintGrid();
            cGrid.MouseLeftButtonDown += new MouseButtonEventHandler(block_MouseLeftButtonDown);
            this.listViewContraints.Items.Add(cGrid);
            this.listViewContraints.SelectedItem = cGrid;

            foreach (IConstraintMember member in constraint.Constraints)
            {

                TextBlock block = new TextBlock();
                block.Text = member.GetMemberName();
                //block.FontSize = 16;
                if (member is AtlasConstraintOperator)
                    block.Foreground = new SolidColorBrush(Colors.Green);
                else
                    block.Foreground = new SolidColorBrush(Colors.Blue);

                block.Margin = new Thickness(5, 2, 5, 2);
                ConstraintGrid grid = this.listViewContraints.SelectedItem as ConstraintGrid;

                if (grid == null)
                    return;

                grid.GetInnerContainer().Children.Add(block);

            }
        }

        public void Clear()
        {
            this.listViewContraints.Items.Clear();
        }
        #endregion

        /// <summary>
        /// Load feature list from atlas. Filtered features names must match with filter string.
        /// </summary>
        private void loadContraintFilter(string p)
        {

            if (this.atlas == null)
                return;

            List<string> listString = new List<string>();
            this.uniformGridFeatureContainer.Children.Clear();

            //filters list
            foreach (AtlasFeature f in atlas.Features)
            {
                if (f.Name.ToLower().Contains(p.ToLower()))
                {
                    listString.Add(f.Name);
                }
            }

            //sort feature names
            listString.Sort();

            foreach (string ss in listString)
            {
                HyperlinkTextBlock block = new HyperlinkTextBlock(ss);
                block.MouseDown += new MouseButtonEventHandler(block_MouseDown);
                this.uniformGridFeatureContainer.Children.Add(block);
            }
        }

        #region Events and Delegates

        /// <summary>
        /// Filters feature list to match criteria.
        /// </summary>
        private void textBoxFilter_KeyDown(object sender, KeyEventArgs e)
        {

            switch (e.Key)
            {
                case Key.Enter:
                    this.loadContraintFilter(this.textBoxFilter.Text);
                    break;
            }
        }

        #endregion

        void block_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.addFeature(((TextBlock)sender).Text);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        private void addFeature(string p)
        {
            TextBlock block = new TextBlock();
            block.Text = p;
            block.Foreground = new SolidColorBrush(Colors.Blue);
            block.Margin = new Thickness(5, 2, 5, 2);
            ConstraintGrid grid = this.listViewContraints.SelectedItem as ConstraintGrid;

            if (grid == null)
                return;

            grid.GetInnerContainer().Children.Add(block);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            ConstraintGrid cGrid = new ConstraintGrid();
            cGrid.MouseLeftButtonDown += new MouseButtonEventHandler(block_MouseLeftButtonDown);
            this.listViewContraints.Items.Add(cGrid);
            this.listViewContraints.SelectedItem = cGrid;
        }

        void block_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.listViewContraints.SelectedItem = sender;
        }


        /// <summary>
        /// Adds an or operator to end of selected constraint.
        /// </summary>
        private void buttonOr_Click(object sender, RoutedEventArgs e)
        {
            TextBlock block = new TextBlock();
            block.Text = "∨";
            block.Foreground = new SolidColorBrush(Colors.Green);
            block.Margin = new Thickness(5, 2, 5, 2);
            ConstraintGrid grid = this.listViewContraints.SelectedItem as ConstraintGrid;

            if (grid == null)
                return;

            grid.GetInnerContainer().Children.Add(block);
        }

        /// <summary>
        /// Adds an and operator to end of selected constraint.
        /// </summary>
        private void buttonAnd_Click(object sender, RoutedEventArgs e)
        {
            TextBlock block = new TextBlock();
            block.Text = "∧";
            block.Foreground = new SolidColorBrush(Colors.Green);
            block.Margin = new Thickness(5, 2, 5, 2);
            ConstraintGrid grid = this.listViewContraints.SelectedItem as ConstraintGrid;

            if (grid == null)
                return;

            grid.GetInnerContainer().Children.Add(block);
        }

        /// <summary>
        /// Adds a not operator to end of selected constraint.
        /// </summary>
        private void buttonNot_Click(object sender, RoutedEventArgs e)
        {
            TextBlock block = new TextBlock();
            block.Text = "¬";
            block.Foreground = new SolidColorBrush(Colors.Green);
            block.Margin = new Thickness(5, 2, 5, 2);
            ConstraintGrid grid = this.listViewContraints.SelectedItem as ConstraintGrid;

            if (grid == null)
                return;

            grid.GetInnerContainer().Children.Add(block);
        }

        /// <summary>
        /// Adds an implies operator to end of selected constraint.
        /// </summary>
        private void buttonImplies_Click(object sender, RoutedEventArgs e)
        {
            TextBlock block = new TextBlock();
            block.Text = "⇒";
            block.Foreground = new SolidColorBrush(Colors.Green);
            block.Margin = new Thickness(5, 2, 5, 2);
            ConstraintGrid grid = this.listViewContraints.SelectedItem as ConstraintGrid;

            if (grid == null)
                return;

            grid.GetInnerContainer().Children.Add(block);
        }

        /// <summary>
        /// Remove last constraint element.
        /// </summary>
        private void buttonBackspace_Click(object sender, RoutedEventArgs e)
        {
            ConstraintGrid grid = this.listViewContraints.SelectedItem as ConstraintGrid;

            if (grid != null)
            {
                //get last block
                TextBlock block = grid.GetInnerContainer().Children.OfType<TextBlock>().LastOrDefault();
                if (block != null)
                    grid.GetInnerContainer().Children.Remove(block);
            }
        }

        /// <summary>
        /// Remove selected constraint.
        /// </summary>
        private void buttonRemove_Click(object sender, RoutedEventArgs e)
        {
            if (this.listViewContraints.SelectedIndex > -1)
                this.listViewContraints.Items.Remove(this.listViewContraints.SelectedItem);
        }

        /// <summary>
        /// Insert current constraints into given feature model.
        /// </summary>
        public AtlasFeatureModel UpdateConstraintEditor(AtlasFeatureModel atlas)
        {
            foreach (ConstraintGrid grid in this.listViewContraints.Items.OfType<ConstraintGrid>())
            {
                try
                {
                    atlas.AddConstraint(grid.GetContraintAsString());
                    TextBlock tb = new TextBlock();
                    tb.Text = grid.GetContraintAsString();

                }
                catch (Exception)
                {

                }

            }
            return atlas;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ListView contraintList()
        {
            return this.listViewContraints;
        }


        /// <summary>
        /// Adds a open parenthesis  to  selected constraint. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOpenParentesis_Click(object sender, RoutedEventArgs e)
        {
            TextBlock block = new TextBlock();
            block.Text = "(";
            block.Foreground = new SolidColorBrush(Colors.Green);
            block.Margin = new Thickness(5, 2, 5, 2);
            ConstraintGrid grid = this.listViewContraints.SelectedItem as ConstraintGrid;

            if (grid == null)
                return;

            grid.GetInnerContainer().Children.Add(block);
        }

        /// <summary>
        /// Adds a close parentheses to selected constraint
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCloseParentesis_Click(object sender, RoutedEventArgs e)
        {
            TextBlock block = new TextBlock();
            block.Text = ")";
            block.Foreground = new SolidColorBrush(Colors.Green);
            block.Margin = new Thickness(5, 2, 5, 2);
            ConstraintGrid grid = this.listViewContraints.SelectedItem as ConstraintGrid;

            if (grid == null)
                return;

            grid.GetInnerContainer().Children.Add(block);
        }


        /// <summary>
        /// Clear the Filter, in first time
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxFilter_GotFocus(object sender, RoutedEventArgs e)
        {
            textBoxFilter.Text = String.Empty;
        }

        /// <summary>
        /// foreach Contraints vefifed if is valid or not
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonValidate_Click(object sender, RoutedEventArgs e)
        {
            
                try
                {
                    if (radioButtonSat2.IsChecked == true)
                        sat2();
                    else if (radioButtonSat3.IsChecked == true)
                        sat3();
                    else
                        MessageBox.Show("Select Sat2 or Sat3 to start validation.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                catch (Exception ee)
                {
                    MessageBox.Show(ee.Message, ee.GetType().Name, 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                

        }

        /// <summary>
        /// Contraints
        /// </summary>
        private void sat2()
        {
            //procura por formulas dentro da lista
            foreach (ConstraintGrid g in listViewContraints.Items.OfType<ConstraintGrid>())
            {
                //para cada formula, monta a representacao em string
                String formula = "";
                foreach (TextBlock t in g.GetInnerContainer().Children)
                    formula += t.Text;

                //para cada disjuncao, valida
                String[] disjunctions = formula.Split('∧');
                foreach (String s in disjunctions)
                {

                    //Testa se a formula nao começa ou nao termina com parentese
                    if (!s.StartsWith("(") || !s.EndsWith(")"))
                    {
                        throw new ParentesisMismatchException("The " + formula + " is invalid. The Parentheses aren't open or close ");
                    }
                        int parentesis = 0;

                    //calcula quantos parentesis de abertura existem
                    parentesis = (from Char c in s.ToArray() where c.Equals('(') select c).Count();
                    if (parentesis != 1)
                    {

                        throw new ParentesisMismatchException("The " + formula + " is invalid. More  Parentheses are open ");
                        
                    }
                    //calcula quantos parentesis de fechamento existem
                    parentesis = (from Char c in s.ToArray() where c.Equals(')') select c).Count();
                    if (parentesis != 1)
                    {

                        throw new ParentesisMismatchException("The " + formula + " is invalid. More  Parentheses are close ");
                       
                    }
                    //remove os parentesis da formula 
                   string s2 = s.Substring(1, s.Length - 2);

                   String[] termos = s2.Split('∨');


                    //checa se existem 2 termos separados por disjuncao
                   if (termos.Count() != 2)
                   {
                       
                       throw new PropositionCountException("The " + formula + " is malformated.Proposiiton 'v' > 1");
                  
                   }
                    foreach (String s3 in termos)
                    {

                        if (s3.IndexOf('¬') > 1)
                        {
                            
                            throw new NotMalformatedException("The " + formula + " is malformated. Propositon not valid");
                        
                        }
   

                    }

                    int implies = (from Char c in s.ToArray() where c.Equals('⇒') select c).Count();
                    if (implies >= 1)
                    {

                        throw new ImplicationFoundException("The " + formula + " is malformated. Can not have Implication ");
                        
                    }
                
                }

                MessageBox.Show("Valido");
            }
        }
           
        
        

        private void sat3()
        {

            //procura por formulas dentro da lista
            foreach (ConstraintGrid g in listViewContraints.Items.OfType<ConstraintGrid>())
            {
                //para cada formula, monta a representacao em string
                String formula = "";
                foreach (TextBlock t in g.GetInnerContainer().Children)
                    formula += t.Text;

                //para cada disjuncao, valida
                String[] disjunctions = formula.Split('∧');
                foreach (String s in disjunctions)
                {

                    //Testa se a formula nao começa ou nao termina com parentese
                    if (!s.StartsWith("(") || !s.EndsWith(")"))
                    {
                        throw new ParentesisMismatchException("The " + formula + " is malformated. The Parentheses aren't open or close");
                    
                    }
                    int parentesis = 0;

                    //calcula quantos parentesis de abertura existem
                    parentesis = (from Char c in s.ToArray() where c.Equals('(') select c).Count();
                    if (parentesis != 1)
                        throw new ParentesisMismatchException("The " + formula + " is malformated.More Parentheses open");

                    //calcula quantos parentesis de fechamento existem
                    parentesis = (from Char c in s.ToArray() where c.Equals(')') select c).Count();
                    if (parentesis != 1)
                        throw new ParentesisMismatchException("The " + formula + " is malformated.More Parentheses close");

                    //remove os parentesis da formula 
                    string s2 = s.Substring(1, s.Length - 2);

                    String[] termos = s2.Split('∨');


                    //checa se existem 2 termos separados por disjuncao
                    if (termos.Count() != 3)
                        throw new PropositionCountException("The " + formula + " is malformated.Proposiiton 'v' > 2");

                    foreach (String s3 in termos)
                    {
                        if (s3.IndexOf('¬') > 1)
                        {
                            throw new NotMalformatedException("The " + formula + " is malformated.Propositon not valid");
                        }
                    }

                    int implies = (from Char c in s.ToArray() where c.Equals('⇒') select c).Count();
                    if (implies >= 1)
                    {
                        throw new ImplicationFoundException("The " + formula + " is malformated.Can not have Implication");
                    }

                }



                MessageBox.Show("Valid");
            }
        }


    }
    }



