using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Threading;
using System.Windows;
using System.Threading;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.IO;
using vbAccelerator.Components.ImageList;
using System.Diagnostics;
using System.Windows.Resources;

namespace QuickZip.IO.COFE.UserControls
{

    [ValueConversion(typeof(IFileSystemInfoExA), typeof(ImageSource))]
    public class ExAToIconConverter : IconConverterBase
    {
        private static string imageFilter = ".jpg,.jpeg,.png,.gif,.bmp,.tiff";
        private static string exeFilter = ".exe";
        private static string fileBasedFSFilter = ".zip,.7z";
        private static string tempPath = System.IO.Path.GetTempPath();

        #region Win32api
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        [StructLayout(LayoutKind.Sequential)]
        internal struct SHFILEINFO
        {
            public IntPtr hIcon;
            public IntPtr iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };

        internal const uint SHGFI_ICON = 0x100;
        internal const uint SHGFI_TYPENAME = 0x400;
        internal const uint SHGFI_PIDL = 0x000000008;
        internal const uint SHGFI_LARGEICON = 0x0; // 'Large icon
        internal const uint SHGFI_SMALLICON = 0x1; // 'Small icon
        internal const uint SHGFI_SYSICONINDEX = 16384;
        internal const uint SHGFI_USEFILEATTRIBUTES = 16;

        // <summary>
        /// Get Icons that are associated with files.
        /// To use it, use (System.Drawing.Icon myIcon = System.Drawing.Icon.FromHandle(shinfo.hIcon));
        /// hImgSmall = SHGetFileInfo(fName, 0, ref shinfo,(uint)Marshal.SizeOf(shinfo),Win32.SHGFI_ICON |Win32.SHGFI_SMALLICON);
        /// </summary>
        [DllImport("shell32.dll")]
        internal static extern IntPtr SHGetFileInfo(IntPtr pszPath, uint dwFileAttributes,
                                                  ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);
        [DllImport("shell32.dll")]
        internal static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes,
                                                  ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

        internal static Bitmap GetFileIcon(IntPtr fileName, IconSize size)
        {
            SHFILEINFO shinfo = new SHFILEINFO();

            uint flags = SHGFI_SYSICONINDEX | SHGFI_USEFILEATTRIBUTES | SHGFI_PIDL;
            if (size == IconSize.small)
                flags = flags | SHGFI_ICON | SHGFI_SMALLICON;
            else flags = flags | SHGFI_ICON;

            SHGetFileInfo(fileName, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), flags);
            return shinfo.hIcon != IntPtr.Zero ? Icon.FromHandle(shinfo.hIcon).ToBitmap() : new Bitmap(1, 1);
        }

