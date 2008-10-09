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
using RedBlackCS;
using WarhammerOnlineHash;

namespace WarhammerOnlineHashBuilder
{
    public class HashTreeObject
    {
        private long myKey;
        private string myData;

        public long Key
        {
            get { return myKey; }
            set { myKey = value; }
        }
        public string Data
        {
            get { return myData; }
            set { myData = value; }
        }

        public HashTreeObject(long key, string data)
        {
            this.Key = key;
            this.Data = data;
        }
    }

    public class HashTreeKey : IComparable
    {
        private long myKey;
        public long Key
        {
            get { return myKey; }
            set { myKey = value; }
        }
        public HashTreeKey(long key)
        {
            myKey = key;
        }

        public int CompareTo(object key)
        {
            if (Key > ((HashTreeKey)key).Key)
                return 1;
            else
                if (Key < ((HashTreeKey)key).Key)
                    return -1;
                else
                    return 0;
        }
    }

    public class HashData
    {
        public string filename;
        public int crc;
        public uint ph;
        public uint sh;

        public HashData(uint ph, uint sh, string filename, int crc)
        {
            this.ph = ph;
            this.sh = sh;
            this.filename = filename;
            this.crc = crc;
        }

        //(uint)Convert.ToUInt32(strsplt[0], 16);
    }

    public class Hasher
    {
        SortedList<long, HashData> hashList = new SortedList<long, HashData>();
        RedBlack hashtree = new RedBlack();
        List<string> dirListing = new List<string>();
        List<string> fileListing = new List<string>();
        List<string> extListing = new List<string>();

        bool dev = false;

        public SortedList<long, HashData> HashList { get { return hashList; } }
        public List<string> DirListing { get { return dirListing; } }
        public List<string> ExtListing { get { return extListing; } }
        public List<string> FileListing { get { return fileListing; } }

        string dictionaryFile = "Hash/hashes_filename.txt";
        string directoryListingFile = "Hash/dirlist.txt";

        #region Constructors
        /// <summary>
        /// Creates a new hasher with default dictionary and dir listing
        /// Dictionnary: Hash/hashes_filename.txt
        /// DirListing: Hash/dirlist.Txt
        /// </summary>
        /// <param name="dev">Builder will create directory,filename and extension files</param>
        public Hasher(bool dev)
        {
            this.dev = dev;
        }
        /// <summary>
        /// Creates a new hasher with default dir listing
        /// DirListing: Hash/dirlist.Txt
        /// </summary>
        /// <param name="dev">Builder will create directory,filename and extension files</param>
        /// <param name="dictionaryFile">dictionary file to use file format should be ph#sh#filename</param>
        public Hasher(bool dev, string dictionaryFile)
        {
            this.dev = dev;
            this.dictionaryFile = dictionaryFile;
        }

        /// <summary>
        /// Creates a new hasher
        /// </summary>
        /// <param name="dev">Builder will create directory,filename and extension files</param>
        /// <param name="dictionaryFile">dictionary file to use file format should be ph#sh#filename</param>
        /// <param name="directoryListingFile">dir listing file format should be one full path folder per line</param>
        public Hasher(bool dev, string dictionaryFile, string directoryListingFile)
            : this(dev, dictionaryFile)
        {
            this.directoryListingFile = directoryListingFile;
        }
        #endregion

        /// <summary>
        /// Add a key to the hash tree
        /// </summary>
        /// <param name="ph"></param>
        /// <param name="sh"></param>
        /// <param name="name"></param>
        void AddHashTree(uint ph, uint sh, string name)
        {
            long sig = (((long)ph) << 32) + sh;
            hashtree.Add(new HashTreeKey(sig), new HashTreeObject(sig, name));
        }

        void AddHash(uint ph, uint sh, string name, int crc)
        {
            long sig = (((long)ph) << 32) + sh;
            hashList.Add(sig, new HashData(ph, sh, name, crc));
            AddDirectory(name);
        }

        #region Generation Helpers
        /// <summary>
        /// Add a hash entry, used when parsing the myp files in a folder
        /// Used in the generation process though F1 to add just a hash to the list.
        /// </summary>
        /// <param name="ph">ph value</param>
        /// <param name="sh">sh value</param>
        public void AddHash(uint ph, uint sh)
        {
            long sig = (((long)ph) << 32) + sh;
            //if the list contains the sig, then we update
            if (!hashList.ContainsKey(sig))
            {
                hashList.Add(sig, new HashData(ph, sh, "", 0));
            }
        }

