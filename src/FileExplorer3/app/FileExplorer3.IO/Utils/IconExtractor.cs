using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using QuickZip.UserControls;
using System.Threading;
using System.IO.Tools;
using QuickZip.Converters;

namespace QuickZip.UserControls.Logic.Tools.IconExtractor
{
    public class IconExtractor
    {
        public static string ImageFilter = ".jpg,.jpeg,.png,.gif,.bmp,.tiff";
        public static string SpecialFilter = ".exe,.lnk";
        protected static string tempPath = System.IO.Path.GetTempPath();

        //private static Dictionary<Type, IconExtractor> _iconExtractorDic = new Dictionary<Type, IconExtractor>();
        //public static void RegisterIconExtractor<T>(IconExtractor<T> iconExtractor)
        //{
        //    if (!_iconExtractorDic.ContainsKey(typeof(T)))
        //        lock (_iconExtractorDic)
        //            _iconExtractorDic.Add(typeof(T), iconExtractor);
        //}

        //public static IconExtractor<T> GetIconExtractor<T>()
        //{
        //    return (IconExtractor<T>)_iconExtractorDic[typeof(T)];
        //}

        public IconExtractor()
        {
            //Debug.WriteLine("AAA");



        }

        #region Win32api
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        protected static extern bool DeleteObject(IntPtr hObject);

        [StructLayout(LayoutKind.Sequential)]
        protected struct SHFILEINFO
        {
            public IntPtr hIcon;
            public IntPtr iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };

        [DllImport("User32.dll")]
        public static extern int DestroyIcon(IntPtr hIcon);

        protected const uint SHGFI_ICON = 0x100;
        protected const uint SHGFI_TYPENAME = 0x400;
        protected const uint SHGFI_PIDL = 0x000000008;
        protected const uint SHGFI_LARGEICON = 0x0; // 'Large icon
        protected const uint SHGFI_SMALLICON = 0x1; // 'Small icon
        protected const uint SHGFI_SYSICONINDEX = 16384;
        protected const uint SHGFI_USEFILEATTRIBUTES = 16;

        // <summary>
        /// Get Icons that are associated with files.
        /// To use it, use (System.Drawing.Icon myIcon = System.Drawing.Icon.FromHandle(shinfo.hIcon));
        /// hImgSmall = SHGetFileInfo(fName, 0, ref shinfo,(uint)Marshal.SizeOf(shinfo),Win32.SHGFI_ICON |Win32.SHGFI_SMALLICON);
        /// </summary>
        [DllImport("shell32.dll")]
        protected static extern IntPtr SHGetFileInfo(IntPtr pszPath, uint dwFileAttributes,
                                                  ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);
        [DllImport("shell32.dll")]
        protected static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes,
                                                  ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

        #endregion

        static SystemImageListCollection sysImgList = new SystemImageListCollection();
        static ReaderWriterLock sysImgListLock = new ReaderWriterLock();
        static TimeSpan lockWaitTime = TimeSpan.FromSeconds(5);

        #region Public Properties


        #endregion

        #region Static Methods

        #region IconSize Utils
        public static System.Drawing.Size IconSizeToSize(IconSize size)
        {
            switch (size)
            {
                case IconSize.thumbnail: return new System.Drawing.Size(128, 128);
                case IconSize.jumbo: return new System.Drawing.Size(80, 80);
                case IconSize.extraLarge: return new System.Drawing.Size(64, 64);
                case IconSize.large: return new System.Drawing.Size(32, 32);
                default: return new System.Drawing.Size(16, 16);
            }
        }

        public static IconSize SizeToIconSize(int size)
        {
            if (size <= 16) return IconSize.small;
            else if (size <= 32) return IconSize.large;
            else if (size <= 47) return IconSize.extraLarge;
            //else if (iconSize <= 72) return IconSize.jumbo;
            else return IconSize.thumbnail;
        }

        #endregion

        /// <summary>
        /// Return Exif thumbnail for jpegs.
        /// </summary>
        /// <param name="fileName">Jpeg filename</param>
        /// <returns>Bitmap, null if anything goes wrong</returns>
        protected static Bitmap GetExifThumbnail(string fileName)
        {
            string ext = Path.GetExtension(fileName).ToLower();
            if (IsJpeg(ext) && File.Exists(fileName))
            {
                ExifLib.ExifReader reader = new ExifLib.ExifReader(fileName);
                var bitmapBytes = reader.GetJpegThumbnailBytes();
                if (bitmapBytes != null && bitmapBytes.Length > 0)
                    return new Bitmap(new MemoryStream(bitmapBytes));
            }
            return null;
        }

