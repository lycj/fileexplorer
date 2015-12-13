using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;

namespace QuickZip.Converters
{
    public abstract partial class IconConverterBase
    {

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
    }
}
