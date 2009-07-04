/******************************************************************************
 * This file contains methods for the extraction process of files in a 
 * MYP Archive
 * 
 * 
 * Chryso
 *****************************************************************************/

using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Diagnostics;


namespace MYPHandler
{
    public partial class MYPHandler
    {
        #region Events
        /// <summary>
        /// Event variable for treating extraction events
        /// </summary>
        public event del_FileEventHandler event_Extraction;
        private void TriggerExtractionEvent(MYPFileEventArgs e)
        {
            if (event_Extraction != null)
            {
                event_Extraction(this, e);
            }
        }
        #endregion

        #region Attributes
        long error_ExtractionNumber = 0; //The number of extraction errors
        long numExtractedFiles = 0; //The number of file extracted
        #endregion

        #region Properties
        public long Error_ExtractionNumber { get { return error_ExtractionNumber; } } //To obtain the number of extraction error
        #endregion

        #region Read & Extract File From Archive
        /// <summary>
        /// Extracts all file from the myp archive
        /// </summary>
        public void ExtractAll()
        {
            ExtractFileList(archiveFileList);
        }

        bool multithreadExtraction = true;
        int numOfFileInExtractionList = 0;
        /// <summary>
        /// Extracts all file from the myp archive
        /// </summary>
        public void ExtractFileList(object obj)
        {
            boList = new BufferObjectList();
            numExtractedFiles = 0;
            List<FileInArchive> fileList = (List<FileInArchive>)obj;
            numOfFileInExtractionList = fileList.Count; //needed since the events are launched in the save buffer now

            //If we are using more than one disk, then we multi thread
            //Otherwise having 2 threads doing the reading and the writting is bad
            if (mypPath[0] == extractionPath[0])
            {
                multithreadExtraction = false;
            }
            else
            {
                multithreadExtraction = true;
            }

            if (multithreadExtraction)
            {
                boList.Active = true;
                Thread t_BOwriter = new Thread(new ThreadStart(ThreadWrite));
                t_BOwriter.Start();
                //Multi threading the writes is useless even when we have lots of small files.
                //Thread t_BOwriter2 = new Thread(new ThreadStart(ThreadWrite));
                //t_BOwriter2.Start();
            }

            for (int i = 0; i < fileList.Count; i++)
            {
                ExtractFile(fileList[i]);
            }

            if (!multithreadExtraction)
            {
                TriggerExtractionEvent(new MYPFileEventArgs(Event_ExtractionType.ExtractionFinished, fileList.Count - error_ExtractionNumber));
            }
            else
            {
                boList.Active = false;
            }
        }

        protected PerformanceCounter ramCounter = new PerformanceCounter("Process", "Private Bytes", Process.GetCurrentProcess().ProcessName);

        float getUsedRAM()
        {
            return ramCounter.NextValue();
        }

        int garbageRuns = 0;
        long archfilesBufferMem;
        long oldWorkingSet, newWorkingSet;
        float usedRam, oldUsedRam;

        System.Diagnostics.Process curProcess = System.Diagnostics.Process.GetCurrentProcess();

        /// <summary>
        /// Extracts input file from the myp archive
        /// (this is temporary, it will soon be hidden so that the input parameter is a string)
        /// </summary>
        /// <param name="archFile">file to extract</param>
        public void ExtractFile(FileInArchive archFile)
        {
            error_ExtractionNumber = 0;

            #region MultiThreading Stuff
            if (multithreadExtraction)
            {
                //Some stuff to free the memory
                //This is in case we explode the memory
                usedRam = getUsedRAM();
                if (usedRam > programMemory)
                {
                    oldUsedRam = usedRam;
                    garbageRuns++;
                    //We empty the file list until we get under 1000
                    //Redundant with the check done when adding a file
                    while (boList.Count > 100)
                    {
                        Thread.Sleep(1000);
                    }
                    GC.Collect();
                    usedRam = getUsedRAM();
                }
            }
            #endregion

            archFile.data = new byte[archFile.descriptor.compressedSize];
            archiveStream.Seek((long)(archFile.descriptor.startingPosition + archFile.descriptor.fileHeaderSize), SeekOrigin.Begin);

            for (int i = 0; i < archFile.data.Length; i++)
            {
                archFile.data[i] = (byte)archiveStream.ReadByte();
            }

            TreatExtractedFile(archFile);

            archFile.data = null; //nullify it because it has either been treated or copied
        }

