using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using MYPHandler;
using System.Threading;

namespace EasyMYP
{
    public class CustomTreeNode : TreeNode
    {
        public bool clicked = false;

        public CustomTreeNode(string name)
            : base(name)
        {
        }
    }

    public class FiaTreeNode : TreeNode
    {
        public List<FileInArchive> fiaList = new List<FileInArchive>();

        public FiaTreeNode(string name)
            : base(name)
        {
        }

        public void AddFia(FileInArchive fia)
        {
            fiaList.Add(fia);
            if (Parent != null && Parent.GetType() == typeof(FiaTreeNode))
            {
                ((FiaTreeNode)Parent).AddFia(fia);
            }
        }
    }

    public static class TreeViewManager
    {
        public static void PopulateSystemTreeNode(TreeView tv)
        {
            //DirectoryInfo info = new DirectoryInfo();
            string[] drives = Environment.GetLogicalDrives();

            for (int i = 0; i < drives.Length; i++)
            {
                DirectoryInfo info = new DirectoryInfo(drives[i]);
                CustomTreeNode rootNode;
                if (info.Exists && (info.Attributes & FileAttributes.ReadOnly) != FileAttributes.ReadOnly)
                {
                    rootNode = new CustomTreeNode(info.Name);
                    rootNode.Tag = info;
                    rootNode.ImageIndex = 0;
                    rootNode.SelectedImageIndex = 0;
                    tv.Nodes.Add(rootNode);
                }
            }

            //tv.Sort();
        }

        public static void SystemNodeMouseClick(object sender,
            TreeNodeMouseClickEventArgs e)
        {
            CustomTreeNode newSelected = (CustomTreeNode)e.Node;
            if (!newSelected.clicked)
            {
                newSelected.clicked = true;

                if (newSelected.Tag.GetType() == typeof(DirectoryInfo))
                {
                    DirectoryInfo info = (DirectoryInfo)newSelected.Tag;

                    foreach (DirectoryInfo dir in info.GetDirectories())
                    {
                        CustomTreeNode node;
                        node = new CustomTreeNode(dir.Name);
                        node.Tag = dir;
                        node.ImageIndex = 1;
                        node.SelectedImageIndex = 1;
                        e.Node.Nodes.Add(node);
                    }

                    foreach (FileInfo file in info.GetFiles())
                    {
                        CustomTreeNode node;
                        node = new CustomTreeNode(file.Name);
                        node.Tag = file;
                        node.ImageIndex = 4;
                        node.SelectedImageIndex = 4;
                        e.Node.Nodes.Add(node);
                    }
                }
            }
        }

        // not need
        public static void ArchiveNodeMouseClick(object sender,
            TreeNodeMouseClickEventArgs e)
        {
        }

        public static void PopulateArchiveTreeNode(SortableBindingList<FileInArchive> FIAList, TreeView tv)
        {
            tv.Nodes.Clear(); // By changing the FileInArchive format, we would not need to clear this anymore
            // since we could actually store the source filename of the fileinarchive
            // thus allowing extraction even though we opened another file.
            FiaTreeNode root = new FiaTreeNode("root");
            tv.Nodes.Add(root);

            for (int i = 0; i < FIAList.Count; i++)
            {
                FileInArchive fia = FIAList[i];
                string filename = fia.Filename;
                string[] pathes = filename.Split('/');
                FiaTreeNode tn = null;

                for (int j = 0; j < root.Nodes.Count && tn == null; j++)
                {

                    //Check if the level 0 node already exists
                    if (root.Nodes[j].Text == pathes[0])
                    {
                        if (pathes.Length > 1) //Check that this node has childs nodes
                        {
                            //we assign the correct icon
                            //we recursively go down the pathes.
                            tn = ArchiveNodeAdd((FiaTreeNode)root.Nodes[j], pathes, 1, fia);
                        }
                    }
                }

                //If no node where found
                if (tn == null)
                {
                    //We create the level 0 node
                    tn = new FiaTreeNode(pathes[0]);
                    root.Nodes.Add(tn); // add it to the treview
                    if (pathes.Length > 1) //Check that this node has childs nodes
                    {
                        //we assign the correct icon
                        tn.ImageIndex = 1;
                        tn.SelectedImageIndex = 1;
                        //we recursively go down the pathes.
                        ArchiveNodeAdd(tn, pathes, 1, fia);
                    }
                    else
                    {
                        // this means it is actually a file at level 0, we change the icon
                        tn.ImageIndex = 4;
                        tn.SelectedImageIndex = 4;
                        tn.AddFia(fia);
                    }
                }
            }

            //tv.Sort();
            //Thread t = new Thread(new ThreadStart(tv.Sort));
            //t.Start();
        }

        public static FiaTreeNode ArchiveNodeAdd(FiaTreeNode parentNode, string[] pathes, int pathDepth, FileInArchive fia)
        {
            FiaTreeNode childNode = null;

            if (pathes.Length > pathDepth + 1) //means we are not yet at filelevel
            {
                //We search the child nodes for the path part
                for (int j = 0; j < parentNode.Nodes.Count; j++)
                {
                    if (parentNode.Nodes[j].Text == pathes[pathDepth])
                    {
                        //If the node is found we get to the next level
                        childNode = ArchiveNodeAdd((FiaTreeNode)parentNode.Nodes[j], pathes, pathDepth + 1, fia);
                    }
                }
            }
            else
            {
                //We are at file level, we create the node
                childNode = new FiaTreeNode(pathes[pathDepth]);
                parentNode.Nodes.Add(childNode);
                childNode.ImageIndex = 4;
                childNode.SelectedImageIndex = 4;
                childNode.AddFia(fia);
            }

            if (childNode == null)
            {
                //If we did not find a node, we create one
                childNode = new FiaTreeNode(pathes[pathDepth]);
                childNode.ImageIndex = 1;
                childNode.SelectedImageIndex = 1;

                parentNode.Nodes.Add(childNode); //we add the node to the parent node

                //We keep going deeper
                ArchiveNodeAdd(childNode, pathes, pathDepth + 1, fia);
            }

            return childNode;
        }
    }

    // Create a node sorter that implements the IComparer interface.
    public class NodeSorter : IComparer
    {
        public int Compare(object x, object y)
        {
            TreeNode tx = x as TreeNode;
            TreeNode ty = y as TreeNode;

            if (tx.Nodes.Count == 0 && ty.Nodes.Count > 0)
            {
                //If the first node is a file and the second a folder, we switch
                return 1;
            }
            else if (ty.Nodes.Count == 0 && tx.Nodes.Count > 0)
            {
                //If the first node is a folder and the second a file, we do not switch
                return -1;
            }
            //else
            return string.Compare(tx.Text, ty.Text);
        }
    }
}
