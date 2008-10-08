using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

using WarhammerHash;

namespace MYPHasherBuilder
{
    public class Hasher_v1
    {
        public SortedList<uint,PrimaryHashEntry> hashList = new SortedList<uint,PrimaryHashEntry>();

        public void BuildHashList()
        {
            dirListing = new List<string>();

            if (File.Exists("hashes.xml"))
            {

                XmlSerializer serializer = new XmlSerializer(typeof(List<PrimaryHashEntry>));
                FileStream xmlStream = new FileStream("hashes.xml", FileMode.OpenOrCreate);
                List<PrimaryHashEntry> tmpList = (List<PrimaryHashEntry>)serializer.Deserialize(xmlStream);

                for (int i = 0; i < tmpList.Count; i++)
                {
                    hashList.Add(tmpList[i].ph, tmpList[i]);
                }

                xmlStream.Close();

            }
            else
            {
                TreatHashFile("lists/hashlist_0.txt");
                TreatHashFile("lists/hashlist_1.txt");
                TreatHashFile("lists/hashlist_2.txt");
                TreatHashFile("lists/hashlist_3.txt");
                TreatHashFile("lists/hashlist_4.txt");

                TreatCSVFile("lists/warhammer.csv", 0);
                TreatCSVFile("lists/warhammer2.csv", 0);
                TreatCSVFile("lists/warhammer3.csv", 0);
                TreatCSVFile("lists/warhammer4.csv", 0);
                //TreatCSVFile("lists/warhammer5.csv", 0);
                //TreatCSVFile("lists/warhammer6.csv", 0);
                //TreatCSVFile("lists/warhammer7.csv", 0);

                TreatCSVFile("lists/path_0.csv", 0);
                TreatCSVFile("lists/path_1.csv", 0);
                TreatCSVFile("lists/path_2.csv", 0);
                TreatCSVFile("lists/path_3.csv", 0);


                XmlSerializer serializer = new XmlSerializer(typeof(List<PrimaryHashEntry>));
                FileStream xmlStream = new FileStream("hashes.xml", FileMode.OpenOrCreate);

                List<PrimaryHashEntry> tmpList = new List<PrimaryHashEntry>();

                for (int i = 0; i < hashList.Count; i++)
                {
                    tmpList.Add(hashList.Values[i]);
                }

                serializer.Serialize(xmlStream, tmpList);

                xmlStream.Close();

                FileStream fs = new FileStream("longhashlist.txt", FileMode.OpenOrCreate);
                StreamWriter streamwriter = new StreamWriter(fs);

                for (int i = 0; i < tmpList.Count; i++)
                {
                    for (int j = 0; j < tmpList[i].shList.Count; j++)
                    {
                        streamwriter.WriteLine("{0}{1}:{2}", tmpList[i].ph.ToString("X8"), tmpList[i].shList[j].sh.ToString("X8"), tmpList[i].shList[j].filename);
                    }
                }
                streamwriter.Close();
                fs.Close();

                fs = new FileStream("dirListing.txt", FileMode.OpenOrCreate);
                streamwriter = new StreamWriter(fs);

                for (int i = 0; i < dirListing.Count; i++)
                {
                    streamwriter.WriteLine("{0}", dirListing[i]);
                }
                streamwriter.Close();
                fs.Close();
            }
        }

        WarhammerHash1 hasher;
        List<string> dirListing;

        void TreatCSVFile(string csvFileName, int column)
        {
            FileStream hashFS = new FileStream(csvFileName, FileMode.Open);
            StreamReader reader = new StreamReader(hashFS);
            hasher = new WarhammerHash1();
            string line;
            line = reader.ReadLine();
            while ((line = reader.ReadLine()) != null)
            {
                string aFileName = (line.Split(','))[column];

                aFileName = aFileName.Replace("\"", "");

                if (aFileName.IndexOf("F:\\Jeux\\WAR\\") >= 0 || aFileName.IndexOf(':') < 0)
                {
                    aFileName = aFileName.Replace("F:\\Jeux\\WAR\\", "");
                    string modFileName = aFileName;

                    if (aFileName.IndexOf('.') >= 0)
                    {
                        TreatString(aFileName);
                        if (aFileName.Replace('\\', '/').IndexOf('/') >= 0)
                        {
                            string dir = aFileName.Replace('\\', '/').Substring(0, aFileName.Replace('\\', '/').LastIndexOf('/'));
                            if (!dirListing.Contains(dir))
                            {
                                dirListing.Add(dir);
                            }
                        }
                    }
                    else
                    {
                        if (!dirListing.Contains(aFileName.Replace('\\', '/')))
                        {
                            dirListing.Add(aFileName.Replace('\\', '/'));
                        }
                    }
                }
            }
            reader.Close();
            hashFS.Close();

        }

        void TreatHashFile(string hashFileName)
        {
            FileStream hashFS = new FileStream(hashFileName, FileMode.Open);
            StreamReader reader = new StreamReader(hashFS);
            hasher = new WarhammerHash1();

            string line;

            while ((line = reader.ReadLine()) != null)
            {
                string aFileName = (line.Split('|'))[0];
                TreatString(aFileName);
                if (aFileName.IndexOf('.') >= 0)
                {
                    TreatString(aFileName);
                    if (aFileName.Replace('\\', '/').IndexOf('/') >= 0)
                    {
                        string dir = aFileName.Replace('\\', '/').Substring(0, aFileName.Replace('\\', '/').LastIndexOf('/'));
                        if (!dirListing.Contains(dir))
                        {
                            dirListing.Add(dir);
                        }
                    }
                }
                else
                {
                    if (!dirListing.Contains(aFileName.Replace('\\', '/')))
                    {
                        dirListing.Add(aFileName.Replace('\\', '/'));
                    }
                }
            }

            reader.Close();
            hashFS.Close();
        }

        void TreatString(string name)
        {
            string modFileName = name;
            uint seed = 0xDEADBEEF, sh, ph;

            for (int j = 0; j < 5; j++)
            {
                if ((j & 1) == 0)
                {
                    modFileName = name.Replace('\\', '/');
                }
                if ((j & 1) == 1)
                {
                    modFileName = name.Replace('/', '\\');
                }
                if ((j & 2) == 2)
                {
                    modFileName = name.ToLower();
                }
                if ((j & 4) == 4)
                {
                    modFileName = name;
                }

                hasher.Hash(modFileName, seed);
                sh = hasher.sh;
                ph = hasher.ph;

                SecondaryHashEntry se = new SecondaryHashEntry(sh, modFileName);
                PrimaryHashEntry pe = null;

                for (int i = 0; i < hashList.Count; i++)
                {
                    if (hashList.ContainsKey(ph))
                    {
                        pe = hashList[ph];
                        break;
                    }
                }

                if (pe == null)
                {
                    pe = new PrimaryHashEntry(ph, se);
                    hashList.Add(ph, pe);
                }
                else
                {
                    pe.AddSE(se);
                }
            }
        }

        public string SearchHashList(uint ph, uint sh)
        {
            string ffn = "";
            if (hashList.ContainsKey(ph))
            {
                for (int i = 0; i < hashList[ph].shList.Count; i++)
                {
                    if (hashList[ph].shList[i].sh == sh)
                    {
                        ffn = hashList[ph].shList[i].filename;
                        break;
                    }
                }
            }
            return ffn;
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
