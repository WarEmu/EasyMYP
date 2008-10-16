/******************************************************************************
 * This file contains all method necessary to read file header, search
 * and replace files in a myp archive
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
    #region MYP File Format Helpers
    public enum FileInArchiveState
    {
        NEW,
        MODIFIED,
        UNCHANGED
    }
    /// <summary>
    /// An object that represents a file in the archive
    /// </summary>
    public class FileInArchive
    {
        public FileInArchiveDescriptor descriptor = new FileInArchiveDescriptor();
        public byte[] metadata;
        public byte[] data;
        public byte[] data_start_200 = new byte[200];
        private FileInArchiveState state = FileInArchiveState.UNCHANGED;

        //public FileTableEntryDescriptor Descriptor { get { return descriptor; } }

        #region Properties
        public long Offset { get { return descriptor.startingPosition + descriptor.startingPosition65536; } }
        public long Size { get { return descriptor.uncompressedSize; } }
        public long CompressedSize { get { return descriptor.compressedSize; } }
        public int CompressionMethod { get { return descriptor.compressionMethod; } }
        public string Filename { get { return descriptor.filename; } }
        public string Extension { get { return descriptor.extension; } }
        public FileInArchiveState State { get { return state; } set { state = value; } }
        #endregion
    }

    /// <summary>
    /// An object that enables to create sub list of archive files depending on the file table
    /// it was found in
    /// (not used)
    /// </summary>
    public class FileTable
    {
        public long offset;
        public long num_of_files;

        public List<FileInArchive> files = new List<FileInArchive>();
    }

    /// <summary>
    /// This class describes an archive file
    /// Contains information like size, offset, fileheader size and stuff like that
    /// </summary>
    public class FileInArchiveDescriptor
    {
        public static int fileDescriptorSize = 34;

        #region Variables
        public long fileTableEntryPosition;

        public long startingPosition;
        public long startingPosition65536;
        public long fileHeaderSize;
        public long compressedSize;
        public long uncompressedSize;
        public int compressionMethod;
        public int crc = 0;
        public byte[] file_hash = new byte[8];
        public uint ph, sh;

        public string filename = "";
        public bool foundFileName = false;

        public string strUTF8;
        public string strUTF16;

        public bool isCompressed;

        public string extension = "";
        #endregion

        public FileInArchiveDescriptor() { }

        public FileInArchiveDescriptor(byte[] buffer)
        {
            startingPosition = convertLittleEndianBufferToInt(buffer, 0); //Last 32 bits
            startingPosition65536 = convertLittleEndianBufferToInt(buffer, 4); //Fisrt 32 bits
            startingPosition += startingPosition65536 * 256 * 256 * 256 * 256; //Real starting position

            fileHeaderSize = convertLittleEndianBufferToInt(buffer, 8);

            compressedSize = convertLittleEndianBufferToInt(buffer, 12);
            uncompressedSize = convertLittleEndianBufferToInt(buffer, 16);

            for (int i = 0; i < 8; i++)
            {
                file_hash[i] = buffer[20 + i];
            }

            sh = (uint)convertLittleEndianBufferToInt(buffer, 20);
            ph = (uint)convertLittleEndianBufferToInt(buffer, 24);

            crc = BitConverter.ToInt32(buffer, 28);

            //for (int i = 0; i < 4; i++)
            //{
            //    crc = (int)buffer[28 + i]*(int)Math.Pow(8,i);
            //}

            for (int i = 0; i < 4; i++)
            {
                filename += string.Format("{0:X8}", crc);
            }
            filename += "_";
            for (int i = 0; i < 8; i++)
            {
                filename += string.Format("{0:X2}", file_hash[i]);
            }

            compressionMethod = (int)buffer[32];
            isCompressed = (compressionMethod == 0) ? false : true;
        }

        public static long convertLittleEndianBufferToInt(byte[] intBuffer, long offset)
        {
            return ((long)(uint)intBuffer[offset + 0])
                + (long)(uint)(intBuffer[offset + 1]) * 256
                + (long)(uint)(intBuffer[offset + 2]) * 256 * 256
                + (long)(uint)(intBuffer[offset + 3]) * 256 * 256 * 256;
        }
    }
    #endregion

    public partial class MYPWorker
    {
        #region Event
        /// <summary>
        /// Event variable for treating file table events
        /// </summary>
        public event del_FileTableEventHandler event_FileTable;

        private void OnFileTableEvent(MYPFileTableEventArgs e)
        {
            if (event_FileTable != null)
            {
                event_FileTable(this, e);
            }
        }
        #endregion

        #region Attributes
        Hasher hasherBuilder; //the dictionnary
        public FileStream archiveStream; //the stream to read data from
        string currentFileName; //the current filename of the file being read
        string path; //the path of the filename
        public List<FileInArchive> archiveFileList = new List<FileInArchive>(); //contains all the files information, but not the data because of memory limitation on a 32Bits system
        public List<FileInArchive> archiveNewFileList = new List<FileInArchive>();
        public List<FileInArchive> archiveModifiedFileList = new List<FileInArchive>();
        long tableStart; //start of the file table entry
        string pattern = "*";
        long unCompressedSize = 0; //uncompressed size
        long numberOfFileNamesFound = 0; //number of files with a name found
        long totalNumberOfFiles = 0; //total number of files in the archive
        long numberOfFilesFound = 0; //total number of files found in the archive
        long error_FileEntryNumber = 0; //number of errors in the file table entry
        string extractionPath = "";
        #endregion

        #region Properties
        public string ExtractionPath { get { return extractionPath; } set { extractionPath = value; } }

        public string Pattern { get { return pattern; } set { pattern = value; } }
        /// <summary>
        /// Uncompressed size of the archive
        /// </summary>
        public long UnCompressedSize { get { return unCompressedSize; } }
        /// <summary>
        /// Number of files with a known filename
        /// </summary>
        public long NumberOfFileNamesFound { get { return numberOfFileNamesFound; } }
        /// <summary>
        /// Alleged total number of files in the archive
        /// </summary>
        public long TotalNumberOfFiles { get { return totalNumberOfFiles; } }
        /// <summary>
        /// Total number of files in the archive
        /// </summary>
        public long NumberOfFilesFound { get { return numberOfFilesFound; } }
        /// <summary>
        /// Number of file table entry that gave out an error
        /// </summary>
        public long Error_FileEntryNumber { get { return error_FileEntryNumber; } }
        #endregion

        #region Constructor -- Initialization
        /// <summary>
        /// Creates a new myp worker
        /// </summary>
        /// <param name="filename">the name of the file to work with</param>
        /// <param name="event_FileTable">method to treat events return when reading the file table</param>
        /// <param name="event_Extraction">method to treat events return when extracting files</param>
        /// <param name="hasher">the dictionnary</param>
        public MYPWorker(string filename
            , del_FileTableEventHandler event_FileTable
            , del_FileEventHandler event_Extraction
            , Hasher hasher)
        {
            this.hasherBuilder = hasher;
            this.event_Extraction += event_Extraction;
            this.event_FileTable += event_FileTable;

            //parse the filename to get the path
            this.currentFileName = filename.Substring(filename.LastIndexOf('\\') + 1, filename.Length - filename.LastIndexOf('\\') - 1);
            this.currentFileName = currentFileName.Split('.')[0];
            if (filename.LastIndexOf('\\') >= 0)
            {
                this.path = filename.Substring(0, filename.LastIndexOf('\\'));
            }
            else
            {
                this.path = "";
            }

            //Initialize some data
            pattern = "*";
            unCompressedSize = 0;
            numberOfFileNamesFound = 0;
            totalNumberOfFiles = 0;
            numberOfFilesFound = 0;
            error_FileEntryNumber = 0;
            error_ExtractionNumber = 0;

            //open the archive file
            archiveStream = new FileStream(filename, FileMode.Open, FileAccess.Read);

            //read the position of the starting file table
            archiveStream.Seek(0x0C, SeekOrigin.Begin);
            byte[] buffer = new byte[8];
            archiveStream.Read(buffer, 0, buffer.Length);
            tableStart = FileInArchiveDescriptor.convertLittleEndianBufferToInt(buffer, 0);
            tableStart += FileInArchiveDescriptor.convertLittleEndianBufferToInt(buffer, 4) * 256 * 256 * 256 * 256;
            GetFileNumber();

            //Worker is initialized
        }

        /// <summary>
        /// Set the alleged number of files in the archive
        /// </summary>
        void GetFileNumber()
        {
            archiveStream.Seek(24, SeekOrigin.Begin);
            byte[] buffer = new byte[4];
            archiveStream.Read(buffer, 0, buffer.Length);

            totalNumberOfFiles = FileInArchiveDescriptor.convertLittleEndianBufferToInt(buffer, 0);
        }
        #endregion

        #region Event & Error Stuff
        /// <summary>
        /// Add a file to the list of files
        /// Update the uncompressed size
        /// Raise corresponding event
        /// </summary>
        /// <param name="archFile">file to add to the list</param>
        private void NewFile(FileInArchive archFile)
        {
            archiveFileList.Add(archFile);
            unCompressedSize += archFile.descriptor.uncompressedSize;

            OnFileTableEvent(new MYPFileTableEventArgs(Event_FileTableType.NewFile, archFile));
        }

        /// <summary>
        /// Updates the number of errors in the file table entry
        /// Raise the according event
        /// </summary>
        /// <param name="archFile">File that gave the error</param>
        private void Error_FileTableEntry(FileInArchive archFile)
        {
            error_FileEntryNumber++;
            OnFileTableEvent(new MYPFileTableEventArgs(Event_FileTableType.FileError, archFile));

        }
        #endregion

        #region File Table List Files

        /// <summary>
        /// Parse the source file and reads the file table entries
        /// Raises according event depending on result
        /// (lots of deprecated and debugg stuff)
        /// USES REDBLACK TREE (me thinks)
        /// </summary>
        public void ListFiles()
        {
            error_FileEntryNumber = 0;
            error_ExtractionNumber = 0;
            unCompressedSize = 0;
            numberOfFileNamesFound = 0;
            //totalNumberOfFiles = 0;
            numberOfFilesFound = 0;
            //Init
            long currentReadingPosition;
            long numberOfFileInTable = 0;
            long endOfTableAddress;
            byte[] buffer;
            FileInArchive myArchFile;

            while (tableStart != 0)
            {
                buffer = new byte[12];
                archiveStream.Seek((long)(uint)tableStart, SeekOrigin.Begin);
                archiveStream.Read(buffer, 0, buffer.Length);

                numberOfFileInTable = FileInArchiveDescriptor.convertLittleEndianBufferToInt(buffer, 0); //get number of files
                currentReadingPosition = tableStart + 12;
                endOfTableAddress = tableStart + 12 + FileInArchiveDescriptor.fileDescriptorSize * numberOfFileInTable; // calculates the end address

                tableStart = FileInArchiveDescriptor.convertLittleEndianBufferToInt(buffer, 4); //find the next filetable
                tableStart += FileInArchiveDescriptor.convertLittleEndianBufferToInt(buffer, 8) * 256 * 256 * 256 * 256; //mostly 0

                #region File Table Read
                while (currentReadingPosition < endOfTableAddress)
                {
                    buffer = new byte[FileInArchiveDescriptor.fileDescriptorSize];
                    archiveStream.Seek((long)currentReadingPosition, SeekOrigin.Begin);
                    archiveStream.Read(buffer, 0, FileInArchiveDescriptor.fileDescriptorSize);

                    myArchFile = new FileInArchive();
                    myArchFile.descriptor = new FileInArchiveDescriptor(buffer);

                    myArchFile.descriptor.fileTableEntryPosition = currentReadingPosition;
                    //                            ffn = hasherBuilder.SearchHashTree(myArchFile.descriptor.ph, myArchFile.descriptor.sh);

                    OnFileTableEvent(new MYPFileTableEventArgs(Event_FileTableType.NewFile, myArchFile));

                    currentReadingPosition += FileInArchiveDescriptor.fileDescriptorSize;
                }
                #endregion
            }

        }

        #endregion

        #region File Table Entries

        /// <summary>
        /// Parse the source file and reads the file table entries
        /// Raises according event depending on result
        /// (lots of deprecated and debugg stuff)
        /// </summary>
        public void GetFileTable()
        {
            error_FileEntryNumber = 0;
            error_ExtractionNumber = 0;
            unCompressedSize = 0;
            numberOfFileNamesFound = 0;
            //totalNumberOfFiles = 0;
            numberOfFilesFound = 0;
            //Init
            long currentReadingPosition;
            long numberOfFileInTable = 0;
            long endOfTableAddress;
            byte[] buffer;
            FileInArchive myArchFile;

            while (tableStart != 0)
            {
                buffer = new byte[12];
                archiveStream.Seek((long)(uint)tableStart, SeekOrigin.Begin);
                archiveStream.Read(buffer, 0, buffer.Length);

                numberOfFileInTable = FileInArchiveDescriptor.convertLittleEndianBufferToInt(buffer, 0); //get number of files
                currentReadingPosition = tableStart + 12;
                endOfTableAddress = tableStart + 12 + FileInArchiveDescriptor.fileDescriptorSize * numberOfFileInTable; // calculates the end address

                tableStart = FileInArchiveDescriptor.convertLittleEndianBufferToInt(buffer, 4); //find the next filetable
                tableStart += FileInArchiveDescriptor.convertLittleEndianBufferToInt(buffer, 8) * 256 * 256 * 256 * 256; //mostly 0

                #region File Table Read
                while (currentReadingPosition < endOfTableAddress)
                {
                    buffer = new byte[FileInArchiveDescriptor.fileDescriptorSize];
                    archiveStream.Seek((long)currentReadingPosition, SeekOrigin.Begin);
                    archiveStream.Read(buffer, 0, FileInArchiveDescriptor.fileDescriptorSize);

                    myArchFile = new FileInArchive();
                    myArchFile.descriptor = new FileInArchiveDescriptor(buffer);

                    myArchFile.descriptor.fileTableEntryPosition = currentReadingPosition;

                    if (myArchFile.descriptor.startingPosition > 0
                        && myArchFile.descriptor.compressedSize > 0
                        && myArchFile.descriptor.uncompressedSize > 0 //If the compressed size is 0, then there is no file
                        )
                    {
                        //Search for the filename

                        HashData ffn = null;
                        if (hasherBuilder != null)
                        {
                            ffn = hasherBuilder.SearchHashList(myArchFile.descriptor.ph, myArchFile.descriptor.sh, myArchFile.descriptor.crc);
                            if (ffn != null && ffn.filename != "")
                            {
                                myArchFile.descriptor.foundFileName = true;
                                myArchFile.descriptor.filename = ffn.filename;
                                if (myArchFile.descriptor.crc != ffn.crc)
                                {
                                    myArchFile.State = FileInArchiveState.MODIFIED;
                                    hasherBuilder.UpdateHash(myArchFile.descriptor.ph, myArchFile.descriptor.sh, ffn.filename, myArchFile.descriptor.crc);
                                    archiveModifiedFileList.Add(myArchFile);
                                }
                                numberOfFileNamesFound++;
                                int dotpos = myArchFile.descriptor.filename.LastIndexOf('.');
                                myArchFile.descriptor.extension = myArchFile.descriptor.filename.Substring(dotpos + 1);
                            }
                            else
                            {
                                if (ffn == null)
                                {
                                    myArchFile.State = FileInArchiveState.NEW;
                                    hasherBuilder.AddHash(myArchFile.descriptor.ph, myArchFile.descriptor.sh);
                                    hasherBuilder.UpdateHash(myArchFile.descriptor.ph, myArchFile.descriptor.sh, "", myArchFile.descriptor.crc);
                                    archiveNewFileList.Add(myArchFile);
                                }
                                else if (myArchFile.descriptor.crc != ffn.crc)
                                {
                                    myArchFile.State = FileInArchiveState.MODIFIED;
                                    hasherBuilder.UpdateHash(myArchFile.descriptor.ph, myArchFile.descriptor.sh, ffn.filename, myArchFile.descriptor.crc);
                                    archiveModifiedFileList.Add(myArchFile);
                                }
                                //Retrieve header
                                myArchFile.metadata = new byte[myArchFile.descriptor.fileHeaderSize];

                                archiveStream.Seek((long)myArchFile.descriptor.startingPosition, SeekOrigin.Begin);

                                for (int i = 0; i < myArchFile.metadata.Length; i++)
                                {
                                    myArchFile.metadata[i] = (byte)archiveStream.ReadByte();
                                }

                                //Retrieve head of the data for extension definition purpose
                                if (myArchFile.descriptor.compressedSize < myArchFile.data_start_200.Length)
                                    myArchFile.data_start_200 = new byte[myArchFile.descriptor.compressedSize];

                                archiveStream.Seek((long)(myArchFile.descriptor.startingPosition + myArchFile.descriptor.fileHeaderSize), SeekOrigin.Begin);

                                for (int i = 0; i < myArchFile.data_start_200.Length; i++)
                                {
                                    myArchFile.data_start_200[i] = (byte)archiveStream.ReadByte();
                                }

                                try
                                {
                                    TreatHeader(myArchFile);
                                }
                                catch (Exception e)
                                {
                                    Error_FileTableEntry(myArchFile);
                                }
                            }

                            try
                            {
                                numberOfFilesFound++;
                                if (WildcardMatch(pattern, myArchFile.Filename))
                                    NewFile(myArchFile);
                                else
                                {
                                    OnFileTableEvent(new MYPFileTableEventArgs(Event_FileTableType.UpdateFile, myArchFile));
                                }
                            }
                            catch (Exception e)
                            {
                                Error_FileTableEntry(myArchFile);
                            }
                        }
                    }
                    else
                    {
                        //FileTableEntryError(myArchFile);
                    }
                    currentReadingPosition += FileInArchiveDescriptor.fileDescriptorSize;
                }
                #endregion
            }

            OnFileTableEvent(new MYPFileTableEventArgs(Event_FileTableType.Finished, null));
        }

        #endregion

        #region Header stuff
        private void CopyHeaderStream(System.IO.Stream input, System.IO.Stream output)
        {
            byte[] buffer = new byte[1];
            int len;

            while ((len = input.Read(buffer, 0, 1)) > 0)
            {
                output.Write(buffer, 0, (int)buffer.Length);
            }

            output.Flush();
        }

        /// <summary>
        /// Tries to identify the file extension
        /// Updates the file descriptor in response
        /// </summary>
        /// <param name="archFile">file to consider</param>
        private void TreatHeader(FileInArchive archFile)
        {
            MemoryStream outputMS = new MemoryStream();

            byte[] output_buffer = new byte[4096];

            if (archFile.descriptor.compressionMethod == 1)
            {
                try
                {
                    ICSharpCode.SharpZipLib.Zip.Compression.Inflater inf = new ICSharpCode.SharpZipLib.Zip.Compression.Inflater();
                    inf.SetInput(archFile.data_start_200);
                    inf.Inflate(output_buffer);
                }
                catch (Exception e)
                {
                    Error_FileTableEntry(archFile);
                }

                archFile.descriptor.extension = GetExtension(output_buffer);
            }
            else if (archFile.descriptor.compressionMethod == 0)
            {
                archFile.descriptor.extension = GetExtension(archFile.data_start_200);
            }
            else
            {
            }

        }

        /// <summary>
        /// Identify extension of file
        /// </summary>
        /// <param name="buffer">header of the file</param>
        /// <returns>supposed extension</returns>
        private string GetExtension(byte[] buffer)
        {
            byte[] outbuffer = buffer;

            string file = "";

            int commaNum = 0;

            for (int i = 0; i < outbuffer.Length && i < 10000; i++)
            {
                file += Convert.ToChar(outbuffer[i]).ToString();
                if (Convert.ToChar(outbuffer[i]).ToString() == ",") commaNum++;
            }

            string header = Convert.ToChar(outbuffer[0]).ToString()
                + Convert.ToChar(outbuffer[1]).ToString()
                + Convert.ToChar(outbuffer[2]).ToString()
                + Convert.ToChar(outbuffer[3]).ToString();

            string ext = "txt";

            if (outbuffer[0] == 0 && outbuffer[1] == 1 && outbuffer[2] == 0)
            {
                ext = "ttf";
            }
            else if (outbuffer[0] == 0x0a && outbuffer[1] == 0x05 && outbuffer[2] == 0x01 && outbuffer[3] == 0x08)
            {
                ext = "pcx";
            }
            else if (header.IndexOf("PK") >= 0)
            {
                ext = "zip";
            }
            else if (header.IndexOf("<") >= 0)
            {
                ext = "xml";
            }
            else if (file.IndexOf("lua") >= 0 && file.IndexOf("lua") < 50)
            {
                ext = "lua";
            }
            else if (header.IndexOf("DDS") >= 0)
            {
                ext = "dds";
            }
            else if (header.IndexOf("XSM") >= 0 || header.IndexOf("XAC") >= 0)
            {
                ext = "max";
            }
            else if (header.IndexOf("8BPS") >= 0)
            {
                ext = "8bps";
            }
            else if (header.IndexOf("bdLF") >= 0)
            {
                ext = "bdlf";
            }
            else if (header.IndexOf("gsLF") >= 0)
            {
                ext = "gslf";
            }
            else if (header.IndexOf("idLF") >= 0)
            {
                ext = "idlf";
            }
            else if (header.IndexOf("psLF") >= 0)
            {
                ext = "pslf";
            }
            else if (header.IndexOf("amLF") >= 0)
            {
                ext = "amlf";
            }
            else if (header.IndexOf("ntLF") >= 0)
            {
                ext = "ntlf";
            }
            else if (header.IndexOf("lgLF") >= 0)
            {
                ext = "lglf";
            }
            else if (file.IndexOf("Gamebry") >= 0)
            {
                ext = "nif";
            }
            else if (file.IndexOf("WMPHOTO") >= 0)
            {
                ext = "WMPHOTO";
            }
            else if (header.IndexOf("RIFF") >= 0)
            {
                string data = Convert.ToChar(outbuffer[8]).ToString()
                    + Convert.ToChar(outbuffer[9]).ToString()
                    + Convert.ToChar(outbuffer[10]).ToString()
                    + Convert.ToChar(outbuffer[11]).ToString();
                if (data.IndexOf("WAVE") >= 0)
                {
                    ext = "wav";
                }
                else
                {
                    ext = "riff";
                }
            }
            else if (header.IndexOf("; Zo") >= 0)
            {
                ext = "zone.txt";
            }
            else if (header.IndexOf("\0\0\0\0") >= 0)
            {
                ext = "zero.txt";
            }
            else
            {
                if (commaNum != 0 && commaNum >= 10)//((outbuffer.Length < 10000) ? outbuffer.Length : 10000) / commaNum < 250)
                {
                    ext = "csv";
                }
                else
                {
                }
            }

            if (ext == "txt")
            {
            }

            return ext;
        }
        #endregion

        private void CopyStream(System.IO.Stream input, System.IO.Stream output)
        {
            byte[] buffer = new byte[1];
            int len;
            int totalRead = 0;

            while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, len);
                totalRead += len;
            }
            output.Flush();
        }

        /// <summary>
        /// Dispose of the worker, closes the stream:)
        /// </summary>
        public void Dispose()
        {
            if (archiveStream != null)
                archiveStream.Close();
        }

        /// <summary>
        /// Search for a file in the archive, actually it search for the file in the list built
        /// So you should build the file list before running this method
        /// </summary>
        /// <param name="filename">name of the file</param>
        /// <returns>The file found or null</returns>
        public FileInArchive SearchForFile(string filename)
        {
            for (int i = 0; i < archiveFileList.Count; i++)
            {
                if (archiveFileList[i].descriptor.filename == filename)
                {
                    return archiveFileList[i];
                }
            }
            return null;
        }

        #region Write To MYP Archive
        /// <summary>
        /// Replaces a file by a file
        /// </summary>
        /// <param name="archFile">file to replace</param>
        /// <param name="newFile">new file</param>
        public void ReplaceFile(FileInArchive archFile, FileStream newFile)
        {
            byte[] newFileBuffer = new byte[newFile.Length];
            newFile.Read(newFileBuffer, 0, newFileBuffer.Length);

            MemoryStream inputMS = new MemoryStream(newFileBuffer);
            MemoryStream outputMS = new MemoryStream();

            byte[] output_buffer = new byte[0];

            if (archFile.descriptor.compressionMethod == 1 && false) //ZLib compression
            {
                try
                {
                    ICSharpCode.SharpZipLib.Zip.Compression.Deflater def = new ICSharpCode.SharpZipLib.Zip.Compression.Deflater();
                    ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream defstream = new ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream(outputMS, def);
                    defstream.Write(newFileBuffer, 0, newFileBuffer.Length);
                    defstream.Flush();
                    defstream.Finish();

                    output_buffer = outputMS.GetBuffer();
                }
                catch (Exception e)
                {

                }

                archFile.descriptor.uncompressedSize = inputMS.Length;

                WriteFileToArchive(archFile, outputMS);
            }
            else if (archFile.descriptor.compressionMethod == 0 || true) //No compression
            {
                CopyStream(inputMS, outputMS);

                archFile.descriptor.uncompressedSize = inputMS.Length;

                WriteFileToArchive(archFile, outputMS);
            }
        }

        /// <summary>
        /// Writes buffer to the file and do some other treatment as well
        /// </summary>
        /// <param name="archFile">file to write</param>
        /// <param name="MS">memory stream containing the data</param>
        private void WriteFileToArchive(FileInArchive archFile, MemoryStream MS)
        {
            archiveStream.Seek((long)archFile.descriptor.fileTableEntryPosition + 12, SeekOrigin.Begin);
            int lowMSLength;
            lowMSLength = (int)(MS.Length & 0xFFFFFFFF);
            archiveStream.Write(ConvertLongToByteArray(lowMSLength), 0, 4);

            archiveStream.Seek((long)archFile.descriptor.fileTableEntryPosition + 16, SeekOrigin.Begin);
            lowMSLength = (int)(archFile.descriptor.uncompressedSize & 0xFFFFFFFF);
            archiveStream.Write(ConvertLongToByteArray(lowMSLength), 0, 4);


            byte[] tmp_bytearray = MS.GetBuffer();
            if (MS.Length <= archFile.descriptor.compressedSize)
            {
                archiveStream.Seek((long)(archFile.descriptor.startingPosition + archFile.descriptor.fileHeaderSize), SeekOrigin.Begin);
                archiveStream.Write(tmp_bytearray, 0, (int)MS.Length);
            }
            else
            {
                long fileSize = archiveStream.Length;

                archiveStream.Seek(0, SeekOrigin.End);
                if (archFile.metadata != null)
                {
                    archiveStream.Write(archFile.metadata, 0, archFile.metadata.Length);
                }
                else
                {
                    //Should throw an exception, but not all files have meta data so...
                }
                archiveStream.Write(tmp_bytearray, 0, (int)MS.Length);

                archiveStream.Seek((long)archFile.descriptor.fileTableEntryPosition, SeekOrigin.Begin);
                archiveStream.Write(ConvertLongToByteArray((int)(fileSize & 0xFFFFFFFF)), 0, 4);

                archiveStream.Seek((long)archFile.descriptor.fileTableEntryPosition + 4, SeekOrigin.Begin);
                archiveStream.Write(ConvertLongToByteArray((int)((fileSize >> 32) & 0xFFFFFFFF)), 0, 4);
            }

            archFile.descriptor.compressedSize = MS.Length;
            tmp_bytearray = null;
        }

        /// <summary>
        /// Yeah I know I should use bit converter :)
        /// But was too stupid at the time to know about BitConverter lol
        /// </summary>
        /// <param name="a32bitInt"></param>
        /// <returns></returns>
        private byte[] ConvertLongToByteArray(int a32bitInt)
        {
            byte[] array = new byte[4];
            array[3] = (byte)((a32bitInt & 0xFF000000) >> 24);
            array[2] = (byte)((a32bitInt & 0x00FF0000) >> 16);
            array[1] = (byte)((a32bitInt & 0x0000FF00) >> 8);
            array[0] = (byte)(a32bitInt & 0x000000FF);
            return array;
        }
        #endregion

        /// <summary>
        /// http://xoomer.alice.it/acantato/dev/wildcard/wildmatch.html
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        bool WildcardMatch(string pattern, string path)
        {
            if (pattern == "")
                return true;

            int s, p;
            int str = 0;
            int pat = 0;
            char[] patternTbl = pattern.ToCharArray();
            char[] pathTbl = path.ToCharArray();
            bool star = false;

        loopStart:
            for (s = str, p = pat; s < pathTbl.Length; ++s, ++p)
            {
                if (patternTbl[p] == '*')
                {
                    star = true;
                    str = s;
                    pat = p;
                    if (++pat >= patternTbl.Length)
                        return true;
                    goto loopStart;
                }
                if (pathTbl[s] != patternTbl[p])
                    goto starCheck;
            }
            if (patternTbl[p] == '*')
                ++p;
            return (p >= patternTbl.Length);

        starCheck:
            if (!star)
                return false;
            str++;
            goto loopStart;
        }


        public void SaveHashList()
        {
            hasherBuilder.SaveHashList();
        }
    }
}
