/******************************************************************************
 * This file contains most of the events definitions used by the myp worker
 * 
 * 
 * 
 * Chryso
 *****************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace MYPWorker
{
    /// <summary>
    /// Delegate for the creation of file event
    /// </summary>
    /// <param name="sender">sender</param>
    /// <param name="e">File Event Args</param>
    public delegate void del_FileEventHandler(object sender, MYPFileEventArgs e);
    /// <summary>
    /// Delegate for the creation of file table event
    /// </summary>
    /// <param name="sender">sender</param>
    /// <param name="e">File Table Event Args</param>
    public delegate void del_FileTableEventHandler(object sender, MYPFileTableEventArgs e);

    /// <summary>
    /// Event type enum for file events, extraction, replacement and such
    /// </summary>
    public enum Event_ExtractionType
    {
        UnknownError,
        UnknownCompressionMethod,
        ExtractionFinished,
        FileExtracted,
        FileExtractionError
    }

    /// <summary>
    /// Event Argument class for file events, extraction, replacement and such
    /// </summary>
    public class MYPFileEventArgs : EventArgs
    {
        Event_ExtractionType state;
        long value;

        public long Value { get { return value; } }
        public Event_ExtractionType State { get { return state; } }

        public MYPFileEventArgs(Event_ExtractionType state, long value)
        {
            this.state = state;
            this.value = value;
        }
    }

    /// <summary>
    /// Event type enum for file table events
    /// </summary>
    public enum Event_FileTableType
    {
        NewFile,
        UpdateFile,
        FileError,
        Finished
    }

    /// <summary>
    /// Event Argument class for file table events
    /// </summary>
    public class MYPFileTableEventArgs : EventArgs
    {
        Event_FileTableType type;
        FileInArchive archFile;

        public FileInArchive ArchFile { get { return archFile; } }
        public Event_FileTableType Type { get { return type; } }

        public MYPFileTableEventArgs(Event_FileTableType type, FileInArchive archFile)
        {
            this.type = type;
            this.archFile = archFile;
        }
    }
}
