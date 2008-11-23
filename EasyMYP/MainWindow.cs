using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using MYPWorker;
using WarhammerOnlineHashBuilder;
using nsHashCreator;


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
        State status = State.waiting;
        MYPWorker.MYPWorker worker; //The class that does all the job of reading, extracting and writting to myp files
        public Thread t_worker; //worker thread because it is best that way so not to freeze the gui
        Hasher hasher = new Hasher(false); //The class that contains the dictionnary
        HashCreator hashCreator = new HashCreator();
        //List<FileInArchive> FIAList = new List<FileInArchive>();
        AvancementBar avBar;
        string extractionPath = null;
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            LoadDictionnary(false, "");
            fileInArchiveBindingSource.DataSource = new SortableBindingList<FileInArchive>();
        }

        void LoadDictionnary(bool merge, string fileToMerge)
        {
            //Show a progress bar
            avBar = new AvancementBar();
            avBar.Text = "Loading hash list ...";
            hasher.HashEvent += avBar.UpdateHashEventHandler;

            //Building the dictionnary
            if (!merge)
            {
                t_worker = new Thread(new ThreadStart(hasher.BuildHashList));
                t_worker.Start();
            }
            else
            {
                t_worker = new Thread(new ParameterizedThreadStart(hasher.MergeHashList));
                t_worker.Start(fileToMerge);
            }

            //Define functions that should be called to treat events
            //Needed because of cross thread calls
            OnNewExtractedEvent = TreatExtractionEvent;
            OnNewFileTableEvent = TreatFileTableEvent;

            //Wait until the dictionnary is loaded
            avBar.ShowDialog();
            hasher.HashEvent -= avBar.UpdateHashEventHandler;
            avBar.Dispose();
            //hasher.SaveHashList();

            hashCreator.InitializeHashList(hasher);
        }

        #region Menu
        #region Archive Menu
        private void createToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        public void resetOverall()
        {
            if (t_worker != null)
                t_worker.Abort();
            if (worker != null)
                worker.Dispose();
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
            Loading.Visible = false;
        }

        private void openArchiveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openArchiveDialog.Filter = "MYP Archives|*.myp";
            if (openArchiveDialog.ShowDialog() == DialogResult.OK)
            {
                resetOverall();
                fileInArchiveBindingSource.Clear(); //Clean the filelisting

                string filename = openArchiveDialog.FileName; //get filename selected
                int filenameStartPosition = filename.LastIndexOf('\\');
                label_File_Value.Text = filename.Substring(filenameStartPosition + 1, filename.Length - filenameStartPosition - 1);

                if (worker != null) worker.Dispose();
                worker = new MYPWorker.MYPWorker(filename
                    , FileTableEventHandler, ExtractionEventHandler
                    , hasher);

                if (extractionPath != null)
                {
                    worker.ExtractionPath = extractionPath;
                }

                Loading.Maximum = (int)worker.TotalNumberOfFiles;
                Loading.Value = 0;
                Loading.Visible = true;
                label_EstimatedNumOfFiles_Value.Text = worker.TotalNumberOfFiles.ToString();

                worker.Pattern = Pattern.Text;
                t_worker = new Thread(new ThreadStart(worker.GetFileTable));
                t_worker.Start();
            }
        }

        private void closeArchiveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            resetOverall();
            //            if (worker != null)
            //            {
            //                worker.Dispose(); //releases the worker if set
            //            }
            //fileInArchiveBindingSource.Clear(); //Clean the filelisting
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region Tools Menu
        private void mergeDictionaryFile_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openArchiveDialog.Filter = "Dictionary File|*.txt";
            if (openArchiveDialog.ShowDialog() == DialogResult.OK)
            {
                //Show a progress bar
                avBar = new AvancementBar();
                avBar.Text = "Merging hash list ...";
                hasher.HashEvent += avBar.UpdateHashEventHandler;
                t_worker = new Thread(new ParameterizedThreadStart(hasher.MergeHashList));
                t_worker.Start(openArchiveDialog.FileName);
                //Wait until the dictionnary is loaded
                avBar.ShowDialog();
                hasher.HashEvent -= avBar.UpdateHashEventHandler;
                avBar.Dispose();
            }
        }

        private void testFilenameListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openArchiveDialog.Filter = "File containing filenames to test|*.txt";
            if (openArchiveDialog.ShowDialog() == DialogResult.OK)
            {
                hashCreator.ParseDirAndFilenames(openArchiveDialog.FileName);
            }
        }

        private void testFullFilenameListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openArchiveDialog.Filter = "File containing full filenames to test|*.txt";
            if (openArchiveDialog.ShowDialog() == DialogResult.OK)
            {
                hashCreator.ParseFilenames(openArchiveDialog.FileName);
            }
        }
        #endregion

        #region File Menu
        private void selectExtractionFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                extractionPath = folderBrowserDialog1.SelectedPath;
                if (worker != null)
                {
                    worker.ExtractionPath = extractionPath;
                }
            }
        }

        private void extractSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (extractionPath == null)
            {
                if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    extractionPath = folderBrowserDialog1.SelectedPath;
                    worker.ExtractionPath = extractionPath;
                    if (fileInArchiveBindingSource.Current != null)
                    {
                        worker.ExtractFile((MYPWorker.FileInArchive)fileInArchiveBindingSource.Current);
                    }
                }
            }
            else
            {
                if (fileInArchiveBindingSource.Current != null)
                {
                    worker.ExtractFile((MYPWorker.FileInArchive)fileInArchiveBindingSource.Current);
                }
            }
        }

        private void ExtractFiles(List<FileInArchive> fileList)
        {
            if (worker != null)
            {
                t_worker = new Thread(new ParameterizedThreadStart(worker.ExtractFileList));
                t_worker.Start(fileList);

                //Show a progress bar
                avBar = new AvancementBar();
                avBar.Text = "Extracting";
                avBar.ShowDialog();
                avBar.Dispose();
            }
        }

        private void extractAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //TODO: if extraction path is not set put an alert up ?
            if (worker != null)
            {
                if (extractionPath == null)
                {
                    if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                    {
                        extractionPath = folderBrowserDialog1.SelectedPath;
                        worker.ExtractionPath = extractionPath;
                        ExtractFiles(worker.archiveFileList);
                    }
                }
                else
                {
                    //t_worker = new Thread(new ThreadStart(worker.ExtractAll));
                    //t_worker.Start();

                    ////Show a progress bar
                    //avBar = new AvancementBar();
                    //avBar.Text = "Extracting";
                    //avBar.ShowDialog();
                    //avBar.Dispose();

                    ExtractFiles(worker.archiveFileList);
                }
            }
        }

        private void replaceSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (replaceFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filename = replaceFileDialog.FileName; //get filename selected

                worker.ReplaceFile((MYPWorker.FileInArchive)fileInArchiveBindingSource.Current
                    , new FileStream(filename, FileMode.Open));
            }
        }

        #endregion

        private void buttonExtractNewFiles_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                extractionPath = folderBrowserDialog1.SelectedPath;
                worker.ExtractionPath = extractionPath;
                if (worker != null)
                {
                    ExtractFiles(worker.archiveNewFileList);
                }
            }
        }

        private void buttonExtractModifiedFiles_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                extractionPath = folderBrowserDialog1.SelectedPath;
                worker.ExtractionPath = extractionPath;
                if (worker != null)
                {
                    ExtractFiles(worker.archiveModifiedFileList);
                }
            }
        }
        public Thread t_GenerateFNE;
        private void buttonGenerateFilenames_CurrentFiles_Click(object sender, EventArgs e)
        {
            if (hashCreator != null)
            {
                t_GenerateFNE = new Thread(hashCreator.ParseDirFilenamesAndExtension);
                t_GenerateFNE.Start();
                //hashCreator.ParseDirFilenamesAndExtension();
            }
        }
        public Thread t_GeneratePat;
        private void buttonGenerateFilenames_OnPattern_Click(object sender, EventArgs e)
        {
            if (hashCreator != null)
            {
                t_GeneratePat = new Thread(hashCreator.Patterns);
                t_GeneratePat.Start();
            }
        }

        #endregion

        #region UI_FileListing
        private void FileListing_Add(FileInArchive file)
        {
            fileInArchiveBindingSource.Add(file);
        }
        #endregion

        #region Delegates for events
        delegate void del_NewFileTableEvent(MYPWorker.MYPFileTableEventArgs e);
        del_NewFileTableEvent OnNewFileTableEvent;

        delegate void del_NewExtractedEvent(MYPWorker.MYPFileEventArgs e);
        del_NewExtractedEvent OnNewExtractedEvent;
        #endregion

        #region Event Treatment
        #region New File
        /// <summary>
        /// Receive all events related to the file table entries
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event args</param>
        private void FileTableEventHandler(object sender, MYPWorker.MYPFileTableEventArgs e)
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
        /// Treats an event. It calls the method corresponding to the event type
        /// </summary>
        /// <param name="e">event arguments</param>
        private void TreatFileTableEvent(MYPWorker.MYPFileTableEventArgs e)
        {
            if (e.Type == Event_FileTableType.FileError)
            {
                label_ReadingErrors_Value.Text = worker.Error_FileEntryNumber.ToString();
            }
            else if (e.Type == Event_FileTableType.NewFile)
            {
                Update_OnFileTableEvent();
                FileListing_Add(e.ArchFile);
                //treeView_Archive.Nodes.

            }
            else if (e.Type == Event_FileTableType.UpdateFile)
            {
                Update_OnFileTableEvent();
            }
            else if (e.Type == Event_FileTableType.Finished)
            {
                if (worker.archiveModifiedFileList.Count > 0 || worker.archiveNewFileList.Count > 0)
                {
                    worker.SaveHashList();
                }
            }
        }

        /// <summary>
        /// Updates all the labels regarding file table entries
        /// Adds the file to the datagrid
        /// </summary>
        /// <param name="file">The file to add to the datagrid</param>
        private void Update_OnFileTableEvent()
        {
            label_NumOfFiles_Value.Text = worker.NumberOfFilesFound.ToString();
            label_NumOfNamedFiles_Value.Text = worker.NumberOfFileNamesFound.ToString();
            label_UncompressedSize_Value.Text = worker.UnCompressedSize.ToString();
            label_ModifiedFiles_Value.Text = worker.archiveModifiedFileList.Count.ToString();
            label_NewFiles_Value.Text = worker.archiveNewFileList.Count.ToString();

            if (worker.NumberOfFilesFound == worker.TotalNumberOfFiles)
            {
                Loading.Visible = false;
            }
            else
            {
                Loading.Value = (int)worker.NumberOfFilesFound;
            }
        }

        #endregion

        #region Extraction Event Treatment
        private void ExtractionEventHandler(object sender, MYPWorker.MYPFileEventArgs e)
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

        private void TreatExtractionEvent(MYPWorker.MYPFileEventArgs e)
        {
            if (e.State == Event_ExtractionType.ExtractionFinished)
            {
                if (avBar != null) avBar.UpdateOnEvent(new ProgressEventArgs(100f, true));
                Update_OnExtraction(e.Value);
            }
            else if (e.State == Event_ExtractionType.FileExtractionError)
            {
                Update_OnExtractionError(e.Value);
            }
            else if (e.State == Event_ExtractionType.FileExtracted)
            {
                if (avBar != null)
                    avBar.UpdateOnEvent(new ProgressEventArgs((float)e.Value / (float)worker.TotalNumberOfFiles, false));
                Update_OnExtraction(e.Value);
            }
            else if (e.State == Event_ExtractionType.UnknownCompressionMethod)
            {
            }
            else if (e.State == Event_ExtractionType.UnknownError)
            {
            }
        }

        private void Update_OnExtraction(long numExtracted)
        {
            label_ExtractedFiles_Value.Text = numExtracted.ToString();
        }

        private void Update_OnExtractionError(long numError)
        {
            label_ExtractionErrors_Value.Text = numError.ToString();
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
            if (worker != null)
                worker.Dispose();
            t_worker.Abort();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
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

        ThreadState oldt_GenerateFNE = ThreadState.Unstarted;
        ThreadState oldt_GeneratePat = ThreadState.Unstarted;

        private void tabPage3_Paint(object sender, PaintEventArgs e)
        {
            if (t_GenerateFNE != null)
            {
                if (t_GenerateFNE.ThreadState == ThreadState.Running && oldt_GenerateFNE != t_GenerateFNE.ThreadState)
                {
                    lblGenerateFNE.Text = "Running";
                }
                if (t_GenerateFNE.ThreadState != ThreadState.Running
                    && oldt_GenerateFNE != t_GenerateFNE.ThreadState
                    && t_GenerateFNE.ThreadState != ThreadState.WaitSleepJoin)
                {
                    lblGenerateFNE.Text = "Inactive";
                }
            }
            if (t_GeneratePat != null)
            {
                if (t_GeneratePat.ThreadState == ThreadState.Running && oldt_GeneratePat != t_GeneratePat.ThreadState)
                {
                    lblGeneratePat.Text = "Running";
                }
                if (t_GeneratePat.ThreadState != ThreadState.Running 
                    && oldt_GeneratePat != t_GeneratePat.ThreadState
                    && t_GeneratePat.ThreadState != ThreadState.WaitSleepJoin)
                {
                    lblGeneratePat.Text = "Inactive";
                }
            }
        }



    }
}