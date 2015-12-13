/// Created By LYCJ (2010), released under LGPL license
/// I did some tidy up Based on http://vbaccelerator.com/home/net/code/libraries/Shell_Projects/SysImageList/article.asp

using MetroLog;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;


namespace QuickZip.Converters
{
    public enum IconSize : int
    {
        small = 0x1, large = 0x0, extraLarge = 0x2, jumbo = 0x4, thumbnail = 0x5
    }

    #region Public Enumerations
    /// <summary>
    /// Available system image list sizes
    /// </summary>
    public enum SysImageListSize : int
    {
        /// <summary>
        /// System Large Icon Size (typically 32x32)
        /// </summary>
        largeIcons = 0x0,
        /// <summary>
        /// System Small Icon Size (typically 16x16)
        /// </summary>
        smallIcons = 0x1,
        /// <summary>
        /// System Extra Large Icon Size (typically 48x48).
        /// Only available under XP; under other OS the
        /// Large Icon ImageList is returned.
        /// </summary>
        extraLargeIcons = 0x2,
        jumbo = 0x4
    }

    /// <summary>
    /// Flags controlling how the Image List item is 
    /// drawn
    /// </summary>
    [Flags]
    public enum ImageListDrawItemConstants : int
    {
        /// <summary>
        /// Draw item normally.
        /// </summary>
        ILD_NORMAL = 0x0,
        /// <summary>
        /// Draw item transparently.
        /// </summary>
        ILD_TRANSPARENT = 0x1,
        /// <summary>
        /// Draw item blended with 25% of the specified foreground colour
        /// or the Highlight colour if no foreground colour specified.
        /// </summary>
        ILD_BLEND25 = 0x2,
        /// <summary>
        /// Draw item blended with 50% of the specified foreground colour
        /// or the Highlight colour if no foreground colour specified.
        /// </summary>
        ILD_SELECTED = 0x4,
        /// <summary>
        /// Draw the icon's mask
        /// </summary>
        ILD_MASK = 0x10,
        /// <summary>
        /// Draw the icon image without using the mask
        /// </summary>
        ILD_IMAGE = 0x20,
        /// <summary>
        /// Draw the icon using the ROP specified.
        /// </summary>
        ILD_ROP = 0x40,
        /// <summary>
        /// Preserves the alpha channel in dest. XP only.
        /// </summary>
        ILD_PRESERVEALPHA = 0x1000,
        /// <summary>
        /// Scale the image to cx, cy instead of clipping it.  XP only.
        /// </summary>
        ILD_SCALE = 0x2000,
        /// <summary>
        /// Scale the image to the current DPI of the display. XP only.
        /// </summary>
        ILD_DPISCALE = 0x4000,

        ILD_OVERLAYMASK = 0x00000F00,
        ILD_ASYNC = 0x00008000
    }

    public enum dwRop : uint
    {
        /// <summary>
        ///  
        /// </summary>
        SRCCOPY = 0x00CC0020, /* dest = source */
        /// <summary>
        ///  
        /// </summary>
        SRCPAINT = 0x00EE0086, /* dest = source OR dest */
        /// <summary>
        ///  
        /// </summary>
        SRCAND = 0x008800C6, /* dest = source AND dest */
        /// <summary>
        ///  
        /// </summary>
        SRCINVERT = 0x00660046, /* dest = source XOR dest */
        /// <summary>
        ///  
        /// </summary>
        SRCERASE = 0x00440328, /* dest = source AND (NOT dest ) */
        /// <summary>
        ///  
        /// </summary>
        NOTSRCCOPY = 0x00330008, /* dest = (NOT source) */
        /// <summary>
        ///  
        /// </summary>
        NOTSRCERASE = 0x001100A6, /* dest = (NOT src) AND (NOT dest) */
        /// <summary>
        ///  
        /// </summary>
        MERGECOPY = 0x00C000CA, /* dest = (source AND pattern) */
        /// <summary>
        ///  
        /// </summary>
        MERGEPAINT = 0x00BB0226, /* dest = (NOT source) OR dest */
        /// <summary>
        ///  
        /// </summary>
        PATCOPY = 0x00F00021, /* dest = pattern */
        /// <summary>
        ///  
        /// </summary>
        PATPAINT = 0x00FB0A09, /* dest = DPSnoo */
        /// <summary>
        ///  
        /// </summary>
        PATINVERT = 0x005A0049, /* dest = pattern XOR dest */
        /// <summary>
        ///  
        /// </summary>
        DSTINVERT = 0x00550009, /* dest = (NOT dest) */
        /// <summary>
        ///  
        /// </summary>
        BLACKNESS = 0x00000042, /* dest = BLACK */
        /// <summary>
        ///  
        /// </summary>
        WHITENESS = 0x00FF0062, /* dest = WHITE */
    }

