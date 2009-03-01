using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace FileListCreator
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;

            if (saveFileDialog1.ShowDialog() != DialogResult.OK)
                return;

            FileStream inputFile = File.Open(openFileDialog1.FileName, FileMode.Open);
            StreamReader sr = new StreamReader(inputFile);

            FileStream outputFile = File.Open(saveFileDialog1.FileName, FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(outputFile);

            HashSet<String> fileRoots = new HashSet<string>();
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                line = ExtractFileName(line);
                fileRoots.Add(line); // this removes duplicates
            }

            sr.Close();

            foreach (string fileRoot in fileRoots)
            {
                sw.WriteLine("art/anims/" + fileRoot + ".xsm");
                sw.WriteLine("art/nifs/effects/" + fileRoot + ".nif");
                sw.WriteLine("art/nifs/effects/" + fileRoot + ".kf");
                sw.WriteLine("art/nifs/effects/" + fileRoot + ".kfm");
                sw.WriteLine("art/nifs/effects/" + fileRoot + ".xml");
                sw.WriteLine("art/nifs/fixtures/" + fileRoot + ".kf");
                sw.WriteLine("art/nifs/fixtures/" + fileRoot + ".kfm");
                sw.WriteLine("art/nifs/scenery/" + fileRoot + ".nif");
                sw.WriteLine("art/pregame/" + fileRoot + ".dds"); //has some kf,kfm and nifs too
                sw.WriteLine("art/skeletons/" + fileRoot + ".xac");
                sw.WriteLine("art/textures/effects/" + fileRoot + ".dds");
                sw.WriteLine("art/textures/scenery/" + fileRoot + ".dds");
                sw.WriteLine("art/textures/scenery/" + fileRoot + "tga");
                sw.WriteLine("art/textures/terrain/" + fileRoot + ".dds");
                sw.WriteLine("art/textures/water/" + fileRoot + ".dds");
                sw.WriteLine("assetdb/textures/fi.0.0." + fileRoot + ".stx");
                sw.WriteLine("assetdb/textures/it.0.0." + fileRoot + ".stx");
                sw.WriteLine("assetdb/textures/sk.0.0." + fileRoot + ".stx");
                sw.WriteLine("assetdb/textures/fg.0.0." + fileRoot + ".stx"); //never seen, just in case
                sw.WriteLine("assetdb/charmesh/fg.0.0." + fileRoot + ".geom");
                sw.WriteLine("assetdb/decals/0.fg.0.0." + fileRoot + ".@.@.mask");
                sw.WriteLine("assetdb/decals/0.fg.0.0." + fileRoot + ".@.fg.0.0." + fileRoot + "_spec.specular");
                sw.WriteLine("assetdb/decals/0.fg.0.0." + fileRoot + ".@.fg.0.0." + fileRoot + "_spec_01.specular");
                sw.WriteLine("assetdb/decals/0.fg.0.0." + fileRoot + ".fg.0.0." + fileRoot + ".@.diffuse");
                sw.WriteLine("assetdb/decals/0.fg.0.0." + fileRoot + ".@.fg.0.0." + fileRoot + "_tint_01.tint");
                //sw.WriteLine("assetdb/decals/0.fg.0.0." + fileRoot + ".@.fg.0.0." + fileRoot + "_tint.tint"); //never seen. But I miss a lot of tints. Useless.
                sw.WriteLine("assetdb/decals/0.fg.0.0." + fileRoot + ".fg.0.0." + fileRoot + "_glow_01.@.glow");
                sw.WriteLine("assetdb/fixtures/fi.0.0." + fileRoot + ".nif");
                sw.WriteLine("assetdb/fixtures/it.0.0." + fileRoot + ".nif");
                sw.WriteLine("assetdb/fixtures/sk.0.0." + fileRoot + ".nif");
            }
            sw.Close();
        }

        private string ExtractFileName(string line)
        {

            try
            {
                line = Path.GetFileNameWithoutExtension(line);
                line = line.Replace("0.fg.0.0.", "");
                int place = line.IndexOf("fg.0.0.");
                switch (place)
                {
                    case -1:
                        break;
                    case 0:
                        line = line.Replace("fg.0.0.", "");
                        break;
                    default:
                        line = line.Remove(place);
                        break;
                }
                line.TrimEnd('.', '@');
                line = line.Replace("fi.0.0", "");
                line = line.Replace("it.0.0", "");
                line = line.Replace("sk.0.0", "");
                if (line.StartsWith("lm.0.")) // do not care about light maps
                    line = "";
                return line;
            }
            catch (ArgumentException e)
            {
                return ""; // invalid character in path
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;

            if (saveFileDialog1.ShowDialog() != DialogResult.OK)
                return;

            FileStream inputFile = File.Open(openFileDialog1.FileName, FileMode.Open);
            StreamReader sr = new StreamReader(inputFile);

            FileStream outputFile = File.Open(saveFileDialog1.FileName, FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(outputFile);

            HashSet<String> fileRoots = new HashSet<string>();
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                line = ExtractFileName(line);
                fileRoots.Add(line); // this removes duplicates
            }

            sr.Close();

            foreach (string fileRoot in fileRoots)
            {
                sw.WriteLine(fileRoot);
            }
            sw.Close();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;

            if (saveFileDialog1.ShowDialog() != DialogResult.OK)
                return;

            FileStream inputFile = File.Open(openFileDialog1.FileName, FileMode.Open);
            StreamReader sr = new StreamReader(inputFile);

            FileStream outputFile = File.Open(saveFileDialog1.FileName, FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(outputFile);

            HashSet<String> I8ned = new HashSet<string>();
            string line;

            string [] languages= {"english","french","german","italian","spanish"};

            while ((line = sr.ReadLine()) != null)
            {
                foreach (string language in languages)
                    if (line.Contains(language))
                    {
                        foreach (string replacement in languages)
                            I8ned.Add(line.Replace(language, replacement));
                    }
            }

            sr.Close();

            foreach (string file in I8ned)
            {
                sw.WriteLine(file);
            }
            sw.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (fileTextBox.Text == null || fileTextBox.Text.Length == 0)
            {
                MessageBox.Show("Please enter a filename without extension", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (saveFileDialog1.ShowDialog() != DialogResult.OK)
                return;

            FileStream outputFile = File.Open(saveFileDialog1.FileName, FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(outputFile);

            char i, j, k;
            for (i = 'a'; i <= 'z'; i++)
            {
                for (j = 'a'; j <= 'z'; j++)
                {
                    for (k = 'a'; k <= 'z'; k++)
                    {
                        sw.WriteLine(fileTextBox.Text+"."+i+j+k);
                    }
                }
            }
            sw.Close();
        }
    }
}
