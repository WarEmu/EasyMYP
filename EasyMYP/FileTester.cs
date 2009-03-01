using System;
using System.Windows.Forms;

namespace EasyMYP
{
    public partial class FileTester : Form
    {
        public string customDirFile;
        public string customFileFile;
        public string customExtFile;

        public FileTester()
        {
            InitializeComponent();
        }

        private void chooseDirbutton_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "File containing directories to test|*.txt";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                customDirFile = openFileDialog1.FileName;
                customDirLabel.Text = openFileDialog1.FileName;
                customDirBox.Checked = true;
            }
        }

        private void chooseFileButton_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "File containing files to test|*.txt";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                customFileFile = openFileDialog1.FileName;
                customFileLabel.Text = openFileDialog1.FileName;
                customFileBox.Checked = true;
            }
        }

        private void chooseExtButton_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "File containing extensions to test|*.txt";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                customExtFile = openFileDialog1.FileName;
                customExtLabel.Text = openFileDialog1.FileName;
                customExtBox.Checked = true;
            }
        }

        private void goButton_Click(object sender, EventArgs e)
        {
            if (knownFileBox.Checked == false && customFileBox.Checked == false)
            {
                MessageBox.Show("Please choose filenames to test", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (customFileBox.Checked == true && customFileFile == null)
            {
                MessageBox.Show("Please choose custom filenames list", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (customDirBox.Checked == true && customDirFile == null)
            {
                MessageBox.Show("Please choose custom directory list", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (customExtBox.Checked == true && customExtFile == null)
            {
                MessageBox.Show("Please choose custom Extension list", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