        /// <summary>
        /// Treats the data extracted from the myp archive
        /// Uncompress the data if said data was compressed
        /// </summary>
        /// <param name="archFile"></param>
        private void TreatExtractedFile(FileInArchive archFile)
        {
            try
            {
                if (archFile.descriptor.compressionMethod == 1) //ZLib compression
                {
                    try
                    {
                        //Create the output_buffer, useless to create it if no compression
                        byte[] output_buffer = new byte[archFile.descriptor.uncompressedSize];

                        ICSharpCode.SharpZipLib.Zip.Compression.Inflater inf = new ICSharpCode.SharpZipLib.Zip.Compression.Inflater();
                        inf.SetInput(archFile.data);
                        inf.Inflate(output_buffer);

                        if (!multithreadExtraction)
                        {
                            //Treat directly the write
                            SaveBufferToFile(output_buffer, archFile.descriptor.foundFileName, archFile.descriptor.filename, archFile.descriptor.extension);
                        }
                        else
                        {
                            boList.AddBufferItemToQueue(output_buffer, archFile.descriptor.foundFileName, archFile.descriptor.filename, archFile.descriptor.extension);
                        }
                        //Clear the buffer (useless in CSharp)
                        output_buffer = null;
                    }
                    catch (Exception e)
                    {
                        TriggerExtractionEvent(new MYPFileEventArgs(Event_ExtractionType.FileExtractionError, error_ExtractionNumber++));
                    }
                }
                else if (archFile.descriptor.compressionMethod == 0) //No compression
                {
                    if (!multithreadExtraction)
                    {
                        //Treat directly the write
                        SaveBufferToFile(archFile.data, archFile.descriptor.foundFileName, archFile.descriptor.filename, archFile.descriptor.extension);
                    }
                    else
                    {
                        boList.AddBufferItemToQueue(archFile.data, archFile.descriptor.foundFileName, archFile.descriptor.filename, archFile.descriptor.extension);
                    }
                }
                else
                {
                    TriggerExtractionEvent(new MYPFileEventArgs(Event_ExtractionType.UnknownCompressionMethod, error_ExtractionNumber++));
                }
            }
            catch (Exception e)
            {
                TriggerExtractionEvent(new MYPFileEventArgs(Event_ExtractionType.UnknownError, error_ExtractionNumber++));
            }
        }

        /// <summary>
        /// Save extracted and treated data to a file
        /// </summary>
        /// <param name="buffer">the data</param>
        /// <param name="trueFileName">Is the filename correct?</param>
        /// <param name="filename">filename</param>
        /// <param name="ext">extension</param>
        private void SaveBufferToFile(byte[] buffer, bool trueFileName, string filename, string ext)
        {
            string test = "";

            // Well this should not really be here but more around extension guession function.
            if (buffer.Length > 100 && !trueFileName)
            {
                for (long i = buffer.Length - 18; i < buffer.Length; i++)
                {
                    test += Convert.ToChar(buffer[i]).ToString();
                }
                if (ext == "txt")
                {
                    if (test.IndexOf("TRUEVISION-XFILE") >= 0)
                    {
                        ext = "tga";
                    }
                }
            }

            string extraction_filename = "";
            string lPath = mypPath;
            // so that we have a default path and an extraction path.
            if (extractionPath != "") lPath = extractionPath;

            if (!trueFileName)
            {

                if (!Directory.Exists(lPath + "\\" + currentMypFileName)) Directory.CreateDirectory(lPath + "\\" + currentMypFileName);
                if (!Directory.Exists(lPath + "\\" + currentMypFileName + "\\" + ext)) Directory.CreateDirectory(lPath + "\\" + currentMypFileName + "\\" + ext);

                extraction_filename = lPath + "\\" + currentMypFileName + "\\" + ext + "\\" + filename + "." + ext;
            }
            else
            {
                filename = filename.Replace('\\', '/');
                string[] folders = filename.Split('/');

                if (folders.Length > 1)
                {
                    string tmpPath = lPath + '/' + folders[0];
                    if (!Directory.Exists(tmpPath)) Directory.CreateDirectory(tmpPath);

                    for (int i = 1; i < folders.Length - 1; i++)
                    {
                        tmpPath += '/' + folders[i];
                        if (!Directory.Exists(tmpPath)) Directory.CreateDirectory(tmpPath);
                    }
                }
                extraction_filename = lPath + '/' + filename;
            }

            if (File.Exists(extraction_filename)) File.Delete(extraction_filename);

            FileStream outputFS = new FileStream(extraction_filename, FileMode.Create);
            outputFS.Write(buffer, 0, (int)buffer.Length);
            outputFS.Close();
            buffer = null;

            TriggerExtractionEvent(new MYPFileEventArgs(Event_ExtractionType.FileExtracted, numExtractedFiles++));
        }

