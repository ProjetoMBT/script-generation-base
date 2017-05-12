namespace Coc.Data.Xmi
{
    partial class DatabankConfigForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnSrchDatabank = new System.Windows.Forms.Button();
            this.btnSrchRepository = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtRepository = new System.Windows.Forms.TextBox();
            this.txtDatabankPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblDatabankFile = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnSrchDatabank
            // 
            this.btnSrchDatabank.Location = new System.Drawing.Point(347, 69);
            this.btnSrchDatabank.Name = "btnSrchDatabank";
            this.btnSrchDatabank.Size = new System.Drawing.Size(25, 23);
            this.btnSrchDatabank.TabIndex = 0;
            this.btnSrchDatabank.Text = "...";
            this.btnSrchDatabank.UseVisualStyleBackColor = true;
            this.btnSrchDatabank.Click += new System.EventHandler(this.btnSrchDatabank_Click);
            // 
            // btnSrchRepository
            // 
            this.btnSrchRepository.Location = new System.Drawing.Point(347, 43);
            this.btnSrchRepository.Name = "btnSrchRepository";
            this.btnSrchRepository.Size = new System.Drawing.Size(25, 23);
            this.btnSrchRepository.TabIndex = 1;
            this.btnSrchRepository.Text = "...";
            this.btnSrchRepository.UseVisualStyleBackColor = true;
            this.btnSrchRepository.Click += new System.EventHandler(this.btnSrchRepository_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(216, 97);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(297, 97);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // txtRepository
            // 
            this.txtRepository.Location = new System.Drawing.Point(90, 45);
            this.txtRepository.Name = "txtRepository";
            this.txtRepository.Size = new System.Drawing.Size(251, 20);
            this.txtRepository.TabIndex = 4;
            // 
            // txtDatabankPath
            // 
            this.txtDatabankPath.Location = new System.Drawing.Point(90, 71);
            this.txtDatabankPath.Name = "txtDatabankPath";
            this.txtDatabankPath.Size = new System.Drawing.Size(251, 20);
            this.txtDatabankPath.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Repository";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Databank Path";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Databank File";
            // 
            // lblDatabankFile
            // 
            this.lblDatabankFile.AutoSize = true;
            this.lblDatabankFile.Location = new System.Drawing.Point(92, 24);
            this.lblDatabankFile.Name = "lblDatabankFile";
            this.lblDatabankFile.Size = new System.Drawing.Size(16, 13);
            this.lblDatabankFile.TabIndex = 9;
            this.lblDatabankFile.Text = "...";
            // 
            // DatabankConfigForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(382, 132);
            this.Controls.Add(this.lblDatabankFile);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtDatabankPath);
            this.Controls.Add(this.txtRepository);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnSrchRepository);
            this.Controls.Add(this.btnSrchDatabank);
            this.MaximizeBox = false;
            this.Name = "DatabankConfigForm";
            this.Text = "Databank";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DatabankConfigForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSrchDatabank;
        private System.Windows.Forms.Button btnSrchRepository;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtRepository;
        private System.Windows.Forms.TextBox txtDatabankPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblDatabankFile;
    }
}