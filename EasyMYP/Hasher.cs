using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using WarhammerOnlineHash;

namespace MYPHasherBuilder
{
    public class Hasher
    {
        public SortedList<long, string> hashList = new SortedList<long, string>();
        public List<string> dirListing = new List<string>();

        public void BuildHashList()
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
                long sig = ((long)ph << 32) + sh;

                if (!hashList.ContainsKey(sig))
                {
                    hashList.Add(sig, filename);
                }
                if (filename.IndexOf('/') >= 0 || filename.IndexOf('\\') >= 0)
                {
                    string parsefn = filename.Replace('\\', '/');
                    string path = parsefn.Substring(0, filename.LastIndexOf('/'));
                    if (!dirListing.Contains(path))
                    {
                        dirListing.Add(path);
                    }
                }
            }

            reader.Close();
            fs.Close();
        }

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
