using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Coc.Data.Xmi
{
    public partial class DatabankConfigForm : Form
    {
        
        private bool okPressed;


        public DatabankConfigForm(string databankFileName)
        {
            if (string.IsNullOrEmpty(databankFileName))
            {
                throw new Exception("Invalid Databank file name: " + databankFileName);
            }

            InitializeComponent();
            okPressed = false;

            if(databankFileName.Contains('.')){
                databankFileName = databankFileName.Substring(0, databankFileName.IndexOf('.'));
            }

            Name = databankFileName;
        }




        private void btnSrchRepository_Click(object sender, EventArgs e)
        {
            openDialog("");
        }

        private void btnSrchDatabank_Click(object sender, EventArgs e)
        {
            openDialog("");
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            okPressed = true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {

        }

        private void openDialog(string filter)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = filter;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get; set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Repository
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string DatabankPath
        {
            get;
            set;
        }
        
        
        
        
        public void capture()
        {
            //enquanto nao adicionou os valores
            while (!okPressed)
            {
                //aguarda tempo
                System.Threading.Thread.Sleep(5000);
            }


            //Name = txtRepository.Text;
            Repository = txtRepository.Text;
            DatabankPath = txtDatabankPath.Text;

            //verifica os valores e ajusta as propriedades
            if (string.IsNullOrEmpty(Repository))
            {
                Repository = "Default";
            }

            if (string.IsNullOrEmpty(DatabankPath))
            {
                DatabankPath = "\\DataBank\\file.csv";
            }


            //retorna para ScriptParser

        }

        private void DatabankConfigForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            capture();
        }

    }
}
