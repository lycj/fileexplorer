using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using vbAccelerator.Components.ImageList;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Threading;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.IO;
using System.Diagnostics;
using System.Windows.Resources;

namespace QuickZip.IO.COFE.UserControls
{
    public abstract class IconConverterBase : IValueConverter
    {

        #region Defines
        public enum IconSize
        {
            small, large, extraLarge, jumbo, thumbnail
        }
        private static string moduleName = null;
        private int defaultsize;

        public int DefaultSize { get { return defaultsize; } set { defaultsize = value; } }

        private class thumbnailInfo
        {
            public IconSize iconsize;
            public WriteableBitmap bitmap;
            public IFileSystemInfoExA entry { get { return FileSystemInfoExA.FromStringParse(fullPath); } }
            public string fullPath;
            public int roll;
            public thumbnailInfo(WriteableBitmap b, string path, IconSize size, int rol)
            {
                bitmap = b;
                fullPath = path;
                iconsize = size;
                roll = rol;
            }
        }
        #endregion

        #region Environment Tools
        public static bool isVistaUp()
        {
            return (Environment.OSVersion.Version.Major >= 6);
        }

        #endregion

        #region Win32api
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
        #endregion

        #region Image Tools
        public static BitmapSource loadBitmap(Bitmap source)
        {
            IntPtr hBitmap = source.GetHbitmap();
            //Memory Leak fixes, for more info : http://social.msdn.microsoft.com/forums/en-US/wpf/thread/edcf2482-b931-4939-9415-15b3515ddac6/
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty,
                   BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(hBitmap);
            }

        }

