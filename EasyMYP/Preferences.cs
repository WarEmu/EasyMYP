using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EasyMYP
{
    public partial class Preferences : Form
    {
        public Preferences()
        {
            InitializeComponent();

            //EasyMYP Configuration
            textBox_ExtractionPath.Text = EasyMypConfig.ExtractionPath;

            //MYPHandler Configuration
            checkBox_MultiThreadedExtraction.Checked = MYPHandler.MypHandlerConfig.MultithreadedExtraction;

            //Hash Creator Configuration
            for (int i = 0; i < System.Environment.ProcessorCount || i < 2; i++)
            {
                dropdownlist_MaxOperationThread.Items.Add(i + 1);
            }
            dropdownlist_MaxOperationThread.SelectedItem = nsHashCreator.HashCreatorConfig.MaxOperationThread;
            dropdownlist_MaxCombinationPerPattern.SelectedItem = nsHashCreator.HashCreatorConfig.MaxCombinationPerPattern;
        }

        private void button_Save_Click(object sender, EventArgs e)
        {
            //EasyMYP Configuration
            EasyMypConfig.ExtractionPath = textBox_ExtractionPath.Text;

            //MYPHandler Configuration
            MYPHandler.MypHandlerConfig.MultithreadedExtraction = checkBox_MultiThreadedExtraction.Checked;

            //Hash Creator Configuration
            if (dropdownlist_MaxOperationThread.SelectedItem != null)
            {
                nsHashCreator.HashCreatorConfig.MaxOperationThread = (int)dropdownlist_MaxOperationThread.SelectedItem;
            }
            if (dropdownlist_MaxCombinationPerPattern.SelectedItem != null)
            {
                nsHashCreator.HashCreatorConfig.MaxCombinationPerPattern = (int)dropdownlist_MaxCombinationPerPattern.SelectedItem;
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void button_SetExtractionPath_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox_ExtractionPath.Text = folderBrowserDialog1.SelectedPath;
            }
        }
    }
}
