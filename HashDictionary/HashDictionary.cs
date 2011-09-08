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
using System.Collections.Generic;
using System.IO;
//using RedBlackCS;

namespace nsHashDictionary
{
    #region hashtree (for future use)
    /*
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
    */
    #endregion
    public class HashData
    {
        public string archiveName;
        public string filename;
        public int crc;
        public uint ph;
        public uint sh;

        public HashData(uint ph, uint sh, string filename, int crc, string archiveName)
        {
            this.ph = ph;
            this.sh = sh;
            this.filename = filename;
            this.crc = crc;
            this.archiveName = archiveName;
        }

    }

    public enum UpdateResults
    {
        NOT_FOUND,
        UPTODATE,
        NAME_UPDATED,
        ARCHIVE_UPDATED
    }

    public class HashDictionary
    {

        ///  \todo Speed: move that to an SortedDictionnary?
        SortedList<string, SortedList<long, HashData>> hashList = new SortedList<string, SortedList<long, HashData>>();
        //RedBlack hashtree = new RedBlack();
        HashSet<string> dirListing = new HashSet<string>();
        HashSet<string> fileListing = new HashSet<string>();
        HashSet<string> extListing = new HashSet<string>();

        public SortedList<string, SortedList<long, HashData>> HashList { get { return hashList; } }
        public HashSet<string> DirListing { get { return dirListing; } }
        public HashSet<string> ExtListing { get { return extListing; } }
        public HashSet<string> FileListing { get { return fileListing; } }

        string dictionaryFile = "Hash/hashes_filename.txt";
        string directoryListingFile = "Hash/dirlist.txt";
        string extensionListingFile = "Hash/extList.txt";
        string fileListingFile = "Hash/fileList.txt";
        bool helpersCreated = false;
        public bool needsSave = false;
        public static char hashSeparator = '?';

        #region Constructors
        /// <summary>
        /// Creates a new hasher with default dir listing
        /// DirListing: Hash/dirlist.Txt
        /// </summary>
        public HashDictionary()
        {
        }        /// <summary>
        /// Creates a new hasher with default dir listing
        /// DirListing: Hash/dirlist.Txt
        /// </summary>
        /// <param name="dictionaryFile">dictionary file to use file format should be ph#sh#filename</param>
        public HashDictionary(string dictionaryFile)
        {
            this.dictionaryFile = dictionaryFile;
        }
        #endregion

        #region HashList manipulation


        public void AddHash(uint ph, uint sh, string archiveName)
        {
            AddHash(ph, sh, "", 0, archiveName);
        }

        /// <summary>
        /// Add/update a hash entry
        /// </summary>
        /// <param name="ph">ph value</param>
        /// <param name="sh">sh value</param>
        public void AddHash(uint ph, uint sh, string name, int crc, string archiveName)
        {
            long sig = (((long)ph) << 32) + sh;
            if (!hashList[archiveName].ContainsKey(sig))
            {
                hashList[archiveName].Add(sig, new HashData(ph, sh, name, crc, archiveName));
                needsSave = true;
                if (name.CompareTo("") != 0)
                {
                    AddDirectory(name);
                    AddFileandExtension(name);
                }
            }
            else

                UpdateHash(ph, sh, name, crc, archiveName);
        }

        public void LoadHash(uint ph, uint sh, string name, int crc, string archiveName)
        {
            long sig = (((long)ph) << 32) + sh;
            if (!hashList.ContainsKey(archiveName))
            {
                hashList.Add(archiveName, new SortedList<long, HashData>());
            }
            hashList[archiveName].Add(sig, new HashData(ph, sh, name, crc, archiveName));
        }

        /// <summary>
        /// Lookup in all the archives if a hash matches
        /// Update hash with name if the hash can be found in the hash list
        /// This is used for generation purposes
        /// </summary>
        /// <param name="ph">ph value</param>
        /// <param name="sh">sh value</param>
        /// <param name="name">equivalent of the hash as a string</param>
        /// <returns>0=not found, 1=already up-to-date, 2= name updated, 3=archive updated</returns>
        public UpdateResults UpdateHash(uint ph, uint sh, string name, int crc)
        {
            int i = 0;
            UpdateResults result = UpdateResults.NOT_FOUND;
            while (i < hashList.Count && (result = UpdateHash(ph, sh, name, crc, hashList.Keys[i])) == UpdateResults.NOT_FOUND)
            {
                i++;
            }
            return result;
        }

