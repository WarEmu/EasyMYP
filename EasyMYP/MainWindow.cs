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
        //List<FileInArchive> FIAList = new List<FileInArchive>();
        AvancementBar avBar;
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            //Building the dictionnary
            t_worker = new Thread(new ThreadStart(hasher.BuildHashList));
            t_worker.Start();

            //Define functions that should be called to treat events
            //Needed because of cross thread calls
            OnNewExtractedEvent = TreatExtractionEvent;
            OnNewFileTableEvent = TreatFileTableEvent;

            //Wait until the dictionnary is loaded
            //Show a progress bar
            avBar = new AvancementBar();
            avBar.Text = "Loading hash list ...";
            hasher.HashEvent += avBar.UpdateHashEventHandler;
            avBar.ShowDialog();
            hasher.HashEvent -= avBar.UpdateHashEventHandler;
            avBar.Dispose();
            //hasher.SaveHashList();
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

        #region Edit Menu
        private void searchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (worker != null)
            {
                worker.SearchForFile("");
            }
        }

        private void writeToArchiveToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (worker != null)
            {

            }
        }
        #endregion

        #region Tools Menu
        private void createDirectoryStructureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            status = State.CreatingDirectoryStructure;

            status = State.waiting;
        }

        private void recalculateHashesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            status = State.RebuildingHashList;

            status = State.waiting;
        }
        #endregion

        #region File Menu
        private void selectExtractionFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                worker.ExtractionPath = folderBrowserDialog1.SelectedPath;

            }
        }

        private void extractSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fileInArchiveBindingSource.Current != null)
            {
                worker.ExtractFile((MYPWorker.FileInArchive)fileInArchiveBindingSource.Current);
            }
        }

        private void extractAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //TODO: if extraction path is not set put an alert up ?
            if (worker != null)
            {
                t_worker = new Thread(new ThreadStart(worker.ExtractAll));
                t_worker.Start();

                //Show a progress bar
                avBar = new AvancementBar();
                avBar.Text = "Extracting";
                avBar.ShowDialog();
                avBar.Dispose();
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

        private void panel_status_Paint(object sender, PaintEventArgs e)
        {

        }

        #endregion

        private void fileInArchiveDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void LabelFilter_Click(object sender, EventArgs e)
        {

        }

        private void panel_output_Paint(object sender, PaintEventArgs e)
        {

        }

        private void fileInArchiveDataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            return;
            // Check which column is selected, otherwise set NewColumn to null.
            DataGridViewColumn newColumn =                              
                fileInArchiveDataGridView.SelectedColumns.Count > 0 ?
                fileInArchiveDataGridView.SelectedColumns[0] : fileInArchiveDataGridView.Columns[0];
 
            DataGridViewColumn oldColumn = fileInArchiveDataGridView.SortedColumn;
            ListSortDirection direction;
 
            // If oldColumn is null, then the DataGridView is not currently sorted.
            if (oldColumn != null)
            {
                // Sort the same column again, reversing the SortOrder.
                if (oldColumn == newColumn &&
                    fileInArchiveDataGridView.SortOrder == SortOrder.Ascending)
                {
                    direction = ListSortDirection.Descending;
                }
                else
                {
                    // Sort a new column and remove the old SortGlyph.
                    direction = ListSortDirection.Ascending;
                    oldColumn.HeaderCell.SortGlyphDirection = SortOrder.None;
                }
            }
            else
            {
                direction = ListSortDirection.Ascending;
            }

            fileInArchiveDataGridView.Sort(newColumn, direction);

            
            newColumn.HeaderCell.SortGlyphDirection =
                direction == ListSortDirection.Ascending ?
                SortOrder.Ascending : SortOrder.Descending;
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {

        }

        private void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (worker != null)
                worker.Dispose();
            t_worker.Abort();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

    }
}