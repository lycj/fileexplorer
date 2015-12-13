using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FileExplorer.WPF.Utils;
using FileExplorer.Defines;

namespace FileExplorer
{
    /// <summary>
    /// Path related utils.
    /// </summary>
    public static class PathFE
    {

        /// <summary>
        /// Make temp path for virtual item, this is just relative path, combine with UserProfile.TempPath.
        /// </summary>
        /// <param name="rootParseName"></param>
        /// <param name="relPath"></param>
        /// <returns></returns>
        public static string MakeTempPath(string rootParseName, string relPath)         
        {
            int pKey = rootParseName.ToLower().GetHashCode();
            return PathFE.Combine(pKey.ToString(), PathFE.Combine(PathFE.GetFileName(rootParseName), relPath));
        }

        /// <summary>
        /// Convert volume key (e.g. TEMP) and parseName (Arch3.7z) to a full parsable name (e.g. {TEMP}\arch3.7z) 
        /// </summary>
        /// <param name="listerKey"></param>
        /// <param name="parseName"></param>
        /// <returns></returns>
        public static string FormatFullParseName(string listerKey, string parseName)
        {            
            if (String.IsNullOrEmpty(listerKey))
                return parseName;

            if (parseName.StartsWith(String.Format("{{{0}}}", listerKey))) //directory volume.
                return parseName;

            if (parseName == "\\")
                parseName = "";
            else parseName = parseName.TrimStart('\\');

            return String.Format("{{{0}}}\\{1}", listerKey, parseName).TrimEnd('\\');
        }



        /// <summary>
        /// Convert full parsable name (e.g. {TEMP}\arch3.7z) to volume key (e.g. TEMP) and parseName (Arch3.7z).
        /// </summary>
        /// <param name="fullParsePath"></param>
        /// <param name="key"></param>
        /// <param name="parsePath"></param>
        /// <returns></returns>
        public static bool ParseFullParseName(string fullParsePath, out string key, out string parsePath)
        {
            //if (fullParsePath == "" || fullParsePath.StartsWith("{" + ProfileRootDirectoryLister.ProfileRootListerKey + "}"))
            //{
            //    key = ProfileRootDirectoryLister.ProfileRootListerKey;
            //    parsePath = "";
            //    return true;
            //}

            Regex regex = new Regex(RegexPatterns.ParseDirectoryListerLink);
            Match match = regex.Match(fullParsePath);
            key = "";
            parsePath = null;

            if (match.Success)
            {
                key = match.Groups["key"].Success ? match.Groups["key"].Value : null;
                parsePath = match.Groups["path"].Success ? match.Groups["path"].Value : null;
            }
            return match.Success;
        }

        public static string GetListerKeyFromFullParseName(string fullParsePath)
        {
            string key, parsePath;
            if (ParseFullParseName(fullParsePath, out key, out parsePath))
                return key;
            return null;
        }

        public static string GetParsePathFromFullParseName(string fullParsePath)
        {
            string key, parsePath;

            if (ParseFullParseName(fullParsePath, out key, out parsePath))
                return parsePath;
            return null;
        }

        public static string ConvertSlash(string path, char toSlash = '\\')
        {
            if (toSlash != '\\' && toSlash != '/')
                throw new ArgumentException("toSlash");

            if (toSlash == '\\')
                return path.Replace('/', '\\');
            else return path.Replace('\\', '/');
        }

        public static string GetDirectoryName(string path)
        {            
            if (path.EndsWith("\\"))
                path = path.Substring(0, path.Length - 1); //Remove ending slash.

            int idx = path.LastIndexOf('\\');
            if (idx == -1)
                return "";
            return path.Substring(0, idx);
        }
        
        public static string GetFileName(string path)
        {
            int idx = path.LastIndexOf('\\');
            if (idx == -1)
                return path;
            return path.Substring(idx + 1);
        }       

        public static string GetExtension(string path)
        {
            return System.IO.Path.GetExtension(path);
        }

        public static string ChangeExtension(string path, string extension)
        {
            return System.IO.Path.ChangeExtension(path, extension);
        }