        /// <summary>
        /// Update hash with name if the hash can be found in the hash list
        /// This is used for generation purposes
        /// </summary>
        /// <param name="ph">ph value</param>
        /// <param name="sh">sh value</param>
        /// <param name="name">equivalent of the hash as a string</param>
        /// <param name="archiveName">the name of the archive in which to look / update</param>
        /// <returns>0=not found, 1=already up-to-date, 2= name updated, 3=archive updated</returns>
        public UpdateResults UpdateHash(uint ph, uint sh, string name, int crc, string archiveName)
        {
            long sig = (((long)ph) << 32) + sh;
            UpdateResults result = UpdateResults.NOT_FOUND;
            //if the list contains the sig, then we update
            if (hashList[archiveName].ContainsKey(sig))
            {
                result = UpdateResults.UPTODATE;

                if (name != "" && hashList[archiveName][sig].filename != name)
                {
                    // updates the filename if it has changed
                    hashList[archiveName][sig].filename = name;
                    result = UpdateResults.NAME_UPDATED;
                    AddDirectory(name);
                    AddFileandExtension(name);
                    needsSave = true;
                }
                if (archiveName != hashList[archiveName][sig].archiveName)
                {
                    // updates the archivename if the file has switched archive
                    hashList[archiveName][sig].archiveName = archiveName;
                    result = UpdateResults.ARCHIVE_UPDATED;
                    needsSave = true;
                }
                if (crc != 0)
                {
                    hashList[archiveName][sig].crc = crc;
                    needsSave = true;
                }
            }
            return result;
        }

        public void UpdateCRC(uint ph, uint sh, int crc, string archiveName)
        {
            long sig = ((long)ph << 32) + sh;
            if (hashList[archiveName].ContainsKey(sig))
            {
                hashList[archiveName][sig].crc = crc;
                needsSave = true;
            }
        }

        public void LoadHashList()
        {
            LoadHashList(this.dictionaryFile, false);
        }
        public void MergeHashList(object obj)
        {
            LoadHashList((string)obj, true);
        }

        /// <summary>
        /// Creates/merges a sorted list (hashlist) based on the dictionary file
        /// </summary>
        private void LoadHashList(string file, bool merge)
        {

            if (!Path.IsPathRooted(file))
            {
                string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                file = path + "/" + file;
            }

            if (File.Exists(file))
            {
                if (merge)
                    CreateHelpers();

                FileStream fs = new FileStream(file, FileMode.Open);
                StreamReader reader = new StreamReader(fs);

                string line;
                int i = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    i++;
                    string[] strsplt = line.Split(hashSeparator);
                    string archiveName = strsplt[0];
                    uint ph = Convert.ToUInt32(strsplt[1], 16);
                    uint sh = Convert.ToUInt32(strsplt[2], 16);
                    string filename = strsplt[3];
                    int crc;
                    if (strsplt.Length > 4)
                    {
                        crc = Convert.ToInt32(strsplt[4], 16);
                    }
                    else
                    {
                        crc = 0;
                    }

                    if (merge)
                        AddHash(ph, sh, filename, crc, archiveName);
                    else
                        LoadHash(ph, sh, filename, crc, archiveName);
                    if (i % 200 == 0)
                        TriggerHashEvent(new DictionaryEventArgs(DictionaryState.Building, (float)reader.BaseStream.Position / (float)reader.BaseStream.Length));
                }

                reader.Close();
                fs.Close();
            }
            TriggerHashEvent(new DictionaryEventArgs(DictionaryState.Finished, 100f));
        }

        public void SaveHashList(string dictionaryFile)
        {
            this.dictionaryFile = dictionaryFile;
            SaveHashList();
        }

        /// <summary>
        /// Saves the hashlist and all dir/file/ext to new files
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

            if (!Directory.Exists(path + "/Hash")) Directory.CreateDirectory(path + "/Hash");

            //Save dictionary
            if (File.Exists(dicFile)) File.Move(dicFile, path + "/Hash/oldHashList_" + elapsedSpan.TotalSeconds.ToString() + ".txt");
            FileStream fs = new FileStream(dicFile, FileMode.OpenOrCreate);
            StreamWriter writer = new StreamWriter(fs);
            SortedList<long, HashData> subHashList;

            for (int j = 0; j < hashList.Count; j++)
            {
                subHashList = hashList.Values[j];
                for (int i = 0; i < subHashList.Count; i++)
                {
                    writer.WriteLine("{0:X8}" + hashSeparator
                        + "{1:X8}" + hashSeparator
                        + "{2:X8}" + hashSeparator
                        + "{3}" + hashSeparator
                        + "{4:X8}"
                        , subHashList.Values[i].archiveName
                        , (uint)(subHashList.Keys[i] >> 32)
                        , (uint)(subHashList.Keys[i] & 0xFFFFFFFF)
                        , subHashList.Values[i].filename, subHashList.Values[i].crc);

                    if (i % 200 == 0)
                        TriggerHashEvent(new DictionaryEventArgs(DictionaryState.Building, (float)i / (float)hashList.Count));
                }
            }

            writer.Close();
            fs.Close();

