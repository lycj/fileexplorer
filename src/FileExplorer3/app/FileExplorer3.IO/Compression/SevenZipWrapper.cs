#define SZW_LOADASSTREAM //getExtractor() func load using FileStream instead of FileName

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if !NETFX_CORE
using SevenZip;
#endif
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using FileExplorer.IO.Interfaces;
using FileExplorer.WPF.Utils;


namespace FileExplorer.IO.Compress
{
#if !NETFX_CORE
    public class SevenZipWrapper : CompressionWrapperBase, ICompressorWrapper
    {
        #region mini tuple implementation

        public class tuple<T1, T2>
        {
            public T1 Value1 { get; private set; }
            public T2 Value2 { get; private set; }
            public tuple(T1 value1, T2 value2)
            {
                Value1 = value1;
                Value2 = value2;
            }
        }

        #endregion

        #region Constructor

        public SevenZipWrapper()
        {
            //_archiveFile = archiveFile;
        }

        #endregion

        #region Static Methods


        public static OutArchiveFormat getArchiveFormat(string archiveName)
        {

            switch (PathFE.GetExtension(archiveName).ToLower())
            {
                case ".7z":
                case ".7zip": return OutArchiveFormat.SevenZip;
                case ".zip": return OutArchiveFormat.Zip;
                case ".bzip": return OutArchiveFormat.BZip2;
                case ".z":
                case ".gz":
                case ".gzip":
                    return OutArchiveFormat.GZip;
                case ".tar":
                    return OutArchiveFormat.Tar;
                default: throw new NotImplementedException();
            }
        }

        #endregion

        #region Methods

        private SevenZipExtractor getExtractor(Stream stream, string password = null)
        {
            if (!String.IsNullOrEmpty(password))
                return new SevenZipExtractor(stream, password);
            return new SevenZipExtractor(stream);
        }

        private SevenZipExtractor getExtractor(string archivePath, string password = null)
        {
            if (!String.IsNullOrEmpty(password))
                return new SevenZipExtractor(archivePath, password);
            return new SevenZipExtractor(archivePath);
        }


        private SevenZipCompressor getCompressor(OutArchiveFormat archiveFormat, string password = null,
            IProgress<FileExplorer.Defines.ProgressEventArgs> progress = null)
        {
            SevenZipCompressor compressor = new SevenZipCompressor();
            compressor.DirectoryStructure = true;
            compressor.ArchiveFormat = archiveFormat;
            compressor.CompressionMode = CompressionMode.Append;

            if (progress != null)
            {
                string workingFile = null;
                compressor.Compressing += (o, e) =>
                    {
                        var args = new FileExplorer.Defines.ProgressEventArgs(e.PercentDone, 100, workingFile);
                        progress.Report(args);
                        if (args.Cancel)
                            e.Cancel = true;
                    };

                compressor.FileCompressionStarted += (o, e) =>
                    {
                        workingFile = e.FileName;

                        var args = new FileExplorer.Defines.ProgressEventArgs(e.PercentDone, 100, workingFile);
                        progress.Report(args);
                        if (args.Cancel)
                            e.Cancel = true;
                    };
            }

            return compressor;
        }


        private IEnumerable<tuple<int, string>> lookup(SevenZipExtractor extractor, string pathOrMask)
        {
            return lookup(extractor, p => PathFE.MatchFileMask(p, pathOrMask));
        }

        private IEnumerable<tuple<int, string>> lookup(SevenZipExtractor extractor, Func<string, bool> filter)
        {
            foreach (ArchiveFileInfo afi in extractor.ArchiveFileData)
                if (filter(afi.FileName))
                {
                    yield return new tuple<int, string>(afi.Index, afi.FileName);
                }
        }

        protected override IEnumerable<object> list(Stream stream, string pattern, bool listSubdir = false)
        {
            return listCore(stream, pattern, listSubdir).Cast<object>().ToList();
            //foreach (var afi in listCore(stream, pattern, listSubdir))
            //{
            //    yield return afi;
            //}
        }

