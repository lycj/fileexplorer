using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using QuickZip.IO.COFE;
using System.IO;
using System.Drawing;
using System.Threading;
using QuickZip.UserControls.Logic.Tools.IconExtractor;
using ShellDll;

namespace QuickZip.Converters
{
    public class ExIconExtractor : IconExtractor<FileSystemInfoEx>
    {
        protected static string fileBasedFSFilter = ".zip,.7z,.lha,.lzh,.sqx,.cab,.ace";

        #region Methods

        private bool IsGuidPath(string FullName)
        {
            return FullName.StartsWith("::{");
        }

        protected override Bitmap GetIconInner(FileSystemInfoEx entry, string key, IconSize size)
        {
            if (key.StartsWith("."))
                throw new Exception("ext item is handled by IconExtractor");

            if (entry is FileInfoEx)
            {
                Bitmap retVal = null;

                string ext = PathEx.GetExtension(entry.Name);
                if (IsJpeg(ext))
                {
                    retVal = GetExifThumbnail(entry.FullName);
                }
                if (IsImageIcon(ext))
                    try
                    {
                        retVal = new Bitmap(entry.FullName);
                    }
                    catch { retVal = null; }

                if (retVal != null)
                    return retVal;
            }

            return entry.RequestPIDL(pidl => GetBitmap(size, pidl.Ptr, entry is DirectoryInfoEx, false));
        }

        protected override void GetIconKey(FileSystemInfoEx entry, IconSize size, out string fastKey, out string slowKey)
        {
            string ext = PathEx.GetExtension(entry.Name);
            if (entry is DirectoryInfoEx)
            {
                fastKey = entry.FullName;
                slowKey = entry.FullName;
            }

            else
                if (IsGuidPath(entry.Name))
                {
                    fastKey = entry.FullName;
                    slowKey = entry.FullName;
                }
                else
                    if (IsImageIcon(ext) || IsSpecialIcon(ext))
                    {
                        fastKey = ext;
                        slowKey = entry.FullName;
                    }
                    else
                    {
                        fastKey = slowKey = ext;
                    }

        }

        #endregion
    }
}