        BufferObjectList boList;
        private void ThreadWrite()
        {
            List<BufferObject> bol;
            //BufferObject bo;
            while (boList != null && boList.Active == true)
            {
                //To reduce locks we get a list of files
                bol = boList.RemoveBufferItemListFromQueue();
                for (int i = 0; i < bol.Count; i++)
                {
                    SaveBufferToFile(bol[i].buffer, bol[i].trueFileName, bol[i].filename, bol[i].ext);
                }
                bol.Clear();

                //Full throttle test code, we handle stuff as fast as possible
                //bo = boList.RemoveBufferItemFromQueue();
                //if (bo != null)
                //{
                //    SaveBufferToFile(bo.buffer, bo.trueFileName, bo.filename, bo.ext);
                //}
                //else
                //{
                //    Thread.Sleep(10);
                //}

                //we clean the memory after the writes
                if (boList != null && boList.Collect)
                {
                    boList.RunCollect();
                }
            }
            TriggerExtractionEvent(new MYPFileEventArgs(Event_ExtractionType.ExtractionFinished, numOfFileInExtractionList - error_ExtractionNumber));

            bol = null;
            if (boList != null)
            {
                boList.Clear();
            }

            GC.Collect();            // final GC collection to clean up everything
        }

        #endregion
    }

    public class BufferObjectList
    {
        List<BufferObject> bufferObjectList = new List<BufferObject>();
        object lock_bufferobject = new object();
        long buffersize = 0;
        int smallbuffersize = maxTmpListSize;
        int bigbuffersize = 2500;
        bool peak = false;
        bool collect = false;

        bool active = false;
        public bool Active
        {
            get { return active || bufferObjectList.Count > 0; }
            set { active = value; }
        }
        public int Count { get { return bufferObjectList.Count; } }
        public bool Peak { get { return peak; } }
        public int BigBufferSize { get { return bigbuffersize; } }
        public int SmallBufferSize { get { return smallbuffersize; } }
        public bool Collect { get { return collect; } }

        public BufferObjectList()
        {

        }

        public void AddBufferItemToQueue(byte[] buffer, bool trueFileName, string filename, string ext)
        {
            while (buffersize > 500000000 && bufferObjectList.Count > 2
                || peak)
            {
                Thread.Sleep(1000);
            }

            lock (lock_bufferobject)
            {
                bufferObjectList.Add(new BufferObject(buffer, trueFileName, filename, ext));
                buffersize += buffer.Length;
            }

            if (bufferObjectList.Count > bigbuffersize)
            {
                peak = true;
            }
        }

        public static int maxTmpListSize = 100;
        public List<BufferObject> RemoveBufferItemListFromQueue()
        {
            List<BufferObject> tmpList = new List<BufferObject>();
            // Not a while loop just so that write gaps are actually small.
            if (bufferObjectList.Count <= maxTmpListSize && active)
            {
                Thread.Sleep(100);
            }

            lock (lock_bufferobject)
            {
                //bufferObjectList.CopyTo(0, tmpList, 0, Math.Min(maxTmpListSize, bufferObjectList.Count));
                for (int i = 0; i < maxTmpListSize && i < bufferObjectList.Count; i++)
                {
                    tmpList.Add(bufferObjectList[i]);
                    buffersize -= bufferObjectList[i].buffer.Length;
                    //bufferObjectList.RemoveAt(0);
                }
                bufferObjectList.RemoveRange(0, tmpList.Count);

                if (peak && bufferObjectList.Count < smallbuffersize)
                {
                    peak = false;
                    collect = true;
                }
            }
            return tmpList;
        }


        public BufferObject RemoveBufferItemFromQueue()
        {
            BufferObject tmpBO = null;
            // Not a while loop just so that write gaps are actually small.
            // And just enough sleep to not be processor hogging
            if (bufferObjectList.Count < 1 && active)
            {
                Thread.Sleep(10);
            }

            lock (lock_bufferobject)
            {
                if (bufferObjectList.Count > 0)
                {
                    tmpBO = bufferObjectList[0];
                    buffersize -= tmpBO.buffer.Length;
                    bufferObjectList.RemoveAt(0);
                }
                if (peak && bufferObjectList.Count < smallbuffersize)
                {
                    collect = true;
                    peak = false;
                }
            }
            return tmpBO;
        }

        public void RunCollect()
        {
            lock (lock_bufferobject)
            {
                collect = false;
                GC.Collect();
            }
        }

        public void Clear()
        {
            bufferObjectList.Clear();
        }
    }

    public class BufferObject
    {
        public byte[] buffer;
        public bool trueFileName;
        public string filename;
        public string ext;

        public BufferObject(byte[] buffer, bool trueFileName, string filename, string ext)
        {
            this.buffer = new byte[buffer.Length];
            buffer.CopyTo(this.buffer, 0); //just to make sure we are working on a copy

            this.trueFileName = trueFileName;
            this.filename = filename;
            this.ext = ext;
        }
    }
}
