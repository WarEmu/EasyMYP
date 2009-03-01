namespace EasyMYP
{
    partial class FileTester
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.customDirLabel = new System.Windows.Forms.Label();
            this.chooseDirbutton = new System.Windows.Forms.Button();
            this.customDirBox = new System.Windows.Forms.CheckBox();
            this.knownDirBox = new System.Windows.Forms.CheckBox();
            this.chooseFileButton = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.customFileLabel = new System.Windows.Forms.Label();
            this.customFileBox = new System.Windows.Forms.CheckBox();
            this.knownFileBox = new System.Windows.Forms.CheckBox();
            this.chooseExtButton = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.customExtLabel = new System.Windows.Forms.Label();
            this.customExtBox = new System.Windows.Forms.CheckBox();
            this.knownExtBox = new System.Windows.Forms.CheckBox();
            this.goButton = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.customDirLabel);
            this.groupBox1.Controls.Add(this.chooseDirbutton);
            this.groupBox1.Controls.Add(this.customDirBox);
            this.groupBox1.Controls.Add(this.knownDirBox);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(349, 84);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Directories";
            // 
            // customDirLabel
            // 
            this.customDirLabel.AutoSize = true;
            this.customDirLabel.Location = new System.Drawing.Point(6, 64);
            this.customDirLabel.Name = "customDirLabel";
            this.customDirLabel.Size = new System.Drawing.Size(103, 13);
            this.customDirLabel.TabIndex = 3;
            this.customDirLabel.Text = "Custom directory list:";
            // 
            // chooseDirbutton
            // 
            this.chooseDirbutton.Location = new System.Drawing.Point(147, 40);
            this.chooseDirbutton.Name = "chooseDirbutton";
            this.chooseDirbutton.Size = new System.Drawing.Size(75, 23);
            this.chooseDirbutton.TabIndex = 2;
            this.chooseDirbutton.Text = "Choose...";
            this.chooseDirbutton.UseVisualStyleBackColor = true;
            this.chooseDirbutton.Click += new System.EventHandler(this.chooseDirbutton_Click);
            // 
            // customDirBox
            // 
            this.customDirBox.AutoSize = true;
            this.customDirBox.Location = new System.Drawing.Point(7, 44);
            this.customDirBox.Name = "customDirBox";
            this.customDirBox.Size = new System.Drawing.Size(119, 17);
            this.customDirBox.TabIndex = 1;
            this.customDirBox.Text = "Custom directory list";
            this.customDirBox.UseVisualStyleBackColor = true;
            // 
            // knownDirBox
            // 
            this.knownDirBox.AutoSize = true;
            this.knownDirBox.Location = new System.Drawing.Point(7, 20);
            this.knownDirBox.Name = "knownDirBox";
            this.knownDirBox.Size = new System.Drawing.Size(110, 17);
            this.knownDirBox.TabIndex = 0;
            this.knownDirBox.Text = "Known directories";
            this.knownDirBox.UseVisualStyleBackColor = true;
            // 
            // chooseFileButton
            // 
            this.chooseFileButton.Location = new System.Drawing.Point(147, 38);
            this.chooseFileButton.Name = "chooseFileButton";
            this.chooseFileButton.Size = new System.Drawing.Size(75, 23);
            this.chooseFileButton.TabIndex = 2;
            this.chooseFileButton.Text = "Choose...";
            this.chooseFileButton.UseVisualStyleBackColor = true;
            this.chooseFileButton.Click += new System.EventHandler(this.chooseFileButton_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.customFileLabel);
            this.groupBox2.Controls.Add(this.chooseFileButton);
            this.groupBox2.Controls.Add(this.customFileBox);
            this.groupBox2.Controls.Add(this.knownFileBox);
            this.groupBox2.Location = new System.Drawing.Point(12, 102);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(349, 84);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Files";
            // 
            // customFileLabel
            // 
            this.customFileLabel.AutoSize = true;
            this.customFileLabel.Location = new System.Drawing.Point(6, 62);
            this.customFileLabel.Name = "customFileLabel";
            this.customFileLabel.Size = new System.Drawing.Size(76, 13);
            this.customFileLabel.TabIndex = 4;
            this.customFileLabel.Text = "Custom file list:";
            // 
            // customFileBox
            // 
            this.customFileBox.AutoSize = true;
            this.customFileBox.Location = new System.Drawing.Point(6, 42);
            this.customFileBox.Name = "customFileBox";
            this.customFileBox.Size = new System.Drawing.Size(92, 17);
            this.customFileBox.TabIndex = 1;
            this.customFileBox.Text = "Custom file list";
            this.customFileBox.UseVisualStyleBackColor = true;
            // 
            // knownFileBox
            // 
            this.knownFileBox.AutoSize = true;
            this.knownFileBox.Location = new System.Drawing.Point(7, 19);
            this.knownFileBox.Name = "knownFileBox";
            this.knownFileBox.Size = new System.Drawing.Size(80, 17);
            this.knownFileBox.TabIndex = 0;
            this.knownFileBox.Text = "Known files";
            this.knownFileBox.UseVisualStyleBackColor = true;
            // 
            // chooseExtButton
            // 
            this.chooseExtButton.Location = new System.Drawing.Point(147, 38);
            this.chooseExtButton.Name = "chooseExtButton";
            this.chooseExtButton.Size = new System.Drawing.Size(75, 23);
            this.chooseExtButton.TabIndex = 2;
            this.chooseExtButton.Text = "Choose...";
            this.chooseExtButton.UseVisualStyleBackColor = true;
            this.chooseExtButton.Click += new System.EventHandler(this.chooseExtButton_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.customExtLabel);
            this.groupBox3.Controls.Add(this.chooseExtButton);
            this.groupBox3.Controls.Add(this.customExtBox);
            this.groupBox3.Controls.Add(this.knownExtBox);
            this.groupBox3.Location = new System.Drawing.Point(12, 192);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(349, 84);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Extensions";
            // 
            // customExtLabel
            // 
            this.customExtLabel.AutoSize = true;
            this.customExtLabel.Location = new System.Drawing.Point(6, 62);
            this.customExtLabel.Name = "customExtLabel";
            this.customExtLabel.Size = new System.Drawing.Size(108, 13);
            this.customExtLabel.TabIndex = 4;
            this.customExtLabel.Text = "Custom extension list:";
            // 
            // customExtBox
            // 
            this.customExtBox.AutoSize = true;
            this.customExtBox.Location = new System.Drawing.Point(6, 42);
            this.customExtBox.Name = "customExtBox";
            this.customExtBox.Size = new System.Drawing.Size(124, 17);
            this.customExtBox.TabIndex = 1;
            this.customExtBox.Text = "Custom extension list";
            this.customExtBox.UseVisualStyleBackColor = true;
            // 
            // knownExtBox
            // 
            this.knownExtBox.AutoSize = true;
            this.knownExtBox.Location = new System.Drawing.Point(7, 19);
            this.knownExtBox.Name = "knownExtBox";
            this.knownExtBox.Size = new System.Drawing.Size(112, 17);
            this.knownExtBox.TabIndex = 0;
            this.knownExtBox.Text = "Known extensions";
            this.knownExtBox.UseVisualStyleBackColor = true;
            // 
            // goButton
            // 
            this.goButton.Location = new System.Drawing.Point(286, 282);
            this.goButton.Name = "goButton";
            this.goButton.Size = new System.Drawing.Size(75, 23);
            this.goButton.TabIndex = 6;
            this.goButton.Text = "Go!";
            this.goButton.UseVisualStyleBackColor = true;
            this.goButton.Click += new System.EventHandler(this.goButton_Click);
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Location = new System.Drawing.Point(205, 282);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // FileTester
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(375, 313);
            this.ControlBox = false;
            this.Controls.Add(this.button1);
            this.Controls.Add(this.goButton);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "FileTester";
            this.ShowInTaskbar = false;
            this.Text = "File Tester";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label customDirLabel;
        private System.Windows.Forms.Button chooseDirbutton;
        private System.Windows.Forms.Button chooseFileButton;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label customFileLabel;
        private System.Windows.Forms.Button chooseExtButton;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label customExtLabel;
        private System.Windows.Forms.Button goButton;
        private System.Windows.Forms.Button button1;
        public System.Windows.Forms.CheckBox knownDirBox;
        public System.Windows.Forms.CheckBox customDirBox;
        public System.Windows.Forms.CheckBox customFileBox;
        public System.Windows.Forms.CheckBox knownFileBox;
        public System.Windows.Forms.CheckBox customExtBox;
        public System.Windows.Forms.CheckBox knownExtBox;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}