        // <summary>
        /// Return large file icon of the specified file.
        /// </summary>
        internal static Bitmap GetFileIcon(string fileName, IconSize size)
        {
            if (fileName.StartsWith("."))
                fileName = "AAA" + fileName;

            SHFILEINFO shinfo = new SHFILEINFO();

            uint flags = SHGFI_SYSICONINDEX;
            if (fileName.IndexOf(":") == -1)
                flags = flags | SHGFI_USEFILEATTRIBUTES;
            if (size == IconSize.small)
                flags = flags | SHGFI_ICON | SHGFI_SMALLICON;
            else flags = flags | SHGFI_ICON;

            SHGetFileInfo(fileName, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), flags);
            return shinfo.hIcon != IntPtr.Zero ? Icon.FromHandle(shinfo.hIcon).ToBitmap() : new Bitmap(1, 1);
        }

        #endregion
        

        public static bool IsSpecialFolder(string path)
        {
            return path.EndsWith(":\\") || path.EndsWith(":") || 
                (path.StartsWith("::") && path.Split('\\').Count() <= 2);
        }

        protected Bitmap GetFileBasedFSBitmap(string ext, IconSize size)
        {
            string lookup = UCUtils.GetProgramPath();
            Bitmap folderBitmap = KeyToBitmap(lookup, size);
            if (ext != "")
            {
                ext = ext.Substring(0, 1).ToUpper() + ext.Substring(1).ToLower();

                using (Graphics g = Graphics.FromImage(folderBitmap))
                {
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                    Font font = new Font("Comic Sans MS", folderBitmap.Width / 5, System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic);
                    float height = g.MeasureString(ext, font).Height;
                    float rightOffset = folderBitmap.Width / 5;

                    if (size == IconSize.small)
                    {
                        font = new Font("Arial", 5, System.Drawing.FontStyle.Bold);
                        height = g.MeasureString(ext, font).Height;
                        rightOffset = 0;
                    }


                    g.DrawString(ext, font,
                                System.Drawing.Brushes.Black,
                                new RectangleF(0, folderBitmap.Height - height, folderBitmap.Width - rightOffset, height),
                                new StringFormat(StringFormatFlags.DirectionRightToLeft));

                }
            }

            return folderBitmap;
        }

        protected override void ValueToKey(object value, out string key, out string fastKey, out bool delayLoading)
        {
            delayLoading = false;
            key = "";
            fastKey = "";

            if (value is IFileSystemInfoExA)
            {
                IFileSystemInfoExA entry = value as IFileSystemInfoExA;

                if (value is IFileInfoExA)
                {
                    fastKey = PathEx.GetExtension(entry.Name);
                    if (imageFilter.IndexOf(fastKey) == -1)
                        key = entry.ParseName;
                    else key = fastKey;
                    delayLoading = key != fastKey;
                }
                else //IDirectoryInfoExA
                {
                    IDirectoryInfoExA dirEntry = entry as IDirectoryInfoExA;
                    if (dirEntry.IsFileBasedFS)
                        fastKey = key = PathEx.GetExtension(entry.Name);
                    else
                    {
                        fastKey = tempPath;
                        key = fastKey;
                        if (IsSpecialFolder(entry.ParseName))
                        {
                            key = entry.ParseName;
                            delayLoading = true;
                        }
                    }
                }

            }

        }


        private static SysImageList _imgList = new SysImageList(SysImageListSize.jumbo);
        private static Bitmap loadJumbo(string lookup, bool forceLoadFromDisk)
        {
            _imgList.ImageListSize = isVistaUp() ? SysImageListSize.jumbo : SysImageListSize.extraLargeIcons;

            Icon icon = _imgList.Icon(_imgList.IconIndex(lookup, forceLoadFromDisk));
            Bitmap bitmap = icon.ToBitmap();
            icon.Dispose();

            System.Drawing.Color empty = System.Drawing.Color.FromArgb(0, 0, 0, 0);

            if (bitmap.Width < 256)
                bitmap = resizeImage(bitmap, new System.Drawing.Size(256, 256), 0);
            else if (bitmap.GetPixel(100, 100) == empty && bitmap.GetPixel(200, 200) == empty && bitmap.GetPixel(200, 200) == empty)
            {
                _imgList.ImageListSize = SysImageListSize.largeIcons;
                bitmap = resizeJumbo(_imgList.Icon(_imgList.IconIndex(lookup)).ToBitmap(), new System.Drawing.Size(200, 200), 5);
            }

            return bitmap;
        }

        protected override Bitmap KeyToBitmap(string key, IconSize size)
        {
            bool isDiskFolder = key.EndsWith(":\\");

            if (IsSpecialFolder(key))
            {
                return GetFileIcon(FileSystemInfoExA.FromStringParse(key).PIDL.Ptr, size);
            }
            else
                if (fileBasedFSFilter.IndexOf(key) != -1)
                    return GetFileBasedFSBitmap(key, size);
                else
                    if (key.StartsWith(".")) //Extension 
                    {
                        return GetFileIcon(key, size);
                    }
                    else
                        switch (size)
                        {
                            case IconSize.thumbnail:
                            case IconSize.jumbo:
                                return loadJumbo(key, isDiskFolder);
                            case IconSize.extraLarge:
                                _imgList.ImageListSize = SysImageListSize.extraLargeIcons;
                                return _imgList.Icon(_imgList.IconIndex(key, isDiskFolder)).ToBitmap();
                            //case IconSize.large :
                            //    _imgList.ImageListSize = SysImageListSize.largeIcons;
                            //    return _imgList.Icon(_imgList.IconIndex(key, isDiskFolder)).ToBitmap();
                            //case IconSize.small :
                            //    _imgList.ImageListSize = SysImageListSize.smallIcons;
                            //    return _imgList.Icon(_imgList.IconIndex(key, isDiskFolder)).ToBitmap();
                            default:
                                try
                                {
                                    return GetFileIcon(key, size);
                                }
                                catch { return KeyToBitmap(UCUtils.GetProgramPath(), size); }
                        }
        }
    }

}
