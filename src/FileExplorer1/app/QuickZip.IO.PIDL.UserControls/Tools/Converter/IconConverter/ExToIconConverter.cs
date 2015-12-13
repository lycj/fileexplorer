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
using vbAccelerator.Components.Shell;
using QuickZip.UserControls;
using QuickZip.Converters;
using ShellDll;
using QuickZip.IO.PIDL.UserControls.ViewModel;
using System.IO.Tools;

namespace QuickZip.IO.PIDL.UserControls
{

    [ValueConversion(typeof(FileSystemInfoEx), typeof(ImageSource))]
    public class ExToIconConverter : IconConverterBase
    {
        //protected static string imageFilter = ".jpg,.jpeg,.png,.gif,.bmp,.tiff";
        protected static string specialExtFilter = ".exe,.lnk";
        protected static string fileBasedFSFilter = ".zip,.7z,.lha,.lzh,.sqx,.cab,.ace";
        protected static string tempPath = System.IO.Path.GetTempPath();

        public ExToIconConverter()
        {
            if (!Directory.Exists(tempPath))
                Directory.CreateDirectory(tempPath);
        }

        public static bool IsSpecialFolder(string path)
        {
            return path.EndsWith(":\\") || path.EndsWith(":") ||
                (path.StartsWith("::") && path.Split('\\').Count() <= 2) ||
                (path.StartsWith(DirectoryInfoEx.CurrentUserDirectory.FullName) && PathEx.FullNameToGuidName(path).Split('\\').Count() <= 2);
        }

        protected Bitmap GetFileBasedFSBitmap(string ext, IconSize size)
        {
            string lookup = tempPath;
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

            if (value is FileSystemInfoEx)
            {
                FileSystemInfoEx entry = value as FileSystemInfoEx;

                if (value is FileInfoEx)
                {
                    fastKey = PathEx.GetExtension(entry.Name);
                    if (imageFilter.IndexOf(fastKey, StringComparison.InvariantCultureIgnoreCase) != -1 ||
                        specialExtFilter.Split(',').Contains(fastKey))
                        key = entry.FullName;
                    else key = fastKey;
                    delayLoading = key != fastKey;

                }
                else //DirectoryInfoEx
                {
                    DirectoryInfoEx dirEntry = entry as DirectoryInfoEx;
                    fastKey = tempPath;
                    key = fastKey;
                    if (IsSpecialFolder(entry.FullName))
                    {
                        key = entry.FullName;
                        delayLoading = true;
                    }

                }

            }

        }

        //private static SysImageList _imgListJumbo = new SysImageList(SysImageListSize.jumbo);
        //private static SysImageList _imgListXL = new SysImageList(SysImageListSize.extraLargeIcons);

        private static Bitmap loadThumbnail(string lookup, bool forceLoadFromDisk)
        {
            string ext = PathEx.GetExtension(lookup).ToLower();
            if (!String.IsNullOrEmpty(ext) && imageFilter.IndexOf(ext) != -1)
                try
                {
                    return new Bitmap(FileSystemInfoEx.FromString(lookup).FullName);
                }
                catch { return new Bitmap(1, 1); }
            else return GetFileIcon(lookup, IconSize.large);
        }

        //private static Bitmap loadJumbo(string lookup, bool forceLoadFromDisk, IconSize size)
        //{
        //    SysImageList _imgList = isVistaUp() && ((size == IconSize.thumbnail) || (size == IconSize.jumbo)) ? _imgListJumbo : _imgListXL;

        //    try
        //    {
        //        Icon icon = _imgList.Icon(_imgList.IconIndex(lookup, forceLoadFromDisk));
        //        Bitmap bitmap = icon.ToBitmap();
        //        icon.Dispose();

        //        System.Drawing.Color empty = System.Drawing.Color.FromArgb(0, 0, 0, 0);

        //        if (bitmap.Width < 256)
        //            bitmap = resizeImage(bitmap, new System.Drawing.Size(256, 256), 0);
        //        else if (bitmap.GetPixel(100, 100) == empty && bitmap.GetPixel(200, 200) == empty && bitmap.GetPixel(200, 200) == empty)
        //        {
        //            _imgList.ImageListSize = SysImageListSize.largeIcons;
        //            bitmap = resizeJumbo(_imgList.Icon(_imgList.IconIndex(lookup)).ToBitmap(), new System.Drawing.Size(200, 200), 5);
        //        }

        //        return bitmap;
        //    }
        //    catch
        //    {
        //        return GetFileIcon(lookup, IconSize.large);
        //    }

        //}

        protected override Bitmap KeyToBitmap(string key, IconSize size)
        {
            try
            {
                bool isDiskFolder = key.EndsWith(":\\");
                bool isTemp = key.Equals(tempPath);

                string ext = PathEx.GetExtension(key).ToLower();

                if (!ext.StartsWith("."))
                {
                    if (specialExtFilter.Split(',').Contains(key))
                    {
                        switch (ext)
                        {
                            case ".exe":
                                ShellDll.PIDL pidlLookup = FileSystemInfoEx.FromString(key).PIDL;
                                try
                                {
                                    return GetFileIcon(pidlLookup.Ptr, size);
                                }
                                finally { if (pidlLookup != null) pidlLookup.Free(); }

                            case ".lnk":
                                using (ShellLink sl = new ShellLink(key))
                                    switch (size)
                                    {
                                        case IconSize.small:
                                            return sl.SmallIcon.ToBitmap();
                                        default: return sl.LargeIcon.ToBitmap();
                                    }
                        }
                    }
                }



                if (fileBasedFSFilter.Split(',').Contains(key))
                    return GetFileBasedFSBitmap(key, size);
                else
                    if (key == "" || key.StartsWith(".")) //Extension 
                    {
                        return GetFileIcon(key, size);
                    }
                    else
                        if (IsSpecialFolder(key))
                        {
                            ShellDll.PIDL pidlLookup = FileSystemInfoEx.FromString(key).PIDL;
                            try
                            {
                                return GetFileIcon(pidlLookup.Ptr, size);
                            }
                            finally { if (pidlLookup != null) pidlLookup.Free(); }
                        }
                        else
                            switch (size)
                            {
                                case IconSize.thumbnail:
                                    return loadThumbnail(key, isDiskFolder || isTemp);
                                //case IconSize.jumbo:
                                //case IconSize.extraLarge:
                                //    return loadJumbo(key, isDiskFolder || isTemp, size);
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
            catch { return new Bitmap(1, 1); }
        }
    }

}
