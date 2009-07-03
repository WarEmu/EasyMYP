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

            if (multithreadExtraction)
            {
                boList.Active = true;
                Thread t_BOwriter = new Thread(new ThreadStart(ThreadWrite));
                t_BOwriter.Start();
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

            if (multithreadExtraction)
            {
                archfilesBufferMem = (archFile.descriptor.uncompressedSize + archFile.descriptor.compressedSize) * 2;

                //Some stuff to free the memory
                usedRam = getUsedRAM();
                if (usedRam > programMemory)
                {
                    oldUsedRam = usedRam;
                    garbageRuns++;
                    //We empty the file list until we get under 1000
                    //Redundant with the check done when adding a file
                    while (boList.Count > 1000)
                    {
                        Thread.Sleep(1000);
                    }
                    GC.Collect();
                    usedRam = getUsedRAM();
                }
            }

            archFile.data = new byte[archFile.descriptor.compressedSize];
            archiveStream.Seek((long)(archFile.descriptor.startingPosition + archFile.descriptor.fileHeaderSize), SeekOrigin.Begin);

            for (int i = 0; i < archFile.data.Length; i++)
            {
                archFile.data[i] = (byte)archiveStream.ReadByte();
            }

            TreatExtractedFile(archFile);

            archFile.data = null;
        }

        /// <summary>
        /// Treats the data extracted from the myp archive
        /// Uncompress the data if said data was compressed
        /// </summary>
        /// <param name="archFile"></param>
        private void TreatExtractedFile(FileInArchive archFile)
        {
            MemoryStream inputMS = new MemoryStream(archFile.data, 0, archFile.data.Length);
            MemoryStream outputMS = new MemoryStream();

            byte[] output_buffer = new byte[archFile.descriptor.uncompressedSize];

            try
            {
                if (archFile.descriptor.compressionMethod == 1) //ZLib compression
                {
                    try
                    {
                        ICSharpCode.SharpZipLib.Zip.Compression.Inflater inf = new ICSharpCode.SharpZipLib.Zip.Compression.Inflater();
                        inf.SetInput(archFile.data);
                        inf.Inflate(output_buffer);

                        if (multithreadExtraction)
                        {
                            boList.AddBufferItemToQueue(output_buffer, archFile.descriptor.foundFileName, archFile.descriptor.filename, archFile.descriptor.extension);
                        }
                        else
                        {
                            SaveBufferToFile(output_buffer, archFile.descriptor.foundFileName, archFile.descriptor.filename, archFile.descriptor.extension);
                        }
                        //TriggerExtractionEvent(new MYPFileEventArgs(Event_ExtractionType.FileExtracted, numExtractedFiles++));
                    }
                    catch (Exception e)
                    {
                        TriggerExtractionEvent(new MYPFileEventArgs(Event_ExtractionType.FileExtractionError, error_ExtractionNumber++));
                    }
                }
                else if (archFile.descriptor.compressionMethod == 0) //No compression
                {
                    if (multithreadExtraction)
                    {
                        boList.AddBufferItemToQueue(output_buffer, archFile.descriptor.foundFileName, archFile.descriptor.filename, archFile.descriptor.extension);
                    }
                    else
                    {
                        SaveBufferToFile(output_buffer, archFile.descriptor.foundFileName, archFile.descriptor.filename, archFile.descriptor.extension);
                    }
                    //TriggerExtractionEvent(new MYPFileEventArgs(Event_ExtractionType.FileExtracted, numExtractedFiles++));
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

            output_buffer = null;
            inputMS.Flush();
            outputMS.Flush();
            inputMS.Close();
            outputMS.Close();
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
            TriggerExtractionEvent(new MYPFileEventArgs(Event_ExtractionType.FileExtracted, numExtractedFiles++));

            //byte[] outbuffer = buffer;
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
        }

        private void ThreadWrite()
        {
            List<BufferObject> bol;
            while (boList.Active == true)
            {
                bol = boList.RemoveBufferItemFromQueue();
                for (int i = 0; i < bol.Count; i++)
                {
                    SaveBufferToFile(bol[i].buffer, bol[i].trueFileName, bol[i].filename, bol[i].ext);
                }
                if (bol.Count <= 100)
                {
                    Thread.Sleep(1000);
                }
            }
            TriggerExtractionEvent(new MYPFileEventArgs(Event_ExtractionType.ExtractionFinished, numOfFileInExtractionList - error_ExtractionNumber));
        }

        BufferObjectList boList;
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

        bool active = false;
        public bool Active
        {
            get { return active || bufferObjectList.Count > 0; }
            set { active = value; }
        }
        public int Count { get { return bufferObjectList.Count; } }

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

        List<BufferObject> tmpList = new List<BufferObject>();
        public static int maxTmpListSize = 666;
        public List<BufferObject> RemoveBufferItemFromQueue()
        {
            tmpList.Clear();
            lock (lock_bufferobject)
            {
                for (int i = 0; i < maxTmpListSize && i < bufferObjectList.Count; i++)
                {
                    tmpList.Add(bufferObjectList[0]);
                    buffersize -= bufferObjectList[0].buffer.Length;
                    bufferObjectList.RemoveAt(0);
                }

                if (peak && bufferObjectList.Count < smallbuffersize)
                {
                    peak = false;
                    //GC.Collect();
                }
            }
            return tmpList;
        }
    }

    public class BufferObject
    {
        public byte[] buffer;
        public bool trueFileName;
        public string filename;
        public string ext;

        private bool disposed = false;

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