        public Bitmap GetThumbnail(string path, IconSize size)
        {
            if (path != null)
                if (File.Exists(path))
                {
                    Bitmap thumbnail = ImageExtractor.ExtractImage(path, IconSizeToSize(size), false);
                    if (thumbnail == null)
                        return thumbnail;
                }
            return null;
        }

        protected static bool IsJpeg(string ext)
        {
            if (!(ext.StartsWith(".")))
                ext = Path.GetExtension(ext);
            if (String.IsNullOrEmpty(ext)) return false;

            ext = ext.ToLower();
            return ext == ".jpg" || ext == ".jpeg";
        }

        protected static bool IsSpecialIcon(string ext)
        {
            if (!(ext.StartsWith(".")))
                ext = Path.GetExtension(ext);
            if (String.IsNullOrEmpty(ext)) return false;

            if (SpecialFilter.IndexOf(ext.ToLower()) != -1)
                return true;
            return false;
        }

        protected static bool IsImageIcon(string ext)
        {
            if (!(ext.StartsWith(".")))
                ext = Path.GetExtension(ext);
            if (String.IsNullOrEmpty(ext)) return false;

            if (ImageFilter.IndexOf(ext.ToLower()) != -1)
                return true;
            return false;
        }
        #endregion

        #region Methods

        public Bitmap GetBitmap(IconSize size, IntPtr ptr, bool isDirectory, bool forceLoad)
        {
            Bitmap retVal = null;



            using (var imgList = new SystemImageList(size))
                retVal = imgList[ptr, isDirectory, forceLoad];

            //sysImgListLock.AcquireReaderLock(1000);

            //try
            //{
            //    if (size != sysImgList.CurrentImageListSize)
            //    {
            //        LockCookie lockCookie = sysImgListLock.UpgradeToWriterLock(lockWaitTime);
            //        try
            //        {
            //            SystemImageList imgList = sysImgList[size];
            //            retVal = imgList[ptr, isDirectory, forceLoad];
            //        }
            //        finally
            //        {
            //            sysImgListLock.DowngradeFromWriterLock(ref lockCookie);
            //        }
            //    }
            //    else
            //    {
            //        retVal = sysImgList[size][ptr, isDirectory, forceLoad];
            //    }
            //}
            //finally { sysImgListLock.ReleaseReaderLock(); }


            return retVal;
        }

        public Bitmap GetBitmap(IconSize size, string fileName, bool isDirectory, bool forceLoad)
        {

            Bitmap retVal = null;

            using (var imgList = new SystemImageList(size))
                retVal = imgList[fileName, isDirectory, forceLoad];

            //sysImgListLock.AcquireReaderLock(1000);
            //try
            //{
            //    if (!sysImgList.IsImageListInited || size != sysImgList.CurrentImageListSize)
            //    {
            //        LockCookie lockCookie = sysImgListLock.UpgradeToWriterLock(lockWaitTime);
            //        try
            //        {
            //            SystemImageList imgList = sysImgList[size];
            //            retVal = imgList[fileName, isDirectory, forceLoad];
            //        }
            //        finally
            //        {
            //            sysImgListLock.DowngradeFromWriterLock(ref lockCookie);
            //        }
            //    }
            //    else
            //    {
            //        retVal = sysImgList[size][fileName, isDirectory, forceLoad];
            //    }
            //}
            //finally { sysImgListLock.ReleaseReaderLock(); }

            return retVal;
        }

