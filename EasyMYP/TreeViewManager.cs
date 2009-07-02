using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using MYPHandler;

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
            for (int i = 0; i < FIAList.Count; i++)
            {
                FileInArchive fia = FIAList[i];
                string filename = fia.Filename;
                string[] pathes = filename.Split('/');
                TreeNode tn = null;

                for (int j = 0; j < tv.Nodes.Count && tn == null; j++)
                {
                    //Check if the level 0 node already exists
                    if (tv.Nodes[j].Text == pathes[0])
                    {
                        if (pathes.Length > 1) //Check that this node has childs nodes
                        {
                            //we assign the correct icon
                            //we recursively go down the pathes.
                            tn = ArchiveNodeAdd(tv.Nodes[j], pathes, 1);
                        }
                    }
                }

                //If no node where found
                if (tn == null)
                {
                    //We create the level 0 node
                    tn = new TreeNode(pathes[0]);
                    tv.Nodes.Add(tn); // add it to the treview
                    if (pathes.Length > 1) //Check that this node has childs nodes
                    {
                        //we assign the correct icon
                        tn.ImageIndex = 1;
                        tn.SelectedImageIndex = 1;
                        //we recursively go down the pathes.
                        ArchiveNodeAdd(tn, pathes, 1);
                    }
                    else
                    {
                        // this means it is actually a file at level 0, we change the icon
                        tn.ImageIndex = 4;
                        tn.SelectedImageIndex = 4;
                    }
                }
            }
        }

        public static TreeNode ArchiveNodeAdd(TreeNode parentNode, string[] pathes, int pathDepth)
        {
            TreeNode childNode = null;

            if (pathes.Length > pathDepth + 1) //means we are not yet at filelevel
            {
                //We search the child nodes for the path part
                for (int j = 0; j < parentNode.Nodes.Count; j++)
                {
                    if (parentNode.Nodes[j].Text == pathes[pathDepth])
                    {
                        //If the node is found we get to the next level
                        childNode = ArchiveNodeAdd(parentNode.Nodes[j], pathes, pathDepth + 1);
                    }
                }
            }
            else
            {
                //We are at file level, we create the node
                childNode = new TreeNode(pathes[pathDepth]);
                parentNode.Nodes.Add(childNode);
                childNode.ImageIndex = 4;
                childNode.SelectedImageIndex = 4;
            }

            if (childNode == null)
            {
                //If we did not find a node, we create one
                childNode = new TreeNode(pathes[pathDepth]);
                childNode.ImageIndex = 1;
                childNode.SelectedImageIndex = 1;

                parentNode.Nodes.Add(childNode); //we add the node to the parent node

                //We keep going deeper
                childNode = ArchiveNodeAdd(childNode, pathes, pathDepth + 1);
            }

            return childNode;
        }
    }
}
