using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.IO;

namespace EasyMYP
{
    partial class About : Form
    {
        public About()
        {
            InitializeComponent();

            if (File.Exists("README.txt"))
            {
                this.richTextBox1.LoadFile("README.txt", RichTextBoxStreamType.PlainText);
            }
        }

        private void tableLayoutPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
