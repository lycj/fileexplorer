///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// LYCJ (c) 2009 - http://www.quickzip.org/components                                                            //
// Release under LGPL license.                                                                                   //
//                                                                                                               //
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
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
using QuickZip.UserControls;
using QuickZip.Converters;

namespace QuickZip.IO.PIDL.UserControls
{

    [ValueConversion(typeof(string), typeof(ImageSource))]
    public class FileNameToIconConverter : IconConverterBase
    {
        //private static string imageFilter = ".jpg,.jpeg,.png,.gif,.bmp,.tiff";
        private static string tempPath = System.IO.Path.GetTempPath();

        protected override void ValueToKey(object value, out string key, out string imageKey, out bool delayLoading)
        {
            key = imageKey = ""; delayLoading = false;

            if (value is string)
            {
                string path = value as string;
                if (Path.GetExtension(path) != "")
                {
                    key = imageKey = Path.GetExtension(path);

                    if (imageFilter.IndexOf(key) != -1)
                    {
                        imageKey = path;
                        delayLoading = true;
                    }
                }
                else
                    if (File.Exists(path)) //File without extension
                        key = imageKey = ".AaAaA";
                    else //Directory
                        key = imageKey = tempPath;

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

            string ext = PathEx.GetExtension(key);
            if ((ext != "" && ext != key && imageFilter.IndexOf(ext) != -1))
            {
                return GetFileIcon(FileSystemInfoEx.FromString(key).PIDL.Ptr, size);
            }
            else

                if (key == "" || key.StartsWith(".")) //Extension 
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