        /// <summary>
        /// Update hash with name if the hash can be found in the hash list
        /// This is used for generation purposes
        /// </summary>
        /// <param name="ph">ph value</param>
        /// <param name="sh">sh value</param>
        /// <param name="name">equivalent of the hash as a string</param>
        public bool UpdateHash(uint ph, uint sh, string name, int crc)
        {
            long sig = (((long)ph) << 32) + sh;
            //if the list contains the sig, then we update
            if (hashList.ContainsKey(sig))
            {
                // the != name shoud actually be != ""
                if (hashList[sig].filename != name && name != "")
                {
                    hashList[sig].filename = name;
                }
                if (crc != 0)
                {
                    hashList[sig].crc = crc;
                }

                AddDirectory(name);
                AddFile(name);
                return true;
            }
            return false;
        }

        /// <summary>
        /// See UpdateHash
        /// </summary>
        /// <param name="ph"></param>
        /// <param name="sh"></param>
        /// <param name="name"></param>
        public void UpdateTreeHash(uint ph, uint sh, string name)
        {
            long sig = (((long)ph) << 32) + sh;
            string tmpstr;
            if ((tmpstr = SearchHashTree(ph, sh)) != "")
            {
                ((HashTreeObject)hashtree.GetData(new HashTreeKey(sig))).Data = name;
            }
        }

        /// <summary>
        /// Adds a directory to the directory list
        /// Used for generation purposes
        /// </summary>
        /// <param name="filename"></param>
        public void AddDirectory(string filename)
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

        /// <summary>
        /// Adds a filename to the filename list without extension, if there is an extension it is removed
        /// Used for generation purposes
        /// </summary>
        /// <param name="filename"></param>
        public void AddFile(string filename)
        {
            if (filename.IndexOf(".") >= 0)
            {
                AddExtension(filename);
            }

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

        /// <summary>
        /// Adds an extension to the extension list
        /// Used for generation purposes
        /// </summary>
        /// <param name="filename"></param>
        public void AddExtension(string filename)
        {
            string ext = filename.Split('.')[filename.Split('.').Length - 1];
            if (!extListing.Contains(ext))
            {
                extListing.Add(ext);
            }
        }
        #endregion

        #region Hash to Filename list construction
        public void BuildHashList(string dictionaryFile)
        {
            this.dictionaryFile = dictionaryFile;
            BuildHashList();
        }

        /// <summary>
        /// Creates a sorted list (hashlist) based on the dictionary file
        /// Deprecated since Tree were put in place by Vjeux: need to remove it from all project though.
        /// </summary>
        public void BuildHashList()
        {
            LoadDirListing();

            if (File.Exists(dictionaryFile))
            {
                FileStream fs = new FileStream(dictionaryFile, FileMode.Open);
                StreamReader reader = new StreamReader(fs);

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] strsplt = line.Split('#');
                    uint ph = (uint)Convert.ToUInt32(strsplt[0], 16);
                    uint sh = (uint)Convert.ToUInt32(strsplt[1], 16);
                    string filename = strsplt[2];
                    int crc;
                    if (strsplt.Length > 3)
                    {
                        crc = (int)Convert.ToUInt32(strsplt[3], 16);
                    }
                    else
                    {
                        crc = 0;
                    }

                    AddHash(ph, sh, filename, crc);
                    OnHashEvent(new HashEventArgs(HashState.Building, (float)reader.BaseStream.Position / (float)reader.BaseStream.Length));
                }

                reader.Close();
                fs.Close();
            }
            OnHashEvent(new HashEventArgs(HashState.Finished, 100f));
        }

        /// <summary>
        /// Creates a hash tree based on the dictionnary file
        /// </summary>
        public void BuildHashTree()
        {
            LoadDirListing();

            if (File.Exists(dictionaryFile))
            {
                FileStream fs = new FileStream(dictionaryFile, FileMode.Open);
                StreamReader reader = new StreamReader(fs);

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] strsplt = line.Split('#');
                    uint ph = (uint)Convert.ToUInt32(strsplt[0], 16);
                    uint sh = (uint)Convert.ToUInt32(strsplt[1], 16);
                    string filename = strsplt[2];

                    AddHashTree(ph, sh, filename);
                    OnHashEvent(new HashEventArgs(HashState.Building, (float)reader.BaseStream.Position / (float)reader.BaseStream.Length));
                }

