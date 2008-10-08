using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using MYPWorker;
using WarhammerOnlineHashBuilder;

namespace MYPHashFileListing
{
    public class mypfile
    {
        public uint ph, sh;
        public string filename;
    }

    class Program
    {
        static List<string> dirListing = new List<string>();

        static void Main(string[] args)
        {
            //CleanFile();
            //FindPattern();
            //return;
            string path = "";
            if (args.Length == 1)
            {
                path = args[0];
            }

            HashCreator worker = new HashCreator();
            //worker.F1(path); //builds the hashlist

            worker.InitializeHashList(); //add the ones already calculated and known

            //worker.IntegrateOldHashList();
            //worker.Save();

            //worker.ParseFilenames();
            //worker.ParseDirAndFilenames();
            worker.ParseDirFilenamesAndExtension();
            
            worker.ConvertToPattern();
            worker.Patterns();
            //worker.ConvertToPattern();
            
            worker.Save();

            //worker.InitializeHashList();
            //worker.ParseDirFilenamesAndExtension();
            //worker.ConvertToPattern();

            //worker.Patterns();
            //worker.Save();

            //CleanFile();
            //CleanTBuild();
            //CreateDir();
            //CleanDirListing();
        }

        static void CreateDir()
        {
            FileStream stream = new FileStream("Hash/dirList.txt", FileMode.Open);
            StreamReader sr = new StreamReader(stream);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                line = "F:/Jeux/War/" + line;
                if (!Directory.Exists(line))
                {
                    Directory.CreateDirectory(line);
                }
            }
        }

        static void CleanTBuild()
        {
            FileStream fs = new FileStream("tbuild.txt", FileMode.Open);
            FileStream fs2 = new FileStream("tbuild2.txt", FileMode.OpenOrCreate);
            StreamReader reader = new StreamReader(fs);
            StreamWriter writer = new StreamWriter(fs2);
            string line = "";

            while ((line = reader.ReadLine()) != null)
            {
                writer.WriteLine(line.Split('|')[0]);
            }

            reader.Close();
            writer.Close();
            fs.Close();
            fs2.Close();

        }

        static void CleanDirListing()
        {
            FileStream fs = new FileStream("dir_filenames.txt", FileMode.Open);
            StreamReader reader = new StreamReader(fs);
            string line = "";
            string cor_line = "";

            List<string> strList = new List<string>();

            while ((line = reader.ReadLine()) != null)
            {
                if (line.Length > 36)
                {
                    //cor_line = line.Substring(29);
                    cor_line = line.Substring(36);
                    cor_line = cor_line.Split('.')[0] + "_disabled." + cor_line.Split('.')[1];

                    strList.Add(cor_line);
                }
            }

            reader.Close();
            fs.Close();
            if (File.Exists("temp2.txt")) File.Delete("temp2.txt");
            fs = new FileStream("temp2.txt", FileMode.OpenOrCreate);
            StreamWriter writer = new StreamWriter(fs);

            for (int i = 0; i < strList.Count; i++)
            {
                writer.WriteLine(strList[i]);
            }

            writer.Close();
            fs.Close();

        }

