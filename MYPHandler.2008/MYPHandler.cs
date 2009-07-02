/******************************************************************************
 * This file contains all method necessary to read file header, search
 * and replace files in a myp archive
 * 
 * 
 * Chryso
 *****************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using nsHashDictionary;


namespace MYPHandler
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

        #region Properties
        public long Offset { get { return descriptor.startingPosition;  } }
        public uint Size { get { return descriptor.uncompressedSize; } }
        public uint CompressedSize { get { return descriptor.compressedSize; } }
        public byte CompressionMethod { get { return descriptor.compressionMethod; } }
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
    class FileTable
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
        public uint fileHeaderSize;
        public uint compressedSize;
        public uint uncompressedSize;
        public byte compressionMethod;
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

        /// <summary>
        /// Fill the descriptor out of the 
        /// </summary>
        /// <param name="buffer"></param>
        public FileInArchiveDescriptor(byte[] buffer)
        {
            startingPosition = convertLittleEndianBufferToInt(buffer, 0); //Last 32 bits
            long startingPosition65536 = convertLittleEndianBufferToInt(buffer, 4); //Fisrt 32 bits
            startingPosition += startingPosition65536 << 32; //Real starting position

            fileHeaderSize = convertLittleEndianBufferToInt(buffer, 8);

            compressedSize = convertLittleEndianBufferToInt(buffer, 12);
            uncompressedSize = convertLittleEndianBufferToInt(buffer, 16);

            Array.Copy(buffer, 20, file_hash, 0, 8);

            sh = convertLittleEndianBufferToInt(buffer, 20);
            ph = convertLittleEndianBufferToInt(buffer, 24);

            crc = BitConverter.ToInt32(buffer, 28);

            filename += string.Format("{0:X8}", crc);
            filename += "_";
            filename += string.Format("{0:X16}", BitConverter.ToInt64(file_hash,0));

            compressionMethod = buffer[32];
            isCompressed = (compressionMethod == 0) ? false : true;
        }

        public static uint convertLittleEndianBufferToInt(byte[] intBuffer, long offset)
        {
            uint result = 0;
            for (int i = 3; i >=0; i--)
            {
                result = result << 8;
                result += intBuffer[offset + i];
            }
            return result;
        }
    }
    #endregion

    public partial class MYPHandler
    {
        #region Event
        /// <summary>
        /// Event variable for treating file table events
        /// </summary>
        public event del_FileTableEventHandler event_FileTable;

        private void TriggerFileTableEvent(MYPFileTableEventArgs e)
        {
            if (event_FileTable != null)
            {
                event_FileTable(this, e);
            }
        }

        /// <summary>
        /// Updates the number of errors in the file table entry
        /// Raise the according event
        /// </summary>
        /// <param name="archFile">File that gave the error</param>
        private void Error_FileTableEntry(FileInArchive archFile)
        {
            error_FileEntryNumber++;
            TriggerFileTableEvent(new MYPFileTableEventArgs(Event_FileTableType.FileError, archFile));

        }
        #endregion

        #region Attributes
        HashDictionary hashDictionary; //the dictionnary
        public FileStream archiveStream; //the stream to read data from
        string currentMypFileName; //the current filename of the myp file being read
        string fullMypFileName;
        string mypPath; //the path of the filename
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
        public MYPHandler(string filename
            , del_FileTableEventHandler eventHandler_FileTable
            , del_FileEventHandler eventHandler_Extraction
            , HashDictionary hashDic)
        {
            this.hashDictionary = hashDic;
            if (eventHandler_Extraction != null)
                this.event_Extraction += eventHandler_Extraction;
            if (eventHandler_FileTable != null)
                this.event_FileTable += eventHandler_FileTable;

            //parse the filename to get the path
            this.currentMypFileName = filename.Substring(filename.LastIndexOf('\\') + 1, filename.Length - filename.LastIndexOf('\\') - 1);
            this.currentMypFileName = currentMypFileName.Split('.')[0];
            this.fullMypFileName = filename;
            if (filename.LastIndexOf('\\') >= 0)
            {
                this.mypPath = filename.Substring(0, filename.LastIndexOf('\\'));
            }
            else
            {
                this.mypPath = "";
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
            tableStart += ((long)FileInArchiveDescriptor.convertLittleEndianBufferToInt(buffer, 4)) << 32;
            GetFileNumber();
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

        #region File Table Entries

        public void ScanFileTable()
        {
            GetFileTable();
            TriggerExtractionEvent(new MYPFileEventArgs(Event_ExtractionType.Scanning,0));
        }
        /// <summary>
        /// Parse the source file and reads the file table entries
        /// Raises according event depending on result
        /// </summary>
        public void GetFileTable()
        {
            error_FileEntryNumber = 0;
            error_ExtractionNumber = 0;
            unCompressedSize = 0;
            numberOfFileNamesFound = 0;
            numberOfFilesFound = 0;

            //Init
            long currentReadingPosition;
            uint numberOfFileInTable = 0;
            long endOfTableAddress;
            byte[] bufferTableHeader = new byte[12];
            byte[] bufferFileDesc = new byte[FileInArchiveDescriptor.fileDescriptorSize];
            FileInArchive myArchFile;

            while (tableStart != 0)
            {
                archiveStream.Seek(tableStart, SeekOrigin.Begin);
                archiveStream.Read(bufferTableHeader, 0, bufferTableHeader.Length);

                numberOfFileInTable = FileInArchiveDescriptor.convertLittleEndianBufferToInt(bufferTableHeader, 0); //get number of files
                currentReadingPosition = tableStart + 12;
                endOfTableAddress = tableStart + 12 + (long)FileInArchiveDescriptor.fileDescriptorSize * (long)numberOfFileInTable; // calculates the end address

                tableStart = FileInArchiveDescriptor.convertLittleEndianBufferToInt(bufferTableHeader, 4); //find the next filetable
                tableStart += (long)FileInArchiveDescriptor.convertLittleEndianBufferToInt(bufferTableHeader, 8) <<32; //mostly 0

                #region File Table Read
                while (currentReadingPosition < endOfTableAddress)
                {
                    archiveStream.Seek(currentReadingPosition, SeekOrigin.Begin);
                    archiveStream.Read(bufferFileDesc, 0, bufferFileDesc.Length);

                    myArchFile = new FileInArchive();
                    myArchFile.descriptor = new FileInArchiveDescriptor(bufferFileDesc);

                    myArchFile.descriptor.fileTableEntryPosition = currentReadingPosition;

                    if (myArchFile.descriptor.startingPosition > 0
                        && myArchFile.descriptor.compressedSize > 0
                        && myArchFile.descriptor.uncompressedSize > 0 //If the compressed size is 0, then there is no file
                        )
                    {
                        //Search for the filename

                        HashData ffn = null;
                        if (hashDictionary != null)
                        {
                            ffn = hashDictionary.SearchHashList(myArchFile.descriptor.ph, myArchFile.descriptor.sh);
                            if (ffn != null && ffn.filename != "")
                            {
                                myArchFile.descriptor.foundFileName = true;
                                myArchFile.descriptor.filename = ffn.filename;
                                if (myArchFile.descriptor.crc != ffn.crc)
                                {
                                    myArchFile.State = FileInArchiveState.MODIFIED;
                                    hashDictionary.UpdateCRC(myArchFile.descriptor.ph, myArchFile.descriptor.sh, myArchFile.descriptor.crc);
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
                                    hashDictionary.AddHash(myArchFile.descriptor.ph, myArchFile.descriptor.sh, "", myArchFile.descriptor.crc);
                                    archiveNewFileList.Add(myArchFile);
                                }
                                else if (myArchFile.descriptor.crc != ffn.crc)
                                {
                                    myArchFile.State = FileInArchiveState.MODIFIED;
                                    hashDictionary.UpdateCRC(myArchFile.descriptor.ph, myArchFile.descriptor.sh, myArchFile.descriptor.crc);
                                    archiveModifiedFileList.Add(myArchFile);
                                }
                                //Retrieve header
                                myArchFile.metadata = new byte[myArchFile.descriptor.fileHeaderSize];

                                archiveStream.Seek(myArchFile.descriptor.startingPosition, SeekOrigin.Begin);
                                archiveStream.Read(myArchFile.metadata, 0, myArchFile.metadata.Length);

                                //Retrieve head of the data for extension definition purpose
                                if (!myArchFile.descriptor.foundFileName)
                                {
                                    if (myArchFile.descriptor.compressedSize < myArchFile.data_start_200.Length)
                                        myArchFile.data_start_200 = new byte[myArchFile.descriptor.compressedSize];

                                    archiveStream.Seek(myArchFile.descriptor.startingPosition + myArchFile.descriptor.fileHeaderSize, SeekOrigin.Begin);
                                    archiveStream.Read(myArchFile.data_start_200, 0, myArchFile.data_start_200.Length);

                                    try
                                    {
                                        TreatHeader(myArchFile);
                                    }
                                    catch (Exception e)
                                    {
                                        Error_FileTableEntry(myArchFile);
                                    }
                                }
                            }

                            //weird Wildcard match.
                            //Updates the table
                            try
                            {
                                numberOfFilesFound++;
                                if (WildcardMatch(pattern, myArchFile.Filename))
                                {
                                    archiveFileList.Add(myArchFile);
                                    unCompressedSize += myArchFile.descriptor.uncompressedSize;
                                    /// \todo Speed: we should do something here to avoid passing an event for each and every file....
                                    TriggerFileTableEvent(new MYPFileTableEventArgs(Event_FileTableType.NewFile, myArchFile));
                                }   
                                else
                                {
                                    TriggerFileTableEvent(new MYPFileTableEventArgs(Event_FileTableType.UpdateFile, null));
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
                } // currentReadingPosition < endOfTableAddress
                #endregion
            } //tableStart != 0

            TriggerFileTableEvent(new MYPFileTableEventArgs(Event_FileTableType.Finished, null));
        }

        #endregion

        #region Header stuff
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

            file = System.Text.Encoding.ASCII.GetString(outbuffer, 0, outbuffer.Length);
            char[] separators = { ',' };
            int commaNum = file.Split(separators, 10).Length;
            string header = System.Text.Encoding.ASCII.GetString(outbuffer, 0, 4);
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
            else if (header.IndexOf("XSM") >= 0)
            {
                ext = "xsm";
            }
            else if (header.IndexOf("XAC") >= 0)
            {
                ext = "xac";
            }
            else if (header.IndexOf("8BPS") >= 0)
            {
                ext = "8bps";
            }
            else if (header.IndexOf("bdLF") >= 0)
            {
                ext = "db";
            }
            else if (header.IndexOf("gsLF") >= 0)
            {
                ext = "geom";
            }
            else if (header.IndexOf("idLF") >= 0)
            {
                ext = "diffuse";
            }
            else if (header.IndexOf("psLF") >= 0)
            {
                ext = "specular";
            }
            else if (header.IndexOf("amLF") >= 0)
            {
                ext = "mask";
            }
            else if (header.IndexOf("ntLF") >= 0)
            {
                ext = "tint";
            }
            else if (header.IndexOf("lgLF") >= 0)
            {
                ext = "glow";
            }
            else if (file.IndexOf("Gamebry") >= 0)
            {
                ext = "nif";
            }
            else if (file.IndexOf("WMPHOTO") >= 0)
            {
                ext = "lmp";
            }
            else if (header.IndexOf("RIFF") >= 0)
            {
                string data = System.Text.Encoding.ASCII.GetString(outbuffer, 8, 4);
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
            else if (header.IndexOf("PNG") >= 0)
            {
                ext = "png";
            }
            else if (commaNum >= 10)
            {
                ext = "csv";
            }

            return ext;
        }
        #endregion

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

        public void DumpFileList()
        {
            string filelistname = extractionPath + "\\" + Path.GetFileNameWithoutExtension(fullMypFileName) + "_FileList.txt";
            if (File.Exists(filelistname)) File.Delete(filelistname);

            FileStream outputFS = new FileStream(filelistname, FileMode.Create);
            StreamWriter writer = new StreamWriter(outputFS);

            foreach (FileInArchive file in archiveFileList)
            {
                if (file.descriptor.filename.CompareTo("") != 0)
                {
                    writer.WriteLine(file.descriptor.filename);
                }
            }
            writer.Close();
        }

        #region Write To MYP Archive

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

                archFile.descriptor.uncompressedSize = (uint)inputMS.Length;

                WriteFileToArchive(archFile, outputMS);
            }
            else if (archFile.descriptor.compressionMethod == 0 || true) //No compression
            {
                archFile.descriptor.compressionMethod = 0;
                CopyStream(inputMS, outputMS);

                archFile.descriptor.uncompressedSize = (uint)inputMS.Length;

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
            archiveStream.Close();
            try
            {
                archiveStream = new FileStream(fullMypFileName, FileMode.Open, FileAccess.ReadWrite);
            }
            catch (Exception e)
            {
                throw new Exception("You need to stop application currently using the following file: " + fullMypFileName);
            }
            #region Writting

            archiveStream.Seek((long)archFile.descriptor.fileTableEntryPosition + 12, SeekOrigin.Begin);
            int lowMSLength;
            lowMSLength = (int)(MS.Length & 0xFFFFFFFF);
            archiveStream.Write(ConvertLongToByteArray(lowMSLength), 0, 4);

            archiveStream.Seek((long)archFile.descriptor.fileTableEntryPosition + 16, SeekOrigin.Begin);
            lowMSLength = (int)(archFile.descriptor.uncompressedSize & 0xFFFFFFFF);
            archiveStream.Write(ConvertLongToByteArray(lowMSLength), 0, 4);

            archiveStream.Seek((long)archFile.descriptor.fileTableEntryPosition + 32, SeekOrigin.Begin);
            byte[] bArray = new byte[1];
            bArray[0] = (byte)archFile.CompressionMethod;
            archiveStream.Write(bArray, 0, 1);


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
                    byte[] fakeMetadata = new byte[archFile.descriptor.fileHeaderSize];
                    archiveStream.Write(fakeMetadata, 0, fakeMetadata.Length);
                    //Should throw an exception, but not all files have meta data so...
                }
                archiveStream.Seek(0, SeekOrigin.End);
                archiveStream.Write(tmp_bytearray, 0, (int)MS.Length);

                archiveStream.Seek((long)archFile.descriptor.fileTableEntryPosition, SeekOrigin.Begin);
                archiveStream.Write(ConvertLongToByteArray((int)(fileSize & 0xFFFFFFFF)), 0, 4);

                archiveStream.Seek((long)archFile.descriptor.fileTableEntryPosition + 4, SeekOrigin.Begin);
                archiveStream.Write(ConvertLongToByteArray((int)((fileSize >> 32) & 0xFFFFFFFF)), 0, 4);

                archFile.descriptor.startingPosition = (int)(fileSize & 0xFFFFFFFF);
            }

            archFile.descriptor.compressedSize = (uint)MS.Length;
            tmp_bytearray = null;
            #endregion

            archiveStream.Close();
            archiveStream = new FileStream(fullMypFileName, FileMode.Open, FileAccess.Read);

        }

        /// <summary>
        /// Yeah I know I should use bit converter :)
        /// But was too stupid at the time to know about BitConverter lol
        /// </summary>
        /// <param name="a32bitInt"></param>
        /// <returns></returns>
        private byte[] ConvertLongToByteArray(int a32bitInt)
        {
            return BitConverter.GetBytes(a32bitInt);
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

    }

    // pass data to threads
    public struct MypThreadParam
    {
        public string[] fileNames;
        public int currentFile;

        public MypThreadParam(string[] fileNames, int currenFile)
        {
            this.fileNames = fileNames;
            this.currentFile = currenFile;
        }
    }
}
