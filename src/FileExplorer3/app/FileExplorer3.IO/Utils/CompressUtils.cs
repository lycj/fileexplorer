using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using FileExplorer.WPF.Utils;
using FileExplorer.Models;

namespace FileExplorer.WPF.Utils
{
    internal static class CompressionUtils
    {
        #region EmptyArchiveContent
        internal static string archiveFilter = ".zip,.7z,.7zip,.tar,.tgz,.gz,.z,.bzip";        
        internal static string archiveWriteFilter = ".zip, .7z";
        internal static byte[] EmptyZip = new byte[] { 0x50, 0x4B, 0x05, 0x06, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00 }; //PK
        internal static byte[] Empty7z = new byte[] {0x37,0x7A,0xBC,0xAF,0x27,0x1C,0x00,0x02,0x8D,0x9B,0xD5,0x0F,0x00,0x00,0x00,0x00,
                                         0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00 }; //7z

        #endregion       

        #region Methods

        public static bool IsZip(string archiveName)
        {
            string ext = PathFE.GetExtension(archiveName).ToLower();
            return ext == ".zip";
        }

        public static bool IsArchive(string archiveName)
        {
            string ext = PathFE.GetExtension(archiveName).ToLower() + ",";
            return (ext != ",") && (archiveFilter + ",").IndexOf(ext) != -1;
        }

        public static bool IsArchiveWritable(string archiveName)
        {
            string ext = PathFE.GetExtension(archiveName).ToLower() + ",";
            return (ext != ",") && (archiveWriteFilter + ",").IndexOf(ext) != -1;
        }


        //#region guessTempPath
        ////For finding cache in ParseDisplayName only, not necessary accurate.
        //internal static string guessTempPath(string parsePath, out string relPath)
        //{
        //    relPath = "";
        //    parsePath = IOTools.ExpandPath(parsePath);
        //    if (FileEx.Exists(parsePath) || parsePath.EndsWith(":")  //Drive
        //        || DirectoryEx.Exists(PathEx.GetDirectoryName(parsePath)))
        //        return null;

        //    string fullParseName = PathEx.GetDirectoryName(parsePath);
        //    relPath = Path.GetFileName(parsePath);
        //    while (!SevenZipUtils.IsArchive(fullParseName))
        //    {
        //        if (!COFETools.PathShiftLeft(ref fullParseName, ref relPath))
        //            return null;
        //    }
        //    if (SevenZipUtils.IsArchive(fullParseName))
        //        return COFETools.makeTempPath(fullParseName,
        //            Path.GetFileName(fullParseName) + "\\" + relPath);
        //    else return null;
        //}
        //#endregion

        #region getSubdirectoryList


        internal static List<string> getSubdirectoryList(List<string> dirList, string directory)
        {
            List<string> retVal = new List<string>();

            foreach (string s in dirList)
            {
                if (directory != "")
                    directory = PathHelper.Disk.AppendFrontSlash(PathHelper.Disk.AppendSlash(directory));

                if (s.IndexOf(directory) == 0)
                {
                    string subdir = PathHelper.Disk.ExtractFirstDir(s, directory.Length);

                    if (retVal.IndexOf(subdir) == -1)
                        retVal.Add(subdir);
                }
            }

            return retVal;
        }

        #endregion

        
        //public static string getListPattern(IInternalFileSystemInfoExA item)
        //{
        //    string folder = "";

        //    var root = item.getDirectoryRoot(true);

        //    if (root.Equals(item))
        //        folder = "";
        //    else
        //        folder = item.GetRelativePath();

        //    if (!folder.EndsWith("\\") && !string.IsNullOrEmpty(folder))
        //        folder += "\\";

        //    return String.Format(RegexPatterns.SevenZipListPattern, Regex.Escape(folder));
        //}

        

        #endregion
    }
}
