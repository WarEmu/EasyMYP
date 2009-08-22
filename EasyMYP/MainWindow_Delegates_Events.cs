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
    /// <summary>
    /// Event Argument class for file table events
    /// </summary>
    public class EasyMYPUpdateLabelsEvent : EventArgs
    {
        Label label;
        string text;

        public Label Label { get { return label; } }
        public string Text { get { return text; } }

        public EasyMYPUpdateLabelsEvent(Label label, string text)
        {
            this.label = label;
            this.text = text;
        }
    }

    /// <summary>
    /// Event Argument class to lock the menues
    /// </summary>
    public class EasyMYPMenuActivationEvent : EventArgs
    {
        bool enabled;
        public bool Enabled { get { return enabled; } }

        public EasyMYPMenuActivationEvent(bool enabled)
        {
            this.enabled = enabled;
        }
    }

    /// <summary>
    /// Event Argument to specify the visibility of a progree bar.
    /// </summary>
    public class EasyMYPProgressBarVisibilityEvent : EventArgs
    {
        bool visible;
        ProgressBar pb;
        int maximum;
        int value;

        public bool Visible { get { return visible; } }
        public ProgressBar ProgressBar { get { return pb; } }
        public int Max { get { return maximum; } }
        public int Value { get { return value; } }

        public EasyMYPProgressBarVisibilityEvent(ProgressBar pb, bool visible, int maximum, int value)
        {
            this.pb = pb;
            this.visible = visible;
            this.maximum = maximum;
            this.value = value;
        }
    }

    public partial class MainWindow : Form
    {
        #region Delegates for cross threading
        delegate void del_NewFileTableEvent(MYPHandler.MYPFileTableEventArgs e);
        del_NewFileTableEvent OnNewFileTableEvent;

        delegate void del_NewExtractedEvent(MYPHandler.MYPFileEventArgs e);
        del_NewExtractedEvent OnNewExtractedEvent;

        delegate void del_NewFilenameTestEvent(nsHashCreator.MYPFilenameTestEventArgs e);
        del_NewFilenameTestEvent OnNewFilenameTestEvent;

        delegate void del_LabelsReset();
        del_LabelsReset OnLabelsReset;

        delegate void del_LabelUpdate(EasyMYPUpdateLabelsEvent e);
        del_LabelUpdate OnLabelUpdate;

        delegate void del_ProgressBarVisibilityUpdate(EasyMYPProgressBarVisibilityEvent e);
        del_ProgressBarVisibilityUpdate OnProgressBarVisibilityUpdate;

        delegate void del_MenuActivation(EasyMYPMenuActivationEvent e);
        del_MenuActivation OnMenuActivation;
        #endregion

        #region Event Treatment
        #region Labels

        /// <summary>
        /// Used to reset all the stuff that needs reset when we are opening a new file.
        /// Should create a pane with overall stats :)
        /// </summary>
        public void ResetOverall()
        {
            CurrentMypFH = null;

            if (label_File_Value.InvokeRequired)
            {
                Invoke(OnLabelsReset);
            }
            else
            {
                LabelsTextReset();
            }
        }

        /// <summary>
        /// Resets the text of the labels
        /// </summary>
        private void LabelsTextReset()
        {
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
        }

        /// <summary>
        /// Updates the text of a specific label
        /// Call this method from an alternate thread.
        /// </summary>
        /// <param name="e"></param>
        private void LabelTextUpdate(EasyMYPUpdateLabelsEvent e)
        {
            if (e.Label.InvokeRequired)
            {
                Invoke(OnLabelUpdate, e);
            }
            else
            {
                _LabelUpdate(e);
            }
        }

        /// <summary>
        /// The method that updates the label.
        /// </summary>
        /// <param name="e"></param>
        private void _LabelUpdate(EasyMYPUpdateLabelsEvent e)
        {
            e.Label.Text = e.Text;
        }

        private void MenuStateSwitch(EasyMYPMenuActivationEvent e)
        {
            if (contextMenuStripFileSystemTreeView.InvokeRequired)
            {
                Invoke(OnMenuActivation, e);
            }
            else
            {
                _MenuActivation(e);
            }
        }

        private void _MenuActivation(EasyMYPMenuActivationEvent e)
        {
            extractAllToolStripMenuItem.Enabled = e.Enabled;
            extractFileListToolStripMenuItem.Enabled = e.Enabled;
            extractSelectedToolStripMenuItem.Enabled = e.Enabled;
            replaceSelectedToolStripMenuItem.Enabled = e.Enabled;
            contextMenuStripFileSystemTreeView.Enabled = e.Enabled;

            if (e.Enabled)
            {
                ProgressBarVisibilityUpdate(new EasyMYPProgressBarVisibilityEvent(statusPB, false, 0, 0));
            }
        }

        /// <summary>
        /// Updates the text of a specific label
        /// Call this method from an alternate thread.
        /// </summary>
        /// <param name="e"></param>
        private void ProgressBarVisibilityUpdate(EasyMYPProgressBarVisibilityEvent e)
        {
            if (e.ProgressBar.InvokeRequired)
            {
                Invoke(OnProgressBarVisibilityUpdate, e);
            }
            else
            {
                _ProgressBarVisibilityUpdate(e);
            }
        }

        private void _ProgressBarVisibilityUpdate(EasyMYPProgressBarVisibilityEvent e)
        {
            e.ProgressBar.Visible = e.Visible;
            e.ProgressBar.Maximum = e.Max;
            e.ProgressBar.Value = e.Value;
        }

        #endregion

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

        int modulus = 0; //an int to not invoke the winform update every time
        /// <summary>
        /// Treats an event in the UI thread
        /// </summary>
        /// <param name="e">event arguments</param>
        private void TreatFileTableEvent(MYPHandler.MYPFileTableEventArgs e)
        {
            modulus++;
            if (e.Type == Event_FileTableType.FileError)
            {
                label_ReadingErrors_Value.Text = CurrentMypFH.Error_FileEntryNumber.ToString();
            }
            else if (e.Type == Event_FileTableType.NewFile)
            {
                if (modulus % 1000 == 0)
                {
                    Update_OnFileTableEvent();
                }
                FileListing_Add(e.ArchFile);
            }
            else if (e.Type == Event_FileTableType.UpdateFile)
            {
                if (modulus % 1000 == 0)
                {
                    Update_OnFileTableEvent();
                }
            }
            else if (e.Type == Event_FileTableType.Finished)
            {
                //Final update
                Update_OnFileTableEvent();
                if (CurrentMypFH.archiveModifiedFileList.Count > 0 || CurrentMypFH.archiveNewFileList.Count > 0)
                {
                    hashDic.SaveHashList();
                }
                fileInArchiveDataGridView.DataSource = fileInArchiveBindingSource;
                //fileInArchiveDataGridView.Show();
                Update_TreeView();
                OperationFinished();
            }
        }

        /// <summary>
        /// Updates all the labels regarding file table entries and progress bar
        /// </summary>
        private void Update_OnFileTableEvent()
        {
            label_NumOfFiles_Value.Text = CurrentMypFH.NumberOfFilesFound.ToString("#,#");
            label_NumOfNamedFiles_Value.Text = CurrentMypFH.NumberOfFileNamesFound.ToString("#,#");
            label_UncompressedSize_Value.Text = CurrentMypFH.UnCompressedSize.ToString("#,#");
            label_ModifiedFiles_Value.Text = CurrentMypFH.archiveModifiedFileList.Count.ToString("#,#");
            label_NewFiles_Value.Text = CurrentMypFH.archiveNewFileList.Count.ToString("#,#");

            if (CurrentMypFH.NumberOfFilesFound == CurrentMypFH.TotalNumberOfFiles)
            {
                statusPB.Visible = false;
            }
            else
            {
                statusPB.Value = (int)CurrentMypFH.NumberOfFilesFound;
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
                        OperationFinished();
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
                            CurrentMypFH = new MYPHandler.MYPHandler(scanFiles[0]
                                , FileTableEventHandler, ExtractionEventHandler
                                , hashDic);

                            scanFiles.RemoveAt(0);

                            CurrentMypFH.Pattern = Pattern.Text;
                            t_worker = new Thread(new ThreadStart(CurrentMypFH.ScanFileTable));
                            t_worker.Start();
                        }
                        else
                        {
                            OperationFinished();
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
                        OperationFinished();
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

        private void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (CurrentMypFH != null)
                CurrentMypFH.Dispose();
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
        private void MainWindow_DragDrop(object sender, DragEventArgs e)
        {
            object data = null;
            if ((data = e.Data.GetData(DataFormats.FileDrop)) != null)
            {
                if (data.GetType() == typeof(string[]))
                {
                    // Obligation of using a thread here.
                    // Otherwise the events from the file extraction or the file listing
                    // do not work because here we are trying to open multiple files
                    // at the same time.
                    Thread t_DnD = new Thread(new ParameterizedThreadStart(TreatDragDrop));
                    t_DnD.Start(data);
                }
            }
        }

        private void TreatDragDrop(object obj_Filenames)
        {
            string filename;
            string[] filenames = (string[])obj_Filenames;

            for (int i = 0; i < filenames.Length; i++)
            {
                filename = filenames[i];
                if (File.Exists(filename) && filename.Substring(filename.LastIndexOf('.')) == ".myp")
                {
                    if (!MypFHList.Keys.Contains(filename))
                    {
                        OpenArchive(filename);
                        while (operationRunning)
                        {
                            Thread.Sleep(10);
                        }
                    }
                }
            }
        }


        private void MainWindow_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link;
            else
                e.Effect = DragDropEffects.None;

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

            List<FileInArchive> list = (List<FileInArchive>)e.Data.GetData(typeof(List<FileInArchive>));

            if (list.Count > 0)
            {
                if (sender.GetType() == typeof(TreeView))
                {
                    TreeView tv = (TreeView)sender;

                    if (tv.SelectedNode != null)
                    {
                        string path = tv.SelectedNode.FullPath;

                        //Check if an extraction path is set (TODO: fix this small issue :) )
                        EasyMypConfig.ExtractionPath = path;

                        if (MypFHList.Keys.Contains(tv.SelectedNode.Tag.ToString()))
                        {
                            CurrentMypFH = MypFHList[tv.SelectedNode.Tag.ToString()];
                            if (CurrentMypFH != null)
                            {
                                CurrentMypFH.ExtractionPath = path;
                            }

                            ExtractFiles(CurrentMypFH, list);
                        }
                    }
                }
            }
        }

        private void treeView_FileSystem_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(List<FileInArchive>)))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
                e.Effect = DragDropEffects.None;
        }

        #endregion

        #region TreeView Management Events

        //private void treeView_Archive_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        //{
        //    if (e.Button == MouseButtons.Left)
        //    {
        //        ((TreeView)sender).DoDragDrop(((FiaTreeNode)e.Node).fiaList, DragDropEffects.Copy);
        //    }

        //    if (e.Button == MouseButtons.Right)
        //    {
        //        treeView_Archive.SelectedNode = e.Node;
        //    }
        //}

        /// <summary>
        /// Todo: Need to cleanup this by using the correct event :)
        /// I just couldn't get the event to the correct object at first :)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contextMenuStripFileSystemTreeView_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text == "Extract")
            {
                if (treeView_Archive.SelectedNode != null)
                {
                    if (MypFHList.Keys.Contains(treeView_Archive.SelectedNode.Tag.ToString()))
                    {
                        if (MypFHList[treeView_Archive.SelectedNode.Tag.ToString()] != null)
                        {
                            ExtractFiles(MypFHList[treeView_Archive.SelectedNode.Tag.ToString()], ((FiaTreeNode)treeView_Archive.SelectedNode).fiaList);
                        }
                    }
                }
                else
                {
                    //Check if an extraction path is set
                    if (EasyMypConfig.ExtractionPath == null)
                    {
                        if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                        {
                            EasyMypConfig.ExtractionPath = folderBrowserDialog1.SelectedPath;
                            for (int i = 0; i < MypFHList.Count; i++)
                            {
                                MypFHList.Values[i].ExtractionPath = EasyMypConfig.ExtractionPath;
                            }
                        }
                        else
                            return;
                    }
                    Thread t_Extract = new Thread(new ParameterizedThreadStart(ThreadedExtractFiles));
                    t_Extract.Start(ExtractionType.All);
                }
            }
            else if (e.ClickedItem.Text == "Sort")
            {
                // This is quite problematic since we can't just sort a node
                // but we have to sort the whole tree => can take a very long time
                // and freeze the window
                //treeView_Archive.Sort();
                treeView_Archive.BeginUpdate();
                if (treeView_Archive.SelectedNode == null)
                {
                    SortNodes(treeView_Archive.Nodes, false);
                }
                else
                {
                    SortNodes(treeView_Archive.SelectedNode.Nodes, false);
                }
                treeView_Archive.EndUpdate();
            }
            else if (e.ClickedItem.Text == "Recursive Sort")
            {
                // This is quite problematic since we can't just sort a node
                // but we have to sort the whole tree => can take a very long time
                // and freeze the window
                //treeView_Archive.Sort();
                treeView_Archive.BeginUpdate();
                if (treeView_Archive.SelectedNode == null)
                {
                    SortNodes(treeView_Archive.Nodes, true);
                }
                else
                {
                    SortNodes(treeView_Archive.SelectedNode.Nodes, true);
                }
                treeView_Archive.EndUpdate();
            }
        }

        /// <summary>
        /// Sorts the node in a node collection
        /// </summary>
        /// <param name="nodeCollection"></param>
        private void SortNodes(TreeNodeCollection nodeCollection, bool recursive)
        {
            ArrayList list = new ArrayList(nodeCollection.Count);
            foreach (TreeNode node in nodeCollection)
            {
                if (recursive && node.Nodes.Count > 0)
                {
                    SortNodes(node.Nodes, recursive);
                }
                list.Add(node);
            }
            NodeSorter ns = new NodeSorter();
            list.Sort(ns);

            nodeCollection.Clear();
            foreach (TreeNode node in list)
            {
                nodeCollection.Add(node);
            }
        }
        #endregion

        #endregion
    }
}
