using FileExplorer.IO.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FileExplorer.IO
{
    public abstract class CompressionWrapperBase : ICompressorWrapper
    {
        #region Constructor

        #endregion

        #region Methods

        public abstract bool IsArchive(string fileName);
        public abstract byte[] GetEmptyArchiveBytes(string type);
        public abstract Task<bool> TestAsync(Stream stream);

        protected abstract bool compressMultiple(string type, Stream stream, Dictionary<string, System.IO.Stream> streamDic, 
            IProgress<FileExplorer.Defines.ProgressEventArgs> progress);
        protected abstract bool delete(string type, Stream stream, Func<string, bool> filter);
        protected abstract IEnumerable<object> list(Stream stream, string pattern, bool listSubdir);
        protected abstract bool exists(Stream stream, string pathOrMask, bool isFolder);
        protected abstract bool extractOne(Stream stream, string fileName, string password, System.IO.Stream outputStream);

        public bool ExtractOne(Stream stream, string fileName, string password, System.IO.Stream outputStream)
        {
            return extractOne(stream, fileName, password, outputStream);
        }
       
        public bool CompressMultiple(string type, Stream stream, Dictionary<string, System.IO.Stream> streamDic, 
            IProgress<FileExplorer.Defines.ProgressEventArgs> progress = null)
        {
            return compressMultiple(type, stream, streamDic, progress);
        }

        public bool Delete(string type, Stream stream, Func<string, bool> filter)
        {
            return delete(type, stream, filter);
        }

        public IEnumerable<object> List(Stream stream, string pattern, bool listSubdir = false)
        {
            return list(stream, pattern, listSubdir);
        }


        public bool Exists(Stream stream, string pathOrMask, bool isFolder)
        {
            return exists(stream, pathOrMask, isFolder);
        }


        #endregion

        #region Data

        #endregion

        #region Public Properties

        #endregion


    }
}
