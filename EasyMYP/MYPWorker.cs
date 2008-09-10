using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.IO.Compression;
using WarhammerOnlineHash;
using MYPHasherBuilder;

namespace MYPWorker
{
    #region MYP File Format Helpers
    public class FileInArchive
    {
        public FileInArchiveDescriptor descriptor = new FileInArchiveDescriptor();
        public byte[] metadata;
        public byte[] data;
        public byte[] data_start_200 = new byte[200];

        //public FileTableEntryDescriptor Descriptor { get { return descriptor; } }

        #region Properties
        public long Offset { get { return descriptor.startingPosition + descriptor.startingPosition65536; } }
        public long Size { get { return descriptor.uncompressedSize; } }
        public long CompressedSize { get { return descriptor.compressedSize; } }
        public int CompressionMethod { get { return descriptor.compressionMethod; } }
        public string Filename { get { return descriptor.filename; } }
        public string Extension { get { return descriptor.extension; } }
        #endregion
    }

    public class FileTable
    {
        public long offset;
        public long num_of_files;

        public List<FileInArchive> files = new List<FileInArchive>();
    }

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
        public byte[] crc = new byte[4];
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

            for (int i = 0; i < 4; i++)
            {
                crc[i] = buffer[28 + i];
            }

            for (int i = 0; i < 4; i++)
            {
                filename += string.Format("{0:X2}", crc[i]);
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

    public class MYPWorker
    {
        #region Delegates
        public delegate void del_NewFileEntry(FileInArchive archFile);
        del_NewFileEntry OnNewFileEntry;

        public delegate void del_Error_FileTableEntry(FileInArchive archFile);
        del_Error_FileTableEntry OnError_FileTableEntry;

        public delegate void del_Error_ExtractionFile(FileInArchive archFile);
        del_Error_ExtractionFile OnError_ExtractionFile;
        #endregion

        #region Attributes
        Hasher hasherBuilder = new Hasher();
        FileStream archiveStream;
        string currentFileName;
        string path;
        public List<FileInArchive> archiveFileList = new List<FileInArchive>(); //contains all the files information, but not the data because of memory limitation on a 32Bits system
        long tableStart;
        long unCompressedSize = 0;
        long numberOfFileNamesFound = 0;
        long totalNumberOfFiles = 0;
        long numberOfFilesFound = 0;
        long error_FileEntryNumber = 0;
        long error_ExtractionNumber = 0;
        #endregion

        #region Properties
        public long UnCompressedSize { get { return unCompressedSize; } }
        public long NumberOfFileNamesFound { get { return numberOfFileNamesFound; } }
        public long TotalNumberOfFiles { get { return totalNumberOfFiles; } }
        public long NumberOfFilesFound { get { return numberOfFilesFound; } }
        public long Error_FileEntryNumber { get { return error_FileEntryNumber; } }
        public long Error_ExtractionNumber { get { return error_ExtractionNumber; } }
        #endregion

        #region Constructor -- Initialization
        public MYPWorker(string filename, del_NewFileEntry OnNewFileEntry
            , del_Error_FileTableEntry OnError_FileTableEntry
            , del_Error_ExtractionFile OnError_ExtractionFile
            , Hasher hasher)
        {
            this.hasherBuilder = hasher;

            this.OnNewFileEntry = OnNewFileEntry;
            this.OnError_FileTableEntry = OnError_FileTableEntry;
            this.OnError_ExtractionFile = OnError_ExtractionFile;

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

            unCompressedSize = 0;
            numberOfFileNamesFound = 0;
            totalNumberOfFiles = 0;
            numberOfFilesFound = 0;
            error_FileEntryNumber = 0;
            error_ExtractionNumber = 0;

            archiveStream = new FileStream(filename, FileMode.Open);

            //read the position of the starting file table
            archiveStream.Seek(0x0C, SeekOrigin.Begin);
            byte[] buffer = new byte[8];
            archiveStream.Read(buffer, 0, buffer.Length);
            tableStart = FileInArchiveDescriptor.convertLittleEndianBufferToInt(buffer, 0);
            tableStart += FileInArchiveDescriptor.convertLittleEndianBufferToInt(buffer, 4) * 256 * 256 * 256 * 256;
            GetFileNumber();
        }

        void GetFileNumber()
        {
            archiveStream.Seek(24, SeekOrigin.Begin);
            byte[] buffer = new byte[4];
            archiveStream.Read(buffer, 0, buffer.Length);

            totalNumberOfFiles = FileInArchiveDescriptor.convertLittleEndianBufferToInt(buffer, 0);
        }
        #endregion

        #region Event & Error Stuff
        private void NewFile(FileInArchive file)
        {
            archiveFileList.Add(file);
            unCompressedSize += file.descriptor.uncompressedSize;
            numberOfFilesFound++;
            if (OnNewFileEntry != null)
            {
                OnNewFileEntry(file);
            }
        }

        private void Error_FileTableEntry(FileInArchive archFile)
        {
            error_FileEntryNumber++;
            if (OnError_FileTableEntry != null) OnError_FileTableEntry(archFile);
        }

        private void Error_ExtractingFile(FileInArchive archFile)
        {
            error_ExtractionNumber++;
            if (OnError_ExtractionFile != null) OnError_ExtractionFile(archFile);
        }
        #endregion

        #region File Table Entries

        public void GetFileTable()
        {
            error_FileEntryNumber = 0;
            error_ExtractionNumber = 0;
            unCompressedSize = 0;
            numberOfFileNamesFound = 0;
            totalNumberOfFiles = 0;
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
                        string ffn = hasherBuilder.SearchHashList(myArchFile.descriptor.ph, myArchFile.descriptor.sh);

                        if (ffn != "")
                        {
                            myArchFile.descriptor.foundFileName = true;
                            myArchFile.descriptor.filename = ffn;
                            numberOfFileNamesFound++;
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
                            NewFile(myArchFile);
                        }
                        catch (Exception e)
                        {
                            Error_FileTableEntry(myArchFile);
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
        }

        #endregion

        #region Header stuff
        public void CopyHeaderStream(System.IO.Stream input, System.IO.Stream output)
        {
            byte[] buffer = new byte[1];
            int len;

            while ((len = input.Read(buffer, 0, 1)) > 0)
            {
                output.Write(buffer, 0, (int)buffer.Length);
            }

            output.Flush();
        }

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
                archFile.descriptor.extension = GetExtension(output_buffer);
            }
            else
            {
            }

        }

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

        public void CopyStream(System.IO.Stream input, System.IO.Stream output)
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

        public void Dispose()
        {
            if (archiveStream != null)
                archiveStream.Close();
        }

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

        #region Read & Extract File From Archive
        public void ExtractAll()
        {
            for (int i = 0; i < archiveFileList.Count; i++)
            {
                ExtractFile(archiveFileList[i]);
            }
        }

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
                    }
                    catch (Exception e)
                    {
                        Error_ExtractingFile(archFile);
                    }