        static void CleanFile()
        {
            Regex r = new Regex(@"[a-zA-Z0-9]");
            FileStream fs = new FileStream("temp.txt", FileMode.Open);
            StreamReader reader = new StreamReader(fs);
            string line = "";
            string cor_line = "";

            List<string> strList = new List<string>();

            while ((line = reader.ReadLine()) != null)
            {
                if (line.Length > 27)
                {
                    cor_line = line.Substring(27);
                    //cor_line = line;

                    if (cor_line.IndexOf(' ') < 0
                        && cor_line.IndexOf("\\") < 0
                        && cor_line.IndexOf('[') < 0
                        && cor_line.IndexOf('(') < 0
                        && cor_line.IndexOf(')') < 0
                        && cor_line.IndexOf(']') < 0
                        && cor_line.IndexOf('$') < 0
                        && cor_line.IndexOf('~') < 0
                        && cor_line.IndexOf('<') < 0
                        && cor_line.IndexOf('>') < 0
                        && cor_line.IndexOf('=') < 0
                        && cor_line.IndexOf('?') < 0
                        && cor_line.IndexOf('&') < 0
                        && cor_line.IndexOf('*') < 0
                        && cor_line.IndexOf('+') < 0
                        && cor_line.IndexOf('|') < 0
                        && cor_line.IndexOf(':') < 0
                        && cor_line.IndexOf('^') < 0
                        && cor_line.IndexOf('\'') < 0
                        && cor_line.IndexOf('`') < 0
                        && cor_line.IndexOf('#') < 0
                        && cor_line.IndexOf('{') < 0
                        && cor_line.IndexOf('}') < 0
                        && cor_line.IndexOf('!') < 0
                        && cor_line.IndexOf(';') < 0
                        && cor_line.IndexOf(',') < 0
                        && cor_line.IndexOf('@') < 0
                        )
                    {

                        if (r.IsMatch(cor_line[0].ToString()) || cor_line[0] == '%')
                        {
                            strList.Add(cor_line);
                        }
                    }
                }
            }

            reader.Close();
            fs.Close();
            if (File.Exists("temp2.txt")) File.Delete("temp2.txt");
            fs = new FileStream("temp2.txt", FileMode.OpenOrCreate);
            StreamWriter writer = new StreamWriter(fs);

            for (int i = 0; i < strList.Count; i++)
            {
                writer.WriteLine(strList[i]);
            }

            writer.Close();
            fs.Close();

        }

        static void FindPattern()
        {
            string directoryListingFile = "Hash/dirlist.txt";
            FileStream fs;
            StreamReader reader;
            StreamWriter writer;
            string line;

            if (File.Exists(directoryListingFile))
            {
                fs = new FileStream(directoryListingFile, FileMode.Open);
                reader = new StreamReader(fs);

                while ((line = reader.ReadLine()) != null)
                {
                    AddDirectory(line);
                }

                reader.Close();
                fs.Close();
            }


            string fn = "CleanStringListIDA.txt";
            List<string> strPatList = new List<string>();
            List<string> numPatList = new List<string>();
            fs = new FileStream(fn, FileMode.Open);
            reader = new StreamReader(fs);

            while ((line = reader.ReadLine()) != null)
            {
                if (line.IndexOf("%s") >= 0 && !strPatList.Contains(line))
                {
                    strPatList.Add(line);
                } else if ((line.IndexOf("%04d") >= 0 || line.IndexOf("%03d") >= 0 || line.IndexOf("%02d") >= 0 || line.IndexOf("%d") >= 0) && !numPatList.Contains(line))
                {
                    line = line.Replace("%d", "[0-9]");
                    line = line.Replace("%02d", "[0-9][0-9]");
                    line = line.Replace("%03d", "[0-9][0-9][0-9]");
                    line = line.Replace("%04d", "[0-9][0-9][0-9][0-9]");

                    numPatList.Add(line);
                }
            }

            reader.Close();
            fs.Close();

            fs = new FileStream("strPatList.txt", FileMode.OpenOrCreate);
            writer = new StreamWriter(fs);

            for (int i = 0; i < strPatList.Count; i++)
            {
                writer.WriteLine(strPatList[i]);
            }

            writer.Close();
            fs.Close();

            fs = new FileStream("numPatList.txt", FileMode.OpenOrCreate);
            writer = new StreamWriter(fs);

            for (int i = 0; i < numPatList.Count; i++)
            {
                for (int j = 0; j < dirListing.Count; j++)
                {
                    writer.WriteLine(dirListing[j]+"/"+numPatList[i]);
                }
            }

            writer.Close();
            fs.Close();
        }

        static void AddDirectory(string filename)
        {
            if (filename.Replace('\\', '/').IndexOf('/') >= 0 && filename.LastIndexOf('.') >= 0)
            {
                string dir = filename.Replace('\\', '/').Substring(0, filename.Replace('\\', '/').LastIndexOf('/'));

                if (dir.IndexOf(' ') < 0 && !dirListing.Contains(dir))
                {
                    dirListing.Add(dir);
                }
            }
            else
            {
                string dir = filename.Replace('\\', '/');
                if (dir.IndexOf(' ') < 0 && !dirListing.Contains(dir))
                {
                    dirListing.Add(dir);
                }
            }
        }
    }
}
