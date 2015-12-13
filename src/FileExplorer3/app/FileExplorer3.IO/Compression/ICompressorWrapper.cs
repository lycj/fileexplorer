using FileExplorer.Defines;
using FileExplorer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.IO.Interfaces
{
    /// <summary>
    /// An implementation of Compressor, called by ComressionPropertyProvider.Factory.
    /// </summary>
    public interface ICompressorWrapper
    {
        /// <summary>
        /// Whether the specified file is archive by the current wrapper?
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        bool IsArchive(string fileName);
        
        /// <summary>
        /// Return the contents of specified type of archive, in bytes.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        byte[] GetEmptyArchiveBytes(string type);

        /// <summary>
        /// Takes a stream and a pattern (See RegexPatterns.CompressionListPattern), and return it's subcontents entity
        /// </summary>
        /// <param name="container"></param>
        /// <param name="pattern"></param>
        /// <param name="listSubdir"></param>
        /// <returns></returns>
        IEnumerable<object> List(Stream stream, string pattern, bool listSubdir = false);

        /// <summary>
        /// Takes a stream and return whether the archive is valid.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        Task<bool> TestAsync(Stream stream);

        /// <summary>
        /// Return whether a file/mask exists, this should be faster than calling list.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="pathOrMask"></param>
        /// <param name="isFolder"></param>
        /// <returns></returns>
        bool Exists(Stream stream, string pathOrMask, bool isFolder);

        /// <summary>
        /// Extract one file to outputStream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fileName"></param>
        /// <param name="password"></param>
        /// <param name="outputStream"></param>
        bool ExtractOne(Stream stream, string fileName, string password, Stream outputStream);
        
        /// <summary>
        /// Compress multiple items (in streamDic) to archive.
        /// </summary>
        /// <param name="archivePath"></param>
        /// <param name="streamDic"></param>
        /// <param name="progress"></param>
        bool CompressMultiple(string type, Stream stream, Dictionary<string, Stream> streamDic,
            IProgress<ProgressEventArgs> progress = null);

        /// <summary>
        /// Delete items from archive.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="delPathOrMask"></param>
        bool Delete(string type, Stream stream, Func<string, bool> filter);
    }
}