                reader.Close();
                fs.Close();
            }

            OnHashEvent(new HashEventArgs(HashState.Finished, 100f));
        }

        public void MergeHashList(object obj)
        {
            MergeHashList((string)obj);
        }

        /// <summary>
        /// Check to see if values from an old dictionnary list can be added to the current dictionnary
        /// Used because Mythic changed 60k filenames in the beta3... so had to make a clean hash listing
        /// then transfer all the values that are correct from the old list to this new list
        /// </summary>
        /// <param name="file"></param>
        public void MergeHashList(string file)
        {
            //LoadDirListing();

            if (File.Exists(file))
            {
                FileStream fs = new FileStream(file, FileMode.Open);
                StreamReader reader = new StreamReader(fs);

                WarHasher warhash = new WarHasher();

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] strsplt = line.Split('#');
                    uint ph = (uint)Convert.ToUInt32(strsplt[0], 16);
                    uint sh = (uint)Convert.ToUInt32(strsplt[1], 16);
                    string filename = strsplt[2];
                    int crc;
                    if (strsplt.Length > 3)
                    {
                        crc = (int)Convert.ToUInt32(strsplt[3], 16);
                    }
                    else
                    {
                        crc = 0;
                    }

                    warhash.Hash(filename, 0xDEADBEEF);

                    if (!UpdateHash(ph, sh, filename, crc))
                    {
                        // Add hashes from old files too... just because of the US and EU hash lists that are different, will also help to use this method as a merge
                        AddHash(ph, sh, filename, crc);
                        //UpdateHash(ph, sh, filename, crc);
                    }
                    //UpdateTreeHash(ph, sh, filename);
                    OnHashEvent(new HashEventArgs(HashState.Building, (float)reader.BaseStream.Position / (float)reader.BaseStream.Length));
                }

                reader.Close();
                fs.Close();
            }
            SaveHashList();
            OnHashEvent(new HashEventArgs(HashState.Finished, 100f));
        }

        /// <summary>
        /// Builds a directory list from directory file.
        /// </summary>
        void LoadDirListing()
        {
            AddDirectory("");

            if (File.Exists(directoryListingFile))
            {
                FileStream fs = new FileStream(directoryListingFile, FileMode.Open);
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
        #endregion

        /// <summary>
        /// Saves the hashlist to a new file
        /// </summary>
        public void SaveHashList()
        {
            string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

            DateTime centuryBegin = new DateTime(2001, 1, 1);
            DateTime currentDate = DateTime.Now;
            long elapsedTicks = currentDate.Ticks - centuryBegin.Ticks;
            TimeSpan elapsedSpan = new TimeSpan(elapsedTicks);

            string dicFile = path + "/" + dictionaryFile;
            string[] folders = dicFile.Split('/');
            string tmpPath = folders[0];
            if (!Directory.Exists(tmpPath)) Directory.CreateDirectory(tmpPath);

            for (int i = 1; i < folders.Length - 1; i++)
            {
                tmpPath += '/' + folders[i];
                if (!Directory.Exists(tmpPath)) Directory.CreateDirectory(tmpPath);
            }

            if (File.Exists(dicFile)) File.Move(dicFile, path + "/Hash/oldHashList_" + elapsedSpan.TotalSeconds.ToString() + ".txt");
            FileStream fs = new FileStream(dicFile, FileMode.OpenOrCreate);
            StreamWriter writer = new StreamWriter(fs);

            for (int i = 0; i < hashList.Count; i++)
            {
                if (hashList.Values[i].crc != 0)
                {
                }
                writer.WriteLine("{0:X8}#{1:X8}#{2}#{3:X8}", (uint)(hashList.Keys[i] >> 32), (uint)(hashList.Keys[i] & 0xFFFFFFFF), hashList.Values[i].filename, hashList.Values[i].crc);
            }

            writer.Close();
            fs.Close();

            if (dev || true)
            {
                if (!Directory.Exists(path + "/Hash")) Directory.CreateDirectory(path + "/Hash");

                if (File.Exists(path + "/Hash/hashes_only_filename.txt")) File.Delete(path + "/Hash/hashes_only_filename.txt");
                FileStream fs_hof = new FileStream(path + "/Hash/hashes_only_filename.txt", FileMode.OpenOrCreate);
                StreamWriter writer_hof = new StreamWriter(fs_hof);

                if (File.Exists(path + "/pattern_num.txt")) File.Delete(path + "/pattern_num.txt");
                FileStream fs_pn = new FileStream(path + "/pattern_num.txt", FileMode.OpenOrCreate);
                StreamWriter writer_pn = new StreamWriter(fs_pn);

                for (int i = 0; i < hashList.Count; i++)
                {
                    if (hashList.Values[i].filename != "")
                    {
                        writer_hof.WriteLine("{0:X8}#{1:X8}#{2}#{3:X8}", (uint)(hashList.Keys[i] >> 32), (uint)(hashList.Keys[i] & 0xFFFFFFFF), hashList.Values[i].filename, hashList.Values[i].crc); ;
                        writer_pn.WriteLine(hashList.Values[i].filename);
                    }
                }

                writer_hof.Close();
                fs_hof.Close();

                writer_pn.Close();
                fs_pn.Close();

                if (File.Exists(path + "/fileList.txt")) File.Delete(path + "/fileList.txt");
                fs = new FileStream(path + "/fileList.txt", FileMode.OpenOrCreate);
                writer = new StreamWriter(fs);

                for (int i = 0; i < fileListing.Count; i++)
                {
                    writer.WriteLine(fileListing[i]);
                }

                writer.Close();
                fs.Close();


                if (File.Exists(path + "/extList.txt")) File.Delete(path + "/extList.txt");
                fs = new FileStream(path + "/extList.txt", FileMode.OpenOrCreate);
                writer = new StreamWriter(fs);

                for (int i = 0; i < extListing.Count; i++)
                {
                    writer.WriteLine(extListing[i]);
                }

                writer.Close();
                fs.Close();
            }

            if (File.Exists(path + "/" + directoryListingFile)) File.Delete(path + "/" + directoryListingFile);
            fs = new FileStream(path + "/" + directoryListingFile, FileMode.OpenOrCreate);
            writer = new StreamWriter(fs);

            for (int i = 0; i < dirListing.Count; i++)
            {
                writer.WriteLine(dirListing[i]);
            }

            writer.Close();
            fs.Close();
        }

        /// <summary>
        /// Search the hash tree
        /// </summary>
        /// <param name="ph"></param>
        /// <param name="sh"></param>
        /// <returns></returns>
        public string SearchHashTree(uint ph, uint sh)
        {
            long sig = ((long)ph << 32) + sh;

            try
            {
                return ((HashTreeObject)hashtree.GetData(new HashTreeKey(sig))).Data;
            }
            catch (Exception e)
            {
                return "";
            }
        }

        /// <summary>
        /// Searches the hashlist
        /// </summary>
        /// <param name="ph"></param>
        /// <param name="sh"></param>
        /// <returns>returns "" if filename could not be found, filename otherwise</returns> 
        public HashData SearchHashList(uint ph, uint sh, int crc)
        {
            long sig = ((long)ph << 32) + sh;
            if (hashList.ContainsKey(sig))
            {
                return hashList[sig];
            }
            return null;
        }

        public void UpdateCRC(uint ph, uint sh, int crc)
        {
            long sig = ((long)ph << 32) + sh;
            if (hashList.ContainsKey(sig))
            {
                hashList[sig].crc = crc;
            }
        }

        #region Events
        public event HashEventHandler HashEvent;

        private void OnHashEvent(HashEventArgs e)
        {
            if (HashEvent != null)
            {
                HashEvent(this, e);
            }
        }
        #endregion
    }

    public delegate void HashEventHandler(object sender, HashEventArgs e);

    public enum HashState
    {
        Building,
        Finished
    }

    public class HashEventArgs : EventArgs
    {
        HashState state;
        float value;

        public float Value { get { return value; } }
        public HashState State { get { return state; } }

        public HashEventArgs(HashState state, float value)
        {
            this.state = state;
            this.value = value;
        }
    }
}
