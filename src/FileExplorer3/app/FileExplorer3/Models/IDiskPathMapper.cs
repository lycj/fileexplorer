using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Models
{
    public class DiskMapInfo
    {
        public bool IsCached { get; private set; }
        public bool IsVirtual { get; private set; }
        public string IOPath { get; private set; }
        public Uri SourceUrl { get; set; }

        public DiskMapInfo(string ioPath, bool isCached, bool isVirtual)
        {
            IOPath = ioPath;
            IsCached = isCached;
            IsVirtual = isVirtual;
        }
    }

    /// <summary>
    /// Map EntryModel to Local disk path
    /// </summary>
    public interface IDiskPathMapper
    {
        DiskMapInfo this[IEntryModel model] { get; }
     
    }

   
}