            TriggerHashEvent(new DictionaryEventArgs(DictionaryState.Finished, 100f));
        }

        /// <summary>
        /// Searches the hashlist
        /// </summary>
        /// <param name="ph"></param>
        /// <param name="sh"></param>
        /// <returns>returns the HashData object or null</returns> 
        public HashData SearchOtherArchive(uint ph, uint sh, string archiveName)
        {
            long sig = ((long)ph << 32) + sh;
            HashData result = null;
            for (int i = 0; i < hashList.Count; i++)
            {
                if (hashList.Keys[i] != archiveName)
                {
                    if (hashList.Values[i].ContainsKey(sig))
                    {
                        result = hashList.Values[i][sig];
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Searches in all the archives hashlists
        /// </summary>
        /// <param name="ph"></param>
        /// <param name="sh"></param>
        /// <returns>returns the HashData object or null</returns> 
        public HashData SearchHashList(uint ph, uint sh, string archiveName)
        {
            long sig = ((long)ph << 32) + sh;
            if (!hashList.ContainsKey(archiveName))
            {
                hashList.Add(archiveName, new SortedList<long, HashData>());
            }
            if (hashList[archiveName].ContainsKey(sig))
            {
                return hashList[archiveName][sig];
            }
            return null;
        }

        #endregion

        #region HashTree manipulation (for future usage)
        /*
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
 */
        #endregion

        #region Generation Helpers

        /// <summary>
        /// Adds a directory to the directory list
        /// Used for generation purposes
        /// </summary>
        /// <param name="filename"></param>
        public void AddDirectory(string filename)
        {
            string file = filename.Replace('\\', '/');

            if (file.IndexOf('/') >= 0 && file.LastIndexOf('.') >= 0)
            {
                string dir = file.Substring(0, filename.LastIndexOf('/'));

                // why this check?
                if (dir.IndexOf(' ') < 0)
                {
                    while (!dirListing.Contains(dir)) //we check explicitly to cut the loop if the root of the directory is already known.
                    {
                        dirListing.Add(dir);
                        if (dir.LastIndexOf('/') >= 0)
                            dir = dir.Substring(0, dir.LastIndexOf('/'));
                        else
                            break;
                    }
                }
            }
            else
            {
                if (file.IndexOf(' ') < 0)
                {
                    dirListing.Add(file);
                }
            }
        }

        /// <summary>
        /// Adds a filename to the filename list without extension, if there is an extension it is removed
        /// Used for generation purposes
        /// </summary>
        /// <param name="filename"></param>
        public void AddFileandExtension(string filename)
        {
            string cur_fn = filename.Replace('\\', '/');

            if (cur_fn.IndexOf('/') >= 0)
            {
                cur_fn = cur_fn.Substring(cur_fn.LastIndexOf('/') + 1);
                if (cur_fn.Contains("."))
                    cur_fn = cur_fn.Substring(0, cur_fn.LastIndexOf('.')); ;
            }

            fileListing.Add(cur_fn);

            AddExtension(filename);
        }

        /// <summary>
        /// Adds an extension to the extension list
        /// Used for generation purposes
        /// </summary>
        /// <param name="filename"></param>
        public void AddExtension(string filename)
        {
            if (filename.IndexOf(".") >= 0)
            {
                string ext = filename.Substring(filename.LastIndexOf('.') + 1);
                extListing.Add(ext);
            }
        }

        // not used at the moment
        private void LoadList(HashSet<String> theList, string fileName)
        {
            FileStream fs;
            StreamReader reader;
            // Save filenames, extensions and dirs
            if (File.Exists(fileName))
            {
                fs = new FileStream(fileName, FileMode.Open);
                reader = new StreamReader(fs);

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    theList.Add(line);
                }

                reader.Close();
                fs.Close();
            }
        }

        public void CreateHelpers()
        {
            // check if this is not already created
            if (!helpersCreated)
            {
                int i = 0;
                SortedList<long, HashData> subHashList;

                for (int j = 0; j < hashList.Count; j++)
                {
                    subHashList = hashList.Values[j];
                    for (i = 0; i < subHashList.Count; i++)
                    {
                        AddDirectory(subHashList.Values[i].filename);
                        AddFileandExtension(subHashList.Values[i].filename);
                    }
                }
                helpersCreated = true;
            }
        }

        public void SaveHelpers()
        {
            string rootPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            SaveList(dirListing, rootPath + "/" + directoryListingFile);
            SaveList(fileListing, rootPath + "/" + fileListingFile);
            SaveList(extListing, rootPath + "/" + extensionListingFile);
        }

        private void SaveList(HashSet<String> theHashSet, string fileName)
        {
            FileStream fs;
            StreamWriter writer;
            // Save filenames, extensions and dirs
            if (File.Exists(fileName)) File.Delete(fileName);
            fs = new FileStream(fileName, FileMode.OpenOrCreate);
            writer = new StreamWriter(fs);

            foreach (String item in theHashSet)
            {
                writer.WriteLine(item);
            }

            writer.Close();
            fs.Close();
        }

        #endregion

        #region Events
        public event DictionaryEventHandler HashEvent;

        private void TriggerHashEvent(DictionaryEventArgs e)
        {
            if (HashEvent != null)
            {
                HashEvent(this, e);
            }
        }
        #endregion

    }

    public delegate void DictionaryEventHandler(object sender, DictionaryEventArgs e);

    public enum DictionaryState
    {
        Building,
        Finished
    }

    public class DictionaryEventArgs : EventArgs
    {
        DictionaryState state;
        float value;

        public float Value { get { return value; } }
        public DictionaryState State { get { return state; } }

        public DictionaryEventArgs(DictionaryState state, float value)
        {
            this.state = state;
            this.value = value;
        }
    }
}
