using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using MYPHandler;
using nsHashCreator;
using nsHashDictionary;
using System.Configuration;

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
        MYPHandler.MYPHandler CurrentMypFH; //The class that does all the job of reading, extracting and writting to myp files
        SortedList<string, MYPHandler.MYPHandler> MypFHList = new SortedList<string, MYPHandler.MYPHandler>();

        public Thread t_worker; //worker thread because it is best that way so not to freeze the gui
        HashDictionary hashDic = new HashDictionary(); //The class that contains the dictionnary
        HashDictionary patternDic;
        HashCreator hashCreator;
        //List<FileInArchive> FIAList = new List<FileInArchive>();
        AvancementBar avBar;
        //string extractionPath = null;
        bool operationRunning = false;
        bool multipleFilesScan = false; // needed to avoir problems linked to scanFiles.Count() and it reaching 0

        List<String> scanFiles = new List<string>();

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
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
            OnLabelsReset = LabelsTextReset;
            OnLabelUpdate = _LabelUpdate;
            OnMenuActivation = _MenuActivation;
            OnProgressBarVisibilityUpdate = _ProgressBarVisibilityUpdate;

            // Populate the system tree view
            TreeViewManager.PopulateSystemTreeNode(treeView_FileSystem);
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~MainWindow()
        {
            for (int i = 0; i < MypFHList.Count; i++)
            {
                if (MypFHList.Values[i] != null)
                {
                    MypFHList.Values[i].Dispose();
                }
            }
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
            // Disable all the necessary menues
            // TODO: add the context menues to the disabled list (need a delegate)
            MenuStateSwitch(new EasyMYPMenuActivationEvent(false));

            return true;
        }

        private void OperationFinished()
        {
            operationRunning = false;

            // Reenable all the necessary menues
            // TODO: add the context menues to the reenabled list
            MenuStateSwitch(new EasyMYPMenuActivationEvent(true));
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

            OperationFinished(); //valid here because we are waiting for the dialog
        }

        #region Menu
        #region Archive Menu
        private void openArchiveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            multipleFilesScan = false;
            openArchiveDialog.Filter = "MYP Archives|*.myp;*.tor";
            if (openArchiveDialog.ShowDialog() == DialogResult.OK)
            {
                OpenArchive(openArchiveDialog.FileName);
            }
        }

        private void OpenArchive(string filename)
        {
            if (!SetOperationRunning()) return; //reset in eventhandler
            ResetOverall(); // Huge problem here, resetOverall sets the operationRunning thingy to false... bad!
            //fileInArchiveBindingSource.Clear(); //Clean the filelisting
            fileInArchiveDataGridView.DataSource = null; // switched by when Event_FileTableType.Finished is received.

            LabelTextUpdate(new EasyMYPUpdateLabelsEvent(label_File_Value
                , filename.Substring(filename.LastIndexOf('\\') + 1)));

            if (!MypFHList.Keys.Contains(filename))
            {
                //If we haven't open the file yet, we open it, set it as current, add it to the list
                CurrentMypFH = new MYPHandler.MYPHandler(filename
                    , FileTableEventHandler, ExtractionEventHandler
                    , hashDic);
                MypFHList.Add(filename, CurrentMypFH); //Beware here, the filename needs to be the full filename! Do not change it!
            }
            else
            {
                //Otherwise we load the old file
                CurrentMypFH = MypFHList[filename];
            }

            if (EasyMypConfig.ExtractionPath != null)
            {
                CurrentMypFH.ExtractionPath = EasyMypConfig.ExtractionPath;
            }

            ProgressBarVisibilityUpdate(new EasyMYPProgressBarVisibilityEvent(statusPB
                , true, (int)CurrentMypFH.TotalNumberOfFiles, 0));

            statusPB.Visible = true;

            LabelTextUpdate(new EasyMYPUpdateLabelsEvent(label_EstimatedNumOfFiles_Value
                , CurrentMypFH.TotalNumberOfFiles.ToString("#,#")));

            CurrentMypFH.Pattern = Pattern.Text;
            t_worker = new Thread(new ThreadStart(CurrentMypFH.GetFileTable));
            t_worker.Start();

        }

        private void closeArchiveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetOverall();
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

            OperationFinished();
        }

        private void testDirFilenameListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!SetOperationRunning()) return;

            openArchiveDialog.Filter = "File containing filenames to test|*.txt";
            if (openArchiveDialog.ShowDialog() == DialogResult.OK)
            {
                // supposed to be small enough to avoid threading.
                long newlyFound = hashCreator.ParseDirFilenames(openArchiveDialog.FileName
                    , Path.GetDirectoryName(openArchiveDialog.FileName) + '/' + "dirnames.txt");
            }

            OperationFinished();
        }

        /// <summary>
        /// Parses all the myp files to extract all the possible hashes.
        /// </summary>
        /// 
        private void scanAllMypsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            multipleFilesScan = true;
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                string[] mypfiles = Directory.GetFiles(folderBrowserDialog1.SelectedPath, "*.myp");
                if (mypfiles.Length == 0)
                {
                    MessageBox.Show("No myp files found in folder", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!SetOperationRunning()) return; //reset in event handler

                ResetOverall();
                foreach (string file in mypfiles)
                {
                    scanFiles.Add(file);
                }

                statusPB.Maximum = (int)mypfiles.Length;
                statusPB.Value = 0;
                statusPB.Visible = true;

                if (!MypFHList.Keys.Contains(scanFiles[0]))
                {
                    //If we haven't open the file yet, we open it, set it as current, add it to the list
                    CurrentMypFH = new MYPHandler.MYPHandler(scanFiles[0]
                        , FileTableEventHandler, ExtractionEventHandler
                        , hashDic);
                    MypFHList.Add(scanFiles[0], CurrentMypFH);
                }
                else
                {
                    //Otherwise we load the old file
                    CurrentMypFH = MypFHList[scanFiles[0]];
                }

                scanFiles.RemoveAt(0);

                CurrentMypFH.Pattern = Pattern.Text;
                t_worker = new Thread(new ThreadStart(CurrentMypFH.ScanFileTable));
                t_worker.Start();
            }
        }

        /// <summary>
        /// Parses all the myp files to extract all the possible hashes.
        /// </summary>
        /// 
        private void scanAllTorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            multipleFilesScan = true;
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                string[] mypfiles = Directory.GetFiles(folderBrowserDialog1.SelectedPath, "*.tor");
                if (mypfiles.Length == 0)
                {
                    MessageBox.Show("No tor files found in folder", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!SetOperationRunning()) return; //reset in event handler

                ResetOverall();
                foreach (string file in mypfiles)
                {
                    scanFiles.Add(file);
                }

                statusPB.Maximum = (int)mypfiles.Length;
                statusPB.Value = 0;
                statusPB.Visible = true;

                if (!MypFHList.Keys.Contains(scanFiles[0]))
                {
                    //If we haven't open the file yet, we open it, set it as current, add it to the list
                    CurrentMypFH = new MYPHandler.MYPHandler(scanFiles[0]
                        , FileTableEventHandler, ExtractionEventHandler
                        , hashDic);
                    MypFHList.Add(scanFiles[0], CurrentMypFH);
                }
                else
                {
                    //Otherwise we load the old file
                    CurrentMypFH = MypFHList[scanFiles[0]];
                }

                scanFiles.RemoveAt(0);

                CurrentMypFH.Pattern = Pattern.Text;
                t_worker = new Thread(new ThreadStart(CurrentMypFH.ScanFileTable));
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

                OperationFinished();
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
                EasyMypConfig.ExtractionPath = folderBrowserDialog1.SelectedPath;

                if (CurrentMypFH != null)
                {
                    CurrentMypFH.ExtractionPath = EasyMypConfig.ExtractionPath;
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
            SortedList<string, List<FileInArchive>> multiFileList = new SortedList<string, List<FileInArchive>>();

            foreach (DataGridViewRow theRow in fileInArchiveDataGridView.SelectedRows)
            {
                if (theRow.DataBoundItem != null)
                {
                    MYPHandler.FileInArchive fiaFile = (MYPHandler.FileInArchive)theRow.DataBoundItem;
                    if (!multiFileList.Keys.Contains(fiaFile.sourceFileName))
                    {
                        multiFileList.Add(fiaFile.sourceFileName, new List<FileInArchive>());
                    }
                    multiFileList[fiaFile.sourceFileName].Add(fiaFile);
                }
            }

            Thread t_multiExtract = new Thread(new ParameterizedThreadStart(MultiListExtraction));
            t_multiExtract.Start(multiFileList);
        }

        private void MultiListExtraction(object obj)
        {
            SortedList<string, List<FileInArchive>> multiFileList = (SortedList<string, List<FileInArchive>>)obj;

            for (int i = 0; i < multiFileList.Count; i++)
            {
                if (MypFHList.Keys.Contains(multiFileList.Keys[i]))
                {
                    if (MypFHList[multiFileList.Keys[i]] != null)
                    {
                        ExtractFiles(MypFHList[multiFileList.Keys[i]], multiFileList.Values[i]);
                    }
                    while (operationRunning)
                    {
                        Thread.Sleep(10);
                    }
                }
            }
        }

        private void ExtractFiles(MYPHandler.MYPHandler mypHandler
            , List<FileInArchive> fileList)
        {
            //Check if an extraction path is set
            if (EasyMypConfig.ExtractionPath == null || mypHandler != null && mypHandler.ExtractionPath == "")
            {
                if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    EasyMypConfig.ExtractionPath = folderBrowserDialog1.SelectedPath;
                    mypHandler.ExtractionPath = EasyMypConfig.ExtractionPath;
                }
                else
                    return;
            }

            if (mypHandler != null)
            {
                if (!SetOperationRunning()) return; //reset in event

                ProgressBarVisibilityUpdate(new EasyMYPProgressBarVisibilityEvent(statusPB
                    , true, fileList.Count, 0));

                t_worker = new Thread(new ParameterizedThreadStart(mypHandler.Extract));
                t_worker.Start(fileList);
            }
        }

        /// <summary>
        /// Extracts now all the files that were loaded!
        /// Beware!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void extractAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Thread t_Extract = new Thread(new ParameterizedThreadStart(ThreadedExtractFiles));
            t_Extract.Start(ExtractionType.All);
        }

        /// <summary>
        /// Extracts all the new possible files!
        /// Beware!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonExtractNewFiles_Click(object sender, EventArgs e)
        {
            Thread t_Extract = new Thread(new ParameterizedThreadStart(ThreadedExtractFiles));
            t_Extract.Start(ExtractionType.New);
        }

        private void buttonExtractModifiedFiles_Click(object sender, EventArgs e)
        {
            Thread t_Extract = new Thread(new ParameterizedThreadStart(ThreadedExtractFiles));
            t_Extract.Start(ExtractionType.Modified);
        }

        enum ExtractionType
        {
            All,
            Modified,
            New
        }

        private void ThreadedExtractFiles(object obj)
        {
            for (int i = 0; i < MypFHList.Count; i++)
            {
                if (MypFHList.Values[i] != null)
                {
                    if ((ExtractionType)obj == ExtractionType.All)
                    {
                        ExtractFiles(MypFHList.Values[i], MypFHList.Values[i].archiveFileList); // set the operation flag
                    }
                    else if ((ExtractionType)obj == ExtractionType.Modified)
                    {
                        ExtractFiles(MypFHList.Values[i], MypFHList.Values[i].archiveModifiedFileList); // set the operation flag
                    }
                    else
                    {
                        ExtractFiles(MypFHList.Values[i], MypFHList.Values[i].archiveNewFileList); // set the operation flag
                    }
                    while (operationRunning)
                    {
                        Thread.Sleep(10);
                    }
                }
            }
        }

        private void extractFileListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentMypFH != null)
            {
                if (EasyMypConfig.ExtractionPath == null)
                {
                    if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                    {
                        EasyMypConfig.ExtractionPath = folderBrowserDialog1.SelectedPath;
                        CurrentMypFH.ExtractionPath = EasyMypConfig.ExtractionPath;
                    }
                    else
                        return;
                }
                CurrentMypFH.DumpFileList();
            }
        }

        /// <summary>
        /// Todo: Correct this section so that it replaces the file in the correct myp
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void replaceSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentMypFH != null)
            {
                if (replaceFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filename = replaceFileDialog.FileName; //get filename selected

                    if (!SetOperationRunning()) return;

                    CurrentMypFH.ReplaceFile((MYPHandler.FileInArchive)fileInArchiveBindingSource.Current
                        , new FileStream(filename, FileMode.Open));

                    OperationFinished();
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

            OperationFinished();
        }

        public Thread t_GeneratePat;
        private void testPatternButton_Click(object sender, EventArgs e)
        {
            if (t_GeneratePat == null) //reset in event handler
            {
                openArchiveDialog.Filter = "Pattern File|*.txt";
                if (openArchiveDialog.ShowDialog() != DialogResult.OK)
                    return;

                testPatternButton.Enabled = false;

                hashCreator.loadPatterns(openArchiveDialog.FileName);

                //make a copy of the dictionary to avoid conflicts, with only unknown file name to speed up.
                patternDic = new HashDictionary("Hash/PatternDic.txt");
                foreach (KeyValuePair<long, HashData> kvp in hashDic.HashList)
                    if (kvp.Value.filename.CompareTo("") == 0)
                        patternDic.LoadHash(kvp.Value.ph, kvp.Value.sh, kvp.Value.filename, kvp.Value.crc);

                hashCreator.event_FilenameTest += FilenameTestEventHandler; //reset in eventhandler

                t_GeneratePat = new Thread(new ParameterizedThreadStart(hashCreator.Patterns));
                t_GeneratePat.Start(patternDic);


                button_Pause.Enabled = true;
                button_Stop.Enabled = true;
            }
            else
                MessageBox.Show("Already testing! Please wait for completion", "Please wait", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Pauses the pattern test
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Pause_Click(object sender, EventArgs e)
        {
            if (hashCreator != null && hashCreator.Active)
            {
                if (hashCreator.Paused)
                {
                    hashCreator.Resume();
                    lblGeneratePat.Text = "Resuming ...";
                    button_Pause.Text = "Pause";
                }
                else
                {
                    hashCreator.Pause();
                    lblGeneratePat.Text = "Paused";
                    button_Pause.Text = "Resume";
                }
            }
        }

        /// <summary>
        /// Stops the pattern testing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Stop_Click(object sender, EventArgs e)
        {
            if (hashCreator != null && hashCreator.Active)
            {
                hashCreator.Stop();
                lblGeneratePat.Text = "Stopped";
                button_Pause.Text = "Pause"; // Just in case it is set to resume :)

                testPatternButton.Enabled = true;
                button_Pause.Enabled = false;
                button_Stop.Enabled = false;
            }
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
            TreeViewManager.PopulateArchiveTreeNode(CurrentMypFH.fullMypFileName
                , CurrentMypFH.archiveFileList
                , treeView_Archive);
        }
        #endregion



        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Preferences pref = new Preferences();
            if (pref.ShowDialog() == DialogResult.OK)
            {
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About about = new About();
            about.Show();
        }

        private void treeView_Archive_MouseUp(object sender, MouseEventArgs e)
        {
            treeView_Archive.SelectedNode = treeView_Archive.GetNodeAt(treeView_Archive.PointToClient(Control.MousePosition));
            if (treeView_Archive.SelectedNode != null)
            {
                ((TreeView)sender).DoDragDrop(((FiaTreeNode)treeView_Archive.SelectedNode).fiaList, DragDropEffects.Copy);
            }
        }
    }
}