        public Bitmap GetFileBasedFSBitmap(string ext, IconSize size)
        {
            string lookup = tempPath;
            Bitmap folderBitmap = GetGenericIcon(lookup, size, true);
            if (ext != "")
            {
                ext = ext.Substring(0, 1).ToUpper() + ext.Substring(1).ToLower();

                using (Graphics g = Graphics.FromImage(folderBitmap))
                {
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                    Font font = new Font("Comic Sans MS", Math.Max(folderBitmap.Width / 5, 1), System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic);
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

        protected Bitmap GetGenericIcon(string fullPathOrExt, IconSize size, bool isFolder = false, bool forceLoad = false)
        {
            try
            {
                string fileName = fullPathOrExt.StartsWith(".") ? "AAA" + fullPathOrExt : fullPathOrExt;

                switch (size)
                {
                    case IconSize.thumbnail:
                    case IconSize.extraLarge:
                    //case IconSize.large:
                    case IconSize.jumbo:
                        Bitmap retImage = null;

                        try
                        {
                            retImage = GetBitmap(size, fileName, isFolder, forceLoad);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("GetGenericIcon - " + ex.Message);
                            //TO-DO: Fix exception:
                            //GetGenericIcon Unable to cast COM object of type 'System.__ComObject' to interface type 'IImageList'. 
                            //This operation failed because the QueryInterface call on the COM component for the interface with IID 
                            //'{46EB5926-582E-4017-9FDF-E8998DAA0950}' failed due to the following error: No such interface supported 
                            //(Exception from HRESULT: 0x80004002 (E_NOINTERFACE)).

                            //FailSafe
                            if (size > IconSize.large)
                                return GetGenericIcon(fullPathOrExt, IconSize.large, isFolder, forceLoad);
                        }

                        if (ImageTools.CheckImage(retImage))
                            return ImageTools.resizeImage(retImage, IconSizeToSize(size), 0);
                        else return ImageTools.resizeImage(ImageTools.cutImage(retImage, new Size(48, 48)), IconSizeToSize(size), 0);
                }

                SHFILEINFO shinfo = new SHFILEINFO();

                uint flags = SHGFI_SYSICONINDEX; // | SHGFI_PIDL
                if (!isFolder)
                    flags |= SHGFI_USEFILEATTRIBUTES;

                if (size == IconSize.small)
                    flags = flags | SHGFI_ICON | SHGFI_SMALLICON;
                else flags = flags | SHGFI_ICON;
                try
                {
                    SHGetFileInfo(fileName, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), flags);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("GetGenericIcon - " + ex.Message);
                    return new Bitmap(1, 1);
                }
                if (shinfo.hIcon != IntPtr.Zero)
                {
                    Bitmap retVal = Icon.FromHandle(shinfo.hIcon).ToBitmap();
                    DestroyIcon(shinfo.hIcon);
                    return retVal;
                }
                else return new Bitmap(1, 1);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("GetGenericIcon - " + ex.Message);
                return new Bitmap(1, 1);
            }
        }

        protected Bitmap GetGenericIcon(IntPtr ptr, IconSize size, bool isFolder = false, bool forceLoad = false)
        {
            switch (size)
            {
                case IconSize.thumbnail:
                case IconSize.extraLarge:
                case IconSize.large:
                case IconSize.jumbo:
                    Bitmap retImage = null;
                    retImage = GetBitmap(size, ptr, isFolder, forceLoad);
                    if (ImageTools.CheckImage(retImage))
                        return ImageTools.resizeImage(retImage, IconSizeToSize(size), 0);
                    else return ImageTools.resizeImage(ImageTools.cutImage(retImage, new Size(48, 48)), IconSizeToSize(size), 0);
            }

            SHFILEINFO shinfo = new SHFILEINFO();

            uint flags = SHGFI_SYSICONINDEX | SHGFI_PIDL;
            if (!isFolder)
                flags |= SHGFI_USEFILEATTRIBUTES;

            if (size == IconSize.small)
                flags = flags | SHGFI_ICON | SHGFI_SMALLICON;
            else flags = flags | SHGFI_ICON;
            try
            {
                SHGetFileInfo(ptr, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), flags);
            }
            catch
            {
                return new Bitmap(1, 1);
            }

            if (shinfo.hIcon != IntPtr.Zero)
            {
                Bitmap retVal = Icon.FromHandle(shinfo.hIcon).ToBitmap();
                DestroyIcon(shinfo.hIcon);
                return retVal;
            }
            else return new Bitmap(1, 1);
        }
        #endregion
    }

    /// <summary>
    /// .Net 2.0 WinForms level icon extractor with cache support.
    /// </summary>
    /// <typeparam name="FSI"></typeparam>
    public abstract class IconExtractor<FSI> : IconExtractor //T may be FileSystemInfo, Ex or ExA
    {
        #region Data



