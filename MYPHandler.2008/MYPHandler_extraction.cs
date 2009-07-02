/******************************************************************************
 * This file contains methods for the extraction process of files in a 
 * MYP Archive
 * 
 * 
 * Chryso
 *****************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;

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

        /// <summary>
        /// Extracts all file from the myp archive
        /// </summary>
        public void ExtractFileList(object obj)
        {
            numExtractedFiles = 0;
            List<FileInArchive> fileList = (List<FileInArchive>)obj;

            for (int i = 0; i < fileList.Count; i++)
            {
                ExtractFile(fileList[i]);
            }

            TriggerExtractionEvent(new MYPFileEventArgs(Event_ExtractionType.ExtractionFinished, fileList.Count - error_ExtractionNumber));
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

                        SaveBufferToFile(output_buffer, archFile.descriptor.foundFileName, archFile.descriptor.filename, archFile.descriptor.extension);
                        TriggerExtractionEvent(new MYPFileEventArgs(Event_ExtractionType.FileExtracted, numExtractedFiles++));
                    }
                    catch (Exception e)
                    {
                        TriggerExtractionEvent(new MYPFileEventArgs(Event_ExtractionType.FileExtractionError, error_ExtractionNumber++));
                    }
                }
                else if (archFile.descriptor.compressionMethod == 0) //No compression
                {
                    SaveBufferToFile(archFile.data, archFile.descriptor.foundFileName, archFile.descriptor.filename, archFile.descriptor.extension);
                    TriggerExtractionEvent(new MYPFileEventArgs(Event_ExtractionType.FileExtracted, numExtractedFiles++));
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
            byte[] outbuffer = buffer;
            string test = "";

            // Well this should not really be here but more around extension guession function.
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
        }
        #endregion
    }
}
