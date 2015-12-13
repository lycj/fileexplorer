using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.UIEventHub.Defines
{

    public enum UIInputType { None, Mouse, MouseLeft, MouseRight, Touch, Stylus }
    public enum UIInputState { NotApplied, Pressed, Hold, Released }
    public enum UITouchGesture { NotApplied, Drag, FlickLeft, FlickRight, FlickUp, FlickDown }

    public static class ShellClipboardFormats
    {
        public static string CFSTR_AUTOPLAY_SHELLIDLISTS = "Autoplay Enumerated IDList Array";
        public static string CFSTR_DRAGCONTEXT = "DragContext";
        public static string CFSTR_FILECONTENTS = "FileContents";
        public static string CFSTR_FILEDESCRIPTORA = "FileGroupDescriptor";
        public static string CFSTR_FILEDESCRIPTORW = "FileGroupDescriptorW";
        public static string CFSTR_FILENAMEA = "FileName";
        public static string CFSTR_FILENAMEMAPA = "FileNameMap";
        public static string CFSTR_FILENAMEMAPW = "FileNameMapW";
        public static string CFSTR_FILENAMEW = "FileNameW";
        public static string CFSTR_INDRAGLOOP = "InShellDragLoop";
        public static string CFSTR_INETURLA = "UniformResourceLocator";
        public static string CFSTR_INETURLW = "UniformResourceLocatorW";
        public static string CFSTR_LOGICALPERFORMEDDROPEFFECT = "Logical Performed DropEffect";
        public static string CFSTR_MOUNTEDVOLUME = "MountedVolume";
        public static string CFSTR_NETRESOURCES = "Net Resource";
        public static string CFSTR_PASTESUCCEEDED = "Paste Succeeded";
        public static string CFSTR_PERFORMEDDROPEFFECT = "Performed DropEffect";
        public static string CFSTR_PERSISTEDDATAOBJECT = "PersistedDataObject";
        public static string CFSTR_PREFERREDDROPEFFECT = "Preferred DropEffect";
        public static string CFSTR_PRINTERGROUP = "PrinterFreindlyName";
        public static string CFSTR_SHELLIDLIST = "Shell IDList Array";
        public static string CFSTR_SHELLIDLISTOFFSET = "Shell Object Offsets";
        public static string CFSTR_SHELLURL = "UniformResourceLocator";
        public static string CFSTR_TARGETCLSID = "TargetCLSID";
    }

    
}
