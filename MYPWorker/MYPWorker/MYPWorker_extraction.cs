/******************************************************************************
 * This file contains methods for the extraction process of files in a 
 * MYP Archive
 * 
 * 
 * Chryso
 *****************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.IO.Compression;
using WarhammerOnlineHash;
using WarhammerOnlineHashBuilder;

namespace MYPWorker
{
    public partial class MYPWorker
    {
        #region Events
        /// <summary>
        /// Event variable for treating extraction events
        /// </summary>
        public event del_FileEventHandler event_Extraction;
        private void OnExtractionEvent(MYPFileEventArgs e)
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
            numExtractedFiles = 0;

            for (int i = 0; i < archiveFileList.Count; i++)
            {
                ExtractFile(archiveFileList[i]);
            }

            OnExtractionEvent(new MYPFileEventArgs(Event_ExtractionType.ExtractionFinished, archiveFileList.Count - error_ExtractionNumber));
        }

        /// <summary>
        /// Extracts input file from the myp archive
        /// (this is temporary, it will soon be hidden so that the input parameter is a string)
        /// </summary>
        /// <param name="archFile">file to extract</param>
        public void ExtractFile(FileInArchive archFile)
        {
            error_ExtractionNumber = 0;
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

                        SaveBufferToFile(output_buffer, archFile.descriptor.startingPosition, true, archFile.descriptor.foundFileName, archFile.descriptor.filename, archFile.descriptor.extension);
                        OnExtractionEvent(new MYPFileEventArgs(Event_ExtractionType.FileExtracted, numExtractedFiles++));
                    }
                    catch (Exception e)
                    {
                        OnExtractionEvent(new MYPFileEventArgs(Event_ExtractionType.FileExtractionError, error_ExtractionNumber++));
                    }
                }
                else if (archFile.descriptor.compressionMethod == 0) //No compression
                {
                    SaveBufferToFile(archFile.data, archFile.descriptor.startingPosition, false, archFile.descriptor.foundFileName, archFile.descriptor.filename, archFile.descriptor.extension);
                    OnExtractionEvent(new MYPFileEventArgs(Event_ExtractionType.FileExtracted, numExtractedFiles++));
                }
                else
                {
                    OnExtractionEvent(new MYPFileEventArgs(Event_ExtractionType.UnknownCompressionMethod, error_ExtractionNumber++));
                }
            }
            catch (Exception e)
            {
                OnExtractionEvent(new MYPFileEventArgs(Event_ExtractionType.UnknownError, error_ExtractionNumber++));
            }
        }

        /// <summary>
        /// Save extracted and treated data to a file
        /// </summary>
        /// <param name="buffer">the data</param>
        /// <param name="offset">an offset that is not used anymore :)</param>
        /// <param name="cp">compression info not used anymore :)</param>
        /// <param name="trueFileName">Is the filename correct?</param>
        /// <param name="filename">filename</param>
        /// <param name="ext">extension</param>
        private void SaveBufferToFile(byte[] buffer, long offset, bool cp, bool trueFileName, string filename, string ext)
        {
            byte[] outbuffer = buffer;
            string test = "";

            if (buffer.Length > 100 && !trueFileName)
            {
                for (long i = buffer.Length - 18; i < buffer.Length; i++)
                {
                    test += Convert.ToChar(outbuffer[i]).ToString();
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
            string lPath = path;
            // so that we have a default path and an extraction path.
            if (extractionPath != "") lPath = extractionPath;

            if (!trueFileName)
            {
                if (!Directory.Exists(lPath + "\\" + currentFileName)) Directory.CreateDirectory(path + "\\" + currentFileName);
                if (!Directory.Exists(lPath + "\\" + currentFileName + "\\" + ext)) Directory.CreateDirectory(path + "\\" + currentFileName + "\\" + ext);

                extraction_filename = lPath + "\\" + currentFileName + "\\" + ext + "\\" + filename + "." + ext;
            }
            else
            {
                filename = filename.Replace('\\', '/');
                string[] folders = filename.Split('/');
                string tmpPath = extractionPath + '/' + folders[0];
                if (!Directory.Exists(tmpPath)) Directory.CreateDirectory(tmpPath);

                for (int i = 1; i < folders.Length - 1; i++)
                {
                    tmpPath += '/' + folders[i];
                    if (!Directory.Exists(tmpPath)) Directory.CreateDirectory(tmpPath);
                }

                extraction_filename = extractionPath + '/' + filename;
            }

            if (File.Exists(extraction_filename)) File.Delete(extraction_filename);

            FileStream outputFS = new FileStream(extraction_filename, FileMode.Create);
            outputFS.Write(buffer, 0, (int)buffer.Length);
            outputFS.Close();
        }
        #endregion
    }
}
