using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows;
using ShellDll;

namespace WPFDemo
{
    public class GetIconConverter : IValueConverter
    {

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
        internal const uint SHGFI_PIDL = 0x000000008;
        internal const uint SHGFI_TYPENAME = 0x400;
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

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        // <summary>
        /// Return large file icon of the specified file.
        /// </summary>
        internal static Icon GetFileIcon(IntPtr fileName)
        {
            SHFILEINFO shinfo = new SHFILEINFO();

            uint flags = SHGFI_SYSICONINDEX | SHGFI_USEFILEATTRIBUTES | SHGFI_PIDL | SHGFI_ICON | SHGFI_SMALLICON | SHGFI_LARGEICON;

            SHGetFileInfo(fileName, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), flags);
            return shinfo.hIcon != IntPtr.Zero ? Icon.FromHandle(shinfo.hIcon) : null;
        }

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

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (value is FileSystemInfoEx)
                {
                    return (value as FileSystemInfoEx).RequestPIDL(pidl =>
                    {
                        Icon ico = GetFileIcon(pidl.Ptr);
                        return ico != null ? loadBitmap(ico.ToBitmap()) : null;
                    });
                    
                }
            }
            catch
            {
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
