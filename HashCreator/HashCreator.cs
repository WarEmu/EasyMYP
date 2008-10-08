#region Description
/********************************************************************
 * This file creates filenames based on patterns
 * 
 * This file also tries to create filenames based on the figleaf
 * filename database
 * 
 *******************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using MYPWorker;
using WarhammerOnlineHash;
using WarhammerOnlineHashBuilder;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.Threading;

namespace nsHashCreator
{
    public class mypfile
    {
        public uint ph, sh;
        public string filename;
    }

    public class HashCreator
    {
        List<mypfile> mlist = new List<mypfile>();
        //List<string> bruteList = new List<string>();
        //string bruteFile = "brute.txt";

        /// <summary>
        /// Converts all the filenames that contained numbers to filenames with a pattern for futur use
        /// (based on the known files (that is the truly first version of the myp which had the names))
        /// </summary>
        public void ConvertToPattern()
        {
            List<string> patternList = new List<string>();
            string line = "";
            if (File.Exists("pattern_n.txt"))
            {
                FileStream fs = new FileStream("pattern_n.txt", FileMode.Open);
                StreamReader rfs = new StreamReader(fs);

                while ((line = rfs.ReadLine()) != null)
                {
                    if (!patternList.Contains(line))
                    {
                        patternList.Add(line);
                    }
                }

                rfs.Close();
                fs.Close();
            }

            FileStream stream = new FileStream("pattern_num.txt", FileMode.Open);
            StreamReader reader = new StreamReader(stream);

            Regex r = new Regex("[0-9]");

            while ((line = reader.ReadLine()) != null)
            {
                if (r.IsMatch(line))
                {
                    if (!patternList.Contains(r.Replace(line, "[0-9]")))
                    {
                        patternList.Add(r.Replace(line, "[0-9]"));
                    }
                }
            }
            stream.Close();
            if (File.Exists("pattern_n.txt")) File.Delete("pattern_n.txt");
            stream = new FileStream("pattern_n.txt", FileMode.OpenOrCreate);
            StreamWriter writer = new StreamWriter(stream);

            for (int i = 0; i < patternList.Count; i++)
            {
                writer.WriteLine(patternList[i]);
            }

            writer.Close();
            stream.Close();
        }

        Hasher hasher;

        /// <summary>
        /// Treats the pattern and generates the filenames out of it
        /// </summary>
        public void Patterns()
        {
            #region extensions
            string extFile = "extList.txt";
            string line;
            FileStream fs = new FileStream(extFile, FileMode.Open);
            StreamReader r2 = new StreamReader(fs);

            while ((line = r2.ReadLine()) != null)
            {
                if (!patExtList.Contains(line))
                {
                    patExtList.Add(line);
                }
            }
            r2.Close();
            fs.Close();
            #endregion

            FileStream stream = new FileStream("pattern_n.txt", FileMode.Open);
            StreamReader reader = new StreamReader(stream);
            WarHasher warhash = new WarHasher();


            int a_counter = 0;
            while ((line = reader.ReadLine()) != null)
            {
                if (!patternList.Contains(line))
                {
                    patternList.Add(line);
                }
            }

            reader.Close();
            stream.Close();

            patPlace = 0;

            Thread pat1 = new Thread(new ThreadStart(TreatPattern));
            Thread pat2 = new Thread(new ThreadStart(TreatPattern));

            pat1.Start();
            pat2.Start();

            while (pat1.ThreadState == ThreadState.Running || pat2.ThreadState == ThreadState.Running)
            {
                Thread.Sleep(1);
            }
        }

        List<string> patExtList = new List<string>();
        List<string> patternList = new List<string>();
        int patPlace;
        object lock_obj = new object();

        string getPattern()
        {
            lock (lock_obj)
            {
                if (patPlace % 250 == 0) Save();

                Console.WriteLine("{0}", patPlace * 100 / patternList.Count);
                if (patPlace < patternList.Count)
                {
                    patPlace++;
                    return patternList[patPlace - 1];
                }
                else
                {
                    return null;
                }
            }
        }

        void TreatPattern()
        {
            string line;
            WarHasher warhasher = new WarHasher();
            while ((line = getPattern()) != null)
            {
                TreatPatternLine(line, warhasher);
            }
        }

        void TreatPatternLine(string line, WarHasher warhash)
        {
            int occ = line.Replace("[0-9]", "|").Split('|').Length - 1;
            long max = (long)Math.Pow(10, occ);

            string[] spl_str = line.Replace("[0-9]", "|").Split('|');
            string format = "";
            for (int i = 0; i < occ; i++)
            {
                format += "0";
            }
            if (occ <= 12)
            {
                if (line.IndexOf(".") >= 0)
                {
                    for (long i = 0; i < max; i++)
                    {
                        string cur_i = i.ToString(format);

                        string cur_str = "";


                        for (int j = 0; j < occ; j++)
                        {
                            cur_str += spl_str[j];
                            cur_str += cur_i[j];
                        }
                        cur_str += spl_str[occ];

                        warhash.Hash(cur_str, 0xDEADBEEF);
                        hasher.UpdateHash(warhash.ph, warhash.sh, cur_str, 0);

                        //string brute_str = "";
                        //brute_str = string.Format("{0:X8}#{1:X8}#{2}", (uint)(warhash.ph), (uint)(warhash.sh), cur_str);
                        //AddBruteLine(brute_str);
                    }
                }
                else
                {

                    for (int k = 0; k < patExtList.Count; k++)
                    {
                        for (int i = 0; i < max; i++)
                        {
                            string cur_i = i.ToString(format);

                            string cur_str = "";


                            for (int j = 0; j < occ; j++)
                            {
                                cur_str += spl_str[j];
                                cur_str += cur_i[j];
                            }
                            cur_str += spl_str[occ];
                            cur_str += "." + patExtList[k];
                            warhash.Hash(cur_str, 0xDEADBEEF);
                            hasher.UpdateHash(warhash.ph, warhash.sh, cur_str, 0);
                        }
                    }
                }
            }
        }

        //void AddBruteLine(string line)
        //{
        //    if (!bruteList.Contains(line))
        //    {
        //        bruteList.Add(line);
        //    }
        //}

        public void InitializeHashList()
        {
            hasher = new Hasher(true);
            hasher.BuildHashList();
        }

        public void IntegrateOldHashList()
        {
            string file = "Hash/OldHashList.txt";
            hasher.MergeHashList(file);
        }


        /// <summary>
        /// Parses all the myp files to extract all the possible hashes
        /// </summary>
        public void F1(string path)
        {
            hasher = new Hasher(true);

            MYPWorker.del_FileTableEventHandler OnNewFileEntry = NewFile;
            if (path == "")
            {
                path = @"F:\Jeux\WAR";
            }

            string[] mypfiles = Directory.GetFiles(path, "*.myp");

            for (int i = 0; i < mypfiles.Length; i++)
            {
                Console.WriteLine(mypfiles[i]);
                MYPWorker.MYPWorker worker = new MYPWorker.MYPWorker(mypfiles[i], OnNewFileEntry, null, hasher);
                worker.GetFileTable();
                Save();
            }
        }

        /// <summary>
        /// Saves all the information possible to text files that can be used by the Hasher afterwards
        /// </summary>
        public void Save()
        {
            hasher.SaveHashList();

            //if (File.Exists(bruteFile)) File.Delete(bruteFile);
            //FileStream output_hashes = new FileStream(bruteFile, FileMode.Create);
            //StreamWriter writer_oh = new StreamWriter(output_hashes);
            //for (int i = 0; i < bruteList.Count; i++)
            //{
            //    writer_oh.WriteLine(bruteList[i]);
            //}
            //writer_oh.Close();
            //output_hashes.Close();

        }

        public void NewFile(object sender, MYPWorker.MYPFileTableEventArgs e)
        {
            FileInArchive file = e.ArchFile;
            string fn = "";
            mypfile newfile = new mypfile();

            //if (file.descriptor.foundFileName) fn = file.descriptor.filename;
            newfile.ph = file.descriptor.ph;
            newfile.sh = file.descriptor.sh;
            newfile.filename = fn;
            //mlist.Add(newfile);

            hasher.AddHash(newfile.ph, newfile.sh);
            hasher.UpdateHash(newfile.ph, newfile.sh, newfile.filename, 0);
        }

        public void ParseFilenames()
        {
            if (File.Exists("full_filenames.txt"))
            {
                WarHasher warhash = new WarHasher();

                FileStream fs = new FileStream("full_filenames.txt", FileMode.Open);
                StreamReader reader = new StreamReader(fs);

                List<string> filenameList = new List<string>();

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (!filenameList.Contains(line))
                    {
                        filenameList.Add(line);
                    }
                }

                reader.Close();
                fs.Close();

                File.Delete("full_filenames.txt");
                fs = new FileStream("full_filenames.txt", FileMode.Create);
                StreamWriter writer = new StreamWriter(fs);

                for (int i = 0; i < filenameList.Count; i++)
                {
                    writer.WriteLine(filenameList[i]);
                }

                writer.Close();
                fs.Close();

                for (int i = 0; i < filenameList.Count; i++)
                {
                    Console.WriteLine((float)i / (float)filenameList.Count * (float)100);
                    //string brute_str = "";

                    string cur_str = filenameList[i];
                    //warhash.Hash(cur_str, 0xDEADBEEF);
                    //hasher.UpdateHash(warhash.ph, warhash.sh, cur_str);

                    //brute_str = string.Format("{0:X8}#{1:X8}#{2}", (uint)(warhash.ph), (uint)(warhash.sh), cur_str);
                    //AddBruteLine(brute_str);

                    cur_str = cur_str.ToLower().Replace('\\', '/');
                    warhash.Hash(cur_str, 0xDEADBEEF);
                    hasher.UpdateHash(warhash.ph, warhash.sh, cur_str, 0);

                    //brute_str = string.Format("{0:X8}#{1:X8}#{2}", (uint)(warhash.ph), (uint)(warhash.sh), cur_str);
                    //AddBruteLine(brute_str);

                    //cur_str = filenameList[i].Replace('\\', '/');
                    //warhash.Hash(cur_str, 0xDEADBEEF);
                    //hasher.UpdateHash(warhash.ph, warhash.sh, cur_str);

                    //brute_str = string.Format("{0:X8}#{1:X8}#{2}", (uint)(warhash.ph), (uint)(warhash.sh), cur_str);
                    //AddBruteLine(brute_str);

                    //cur_str = cur_str.ToLower();
                    //warhash.Hash(cur_str, 0xdeadbeef);
                    //hasher.UpdateHash(warhash.ph, warhash.sh, cur_str);

                    //brute_str = string.Format("{0:X8}#{1:X8}#{2}", (uint)(warhash.ph), (uint)(warhash.sh), cur_str);
                    //AddBruteLine(brute_str);

                    //cur_str = filenameList[i].Replace('/', '\\');
                    //warhash.Hash(cur_str, 0xDEADBEEF);
                    //hasher.UpdateHash(warhash.ph, warhash.sh, cur_str);

                    //brute_str = string.Format("{0:X8}#{1:X8}#{2}", (uint)(warhash.ph), (uint)(warhash.sh), cur_str);
                    //AddBruteLine(brute_str);

                    //cur_str = cur_str.ToLower();
                    //warhash.Hash(cur_str, 0xDEADBEEF);
                    //hasher.UpdateHash(warhash.ph, warhash.sh, cur_str);

                    //brute_str = string.Format("{0:X8}#{1:X8}#{2}", (uint)(warhash.ph), (uint)(warhash.sh), cur_str);
                    //AddBruteLine(brute_str);

                }
            }
        }

        public void ParseDirAndFilenames()
        {
            if (File.Exists("filenames.txt"))
            {
                WarHasher warhash = new WarHasher();

                FileStream fs = new FileStream("filenames.txt", FileMode.Open);
                StreamReader reader = new StreamReader(fs);

                List<string> filenameList = new List<string>();

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (!filenameList.Contains(line))
                    {
                        filenameList.Add(line);
                    }
                }

                reader.Close();
                fs.Close();

                File.Delete("filenames.txt");
                fs = new FileStream("filenames.txt", FileMode.Create);
                StreamWriter writer = new StreamWriter(fs);

                for (int i = 0; i < filenameList.Count; i++)
                {
                    writer.WriteLine(filenameList[i]);
                }

                writer.Close();
                fs.Close();

                for (int i = 0; i < filenameList.Count; i++)
                {
                    for (int j = 0; j < hasher.DirListing.Count; j++)
                    {
                        Console.WriteLine("{0} {1}", j, (float)i / (float)filenameList.Count * (float)100);
                        //string brute_str = "";
                        line = hasher.DirListing[j] + '/' + filenameList[i];
                        line = line.ToLower();
                        line = line.Replace('\\', '/');

                        if (line.IndexOf("[0-9]") >= 0)
                        {
                            int occ = line.Replace("[0-9]", "|").Split('|').Length - 1;
                            int max = (int)Math.Pow(10, occ);

                            string[] spl_str = line.Replace("[0-9]", "|").Split('|');
                            string format = "";
                            for (int l = 0; l < occ; l++)
                            {
                                format += "0";
                            }
                            if (max < 10000000)
                            {
                                if (line.IndexOf(".") >= 0)
                                {
                                    for (int l = 0; l < max; l++)
                                    {
                                        string cur_i = l.ToString(format);

                                        string cur_str = "";


                                        for (int m = 0; m < occ; m++)
                                        {
                                            cur_str += spl_str[m];
                                            cur_str += cur_i[m];
                                        }
                                        cur_str += spl_str[occ];

                                        warhash.Hash(cur_str, 0xDEADBEEF);
                                        hasher.UpdateHash(warhash.ph, warhash.sh, cur_str, 0);

                                        //string brute_str = "";
                                        //brute_str = string.Format("{0:X8}#{1:X8}#{2}", (uint)(warhash.ph), (uint)(warhash.sh), cur_str);
                                        //AddBruteLine(brute_str);
                                    }
                                }
                            }
                        }
                        else
                        {

                            string cur_str = hasher.DirListing[j] + '/' + filenameList[i];
                            //warhash.Hash(cur_str, 0xDEADBEEF);
                            //hasher.UpdateHash(warhash.ph, warhash.sh, cur_str);

                            //brute_str = string.Format("{0:X8}#{1:X8}#{2}", (uint)(warhash.ph), (uint)(warhash.sh), cur_str);
                            //AddBruteLine(brute_str);

                            cur_str = cur_str.ToLower().Replace('\\', '/');
                            warhash.Hash(cur_str, 0xDEADBEEF);
                            hasher.UpdateHash(warhash.ph, warhash.sh, cur_str, 0);

                            //brute_str = string.Format("{0:X8}#{1:X8}#{2}", (uint)(warhash.ph), (uint)(warhash.sh), cur_str);
                            //AddBruteLine(brute_str);

                            //cur_str = hasher.DirListing[j] + '\\' + filenameList[i];
                            //cur_str = cur_str.Replace('\\', '/');
                            //warhash.Hash(cur_str, 0xDEADBEEF);
                            //hasher.UpdateHash(warhash.ph, warhash.sh, cur_str);

                            //brute_str = string.Format("{0:X8}#{1:X8}#{2}", (uint)(warhash.ph), (uint)(warhash.sh), cur_str);
                            //AddBruteLine(brute_str);

                            //cur_str = cur_str.ToLower();
                            //warhash.Hash(cur_str, 0xDEADBEEF);
                            //hasher.UpdateHash(warhash.ph, warhash.sh, cur_str);

                            //brute_str = string.Format("{0:X8}#{1:X8}#{2}", (uint)(warhash.ph), (uint)(warhash.sh), cur_str);
                            //AddBruteLine(brute_str);

                            //cur_str = hasher.DirListing[j] + '\\' + filenameList[i];
                            //cur_str = cur_str.Replace('/', '\\');
                            //warhash.Hash(cur_str, 0xDEADBEEF);
                            //hasher.UpdateHash(warhash.ph, warhash.sh, cur_str);

                            //brute_str = string.Format("{0:X8}#{1:X8}#{2}", (uint)(warhash.ph), (uint)(warhash.sh), cur_str);
                            //AddBruteLine(brute_str);

                            //cur_str = cur_str.ToLower();
                            //warhash.Hash(cur_str, 0xDEADBEEF);
                            //hasher.UpdateHash(warhash.ph, warhash.sh, cur_str);

                            //brute_str = string.Format("{0:X8}#{1:X8}#{2}", (uint)(warhash.ph), (uint)(warhash.sh), cur_str);
                            //AddBruteLine(brute_str);
                        }
                    }
                }
            }
        }

        public void ParseDirFilenamesAndExtension()
        {
            string DFEFile = "fileList.txt";
            string extFile = "extList.txt";
            if (File.Exists(DFEFile) && File.Exists(extFile))
            {
                #region dfenames
                FileStream fs = new FileStream(DFEFile, FileMode.Open);
                StreamReader reader = new StreamReader(fs);
                int outint;
                List<string> filenameList = new List<string>();

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (!int.TryParse(line, out outint))
                    {
                        if (!filenameList.Contains(line))
                        {
                            filenameList.Add(line);
                        }
                    }
                }

                reader.Close();
                fs.Close();
                #endregion

                #region extensions
                fs = new FileStream(extFile, FileMode.Open);
                reader = new StreamReader(fs);

                List<string> extensionList = new List<string>();

                while ((line = reader.ReadLine()) != null)
                {
                    if (!extensionList.Contains(line))
                    {
                        extensionList.Add(line);
                    }
                }
                reader.Close();
                fs.Close();
                #endregion

                #region Save Unique
                File.Delete(DFEFile);
                fs = new FileStream(DFEFile, FileMode.Create);
                StreamWriter writer = new StreamWriter(fs);

                for (int i = 0; i < filenameList.Count; i++)
                {
                    //if (!int.TryParse(filenameList[i], out outint))
                    //{
                    writer.WriteLine(filenameList[i]);
                    //}
                }

                writer.Close();
                fs.Close();
                #endregion

                Thread t1 = new Thread(new ParameterizedThreadStart(calc));
                Thread t2 = new Thread(new ParameterizedThreadStart(calc));

                t1.Start(new obj(filenameList, extensionList, 0, hasher.DirListing.Count / 2));
                t2.Start(new obj(filenameList, extensionList, hasher.DirListing.Count / 2, hasher.DirListing.Count));

                while (t1.IsAlive || t2.IsAlive)
                {
                    Thread.Sleep(60000);
                }
            }
        }

        struct obj
        {
            public List<string> filenameList;
            public List<string> extensionList;
            public int jstart;
            public int jend;

            public obj(List<string> filenameList, List<string> extensionList, int jstart, int jend)
            {
                this.filenameList = filenameList;
                this.extensionList = extensionList;
                this.jstart = jstart;
                this.jend = jend;
            }
        }

        private void calc(object o)
        {
            WarHasher warhash = new WarHasher();
            List<string> filenameList = ((obj)o).filenameList;
            List<string> extensionList = ((obj)o).extensionList;
            int jstart = ((obj)o).jstart;
            int jend = ((obj)o).jend;

            for (int j = jstart; j < jend; j++)
            {
                if (j % 25 == 0 && j != 0 && j < hasher.DirListing.Count / 2)
                {
                    Save();
                }
                Console.WriteLine("j: " + j + "    " + (float)(j - jstart) / (float)(jend - jstart) * (float)100);
                for (int i = 0; i < filenameList.Count; i++)
                {
                    for (int k = 0; k < extensionList.Count; k++)
                    {
                        string cur_str = hasher.DirListing[j] + '/' + filenameList[i] + "." + extensionList[k];
                        //warhash.Hash(cur_str, 0xDEADBEEF);
                        //hasher.UpdateHash(warhash.ph, warhash.sh, cur_str);

                        cur_str = cur_str.Replace('\\', '/').ToLower();
                        warhash.Hash(cur_str, 0xDEADBEEF);
                        hasher.UpdateHash(warhash.ph, warhash.sh, cur_str, 0);

                        //cur_str = cur_str.ToLower();
                        //warhash.Hash(cur_str, 0xDEADBEEF);
                        //hasher.UpdateHash(warhash.ph, warhash.sh, cur_str);

                        //cur_str = cur_str.ToLower();
                        //warhash.Hash(cur_str, 0xDEADBEEF);
                        //hasher.UpdateHash(warhash.ph, warhash.sh, cur_str);

                        //cur_str = cur_str.Replace('/', '\\');
                        //warhash.Hash(cur_str, 0xDEADBEEF);
                        //hasher.UpdateHash(warhash.ph, warhash.sh, cur_str);

                        //cur_str = cur_str.ToLower();
                        //warhash.Hash(cur_str, 0xDEADBEEF);
                        //hasher.UpdateHash(warhash.ph, warhash.sh, cur_str);
                    }
                }
            }
        }
    }


    public class PrimaryHashEntry
    {
        public uint ph;
        public List<SecondaryHashEntry> shList = new List<SecondaryHashEntry>();

        public PrimaryHashEntry(uint ph, SecondaryHashEntry se)
        {
            this.ph = ph;
            AddSE(se);
        }

        public void AddSE(SecondaryHashEntry se)
        {
            bool found = false;
            for (int i = 0; i < shList.Count; i++)
            {
                if (shList[i].filename == se.filename && shList[i].sh == se.sh)
                {
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                shList.Add(se);
            }
        }

        public PrimaryHashEntry() { }
    }

    public class SecondaryHashEntry
    {
        public uint sh;
        public string filename;

        public SecondaryHashEntry(uint sh, string filename)
        {
            this.sh = sh;
            this.filename = filename;
        }

        public SecondaryHashEntry() { }
    }
}
