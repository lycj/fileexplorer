using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.IO.Defines
{
    public static class FileExtensions
    {
        public static string ExifExtensions = ".jpeg,.jpg";
        public static string ImageExtensions = ".gif,.bmp,.png," + ExifExtensions;
    }
}
