using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace FileExplorer.WPF.Utils
{
    public static class ShellUtils
    {
        //By Richard Moss - http://cyotek.com/blog/mime-types-and-file-extensions

        /// <summary>
        /// Convert extension (e.g. txt) from mimetype  from MimeType (e.g. text/plain)
        /// using registry (HKEY_CLASSES_ROOT\MIME\Database\Content Type\)
        /// </summary>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        public static string MIMEType2Extension(string mimeType)
        {
            string result;
            RegistryKey key;
            object value;

            key = Registry.ClassesRoot.OpenSubKey(@"MIME\Database\Content Type\" + mimeType, false);
            value = key != null ? key.GetValue("Extension", null) : null;
            result = value != null ? value.ToString() : string.Empty;

            return result;
        }


        public static string Extension2MIMEType(string extension)
        {
            string result;
            RegistryKey key;
            object value;

            if (!extension.StartsWith("."))
                extension = "." + extension;

            key = Registry.ClassesRoot.OpenSubKey(extension, false);
            value = key != null ? key.GetValue("Content Type", null) : null;
            result = value != null ? value.ToString() : string.Empty;

            return result;
        }
    }


}
