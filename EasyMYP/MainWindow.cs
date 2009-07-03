using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using MYPHandler;
using nsHashCreator;
using nsHashDictionary;

namespace EasyMYP
{
    public enum State
    {
        OpeningArchive,
        CreatingArchive,
        ExtractingArchive,
        ClosingArchive,
        Exiting,
        SearchingForFile,
        ReplacingFile,
        AddingFile,
        CreatingDirectoryStructure,
        RebuildingHashList,
        waiting
    }

    public partial class MainWindow : Form
    {
        #region Attributes
        MYPHandler.MYPHandler MypFileHandler; //The class that does all the job of reading, extracting and writting to myp files
        public Thread t_worker; //worker thread because it is best that way so not to freeze the gui
        HashDictionary hashDic = new HashDictionary(); //The class that contains the dictionnary
        HashDictionary patternDic;
        HashCreator hashCreator;
        //List<FileInArchive> FIAList = new List<FileInArchive>();
        AvancementBar avBar;
        string extractionPath = null;
        bool operationRunning = false;

        List<String> scanFiles = new List<string>();
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            LoadDictionnary(false, "");
            hashCreator = new HashCreator(hashDic);
            fileInArchiveBindingSource.DataSource = new SortableBindingList<FileInArchive>();

            //Define functions that should be called to treat events
            //Needed because of cross thread calls
            OnNewExtractedEvent = TreatExtractionEvent;
            OnNewFileTableEvent = TreatFileTableEvent;
            OnNewFilenameTestEvent = TreatFilenameTestEvent;

            // Populate the system tree view
            TreeViewManager.PopulateSystemTreeNode(treeView_FileSystem);
        }