    /// <summary>
    /// Enumeration containing XP ImageList Draw State options
    /// </summary>
    [Flags]
    public enum ImageListDrawStateConstants : int
    {
        /// <summary>
        /// The image state is not modified. 
        /// </summary>
        ILS_NORMAL = (0x00000000),
        /// <summary>
        /// Adds a glow effect to the icon, which causes the icon to appear to glow 
        /// with a given color around the edges. (Note: does not appear to be
        /// implemented)
        /// </summary>
        ILS_GLOW = (0x00000001), //The color for the glow effect is passed to the IImageList::Draw method in the crEffect member of IMAGELISTDRAWPARAMS. 
        /// <summary>
        /// Adds a drop shadow effect to the icon. (Note: does not appear to be
        /// implemented)
        /// </summary>
        ILS_SHADOW = (0x00000002), //The color for the drop shadow effect is passed to the IImageList::Draw method in the crEffect member of IMAGELISTDRAWPARAMS. 
        /// <summary>
        /// Saturates the icon by increasing each color component 
        /// of the RGB triplet for each pixel in the icon. (Note: only ever appears
        /// to result in a completely unsaturated icon)
        /// </summary>
        ILS_SATURATE = (0x00000004), // The amount to increase is indicated by the frame member in the IMAGELISTDRAWPARAMS method. 
        /// <summary>
        /// Alpha blends the icon. Alpha blending controls the transparency 
        /// level of an icon, according to the value of its alpha channel. 
        /// (Note: does not appear to be implemented).
        /// </summary>
        ILS_ALPHA = (0x00000008) //The value of the alpha channel is indicated by the frame member in the IMAGELISTDRAWPARAMS method. The alpha channel can be from 0 to 255, with 0 being completely transparent, and 255 being completely opaque. 
    }

    /// <summary>
    /// Flags specifying the state of the icon to draw from the Shell
    /// </summary>
    [Flags]
    public enum ShellIconStateConstants
    {
        /// <summary>
        /// Get icon in normal state
        /// </summary>
        ShellIconStateNormal = 0,
        /// <summary>
        /// Put a link overlay on icon 
        /// </summary>
        ShellIconStateLinkOverlay = 0x8000,
        /// <summary>
        /// show icon in selected state 
        /// </summary>
        ShellIconStateSelected = 0x10000,
        /// <summary>
        /// get open icon 
        /// </summary>
        ShellIconStateOpen = 0x2,
        /// <summary>
        /// apply the appropriate overlays
        /// </summary>
        ShellIconAddOverlays = 0x000000020,
    }
    #endregion

    public class SystemImageListCollection
    {
        #region Constructor
        public SystemImageListCollection()
        {
            //imageListDic = new Dictionary<IconSize, SystemImageList>()
            //{
            //    { IconSize.small, smallImageList },
            //    { IconSize.large, largeImageList },
            //    { IconSize.extraLarge, exLargeImageList },
            //    { IconSize.jumbo, jumboImageList },
            //    { IconSize.thumbnail, jumboImageList }
            //};
        }

