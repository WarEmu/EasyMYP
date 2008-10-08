#region Description
/******************************************************************************
 * This file only creates a list of hashes which can then be searched based on 
 * a text file which should be placed in Hash/hashes_filename.txt
 * 
 * 
 * 
 * Chryso
 *****************************************************************************/
#endregion

using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using WarhammerHash;
using WarhammerOnlineHashBuilder;

namespace MYPHasherBuilder
{
    public class Hasher
    {
        SortedList<long, string> hashList = new SortedList<long, string>();
        List<string> dirListing = new List<string>();
        List<string> fileListing = new List<string>();
        List<string> extListing = new List<string>();

        public SortedList<long, string> HashList { get { return hashList; } }
        public List<string> DirListing { get { return dirListing; } }

        void AddHash(uint ph, uint sh, string name)
        {
            long sig = (((long)ph) << 32) + sh;
            if (!hashList.ContainsKey(sig))
            {
                hashList.Add(sig, name);

                if (name != "" && !dirListing.Contains(name))
                {
                    AddDirectory(name);
                    AddFile(name);
                }
            }
            else
            {
                if (hashList[sig] != name && name != "")
                {
                    hashList[sig] = name;
                }
            }
        }

        public void AddHash(uint ph, uint sh)
        {
            long sig = (((long)ph) << 32) + sh;
            //if the list contains the sig, then we update
            if (!hashList.ContainsKey(sig))
            {
                hashList.Add(sig, "");
            }
        }

        public void UpdateHash(uint ph, uint sh, string name)
        {
            long sig = (((long)ph) << 32) + sh;
            //if the list contains the sig, then we update
            if (hashList.ContainsKey(sig))
            {
                if (hashList[sig] != name && name != "")
                {
                    hashList[sig] = name;
                }

                AddDirectory(name);
                AddFile(name);
            }
        }

        void AddDirectory(string filename)
        {
            if (filename.Replace('\\', '/').IndexOf('/') >= 0 && filename.LastIndexOf('.') >= 0)
            {
                string dir = filename.Replace('\\', '/').Substring(0, filename.Replace('\\', '/').LastIndexOf('/'));

                if (!dirListing.Contains(dir))
                {
                    dirListing.Add(dir);
                }
            }
            else
            {
                string dir = filename.Replace('\\', '/');
                if (!dirListing.Contains(dir))
                {
                    dirListing.Add(dir);
                }
            }
        }

        void AddFile(string filename)
        {
            AddExtension(filename);

            string cur_fn = filename.Replace('\\', '/');

            if (cur_fn.IndexOf('/') >= 0)
            {
                cur_fn = cur_fn.Split('/')[cur_fn.Split('/').Length - 1];
                cur_fn = cur_fn.Split('.')[0];
            }
            if (!fileListing.Contains(cur_fn))
            {
                fileListing.Add(cur_fn);
            }
        }

        void AddExtension(string filename)
        {
            string ext = filename.Split('.')[filename.Split('.').Length - 1];
            if (!extListing.Contains(ext))
            {
                extListing.Add(ext);
            }
        }

        /// <summary>
        /// Creates a sorted list (hashlist) based on the hashes_filename.txt file
        /// </summary>
        public void BuildHashList()
        {
            LoadDirListing();

            if (File.Exists("Hash/hashes_filename.txt"))
            {
                FileStream fs = new FileStream("Hash/hashes_filename.txt", FileMode.Open);
                StreamReader reader = new StreamReader(fs);

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] strsplt = line.Split('#');
                    uint ph = (uint)Convert.ToUInt32(strsplt[0], 16);
                    uint sh = (uint)Convert.ToUInt32(strsplt[1], 16);
                    string filename = strsplt[2];

                    AddHash(ph, sh, filename);
                }

                reader.Close();
                fs.Close();
            }
        }

        void LoadDirListing()
        {
            AddDirectory("");

            if (File.Exists("Hash/dirlist.txt"))
            {
                FileStream fs = new FileStream("Hash/dirlist.txt", FileMode.Open);
                StreamReader reader = new StreamReader(fs);

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    AddDirectory(line);
                }

                reader.Close();
                fs.Close();
            }
        }

        /// <summary>
        /// Saves the hashlist to a new file
        /// </summary>
        public void SaveHashList()
        {
            if (File.Exists("Hash/hashes_filename.txt")) File.Delete("Hash/hashes_filename.txt");
            FileStream fs = new FileStream("Hash/hashes_filename.txt", FileMode.OpenOrCreate);
            StreamWriter writer = new StreamWriter(fs);

            if (File.Exists("Hash/hashes_only_filename.txt")) File.Delete("Hash/hashes_only_filename.txt");
            FileStream fs_hof = new FileStream("Hash/hashes_only_filename.txt", FileMode.OpenOrCreate);
            StreamWriter writer_hof = new StreamWriter(fs_hof);

            if (File.Exists("pattern_num.txt")) File.Delete("pattern_num.txt");
            FileStream fs_pn = new FileStream("pattern_num.txt", FileMode.OpenOrCreate);
            StreamWriter writer_pn = new StreamWriter(fs_pn);

            for (int i = 0; i < hashList.Count; i++)
            {
                writer.WriteLine("{0:X8}#{1:X8}#{2}", (uint)(hashList.Keys[i] >> 32), (uint)(hashList.Keys[i] & 0xFFFFFFFF), hashList.Values[i]); ;
                if (hashList.Values[i] != "")
                {
                    writer_hof.WriteLine("{0:X8}#{1:X8}#{2}", (uint)(hashList.Keys[i] >> 32), (uint)(hashList.Keys[i] & 0xFFFFFFFF), hashList.Values[i]); ;
                    writer_pn.WriteLine(hashList.Values[i]);
                }
            }

            writer.Close();
            fs.Close();

            writer_hof.Close();
            fs_hof.Close();

            writer_pn.Close();
            fs_pn.Close();

            if (File.Exists("fileList.txt")) File.Delete("fileList.txt");
            fs = new FileStream("fileList.txt", FileMode.OpenOrCreate);
            writer = new StreamWriter(fs);

            for (int i = 0; i < fileListing.Count; i++)
            {
                writer.WriteLine(fileListing[i]);
            }

            writer.Close();
            fs.Close();

            if (File.Exists("extList.txt")) File.Delete("extList.txt");
            fs = new FileStream("extList.txt", FileMode.OpenOrCreate);
            writer = new StreamWriter(fs);

            for (int i = 0; i < extListing.Count; i++)
            {
                writer.WriteLine(extListing[i]);
            }

            writer.Close();
            fs.Close();

            if (File.Exists("Hash/dirlist.txt")) File.Delete("Hash/dirlist.txt");
            fs = new FileStream("Hash/dirlist.txt", FileMode.OpenOrCreate);
            writer = new StreamWriter(fs);

            for (int i = 0; i < dirListing.Count; i++)
            {
                writer.WriteLine(dirListing[i]);
            }

            writer.Close();
            fs.Close();
        }

        /// <summary>
        /// Searches the hashlist
        /// </summary>
        /// <param name="ph"></param>
        /// <param name="sh"></param>
        /// <returns>"" if filename could not be found, filename otherwise</returns>
        public string SearchHashList(uint ph, uint sh)
        {
            long sig = ((long)ph << 32) + sh;
            if (hashList.ContainsKey(sig))
            {
                return hashList[sig];
            }
            return "";
        }

    }

}