        /// <summary>
        /// Protects for concurrent operations
        /// </summary>
        /// <returns></returns>
        private bool SetOperationRunning()
        {
            if (operationRunning)
            {
                MessageBox.Show("Please wait until completion of previous operation", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            operationRunning = true;
            return true;
        }

        void LoadDictionnary(bool merge, string fileToMerge)
        {
            if (!SetOperationRunning()) return;

            //Show a progress bar. Use it to block other operations while hashDic not initialized correctly
            avBar = new AvancementBar();
            avBar.Text = "Loading hash list ...";

            hashDic.HashEvent += avBar.UpdateDictionaryEventHandler;

            //Building the dictionnary
            if (!merge)
            {
                t_worker = new Thread(new ThreadStart(hashDic.LoadHashList));
                t_worker.Start();
            }
            else
            {
                t_worker = new Thread(new ParameterizedThreadStart(hashDic.MergeHashList));
                t_worker.Start(fileToMerge);
            }

            //Wait until the dictionnary is loaded
            avBar.ShowDialog();
            hashDic.HashEvent -= avBar.UpdateDictionaryEventHandler;
            avBar.Dispose();

            operationRunning = false;
        }

        #region Menu
        #region Archive Menu

        public void resetOverall()
        {
            if (t_worker != null)
                t_worker.Abort();

            if (MypFileHandler != null)
                MypFileHandler.Dispose();

            operationRunning = false;

            label_File_Value.Text = "";
            label_NewFiles_Value.Text = "0";
            label_ModifiedFiles_Value.Text = "0";
            label_EstimatedNumOfFiles_Value.Text = "0";
            label_NumOfFiles_Value.Text = "0";
            label_NumOfNamedFiles_Value.Text = "0";
            label_ReadingErrors_Value.Text = "0";
            label_UncompressedSize_Value.Text = "0";
            label_ExtractedFiles_Value.Text = "0";
            label_ExtractionErrors_Value.Text = "0";
            fileInArchiveBindingSource.Clear(); //Clean the filelisting

            extractAllToolStripMenuItem.Enabled = false;
            extractFileListToolStripMenuItem.Enabled = false;
            extractSelectedToolStripMenuItem.Enabled = false;
            replaceSelectedToolStripMenuItem.Enabled = false;

            statusPB.Visible = false;
        }

        private void openArchiveToolStripMenuItem_Click(object sender, EventArgs e)
        {

            openArchiveDialog.Filter = "MYP Archives|*.myp";
            if (openArchiveDialog.ShowDialog() == DialogResult.OK)
            {
                if (!SetOperationRunning()) return; //reset in eventhandler
                resetOverall();
                fileInArchiveBindingSource.Clear(); //Clean the filelisting
                fileInArchiveDataGridView.DataSource = null; // switched by when Event_FileTableType.Finished is received.

                string filename = openArchiveDialog.FileName; //get filename selected
                int filenameStartPosition = filename.LastIndexOf('\\');
                label_File_Value.Text = filename.Substring(filenameStartPosition + 1, filename.Length - filenameStartPosition - 1);

                MypFileHandler = new MYPHandler.MYPHandler(filename
                    , FileTableEventHandler, ExtractionEventHandler
                    , hashDic);

                if (extractionPath != null)
                {
                    MypFileHandler.ExtractionPath = extractionPath;
                }

                statusPB.Maximum = (int)MypFileHandler.TotalNumberOfFiles;
                statusPB.Value = 0;
                statusPB.Visible = true;
                label_EstimatedNumOfFiles_Value.Text = MypFileHandler.TotalNumberOfFiles.ToString("#,#");

                MypFileHandler.Pattern = Pattern.Text;
                t_worker = new Thread(new ThreadStart(MypFileHandler.GetFileTable));
                t_worker.Start();

                //Protected by isRunning
                extractAllToolStripMenuItem.Enabled = true;
                extractFileListToolStripMenuItem.Enabled = true;
                extractSelectedToolStripMenuItem.Enabled = true;
                replaceSelectedToolStripMenuItem.Enabled = true;
            }
        }

        private void closeArchiveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            resetOverall();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region Dictionary Menu

        private void testFilenameListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileTester tester = new FileTester();

            if (tester.ShowDialog() == DialogResult.OK)
            {

                if (!SetOperationRunning()) return; //reset in eventhandler

                HashSet<String> dirs = new HashSet<string>();
                HashSet<String> files = new HashSet<string>();
                HashSet<String> exts = new HashSet<string>();
                string outputFileRoot = null;

                hashDic.CreateHelpers();

                if (tester.knownDirBox.Checked)
                    dirs.UnionWith(hashDic.DirListing);
                if (tester.customDirBox.Checked)
                    AddToHashSetFromFile(dirs, tester.customDirFile);

                if (tester.knownFileBox.Checked)
                    files.UnionWith(hashDic.FileListing);
                if (tester.customFileBox.Checked)
                {
                    AddToHashSetFromFile(files, tester.customFileFile);
                    if (!tester.knownFileBox.Checked) // special case where we only test custom files
                    {
                        outputFileRoot = tester.customFileFile;
                        if (Path.HasExtension(outputFileRoot))
                            outputFileRoot = Path.GetDirectoryName(outputFileRoot) + "\\" + Path.GetFileNameWithoutExtension(outputFileRoot);
                    }
                }

                if (tester.knownExtBox.Checked)
                    exts.UnionWith(hashDic.ExtListing);
                if (tester.customExtBox.Checked)
                    AddToHashSetFromFile(exts, tester.customExtFile);

                ThreadParam threadParam = new ThreadParam(dirs, files, exts, 0, 0, outputFileRoot);

                statusPB.Visible = true;
                statusPB.Maximum = ((dirs.Count == 0) ? 1 : dirs.Count) * ((exts.Count == 0) ? 1 : exts.Count);
                statusPB.Value = 0;
                hashCreator.event_FilenameTest += FilenameTestEventHandler; //reset in eventhandler

                t_worker = new Thread(new ParameterizedThreadStart(hashCreator.ParseDirFilenamesAndExtension));
                t_worker.Start(threadParam);

            }
        }

        private void AddToHashSetFromFile(HashSet<String> theHashSet, string filename)
        {
            if (File.Exists(filename))
            {
                FileStream fs = new FileStream(filename, FileMode.Open);
                StreamReader reader = new StreamReader(fs);

                string line;

                // read the file and remove duplicates.
                while ((line = reader.ReadLine()) != null)
                {
                    theHashSet.Add(line);
                }
                reader.Close();
            }
        }

        private void testFullFilenameListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!SetOperationRunning()) return;

