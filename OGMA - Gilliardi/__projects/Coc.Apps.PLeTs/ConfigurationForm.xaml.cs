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
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Globalization;
using Coc.Data.Xmi.Script;
using System.Text.RegularExpressions;
//using Coc.Data.Xmi.Script;

namespace Coc.Apps.PleTs
{
    /// <summary>
    /// Interaction logic for ConfigurationForm.xaml
    /// </summary>
    public partial class ConfigurationForm : Window
    {
        public ConfigurationForm()
        {

            InitializeComponent();

            //cmbColor.ShowAdvancedButton = true;
        }

        private void button_Salvar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Configuration c = Configuration.getInstance();

                c.setConfiguration(Configuration.Fields.waittime, txtThinkTime.Text);
                c.setConfiguration(Configuration.Fields.astahpath, txtAstahPath.Text);
                c.setConfiguration(Configuration.Fields.oatspath, txtOATSPath.Text);
                c.setConfiguration(Configuration.Fields.workspacepath, txtWorkspacePath.Text);
               c.setConfiguration(Configuration.Fields.verifyall, chkVerifyAll.IsChecked.ToString());

                //c.setConfiguration(Configuration.Fields.actioncolor, 
                  //  "#" +
                    //cmbColor.SelectedColor.Value.A.ToString("X").PadLeft(2, '0') +
                    //cmbColor.SelectedColor.Value.R.ToString("X").PadLeft(2, '0') +
                    //cmbColor.SelectedColor.Value.G.ToString("X").PadLeft(2, '0') +
                    //cmbColor.SelectedColor.Value.B.ToString("X").PadLeft(2, '0')
             

                c.setConfiguration(Configuration.Fields.actionlist, txtActions.Text);

                if (c.writeConfigFile())
                {
                    System.Windows.Forms.MessageBox.Show(
                        "Config file saved!", this.Title, 
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show(
                        "Error saving Config file!", this.Title, 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                this.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }

        }

        private void configurationForm_Loaded(object sender, RoutedEventArgs e){
            try
            {

                #region FIELDS
                txtThinkTime.Text = Configuration.getInstance().getConfiguration(Configuration.Fields.waittime);//.ToString("0.000"), CultureInfo.GetCultureInfo("EN-US"));
                ThinkTime_slider.Value = Convert.ToDouble(txtThinkTime.Text, CultureInfo.GetCultureInfo("EN-US"));
                txtAstahPath.Text = Configuration.getInstance().getConfiguration(Configuration.Fields.astahpath);
                txtOATSPath.Text = Configuration.getInstance().getConfiguration(Configuration.Fields.oatspath);
                txtWorkspacePath.Text = Configuration.getInstance().getConfiguration(Configuration.Fields.workspacepath);
                txtActions.Text = Configuration.getInstance().getConfiguration(Configuration.Fields.actionlist);
                #endregion
                
                #region COLOR
                {
                    //byte a = 0;
                    byte r = 0;
                    byte g = 0;
                    byte b = 0;
                    string color = Configuration.getInstance().getConfiguration(Configuration.Fields.actioncolor);
                    if (!string.IsNullOrEmpty(color))
                    {
                        color = color.Replace("#", "");

                        //a = byte.Parse(color.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                        r = byte.Parse(color.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                        g = byte.Parse(color.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                        b = byte.Parse(color.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

                    }

                    //cmbColor.SelectedColor = Color.FromRgb(r, g, b);
                }
                #endregion

                #region VERIFYALL
                string val = Configuration.getInstance().getConfiguration(Configuration.Fields.verifyall);
                bool result = false;
                if (bool.TryParse(val, out result))
                {
                    chkVerifyAll.IsChecked = result;
                }
                #endregion
                

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        private void txtThinkTime_LostFocus(object sender, RoutedEventArgs e)
        {
            ThinkTime_slider.Value = Convert.ToDouble(txtThinkTime.Text, CultureInfo.GetCultureInfo("EN-US"));
        }

        private void button_Cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void ThinkTime_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (txtThinkTime != null)
            {
                txtThinkTime.Text = ThinkTime_slider.Value.ToString("0.000", CultureInfo.GetCultureInfo("EN-US"));
            }
        }

        private void btnSrchAstah_Click(object sender, RoutedEventArgs e)
        {
            txtAstahPath.Text = openChooseFolderDialog();
        }

        private void btnSrchScrptOutput_Click(object sender, RoutedEventArgs e)
        {
            txtWorkspacePath.Text = openChooseFolderDialog();
        }

        private void btnSrchOATSPath_Click(object sender, RoutedEventArgs e)
        {
            txtOATSPath.Text = openChooseFolderDialog(); 
        }



        private void cmbColor_SelectedColorChanged(object sender, RoutedEventArgs e)
        {
            /*
             txtColor.Text = "#" + 
                cmbColor.SelectedColor.Value.R.ToString("X") +
                cmbColor.SelectedColor.Value.G.ToString("X") +
                cmbColor.SelectedColor.Value.B.ToString("X");
             */
        }


        private string openChooseFolderDialog()
        {
            string selectedFolder = "";
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK )
            {
                selectedFolder = fbd.SelectedPath;
                fbd.Dispose();
            }

            return selectedFolder;
        }
                
    }
}
