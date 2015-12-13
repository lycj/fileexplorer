using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if !NETFX_CORE
using Microsoft.Win32;
#endif
namespace FileExplorer.WPF.Utils
{
#if !NETFX_CORE
    public static class W32RegistryUtils
    {        
        /// <summary>
        /// Obtain the File and MIME type from registry.
        /// </summary>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static void GetTypeInformation(string ext, out string fileType, out string mimeType)
        {
            fileType = null;
            mimeType = null;

            if (String.IsNullOrEmpty(ext))
            {
                fileType = "File";
                return;
            }

            if (!ext.StartsWith("."))
                throw new ArgumentException("ext");
            
            var extKey = Registry.ClassesRoot.OpenSubKey(ext);
            if (extKey != null)
            {
                object cType = extKey.GetValue("Content Type");
                object fType = extKey.GetValue("");


                mimeType = cType != null ? cType.ToString() : null;
                fileType = fType != null ? fType.ToString() : null;
                if (fileType != null)
                {
                    var fileTypeKey = Registry.ClassesRoot.OpenSubKey(fileType);
                    if (fileTypeKey != null)
                    {
                        object ftType = fileTypeKey.GetValue("");
                        fileType = ftType != null ? ftType.ToString() : fileType;
                    }
                }
            }
        }

        public static string GetFileDescription()
        {
            return "File";
        }

        public static string GetDirectoryDescription()
        {
            var dirKey = Registry.ClassesRoot.OpenSubKey("Directory");
            if (dirKey != null)
            {
                string retVal = dirKey.GetValue("").ToString();
                if (!String.IsNullOrEmpty(retVal))
                    return retVal;
            }
            
            return "File Folder";
        }
    }
#endif
}
