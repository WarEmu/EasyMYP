namespace EasyMYP
{
    partial class Preferences
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
            this.groupBox_EasyMYPConfig = new System.Windows.Forms.GroupBox();
            this.groupBox_HashCreator = new System.Windows.Forms.GroupBox();
            this.groupBox_MYPHandler = new System.Windows.Forms.GroupBox();
            this.checkBox_MultiThreadedExtraction = new System.Windows.Forms.CheckBox();
            this.button_Cancel = new System.Windows.Forms.Button();
            this.button_Save = new System.Windows.Forms.Button();
            this.dropdownlist_MaxOperationThread = new System.Windows.Forms.ComboBox();
            this.label_MaxOperationThread = new System.Windows.Forms.Label();
            this.textBox_ExtractionPath = new System.Windows.Forms.TextBox();
            this.button_SetExtractionPath = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.groupBox_EasyMYPConfig.SuspendLayout();
            this.groupBox_HashCreator.SuspendLayout();
            this.groupBox_MYPHandler.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox_EasyMYPConfig
            // 
            this.groupBox_EasyMYPConfig.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox_EasyMYPConfig.Controls.Add(this.button_SetExtractionPath);
            this.groupBox_EasyMYPConfig.Controls.Add(this.textBox_ExtractionPath);
            this.groupBox_EasyMYPConfig.Location = new System.Drawing.Point(12, 12);
            this.groupBox_EasyMYPConfig.Name = "groupBox_EasyMYPConfig";
            this.groupBox_EasyMYPConfig.Size = new System.Drawing.Size(409, 100);
            this.groupBox_EasyMYPConfig.TabIndex = 0;
            this.groupBox_EasyMYPConfig.TabStop = false;
            this.groupBox_EasyMYPConfig.Text = "Configuration - EasyMYP";
            // 
            // groupBox_HashCreator
            // 
            this.groupBox_HashCreator.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox_HashCreator.Controls.Add(this.label_MaxOperationThread);
            this.groupBox_HashCreator.Controls.Add(this.dropdownlist_MaxOperationThread);
            this.groupBox_HashCreator.Location = new System.Drawing.Point(12, 118);
            this.groupBox_HashCreator.Name = "groupBox_HashCreator";
            this.groupBox_HashCreator.Size = new System.Drawing.Size(409, 100);
            this.groupBox_HashCreator.TabIndex = 1;
            this.groupBox_HashCreator.TabStop = false;
            this.groupBox_HashCreator.Text = "Configuration - Hash Creator";
            // 
            // groupBox_MYPHandler
            // 
            this.groupBox_MYPHandler.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox_MYPHandler.Controls.Add(this.checkBox_MultiThreadedExtraction);
            this.groupBox_MYPHandler.Location = new System.Drawing.Point(12, 224);
            this.groupBox_MYPHandler.Name = "groupBox_MYPHandler";
            this.groupBox_MYPHandler.Size = new System.Drawing.Size(409, 103);
            this.groupBox_MYPHandler.TabIndex = 2;
            this.groupBox_MYPHandler.TabStop = false;
            this.groupBox_MYPHandler.Text = "Configuration - MYP Handler";
            // 
            // checkBox_MultiThreadedExtraction
            // 
            this.checkBox_MultiThreadedExtraction.AutoSize = true;
            this.checkBox_MultiThreadedExtraction.Location = new System.Drawing.Point(6, 19);
            this.checkBox_MultiThreadedExtraction.Name = "checkBox_MultiThreadedExtraction";
            this.checkBox_MultiThreadedExtraction.Size = new System.Drawing.Size(137, 17);
            this.checkBox_MultiThreadedExtraction.TabIndex = 0;
            this.checkBox_MultiThreadedExtraction.Text = "MultiThread Extractions";
            this.checkBox_MultiThreadedExtraction.UseVisualStyleBackColor = true;
            // 
            // button_Cancel
            // 
            this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_Cancel.Location = new System.Drawing.Point(265, 343);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.Size = new System.Drawing.Size(75, 23);
            this.button_Cancel.TabIndex = 9;
            this.button_Cancel.Text = "Cancel";
            this.button_Cancel.UseVisualStyleBackColor = true;
            // 
            // button_Save
            // 
            this.button_Save.Location = new System.Drawing.Point(346, 343);
            this.button_Save.Name = "button_Save";
            this.button_Save.Size = new System.Drawing.Size(75, 23);
            this.button_Save.TabIndex = 8;
            this.button_Save.Text = "Save";
            this.button_Save.UseVisualStyleBackColor = true;
            this.button_Save.Click += new System.EventHandler(this.button_Save_Click);
            // 
            // dropdownlist_MaxOperationThread
            // 
            this.dropdownlist_MaxOperationThread.FormattingEnabled = true;
            this.dropdownlist_MaxOperationThread.Location = new System.Drawing.Point(282, 16);
            this.dropdownlist_MaxOperationThread.Name = "dropdownlist_MaxOperationThread";
            this.dropdownlist_MaxOperationThread.Size = new System.Drawing.Size(121, 21);
            this.dropdownlist_MaxOperationThread.TabIndex = 0;
            // 
            // label_MaxOperationThread
            // 
            this.label_MaxOperationThread.AutoSize = true;
            this.label_MaxOperationThread.Location = new System.Drawing.Point(7, 19);
            this.label_MaxOperationThread.Name = "label_MaxOperationThread";
            this.label_MaxOperationThread.Size = new System.Drawing.Size(236, 13);
            this.label_MaxOperationThread.TabIndex = 1;
            this.label_MaxOperationThread.Text = "Maximum number of threads when brute forcing :";
            // 
            // textBox_ExtractionPath
            // 
            this.textBox_ExtractionPath.Enabled = false;
            this.textBox_ExtractionPath.Location = new System.Drawing.Point(87, 19);
            this.textBox_ExtractionPath.Name = "textBox_ExtractionPath";
            this.textBox_ExtractionPath.Size = new System.Drawing.Size(316, 20);
            this.textBox_ExtractionPath.TabIndex = 0;
            // 
            // button_SetExtractionPath
            // 
            this.button_SetExtractionPath.Location = new System.Drawing.Point(6, 17);
            this.button_SetExtractionPath.Name = "button_SetExtractionPath";
            this.button_SetExtractionPath.Size = new System.Drawing.Size(75, 23);
            this.button_SetExtractionPath.TabIndex = 1;
            this.button_SetExtractionPath.Text = "Set Extraction Path";
            this.button_SetExtractionPath.UseVisualStyleBackColor = true;
            this.button_SetExtractionPath.Click += new System.EventHandler(this.button_SetExtractionPath_Click);
            // 
            // Preferences
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button_Cancel;
            this.ClientSize = new System.Drawing.Size(433, 378);
            this.Controls.Add(this.button_Cancel);
            this.Controls.Add(this.button_Save);
            this.Controls.Add(this.groupBox_MYPHandler);
            this.Controls.Add(this.groupBox_HashCreator);
            this.Controls.Add(this.groupBox_EasyMYPConfig);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Preferences";
            this.Text = "Preferences";
            this.groupBox_EasyMYPConfig.ResumeLayout(false);
            this.groupBox_EasyMYPConfig.PerformLayout();
            this.groupBox_HashCreator.ResumeLayout(false);
            this.groupBox_HashCreator.PerformLayout();
            this.groupBox_MYPHandler.ResumeLayout(false);
            this.groupBox_MYPHandler.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox_EasyMYPConfig;
        private System.Windows.Forms.GroupBox groupBox_HashCreator;
        private System.Windows.Forms.GroupBox groupBox_MYPHandler;
        private System.Windows.Forms.CheckBox checkBox_MultiThreadedExtraction;
        private System.Windows.Forms.Button button_Cancel;
        private System.Windows.Forms.Button button_Save;
        private System.Windows.Forms.ComboBox dropdownlist_MaxOperationThread;
        private System.Windows.Forms.Label label_MaxOperationThread;
        private System.Windows.Forms.Button button_SetExtractionPath;
        private System.Windows.Forms.TextBox textBox_ExtractionPath;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
    }
}