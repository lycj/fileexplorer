using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
#if NETFX_CORE
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;
#else
//using Fesersoft.Hashing;
using System.Security.Cryptography;
using FileExplorer;
#endif

namespace FileExplorer.WPF.Utils
{
    /// <summary>
    /// Stream related Utils.
    /// </summary>
    public static class W32StreamUtils
    {
       

        /// <summary>
        /// Read a stream and return it's MD5, does not reset or close the stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string GetMD5(this Stream stream)
        {
#if NETFX_CORE
            var alg = HashAlgorithmProvider.OpenAlgorithm("MD5");

            IBuffer buff;
            if (stream is MemoryStream)
            {
                buff = WindowsRuntimeBufferExtensions.GetWindowsRuntimeBuffer(stream as MemoryStream);
            }
            else //In case it returned a non-Memory stream.
            {
                MemoryStream ms = new MemoryStream();
                CopyStream(stream, ms);
                buff = WindowsRuntimeBufferExtensions.GetWindowsRuntimeBuffer(ms);
            }
            var hashed = alg.HashData(buff);
            return CryptographicBuffer.EncodeToHexString(hashed);
#else 
            MD5CryptoServiceProvider csp = new MD5CryptoServiceProvider();
            byte[] hash = csp.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
#endif
        }

#if !NETFX_CORE
        public static Stream NewTempStream(out string fileName, string ext)
        {
            if (ext.StartsWith("."))
                ext = ext.TrimStart('.');
            do
            {
                fileName = PathFE.Combine(Path.GetTempPath(), W32StringUtils.RandomString(8) + "." + ext);
            }
            while (File.Exists(fileName));

            return new FileStream(fileName, FileMode.CreateNew);
        }
#endif
    }
}