        //20: Fixed crash when no extension in path.
        public static string RemoveExtension(string path)
        {
            if (!String.IsNullOrEmpty(GetExtension(path)))
                return path.Replace(GetExtension(path), "");
            return path;
        }

        public static string SantizeFileName(string fileName)
        {
            string pattern = RegexPatterns.SantizeFileNamePattern;
            return new Regex(pattern).Replace(fileName, "");
        }

        ///// <summary>
        ///// Combine two path strings
        ///// </summary>
        ///// <param name="path1"></param>
        ///// <param name="path2"></param>
        ///// <returns></returns>
        //public static string Combine(string path1, string path2)
        //{
        //    return System.IO.Path.Combine(path1, path2);
        //}


        /// <summary>
        /// Combine two path strings
        /// </summary>
        /// <param name="path1"></param>
        /// <param name="path2"></param>
        /// <returns></returns>
        public static string Combine(string path1, params string[] paths)
        {
            string retVal = path1;

            foreach (var p in paths)
            {
                retVal = retVal.TrimEnd('\\') + "\\" + p.TrimStart('\\');
            }


            return retVal.TrimStart('\\');
        }


        #region MatchFileMask

        private static string constructFileMaskRegexPattern(string fileMask, bool forceSlashCheck)
        {
            if (!forceSlashCheck)
            {
                return '^' +
                Regex.Escape(fileMask.Replace(".", "__DOT__")
                                .Replace("*", "__STAR__")
                                .Replace("?", "__QM__"))
                    .Replace("__DOT__", "[.]")
                    .Replace("__STAR__", ".*")
                    .Replace("__QM__", ".")
                + '$';
            }
            else
            {
                return '^' +
                 Regex.Escape(fileMask.Replace(".", "__DOT__")
                                 .Replace("\\", "__SLASH__")
                                 .Replace("**", "__DOUBLESTAR__")
                                 .Replace("*", "__STAR__")
                                 .Replace("#", "__VARIABLE__")
                                 .Replace("(", "__OPENQUOTE__")
                                 .Replace(")", "__CLOSEQUOTE__")
                                 .Replace("?", "__QM__"))
                     .Replace("__DOT__", "[.]")
                     .Replace("__DOUBLESTAR__", ".*")
                     .Replace("__STAR__", "[^\\\\]*")
                     .Replace("__SLASH__", "[\\\\]")
                     .Replace("__VARIABLE__", "?")
                     .Replace("__OPENQUOTE__", "(")
                     .Replace("__CLOSEQUOTE__", ")")
                     .Replace("__QM__", ".")
                 + '$';
            }
        }

        public static bool MatchFileMask(string fileName, string fileMask, bool forceSlashCheck, out Match match)
        {
            string pattern = constructFileMaskRegexPattern(fileMask, forceSlashCheck);
            match = new Regex(pattern, RegexOptions.IgnoreCase).Match(fileName);
            return match.Success;
        }

        //http://stackoverflow.com/questions/725341/c-file-mask
        /// <summary>
        /// Return whether filename match fileMask ( * and ? supported)
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileMask"></param>
        /// <returns></returns>
        public static bool MatchFileMask(string fileName, string fileMask, bool forceSlashCheck)
        {
            if (fileName.Equals(fileMask, StringComparison.CurrentCultureIgnoreCase))
                return true;
            if (fileMask == "*.*" || fileMask == "*" && !forceSlashCheck)
                return true;

            string pattern = constructFileMaskRegexPattern(fileMask, forceSlashCheck);

            return new Regex(pattern, RegexOptions.IgnoreCase).IsMatch(fileName);
        }

        public static bool MatchFileMask(string fileName, string fileMask)
        {
            return MatchFileMask(fileName, fileMask, false);
        }

        public static bool MatchFileMasks(string fileName, string fileMasks)
        {
            string[] fileMaskList = fileMasks.Split(new char[] { ',', ';' });
            foreach (string fileMask in fileMaskList)
                if (MatchFileMask(fileName, fileMask))
                    return true;
            return false;
        }

        #endregion
    }
}
