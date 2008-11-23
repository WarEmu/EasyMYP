namespace EasyMYP
{
    partial class MainWindow
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openArchiveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeArchiveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.selectExtractionFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extractSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extractAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.replaceSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mergeDictionaryFile_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testFilenameListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testFullFilenameListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tryAgainToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openArchiveDialog = new System.Windows.Forms.OpenFileDialog();
            this.panel_output = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.findStrip2 = new EasyMYP.FindStrip();
            this.fileInArchiveBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.fileInArchiveBindingNavigator = new System.Windows.Forms.BindingNavigator(this.components);
            this.bindingNavigatorAddNewItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorCountItem = new System.Windows.Forms.ToolStripLabel();
            this.bindingNavigatorDeleteItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMoveFirstItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMovePreviousItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorPositionItem = new System.Windows.Forms.ToolStripTextBox();
            this.bindingNavigatorSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorMoveNextItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMoveLastItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.fileInArchiveBindingNavigatorSaveItem = new System.Windows.Forms.ToolStripButton();
            this.Pattern = new System.Windows.Forms.TextBox();
            this.LabelFilter = new System.Windows.Forms.Label();
            this.fileInArchiveDataGridView = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeView_Archive = new System.Windows.Forms.TreeView();
            this.treeView_FileSystem = new System.Windows.Forms.TreeView();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonGenerateFilenames_OnPattern = new System.Windows.Forms.Button();
            this.buttonGenerateFilenames_CurrentFiles = new System.Windows.Forms.Button();
            this.buttonExtractModifiedFiles = new System.Windows.Forms.Button();
            this.buttonExtractNewFiles = new System.Windows.Forms.Button();
            this.label_ModifiedFiles_Value = new System.Windows.Forms.Label();
            this.label_ModifiedFiles_Text = new System.Windows.Forms.Label();
            this.label_NewFiles_Value = new System.Windows.Forms.Label();
            this.label_NewFiles_Text = new System.Windows.Forms.Label();
            this.panel_status = new System.Windows.Forms.Panel();
            this.Loading = new System.Windows.Forms.ProgressBar();
            this.credits = new System.Windows.Forms.Label();
            this.panel_information = new System.Windows.Forms.Panel();
            this.label_File_Text = new System.Windows.Forms.Label();
            this.label_ExtractedFiles_Value = new System.Windows.Forms.Label();
            this.label_NumOfNamedFiles_Value = new System.Windows.Forms.Label();
            this.label_ExtractedFiles_Text = new System.Windows.Forms.Label();
            this.label_EstimatedNumOfFiles_Text = new System.Windows.Forms.Label();
            this.label_ExtractionErrors_Value = new System.Windows.Forms.Label();
            this.label_NumOfNamedFiles_Text = new System.Windows.Forms.Label();
            this.label_ExtractionErrors_Text = new System.Windows.Forms.Label();
            this.label_EstimatedNumOfFiles_Value = new System.Windows.Forms.Label();
            this.label_File_Value = new System.Windows.Forms.Label();
            this.label_UncompressedSize_Text = new System.Windows.Forms.Label();
            this.label_NumOfFiles_Value = new System.Windows.Forms.Label();
            this.label_NumOfFiles_Text = new System.Windows.Forms.Label();
            this.label_ReadingErrors_Text = new System.Windows.Forms.Label();
            this.label_UncompressedSize_Value = new System.Windows.Forms.Label();
            this.label_ReadingErrors_Value = new System.Windows.Forms.Label();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.replaceFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.lblGeneratePat = new System.Windows.Forms.Label();
            this.lblGenerateFNE = new System.Windows.Forms.Label();
            this.mainMenu.SuspendLayout();
            this.panel_output.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.findStrip2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fileInArchiveBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fileInArchiveBindingNavigator)).BeginInit();
            this.fileInArchiveBindingNavigator.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fileInArchiveDataGridView)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.panel_status.SuspendLayout();
            this.panel_information.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu
            // 
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.fileToolStripMenuItem1,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new System.Drawing.Size(905, 24);
            this.mainMenu.TabIndex = 0;
            this.mainMenu.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openArchiveToolStripMenuItem,
            this.createToolStripMenuItem,
            this.closeArchiveToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(55, 20);
            this.fileToolStripMenuItem.Text = "Archive";
            // 
            // openArchiveToolStripMenuItem
            // 
            this.openArchiveToolStripMenuItem.Name = "openArchiveToolStripMenuItem";
            this.openArchiveToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.openArchiveToolStripMenuItem.Text = "Open Archive";
            this.openArchiveToolStripMenuItem.Click += new System.EventHandler(this.openArchiveToolStripMenuItem_Click);
            // 
            // createToolStripMenuItem
            // 
            this.createToolStripMenuItem.Enabled = false;
            this.createToolStripMenuItem.Name = "createToolStripMenuItem";
            this.createToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.createToolStripMenuItem.Text = "Create Archive";
            this.createToolStripMenuItem.Click += new System.EventHandler(this.createToolStripMenuItem_Click);
            // 
            // closeArchiveToolStripMenuItem
            // 
            this.closeArchiveToolStripMenuItem.Name = "closeArchiveToolStripMenuItem";
            this.closeArchiveToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.closeArchiveToolStripMenuItem.Text = "Close Archive";
            this.closeArchiveToolStripMenuItem.Click += new System.EventHandler(this.closeArchiveToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // fileToolStripMenuItem1
            // 
            this.fileToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectExtractionFolderToolStripMenuItem,
            this.extractSelectedToolStripMenuItem,
            this.extractAllToolStripMenuItem,
            this.replaceSelectedToolStripMenuItem});
            this.fileToolStripMenuItem1.Name = "fileToolStripMenuItem1";
            this.fileToolStripMenuItem1.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem1.Text = "File";
            // 
            // selectExtractionFolderToolStripMenuItem
            // 
            this.selectExtractionFolderToolStripMenuItem.Name = "selectExtractionFolderToolStripMenuItem";
            this.selectExtractionFolderToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.selectExtractionFolderToolStripMenuItem.Text = "Select Extraction Folder";
            this.selectExtractionFolderToolStripMenuItem.Click += new System.EventHandler(this.selectExtractionFolderToolStripMenuItem_Click);
            // 
            // extractSelectedToolStripMenuItem
            // 
            this.extractSelectedToolStripMenuItem.Name = "extractSelectedToolStripMenuItem";
            this.extractSelectedToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.extractSelectedToolStripMenuItem.Text = "Extract Selected";
            this.extractSelectedToolStripMenuItem.Click += new System.EventHandler(this.extractSelectedToolStripMenuItem_Click);
            // 
            // extractAllToolStripMenuItem
            // 
            this.extractAllToolStripMenuItem.Name = "extractAllToolStripMenuItem";
            this.extractAllToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.extractAllToolStripMenuItem.Text = "Extract All";
            this.extractAllToolStripMenuItem.Click += new System.EventHandler(this.extractAllToolStripMenuItem_Click);
            // 
            // replaceSelectedToolStripMenuItem
            // 
            this.replaceSelectedToolStripMenuItem.Name = "replaceSelectedToolStripMenuItem";
            this.replaceSelectedToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.replaceSelectedToolStripMenuItem.Text = "Replace Selected";
            this.replaceSelectedToolStripMenuItem.Click += new System.EventHandler(this.replaceSelectedToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mergeDictionaryFile_ToolStripMenuItem,
            this.testFilenameListToolStripMenuItem,
            this.testFullFilenameListToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // mergeDictionaryFile_ToolStripMenuItem
            // 
            this.mergeDictionaryFile_ToolStripMenuItem.Name = "mergeDictionaryFile_ToolStripMenuItem";
            this.mergeDictionaryFile_ToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.mergeDictionaryFile_ToolStripMenuItem.Text = "Merge Dictionary File";
            this.mergeDictionaryFile_ToolStripMenuItem.Click += new System.EventHandler(this.mergeDictionaryFile_ToolStripMenuItem_Click);
            // 
            // testFilenameListToolStripMenuItem
            // 
            this.testFilenameListToolStripMenuItem.Name = "testFilenameListToolStripMenuItem";
            this.testFilenameListToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.testFilenameListToolStripMenuItem.Text = "Test Filename List";
            this.testFilenameListToolStripMenuItem.Click += new System.EventHandler(this.testFilenameListToolStripMenuItem_Click);
            // 
            // testFullFilenameListToolStripMenuItem
            // 
            this.testFullFilenameListToolStripMenuItem.Name = "testFullFilenameListToolStripMenuItem";
            this.testFullFilenameListToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.testFullFilenameListToolStripMenuItem.Text = "Test Full Filename List";
            this.testFullFilenameListToolStripMenuItem.Click += new System.EventHandler(this.testFullFilenameListToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tryAgainToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // tryAgainToolStripMenuItem
            // 
            this.tryAgainToolStripMenuItem.Name = "tryAgainToolStripMenuItem";
            this.tryAgainToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.tryAgainToolStripMenuItem.Text = "To Be Coded ...";
            // 
            // openArchiveDialog
            // 
            this.openArchiveDialog.Filter = "(*.myp)|*.myp";
            // 
            // panel_output
            // 
            this.panel_output.AutoScroll = true;
            this.panel_output.AutoSize = true;
            this.panel_output.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel_output.Controls.Add(this.tabControl1);
            this.panel_output.Controls.Add(this.panel_status);
            this.panel_output.Controls.Add(this.panel_information);
            this.panel_output.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_output.Location = new System.Drawing.Point(0, 24);
            this.panel_output.MinimumSize = new System.Drawing.Size(600, 300);
            this.panel_output.Name = "panel_output";
            this.panel_output.Size = new System.Drawing.Size(905, 472);
            this.panel_output.TabIndex = 27;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(12, 69);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(881, 374);
            this.tabControl1.TabIndex = 31;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.findStrip2);
            this.tabPage1.Controls.Add(this.fileInArchiveBindingNavigator);
            this.tabPage1.Controls.Add(this.Pattern);
            this.tabPage1.Controls.Add(this.LabelFilter);
            this.tabPage1.Controls.Add(this.fileInArchiveDataGridView);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(873, 348);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Archive File List";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // findStrip2
            // 
            this.findStrip2.BindingSource = this.fileInArchiveBindingSource;
            this.findStrip2.Dock = System.Windows.Forms.DockStyle.None;
            this.findStrip2.Location = new System.Drawing.Point(7, 32);
            this.findStrip2.Name = "findStrip2";
            this.findStrip2.Size = new System.Drawing.Size(377, 25);
            this.findStrip2.TabIndex = 31;
            this.findStrip2.Text = "findStrip2";
            this.findStrip2.ItemFound += new EasyMYP.ItemFoundEventHandler(this.findStrip_ItemFound);
            // 
            // fileInArchiveBindingSource
            // 
            this.fileInArchiveBindingSource.DataSource = typeof(MYPWorker.FileInArchive);
            // 
            // fileInArchiveBindingNavigator
            // 
            this.fileInArchiveBindingNavigator.AddNewItem = this.bindingNavigatorAddNewItem;
            this.fileInArchiveBindingNavigator.BindingSource = this.fileInArchiveBindingSource;
            this.fileInArchiveBindingNavigator.CountItem = this.bindingNavigatorCountItem;
            this.fileInArchiveBindingNavigator.DeleteItem = this.bindingNavigatorDeleteItem;
            this.fileInArchiveBindingNavigator.Dock = System.Windows.Forms.DockStyle.None;
            this.fileInArchiveBindingNavigator.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bindingNavigatorMoveFirstItem,
            this.bindingNavigatorMovePreviousItem,
            this.bindingNavigatorSeparator,
            this.bindingNavigatorPositionItem,
            this.bindingNavigatorCountItem,
            this.bindingNavigatorSeparator1,
            this.bindingNavigatorMoveNextItem,
            this.bindingNavigatorMoveLastItem,
            this.bindingNavigatorSeparator2,
            this.bindingNavigatorAddNewItem,
            this.bindingNavigatorDeleteItem,
            this.fileInArchiveBindingNavigatorSaveItem});
            this.fileInArchiveBindingNavigator.Location = new System.Drawing.Point(3, 3);
            this.fileInArchiveBindingNavigator.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
            this.fileInArchiveBindingNavigator.MoveLastItem = this.bindingNavigatorMoveLastItem;
            this.fileInArchiveBindingNavigator.MoveNextItem = this.bindingNavigatorMoveNextItem;
            this.fileInArchiveBindingNavigator.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
            this.fileInArchiveBindingNavigator.Name = "fileInArchiveBindingNavigator";
            this.fileInArchiveBindingNavigator.PositionItem = this.bindingNavigatorPositionItem;
            this.fileInArchiveBindingNavigator.Size = new System.Drawing.Size(277, 25);
            this.fileInArchiveBindingNavigator.TabIndex = 28;
            this.fileInArchiveBindingNavigator.Text = "bindingNavigator1";
            // 
            // bindingNavigatorAddNewItem
            // 
            this.bindingNavigatorAddNewItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorAddNewItem.Enabled = false;
            this.bindingNavigatorAddNewItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorAddNewItem.Image")));
            this.bindingNavigatorAddNewItem.Name = "bindingNavigatorAddNewItem";
            this.bindingNavigatorAddNewItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorAddNewItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorAddNewItem.Text = "Add new";
            // 
            // bindingNavigatorCountItem
            // 
            this.bindingNavigatorCountItem.Name = "bindingNavigatorCountItem";
            this.bindingNavigatorCountItem.Size = new System.Drawing.Size(36, 22);
            this.bindingNavigatorCountItem.Text = "of {0}";
            this.bindingNavigatorCountItem.ToolTipText = "Total number of items";
            // 
            // bindingNavigatorDeleteItem
            // 
            this.bindingNavigatorDeleteItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorDeleteItem.Enabled = false;
            this.bindingNavigatorDeleteItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorDeleteItem.Image")));
            this.bindingNavigatorDeleteItem.Name = "bindingNavigatorDeleteItem";
            this.bindingNavigatorDeleteItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorDeleteItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorDeleteItem.Text = "Delete";
            // 
            // bindingNavigatorMoveFirstItem
            // 
            this.bindingNavigatorMoveFirstItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveFirstItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveFirstItem.Image")));
            this.bindingNavigatorMoveFirstItem.Name = "bindingNavigatorMoveFirstItem";
            this.bindingNavigatorMoveFirstItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveFirstItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMoveFirstItem.Text = "Move first";
            // 
            // bindingNavigatorMovePreviousItem
            // 
            this.bindingNavigatorMovePreviousItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMovePreviousItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMovePreviousItem.Image")));
            this.bindingNavigatorMovePreviousItem.Name = "bindingNavigatorMovePreviousItem";
            this.bindingNavigatorMovePreviousItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMovePreviousItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMovePreviousItem.Text = "Move previous";
            // 
            // bindingNavigatorSeparator
            // 
            this.bindingNavigatorSeparator.Name = "bindingNavigatorSeparator";
            this.bindingNavigatorSeparator.Size = new System.Drawing.Size(6, 25);
            // 
            // bindingNavigatorPositionItem
            // 
            this.bindingNavigatorPositionItem.AccessibleName = "Position";
            this.bindingNavigatorPositionItem.AutoSize = false;
            this.bindingNavigatorPositionItem.Name = "bindingNavigatorPositionItem";
            this.bindingNavigatorPositionItem.Size = new System.Drawing.Size(50, 21);
            this.bindingNavigatorPositionItem.Text = "0";
            this.bindingNavigatorPositionItem.ToolTipText = "Current position";
            // 
            // bindingNavigatorSeparator1
            // 
            this.bindingNavigatorSeparator1.Name = "bindingNavigatorSeparator1";
            this.bindingNavigatorSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // bindingNavigatorMoveNextItem
            // 
            this.bindingNavigatorMoveNextItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveNextItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveNextItem.Image")));
            this.bindingNavigatorMoveNextItem.Name = "bindingNavigatorMoveNextItem";
            this.bindingNavigatorMoveNextItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveNextItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMoveNextItem.Text = "Move next";
            // 
            // bindingNavigatorMoveLastItem
            // 
            this.bindingNavigatorMoveLastItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveLastItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveLastItem.Image")));
            this.bindingNavigatorMoveLastItem.Name = "bindingNavigatorMoveLastItem";
            this.bindingNavigatorMoveLastItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveLastItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMoveLastItem.Text = "Move last";
            // 
            // bindingNavigatorSeparator2
            // 
            this.bindingNavigatorSeparator2.Name = "bindingNavigatorSeparator2";
            this.bindingNavigatorSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // fileInArchiveBindingNavigatorSaveItem
            // 
            this.fileInArchiveBindingNavigatorSaveItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.fileInArchiveBindingNavigatorSaveItem.Enabled = false;
            this.fileInArchiveBindingNavigatorSaveItem.Image = ((System.Drawing.Image)(resources.GetObject("fileInArchiveBindingNavigatorSaveItem.Image")));
            this.fileInArchiveBindingNavigatorSaveItem.Name = "fileInArchiveBindingNavigatorSaveItem";
            this.fileInArchiveBindingNavigatorSaveItem.Size = new System.Drawing.Size(23, 22);
            this.fileInArchiveBindingNavigatorSaveItem.Text = "Save Data";
            // 
            // Pattern
            // 
            this.Pattern.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Pattern.Location = new System.Drawing.Point(714, 6);
            this.Pattern.Name = "Pattern";
            this.Pattern.Size = new System.Drawing.Size(153, 20);
            this.Pattern.TabIndex = 29;
            this.Pattern.Text = "*";
            this.Pattern.Visible = false;
            // 
            // LabelFilter
            // 
            this.LabelFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LabelFilter.AutoSize = true;
            this.LabelFilter.Location = new System.Drawing.Point(679, 9);
            this.LabelFilter.Name = "LabelFilter";
            this.LabelFilter.Size = new System.Drawing.Size(29, 13);
            this.LabelFilter.TabIndex = 30;
            this.LabelFilter.Text = "Filter";
            this.LabelFilter.Visible = false;
            // 
            // fileInArchiveDataGridView
            // 
            this.fileInArchiveDataGridView.AllowUserToAddRows = false;
            this.fileInArchiveDataGridView.AllowUserToDeleteRows = false;
            this.fileInArchiveDataGridView.AllowUserToResizeRows = false;
            this.fileInArchiveDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.fileInArchiveDataGridView.AutoGenerateColumns = false;
            this.fileInArchiveDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn5,
            this.dataGridViewTextBoxColumn6,
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn4});
            this.fileInArchiveDataGridView.DataSource = this.fileInArchiveBindingSource;
            this.fileInArchiveDataGridView.Location = new System.Drawing.Point(6, 60);
            this.fileInArchiveDataGridView.Name = "fileInArchiveDataGridView";
            this.fileInArchiveDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.fileInArchiveDataGridView.Size = new System.Drawing.Size(867, 288);
            this.fileInArchiveDataGridView.TabIndex = 18;
            this.fileInArchiveDataGridView.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.fileInArchiveDataGridView_ColumnHeaderMouseClick);
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.DataPropertyName = "Filename";
            this.dataGridViewTextBoxColumn5.HeaderText = "Filename";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.ReadOnly = true;
            this.dataGridViewTextBoxColumn5.Width = 300;
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.DataPropertyName = "Extension";
            this.dataGridViewTextBoxColumn6.HeaderText = "Extension";
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.DataPropertyName = "Offset";
            this.dataGridViewTextBoxColumn1.HeaderText = "Offset";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.DataPropertyName = "Size";
            this.dataGridViewTextBoxColumn2.HeaderText = "Size";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.DataPropertyName = "CompressedSize";
            this.dataGridViewTextBoxColumn3.HeaderText = "CompressedSize";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.DataPropertyName = "CompressionMethod";
            this.dataGridViewTextBoxColumn4.HeaderText = "CompressionMethod";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            this.dataGridViewTextBoxColumn4.Width = 125;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.splitContainer1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(873, 348);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Archive Tree View";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(6, 6);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeView_Archive);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.treeView_FileSystem);
            this.splitContainer1.Size = new System.Drawing.Size(823, 336);
            this.splitContainer1.SplitterDistance = 404;
            this.splitContainer1.TabIndex = 0;
            // 
            // treeView_Archive
            // 
            this.treeView_Archive.Location = new System.Drawing.Point(3, 3);
            this.treeView_Archive.Name = "treeView_Archive";
            this.treeView_Archive.Size = new System.Drawing.Size(398, 330);
            this.treeView_Archive.TabIndex = 0;
            // 
            // treeView_FileSystem
            // 
            this.treeView_FileSystem.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView_FileSystem.Location = new System.Drawing.Point(3, 3);
            this.treeView_FileSystem.Name = "treeView_FileSystem";
            this.treeView_FileSystem.Size = new System.Drawing.Size(409, 330);
            this.treeView_FileSystem.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.lblGenerateFNE);
            this.tabPage3.Controls.Add(this.lblGeneratePat);
            this.tabPage3.Controls.Add(this.label1);
            this.tabPage3.Controls.Add(this.buttonGenerateFilenames_OnPattern);
            this.tabPage3.Controls.Add(this.buttonGenerateFilenames_CurrentFiles);
            this.tabPage3.Controls.Add(this.buttonExtractModifiedFiles);
            this.tabPage3.Controls.Add(this.buttonExtractNewFiles);
            this.tabPage3.Controls.Add(this.label_ModifiedFiles_Value);
            this.tabPage3.Controls.Add(this.label_ModifiedFiles_Text);
            this.tabPage3.Controls.Add(this.label_NewFiles_Value);
            this.tabPage3.Controls.Add(this.label_NewFiles_Text);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(873, 348);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "File Changes";
            this.tabPage3.UseVisualStyleBackColor = true;
            this.tabPage3.Paint += new System.Windows.Forms.PaintEventHandler(this.tabPage3_Paint);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(52, 69);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(213, 13);
            this.label1.TabIndex = 29;
            this.label1.Text = "Quitting Easymyp will also kill the generation";
            // 
            // buttonGenerateFilenames_OnPattern
            // 
            this.buttonGenerateFilenames_OnPattern.Location = new System.Drawing.Point(6, 93);
            this.buttonGenerateFilenames_OnPattern.Name = "buttonGenerateFilenames_OnPattern";
            this.buttonGenerateFilenames_OnPattern.Size = new System.Drawing.Size(303, 23);
            this.buttonGenerateFilenames_OnPattern.TabIndex = 28;
            this.buttonGenerateFilenames_OnPattern.Text = "Generate filenames depending on pattern (48hours)";
            this.buttonGenerateFilenames_OnPattern.UseVisualStyleBackColor = true;
            this.buttonGenerateFilenames_OnPattern.Click += new System.EventHandler(this.buttonGenerateFilenames_OnPattern_Click);
            // 
            // buttonGenerateFilenames_CurrentFiles
            // 
            this.buttonGenerateFilenames_CurrentFiles.Location = new System.Drawing.Point(6, 122);
            this.buttonGenerateFilenames_CurrentFiles.Name = "buttonGenerateFilenames_CurrentFiles";
            this.buttonGenerateFilenames_CurrentFiles.Size = new System.Drawing.Size(303, 23);
            this.buttonGenerateFilenames_CurrentFiles.TabIndex = 27;
            this.buttonGenerateFilenames_CurrentFiles.Text = "Generate filenames depending on current files (6hours)";
            this.buttonGenerateFilenames_CurrentFiles.UseVisualStyleBackColor = true;
            this.buttonGenerateFilenames_CurrentFiles.Click += new System.EventHandler(this.buttonGenerateFilenames_CurrentFiles_Click);
            // 
            // buttonExtractModifiedFiles
            // 
            this.buttonExtractModifiedFiles.Location = new System.Drawing.Point(183, 35);
            this.buttonExtractModifiedFiles.Name = "buttonExtractModifiedFiles";
            this.buttonExtractModifiedFiles.Size = new System.Drawing.Size(126, 23);
            this.buttonExtractModifiedFiles.TabIndex = 26;
            this.buttonExtractModifiedFiles.Text = "Extract Modified Files";
            this.buttonExtractModifiedFiles.UseVisualStyleBackColor = true;
            this.buttonExtractModifiedFiles.Click += new System.EventHandler(this.buttonExtractModifiedFiles_Click);
            // 
            // buttonExtractNewFiles
            // 
            this.buttonExtractNewFiles.Location = new System.Drawing.Point(183, 6);
            this.buttonExtractNewFiles.Name = "buttonExtractNewFiles";
            this.buttonExtractNewFiles.Size = new System.Drawing.Size(126, 23);
            this.buttonExtractNewFiles.TabIndex = 25;
            this.buttonExtractNewFiles.Text = "Extract New Files";
            this.buttonExtractNewFiles.UseVisualStyleBackColor = true;
            this.buttonExtractNewFiles.Click += new System.EventHandler(this.buttonExtractNewFiles_Click);
            // 
            // label_ModifiedFiles_Value
            // 
            this.label_ModifiedFiles_Value.AutoSize = true;
            this.label_ModifiedFiles_Value.Location = new System.Drawing.Point(129, 40);
            this.label_ModifiedFiles_Value.Name = "label_ModifiedFiles_Value";
            this.label_ModifiedFiles_Value.Size = new System.Drawing.Size(13, 13);
            this.label_ModifiedFiles_Value.TabIndex = 24;
            this.label_ModifiedFiles_Value.Text = "0";
            // 
            // label_ModifiedFiles_Text
            // 
            this.label_ModifiedFiles_Text.AutoSize = true;
            this.label_ModifiedFiles_Text.Location = new System.Drawing.Point(6, 40);
            this.label_ModifiedFiles_Text.Name = "label_ModifiedFiles_Text";
            this.label_ModifiedFiles_Text.Size = new System.Drawing.Size(71, 13);
            this.label_ModifiedFiles_Text.TabIndex = 23;
            this.label_ModifiedFiles_Text.Text = "Modified Files";
            // 
            // label_NewFiles_Value
            // 
            this.label_NewFiles_Value.AutoSize = true;
            this.label_NewFiles_Value.Location = new System.Drawing.Point(129, 11);
            this.label_NewFiles_Value.Name = "label_NewFiles_Value";
            this.label_NewFiles_Value.Size = new System.Drawing.Size(13, 13);
            this.label_NewFiles_Value.TabIndex = 22;
            this.label_NewFiles_Value.Text = "0";
            // 
            // label_NewFiles_Text
            // 
            this.label_NewFiles_Text.AutoSize = true;
            this.label_NewFiles_Text.Location = new System.Drawing.Point(6, 11);
            this.label_NewFiles_Text.Name = "label_NewFiles_Text";
            this.label_NewFiles_Text.Size = new System.Drawing.Size(53, 13);
            this.label_NewFiles_Text.TabIndex = 21;
            this.label_NewFiles_Text.Text = "New Files";
            // 
            // panel_status
            // 
            this.panel_status.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel_status.Controls.Add(this.Loading);
            this.panel_status.Controls.Add(this.credits);
            this.panel_status.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel_status.Location = new System.Drawing.Point(0, 449);
            this.panel_status.Name = "panel_status";
            this.panel_status.Size = new System.Drawing.Size(905, 23);
            this.panel_status.TabIndex = 18;
            // 
            // Loading
            // 
            this.Loading.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Loading.Location = new System.Drawing.Point(0, 0);
            this.Loading.Name = "Loading";
            this.Loading.Size = new System.Drawing.Size(815, 23);
            this.Loading.TabIndex = 28;
            this.Loading.Visible = false;
            // 
            // credits
            // 
            this.credits.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.credits.AutoSize = true;
            this.credits.Location = new System.Drawing.Point(821, 5);
            this.credits.Name = "credits";
            this.credits.Size = new System.Drawing.Size(70, 13);
            this.credits.TabIndex = 0;
            this.credits.Text = "Chryso Inside";
            // 
            // panel_information
            // 
            this.panel_information.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel_information.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.panel_information.Controls.Add(this.label_File_Text);
            this.panel_information.Controls.Add(this.label_ExtractedFiles_Value);
            this.panel_information.Controls.Add(this.label_NumOfNamedFiles_Value);
            this.panel_information.Controls.Add(this.label_ExtractedFiles_Text);
            this.panel_information.Controls.Add(this.label_EstimatedNumOfFiles_Text);
            this.panel_information.Controls.Add(this.label_ExtractionErrors_Value);
            this.panel_information.Controls.Add(this.label_NumOfNamedFiles_Text);
            this.panel_information.Controls.Add(this.label_ExtractionErrors_Text);
            this.panel_information.Controls.Add(this.label_EstimatedNumOfFiles_Value);
            this.panel_information.Controls.Add(this.label_File_Value);
            this.panel_information.Controls.Add(this.label_UncompressedSize_Text);
            this.panel_information.Controls.Add(this.label_NumOfFiles_Value);
            this.panel_information.Controls.Add(this.label_NumOfFiles_Text);
            this.panel_information.Controls.Add(this.label_ReadingErrors_Text);
            this.panel_information.Controls.Add(this.label_UncompressedSize_Value);
            this.panel_information.Controls.Add(this.label_ReadingErrors_Value);
            this.panel_information.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_information.Location = new System.Drawing.Point(0, 0);
            this.panel_information.Name = "panel_information";
            this.panel_information.Size = new System.Drawing.Size(905, 63);
            this.panel_information.TabIndex = 17;
            // 
            // label_File_Text
            // 
            this.label_File_Text.AutoSize = true;
            this.label_File_Text.Location = new System.Drawing.Point(8, 11);
            this.label_File_Text.Name = "label_File_Text";
            this.label_File_Text.Size = new System.Drawing.Size(26, 13);
            this.label_File_Text.TabIndex = 3;
            this.label_File_Text.Text = "File:";
            // 
            // label_ExtractedFiles_Value
            // 
            this.label_ExtractedFiles_Value.AutoSize = true;
            this.label_ExtractedFiles_Value.Location = new System.Drawing.Point(644, 11);
            this.label_ExtractedFiles_Value.Name = "label_ExtractedFiles_Value";
            this.label_ExtractedFiles_Value.Size = new System.Drawing.Size(13, 13);
            this.label_ExtractedFiles_Value.TabIndex = 12;
            this.label_ExtractedFiles_Value.Text = "0";
            // 
            // label_NumOfNamedFiles_Value
            // 
            this.label_NumOfNamedFiles_Value.AutoSize = true;
            this.label_NumOfNamedFiles_Value.Location = new System.Drawing.Point(402, 11);
            this.label_NumOfNamedFiles_Value.Name = "label_NumOfNamedFiles_Value";
            this.label_NumOfNamedFiles_Value.Size = new System.Drawing.Size(13, 13);
            this.label_NumOfNamedFiles_Value.TabIndex = 16;
            this.label_NumOfNamedFiles_Value.Text = "0";
            // 
            // label_ExtractedFiles_Text
            // 
            this.label_ExtractedFiles_Text.AutoSize = true;
            this.label_ExtractedFiles_Text.Location = new System.Drawing.Point(498, 11);
            this.label_ExtractedFiles_Text.Name = "label_ExtractedFiles_Text";
            this.label_ExtractedFiles_Text.Size = new System.Drawing.Size(127, 13);
            this.label_ExtractedFiles_Text.TabIndex = 11;
            this.label_ExtractedFiles_Text.Text = "Number of files extracted:";
            // 
            // label_EstimatedNumOfFiles_Text
            // 
            this.label_EstimatedNumOfFiles_Text.AutoSize = true;
            this.label_EstimatedNumOfFiles_Text.Location = new System.Drawing.Point(8, 24);
            this.label_EstimatedNumOfFiles_Text.Name = "label_EstimatedNumOfFiles_Text";
            this.label_EstimatedNumOfFiles_Text.Size = new System.Drawing.Size(122, 13);
            this.label_EstimatedNumOfFiles_Text.TabIndex = 14;
            this.label_EstimatedNumOfFiles_Text.Text = "Estimated number of file:";
            // 
            // label_ExtractionErrors_Value
            // 
            this.label_ExtractionErrors_Value.AutoSize = true;
            this.label_ExtractionErrors_Value.Location = new System.Drawing.Point(644, 24);
            this.label_ExtractionErrors_Value.Name = "label_ExtractionErrors_Value";
            this.label_ExtractionErrors_Value.Size = new System.Drawing.Size(13, 13);
            this.label_ExtractionErrors_Value.TabIndex = 10;
            this.label_ExtractionErrors_Value.Text = "0";
            // 
            // label_NumOfNamedFiles_Text
            // 
            this.label_NumOfNamedFiles_Text.AutoSize = true;
            this.label_NumOfNamedFiles_Text.Location = new System.Drawing.Point(256, 11);
            this.label_NumOfNamedFiles_Text.Name = "label_NumOfNamedFiles_Text";
            this.label_NumOfNamedFiles_Text.Size = new System.Drawing.Size(115, 13);
            this.label_NumOfNamedFiles_Text.TabIndex = 15;
            this.label_NumOfNamedFiles_Text.Text = "Number of named files:";
            // 
            // label_ExtractionErrors_Text
            // 
            this.label_ExtractionErrors_Text.AutoSize = true;
            this.label_ExtractionErrors_Text.Location = new System.Drawing.Point(498, 24);
            this.label_ExtractionErrors_Text.Name = "label_ExtractionErrors_Text";
            this.label_ExtractionErrors_Text.Size = new System.Drawing.Size(82, 13);
            this.label_ExtractionErrors_Text.TabIndex = 9;
            this.label_ExtractionErrors_Text.Text = "Extraction Error:";
            // 
            // label_EstimatedNumOfFiles_Value
            // 
            this.label_EstimatedNumOfFiles_Value.AutoSize = true;
            this.label_EstimatedNumOfFiles_Value.Location = new System.Drawing.Point(136, 24);
            this.label_EstimatedNumOfFiles_Value.Name = "label_EstimatedNumOfFiles_Value";
            this.label_EstimatedNumOfFiles_Value.Size = new System.Drawing.Size(13, 13);
            this.label_EstimatedNumOfFiles_Value.TabIndex = 13;
            this.label_EstimatedNumOfFiles_Value.Text = "0";
            // 
            // label_File_Value
            // 
            this.label_File_Value.AutoSize = true;
            this.label_File_Value.Location = new System.Drawing.Point(136, 11);
            this.label_File_Value.Name = "label_File_Value";
            this.label_File_Value.Size = new System.Drawing.Size(71, 13);
            this.label_File_Value.TabIndex = 4;
            this.label_File_Value.Text = "Default Value";
            // 
            // label_UncompressedSize_Text
            // 
            this.label_UncompressedSize_Text.AutoSize = true;
            this.label_UncompressedSize_Text.Location = new System.Drawing.Point(256, 37);
            this.label_UncompressedSize_Text.Name = "label_UncompressedSize_Text";
            this.label_UncompressedSize_Text.Size = new System.Drawing.Size(128, 13);
            this.label_UncompressedSize_Text.TabIndex = 8;
            this.label_UncompressedSize_Text.Text = "Total Uncompressed Size";
            // 
            // label_NumOfFiles_Value
            // 
            this.label_NumOfFiles_Value.AutoSize = true;
            this.label_NumOfFiles_Value.Location = new System.Drawing.Point(136, 37);
            this.label_NumOfFiles_Value.Name = "label_NumOfFiles_Value";
            this.label_NumOfFiles_Value.Size = new System.Drawing.Size(13, 13);
            this.label_NumOfFiles_Value.TabIndex = 2;
            this.label_NumOfFiles_Value.Text = "0";
            // 
            // label_NumOfFiles_Text
            // 
            this.label_NumOfFiles_Text.AutoSize = true;
            this.label_NumOfFiles_Text.Location = new System.Drawing.Point(8, 37);
            this.label_NumOfFiles_Text.Name = "label_NumOfFiles_Text";
            this.label_NumOfFiles_Text.Size = new System.Drawing.Size(80, 13);
            this.label_NumOfFiles_Text.TabIndex = 1;
            this.label_NumOfFiles_Text.Text = "Number of files:";
            // 
            // label_ReadingErrors_Text
            // 
            this.label_ReadingErrors_Text.AutoSize = true;
            this.label_ReadingErrors_Text.Location = new System.Drawing.Point(256, 24);
            this.label_ReadingErrors_Text.Name = "label_ReadingErrors_Text";
            this.label_ReadingErrors_Text.Size = new System.Drawing.Size(89, 13);
            this.label_ReadingErrors_Text.TabIndex = 5;
            this.label_ReadingErrors_Text.Text = "Number of Errors:";
            // 
            // label_UncompressedSize_Value
            // 
            this.label_UncompressedSize_Value.AutoSize = true;
            this.label_UncompressedSize_Value.Location = new System.Drawing.Point(402, 37);
            this.label_UncompressedSize_Value.Name = "label_UncompressedSize_Value";
            this.label_UncompressedSize_Value.Size = new System.Drawing.Size(13, 13);
            this.label_UncompressedSize_Value.TabIndex = 7;
            this.label_UncompressedSize_Value.Text = "0";
            // 
            // label_ReadingErrors_Value
            // 
            this.label_ReadingErrors_Value.AutoSize = true;
            this.label_ReadingErrors_Value.Location = new System.Drawing.Point(402, 24);
            this.label_ReadingErrors_Value.Name = "label_ReadingErrors_Value";
            this.label_ReadingErrors_Value.Size = new System.Drawing.Size(13, 13);
            this.label_ReadingErrors_Value.TabIndex = 6;
            this.label_ReadingErrors_Value.Text = "0";
            // 
            // lblGeneratePat
            // 
            this.lblGeneratePat.AutoSize = true;
            this.lblGeneratePat.Location = new System.Drawing.Point(315, 98);
            this.lblGeneratePat.Name = "lblGeneratePat";
            this.lblGeneratePat.Size = new System.Drawing.Size(45, 13);
            this.lblGeneratePat.TabIndex = 30;
            this.lblGeneratePat.Text = "Inactive";
            // 
            // lblGenerateFNE
            // 
            this.lblGenerateFNE.AutoSize = true;
            this.lblGenerateFNE.Location = new System.Drawing.Point(315, 127);
            this.lblGenerateFNE.Name = "lblGenerateFNE";
            this.lblGenerateFNE.Size = new System.Drawing.Size(45, 13);
            this.lblGenerateFNE.TabIndex = 31;
            this.lblGenerateFNE.Text = "Inactive";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(905, 496);
            this.Controls.Add(this.panel_output);
            this.Controls.Add(this.mainMenu);
            this.MainMenuStrip = this.mainMenu;
            this.Name = "MainWindow";
            this.Text = "EasyMYP";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainWindow_FormClosed);
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.panel_output.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.findStrip2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fileInArchiveBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fileInArchiveBindingNavigator)).EndInit();
            this.fileInArchiveBindingNavigator.ResumeLayout(false);
            this.fileInArchiveBindingNavigator.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fileInArchiveDataGridView)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.panel_status.ResumeLayout(false);
            this.panel_status.PerformLayout();
            this.panel_information.ResumeLayout(false);
            this.panel_information.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mainMenu;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openArchiveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeArchiveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mergeDictionaryFile_ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tryAgainToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem extractSelectedToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openArchiveDialog;
        private System.Windows.Forms.Panel panel_output;
        private System.Windows.Forms.Label label_NumOfNamedFiles_Value;
        private System.Windows.Forms.Label label_NumOfNamedFiles_Text;
        private System.Windows.Forms.Label label_EstimatedNumOfFiles_Text;
        private System.Windows.Forms.Label label_EstimatedNumOfFiles_Value;
        private System.Windows.Forms.Label label_ExtractedFiles_Value;
        private System.Windows.Forms.Label label_ExtractedFiles_Text;
        private System.Windows.Forms.Label label_ExtractionErrors_Value;
        private System.Windows.Forms.Label label_ExtractionErrors_Text;
        private System.Windows.Forms.Label label_UncompressedSize_Text;
        private System.Windows.Forms.Label label_UncompressedSize_Value;
        private System.Windows.Forms.Label label_ReadingErrors_Value;
        private System.Windows.Forms.Label label_ReadingErrors_Text;
        private System.Windows.Forms.Label label_File_Value;
        private System.Windows.Forms.Label label_File_Text;
        private System.Windows.Forms.Label label_NumOfFiles_Text;
        private System.Windows.Forms.Label label_NumOfFiles_Value;
        private System.Windows.Forms.ToolStripMenuItem extractAllToolStripMenuItem;
        private System.Windows.Forms.Panel panel_information;
        private System.Windows.Forms.Panel panel_status;
        private System.Windows.Forms.Label credits;
        private System.Windows.Forms.DataGridView fileInArchiveDataGridView;
        private System.Windows.Forms.BindingSource fileInArchiveBindingSource;
        private System.Windows.Forms.BindingNavigator fileInArchiveBindingNavigator;
        private System.Windows.Forms.ToolStripButton bindingNavigatorAddNewItem;
        private System.Windows.Forms.ToolStripLabel bindingNavigatorCountItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorDeleteItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveFirstItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMovePreviousItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator;
        private System.Windows.Forms.ToolStripTextBox bindingNavigatorPositionItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator1;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveNextItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveLastItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator2;
        private System.Windows.Forms.ToolStripButton fileInArchiveBindingNavigatorSaveItem;
        private System.Windows.Forms.ProgressBar Loading;
        private System.Windows.Forms.Label LabelFilter;
        private System.Windows.Forms.TextBox Pattern;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView treeView_Archive;
        private System.Windows.Forms.TreeView treeView_FileSystem;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.ToolStripMenuItem selectExtractionFolderToolStripMenuItem;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Label label_ModifiedFiles_Value;
        private System.Windows.Forms.Label label_ModifiedFiles_Text;
        private System.Windows.Forms.Label label_NewFiles_Value;
        private System.Windows.Forms.Label label_NewFiles_Text;
        private System.Windows.Forms.Button buttonExtractModifiedFiles;
        private System.Windows.Forms.Button buttonExtractNewFiles;
        private System.Windows.Forms.ToolStripMenuItem testFilenameListToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testFullFilenameListToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem replaceSelectedToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog replaceFileDialog;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private EasyMYP.FindStrip findStrip1;
        private FindStrip findStrip2;
        private System.Windows.Forms.Button buttonGenerateFilenames_OnPattern;
        private System.Windows.Forms.Button buttonGenerateFilenames_CurrentFiles;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblGenerateFNE;
        private System.Windows.Forms.Label lblGeneratePat;
    }
}

