using System.Collections.Generic;
using System.Windows.Forms;
using nsHashDictionary;
using System;

namespace EasyMYP
{
    public partial class DictionnaryStatistics : Form
    {
        public DictionnaryStatistics()
        {
            InitializeComponent();
        }
        public DictionnaryStatistics(SortedList<string, SortedList<long, HashData>> hashlist)
        {
            SortedList<long, HashData> archiveKVP;
            InitializeComponent();
            long totalNumberOfEntries = hashlist.Count, totalFileNames = 0, seenFileNames = 0, seenNumberofEntries = 0;
            for (int i = 0; i < hashlist.Count; i++)
            {
                archiveKVP = hashlist.Values[i];
                foreach (KeyValuePair<long, HashData> kvp in archiveKVP)
                {
                    if (kvp.Value.filename.Length != 0)
                    {
                        totalFileNames++;
                        if (kvp.Value.crc != 0)
                            seenFileNames++;
                    }
                    if (kvp.Value.crc != 0)
                        seenNumberofEntries++;
                }
            }

            tneLabel.Text = totalNumberOfEntries.ToString();
            sneLabel.Text = seenNumberofEntries.ToString();
            tnfLabel.Text = totalFileNames.ToString();
            snfLabel.Text = seenFileNames.ToString();
            if (hashlist.Count != 0)
                tcsLabel.Text = ((float)((float)(totalFileNames * 100) / (float)hashlist.Count)).ToString("F2");
            if (seenNumberofEntries != 0)
                scsLabel.Text = ((float)((float)(seenFileNames * 100) / (float)seenNumberofEntries)).ToString("F2");
        }
    }
}