                    SaveBufferToFile(output_buffer, archFile.descriptor.startingPosition, true, archFile.descriptor.foundFileName, archFile.descriptor.filename, archFile.descriptor.extension);
                }
                else if (archFile.descriptor.compressionMethod == 0) //No compression
                {
                    SaveBufferToFile(archFile.data, archFile.descriptor.startingPosition, false, archFile.descriptor.foundFileName, archFile.descriptor.filename, archFile.descriptor.extension);
                }
                else
                {
                    Error_ExtractingFile(archFile);
                }
            }
            catch (Exception e)
            {
                Error_ExtractingFile(archFile);
            }
        }

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

            if (!trueFileName)
            {
                if (!Directory.Exists(path + "\\" + currentFileName)) Directory.CreateDirectory(path + "\\" + currentFileName);
                if (!Directory.Exists(path + "\\" + currentFileName + "\\" + ext)) Directory.CreateDirectory(path + "\\" + currentFileName + "\\" + ext);

                extraction_filename = path + "\\" + currentFileName + "\\" + ext + "\\" + filename + "." + ext;
            }
            else
            {
                filename = filename.Replace('\\', '/');
                string[] folders = filename.Split('/');
                string tmpPath = folders[0];
                if (!Directory.Exists(tmpPath)) Directory.CreateDirectory(tmpPath);

                for (int i = 1; i < folders.Length - 1; i++)
                {
                    tmpPath += '/' + folders[i];
                    if (!Directory.Exists(tmpPath)) Directory.CreateDirectory(tmpPath);
                }

                extraction_filename = filename;
            }

            if (File.Exists(extraction_filename)) File.Delete(extraction_filename);

            FileStream outputFS = new FileStream(extraction_filename, FileMode.Create);
            outputFS.Write(buffer, 0, (int)buffer.Length);
            outputFS.Close();
        }
        #endregion

        #region Write To MYP Archive
        public void ReplaceFile(FileInArchive archFile, FileStream newFile)
        {
            byte[] newFileBuffer = new byte[newFile.Length];
            newFile.Read(newFileBuffer, 0, newFileBuffer.Length);

            MemoryStream inputMS = new MemoryStream(newFileBuffer);
            MemoryStream outputMS = new MemoryStream();

            byte[] output_buffer = new byte[0];

            if (archFile.descriptor.compressionMethod == 1) //ZLib compression
            {
                try
                {
                    ICSharpCode.SharpZipLib.Zip.Compression.Deflater def = new ICSharpCode.SharpZipLib.Zip.Compression.Deflater(9);
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
            else if (archFile.descriptor.compressionMethod == 0) //No compression
            {
                CopyStream(inputMS, outputMS);

                archFile.descriptor.uncompressedSize = inputMS.Length;

                WriteFileToArchive(archFile, outputMS);
            }
        }

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
                archiveStream.Write(archFile.metadata, 0, archFile.metadata.Length);
                archiveStream.Write(tmp_bytearray, 0, (int)MS.Length);

                archiveStream.Seek((long)archFile.descriptor.fileTableEntryPosition, SeekOrigin.Begin);
                archiveStream.Write(ConvertLongToByteArray((int)(fileSize & 0xFFFFFFFF)), 0, 4);

                archiveStream.Seek((long)archFile.descriptor.fileTableEntryPosition + 4, SeekOrigin.Begin);
                archiveStream.Write(ConvertLongToByteArray((int)((fileSize >> 32) & 0xFFFFFFFF)), 0, 4);
            }

            archFile.descriptor.compressedSize = MS.Length;
            tmp_bytearray = null;
        }

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
    }
}