        /// <summary>
        /// List entries that match the filterStr, the filterStr must be in format of RegexPatterns.SevenZipListPattern.
        /// </summary>
        /// <param name="filterStr"></param>
        /// <returns></returns>
        internal IEnumerable<ArchiveFileInfo> listCore(Stream stream, string pattern, bool listSubdir)
        {
            if (pattern.IndexOf("?<name>") == -1)
                throw new ArgumentException("");
            if (pattern.IndexOf("?<trail>") == -1)
                throw new ArgumentException("");
            if (pattern.IndexOf("?<parent>") == -1)
                throw new ArgumentException("");

            List<string> returnedPathList = new List<string>();



            List<ArchiveFileInfo> afiList;

            try
            {
                using (SevenZipExtractor extractor = getExtractor(stream))
                {
                    afiList = new List<ArchiveFileInfo>(extractor.ArchiveFileData);
                }
            }
            catch (Exception ex) { afiList = new List<ArchiveFileInfo>(); }

            foreach (ArchiveFileInfo afi in afiList)
            {
                Match match = new Regex(pattern).Match(afi.FileName);
                if (match.Success)
                {
                    string parent = match.Groups["parent"].Value;
                    string name = match.Groups["name"].Value;
                    string trail = match.Groups["trail"].Value;

                    if (!afi.IsDirectory && String.IsNullOrEmpty(trail))
                    {
                        if (returnedPathList.IndexOf(afi.FileName.ToLower()) == -1)
                        {
                            yield return afi;
                            returnedPathList.Add(afi.FileName.ToLower());
                        }
                    }
                    else if (!String.IsNullOrEmpty(trail) || listSubdir)
                    {
                        string dirName = PathFE.Combine(parent, name);
                        if (returnedPathList.IndexOf(dirName.ToLower()) == -1)
                        {
                            yield return new ArchiveFileInfo()
                            {
                                FileName = dirName,
                                IsDirectory = true
                            };
                            returnedPathList.Add(dirName.ToLower());
                        }
                    }
                }

            }
        }


        //public IEnumerable<ArchiveFileInfo> listSimple(Stream stream, string relativePathOrMask)
        //{
        //    lock (_lockObject)
        //        using (SevenZipExtractor extractor = getExtractor(stream))
        //            lock (extractor)
        //            {
        //                foreach (ArchiveFileInfo afi in extractor.ArchiveFileData)
        //                {
        //                    if (PathFE.MatchFileMask(afi.FileName, relativePathOrMask, true))
        //                        yield return afi;

        //                }
        //            }
        //}



        public override Task<bool> TestAsync(Stream stream)
        {
            return Task.FromResult<bool>(getExtractor(stream, null).Check());
        }

        protected override bool exists(Stream stream, string pathOrMask, bool isFolder)
        {

            using (SevenZipExtractor extractor = getExtractor(stream))
            {
                foreach (ArchiveFileInfo afi in extractor.ArchiveFileData)
                {
                    if (PathFE.MatchFileMask(afi.FileName, pathOrMask) && (afi.IsDirectory == isFolder))
                        return true;
                    else if (afi.FileName.StartsWith(pathOrMask, StringComparison.InvariantCultureIgnoreCase))
                        return true;
                }
            }
            return false;
        }


        #region Extract


        private void extract(SevenZipExtractor extractor, string mask, string toDir, string password)
        {
            if (extractor.FilesCount == 0)
                return;

            if (!extractor.IsSolid)
            {
                ExtractFileCallback callBack = (t) =>
                {
                    if (PathFE.MatchFileMask(t.ArchiveFileInfo.FileName, mask))
                    {
                        if (t.Reason == ExtractFileCallbackReason.Start)
                        {
                            string outputFileName = PathFE.Combine(toDir, t.ArchiveFileInfo.FileName);
                            string outputDir = PathFE.GetDirectoryName(outputFileName);

                            if (!Directory.Exists(outputDir))
                                Directory.CreateDirectory(outputDir);

                            if (t.ArchiveFileInfo.IsDirectory)
                            {
                                if (!Directory.Exists(outputFileName))
                                    Directory.CreateDirectory(outputFileName);
                            }
                            else
                            {
                                if (File.Exists(outputFileName))
                                    File.Delete(outputFileName);

                                t.ExtractToFile = outputFileName;
                            }
                        }
                    }
                };

                extractor.ExtractFiles(callBack);
            }
            else
            {
                List<int> indexes = new List<int>();
                foreach (var afi in extractor.ArchiveFileData)
                    if (PathFE.MatchFileMask(afi.FileName, mask))
                        indexes.Add(afi.Index);
                extractor.ExtractFiles(toDir, indexes.ToArray());
            }

        }

