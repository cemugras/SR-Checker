namespace SRChecker
{
    partial class CheckerForm
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
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.btnChecker = new System.Windows.Forms.Button();
            this.txtInputFilePath = new System.Windows.Forms.TextBox();
            this.cmbAction = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtResult = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnClean = new System.Windows.Forms.Button();
            this.dtGridView = new System.Windows.Forms.DataGridView();
            this.txtIPList = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnExport = new System.Windows.Forms.Button();
            this.prgBar = new System.Windows.Forms.ProgressBar();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.txtSRID = new System.Windows.Forms.TextBox();
            this.txtSROwner = new System.Windows.Forms.TextBox();
            this.pnlSRFileImport = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dtGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.pnlSRFileImport.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog
            // 
            this.openFileDialog.AddExtension = false;
            this.openFileDialog.AutoUpgradeEnabled = false;
            this.openFileDialog.DefaultExt = "xlsx";
            this.openFileDialog.FileName = "*.xlsx";
            this.openFileDialog.Filter = "Excel |*.xlsx\";";
            this.openFileDialog.RestoreDirectory = true;
            // 
            // btnChecker
            // 
            this.btnChecker.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnChecker.Location = new System.Drawing.Point(123, 208);
            this.btnChecker.Name = "btnChecker";
            this.btnChecker.Size = new System.Drawing.Size(247, 23);
            this.btnChecker.TabIndex = 0;
            this.btnChecker.Text = "Check Results";
            this.btnChecker.UseVisualStyleBackColor = true;
            this.btnChecker.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtInputFilePath
            // 
            this.txtInputFilePath.Location = new System.Drawing.Point(123, 177);
            this.txtInputFilePath.Name = "txtInputFilePath";
            this.txtInputFilePath.ReadOnly = true;
            this.txtInputFilePath.Size = new System.Drawing.Size(518, 20);
            this.txtInputFilePath.TabIndex = 1;
            // 
            // cmbAction
            // 
            this.cmbAction.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbAction.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
            this.cmbAction.FormattingEnabled = true;
            this.cmbAction.Items.AddRange(new object[] {
            "Firewall",
            "DNS",
            "Tracert",
            "SRFileImport",
            "SubnetCalculator"});
            this.cmbAction.Location = new System.Drawing.Point(29, 141);
            this.cmbAction.Name = "cmbAction";
            this.cmbAction.Size = new System.Drawing.Size(88, 90);
            this.cmbAction.TabIndex = 2;
            this.cmbAction.Text = "Select Action";
            this.cmbAction.SelectedIndexChanged += new System.EventHandler(this.cmbAction_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(123, 154);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(247, 23);
            this.label1.TabIndex = 6;
            this.label1.Text = "SR Request File Path";
            // 
            // txtResult
            // 
            this.txtResult.Location = new System.Drawing.Point(29, 264);
            this.txtResult.Multiline = true;
            this.txtResult.Name = "txtResult";
            this.txtResult.ReadOnly = true;
            this.txtResult.Size = new System.Drawing.Size(341, 89);
            this.txtResult.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(29, 245);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Result";
            // 
            // btnClean
            // 
            this.btnClean.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClean.Location = new System.Drawing.Point(394, 208);
            this.btnClean.Name = "btnClean";
            this.btnClean.Size = new System.Drawing.Size(247, 23);
            this.btnClean.TabIndex = 7;
            this.btnClean.Text = "Clear";
            this.btnClean.UseVisualStyleBackColor = true;
            this.btnClean.Click += new System.EventHandler(this.btnClean_Click);
            // 
            // dtGridView
            // 
            this.dtGridView.AllowUserToAddRows = false;
            this.dtGridView.AllowUserToDeleteRows = false;
            this.dtGridView.AllowUserToOrderColumns = true;
            this.dtGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dtGridView.Location = new System.Drawing.Point(32, 373);
            this.dtGridView.Name = "dtGridView";
            this.dtGridView.Size = new System.Drawing.Size(943, 150);
            this.dtGridView.TabIndex = 8;
            this.dtGridView.Visible = false;
            // 
            // txtIPList
            // 
            this.txtIPList.Location = new System.Drawing.Point(394, 264);
            this.txtIPList.Multiline = true;
            this.txtIPList.Name = "txtIPList";
            this.txtIPList.ReadOnly = true;
            this.txtIPList.Size = new System.Drawing.Size(247, 89);
            this.txtIPList.TabIndex = 9;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(391, 245);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Local IP Address";
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(394, 538);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(247, 23);
            this.btnExport.TabIndex = 10;
            this.btnExport.Text = "Export Results";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Visible = false;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // prgBar
            // 
            this.prgBar.Location = new System.Drawing.Point(394, 148);
            this.prgBar.Name = "prgBar";
            this.prgBar.Size = new System.Drawing.Size(247, 23);
            this.prgBar.TabIndex = 11;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::SRChecker.Properties.Resources.Mercury_K;
            this.pictureBox1.Location = new System.Drawing.Point(394, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(225, 118);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 12;
            this.pictureBox1.TabStop = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 3.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label4.Location = new System.Drawing.Point(629, 580);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(12, 6);
            this.label4.TabIndex = 14;
            this.label4.Text = "VM";
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.White;
            this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)), true);
            this.btnClose.ForeColor = System.Drawing.Color.Maroon;
            this.btnClose.Location = new System.Drawing.Point(900, 12);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 15;
            this.btnClose.Text = "Kapat";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // txtSRID
            // 
            this.txtSRID.Location = new System.Drawing.Point(22, 29);
            this.txtSRID.Name = "txtSRID";
            this.txtSRID.Size = new System.Drawing.Size(127, 20);
            this.txtSRID.TabIndex = 16;
            // 
            // txtSROwner
            // 
            this.txtSROwner.Location = new System.Drawing.Point(155, 29);
            this.txtSROwner.Name = "txtSROwner";
            this.txtSROwner.Size = new System.Drawing.Size(127, 20);
            this.txtSROwner.TabIndex = 17;
            // 
            // pnlSRFileImport
            // 
            this.pnlSRFileImport.Controls.Add(this.txtSROwner);
            this.pnlSRFileImport.Controls.Add(this.txtSRID);
            this.pnlSRFileImport.Controls.Add(this.label6);
            this.pnlSRFileImport.Controls.Add(this.label5);
            this.pnlSRFileImport.Location = new System.Drawing.Point(672, 148);
            this.pnlSRFileImport.Name = "pnlSRFileImport";
            this.pnlSRFileImport.Size = new System.Drawing.Size(303, 63);
            this.pnlSRFileImport.TabIndex = 18;
            this.pnlSRFileImport.Visible = false;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(152, 6);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(100, 23);
            this.label6.TabIndex = 6;
            this.label6.Text = "SR Request Owner";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(19, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(100, 23);
            this.label5.TabIndex = 6;
            this.label5.Text = "SR ID";
            // 
            // CheckerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1001, 595);
            this.Controls.Add(this.pnlSRFileImport);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.prgBar);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.txtIPList);
            this.Controls.Add(this.dtGridView);
            this.Controls.Add(this.btnClean);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtResult);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbAction);
            this.Controls.Add(this.txtInputFilePath);
            this.Controls.Add(this.btnChecker);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "CheckerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Mercury SR Checker";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dtGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.pnlSRFileImport.ResumeLayout(false);
            this.pnlSRFileImport.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnChecker;
        private System.Windows.Forms.TextBox txtInputFilePath;
        private System.Windows.Forms.ComboBox cmbAction;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtResult;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnClean;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.DataGridView dtGridView;
        private System.Windows.Forms.TextBox txtIPList;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.ProgressBar prgBar;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.TextBox txtSRID;
        private System.Windows.Forms.TextBox txtSROwner;
        private System.Windows.Forms.Panel pnlSRFileImport;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
    }
}