            openArchiveDialog.Filter = "File containing full filenames to test|*.txt";
            if (openArchiveDialog.ShowDialog() == DialogResult.OK)
            {
                // supposed to be small enough to avoid threading.
                long newlyFound = hashCreator.ParseFilenames(openArchiveDialog.FileName);
                if (newlyFound != 0)
                {
                    MessageBox.Show("You just found " + newlyFound + " new filenames.", "Newly found filenames!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    MessageBox.Show("No new filenames.", "No new filenames", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            operationRunning = false;
        }

        /// <summary>
        /// Parses all the myp files to extract all the possible hashes.
        /// </summary>
        /// 
        private void scanAllMypsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                string[] mypfiles = Directory.GetFiles(folderBrowserDialog1.SelectedPath, "*.myp");
                if (mypfiles.Length == 0)
                {
                    MessageBox.Show("No myp files found in folder", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!SetOperationRunning()) return; //reset in event handler

                resetOverall();
                foreach (string file in mypfiles)
                {
                    scanFiles.Add(file);
                }

                statusPB.Maximum = (int)mypfiles.Length;
                statusPB.Value = 0;
                statusPB.Visible = true;

                MypFileHandler = new MYPHandler.MYPHandler(scanFiles[0]
                    , null, ExtractionEventHandler
                    , hashDic);

                scanFiles.RemoveAt(0);

                MypFileHandler.Pattern = Pattern.Text;
                t_worker = new Thread(new ThreadStart(MypFileHandler.ScanFileTable));
                t_worker.Start();
            }
        }

        private void mergeDictionaryFile_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openArchiveDialog.Filter = "Dictionary File|*.txt";
            if (openArchiveDialog.ShowDialog() == DialogResult.OK)
            {
                if (!SetOperationRunning()) return;

                //Show a progress bar
                avBar = new AvancementBar();
                avBar.Text = "Merging hash list ...";
                hashDic.HashEvent += avBar.UpdateDictionaryEventHandler;
                t_worker = new Thread(new ParameterizedThreadStart(hashDic.MergeHashList));
                t_worker.Start(openArchiveDialog.FileName);
                //Wait until the dictionnary is loaded
                avBar.ShowDialog();
                hashDic.HashEvent -= avBar.UpdateDictionaryEventHandler;
                avBar.Dispose();

                operationRunning = false;
            }
        }