        private static void copyBitmap(BitmapSource source, WriteableBitmap target, bool dispatcher, int spacing)
        {
            int width = source.PixelWidth;
            int height = source.PixelHeight;
            int stride = width * ((source.Format.BitsPerPixel + 7) / 8);

            byte[] bits = new byte[height * stride];
            source.CopyPixels(bits, stride, 0);
            source = null;

            //original code.
            //writeBitmap.Dispatcher.Invoke(DispatcherPriority.Background,
            //    new ThreadStart(delegate
            //    {
            //        //UI Thread
            //        Int32Rect outRect = new Int32Rect(0, (int)(writeBitmap.Height - height) / 2, width, height);                    
            //        writeBitmap.WritePixels(outRect, bits, stride, 0);                                        
            //    }));

            //Bugfixes by h32

            if (dispatcher)
            {
                target.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                new ThreadStart(delegate
                {
                    //UI Thread
                    var delta = target.Height - height;
                    var newWidth = width > target.Width ? (int)target.Width : width;
                    var newHeight = height > target.Height ? (int)target.Height : height;
                    Int32Rect outRect = new Int32Rect((int)((target.Width - newWidth) / 2), (int)(delta >= 0 ? delta : 0) / 2 + spacing, newWidth - (spacing * 2), newWidth - (spacing * 2));
                    try
                    {
                        target.WritePixels(outRect, bits, stride, 0);
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debugger.Break();
                    }
                }));
            }
            else
            {
                var delta = target.Height - height;
                var newWidth = width > target.Width ? (int)target.Width : width;
                var newHeight = height > target.Height ? (int)target.Height : height;
                Int32Rect outRect = new Int32Rect(spacing, (int)(delta >= 0 ? delta : 0) / 2 + spacing, newWidth - (spacing * 2), newWidth - (spacing * 2));
                try
                {
                    target.WritePixels(outRect, bits, stride, 0);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debugger.Break();
                }
            }
        }

        //http://blog.paranoidferret.com/?p=11 , modified a little.
        protected static Bitmap resizeImage(Bitmap imgToResize, System.Drawing.Size size, int spacing)
        {
            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)size.Width / (float)sourceWidth);
            nPercentH = ((float)size.Height / (float)sourceHeight);

            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;

            int destWidth = (int)((sourceWidth * nPercent) - spacing * 4);
            int destHeight = (int)((sourceHeight * nPercent) - spacing * 4);

            int leftOffset = (int)((size.Width - destWidth) / 2);
            int topOffset = (int)((size.Height - destHeight) / 2);


            Bitmap b = new Bitmap(size.Width, size.Height);
            Graphics g = Graphics.FromImage((Image)b);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            g.DrawLines(System.Drawing.Pens.Silver, new PointF[] {
                new PointF(leftOffset - spacing, topOffset + destHeight + spacing), //BottomLeft
                new PointF(leftOffset - spacing, topOffset -spacing),                 //TopLeft
                new PointF(leftOffset + destWidth + spacing, topOffset - spacing)});//TopRight

            g.DrawLines(System.Drawing.Pens.Gray, new PointF[] {
                new PointF(leftOffset + destWidth + spacing, topOffset - spacing),  //TopRight
                new PointF(leftOffset + destWidth + spacing, topOffset + destHeight + spacing), //BottomRight
                new PointF(leftOffset - spacing, topOffset + destHeight + spacing),}); //BottomLeft

            g.DrawImage(imgToResize, leftOffset, topOffset, destWidth, destHeight);
            g.Dispose();

            return b;
        }
        protected static Bitmap resizeJumbo(Bitmap imgToResize, System.Drawing.Size size, int spacing)
        {
            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)size.Width / (float)sourceWidth);
            nPercentH = ((float)size.Height / (float)sourceHeight);

            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;

            int destWidth = 80;
            int destHeight = 80;

            int leftOffset = (int)((size.Width - destWidth) / 2);
            int topOffset = (int)((size.Height - destHeight) / 2);


            Bitmap b = new Bitmap(size.Width, size.Height);
            Graphics g = Graphics.FromImage((Image)b);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            g.DrawLines(System.Drawing.Pens.Silver, new PointF[] {
                new PointF(0 + spacing, size.Height - spacing), //BottomLeft
                new PointF(0 + spacing, 0 + spacing),                 //TopLeft
                new PointF(size.Width - spacing, 0 + spacing)});//TopRight

            g.DrawLines(System.Drawing.Pens.Gray, new PointF[] {
                new PointF(size.Width - spacing, 0 + spacing),  //TopRight
                new PointF(size.Width - spacing, size.Height - spacing), //BottomRight
                new PointF(0 + spacing, size.Height - spacing)}); //BottomLeft

            g.DrawImage(imgToResize, leftOffset, topOffset, destWidth, destHeight);
            g.Dispose();

            return b;
        }
        #endregion

        #region Instance Cache
        private static Dictionary<string, ImageSource> iconDic = new Dictionary<string, ImageSource>();
        private static int currentRoll = 1;
        ReaderWriterLockSlim getPathLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        public void ClearInstanceCache()
        {
            iconDic.Clear();
            currentRoll += 1;
        }

        public ImageSource GetIconImage(string fastKey, string key, IconSize size, bool delayLoading)
        {
            string extKey = KeyToExtKey(key, size);
            if (!iconDic.ContainsKey(extKey))
                if (delayLoading)
                {
                    WriteableBitmap bitmap = new WriteableBitmap(loadBitmap(KeyToBitmap(fastKey, size)));
                    thumbnailInfo info = new thumbnailInfo(bitmap, key, size, currentRoll);
                    if (size == IconSize.thumbnail)
                        ThreadPool.QueueUserWorkItem(new WaitCallback(PollThumbnailCallback), info);
                    else ThreadPool.QueueUserWorkItem(new WaitCallback(PollIconCallback), info);

                    iconDic.Add(extKey, bitmap);
                }
                else
                    iconDic.Add(extKey, loadBitmap(KeyToBitmap(key, size)));
            return iconDic[extKey];
        }

        #region PollCallback
        //Icon (e.g. exe)
        private void PollIconCallback(object state)
        {
            thumbnailInfo input = state as thumbnailInfo;

            IFileSystemInfoExA entry = input.entry;

            if (input.roll != currentRoll) { Debug.Write(entry.Name + " discarded."); return; }
            getPathLock.TryEnterWriteLock(-1);
            string fileName = "";
            try { fileName = entry.FullName; }
            finally { getPathLock.ExitWriteLock(); }

            WriteableBitmap writeBitmap = input.bitmap;
            IconSize size = input.iconsize;

            if (input.roll != currentRoll) { Debug.Write(entry.Name + " discarded."); return; }
            Bitmap origBitmap = KeyToBitmap(fileName, size);
            Bitmap inputBitmap = origBitmap;
            if (size == IconSize.jumbo || size == IconSize.thumbnail)
                inputBitmap = resizeJumbo(origBitmap, IconSizeToSize(size), 5);
            else inputBitmap = resizeImage(origBitmap, IconSizeToSize(size), 0);

            BitmapSource inputBitmapSource = loadBitmap(inputBitmap);
            origBitmap.Dispose();
            inputBitmap.Dispose();

            copyBitmap(inputBitmapSource, writeBitmap, true, 0);
        }
        //Thumbnail (for images)
        private void PollThumbnailCallback(object state)
        {
            //Non UIThread
            thumbnailInfo input = state as thumbnailInfo;
            IFileSystemInfoExA entry = input.entry;
            if (input.roll != currentRoll) { Debug.Write(entry.Name + " discarded."); return; }

            getPathLock.TryEnterWriteLock(-1);
            string fileName = "";
            try { fileName = entry.FullName; }
            finally { getPathLock.ExitWriteLock(); }

            WriteableBitmap writeBitmap = input.bitmap;
            IconSize size = input.iconsize;

            try
            {
                if (input.roll != currentRoll) { Debug.Write(entry.Name + " discarded."); return; }
                Bitmap origBitmap = new Bitmap(fileName);
                Bitmap inputBitmap = resizeImage(origBitmap, IconSizeToSize(size), 5);
                BitmapSource inputBitmapSource = loadBitmap(inputBitmap);
                origBitmap.Dispose();
                inputBitmap.Dispose();

                copyBitmap(inputBitmapSource, writeBitmap, true, 0);
            }
            catch (Exception ex) { Debug.WriteLine("PollThumbnailCallback " + ex.Message + "(" + fileName + ")"); }

        }
        #endregion


        #endregion

        #region Unused


        //#region Static Tools
        //public static bool isVistaUp()
        //{
        //    return (Environment.OSVersion.Version.Major >= 6);
        //}




        //private static System.Drawing.Size getDefaultSize(IconSize size)
        //{
        //    switch (size)
        //    {
        //        case IconSize.jumbo: return new System.Drawing.Size(256, 256);
        //        case IconSize.thumbnail: return new System.Drawing.Size(256, 256);
        //        case IconSize.extraLarge: return new System.Drawing.Size(48, 48);
        //        case IconSize.large: return new System.Drawing.Size(32, 32);
        //        default: return new System.Drawing.Size(16, 16);
        //    }

        //}


        //

        ////http://igorshare.wordpress.com/2009/01/07/wpf-extracting-bitmapimage-from-an-attached-resource-in-referenced-assemblylibrary/
        //private static BitmapImage GetResourceImage(string resourcePath)
        //{
        //    var image = new BitmapImage();            
        //    string resourceLocation =
        //        string.Format("pack://application:,,,/{0};component/{1}", moduleName,
        //                      resourcePath);
        //    try
        //    {
        //        image.BeginInit();
        //        image.CacheOption = BitmapCacheOption.OnLoad;
        //        image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
        //        image.UriSource = new Uri(resourceLocation);
        //        image.EndInit();
        //        image.Freeze();
        //    }
        //    catch (Exception e)
        //    {
        //        System.Diagnostics.Trace.WriteLine(e.ToString());
        //    }

        //    return image;
        //}
        //private static Bitmap GetResourceBitmap(string resourcePath)
        //{            
        //    string resourceLocation =
        //        string.Format("pack://application:,,,/{0};component/{1}", moduleName,
        //                      resourcePath);

        //    StreamResourceInfo info = Application.GetResourceStream(new Uri(resourceLocation));
        //    Bitmap bitmap = new Bitmap(info.Stream);
        //    return bitmap;
        //}

        //private static Bitmap GetFolderBitmap(string lookup, IconSize size)
        //{

        //    switch (size)
        //    {
        //        case IconSize.thumbnail:
        //        case IconSize.jumbo:
        //            return loadJumbo(lookup, isDiskFolder(lookup));
        //        case IconSize.extraLarge:
        //            _imgList.ImageListSize = SysImageListSize.extraLargeIcons;
        //            return _imgList.Icon(_imgList.IconIndex(lookup, isDiskFolder(lookup))).ToBitmap();
        //        //case IconSize.large :
        //        //    _imgList.ImageListSize = SysImageListSize.largeIcons;
        //        //    return _imgList.Icon(_imgList.IconIndex(lookup, isDiskFolder(lookup))).ToBitmap();
        //        //case IconSize.small :
        //        //    _imgList.ImageListSize = SysImageListSize.smallIcons;
        //        //    return _imgList.Icon(_imgList.IconIndex(lookup, isDiskFolder(lookup))).ToBitmap();
        //        default:
        //            try
        //            {
        //                return GetFileIcon(lookup, size).ToBitmap();
        //            }
        //            catch { return GetFolderBitmap(UCUtils.GetProgramPath(), size); }
        //    }
        //}



        //public static bool isImage(IFileSystemInfoExA entry)
        //{
        //    string ext = PathEx.GetExtension(entry.Name).ToLower();
        //    if (ext == "")
        //        return false;
        //    return (imageFilter.IndexOf(ext) != -1 && entry is IFileInfoExA);
        //}

        //private static bool isExecutable(IFileSystemInfoExA entry)
        //{
        //    string ext = PathEx.GetExtension(entry.Name).ToLower();
        //    if (ext == "")
        //        return false;
        //    return (exeFilter.IndexOf(ext) != -1 && entry is IFileInfoExA);
        //}
        //private static bool isImage(string fileName)
        //{
        //    string ext = PathEx.GetExtension(fileName).ToLower();
        //    if (ext == "")
        //        return false;
        //    return (imageFilter.IndexOf(ext) != -1 && File.Exists(fileName));
        //}
        //private static bool isExecutable(string fileName)
        //{
        //    string ext = PathEx.GetExtension(fileName).ToLower();
        //    if (ext == "")
        //        return false;
        //    return (exeFilter.IndexOf(ext) != -1 && File.Exists(fileName));
        //}
        //private static bool isDiskFolder(IFileSystemInfoExA entry)
        //{
        //    return entry.IsFolder && !entry.IsVirtual;
        //}
        //private static bool isDiskFolder(string name)
        //{
        //    return Directory.Exists(name);
        //}

        //static string dirLookup = System.IO.Path.GetTempPath();
        //private static string returnLookupPath(IFileSystemInfoExA entry)
        //{
        //    if (entry is IDirectoryInfoExA)
        //    {
        //        IDirectoryInfoExA dirEntry = entry as IDirectoryInfoExA;
        //        if (dirEntry.IsFileBasedFS)
        //            return "@" + PathEx.GetExtension(entry.Name).TrimStart('.');

        //        if (!dirEntry.IsVirtual)
        //            return entry.FullName;

        //        return dirLookup;
        //    }
        //    else
        //    {
        //        if (PathEx.GetExtension(entry.Name) != "")
        //            return PathEx.GetExtension(entry.Name);

        //        if (!entry.IsVirtual)
        //            return entry.FullName;

        //        return "AFILE";
        //    }
        //}

        //private static string returnKey(IFileSystemInfoExA entry, IconSize size)
        //{
        //    string key = entry.GetHashCode().ToString();
        //    string fileName = entry.ParseName;

        //    if (isExecutable(entry))
        //        key = fileName.ToLower();
        //    if (isImage(entry) && size == IconSize.thumbnail)
        //        key = fileName.ToLower();
        //    if (isDiskFolder(entry))
        //        key = fileName.ToLower();

        //    switch (size)
        //    {
        //        case IconSize.thumbnail: key += isImage(entry) ? "+T" : "+J"; break;
        //        case IconSize.jumbo: key += "+J"; break;
        //        case IconSize.extraLarge: key += "+XL"; break;
        //        case IconSize.large: key += "+L"; break;
        //        case IconSize.small: key += "+S"; break;
        //    }
        //    return key;
        //}

        //private static string returnExtensionKey(string fileName, IconSize size)
        //{
        //    string key = Path.GetExtension(fileName).ToLower();

        //    switch (size)
        //    {
        //        case IconSize.thumbnail: key += isImage(fileName) ? "+T" : "+J"; break;
        //        case IconSize.jumbo: key += "+J"; break;
        //        case IconSize.extraLarge: key += "+XL"; break;
        //        case IconSize.large: key += "+L"; break;
        //        case IconSize.small: key += "+S"; break;
        //    }
        //    return key;
        //}

        //#endregion

        //#region Static Cache
        //private static Dictionary<string, ImageSource> iconDic = new Dictionary<string, ImageSource>();
        //private static SysImageList _imgList = new SysImageList(SysImageListSize.jumbo);

        //private static Bitmap loadJumbo(string lookup, bool forceLoadFromDisk)
        //{
        //    _imgList.ImageListSize = isVistaUp() ? SysImageListSize.jumbo : SysImageListSize.extraLargeIcons;

        //    Icon icon = _imgList.Icon(_imgList.IconIndex(lookup, forceLoadFromDisk));
        //    Bitmap bitmap = icon.ToBitmap();
        //    icon.Dispose();

        //    System.Drawing.Color empty = System.Drawing.Color.FromArgb(0, 0, 0, 0);

        //    if (bitmap.Width < 256)
        //        bitmap = resizeImage(bitmap, new System.Drawing.Size(256, 256), 0);
        //    else if (bitmap.GetPixel(100, 100) == empty && bitmap.GetPixel(200, 200) == empty && bitmap.GetPixel(200, 200) == empty)
        //    {
        //        _imgList.ImageListSize = SysImageListSize.largeIcons;
        //        bitmap = resizeJumbo(_imgList.Icon(_imgList.IconIndex(lookup)).ToBitmap(), new System.Drawing.Size(200, 200), 5);
        //    }

        //    return bitmap;
        //}

        //#endregion




        //#region Constructor
        //public IconConverterBase()
        //{
        //    moduleName = this.GetType().Assembly.GetName().Name;
        //}
        //#endregion
        #endregion


        #region Static Methods
        private static string KeyToExtKey(string key, IconSize size)
        {
            switch (size)
            {
                case IconSize.jumbo: return key + "J";
                case IconSize.thumbnail: return key + "T";
                case IconSize.extraLarge: return key + "E";
                case IconSize.large: return key + "L";
                default: return key;
            }
        }

        private static string ExtKeyToKey(string extKey, IconSize size)
        {
            if (extKey.Length > 2 && extKey[extKey.Length - 2] == '+')
                return extKey.Substring(0, extKey.Length - 2);
            return extKey;
        }

        private static System.Drawing.Size IconSizeToSize(IconSize size)
        {
            switch (size)
            {
                case IconSize.jumbo: return new System.Drawing.Size(256, 256);
                case IconSize.thumbnail: return new System.Drawing.Size(256, 256);
                case IconSize.extraLarge: return new System.Drawing.Size(48, 48);
                case IconSize.large: return new System.Drawing.Size(32, 32);
                default: return new System.Drawing.Size(16, 16);
            }
        }

        private static IconSize SizeToIconSize(int size)
        {
            if (size <= 16) return IconSize.small;
            else if (size <= 32) return IconSize.large;
            else if (size <= 47) return IconSize.extraLarge;
            //else if (iconSize <= 72) return IconSize.jumbo;
            else return IconSize.thumbnail;
        }
        #endregion


        #region IValueConverter Members

        /// <summary>
        /// Return the key to be stored for the input value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="key">Key used for non-thumbnail.</param>
        /// <param name="imageKey">Key used for thumbnail</param>
        /// <param name="delayLoading">Load icon in separate thread.</param>
        protected abstract void ValueToKey(object value, out string key, out string fastKey, out bool delayLoading);
        /// <summary>
        /// Return the Bitmap that represent the specified key and size.
        /// </summary>
        protected abstract Bitmap KeyToBitmap(string key, IconSize size);


        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string key, fastKey;
            bool delayLoading;
            ValueToKey(value, out key, out fastKey, out delayLoading);

            IconSize iconSize = SizeToIconSize(DefaultSize);
            if (parameter is int)
                iconSize = SizeToIconSize((int)parameter);

            return GetIconImage(fastKey, key, iconSize, delayLoading);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
