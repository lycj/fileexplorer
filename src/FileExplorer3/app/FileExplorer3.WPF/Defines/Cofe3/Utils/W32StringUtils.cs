using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
#if NETFX_CORE
using Windows.Security.Cryptography.Core;
using System.Runtime.InteropServices.WindowsRuntime;
#endif

namespace FileExplorer.WPF.Utils
{
    /// <summary>
    /// String related utils.
    /// </summary>
    public static class W32StringUtils
    {

        /// <summary>
        /// Convert StringToStream.
        /// http://www.csharp411.com/c-convert-string-to-stream-and-stream-to-string/
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Stream StringToStream(string s)
        {
            byte[] byteArray = Encoding.Unicode.GetBytes( s );
            return new MemoryStream(byteArray);
        }        

        /// <summary>
        /// Generate Guid from string, taken from 
        /// http://geekswithblogs.net/EltonStoneman/archive/2008/06/26/generating-deterministic-guids.aspx
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Guid GetDeterministicGuid(string input)
        {
            byte[] inputBytes = Encoding.Unicode.GetBytes(input);
            //use MD5 hash to get a 16-byte hash of the string:
#if NETFX_CORE            
            HashAlgorithmProvider objAlgProv = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
            byte[] hashBytes = objAlgProv.HashData(inputBytes.AsBuffer()).ToArray();
#else
            var provider = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] hashBytes = provider.ComputeHash(inputBytes);
#endif
            //generate a guid from the hash:
            Guid hashGuid = new Guid(hashBytes);
            return hashGuid;
        }

        public static Guid GetHashGuid(this string input)
        {
            return GetDeterministicGuid(input);
        }

        //public static string FirstCharToLowercase(this string input)
        //{
        //    if (input.Length > 0)
        //        return input.Substring(0, 1).ToLower() + input.Substring(1);
        //    return input;
        //}

        //public static string FirstCharToUppercase(this string input)
        //{
        //    if (input.Length > 0)
        //        return input.Substring(0, 1).ToUpper() + input.Substring(1);
        //    return input;
        //}


        //public static string DicToParamString(Dictionary<string, string> dic)
        //{
        //    return CofeServices.ParamParser.DictionaryToString(dic);
        //}

        //public static Dictionary<string, string> ParamStringToDic(string fullParamString)
        //{
        //    return CofeServices.ParamParser.StringToDictionary(fullParamString);
        //}


        #region Match File Masks
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
            if (fileName.Equals(fileMask, StringComparison.OrdinalIgnoreCase))
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

        public static bool MatchFileMasks(string fileName, string fileMasks, string appendFileMask = "")
        {
            string[] fileMaskList = fileMasks.Split(new char[] { ',', ';' });
            foreach (string fileMask in fileMaskList)
                if (MatchFileMask(fileName, fileMask + appendFileMask))
                    return true;
            return false;
        }
        #endregion

        ////http://www.codeproject.com/KB/string/fastestcscaseinsstringrep.aspx        
        //static public string Replace(this string original, string pattern, string replacement, StringComparison comparisonType)
        //{
        //    return Replace(original, pattern, replacement, comparisonType, -1);
        //}

        //static public string Replace(this string original, string pattern, string replacement, StringComparison comparisonType, int stringBuilderInitialSize)
        //{
        //    if (original == null)
        //    {
        //        return null;
        //    }

        //    if (String.IsNullOrEmpty(pattern))
        //    {
        //        return original;
        //    }


        //    int posCurrent = 0;
        //    int lenPattern = pattern.Length;
        //    int idxNext = original.IndexOf(pattern, comparisonType);
        //    StringBuilder result = new StringBuilder(stringBuilderInitialSize < 0 ? Math.Min(4096, original.Length) : stringBuilderInitialSize);

        //    while (idxNext >= 0)
        //    {
        //        result.Append(original, posCurrent, idxNext - posCurrent);
        //        result.Append(replacement);

        //        posCurrent = idxNext + lenPattern;

        //        idxNext = original.IndexOf(pattern, posCurrent, comparisonType);
        //    }

        //    result.Append(original, posCurrent, original.Length - posCurrent);

        //    return result.ToString();
        //}

        #region Random string
        //http://stackoverflow.com/questions/1122483/c-sharp-random-string-generator
        public static Random random = new Random((int)(DateTime.Now.Ticks % Int32.MaxValue));//thanks to McAden
        public static string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }
        #endregion


        //public static string ConvertToHex(uint input)
        //{
        //    if (input == 0)
        //        return "00000000";
        //    else return String.Format("{0:x2}", input).ToUpper();
        //}
    }


    /// <summary>
    /// StringUtils generic
    /// </summary>
    /// <typeparam name="T">Target interface type, e.g. IFileSystemInfo, IEntryFilter</typeparam>
    public static class StringUtils<T> 
    {

        #region Methods

        /// <summary>
        /// Get a Guid based on the input string, unlike non-generic version, this is specialized for a type of entry.
        /// GuidGeneratorFunc must be set or standard GetDeterministicGuid() would be called.        
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Guid GetDeterministicGuid(string input)
        {
            if (_guidGenFunc != null)
                return _guidGenFunc(input);
            return W32StringUtils.GetDeterministicGuid(input);
        }
        #endregion

        #region Data
        private static Func<string, Guid> _guidGenFunc = null;
        #endregion

        #region Public Properties

        /// <summary>
        /// Not necessary generation, for database, it query the table to obtain it's key.
        /// </summary>
        public static Func<string, Guid> GuidGeneratorFunc
        {
            get { return _guidGenFunc; }
            set
            {
                if (_guidGenFunc != null) throw new Exception("This property can only set once.");
                if (value == null) throw new ArgumentException("Null is not allowed.");
                _guidGenFunc = value;
            }
        }
        #endregion

    }
}