        private void dumpDirFileExtensionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hashDic.CreateHelpers();
            hashDic.SaveHelpers();
        }

        private void statisticsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DictionnaryStatistics dlg = new DictionnaryStatistics(hashDic.HashList);
            dlg.ShowDialog();
        }

        #endregion

        #region File Menu
        private void selectExtractionFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                extractionPath = folderBrowserDialog1.SelectedPath;
                if (MypFileHandler != null)
                {
                    MypFileHandler.ExtractionPath = extractionPath;
                }
            }
        }

        /// <summary>
        /// Extra selected files from the archive
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void extractSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Retrieve the selected file list
            List<FileInArchive> fileList = new List<FileInArchive>();
            foreach (DataGridViewRow theRow in fileInArchiveDataGridView.SelectedRows)
            {
                if (theRow.DataBoundItem != null)
                    fileList.Add((MYPHandler.FileInArchive)theRow.DataBoundItem);
            }

            ExtractFileList(fileList);
        }

        private void ExtractFiles(List<FileInArchive> fileList)
        {
            if (MypFileHandler != null)
            {
                if (!SetOperationRunning()) return; //reset in event

                statusPB.Value = 0;
                statusPB.Maximum = fileList.Count;
                statusPB.Visible = true;

                t_worker = new Thread(new ParameterizedThreadStart(MypFileHandler.ExtractFileList));
                t_worker.Start(fileList);
            }
        }

        private void extractAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //TODO: if extraction path is not set put an alert up ?
            if (MypFileHandler != null)
            {
                if (extractionPath == null)
                {
                    if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                    {
                        extractionPath = folderBrowserDialog1.SelectedPath;
                        MypFileHandler.ExtractionPath = extractionPath;
                    }
                    else
                        return;
                }
                ExtractFiles(MypFileHandler.archiveFileList); // set the operation flag
            }
        }

        private void extractFileListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MypFileHandler != null)
            {
                if (extractionPath == null)
                {
                    if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                    {
                        extractionPath = folderBrowserDialog1.SelectedPath;
                        MypFileHandler.ExtractionPath = extractionPath;
                    }
                    else
                        return;
                }
                MypFileHandler.DumpFileList();
            }
        }

        private void replaceSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MypFileHandler != null)
            {
                if (replaceFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filename = replaceFileDialog.FileName; //get filename selected

                    if (!SetOperationRunning()) return;

                    MypFileHandler.ReplaceFile((MYPHandler.FileInArchive)fileInArchiveBindingSource.Current
                        , new FileStream(filename, FileMode.Open));

                    operationRunning = false;
                }
            }
        }

        private void buttonExtractNewFiles_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                extractionPath = folderBrowserDialog1.SelectedPath;
                MypFileHandler.ExtractionPath = extractionPath;
                if (MypFileHandler != null)
                {
                    ExtractFiles(MypFileHandler.archiveNewFileList); //set operation flag
                }
            }
        }

        private void buttonExtractModifiedFiles_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                extractionPath = folderBrowserDialog1.SelectedPath;
                MypFileHandler.ExtractionPath = extractionPath;
                if (MypFileHandler != null)
                {
                    ExtractFiles(MypFileHandler.archiveModifiedFileList); //set operation flag
                }
            }
        }

        private void generatePatternButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Pattern File|*.txt";

            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            if (!SetOperationRunning()) return;

            hashCreator.SavePatterns(dlg.FileName);

            operationRunning = false;
        }

        public Thread t_GeneratePat;
        private void testPatternButton_Click(object sender, EventArgs e)
        {
            if (t_GeneratePat == null) //reset in event handler
            {
                openArchiveDialog.Filter = "Pattern File|*.txt";
                if (openArchiveDialog.ShowDialog() != DialogResult.OK)
                    return;

                hashCreator.loadPatterns(openArchiveDialog.FileName);

                //make a copy of the dictionary to avoid conflicts, with only unknown file name to speed up.
                patternDic = new HashDictionary("Hash/PatternDic.txt");
                foreach (KeyValuePair<long, HashData> kvp in hashDic.HashList)
                    if (kvp.Value.filename.CompareTo("") == 0)
                        patternDic.LoadHash(kvp.Value.ph, kvp.Value.sh, kvp.Value.filename, kvp.Value.crc);

                hashCreator.event_FilenameTest += FilenameTestEventHandler; //reset in eventhandler

                t_GeneratePat = new Thread(new ParameterizedThreadStart(hashCreator.Patterns));
                t_GeneratePat.Start(patternDic);
            }
            else
                MessageBox.Show("Already testing! Please wait for completion", "Please wait", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }

        #endregion
        #endregion

        #region UI_FileListing & TreeView
        private void FileListing_Add(FileInArchive file)
        {
            fileInArchiveBindingSource.Add(file);
        }

        private void Update_TreeView()
        {
            TreeViewManager.PopulateArchiveTreeNode(
                (SortableBindingList<FileInArchive>)fileInArchiveBindingSource.DataSource
                , treeView_Archive);
        }
        #endregion

        #region Delegates for cross threading
        delegate void del_NewFileTableEvent(MYPHandler.MYPFileTableEventArgs e);
        del_NewFileTableEvent OnNewFileTableEvent;

        delegate void del_NewExtractedEvent(MYPHandler.MYPFileEventArgs e);
        del_NewExtractedEvent OnNewExtractedEvent;

        delegate void del_NewFilenameTestEvent(nsHashCreator.MYPFilenameTestEventArgs e);
        del_NewFilenameTestEvent OnNewFilenameTestEvent;
        #endregion

        #region Event Treatment
        #region New File
        /// <summary>
        /// Receive all events related to the file table entries
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event args</param>
        private void FileTableEventHandler(object sender, MYPHandler.MYPFileTableEventArgs e)
        {
            //Check if an invoke is required by selecting a random component
            //that might be updated by this event
            if (label_NumOfFiles_Value.InvokeRequired)
            {
                Invoke(OnNewFileTableEvent, e);
            }
            else
            {
                TreatFileTableEvent(e);
            }
        }

        /// <summary>
        /// Treats an event in the UI thread
        /// </summary>
        /// <param name="e">event arguments</param>
        private void TreatFileTableEvent(MYPHandler.MYPFileTableEventArgs e)
        {
            if (e.Type == Event_FileTableType.FileError)
            {
                label_ReadingErrors_Value.Text = MypFileHandler.Error_FileEntryNumber.ToString();
            }
            else if (e.Type == Event_FileTableType.NewFile)
            {
                Update_OnFileTableEvent();
                FileListing_Add(e.ArchFile);
            }
            else if (e.Type == Event_FileTableType.UpdateFile)
            {
                Update_OnFileTableEvent();
            }
            else if (e.Type == Event_FileTableType.Finished)
            {
                if (MypFileHandler.archiveModifiedFileList.Count > 0 || MypFileHandler.archiveNewFileList.Count > 0)
                {
                    hashDic.SaveHashList();
                }
                fileInArchiveDataGridView.DataSource = fileInArchiveBindingSource;
                operationRunning = false;
                Update_TreeView();
            }
        }

        /// <summary>
        /// Updates all the labels regarding file table entries and progress bar
        /// </summary>
        private void Update_OnFileTableEvent()
        {
            label_NumOfFiles_Value.Text = MypFileHandler.NumberOfFilesFound.ToString("#,#");
            label_NumOfNamedFiles_Value.Text = MypFileHandler.NumberOfFileNamesFound.ToString("#,#");
            label_UncompressedSize_Value.Text = MypFileHandler.UnCompressedSize.ToString("#,#");
            label_ModifiedFiles_Value.Text = MypFileHandler.archiveModifiedFileList.Count.ToString("#,#");
            label_NewFiles_Value.Text = MypFileHandler.archiveNewFileList.Count.ToString("#,#");

            if (MypFileHandler.NumberOfFilesFound == MypFileHandler.TotalNumberOfFiles)
            {
                statusPB.Visible = false;
            }
            else
            {
                statusPB.Value = (int)MypFileHandler.NumberOfFilesFound;
            }
        }

        #endregion

        #region Extraction Event Treatment
        private void ExtractionEventHandler(object sender, MYPHandler.MYPFileEventArgs e)
        {
            if (label_ExtractionErrors_Text.InvokeRequired)
            {
                Invoke(OnNewExtractedEvent, e);
            }
            else
            {
                TreatExtractionEvent(e);
            }
        }

        /// <summary>
        /// Treat file extraction events in the UI thread
        /// </summary>
        /// <param name="e"></param>
        private void TreatExtractionEvent(MYPHandler.MYPFileEventArgs e)
        {
            switch (e.State)
            {
                case Event_ExtractionType.ExtractionFinished:
                    {
                        statusPB.Visible = false;
                        operationRunning = false;
                        UpdateLabel_OnExtraction(e.Value);
                        break;
                    }
                case Event_ExtractionType.FileExtractionError:
                    {
                        statusPB.Value = (int)e.Value;
                        UpdateLabel_OnExtractionError(e.Value);
                        break;
                    }
                case Event_ExtractionType.FileExtracted:
                    {
                        statusPB.Value = (int)e.Value;
                        UpdateLabel_OnExtraction(e.Value);
                        break;
                    }
                case Event_ExtractionType.Scanning:
                    {
                        statusPB.Value += 1;
                        if (scanFiles.Count != 0)
                        {
                            MypFileHandler = new MYPHandler.MYPHandler(scanFiles[0]
                                , null, ExtractionEventHandler
                                , hashDic);

                            scanFiles.RemoveAt(0);

                            MypFileHandler.Pattern = Pattern.Text;
                            t_worker = new Thread(new ThreadStart(MypFileHandler.ScanFileTable));
                            t_worker.Start();
                        }
                        else
                        {
                            resetOverall();
                            statusPB.Visible = false;
                            operationRunning = false;
                        }
                        break;
                    }
            }
        }

        private void UpdateLabel_OnExtraction(long numExtracted)
        {
            label_ExtractedFiles_Value.Text = numExtracted.ToString();
        }

        private void UpdateLabel_OnExtractionError(long numError)
        {
            label_ExtractionErrors_Value.Text = numError.ToString();
        }

        #endregion

        #region Filename Tests
        /// <summary>
        /// Receive all events related to the file table entries
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event args</param>
        private void FilenameTestEventHandler(object sender, nsHashCreator.MYPFilenameTestEventArgs e)
        {
            //Check if an invoke is required by selecting a random component
            //that might be updated by this event
            if (label_NumOfFiles_Value.InvokeRequired)
            {
                Invoke(OnNewFilenameTestEvent, e);
            }
            else
            {
                TreatFilenameTestEvent(e);
            }
        }

        /// <summary>
        /// Treats an event in the UI Thread. It calls the method corresponding to the event type
        /// </summary>
        /// <param name="e">event arguments</param>
        private void TreatFilenameTestEvent(nsHashCreator.MYPFilenameTestEventArgs e)
        {
            switch (e.State)
            {
                case Event_FilenameTestType.TestFinished:
                    {
                        hashCreator.event_FilenameTest -= FilenameTestEventHandler;
                        operationRunning = false;
                        statusPB.Visible = false;
                        if (e.Value != 0)
                            MessageBox.Show("You just found " + e.Value + " new filenames.", "Newly found filenames!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        else
                            MessageBox.Show("No new filenames.", "No new filenames", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        break;
                    }
                case Event_FilenameTestType.TestRunning:
                    {
                        statusPB.Value += (int)e.Value;
                        break;
                    }
                case Event_FilenameTestType.PatternRunning:
                    {
                        lblGeneratePat.Text = e.Value.ToString() + " %";
                        break;
                    }
                case Event_FilenameTestType.PatternFinished:
                    {
                        lblGeneratePat.Text = "Done";
                        if (e.Value != 0)
                        {
                            patternDic.SaveHashList("Hash/PatternDic.txt");
                            hashDic.MergeHashList("Hash/PatternDic.txt");
                            MessageBox.Show("Pattern Matching just found " + e.Value + " new filenames.", "Newly found filenames!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                        else
                            MessageBox.Show("No new filenames through Pattern matching.", "No new filenames", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        hashCreator.event_FilenameTest -= FilenameTestEventHandler;
                        t_GeneratePat = null;
                        break;
                    }
            }
        }
        #endregion


        #region Error Table Entry
        private void Error_TableEntry(FileInArchive file)
        {

        }

        #endregion

        #endregion

        int oldColumn = -1;
        SortOrder oldSortOrder = SortOrder.None;
        private void fileInArchiveDataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // Check which column is selected, otherwise set NewColumn to null.
            DataGridViewColumn newColumn =
                fileInArchiveDataGridView.Columns[e.ColumnIndex];

            ListSortDirection direction;

            // If oldColumn is null, then the DataGridView is not currently sorted.
            if (oldColumn != -1)
            {
                // Sort the same column again, reversing the SortOrder.
                if (oldColumn == e.ColumnIndex &&
                    oldSortOrder == SortOrder.Ascending)
                {
                    direction = ListSortDirection.Descending;
                }
                else
                {
                    // Sort a new column and remove the old SortGlyph.
                    direction = ListSortDirection.Ascending;
                    newColumn.HeaderCell.SortGlyphDirection = SortOrder.None;
                }
            }
            else
            {
                direction = ListSortDirection.Ascending;
            }

            fileInArchiveDataGridView.Sort(newColumn, direction);

            oldColumn = e.ColumnIndex;
            if (direction == ListSortDirection.Ascending)
            {
                oldSortOrder = SortOrder.Ascending;
            }
            else
            {
                oldSortOrder = SortOrder.Descending;
            }

            newColumn.HeaderCell.SortGlyphDirection =
                direction == ListSortDirection.Ascending ?
                SortOrder.Ascending : SortOrder.Descending;
        }

        private void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (MypFileHandler != null)
                MypFileHandler.Dispose();
            t_worker.Abort();

            if (hashDic.needsSave == true)
            {
                avBar = new AvancementBar();
                avBar.Text = "Saving hash list ...";
                hashDic.HashEvent += avBar.UpdateDictionaryEventHandler;
                t_worker = new Thread(new ThreadStart(hashDic.SaveHashList));
                t_worker.Start();

                avBar.ShowDialog();
                hashDic.HashEvent -= avBar.UpdateDictionaryEventHandler;
                avBar.Dispose();
            }
            Application.Exit();
        }

        private void findStrip_ItemFound(object sender, ItemFoundEventArgs e)
        {
            // If value found, select row
            if (e.Index >= 0)
            {
                this.fileInArchiveDataGridView.ClearSelection();
                this.fileInArchiveDataGridView.Rows[e.Index].Selected = true;

                // Change current list data source item
                // (to ensure currency across all controls
                // bound to this BindingSource)
                //this.fileInArchiveDataGridView.Po = e.Index;
                this.fileInArchiveDataGridView.CurrentCell = this.fileInArchiveDataGridView.Rows[e.Index].Cells[0];
            }
        }

        private void treeView_FileSystem_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeViewManager.SystemNodeMouseClick(sender, e);
        }


        #region Drag & Drop region
        private void fileInArchiveDataGridView_DragDrop(object sender, DragEventArgs e)
        {

        }

        private void treeView_Archive_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ((TreeView)sender).DoDragDrop(((FiaTreeNode)e.Item).fiaList, DragDropEffects.Copy);

            }
        }

        private void treeView_FileSystem_DragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(List<FileInArchive>))) return;
        }

        #endregion

        private void treeView_Archive_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ((TreeView)sender).DoDragDrop(((FiaTreeNode)e.Node).fiaList, DragDropEffects.Copy);
            }
        }

        private void contextMenuStripFileSystemTreeView_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text == "Extract")
            {
                if (treeView_Archive.SelectedNode != null)
                {
                    ExtractFileList(((FiaTreeNode)treeView_Archive.SelectedNode).fiaList);
                }
            }
        }

        private void ExtractFileList(List<FileInArchive> fileList)
        {
            if (fileList.Count > 0)
            {
                if (MypFileHandler != null)
                {
                    // operation lock is put by 'extract Files'
                    if (extractionPath == null)
                    {
                        if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                        {
                            extractionPath = folderBrowserDialog1.SelectedPath;
                            MypFileHandler.ExtractionPath = extractionPath;
                        }
                        else
                            return;
                    }
                    ExtractFiles(fileList);
                }
            }
            else
            {
                MessageBox.Show("Please select some files to extract", "Select Files", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


    }
}