        public void Dispose()
        {
            //if (smallImageList != null)
            //{
            //    smallImageList.Dispose();
            //    smallImageList = null;
            //}
            //if (largeImageList != null)
            //{
            //    largeImageList.Dispose();
            //    largeImageList = null;
            //}
            //if (exLargeImageList != null)
            //{
            //    exLargeImageList.Dispose();
            //    exLargeImageList = null;
            //}
            //if (jumboImageList != null)
            //{
            //    jumboImageList.Dispose();
            //    jumboImageList = null;
            //}
            if (currentImageList != null)
            { currentImageList.Dispose(); currentImageList = null; }
        }
        #endregion

        #region Data
        //private SystemImageList smallImageList = new SystemImageList(IconSize.small);
        //private SystemImageList largeImageList = new SystemImageList(IconSize.large);
        //private SystemImageList exLargeImageList = new SystemImageList(IconSize.extraLarge);
        //private SystemImageList jumboImageList = new SystemImageList(IconSize.jumbo);
        //private Dictionary<IconSize, SystemImageList> imageListDic;

        private SystemImageList currentImageList = null;
        private IconSize currentImageListSize = IconSize.large;

        #endregion

        #region Public Properties

        public IconSize CurrentImageListSize { get { return currentImageListSize; } }
        public bool IsImageListInited { get { return currentImageList != null; } }

        #endregion

        #region Methods

        public SystemImageList getImageList(IconSize size)
        {
            if (size == IconSize.thumbnail)
                size = IconSize.jumbo;

            if (currentImageList != null && currentImageListSize == size)
                return currentImageList;

            if (currentImageList != null)
            { currentImageList.Dispose(); currentImageList = null; }

            currentImageListSize = size;
            return currentImageList = new SystemImageList(currentImageListSize);
        }

        public SystemImageList this[IconSize size] { get { return getImageList(size); } }

        #endregion

    }

    public class SystemImageList : IDisposable
    {
        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<SystemImageList>();