        protected override bool extractOne(Stream stream, string fileName, string password, Stream outputStream)
        {
            using (SevenZipExtractor extractor = getExtractor(stream, password))
                foreach (var afi in extractor.ArchiveFileData)
                    if (afi.FileName.Equals(fileName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        extractor.ExtractFile(afi.Index, outputStream);
                        return true;
                    }
            return false;
        }

        public void extract(Stream stream, string mask, string toDir, string password,
            EventHandler<FileInfoEventArgs> onExtractionStarted,
            EventHandler<FileInfoEventArgs> onExtractionEnded,
            EventHandler<ProgressEventArgs> onExtracting)
        {
            using (SevenZipExtractor extractor = getExtractor(stream, password))
            {
                if (onExtractionStarted != null) extractor.FileExtractionStarted += onExtractionStarted;
                if (onExtractionEnded != null) extractor.FileExtractionStarted += onExtractionEnded;
                if (onExtracting != null) extractor.Extracting += onExtracting;

                extract(extractor, mask, toDir, password);

                if (onExtractionStarted != null) extractor.FileExtractionStarted -= onExtractionStarted;
                if (onExtractionEnded != null) extractor.FileExtractionStarted -= onExtractionEnded;
                if (onExtracting != null) extractor.Extracting -= onExtracting;


            }
        }

        public void extract(Stream stream, string mask, string toDir, string password)
        {
            extract(stream, mask, toDir, password, null, null, null);
        }
        #endregion

        #region ModifyArchive

        public void Modify(string archivePath, string orginalPath, string destName, bool isFolder,
            IProgress<FileExplorer.Defines.ProgressEventArgs> progress = null)
        {
            OutArchiveFormat archiveFormat = SevenZipWrapper.getArchiveFormat(archivePath);
            string srcParentPath = FileExplorer.Models.PathHelper.Disk.GetDirectoryName(orginalPath);
            string destPath = FileExplorer.Models.PathHelper.Disk.Combine(srcParentPath, destName);
            orginalPath = orginalPath.TrimEnd('\\');

            var dic = lookup(getExtractor(archivePath), isFolder ? orginalPath + "\\*" : orginalPath).ToDictionary(tup => tup.Value1,
                tup => FileExplorer.Models.PathHelper.Disk.Combine(destPath,
                    tup.Value2.Substring(orginalPath.Length)));
            if (dic.Count() > 0)
            {
                SevenZipCompressor compressor = getCompressor(archiveFormat, null, progress);
                compressor.ModifyArchive(archivePath, dic);
            }
        }

        public bool Modify(string type, Stream stream,
            string orginalPath, string destName, bool isFolder, IProgress<FileExplorer.Defines.ProgressEventArgs> progress = null)
        {
            var arcFormat = getArchiveFormat("abc." + type);
            SevenZipCompressor compressor = getCompressor(arcFormat, null, progress);

            string tempFile = null;

            using (var tempStream = TempStreamUtils.NewTempStream(out tempFile, type))
                StreamUtils.CopyStream(stream, tempStream, true, true, false);

            Modify(tempFile, orginalPath, destName, isFolder, progress);

            using (var tempStream = new FileStream(tempFile, FileMode.Open))
                StreamUtils.CopyStream(tempStream, stream, false, true, true);


            return true;
        }

        #endregion

        #region Compress

        public void CompressMultiple(string archivePath, Dictionary<string, Stream> streamDic,
            IProgress<FileExplorer.Defines.ProgressEventArgs> progress = null)
        {
            OutArchiveFormat archiveFormat = SevenZipWrapper.getArchiveFormat(archivePath);

            Delete(archiveFormat, archivePath, p => streamDic.Keys.Contains(p));

            SevenZipCompressor compressor = getCompressor(archiveFormat, null, progress);

            compressor.CompressStreamDictionary(streamDic, archivePath);

        }


        //public void compressOne(string archivePath, string fileName, Stream fileStream)
        //{
        //    CompressMultiple(archivePath, new Dictionary<string, Stream>() { { fileName, fileStream } });
        //}

        //public bool CompressOne(string type, Stream stream, string fileName, Stream fileStream)
        //{
        //    return CompressMultiple(type, stream, new Dictionary<string, Stream>() { { fileName, fileStream } });
        //}




        protected override bool compressMultiple(string type, Stream stream,
            Dictionary<string, Stream> streamDic, IProgress<FileExplorer.Defines.ProgressEventArgs> progress = null)
        {
            var arcFormat = getArchiveFormat("abc." + type);
            SevenZipCompressor compressor = getCompressor(arcFormat, null, progress);

            //SevenZipSharp crash when compressor.CompressStreamDictionary() is used, 
            //if Compression mode is Append (Create is fine).
            //lock (_lockObject)
            //    compressor.CompressStreamDictionary(streamDic, container.Stream);

            string tempFile = null;

            using (var tempStream = TempStreamUtils.NewTempStream(out tempFile, type))
                StreamUtils.CopyStream(stream, tempStream, true, true, false);

            CompressMultiple(tempFile, streamDic, progress);

            using (var tempStream = new FileStream(tempFile, FileMode.Open))
                StreamUtils.CopyStream(tempStream, stream, false, true, true);


            return true;
        }

        #endregion

        #region Delete

        private void Delete(OutArchiveFormat archiveFormat, string archivePath, Func<string, bool> filter)
        {
            Dictionary<int, string> fileDictionary = new Dictionary<int, string>();

            foreach (var foundItem in lookup(getExtractor(archivePath, null), filter))
                fileDictionary.Add(foundItem.Value1, null);

            if (fileDictionary.Count > 0)
            {

                SevenZipCompressor compressor = getCompressor(archiveFormat);
                compressor.ModifyArchive(archivePath, fileDictionary);

            }
        }

        protected override bool delete(string type, Stream stream, Func<string, bool> filter)
        {
            OutArchiveFormat archiveFormat = SevenZipWrapper.getArchiveFormat("abc" + type);
            Dictionary<int, string> fileDictionary = new Dictionary<int, string>();

            foreach (var foundItem in lookup(getExtractor(stream, null), filter))
                fileDictionary.Add(foundItem.Value1, null);

            if (fileDictionary.Count > 0)
            {
                string tempFile;

                using (var tempStream = TempStreamUtils.NewTempStream(out tempFile, "tmp"))
                    StreamUtils.CopyStream(stream, tempStream, true, true, false);

                Delete(archiveFormat, tempFile, filter);

                using (var tempStream = new FileStream(tempFile, FileMode.Open))
                    StreamUtils.CopyStream(tempStream, stream, false, true, true);


                //SevenZipSharp crash when compressor.ModifyArchive() is used, 
                //if Compression mode is Append (Create is fine).
                //lock (_lockObject)
                //{
                //    SevenZipCompressor compressor = getCompressor(archiveFormat);
                //    compressor.ModifyArchive(archivePath, fileDictionary);
                //}
            }

            return true;
        }

        #endregion


        public override bool IsArchive(string fileName)
        {
            return CompressionUtils.IsArchive(fileName);
        }

        public static byte[] GetArchiveBytes(string type)
        {
            switch (type.TrimStart('.').ToLower())
            {
                case "zip": return CompressionUtils.EmptyZip;
                case "7z": return CompressionUtils.Empty7z;
            }
            return null;
        }


        public override byte[] GetEmptyArchiveBytes(string type)
        {
            return SevenZipWrapper.GetArchiveBytes(type);
        }

        #endregion

        #region Data

        //private StreamContainer _archiveFile;                
        //private static object _lockObject = new object();
        //private static object _globalLockObject = new object();

        #endregion

        #region Public Properties

        //public IInternalFileInfoExA ArchiveFile { get { return _archiveFile; } }


        #endregion




    }
#endif
}