        Dictionary<Tuple<string, IconSize>, Bitmap> _iconCache = new Dictionary<Tuple<string, IconSize>, Bitmap>();
        ReaderWriterLock _iconCacheLock = new ReaderWriterLock();

        #endregion

        #region Constructor

        public IconExtractor()
        {
            initCache();
        }

        #endregion



        #region Methods



        protected abstract void GetIconKey(FSI entry, IconSize size, out string fastKey, out string slowKey);
        protected abstract Bitmap GetIconInner(FSI entry, string key, IconSize size);

        protected void initCache()
        {
            Action<IconSize> addToDic = (size) =>
            {
                Tuple<string, IconSize> iconKey = new Tuple<string, IconSize>(tempPath, size);
                this._iconCache.Add(iconKey, GetGenericIcon(tempPath, size, true, false));
            };

            lock (_iconCache)
                foreach (IconSize size in Enum.GetValues(typeof(IconSize)))
                    addToDic(size);
        }

        public bool IsDelayLoading(FSI entry, IconSize size)
        {
            string fastKey, slowKey;
            GetIconKey(entry, size, out fastKey, out slowKey);
            return fastKey != slowKey;
        }

        public Bitmap GetIcon(FSI entry, string key, bool isDir, IconSize size)
        {
            Func<string, IconSize, Bitmap> getIconFromCache =
                (k, s) =>
                {
                    Tuple<string, IconSize> dicKey = new Tuple<string, IconSize>(k, s);

                    try
                    {
                        _iconCacheLock.AcquireReaderLock(0);

                        if (_iconCache.ContainsKey(dicKey))
                            lock (_iconCache[dicKey])
                                return _iconCache[dicKey];
                    }
                    catch { return null; }
                    finally { _iconCacheLock.ReleaseReaderLock(); }

                    return null;
                };

            Action<string, IconSize, Bitmap> addIconToCache =
               (k, s, b) =>
               {
                   Tuple<string, IconSize> dicKey = new Tuple<string, IconSize>(k, s);

                   if (k.StartsWith("."))
                       try
                       {
                           _iconCacheLock.AcquireWriterLock(Timeout.Infinite);
                           if (!_iconCache.ContainsKey(dicKey))
                               _iconCache.Add(dicKey, b);
                           else _iconCache[dicKey] = b;
                       }
                       finally { _iconCacheLock.ReleaseWriterLock(); }

               };


            Bitmap retImg = null;
            retImg = getIconFromCache(key, size);
            //if (retImg != null && !key.StartsWith("::")) retImg.Save(@"C:\temp\" + "AAA" + key + ".png");
            if (retImg != null) return retImg;


            try
            {
                if (key.StartsWith(".")) //ext, retrieve automatically
                    retImg = GetGenericIcon(key, size);
                else
                    if (IsSpecialIcon(key) && File.Exists(key))
                        retImg = GetGenericIcon(key, size, isDir, true);
                    else
                        retImg = GetIconInner(entry, key, size);
            }
            catch (Exception ex)
            { retImg = null; Debug.WriteLine("IconExtractor.GetIcon" + ex.Message); }

            if (retImg != null)
            {
                Size destSize = IconSizeToSize(size);

                if (size == IconSize.jumbo && IsImageIcon(key))
                    retImg = ImageTools.resizeImage(retImg, destSize, 5);
                else retImg = ImageTools.resizeImage(retImg, destSize, 0);

                addIconToCache(key, size, retImg);
            }


            return retImg;

        }

        public Bitmap GetIcon(string fileName, IconSize size, bool isDir)
        {
            return GetBitmap(size, fileName, isDir, true);
        }

        public Bitmap GetIcon(FSI entry, IconSize size, bool isDir, bool fast)
        {
            string fastKey, slowKey;
            GetIconKey(entry, size, out fastKey, out slowKey);
            //return GetIcon(entry, slowKey, size);

            if (fast || size <= IconSize.large)
                return GetIcon(entry, fastKey, isDir, size);
            else
            {
                Bitmap icon = GetIcon(entry, slowKey, isDir, size);
                return icon;

            }
        }

        #endregion



    }
}
