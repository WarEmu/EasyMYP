using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nsHashCreator;
using nsHashDictionary;
using nsHasherFunctions;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            //HashDictionary hashDic = new HashDictionary();
            //HashCreator hashCreator = new HashCreator(hashDic, Hasher.HasherType.WAR);

            //hashCreator.ParseFilenames(
            //    "D:/Development/GoogleCode/EasyMYP/HashTesterConsole/HashTesterConsole/bin/Debug/tor_filenames.txt");
            //hashCreator.Save();

            Hasher hTOR = new Hasher(Hasher.HasherType.TOR);
            Hasher hWAR = new Hasher(Hasher.HasherType.WAR);
            uint seed = 0xDEADBEEF;
            string sTST = "teststring/2345/somemore/andagain.ext";
            hTOR.Hash(sTST, seed);
            hWAR.Hash(sTST, seed);

            if (hTOR.sh == hWAR.sh)
            {
                if (hTOR.ph == hWAR.ph)
                {
                    seed = 0;
                }
            }

        }
    }
}