        public static IntPtr Test()
        {
            SHFILEINFO shfi = new SHFILEINFO();
            uint shfiSize = (uint)Marshal.SizeOf(shfi.GetType());
            return SHGetFileInfo(@"C:\", 16, ref shfi, shfiSize, 16384);
        }

        #region Win32API
        #region UnmanagedCode
        private const int MAX_PATH = 260;

        [DllImport("shell32.dll")]
        private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes,
            ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

        //[DllImport("shell32", CharSet = CharSet.Unicode)]
        //private static extern IntPtr SHGetFileInfo(
        //    string pszPath,
        //    uint dwFileAttributes,
        //    ref SHFILEINFO psfi,
        //    uint cbFileInfo,
        //    uint uFlags);

        [DllImport("shell32.dll")]
        private static extern IntPtr SHGetFileInfo(IntPtr pszPath, uint dwFileAttributes,
            ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

        //[DllImport("shell32", CharSet = CharSet.Unicode)]
        //private static extern IntPtr SHGetFileInfo(
        //    IntPtr pszPath,
        //    uint dwFileAttributes,
        //    ref SHFILEINFO psfi,
        //    uint cbFileInfo,
        //    uint uFlags);

        [DllImport("user32.dll")]
        private static extern int DestroyIcon(IntPtr hIcon);

        private const int FILE_ATTRIBUTE_NORMAL = 0x80;
        private const int FILE_ATTRIBUTE_DIRECTORY = 0x10;

        private const int FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x100;
        private const int FORMAT_MESSAGE_ARGUMENT_ARRAY = 0x2000;
        private const int FORMAT_MESSAGE_FROM_HMODULE = 0x800;
        private const int FORMAT_MESSAGE_FROM_STRING = 0x400;
        private const int FORMAT_MESSAGE_FROM_SYSTEM = 0x1000;
        private const int FORMAT_MESSAGE_IGNORE_INSERTS = 0x200;
        private const int FORMAT_MESSAGE_MAX_WIDTH_MASK = 0xFF;
        [DllImport("kernel32")]
        private extern static int FormatMessage(
            int dwFlags,
            IntPtr lpSource,
            int dwMessageId,
            int dwLanguageId,
            string lpBuffer,
            uint nSize,
            int argumentsLong);

        [DllImport("kernel32")]
        private extern static int GetLastError();

        [DllImport("comctl32")]
        private extern static int ImageList_Draw(
            IntPtr hIml,
            int i,
            IntPtr hdcDst,
            int x,
            int y,
            int fStyle);

        [DllImport("comctl32")]
        private extern static int ImageList_DrawIndirect(
            ref IMAGELISTDRAWPARAMS pimldp);

        [DllImport("comctl32")]
        private extern static int ImageList_GetIconSize(
            IntPtr himl,
            ref int cx,
            ref int cy);

        [DllImport("comctl32")]
        private extern static int ImageList_GetImageInfo(
            IntPtr himl,
            int i,
            ref IMAGEINFO pImageInfo);

        [DllImport("comctl32")]
        private extern static IntPtr ImageList_GetIcon(
            IntPtr himl,
            int i,
            int flags);

        /// <summary>
        /// SHGetImageList is not exported correctly in XP.  See KB316931
        /// http://support.microsoft.com/default.aspx?scid=kb;EN-US;Q316931
        /// Apparently (and hopefully) ordinal 727 isn't going to change.
        /// </summary>
        [DllImport("shell32.dll", EntryPoint = "#727")]
        private extern static int SHGetImageList(
            int iImageList,
            ref Guid riid,
            ref IImageList ppv
            );

        [DllImport("shell32.dll", EntryPoint = "#727")]
        private extern static int SHGetImageListHandle(
            int iImageList,
            ref Guid riid,
            ref IntPtr handle
            );

        #endregion

        #region Private Enumerations
        [Flags]
        private enum SHGetFileInfoConstants : int
        {
            SHGFI_ICON = 0x100,                // get icon 
            SHGFI_DISPLAYNAME = 0x200,         // get display name 
            SHGFI_TYPENAME = 0x400,            // get type name 
            SHGFI_ATTRIBUTES = 0x800,          // get attributes 
            SHGFI_ICONLOCATION = 0x1000,       // get icon location 
            SHGFI_EXETYPE = 0x2000,            // return exe type 
            SHGFI_SYSICONINDEX = 0x4000,       // get system icon index             
            SHGFI_LINKOVERLAY = 0x8000,        // put a link overlay on icon 
            SHGFI_SELECTED = 0x10000,          // show icon in selected state 
            SHGFI_ATTR_SPECIFIED = 0x20000,    // get only specified attributes 
            SHGFI_LARGEICON = 0x0,             // get large icon 
            SHGFI_SMALLICON = 0x1,             // get small icon 
            SHGFI_OPENICON = 0x2,              // get open icon 
            SHGFI_SHELLICONSIZE = 0x4,         // get shell size icon 
            SHGFI_PIDL = 0x8,                  // pszPath is a pidl 
            SHGFI_USEFILEATTRIBUTES = 0x10,     // use passed dwFileAttribute 
            SHGFI_ADDOVERLAYS = 0x000000020,     // apply the appropriate overlays
            SHGFI_OVERLAYINDEX = 0x000000040     // Get the index of the overlay
        }
        #endregion

        #region Private ImageList structures
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            int x;
            int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct IMAGELISTDRAWPARAMS
        {
            public int cbSize;
            public IntPtr himl;
            public int i;
            public IntPtr hdcDst;
            public int x;
            public int y;
            public int cx;
            public int cy;
            public int xBitmap;        // x offest from the upperleft of bitmap
            public int yBitmap;        // y offset from the upperleft of bitmap
            public int rgbBk;
            public int rgbFg;
            public int fStyle;
            public int dwRop;
            public int fState;
            public int Frame;
            public int crEffect;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct IMAGEINFO
        {
            public IntPtr hbmImage;
            public IntPtr hbmMask;
            public int Unused1;
            public int Unused2;
            public RECT rcImage;
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public int dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }
        #endregion

        #region Private ImageList COM Interop (XP)
        [ComImportAttribute()]
        [GuidAttribute("46EB5926-582E-4017-9FDF-E8998DAA0950")]
        [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
        //helpstring("Image List"),
        interface IImageList
        {
            [PreserveSig]
            int Add(
                IntPtr hbmImage,
                IntPtr hbmMask,
                ref int pi);

            [PreserveSig]
            int ReplaceIcon(
                int i,
                IntPtr hicon,
                ref int pi);

            [PreserveSig]
            int SetOverlayImage(
                int iImage,
                int iOverlay);

            [PreserveSig]
            int Replace(
                int i,
                IntPtr hbmImage,
                IntPtr hbmMask);

            [PreserveSig]
            int AddMasked(
                IntPtr hbmImage,
                int crMask,
                ref int pi);

            [PreserveSig]
            int Draw(
                ref IMAGELISTDRAWPARAMS pimldp);

            [PreserveSig]
            int Remove(
                int i);

            [PreserveSig]
            int GetIcon(
                int i,
                int flags,
                ref IntPtr picon);

            [PreserveSig]
            int GetImageInfo(
                int i,
                ref IMAGEINFO pImageInfo);

            [PreserveSig]
            int Copy(
                int iDst,
                IImageList punkSrc,
                int iSrc,
                int uFlags);

            [PreserveSig]
            int Merge(
                int i1,
                IImageList punk2,
                int i2,
                int dx,
                int dy,
                ref Guid riid,
                ref IntPtr ppv);

            [PreserveSig]
            int Clone(
                ref Guid riid,
                ref IntPtr ppv);

            [PreserveSig]
            int GetImageRect(
                int i,
                ref RECT prc);

            [PreserveSig]
            int GetIconSize(
                ref int cx,
                ref int cy);

            [PreserveSig]
            int SetIconSize(
                int cx,
                int cy);

            [PreserveSig]
            int GetImageCount(
                ref int pi);

            [PreserveSig]
            int SetImageCount(
                int uNewCount);

            [PreserveSig]
            int SetBkColor(
                int clrBk,
                ref int pclr);

            [PreserveSig]
            int GetBkColor(
                ref int pclr);

            [PreserveSig]
            int BeginDrag(
                int iTrack,
                int dxHotspot,
                int dyHotspot);

            [PreserveSig]
            int EndDrag();

            [PreserveSig]
            int DragEnter(
                IntPtr hwndLock,
                int x,
                int y);

            [PreserveSig]
            int DragLeave(
                IntPtr hwndLock);

            [PreserveSig]
            int DragMove(
                int x,
                int y);

            [PreserveSig]
            int SetDragCursorImage(
                ref IImageList punk,
                int iDrag,
                int dxHotspot,
                int dyHotspot);

            [PreserveSig]
            int DragShowNolock(
                int fShow);

            [PreserveSig]
            int GetDragImage(
                ref POINT ppt,
                ref POINT pptHotspot,
                ref Guid riid,
                ref IntPtr ppv);

            [PreserveSig]
            int GetItemFlags(
                int i,
                ref int dwFlags);

            [PreserveSig]
            int GetOverlayImage(
                int iOverlay,
                ref int piIndex);
        };
        #endregion
        #endregion

        #region Constructor

        public SystemImageList(IconSize size)
        {
            if (!isXpOrAbove())
                throw new NotSupportedException("Windows XP or above required.");

            _size = size == IconSize.thumbnail ? IconSize.jumbo : size; //There is no thumbnail mode in shell.

            if (!isVistaUp() && (_size == IconSize.jumbo || _size == IconSize.extraLarge)) //XP do not have extra large or jumbo.
                _size = IconSize.large;

            Guid iidImageList = new Guid("46EB5926-582E-4017-9FDF-E8998DAA0950");
            int hr = SHGetImageList((int)_size, ref iidImageList, ref _iImageList); // Get the System IImageList object from the Shell:
            if (hr != 0)
                Marshal.ThrowExceptionForHR(hr);
            // the image list handle is the IUnknown pointer, but using Marshal.GetIUnknownForObject doesn't return
            // the right value.  It really doesn't hurt to make a second call to get the handle:            
            SHGetImageListHandle((int)_size, ref iidImageList, ref _ptrImageList);


            //int cx = 0, cy = 0;
            //ImageList_GetIconSize(_ptrImageList, ref cx, ref cy);
            //Debug.WriteLine(cx);

            //_iImageList.SetImageCount(2);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~SystemImageList()
        {
            Dispose(false);
        }

        public virtual void Dispose(bool disposing)
        {
            logger.Debug(String.Format("Dispose, disposing : {0}", disposing));
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_iImageList != null)
                        Marshal.ReleaseComObject(_iImageList);
                    _iImageList = null;
                }
            }
            _disposed = true;
        }
        #endregion

        #region Methods

        private static IImageList getImageListInterface(IconSize size)
        {
            IImageList iImageList = null;
            Guid iidImageList = new Guid("46EB5926-582E-4017-9FDF-E8998DAA0950");
            int hr = SHGetImageList((int)size, ref iidImageList, ref iImageList); // Get the System IImageList object from the Shell:
            if (hr != 0)
                Marshal.ThrowExceptionForHR(hr);
            return iImageList;
        }

        public Bitmap this[string fileName, bool isDirectory, bool forceLoadFromDisk, ShellIconStateConstants iconState]
        {
            get
            {
                try
                {
                    Icon icon = getIcon(getIconIndex(fileName, isDirectory, forceLoadFromDisk, iconState));
                    return icon == null ? new Bitmap(1, 1) : icon.ToBitmap();
                }
                catch { return new Bitmap(1, 1); }
            }
        }

        public Bitmap this[IntPtr pidlPtr, bool isDirectory, bool forceLoadFromDisk, ShellIconStateConstants iconState]
        {
            get
            {
                try
                {
                    Icon icon = getIcon(getIconIndex(pidlPtr, isDirectory, forceLoadFromDisk, iconState));
                    return icon == null ? new Bitmap(1, 1) : icon.ToBitmap();
                }
                catch { return new Bitmap(1, 1); }
            }
        }

        public Bitmap this[string fileName, bool isDirectory, bool forceLoadFromDisk]
        {
            get { return this[fileName, isDirectory, forceLoadFromDisk, ShellIconStateConstants.ShellIconStateNormal]; }
        }

        public Bitmap this[IntPtr pidlPtr, bool isDirectory, bool forceLoadFromDisk]
        {
            get { return this[pidlPtr, isDirectory, forceLoadFromDisk, ShellIconStateConstants.ShellIconStateNormal]; }
        }

        public Bitmap this[string fileName, bool isDirectory]
        {
            get { return this[fileName, isDirectory, false, ShellIconStateConstants.ShellIconStateNormal]; }
        }

        public Bitmap this[IntPtr pidlPtr, bool isDirectory]
        {
            get { return this[pidlPtr, isDirectory, false, ShellIconStateConstants.ShellIconStateNormal]; }
        }

        private void getAttributes(bool isDirectory, bool forceLoadFromDisk, out uint dwAttr, out SHGetFileInfoConstants dwFlags)
        {
            dwFlags = SHGetFileInfoConstants.SHGFI_SYSICONINDEX;
            dwAttr = 0;

            if (_size == IconSize.small)
                dwFlags |= SHGetFileInfoConstants.SHGFI_SMALLICON;

            if (isDirectory)
            {
                dwAttr = FILE_ATTRIBUTE_DIRECTORY;
            }
            else
                if (!forceLoadFromDisk)
                {
                    dwFlags |= SHGetFileInfoConstants.SHGFI_USEFILEATTRIBUTES;
                    dwAttr = FILE_ATTRIBUTE_NORMAL;
                }
        }

        private int getIconIndex(string fileName, bool isDirectory, bool forceLoadFromDisk, ShellIconStateConstants iconState)
        {
            SHGetFileInfoConstants dwFlags; uint dwAttr;
            getAttributes(isDirectory, forceLoadFromDisk, out dwAttr, out dwFlags);

            // sFileSpec can be any file.

            if (fileName.EndsWith(".lnk", StringComparison.InvariantCultureIgnoreCase))
            {
                dwFlags |= SHGetFileInfoConstants.SHGFI_LINKOVERLAY | SHGetFileInfoConstants.SHGFI_ICON;
                iconState = ShellIconStateConstants.ShellIconStateLinkOverlay;
                forceLoadFromDisk = true;
            }

            SHFILEINFO shfi = new SHFILEINFO();
            uint shfiSize = (uint)Marshal.SizeOf(shfi.GetType());
            IntPtr retVal = SHGetFileInfo(fileName, dwAttr, ref shfi, shfiSize, ((uint)(dwFlags) | (uint)iconState));

            if (retVal.Equals(IntPtr.Zero))
            {
                if (forceLoadFromDisk)
                    return getIconIndex(Path.GetFileName(fileName), isDirectory, false, iconState);
                else
                {
                    System.Diagnostics.Debug.Assert((!retVal.Equals(IntPtr.Zero)), "Failed to get icon index");
                    return -1;
                }
            }
            else return shfi.iIcon;
        }
        private int getIconIndex(IntPtr pidlPtr, bool isDirectory, bool forceLoadFromDisk, ShellIconStateConstants iconState)
        {
            SHGetFileInfoConstants dwFlags; uint dwAttr;
            getAttributes(isDirectory, forceLoadFromDisk, out dwAttr, out dwFlags);
            dwFlags |= SHGetFileInfoConstants.SHGFI_PIDL;

            SHFILEINFO shfi = new SHFILEINFO();
            uint shfiSize = (uint)Marshal.SizeOf(shfi.GetType());
            IntPtr retVal = SHGetFileInfo(pidlPtr, dwAttr, ref shfi, shfiSize, ((uint)(dwFlags) | (uint)iconState));

            if (retVal.Equals(IntPtr.Zero))
            {
                System.Diagnostics.Debug.Assert((!retVal.Equals(IntPtr.Zero)), "Failed to get icon index");
                return -1;
            }
            else return shfi.iIcon;
        }
        private Icon getIcon(int index)
        {
            if (this == null) return null;
            if (index == -1) return null;

            Icon icon = null;

            IntPtr hIcon = IntPtr.Zero;

            //if (_iImageList == null || Thread.CurrentThread.GetApartmentState() == ApartmentState.MTA)
            //{
                hIcon = ImageList_GetIcon(_ptrImageList, index,
                    (int)(ImageListDrawItemConstants.ILD_TRANSPARENT | ImageListDrawItemConstants.ILD_SCALE));
            //}
            //else
            //{
            //InvalidCastException if run through this.
            //    _iImageList.GetIcon(index, (int)ImageListDrawItemConstants.ILD_TRANSPARENT, ref hIcon);
            //}

            if (hIcon != IntPtr.Zero)
            {
                icon = System.Drawing.Icon.FromHandle(hIcon);
            }
            return icon != null ? icon.Clone() as Icon : null;

        }

        //static int CLR_NONE    = (int)0xffffffff;
        //static int CLR_INVALID = CLR_NONE;
        //static int CLR_DEFAULT = (int)0xff000000;


        private Bitmap getBitmap(int index, ImageListDrawItemConstants flags)
        {
            Size bitmapSize = GetImageListIconSize();
            Bitmap bitmap = new Bitmap(bitmapSize.Width, bitmapSize.Height);

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                try
                {
                    g.FillRectangle(Brushes.White, new Rectangle(0, 0, bitmapSize.Width, bitmapSize.Height));

                    IntPtr hdc = g.GetHdc();

                    IMAGELISTDRAWPARAMS pimldp = new IMAGELISTDRAWPARAMS();
                    pimldp.hdcDst = hdc;
                    pimldp.cbSize = Marshal.SizeOf(pimldp.GetType());
                    pimldp.i = index;
                    pimldp.x = 0;
                    pimldp.y = 0;
                    pimldp.cx = bitmapSize.Width;
                    pimldp.cy = bitmapSize.Height;
                    //pimldp.rgbBk = Color.Silver.ToArgb();
                    //pimldp.rgbFg = Color.Silver.ToArgb();
                    //pimldp.crEffect = Color.White.ToArgb();
                    //pimldp.Frame = 255;
                    //pimldp.fState = 0x00000008;                    
                    //pimldp.dwRop = (int)(dwRop.BLACKNESS);
                    pimldp.fStyle = (int)flags;

                    if (_iImageList == null || Thread.CurrentThread.GetApartmentState() == ApartmentState.MTA)
                    {
                        int ret = ImageList_DrawIndirect(ref pimldp);
                    }
                    else
                    {
                        _iImageList.Draw(ref pimldp);
                    }
                }
                finally
                {
                    g.ReleaseHdc();
                }


            }

            bitmap.MakeTransparent();
            return bitmap;
        }


        private Bitmap getBitmap(int index)
        {
            //Bitmap mask = getBitmap(index, ImageListDrawItemConstants.ILD_MASK);
            Bitmap normal = getBitmap(index, ImageListDrawItemConstants.ILD_TRANSPARENT
                | ImageListDrawItemConstants.ILD_IMAGE | ImageListDrawItemConstants.ILD_SCALE);


            //Size bitmapSize = GetImageListIconSize();
            //Bitmap bitmap = new Bitmap(bitmapSize.Width, bitmapSize.Height);


            return normal;
            //string output;
            //return MaskImagePtr(normal, mask, out output);
        }



        private bool isXpOrAbove()
        {
            bool ret = false;
            if (Environment.OSVersion.Version.Major > 5)
            {
                ret = true;
            }
            else if ((Environment.OSVersion.Version.Major == 5) &&
                (Environment.OSVersion.Version.Minor >= 1))
            {
                ret = true;
            }
            return ret;
            //return false;
        }

        private static bool isVistaUp()
        {
            return (Environment.OSVersion.Version.Major >= 6);
        }

        private Size GetImageIconSize(int index)
        {
            IMAGEINFO imgInfo = new IMAGEINFO();
            int hr = 0;

            if (_iImageList == null || Thread.CurrentThread.GetApartmentState() == ApartmentState.MTA)
                hr = ImageList_GetImageInfo(_ptrImageList, index, ref imgInfo);
            else hr = _iImageList.GetImageInfo(index, ref imgInfo);

            if (hr != 0)
                Marshal.ThrowExceptionForHR(hr);

            RECT rect = imgInfo.rcImage;
            return new Size(rect.right - rect.left, rect.bottom - rect.top);
        }



        private Size GetImageListIconSize()
        {
            int cx = 0, cy = 0;
            if (_iImageList == null || Thread.CurrentThread.GetApartmentState() == ApartmentState.MTA)
                ImageList_GetIconSize(_ptrImageList, ref cx, ref cy);
            else _iImageList.GetIconSize(ref cx, ref cy);
            return new Size(cx, cy);
        }

        #endregion

        #region Data

        private IntPtr _ptrImageList = IntPtr.Zero;
        private IImageList _iImageList = null;
        private bool _disposed = false;
        private IconSize _size;
        #endregion



    }
}
